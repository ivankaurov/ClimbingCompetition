using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Threading;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace ClimbingCompetition
{
    [Serializable]
    public sealed class TimerSynchroArgs
    {
        readonly TimeSpan remaining, interval;
        readonly Boolean isRunning;

        public Boolean IsRunning
        {
            get { return isRunning; }
        } 


        public TimeSpan Interval
        {
            get { return interval; }
        } 


        public TimeSpan Remaining
        {
            get { return remaining; }
        }

        public TimerSynchroArgs(TimeSpan remaining, TimeSpan interval, Boolean isRunning)
        {
            this.remaining = remaining;
            this.interval = interval;
            this.isRunning = isRunning;
        }
    }

    public sealed class ClimbingTimer : IDisposable
    {
        private const double SIGNAL_INTERVAL = 1.0;
        private const double INTERNAL_TIMER_INTERVAL = 10.0;
        private const string MACHINE_PARAM_ID = "@MachineParam";

        private static string MachineParam = System.Net.Dns.GetHostName() + "_" + Process.GetCurrentProcess().Id.ToString();

        private bool objectDisposed = false;
        private readonly System.Timers.Timer timer;
        private readonly Stopwatch sw;
        private DateTime lastSignal;
        private readonly bool isMain;
        readonly List<TcpListener> listeners = new List<TcpListener>();

        private bool firstSynchroCompleted = false;

        readonly SqlConnection cn;
        String ConnectionString { get { return cn == null ? null : cn.ConnectionString; } }

        private TimeSpan remainingInterval, rotationInterval;

        public TimeSpan RotationInterval
        {
            get { return rotationInterval; }
            set
            {
                Monitor.Enter(timer);
                try
                {
                    rotationInterval = value;
                }
                finally { Monitor.Exit(value); }
            }
        }

        public int Minutes { get { return RotationInterval.Minutes; } }

        private ClimbingTimer(string connectionString, bool autoReset = true, bool autoSynchro = true)
        {
            sw = new Stopwatch();
            timer = new System.Timers.Timer();
            timer.Interval = INTERNAL_TIMER_INTERVAL;
            timer.AutoReset = true;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            this.autoReset = autoReset;
            if (String.IsNullOrEmpty(connectionString))
                this.cn = null;
            else
            {
                this.cn = new SqlConnection(connectionString);
                this.cn.Open();
                CreateTables();
            }
            this.autoSynchro = autoSynchro;
        }

        public const String SYNCHRO_REQUEST = "SYNCHRO_REQUEST";
        public const String SYNCRHO_CHECK = "SYNCHRO_CHECK";

        void CreateTables()
        {
            if (cn == null)
                return;
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = cn;
                cmd.CommandText = "SELECT count(*) from sys.tables(nolock) where name = 'timer_data'";
                var result = Convert.ToInt32(cmd.ExecuteScalar());
                if (result < 1)
                {
                    cmd.CommandText = "CREATE TABLE timer_data (" +
                                     "  ip_address varchar(16) primary key," +
                                     "  port       int)";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        void OpenListener()
        {
            if (this.cn == null || !this.isMain)
                return;
            foreach (var listener in listeners)
                listener.Stop();
            listeners.Clear();
            List<IPEndPoint> localEndPoints = new List<IPEndPoint>();
            Random rnd = new Random(DateTime.Now.Second);
            foreach (var adr in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (adr.AddressFamily != AddressFamily.InterNetwork)
                    continue;
                IPEndPoint currentEndPoint = null;
                for (int i = 0; i < 100; i++)
                {
                    currentEndPoint = new IPEndPoint(adr, rnd.Next(10000, 25000));
                    TcpListener listener = null;
                    try
                    {
                        listener = new TcpListener(currentEndPoint);
                        listener.Start();
                        listener.BeginAcceptTcpClient(AcceptSocket, listener);
                        listeners.Add(listener);
                        break;
                    }
                    catch (SocketException)
                    {
                        currentEndPoint = null;
                        if (listener != null)
                            listener.Stop();
                        continue;
                    }
                }
                if (currentEndPoint != null)
                    localEndPoints.Add(currentEndPoint);
            }
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = cn;
                cmd.CommandText = "if exists(select 1" +
                                  "            from timer_data" +
                                  "           where ip_address = @ip_address)" +
                                  "  update timer_data set port = @port " +
                                  "else" +
                                  "  insert into timer_data(ip_address, port) values (@ip_address, @port)";
                var ipAddressParam = cmd.Parameters.Add("@ip_address", SqlDbType.VarChar, 16);
                var portParam = cmd.Parameters.Add("@port", SqlDbType.Int);
                cmd.Transaction = cn.BeginTransaction();
                try
                {
                    foreach (var ipEP in localEndPoints)
                    {
                        ipAddressParam.Value = ipEP.Address.ToString();
                        portParam.Value = ipEP.Port;
                        cmd.ExecuteNonQuery();
                    }
                    cmd.Transaction.Commit();
                }
                catch
                {
                    cmd.Transaction.Rollback();
                    throw;
                }
            }
        }

        void AcceptSocket(IAsyncResult res)
        {
            var tcpListener = res.AsyncState as TcpListener;
            if (tcpListener == null)
                return;
            TcpClient client;
            try
            {
                client = tcpListener.EndAcceptTcpClient(res);
                tcpListener.BeginAcceptTcpClient(AcceptSocket, tcpListener);
            }
            catch (ObjectDisposedException) { return; }
            try
            {
                var fmt = new BinaryFormatter();
                using (var stream = client.GetStream())
                {
                    var rq = fmt.Deserialize(stream) as String;
                    if (String.Equals(rq, SYNCHRO_REQUEST, StringComparison.OrdinalIgnoreCase))
                        fmt.Serialize(stream, new TimerSynchroArgs(remainingInterval, rotationInterval, timer != null && timer.Enabled));
                    else if (String.Equals(rq, SYNCRHO_CHECK, StringComparison.OrdinalIgnoreCase))
                        fmt.Serialize(stream, SYNCHRO_REQUEST);
                }
            }
            finally { client.Close(); }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (objectDisposed)
                return;
            if (!Monitor.TryEnter(timer, 10))
                return;
            try
            {
                if (objectDisposed)
                    return;
                var tsCurrent = e.SignalTime - lastSignal;
                if (tsCurrent.TotalSeconds < SIGNAL_INTERVAL)
                    return;
                remainingInterval -= tsCurrent;

                bool stop;
                if (remainingInterval <= TimeSpan.Zero)
                {
                    remainingInterval = rotationInterval;
                    stop = !autoReset;
                }
                else
                    stop = false;
                bool b = !stop;
                if (!this.isMain)
                    b = b && (this.autoSynchro || !this.firstSynchroCompleted);
                b = b && remainingInterval.Seconds % 10 == 0;
                b = b && (prqThread == null || !prqThread.IsAlive);
                b = b && !String.IsNullOrEmpty(ConnectionString);
                if (b)
                {
                    prqThread = new Thread(ProcessRequests);
                    prqThread.Start(new ProcessRequestArgs(ConnectionString, isMain, timer, this));
                }
                lastSignal = e.SignalTime;
                if (timerEvent != null)
                    try { timerEvent(this, new ClimbingTimerEventArgs(remainingInterval)); }
                    catch { }
                if (stop)
                {
                    timer.Stop();
                    if (timerStopped != null)
                        timerStopped(this, new EventArgs());
                }
            }
            finally { Monitor.Exit(timer); }
        }

        public ClimbingTimer(string connectionString, TimeSpan rotationInterval, bool isMain = false, bool autoReset = true, bool autoSynchro = true)
            : this(connectionString, autoReset, autoSynchro)
        {
            this.rotationInterval = rotationInterval;
            this.remainingInterval = this.rotationInterval;
            this.isMain = isMain;
            if (cn != null)
                AccountForm.CreateQueryData(cn);
            if (isMain)
                OpenListener();


        }

        public ClimbingTimer(string connectionString, int rotationMinute, int rotationSecond = 0, bool isMain = false, bool autoReset = true, bool autoSynchro = true) :
            this(connectionString, new TimeSpan(0, rotationMinute, rotationSecond), isMain, autoReset, autoSynchro) { }

        private void ThrowDisposedException()
        {
            throw new ObjectDisposedException("ClimbingTimer");
        }
        
        readonly List<IPEndPoint> endPoints = new List<IPEndPoint>();
        readonly object endPointsLocker = new object();

        void AddEndPoint(IPEndPoint endPoint)
        {
            lock (endPointsLocker) { endPoints.Add(endPoint); }
        }

        void CheckEndPoint(IPEndPoint endPoint)
        {
            TcpClient client = new TcpClient();
            try
            {
                client.BeginConnect(endPoint.Address, endPoint.Port, res =>
                {
                    var tcpClient = res.AsyncState as TcpClient;
                    if (tcpClient == null)
                        return;
                    try { tcpClient.EndConnect(res); }
                    catch (ObjectDisposedException) { return; }
                    catch (SocketException) { return; }

                    try
                    {
                        BinaryFormatter fmt = new BinaryFormatter();
                        using (var stream = tcpClient.GetStream())
                        {
                            fmt.Serialize(stream, SYNCRHO_CHECK);
                            var result = fmt.Deserialize(stream) as String;
                            if (String.Equals(SYNCHRO_REQUEST, result, StringComparison.OrdinalIgnoreCase))
                                AddEndPoint(endPoint);
                        }
                    }
                    catch (IOException) { return; }
                    finally { tcpClient.Close(); }
                }, client);
            }
            catch (SocketException) { client.Close(); }
            catch (IOException) { client.Close(); }
        }

        Boolean SynchronizeByEndPoint(IPEndPoint endPoint)
        {
            using (var client = new TcpClient())
            {
                try
                {
                    client.Connect(endPoint);
                    BinaryFormatter fmt = new BinaryFormatter();
                    using (var stream = client.GetStream())
                    {
                        fmt.Serialize(stream, SYNCHRO_REQUEST);
                        var result = fmt.Deserialize(stream) as TimerSynchroArgs;
                        if (result == null)
                            return false;
                        if (result.IsRunning && !this.IsRunning)
                            this.Start();
                        else if (!result.IsRunning && this.IsRunning)
                            this.Stop();
                        this.remainingInterval = result.Remaining;
                        this.rotationInterval = result.Interval;
                        return true;
                    }
                }
                catch (SocketException) { return false; }
                catch (ObjectDisposedException) { return false; }
                catch (IOException) { return false; }
            }
        }

        public bool IsRunning { get { return timer != null && timer.Enabled; } }

        int synchroInterval = -1;
        void DoSynchronization()
        {
            if (isMain || synchroInterval < 0)
                return;
            System.Threading.Timer tmrTH = new System.Threading.Timer(tmr =>
            {
                if (isMain || synchroInterval < 0)
                {
                    ((System.Threading.Timer)tmr).Dispose();
                    return;
                }
                List<IPEndPoint> currentEndPoints;
                lock (endPointsLocker)
                {
                    currentEndPoints = new List<IPEndPoint>(endPoints);
                }
                foreach (var ep in currentEndPoints)
                    if (SynchronizeByEndPoint(ep))
                        break;
                ((System.Threading.Timer)tmr).Change(synchroInterval * 1000, Timeout.Infinite);
            });
            tmrTH.Change(0, Timeout.Infinite);
        }

        void SetupSynchronizeClient()
        {
            if (this.isMain)
                return;
            endPoints.Clear();
            
            if (cn == null)
                return;
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = cn;
                cmd.CommandText = "SELECT ip_address, port FROM timer_data(nolock)";
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        IPEndPoint endPoint;
                        try { endPoint = new IPEndPoint(IPAddress.Parse(rdr["ip_address"].ToString()), Convert.ToInt32(rdr["port"])); }
                        catch (FormatException) { continue; }
                        CheckEndPoint(endPoint);
                    }
                }
            }
        }
        
        public void Start()
        {
            if (objectDisposed)
                ThrowDisposedException();
            this.lastSignal = DateTime.Now;
            if (timer.Enabled)
                return;
            SetupSynchronizeClient();
            timer.Start();
            if (AutoSynchro && !isMain)
            {
                synchroInterval = 5;
                DoSynchronization();
            }
            else
            {
                synchroInterval = -1;
            }
        }
        public void Reset()
        {
            if(objectDisposed)
                ThrowDisposedException();
            Monitor.Enter(timer);
            try
            {
                this.remainingInterval = rotationInterval;
                if (timerEvent != null)
                    try { timerEvent(this, new ClimbingTimerEventArgs(this.remainingInterval)); }
                    catch { }
            }
            finally { Monitor.Exit(timer); }
        }
        public void Stop()
        {
            if (objectDisposed)
                ThrowDisposedException();
            if (!timer.Enabled)
                return;
            timer.Stop();
            if (autoSynchro && !isMain)
            {
                synchroInterval = 1;
                DoSynchronization();
            }
            else
                synchroInterval = -1;
        }

        private bool autoReset;
        private bool autoSynchro;

        /// <summary>
        /// True если таймер надо перезапускать
        /// </summary>
        public bool AutoReset
        {
            get
            {
                if (objectDisposed)
                    ThrowDisposedException();
                return autoReset;
            }
            set
            {
                if (objectDisposed)
                    ThrowDisposedException();
                Monitor.Enter(timer);
                try { this.autoReset = value; }
                finally { Monitor.Exit(timer); }
            }
        }

        public bool AutoSynchro
        {
            get { return autoSynchro; }
            set
            {
                if (objectDisposed)
                    ThrowDisposedException();
                //Monitor.Enter(timer);
                //try {
                    autoSynchro = value; 
                //}
                //finally { Monitor.Exit(timer); }
            }
        }

        public bool AllowDispose
        {
            get
            {
                if (objectDisposed)
                    return true;
                return (timerEvent == null);
            }
        }

        public bool IsMain
        {
            get
            {
                if (objectDisposed)
                    ThrowDisposedException();
                return isMain;
            }
        }

        private Thread prqThread = null;

        private static SqlCommand AddMachineID(SqlCommand cmd)
        {
            cmd.Parameters.Add(MACHINE_PARAM_ID, SqlDbType.VarChar, 255).Value = MachineParam;
            return cmd;
        }

        private const byte TimerDataFlags = (byte)(ClimbingTimerCommand.Timer_SynchroCompleted | ClimbingTimerCommand.Timer_SynchroRequest);
        private static bool ContainsTimerData(ClimbingTimerCommand c) { return ContainsTimerData((byte)c); }
        private static bool ContainsTimerData(byte b)
        {
            return (b | TimerDataFlags) == TimerDataFlags;
        }

        private Mutex processRequestsMutex = new Mutex();
        private void ProcessRequests(object aRgUmEnT)
        {
            ProcessRequestArgs arg = aRgUmEnT as ProcessRequestArgs;
            if (arg == null)
                return;
            if (!processRequestsMutex.WaitOne(3000))
                return;
            try
            {
                //Обработаем все запросы, связанные с таймером
                using (SqlConnection cn = new SqlConnection(arg.ConnectionString))
                {
                    cn.Open();
                    List<Pair<ClimbingTimerCommand, string>> listParams = new List<Pair<ClimbingTimerCommand, string>>();
                    SqlTransaction tran = cn.BeginTransaction();
                    try
                    {
                        SqlCommand cmd = new SqlCommand("SELECT REQUEST, PARAM, sys_date, GetDate() curDate" +
                                                        "  FROM prg_Qs(NOLOCK)" +
                                                        " WHERE MACHINE <> " + MACHINE_PARAM_ID, cn);
                        AddMachineID(cmd);
                        cmd.Transaction = tran;
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                try
                                {
                                    ClimbingTimerCommand c = StaticClass.GetEnumValueFromStringNumeric<ClimbingTimerCommand>(rdr[0] as string);
                                    if (ContainsTimerData(c))
                                        listParams.Add(new Pair<ClimbingTimerCommand, string>(c, rdr[1] as string, (DateTime)rdr[2], (DateTime)rdr[3]));
                                }
                                catch (FormatException) { }
                            }
                        }
                        bool dataInserted = false;
                        foreach (var v in listParams)
                        {
                            #region ForEachParam
                            //Если мы запрашивали синхронизацию, то обновим значение
                            bool synchroNeeded = endPoints.Count < 1 && !arg.IsMain && (arg.Parent.autoSynchro || !arg.Parent.firstSynchroCompleted);
                            if (synchroNeeded && ((v.Value1 & ClimbingTimerCommand.Timer_SynchroCompleted) == ClimbingTimerCommand.Timer_SynchroCompleted))
                            {
                                int[] vals = new int[4];
                                string[] s = v.Value2.Split(new char[] { ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);
                                if (s.Length < 4)
                                    continue;
                                try
                                {
                                    for (int i = 0; i < vals.Length; i++)
                                        vals[i] = int.Parse(s[i]);
                                }
                                catch (FormatException) { continue; }
                                /*обновим остаток*/
                                if (Monitor.TryEnter(arg.Locker, 2000))
                                {
                                    try
                                    {
                                        TimeSpan oldBase = new TimeSpan(0, vals[0], vals[1]);
                                        oldBase -= (v.DateSelected - v.DateUpdated);
                                        arg.Parent.remainingInterval = oldBase;
                                        arg.Parent.rotationInterval = new TimeSpan(0, vals[2], vals[3]);
                                        arg.Parent.lastSignal = v.DateSelected;
                                        arg.Parent.firstSynchroCompleted = true;
                                    }
                                    finally { Monitor.Exit(arg.Locker); }
                                    arg.Parent.Start();
                                }
                            }
                            if (arg.IsMain && ((v.Value1 & ClimbingTimerCommand.Timer_SynchroRequest) == ClimbingTimerCommand.Timer_SynchroRequest))
                            {
                                //Если кто-то запрашивает синхронизацию
                                //Ответ поставим только один раз
                                if (!dataInserted)
                                {
                                    cmd.Parameters.Clear();
                                    cmd.CommandText = "DELETE FROM prg_Qs WHERE REQUEST IN(@sr, @sc)";
                                    cmd.Parameters.Add("@sr", SqlDbType.VarChar, 255).Value =
                                        StaticClass.GetEnumStringNumericValue(ClimbingTimerCommand.Timer_SynchroRequest);
                                    cmd.Parameters.Add("@sc", SqlDbType.VarChar, 255).Value =
                                        StaticClass.GetEnumStringNumericValue(ClimbingTimerCommand.Timer_SynchroCompleted);
                                    cmd.ExecuteNonQuery();
                                    cmd.CommandText = "INSERT INTO prg_Qs (REQUEST, PARAM, MACHINE) VALUES (@sc,@p," + MACHINE_PARAM_ID + ")";
                                    AddMachineID(cmd);
                                    if (Monitor.TryEnter(arg.Locker, 5000))
                                    {
                                        try
                                        {
                                            cmd.Parameters.Add("@p", SqlDbType.VarChar, 255).Value =
                                                Math.Truncate(arg.Parent.remainingInterval.TotalMinutes).ToString("0") + ";" +
                                                arg.Parent.remainingInterval.Seconds.ToString() + ";" +
                                                Math.Truncate(arg.Parent.rotationInterval.TotalMinutes).ToString("0") + ";" +
                                                arg.Parent.rotationInterval.Seconds.ToString();
                                            cmd.ExecuteNonQuery();
                                            dataInserted = true;
                                        }
                                        finally { Monitor.Exit(arg.Locker); }
                                    }

                                }
                            }
                            #endregion
                        }
                        if (!arg.IsMain && endPoints.Count < 1)
                        {
                            if (Monitor.TryEnter(arg.Locker, 1000))
                                try
                                {
                                    cmd.Parameters.Clear();
                                    cmd.CommandText = "INSERT INTO prg_Qs (REQUEST, MACHINE, PARAM) VALUES(@sr," + MACHINE_PARAM_ID + ", '-')";
                                    cmd.Parameters.Add("@sr", SqlDbType.VarChar, 255).Value =
                                        StaticClass.GetEnumStringNumericValue(ClimbingTimerCommand.Timer_SynchroRequest);
                                    AddMachineID(cmd);
                                    cmd.ExecuteNonQuery();
                                }
                                finally { Monitor.Exit(arg.Locker); }
                        }
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        try { tran.Rollback(); }
                        catch { }
                        throw ex;
                    }

                }
            }
            catch (ThreadAbortException) { return; }
            catch { }
            finally { processRequestsMutex.ReleaseMutex(); }
        }

        sealed class Pair<T1, T2>
        {
            public T1 Value1 { get; private set; }
            public T2 Value2 { get; private set; }
            public DateTime DateUpdated { get; private set; }
            public DateTime DateSelected { get; private set; }
            public Pair(T1 value1, T2 value2, DateTime dateUpdated, DateTime dateSelected)
            {
                this.Value1 = value1;
                this.Value2 = value2;
                this.DateUpdated = dateUpdated;
                this.DateSelected = dateSelected;
            }
        }

        sealed class ProcessRequestArgs
        {
            public string ConnectionString { get; private set; }
            public bool IsMain { get; private set; }
            public object Locker { get; private set; }
            public ClimbingTimer Parent{get;private set;}
            public ProcessRequestArgs(string connectionString, bool isMain, object locker, ClimbingTimer parent)
            {
                this.ConnectionString = connectionString;
                this.IsMain = isMain;
                this.Locker = locker;
                this.Parent = parent; 
            }
        }

        public event EventHandler<ClimbingTimerEventArgs> timerEvent;

        public event EventHandler timerStopped;

        void StopListening()
        {
            foreach (var listener in this.listeners)
                listener.Stop();
            if (cn != null)
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = "DELETE from timer_data where ip_address = @ip_address";
                    var p = cmd.Parameters.Add("@ip_address", SqlDbType.VarChar, 16);
                    cmd.Transaction = cn.BeginTransaction();
                    try
                    {
                        foreach (var lst in listeners)
                        {
                            p.Value = ((IPEndPoint)lst.LocalEndpoint).Address.ToString();
                            cmd.ExecuteNonQuery();
                        }

                        cmd.Transaction.Commit();
                    }
                    catch
                    {
                        cmd.Transaction.Rollback();
                        throw;
                    }
                }
            listeners.Clear();
        }

        #region IDisposable implementation
        private void Dispose(bool disposing)
        {
            synchroInterval = -1;
            objectDisposed = true;
            if (prqThread != null && prqThread.IsAlive)
                if (!prqThread.Join(3000))
                    prqThread.Abort();
            if (disposing)
            {
                if (timer != null)
                {
                    if (timer.Enabled)
                        timer.Stop();
                }
                StopListening();
                if (cn != null)
                    cn.Close();
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }

        ~ClimbingTimer()
        {
            Dispose(false);
        }
        #endregion
    }

    public enum ClimbingTimerCommand : byte
    {
        Timer_SynchroRequest = 0x01,
        Timer_SynchroCompleted = 0x02
    }

    public sealed class ClimbingTimerEventArgs : EventArgs
    {
        private readonly TimeSpan remaining;

        public int Minute { get { return (int)Math.Truncate(remaining.TotalMinutes); } }
        public int Second { get { return remaining.Seconds; } }
        public ClimbingTimerEventArgs(TimeSpan remaining)
        {
            this.remaining = remaining;
        }
    }
}
