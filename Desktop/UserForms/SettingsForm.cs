using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ClimbingCompetition
{
    public partial class SettingsForm : Form
    {
        private SqlConnection cn;
        public enum SpeedSystemType { EKB, EKO, IVN, NON }
        
#if FULL
        public enum FlashShowMode { NOT, ADM, BTH }
        public enum OSQfMode { PLC, PTS }
#endif
        public SettingsForm(SqlConnection cn)
        {
            InitializeComponent();

            this.cn = cn;
#if !FULL
            rbSpeedIvanovo.Enabled = false;
            gb2groups.Enabled = gbShowFlash.Enabled = false;
            rbShowFlashNOT.Checked = false;
            cbCombinedNew.Enabled = false;
#endif
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                CurrentSystem = GetCurrentSystem(cn);
                CurrentAutoNum = GetAutoNum(cn);
                CurrentSetLessOne = GetLessOnePoint(cn);
                CurrentOnly75 = GetOnly75(cn);
#if FULL
                CurrentFlashShowMode = GetFlashShowMode(cn);
                CurrentOSQfMode = GetOSQfMode(cn);
                CurrentCombinedNew = GetCombinedNew(cn);
#else
                if (CurrentSystem == SpeedSystemType.IVN)
                {
                    CurrentSystem = SpeedSystemType.EKB;
                    SetCurrentSystem(SpeedSystemType.EKB, cn);
                }
#endif

                CurrentTextLineSpeed = GetTextLineSpeed(cn);
                CurrentListShowRefresh = GetRefreshSpeed(cn);
                CurrentListShowMoveNext = GetMoveNext(cn);

                CurrentSpeedRules = GetSpeedRules(cn);
                CurrentLeaveTrains = GetLeaveTrains(cn);

                //CurrentUseBestQf = GetUseBestQf(cn);
                //CurrentBestSpeed = GetUseBestSpeed(cn);

                CurrentSharePoints = GetSharePoints(cn);

                CurrentOneListExcel = GetOneListExcel(cn);
                CurrentShowNewQf = GetShowNewQf(cn);

                CurrentIncl40sec = GetIncl40sec(cn);
                CurrentLeadTime = GetLeadTime(cn);

                CurrentPosToFalls = GetPosToFalls(cn);
                //CurrentSpeedTwoWins = GetSpeedTwoWins(cn);

                CurrentTopoHoldColor = GetHoldColor(false, cn);
                CurrentTopoSelectedHoldColor = GetHoldColor(true, cn);

                CurrentPreQfClimb = GetPreQfClimbParam(cn);
                CurrentPointsForFalls = GetPointsForFalls(cn);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных:\r\n" + ex.Message);
                this.Close();
            }
        }

        public static SpeedSystemType GetCurrentSystem(SqlConnection cn)
        {
            if (!SortingClass.CheckColumn("CompetitionData", "SpeedType", "VARCHAR(3) NOT NULL DEFAULT '" + SpeedSystemType.EKB.ToString() + "'", cn))
                return SpeedSystemType.EKB;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT SpeedType FROM CompetitionData(NOLOCK)";
            string str = cmd.ExecuteScalar().ToString();
            try
            {
#if FULL
                return (SpeedSystemType)Enum.Parse(typeof(SpeedSystemType), str, true);
#else
                SpeedSystemType st = (SpeedSystemType)Enum.Parse(typeof(SpeedSystemType), str, true);
                if (st == SpeedSystemType.IVN)
                    throw new Exception();
                return st;
#endif
            }
            catch
            {
                cmd.CommandText = "UPDATE CompetitionData SET SpeedType = '" + SpeedSystemType.EKB.ToString() + "'";
                cmd.ExecuteNonQuery();
                return SpeedSystemType.EKB;
            }
        }

        public static void SetCurrentSystem(SpeedSystemType val, SqlConnection cn)
        {
            SortingClass.CheckColumn("CompetitionData", "SpeedType", "VARCHAR(3) NOT NULL DEFAULT '" + SpeedSystemType.EKB.ToString() + "'", cn);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
#if FULL
            cmd.CommandText = "UPDATE CompetitionData SET SpeedType='" + val.ToString() + "'";
#else
            SpeedSystemType sVal = (val == SpeedSystemType.IVN) ? SpeedSystemType.EKB : val;
            cmd.CommandText = "UPDATE CompetitionData SET SpeedType='" + sVal.ToString() + "'";
#endif
            cmd.ExecuteNonQuery();
        }

        public static AutoNum GetAutoNum(SqlConnection cn)
        {
            return GetAutoNum(cn, null);
        }

        public static AutoNum GetAutoNum(SqlConnection cn, SqlTransaction tran)
        {
            if (!SortingClass.CheckColumn("CompetitionData", "AutoNum", "VARCHAR(3) NOT NULL DEFAULT '" + AutoNum.GRP.ToString() + "'", cn, tran))
                return AutoNum.GRP;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            cmd.CommandText = "SELECT AutoNum FROM CompetitionData(NOLOCK)";
            string str = cmd.ExecuteScalar().ToString();
            try { return (AutoNum)Enum.Parse(typeof(AutoNum), str, true); }
            catch
            {
                cmd.CommandText = "UPDATE CompetitionData SET AutoNum = '" + AutoNum.GRP + "'";
                cmd.ExecuteNonQuery();
                return AutoNum.GRP;
            }
        }

        public static void SetAutoNum(AutoNum val, SqlConnection cn)
        {
            SortingClass.CheckColumn("CompetitionData", "AutoNum", "VARCHAR(3) NOT NULL DEFAULT '" + AutoNum.GRP.ToString() + "'", cn);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE CompetitionData SET AutoNum='" + val.ToString() + "'";
            cmd.ExecuteNonQuery();
        }

#if FULL
        public static FlashShowMode GetFlashShowMode(SqlConnection cn) { return GetFlashShowMode(cn, null); }

        public static FlashShowMode GetFlashShowMode(SqlConnection cn, SqlTransaction tran)
        {
            if (!SortingClass.CheckColumn("CompetitionData", "FlashShowMode", "VARCHAR(3) NOT NULL DEFAULT '" + FlashShowMode.NOT.ToString() + "'", cn, tran))
                return FlashShowMode.NOT;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            cmd.CommandText = "SELECT FlashShowMode FROM CompetitionData(NOLOCK)";
            string str = cmd.ExecuteScalar().ToString();
            try { return (FlashShowMode)Enum.Parse(typeof(FlashShowMode), str, true); }
            catch
            {
                cmd.CommandText = "UPDATE CompetitionData SET FlashShowMode = '" + FlashShowMode.NOT.ToString() + "'";
                cmd.ExecuteNonQuery();
                return FlashShowMode.NOT;
            }
        }

        public static void SetFlashShowMode(FlashShowMode val, SqlConnection cn)
        {
            SortingClass.CheckColumn("CompetitionData", "FlashShowMode", "VARCHAR(3) NOT NULL DEFAULT '" + FlashShowMode.NOT.ToString() + "'", cn);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE CompetitionData SET FlashShowMode='" + val.ToString() + "'";
            cmd.ExecuteNonQuery();
        }

        public static OSQfMode GetOSQfMode(SqlConnection cn)
        {
            return GetOSQfMode(cn, null);
        }

        public static OSQfMode GetOSQfMode(SqlConnection cn, SqlTransaction tran)
        {
            if (GetBooleanParam(cn, tran, "UsePtsForQf", true))
                return OSQfMode.PTS;
            else
                return OSQfMode.PLC;
        }

        public static void SetOSQfMode(OSQfMode val, SqlConnection cn)
        {
            SetBooleanParam((val == OSQfMode.PTS), cn, "UsePtsForQf", true);
        }
#endif

        private static bool GetBooleanParam(SqlConnection cn, SqlTransaction tran, string columnName, bool defaultValue)
        {
            string def;
            if (defaultValue)
                def = "1";
            else
                def = "0";
            if (!SortingClass.CheckColumn("CompetitionData", columnName, "BIT NOT NULL DEFAULT " + def, cn, tran))
                return defaultValue;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            cmd.CommandText = "SELECT " + columnName + " FROM CompetitionData(NOLOCK)";
            try { return Convert.ToBoolean(cmd.ExecuteScalar()); }
            catch
            {
                cmd.CommandText = "UPDATE CompetitionData SET " + columnName + " = " + def;
                cmd.ExecuteNonQuery();
                return defaultValue;
            }
        }

        private static void SetBooleanParam(bool val, SqlConnection cn, string columnName, bool defaultValue)
        {
            string def;
            if (defaultValue)
                def = "1";
            else
                def = "0";
            SortingClass.CheckColumn("CompetitionData", columnName, "BIT NOT NULL DEFAULT " + def, cn);
            SqlCommand cmd = new SqlCommand("UPDATE CompetitionData SET " + columnName + " = @s", cn);
            cmd.Parameters.Add("@s", SqlDbType.Bit);
            cmd.Parameters[0].Value = val;
            cmd.ExecuteNonQuery();
        }

        public static bool GetSharePoints(SqlConnection cn)
        {
            return GetSharePoints(cn, null);
        }

        public static bool GetShowNewQf(SqlConnection cn, SqlTransaction tran = null)
        {
            return GetBooleanParam(cn, tran, "ShowNewQf", false);
        }

        public static void SetShowNewQf(bool val, SqlConnection cn)
        {
            SetBooleanParam(val, cn, "ShowNewQf", false);
        }

        public static bool GetPosToFalls(SqlConnection cn, SqlTransaction tran = null)
        {
            return GetBooleanParam(cn, tran, "PosToFalls", true);
        }

        public static void SetPosToFalls(bool val, SqlConnection cn)
        {
            SetBooleanParam(val, cn, "PosToFalls", true);
        }

        public static bool GetCombinedNew(SqlConnection cn, SqlTransaction tran = null)
        {
            return GetBooleanParam(cn, tran, "CombinedNew", true);
        }

        public static void SetCombinedNew(bool val, SqlConnection cn)
        {
            SetBooleanParam(val, cn, "CombinedNew", true);
        }

        public static bool GetLeaveTrains(SqlConnection cn, SqlTransaction tran = null)
        {
            return GetBooleanParam(cn, tran, "LeaveTrains", true);
        }

        public static void SetLeaveTrains(bool val, SqlConnection cn)
        {
            SetBooleanParam(val, cn, "LeaveTrains", true);
        }

        //public static bool GetSpeedTwoWins(SqlConnection cn, SqlTransaction tran = null)
        //{
        //    SpeedRules
        //    return GetBooleanParam(cn, tran, "SpeedTwoWins", false);
        //}

        //public static void SetSpeedTwoWins(bool val, SqlConnection cn)
        //{
        //    SetBooleanParam(val, cn, "SpeedTwoWins", false);
        //}

        public static bool GetSharePoints(SqlConnection cn, SqlTransaction tran)
        {
            return GetBooleanParam(cn, tran, "SharePoints", true);
        }

        public static void SetSharePoints(bool val, SqlConnection cn)
        {
            SetBooleanParam(val, cn, "SharePoints", true);
        }

        public static bool GetOneListExcel(SqlConnection cn, SqlTransaction tran = null)
        {
            return GetBooleanParam(cn, tran, "ExcelOneList", false);
        }

        public static void SetOneListExcel(bool val, SqlConnection cn)
        {
            SetBooleanParam(val, cn, "ExcelOneList", false);
        }

        public static bool GetLessOnePoint(SqlConnection cn)
        {
            return GetLessOnePoint(cn, null);
        }

        public static bool GetLessOnePoint(SqlConnection cn, SqlTransaction tran)
        {
            return GetBooleanParam(cn, tran, "SetLessOne", false);
        }
        
        public static void SetLessOnePoint(bool val, SqlConnection cn)
        {
            SetBooleanParam(val, cn, "SetLessOne", false);
        }

        public static bool GetOnly75(SqlConnection cn)
        {
            return GetOnly75(cn, null);
        }

        public static bool GetOnly75(SqlConnection cn, SqlTransaction tran)
        {
            return GetBooleanParam(cn, tran, "Only75", true);
        }

        public static void SetOnly75(bool val, SqlConnection cn)
        {
            SetBooleanParam(val, cn, "Only75", true);
        }

        public static bool GetIncl40sec(SqlConnection cn, SqlTransaction tran = null)
        {
            return GetBooleanParam(cn, tran, "Incl40sec", false);
        }

        public static void SetIncl40sec(bool val, SqlConnection cn)
        {
            SetBooleanParam(val, cn, "Incl40sec", false);
        }

        public static bool GetLeadTime(SqlConnection cn, SqlTransaction tran = null)
        {
            return GetBooleanParam(cn, tran, "LeadTime", true);
        }

        public static void SetLeadTime(bool val, SqlConnection cn)
        {
            SetBooleanParam(val, cn, "LeadTime", true);
        }

        static bool GetPreQfClimbParam(SqlConnection cn, SqlTransaction tran = null)
        {
            return GetBooleanParam(cn, tran, "PreQfClimb", false);
        }

        public static bool GetPreQfClimb(SqlConnection cn, int listID, SqlTransaction tran = null)
        {
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = cn;
                cmd.Transaction = tran;
                cmd.CommandText = "SELECT style FROM lists(nolock) WHERE iid = @listID";
                cmd.Parameters.Add("@listID", SqlDbType.Int).Value = listID;

                if (!String.Equals(cmd.ExecuteScalar() as String, "Скорость", StringComparison.OrdinalIgnoreCase))
                    return false;
                return GetPreQfClimbParam(cn, tran);
            }
        }

        public static void SetPreQfClimb(bool val, SqlConnection cn)
        {
            SetBooleanParam(val, cn, "PreQfClimb", false);
        }

        public static bool GetPointsForFalls(SqlConnection cn, SqlTransaction tran = null)
        {
            return GetBooleanParam(cn, tran, "PointsForFalls", false);
        }

        public static void SetPointsForFalls(bool val, SqlConnection cn)
        {
            SetBooleanParam(val, cn, "PointsForFalls", false);
        }

        public static Color GetHoldColor(bool forSelected, SqlConnection cn, SqlTransaction tran = null)
        {
            String paramName = forSelected ? "TopoColorSelected" : "TopoColorHold";
            var defaultColor = (forSelected ? Color.LimeGreen : Color.Red).ToArgb();
            return Color.FromArgb(GetIntParam(cn, tran, paramName, defaultColor));
        }

        public static void SetHoldColor(Color value, bool forSelected, SqlConnection cn, SqlTransaction tran = null)
        {
            String paramName = forSelected ? "TopoColorSelected" : "TopoColorHold";
            var defaultColor = (forSelected ? Color.LimeGreen : Color.Red).ToArgb();
            SetIntParam(cn, tran, paramName, value.ToArgb(), defaultColor);
        }

        public static SpeedRules GetSpeedRules(SqlConnection cn, SqlTransaction tran = null)
        {
            return SortingClass.GetCompRules(cn, tran);
        }

        public static void SetSpeedRules(SpeedRules value, SqlConnection cn, SqlTransaction tran = null)
        {
            SortingClass.SetCompRules(value, cn, tran);
        }

        //public static bool GetUseBestSpeed(SqlConnection cn, SqlTransaction tran = null)
        //{
        //    return GetBooleanParam(cn, tran, "BestSpeed", false);
        //}

        //public static void SetUseBestSpeed(bool val, SqlConnection cn)
        //{
        //    SetBooleanParam(val, cn, "BestSpeed", false);
        //}

        //public static bool GetUseBestQf(SqlConnection cn)
        //{
        //    return GetUseBestQf(cn, null);
        //}

        //public static bool GetUseBestQf(SqlConnection cn, SqlTransaction tran)
        //{
        //    return GetBooleanParam(cn, tran, "UseBestQf", false);
        //}

        //public static void SetUseBestQf(bool val, SqlConnection cn)
        //{
        //    SetBooleanParam(val, cn, "UseBestQf", false);
        //}

        private static int GetIntParam(SqlConnection cn, SqlTransaction tran, string columnName, int defaultValue)
        {
            if (!SortingClass.CheckColumn("CompetitionData", columnName, "INT NOT NULL DEFAULT " + defaultValue.ToString(), cn, tran))
                return defaultValue;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            cmd.CommandText = "SELECT " + columnName + " FROM CompetitionData";
            object oTmp = cmd.ExecuteScalar();
            if (oTmp == null || oTmp == DBNull.Value)
            {
                cmd.CommandText = "UPDATE CompetitionData SET " + columnName + "=" + defaultValue.ToString();
                cmd.ExecuteNonQuery();
                return defaultValue;
            }
            else
                return Convert.ToInt32(oTmp);
        }

        private static void SetIntParam(SqlConnection cn, SqlTransaction tran, string columnName, int val, int defaultValue)
        {
            SortingClass.CheckColumn("CompetitionData", columnName, "INT NOT NULL DEFAULT " + defaultValue.ToString(), cn, tran);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            cmd.CommandText = "UPDATE CompetitionData SET " + columnName + "=" + val.ToString();
            cmd.ExecuteNonQuery();
        }

        private static string GetStringParam(SqlConnection cn, SqlTransaction tran, string columnName, string defaultValue)
        {
            if (!SortingClass.CheckColumn("CompetitionData", columnName, "VARCHAR(4096) DEFAULT " + (defaultValue == null ? "NULL" : "'" + defaultValue + "'"), cn, tran))
                return defaultValue;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            cmd.CommandText = "SELECT " + columnName + " FROM CompetitionData";
            object oTmp = cmd.ExecuteScalar();
            if (oTmp == null || oTmp == DBNull.Value)
                return String.Empty;
            else
                return oTmp.ToString();
        }

        private static void SetStringParam(SqlConnection cn, SqlTransaction tran, string columnName, string val, string defaultValue = null)
        {
            SortingClass.CheckColumn("CompetitionData", columnName, "VARCHAR(4096) DEFAULT " + (defaultValue == null ? "NULL" : "'" + defaultValue + "'"), cn, tran);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            cmd.CommandText = "UPDATE CompetitionData SET " + columnName + " = @val";
            cmd.Parameters.Add("@val", SqlDbType.VarChar, 4096);
            if (val == null)
                cmd.Parameters[0].Value = DBNull.Value;
            else
                cmd.Parameters[0].Value = val;
            cmd.ExecuteNonQuery();
        }

        public static int GetTextLineSpeed(SqlConnection cn) { return GetTextLineSpeed(cn, null); }
        public static int GetTextLineSpeed(SqlConnection cn, SqlTransaction tran)
        {
            return GetIntParam(cn, tran, "TextLineSpeed", 10);
        }

        public static void SetTextLineSpeed(int val, SqlConnection cn) { SetTextLineSpeed(val, cn, null); }
        public static void SetTextLineSpeed(int val, SqlConnection cn, SqlTransaction tran)
        {
            SetIntParam(cn, tran, "TextLineSpeed", val, 10);
        }

        public static int GetRefreshSpeed(SqlConnection cn)
        {
            return GetIntParam(cn, null, "ListShowRefresh", 10);
        }

        public static void SetRefreshSpeed(int val, SqlConnection cn)
        {
            SetIntParam(cn, null, "ListShowRefresh", val, 10);
        }

        public static int GetMoveNext(SqlConnection cn)
        {
            return GetIntParam(cn, null, "ListShowMoveNext", 3);
        }

        public static void SetMoveNext(int val, SqlConnection cn)
        {
            SetIntParam(cn, null, "ListShowMoveNext", val, 3);
        }

        public AutoNum CurrentAutoNum
        {
            get
            {
                if (rbNumGroup.Checked)
                    return AutoNum.GRP;
                else if (rbNumRow.Checked)
                    return AutoNum.ROW;
                else
                    return AutoNum.GRP;
            }
            private set
            {
                switch (value)
                {
                    case AutoNum.GRP:
                        rbNumGroup.Checked = true;
                        rbNumRow.Checked = false;
                        break;
                    case AutoNum.ROW:
                        rbNumRow.Checked = true;
                        rbNumGroup.Checked = false;
                        break;
                }
            }
        }

        public SpeedSystemType CurrentSystem
        {
            get
            {
                if (rbSpeedEKB.Checked)
                    return SpeedSystemType.EKB;
                else if (rbSpeedEKO.Checked)
                    return SpeedSystemType.EKO;
                else if (rbSpeedIvanovo.Checked)
                    return SpeedSystemType.IVN;
                else if (rbSpeedNo.Checked)
                    return SpeedSystemType.NON;
                else
                    return SpeedSystemType.EKB;
            }
            private set
            {
                switch (value)
                {
                    case SpeedSystemType.EKB:
                        rbSpeedEKB.Checked = true;
                        rbSpeedEKO.Checked = rbSpeedIvanovo.Checked = rbSpeedNo.Checked = false;
                        break;
                    case SpeedSystemType.IVN:
                        rbSpeedIvanovo.Checked = true;
                        rbSpeedEKO.Checked = rbSpeedEKB.Checked = rbSpeedNo.Checked = false;
                        break;
                    case SpeedSystemType.EKO:
                        rbSpeedEKO.Checked = true;
                        rbSpeedEKB.Checked = rbSpeedIvanovo.Checked = rbSpeedNo.Checked = false;
                        break;
                    case SpeedSystemType.NON:
                        rbSpeedNo.Checked = true;
                        rbSpeedEKO.Checked = rbSpeedEKB.Checked = rbSpeedIvanovo.Checked = false;
                        break;
                }
            }
        }

#if FULL
        public FlashShowMode CurrentFlashShowMode
        {
            get
            {
                if (rbShowFlashNOT.Checked)
                    return FlashShowMode.NOT;
                else if (rbShowFlashADM.Checked)
                    return FlashShowMode.ADM;
                else if (rbShowFlashBTH.Checked)
                    return FlashShowMode.BTH;
                else
                {
                    CurrentFlashShowMode = FlashShowMode.NOT;
                    return FlashShowMode.NOT;
                }
            }
            private set
            {
                switch (value)
                {
                    case FlashShowMode.NOT:
                        rbShowFlashNOT.Checked = true;
                        rbShowFlashADM.Checked = rbShowFlashBTH.Checked = false;
                        break;
                    case FlashShowMode.ADM:
                        rbShowFlashADM.Checked = true;
                        rbShowFlashBTH.Checked = rbShowFlashNOT.Checked = false;
                        break;
                    case FlashShowMode.BTH:
                        rbShowFlashBTH.Checked = true;
                        rbShowFlashADM.Checked = rbShowFlashNOT.Checked = false;
                        break;
                }
            }
        }

        public OSQfMode CurrentOSQfMode
        {
            get
            {
                if (rbPlace.Checked)
                    return OSQfMode.PLC;
                else
                    return OSQfMode.PTS;
            }
            set
            {
                switch (value)
                {
                    case OSQfMode.PLC:
                        rbPlace.Checked = true;
                        break;
                    default:
                        rbPlace.Checked = false;
                        break;
                }
                rbPts.Checked = !rbPlace.Checked;
            }
        }
#endif
        public bool CurrentSetLessOne
        {
            get { return cbSetLessOne.Checked; }
            set { cbSetLessOne.Checked = value; }
        }

        public bool CurrentOnly75
        {
            get { return cbOnly75.Checked; }
            set { cbOnly75.Checked = value; }
        }

        public bool CurrentPosToFalls
        {
            get { return cbSetPosToFalls.Checked; }
            set { cbSetPosToFalls.Checked = value; }
        }

        public bool CurrentIncl40sec
        {
            get { return cb40sec.Checked; }
            set { cb40sec.Checked = value; }
        }

        public bool CurrentLeadTime
        {
            get { return cbLeadTime.Checked; }
            set { cbLeadTime.Checked = value; }
        }

        public bool CurrentLeaveTrains
        {
            get { return cbLeaveTrains.Checked; }
            set { cbLeaveTrains.Checked = value; }
        }

        //public bool CurrentSpeedTwoWins
        //{
        //    get { return cbSpeedTwoWins.Checked; }
        //    set { cbSpeedTwoWins.Checked = value; }
        //}

        public bool CurrentSharePoints
        {
            get { return cbSharePoints.Checked; }
            set { cbSharePoints.Checked = value; }
        }

        public bool CurrentOneListExcel
        {
            get { return cbOneListExcel.Checked; }
            set { cbOneListExcel.Checked = value; }
        }

        public bool CurrentShowNewQf
        {
            get { return cbShowNewQF.Checked; }
            set { cbShowNewQF.Checked = value; }
        }

        public bool CurrentPreQfClimb
        {
            get { return cbPreQfClimb.Checked; }
            set { cbPreQfClimb.Checked = value; }
        }

        public bool CurrentCombinedNew
        {
            get { return cbCombinedNew.Checked; }
            set { cbCombinedNew.Checked = value; }
        }

        public int CurrentTextLineSpeed
        {
            get { return trackBarTextLineSpeed.Value; }
            set { trackBarTextLineSpeed.Value = value; }
        }

        public int CurrentListShowRefresh
        {
            get
            {
                if (cbListShowRefresh.SelectedItem == null)
                    throw new ArgumentNullException("Не выбран интервал обновления");
                switch (cbListShowRefresh.SelectedItem.ToString())
                {
                    case "5 сек.":
                        return 5;
                    case "10 сек.":
                        return 10;
                    case "15 сек.":
                        return 15;
                    case "20 сек.":
                        return 20;
                    case "30 сек.":
                        return 30;
                    case "1 мин.":
                        return 60;
                    default:
                        throw new ArgumentOutOfRangeException("Интервал обновления выбран неверно");
                }
            }
            set
            {
                if (value <= 5)
                    cbListShowRefresh.SelectedIndex = 0;
                else if (value <= 10)
                    cbListShowRefresh.SelectedIndex = 1;
                else if (value <= 15)
                    cbListShowRefresh.SelectedIndex = 2;
                else if (value <= 20)
                    cbListShowRefresh.SelectedIndex = 3;
                else if (value <= 30)
                    cbListShowRefresh.SelectedIndex = 4;
                else
                    cbListShowRefresh.SelectedIndex = 5;
                RefreshCbListShowMoveNext();
            }
        }

        public int CurrentListShowMoveNext
        {
            get
            {
                if (cbListShowMoveNext.SelectedItem == null || cbListShowMoveNext.SelectedIndex < 0)
                    return 1;
                else
                    return (cbListShowMoveNext.SelectedIndex + 1);
            }
            set
            {
                int toSet = (value - 1);
                if (toSet < 0 || toSet >= cbListShowMoveNext.Items.Count)
                    try { cbListShowMoveNext.SelectedIndex = 0; }
                    catch { }
                else
                    cbListShowMoveNext.SelectedIndex = toSet;
            }
        }

        public Color CurrentTopoHoldColor
        {
            get { return btnTopoHold.BackColor; }
            set { btnTopoHold.BackColor = value; }
        }

        public Color CurrentTopoSelectedHoldColor
        {
            get { return btnTopoSelectedHold.BackColor; }
            set { btnTopoSelectedHold.BackColor = value; }
        }

        public Boolean CurrentPointsForFalls
        {
            get { return this.cbPointsForFalls.Checked; }
            set { this.cbPointsForFalls.Checked = value; }
        }

        //public bool CurrentBestSpeed
        //{
        //    get { return rbSpeedBest.Checked; }
        //    set
        //    {
        //        rbSpeedBest.Checked = value;
        //        rbSpeedSum.Checked = !rbSpeedBest.Checked;
        //    }
        //}

        //public bool CurrentUseBestQf
        //{
        //    get { return rbQfSpeedBest.Checked; }
        //    set
        //    {
        //        rbQfSpeedBest.Checked = value;
        //        rbQfSpeedLast.Checked = !rbQfSpeedBest.Checked;
        //    }
        //}

        public SpeedRules CurrentSpeedRules
        {
            get
            {
                SpeedRules retVal;
                if (rbQfSpeedBest.Checked)
                    retVal = SpeedRules.BestResultFromTwoQfRounds;
                else
                    retVal = SpeedRules.DefaultAll;
                if(cbSpeedTwoWins.Checked)
                    retVal = retVal | SpeedRules.SpeedAdvancedSystem;
                else if (rbSpeedWR.Checked)
                    retVal = retVal | SpeedRules.IFSC_WR;
                else if (rbSpeedBest.Checked)
                    retVal = retVal | SpeedRules.BestRouteInQfRound;
                if (rbSpeedSchemaInternational.Checked)
                    retVal = retVal | SpeedRules.InternationalSchema;
               
                return retVal;
            }
            set
            {
                rbQfSpeedBest.Checked = (value & SpeedRules.BestResultFromTwoQfRounds) == SpeedRules.BestResultFromTwoQfRounds;
                rbQfSpeedLast.Checked = !rbQfSpeedBest.Checked;
                cbSpeedTwoWins.Checked = (value & SpeedRules.SpeedAdvancedSystem) == SpeedRules.SpeedAdvancedSystem;
                if (cbSpeedTwoWins.Checked)
                    rbSpeedWR.Checked = true;
                else
                    rbSpeedWR.Checked = (value & SpeedRules.IFSC_WR) == SpeedRules.IFSC_WR;
                if (rbSpeedWR.Checked)
                    rbSpeedSum.Checked = rbSpeedBest.Checked = false;
                else
                {
                    rbSpeedBest.Checked = (value & SpeedRules.BestRouteInQfRound) == SpeedRules.BestRouteInQfRound;
                    rbSpeedSum.Checked = !rbSpeedBest.Checked;
                }
                rbSpeedSchemaInternational.Checked = (value & SpeedRules.InternationalSchema) == SpeedRules.InternationalSchema;
                rbSpeedSchemaNational.Checked = !rbSpeedSchemaInternational.Checked;
                cbSpeedTwoWins.Checked = (value & SpeedRules.SpeedAdvancedSystem) == SpeedRules.SpeedAdvancedSystem;
            }
        }
        

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbSpeedTwoWins.Checked && ((CurrentSpeedRules & SpeedRules.IFSC_WR) != SpeedRules.IFSC_WR))
                {
                    MessageBox.Show(this, "Для проведения \"парной гонки\" до двух побед установите правила скорости \"Эталон IFSC\"");
                    return;
                }
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                if (CurrentSpeedRules != GetSpeedRules(cn))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandText = "SELECT COUNT(*) cnt FROM speedResults(NOLOCK) WHERE resText IS NOT NULL";
                    object oTmp = cmd.ExecuteScalar();
                    if(oTmp != null && oTmp!= DBNull.Value)
                        try
                        {
                            int n = Convert.ToInt32(oTmp);
                            if (n > 0)
                            {
                                if (MessageBox.Show(this, "Уже начато проведение соревнований на скорость.\r\n" +
                                    "Вы уверены, что хотите изменить правила?", "ALARM!!!",
                                     MessageBoxButtons.YesNo, MessageBoxIcon.Error)
                                    == System.Windows.Forms.DialogResult.No)
                                    return;
                            }
                        }
                        catch { }
                }
                SetSpeedRules(CurrentSpeedRules, cn);
                SetCurrentSystem(CurrentSystem, cn);
                SetAutoNum(CurrentAutoNum, cn);
                SetLessOnePoint(CurrentSetLessOne, cn);
                SetOnly75(CurrentOnly75, cn);
#if FULL
                SetFlashShowMode(CurrentFlashShowMode, cn);
                SetOSQfMode(CurrentOSQfMode, cn);
                SetCombinedNew(CurrentCombinedNew, cn);
#endif
                SetTextLineSpeed(CurrentTextLineSpeed, cn);
                SetRefreshSpeed(CurrentListShowRefresh, cn);
                SetMoveNext(CurrentListShowMoveNext, cn);
                SetSharePoints(CurrentSharePoints, cn);

                SetOneListExcel(CurrentOneListExcel, cn);
                SetShowNewQf(CurrentShowNewQf, cn);

                SetIncl40sec(CurrentIncl40sec, cn);
                SetLeadTime(CurrentLeadTime, cn);

                SetPosToFalls(CurrentPosToFalls, cn);
                SetLeaveTrains(CurrentLeaveTrains, cn);

                SetHoldColor(CurrentTopoHoldColor, false, cn);
                SetHoldColor(CurrentTopoSelectedHoldColor, true, cn);

                SetPreQfClimb(CurrentPreQfClimb, cn);
                SetPointsForFalls(CurrentPointsForFalls, cn);


                this.Close();
            }
            catch (Exception ex) { MessageBox.Show(this, "Не удалось сохранить настройки:\r\n" + ex.Message); }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSetPoints_Click(object sender, EventArgs e)
        {
            try
            {
                PointsSettingsForm pf = new PointsSettingsForm(cn, "");
                pf.ShowDialog(this);
            }
            catch (Exception ex) { MessageBox.Show("Ошибка настройки таблицы очков:\r\n" + ex.Message); }
        }

        private void cbListShowRefresh_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshCbListShowMoveNext();
        }

        private void RefreshCbListShowMoveNext()
        {
            int toSet;
            if (cbListShowMoveNext.SelectedItem != null && cbListShowMoveNext.SelectedIndex >= 0)
                toSet = cbListShowMoveNext.SelectedIndex;
            else
                toSet = -1;
            cbListShowMoveNext.Items.Clear();
            if (cbListShowRefresh.SelectedItem == null)
                return;
            switch (cbListShowRefresh.SelectedItem.ToString())
            {
                case "5 сек.":
                    cbListShowMoveNext.Items.Add("5 сек.");
                    cbListShowMoveNext.Items.Add("10 сек.");
                    cbListShowMoveNext.Items.Add("15 сек.");
                    cbListShowMoveNext.Items.Add("20 сек.");
                    break;
                case "10 сек.":
                    cbListShowMoveNext.Items.Add("10 сек.");
                    cbListShowMoveNext.Items.Add("20 сек.");
                    cbListShowMoveNext.Items.Add("30 сек.");
                    cbListShowMoveNext.Items.Add("40 сек.");
                    break;
                case "15 сек.":
                    cbListShowMoveNext.Items.Add("15 сек.");
                    cbListShowMoveNext.Items.Add("30 сек.");
                    cbListShowMoveNext.Items.Add("45 сек.");
                    cbListShowMoveNext.Items.Add("1 мин.");
                    break;
                case "20 сек.":
                    cbListShowMoveNext.Items.Add("20 сек.");
                    cbListShowMoveNext.Items.Add("40 сек.");
                    cbListShowMoveNext.Items.Add("1 мин.");
                    cbListShowMoveNext.Items.Add("1 мин. 20 сек.");
                    break;
                case "30 сек.":
                    cbListShowMoveNext.Items.Add("30 сек.");
                    cbListShowMoveNext.Items.Add("1 мин.");
                    cbListShowMoveNext.Items.Add("1 мин. 30 сек.");
                    cbListShowMoveNext.Items.Add("2 мин.");
                    break;
                case "1 мин.":
                    cbListShowMoveNext.Items.Add("1 мин.");
                    cbListShowMoveNext.Items.Add("2 мин.");
                    cbListShowMoveNext.Items.Add("3 мин.");
                    cbListShowMoveNext.Items.Add("4 мин.");
                    break;
                default:
                    cbListShowMoveNext.Text = "";
                    return;
            }
            if (toSet >= 0)
                CurrentListShowMoveNext = toSet + 1;
            else
                cbListShowMoveNext.Text = "Выберите интервал";
        }

        private void btnTopoHold_Click(object sender, EventArgs e)
        {
            var source = sender as Control;
            if (source == null)
                return;
            ColorDialog selectColor = new ColorDialog
            {
                AllowFullOpen = true,
                Color = source.BackColor,
                ShowHelp = false
            };
            if (selectColor.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
                return;
            source.BackColor = selectColor.Color;
        }
    }
}
