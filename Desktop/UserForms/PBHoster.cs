using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using System.Threading;

namespace ClimbingCompetition
{
    public class PBHoster
    {
#if FULL
        public const string CommandStart = "COMMAND";
        public const string CommandEnd = ":ENDCMD;";
        public const byte CommandPr = 0xFE;
        public const byte CommandTail = 0xEF;
        public const int TIMEOUT = 5000;

        private TcpListener listener;
        private bool working = false;
        public PBHoster(int port)
        {
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            listener.Start();
            working = true;
            listener.BeginAcceptSocket(EndAcceptSocket, null);
        }

        public void Stop()
        {
            working = false;
            try { listener.Stop(); }
            catch { }
        }

        private class args
        {
            public Socket s;
            public byte[] buffer;
            public List<byte> data;
            public int cnt;
            private System.Timers.Timer t;
            public System.Timers.Timer T
            {
                get { return t; }
                set
                {
                    t = value;
                    t.Elapsed += t_Elapsed;
                }
            }
            private void t_Elapsed(object sender, ElapsedEventArgs e)
            {
                if (timerElapsed != null)
                    timerElapsed(this);
            }
            public delegate void timerElapsedHandler(args sender);
            public event timerElapsedHandler timerElapsed;
        }

        private void EndAcceptSocket(IAsyncResult res)
        {
            if (!working)
                return;
            try
            {
                args a = new args();
                a.s = listener.EndAcceptSocket(res);
                a.buffer = new byte[byte.MaxValue];
                a.cnt = 0;
                a.data = new List<byte>();
                a.T = new System.Timers.Timer(TIMEOUT);
                a.T.AutoReset = false;
                a.timerElapsed += new args.timerElapsedHandler(a_timerElapsed);
                a.s.BeginReceive(a.buffer, 0, a.buffer.Length, SocketFlags.None,
                    EndReceive, a);
            }
            finally { listener.BeginAcceptSocket(EndAcceptSocket, null); }
        }

        void a_timerElapsed(PBHoster.args sender)
        {
            if (sender != null)
                sendError(sender);
        }

        private void EndReceive(IAsyncResult res)
        {
            if (!working)
                return;
            args a = (args)res.AsyncState;
            int k = a.s.EndReceive(res);
            a.cnt += k;
            for (int i = 0; i < k && i < a.buffer.Length; i++)
                a.data.Add(a.buffer[i]);
            byte[] data = CheckBuffer(a.data);
            if (data == null)
                a.s.BeginReceive(a.buffer, 0, a.buffer.Length, SocketFlags.None,
                    EndReceive, a);
            else
                ProcessData(data, a);

        }

        private static byte[] CheckBuffer(List<byte> buffer)
        {
            int begI = buffer.IndexOf(CommandPr);
            if (begI < 0)
                return null;
            int endI = buffer.IndexOf(CommandTail, begI);
            if (endI <= (begI + 1))
                return null;
            byte[] res = new byte[endI - begI - 1];
            for (int i = 0; i < res.Length; i++)
                res[i] = buffer[begI + 1 + i];
            return res;
        }

        private void ProcessData(byte[] data, args a)
        {
            string msg = Encoding.UTF8.GetString(data);
            PBCommandType cmd;
            string strTmp = PBClimber.getParam(msg, CommandStart);
            try { cmd = (PBCommandType)Enum.Parse(typeof(PBCommandType), strTmp); }
            catch
            {
                sendError(a);
                return;
            }
            PBHosterEventArgs e = null;
            ManualResetEvent ev = new ManualResetEvent(false);
            int route, iid;
            switch (cmd)
            {
                case PBCommandType.GETLIST:
                    e = new PBHosterEventArgs(cmd, null, 0, ev);
                    break;
                case PBCommandType.GETNEXT:
                    if (int.TryParse(PBClimber.getParam(msg, "ROUTE"), out route))
                        e = new PBHosterEventArgs(cmd, null, route, ev);
                    break;
                case PBCommandType.GETBYIID:
                    if (int.TryParse(PBClimber.getParam(msg, "ROUTE"), out route))
                    {
                        if (int.TryParse(PBClimber.getParam(msg, "IID"), out iid))
                            e = new PBHosterEventArgs(cmd,
                                new PBClimber(iid, "", "", 0), route, ev);
                    }
                    break;
                case PBCommandType.GETBYSTART:
                    if (int.TryParse(PBClimber.getParam(msg, "ROUTE"), out route))
                    {
                        if (int.TryParse(PBClimber.getParam(msg, "START"), out iid))
                            e = new PBHosterEventArgs(cmd,
                                new PBClimber(0, "", "", iid), route, ev);
                    }
                    break;
                case PBCommandType.SETRES:
                    if (int.TryParse(PBClimber.getParam(msg, "ROUTE"), out route))
                    {
                        PBClimber clm = PBClimber.GetFromString(msg);
                        if (clm != null && !clm.Empty)
                            e = new PBHosterEventArgs(cmd, clm, route, ev);
                    }
                    break;
            }
            bool toSetErr = true;
            if(e != null)
                if (PBHosterEvent != null)
                {
                    PBHosterEvent(this, e);
                    toSetErr = false;
                    ev.WaitOne();
                }
            if (toSetErr)
                sendError(a);
            else
            {
                string pck = CommandStart + "=";
                switch (cmd)
                {
                    case PBCommandType.GETLIST:
                        pck += PBCommandType.LIST.ToString() + ";NAME=" + opResStr + ";";
                        break;
                    case PBCommandType.SETRES:
                        if (opRes)
                            pck += PBCommandType.ACK.ToString() + ";";
                        else
                            pck += PBCommandType.DECL.ToString() + ";";
                        break;
                    default:
                        pck += clm.ToString();
                        break;
                }
                pck += CommandEnd;
                List<byte> pc = new List<byte>();
                pc.Add(CommandPr);
                foreach (byte b in Encoding.UTF8.GetBytes(pck))
                    pc.Add(b);
                pc.Add(CommandTail);
                a.buffer = pc.ToArray();
                a.cnt = 0;
                a.s.BeginSend(a.buffer, 0, a.buffer.Length, SocketFlags.None,
                    EndSend, a);
            }
        }

        private void EndSend(IAsyncResult res)
        {
            if (!working)
                return;
            args a = (args)res.AsyncState;
            try
            {
                
                int k = a.s.EndSend(res);
                a.cnt += k;
                if (a.cnt < a.buffer.Length)
                {
                    a.s.BeginSend(a.buffer, a.cnt, a.buffer.Length - a.cnt,
                         SocketFlags.None, EndSend, a);
                    return;
                }
            }
            catch { }
            a.buffer = new byte[byte.MaxValue];
            a.cnt = 0;
            a.data.Clear();
            a.s.BeginReceive(a.buffer, 0, a.buffer.Length, SocketFlags.None,
                EndReceive, a);
        }

        private void sendError(args a)
        {
            string msg = CommandStart + "=ERR;" + CommandEnd;
            List<byte> pck = new List<byte>();
            pck.Add(CommandPr);
            foreach (byte b in Encoding.UTF8.GetBytes(msg))
                pck.Add(b);
            pck.Add(CommandTail);
            a.cnt = 0;
            a.data.Clear();
            a.buffer = pck.ToArray();
            try
            {
                if (working)
                    a.s.BeginSend(a.buffer, 0, a.buffer.Length, SocketFlags.None,
                        EndSend, a);
            }
            catch { }
        }

        private PBClimber clm = null;
        private bool opRes = false;
        private string opResStr = "";

        public void SetClimber(PBClimber clm, PBHosterEventArgs e)
        {
            if (clm == null)
                this.clm = PBClimber.GetEmpty();
            else
                this.clm = clm;
            e.Event.Set();
        }

        public void SetString(string str, PBHosterEventArgs e)
        {
            this.opResStr = str;
            e.Event.Set();
        }

        public void SetBoolean(bool b, PBHosterEventArgs e)
        {
            this.opRes = b;
            e.Event.Set();
        }

        public delegate void PBHosterEventHandler(object sender, PBHosterEventArgs e);

        public event PBHosterEventHandler PBHosterEvent;

        public class PBHosterEventArgs
        {
            private PBCommandType cmd;
            public PBCommandType Cmd { get { return cmd; } }
            private PBClimber clm;
            public PBClimber Climber { get { return clm; } }
            private int route;
            public int Route { get { return route; } }
            private ManualResetEvent ev;
            public ManualResetEvent Event { get { return ev; } }
            public PBHosterEventArgs(PBCommandType cmd, PBClimber clm, int route, ManualResetEvent ev)
            {
                this.cmd = cmd;
                this.clm = clm;
                this.route = route;
                this.ev = ev;
            }
        }
#endif
    }
#if FULL
    public enum PBCommandType { GETLIST, GETNEXT, GETBYSTART, GETBYIID, SETRES, LIST, ACK, DECL }
#endif
}
