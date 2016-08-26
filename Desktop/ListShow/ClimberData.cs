using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using ClimbingCompetition;
using ListShow.DsListShowTableAdapters;
using System.IO;
using System.Drawing.Imaging;

namespace ListShow
{
    public sealed class ClimberData
    {
        public class ResData
        {
            public string Round { get; private set; }
            public string Pos { get; private set; }
            public string Res { get; private set; }
            public string Style { get; private set; }
            public object[] Row
            {
                get
                {
                    object[] obj = new object[3];
                    obj[0] = Round;
                    obj[1] = Res;
                    obj[2] = Pos;
                    return obj;
                }
            }
            public ResData(string style, string round, string pos, string res)
            {
                this.Style = style;
                this.Round = round;
                this.Pos = pos;
                this.Res = res;
            }
        }
        public string Name { get; private set; }
        public string Team { get; private set; }
        public string Group { get; private set; }
        public Image Photo { get; private set; }
        public int Iid { get; private set; }
        public string IidStr { get { return (this.IsEmpty ? "" : Iid.ToString()); } }
        public string Qf { get; private set; }
        public int Age { get; private set; }
        public string AgeStr { get { return (Age == 0) ? "" : Age.ToString(); } }
        public List<ResData> Results { get; private set; }
        public bool Loaded { get; private set; }

        public bool IsEmpty { get { return (Iid == int.MinValue); } }

        public static ClimberData Empty;

        static ClimberData()
        {
            Empty = new ClimberData(int.MinValue);
            Empty.Age = 0;
            Empty.Name = Empty.Group = Empty.Team = Empty.Qf = "";
            Empty.Photo = null;
            Empty.Loaded = true;
        }

        public DataTable GetResultsTable(string style)
        {
            DataTable dt = new DataTable();
            if (Results == null || Results.Count < 1)
                return dt;
            dt.Columns.Add("Раунд");
            dt.Columns.Add("Рез-т");
            dt.Columns.Add("Место");
            object[] obj = new object[3];
            obj[0] = "";
            obj[1] = obj[2] = "";
            string curSt = "";
            foreach (ResData r in Results)
                if (style.Length == 0 || r.Style == style || style == String.Empty)
                {
                    if (curSt != r.Style)
                    {
                        curSt = r.Style;
                        if (dt.Rows.Count > 0)
                        {
                            obj[0] = "";
                            dt.Rows.Add(obj);
                        }
                        obj[0] = curSt + ":";
                        dt.Rows.Add(obj);
                    }
                    dt.Rows.Add(r.Row);
                }
            return dt;
        }

        public ClimberData(int iid)
        {
            this.Iid = iid;
            Loaded = false;
            Results = new List<ResData>();
        }

        public bool LoadData(SqlConnection cn)
        {
            ParticipantDataTableAdapter ta = new ParticipantDataTableAdapter();
            ta.Connection = cn;
            DsListShow.ParticipantDataDataTable t = ta.GetData(Iid);
            if (t.Rows.Count < 1)
            {
                Loaded = false;
                return false;
            }
            DsListShow.ParticipantDataRow row = (DsListShow.ParticipantDataRow)t.Rows[0];
            this.Name = row.surname.ToUpper();
            if (!row.IsnameNull())
                if (row.name.Length > 0)
                    this.Name += " " + row.name;
            this.Qf = (row.IsqfNull() ? "" : row.qf);
            this.Age = (row.IsageNull() ? 0 : row.age);
            this.Photo = (row.IsphotoNull() ? null : ImageWorker.GetFromBytes(row.photo));
            this.Team = row.team;
            this.Group = row.grp;
            LoadResults(cn);
            this.Loaded = true;
            return true;
        }

        private void LoadResults(SqlConnection cn)
        {
            Results.Clear();
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            try
            {
                cmd.CommandText =
#if FULL
                                  "SELECT l.round, lr.resText, lr.posText, l.style, l.iid l_id " +
                                    "FROM lists l(NOLOCK) " +
                                    "JOIN routeResults lr(NOLOCK) ON l.iid=lr.list_id " +
                                   "WHERE (lr.climber_id = " + Iid.ToString() + ") " +
                                     "AND (lr.posText IS NOT NULL) " +
                                     "AND style = 'Трудность' " +
                                     "AND lr.preQf = 0 " +
                            "UNION " +
#endif
                         "SELECT l.round, lr.resText, lr.posText, l.style, l.iid l_id FROM lists l(NOLOCK) INNER JOIN speedResults lr(NOLOCK) " +
                            "ON l.iid=lr.list_id WHERE (lr.climber_id = " + Iid.ToString() + ") AND " +
                            "(lr.posText IS NOT NULL) AND (lr.preQf=0) ORDER BY style DESC, l_id";
                rdr = cmd.ExecuteReader();
                try
                {
                    while (rdr.Read())
                        Results.Add(new ResData(rdr["style"].ToString(),
                            rdr["round"].ToString(), rdr["posText"].ToString(),
                            rdr["resText"].ToString()));
                }
                finally { rdr.Close(); }
#if FULL
                cmd.CommandText = @"select l.round, lb.nya, lb.disq, lb.tops, lb.topAttempts,
                                           lb.bonuses, lb.bonusAttempts, lb.posText, l.style
                                      from lists l(nolock)
                                      join boulderResults lb(nolock) on lb.list_id = l.iid
                                     where lb.preQf  = 0
                                       and lb.posText is not null
                                       and lb.climber_id = " + Iid.ToString() + " ORDER BY l.iid";
                rdr = cmd.ExecuteReader();
                string round, style, res, pos;
                try
                {
                    while (rdr.Read())
                    {
                        round = rdr["round"].ToString();
                        pos = rdr["postext"].ToString();
                        style = rdr["style"].ToString();
                        if (Convert.ToBoolean(rdr["nya"]))
                            res = "н/я";
                        else if (Convert.ToBoolean(rdr["disq"]))
                            res = "дискв.";
                        else
                            res = rdr["tops"].ToString() + "/" + rdr["topAttempts"].ToString() + " " +
                                rdr["bonuses"].ToString() + "/" + rdr["bonusAttempts"].ToString();
                        Results.Add(new ResData(style, round, pos, res));
                    }
                }
                finally { rdr.Close(); }
                cmd.CommandText = @"select l.round, r.resText, r.posText, l.style
                                      from lists l(nolock)
                                      join routeResults r(nolock) on r.list_id = l.iid
                                     where l.style = 'Боулдеринг'
                                       and r.preQf = 0
                                       and r.posText is not null
                                       and r.climber_id = " + Iid.ToString() + " order by l.iid";
                rdr = cmd.ExecuteReader();
                try
                {
                    while (rdr.Read())
                    {
                        pos = rdr["posText"].ToString();
                        res = rdr["resText"].ToString();
                        round = rdr["round"].ToString();
                        style = rdr["style"].ToString();
                        Results.Add(new ResData(style, round, pos, res));
                    }
                }
                finally { rdr.Close(); }
#endif
            }
            catch { }
        }


        public void SetToListBox(ListView listBox1)
        {
            DataTable climberResTable = GetResultsTable("");
            try
            {
                listBox1.Clear();
                listBox1.Columns.Clear();
                if (climberResTable.Columns.Count < 1)
                    return;
                int j = 0;
                listBox1.Columns.Add(climberResTable.Columns[j].ColumnName, 200, HorizontalAlignment.Center);
                for (j++; j < climberResTable.Columns.Count; j++)
                    listBox1.Columns.Add(climberResTable.Columns[j].ColumnName, 80, HorizontalAlignment.Center);
                //foreach (DataColumn dc in dTamble.Columns)
                //    listBox1.Columns.Add(dc.ColumnName, 40, HorizontalAlignment.Center);
                //listBox1.Columns[0].Width = 200;
                //listBox1.Columns[1].Width = 80;
                //listBox1.Columns[2].Width = 80;
                for (int i = 0; i < climberResTable.Rows.Count; i++)
                {
                    listBox1.Items.Add(climberResTable.Rows[i][0].ToString());
                    for (int k = 1; k < climberResTable.Columns.Count; k++)
                        listBox1.Items[i].SubItems.Add(climberResTable.Rows[i][k].ToString());
                }
            }
            finally
            {
                listBox1.View = View.Details;
                listBox1.Visible = true;
            }
        }



        public static MemoryStream GetEmptyImage()
        {
            MemoryStream emptyStream = new MemoryStream();
            Icon icn = new Icon("clm.ico");
            Bitmap bmp = icn.ToBitmap();
            emptyStream = new MemoryStream();
            bmp.Save(emptyStream, ImageFormat.Jpeg);
            emptyStream.Position = 0;
            return emptyStream;
        }
        public bool NoPhoto { get { return (Photo == null); } }
        public MemoryStream GetImageStream()
        {
            if (Photo == null)
                return GetEmptyImage();
            MemoryStream mstr = ImageWorker.GetStreamFromImage(Photo);
            try { mstr.Position = 0; }
            catch { }
            return mstr;
        }
    }
}
