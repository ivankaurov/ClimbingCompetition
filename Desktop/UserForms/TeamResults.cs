using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Excel = Microsoft.Office.Interop.Excel;

namespace ClimbingCompetition
{
    /// <summary>
    /// ФОрма для отображения командных результатов + родитель для формы отображения результатов многоборья
    /// </summary>
    public partial class TeamResults : BaseForm
    {
        //protected SqlConnection cn;
        protected List<object> generalResults = new List<object>();
        protected DataTable dtRes = new DataTable();
        protected List<string> styles = new List<string>();
        int list_id;

        protected struct team
        {
            public int iid;
            public string name;
        }

        protected struct group
        {
            public int iid;
            public bool female;
        }

        public class teamRes
        {
            public List<styleRes> styleResults = null;
            public double res = 0.0;
            public int pos = -1;
            public string name = "";
            public int iid = -1;
        }

        public class styleRes
        {
            public DataTable dt = null;
            public string style = "";
            public double res = 0.0;
        }

        public TeamResults(SqlConnection baseCon, string compTitle) : base(baseCon, compTitle)
        {
            InitializeComponent();
            dg.DataSource = dtRes;
            this.Text = "Командные результаты";
        }

        public TeamResults(SqlConnection baseCon, int list_id, string competitionTitle)
            : base(baseCon, competitionTitle)
        {
            //try
            //{
            //    if (cn.State != ConnectionState.Open)
            //        cn.Open();
            //}
            //catch { }
            InitializeComponent();
            this.Text = "Командные результаты";
            //this.cn = cn;
            this.list_id = list_id;
            TeamResult tmResType = SelectTeamType.selectResultsType(this.cn, list_id, competitionTitle);
            if (tmResType.type == TeamResultsType.Cancel)
                return;
            List<team> teams = new List<team>();
            List<group> groups = new List<group>();


            if (tmResType.lead)
                styles.Add("Трудность");
            if (tmResType.speed)
                styles.Add("Скорость");
            if (tmResType.boulder)
                styles.Add("Боулдеринг");
            if (styles.Count == 0)
                return;

            SqlCommand cmd = new SqlCommand("SELECT iid,name FROM teams(NOLOCK) ORDER BY name", this.cn);
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                team t;
                t.iid = Convert.ToInt32(rdr[0]);
                t.name = rdr[1].ToString();
                teams.Add(t);
            }
            rdr.Close();

            cmd.CommandText = "DELETE FROM teamResults WHERE list_id = " + list_id.ToString();
            try { cmd.ExecuteNonQuery(); }
            catch
            {
                cmd.CommandText = "CREATE TABLE [dbo].[teamResults] (" +
"	[list_id] [int] NOT NULL ," +
"	[team_id] [int] NOT NULL ," +
"	[pos] [int] NULL ," +
"	CONSTRAINT [PK_teamResults] PRIMARY KEY  CLUSTERED " +
"	(" +
"		[list_id]," +
"		[team_id]" +
"	)  ON [PRIMARY] ," +
"	CONSTRAINT [FK_teamResults_lists] FOREIGN KEY " +
"	(" +
"		[list_id]" +
"	) REFERENCES [dbo].[lists] (" +
"		[iid]" +
"	) ON DELETE CASCADE  ON UPDATE CASCADE ," +
"	CONSTRAINT [FK_teamResults_Teams] FOREIGN KEY " +
"	(" +
"		[team_id]" +
"	) REFERENCES [dbo].[Teams] (" +
"		[iid]" +
"	) ON DELETE CASCADE  ON UPDATE CASCADE " +
")";
                cmd.ExecuteNonQuery();
            }

            cmd.CommandText = "SELECT iid,genderFemale FROM groups(NOLOCK) ORDER BY name";
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                group gr;
                gr.iid = Convert.ToInt32(rdr[0]);
                gr.female = Convert.ToBoolean(rdr[1]);
                groups.Add(gr);
            }
            rdr.Close();
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@q2nd", SqlDbType.Float);
            cmd.Parameters[0].Value = tmResType.Qf2nd;
            cmd.Parameters.Add("@teamID", SqlDbType.Int);
            cmd.Parameters.Add("@style", SqlDbType.VarChar, 255);
            foreach (team tm in teams)
            {
                teamRes tRes = new teamRes();
                tRes.res = 0;
                tRes.styleResults = new List<styleRes>();
                tRes.pos = tm.iid;
                tRes.name = tm.name;
                tRes.iid = tm.iid;
                cmd.Parameters[1].Value = tm.iid;
                foreach (string stle in styles)
                {
                    styleRes stRes = new styleRes();
                    stRes.style = stle;//и g.iid и tm.iid
                    stRes.res = 0;
                    cmd.Parameters[2].Value = stle;
                    stRes.dt = new DataTable();
                    if (tmResType.type == TeamResultsType.Group)
                    {
                        if (cmd.Parameters.Count < 4)
                            cmd.Parameters.Add("@groupID", SqlDbType.Int);
                        foreach (group g in groups)
                        {
                            cmd.Parameters[3].Value = g.iid;
                            if (g.female)
                                cmd.CommandText =
                                    "SELECT TOP " + tmResType.nFemale.ToString() + " p.iid AS [№], " +
                                           "p.surname+' '+p.name AS [Фамилия, Имя], t.name AS [Команда]," +
                                     "      (CASE p.team_id WHEN @teamID THEN 1.0 ELSE @q2nd END) * gr.pts AS [Балл] " +
                                     " FROM lists l(NOLOCK)" +
                                     " JOIN generalResults gr(NOLOCK) ON l.iid = gr.list_id" +
                                     " JOIN Participants p(NOLOCK) ON gr.climber_id = p.iid" +
                                " LEFT JOIN teamsLink tl(nolock) on tl.climber_id = p.iid" +
                                     "                          and 1 = CASE p.team_id WHEN @teamID THEN 0 ELSE 1 END" +
                                     " JOIN Teams t(NOLOCK) ON t.iid = ISNULL(tl.team_id, p.team_id) " +
                                    " WHERE l.style = @style" +
                                    "   AND t.iid = @teamID" +
                                    "   AND l.group_id = @groupID" +
                                    "   AND gr.pts > 0" +
                                 " ORDER BY gr.pts DESC";
                            else
                                cmd.CommandText =
                                    "SELECT TOP " + tmResType.nMale.ToString() + " p.iid AS [№], " +
                                    "       p.surname+' '+p.name AS [Фамилия, Имя], t.name AS [Команда]," +
                                    "       (CASE p.team_id WHEN @teamID THEN 1.0 ELSE @q2nd END) * gr.pts AS [Балл] " +
                                    "  FROM lists l(NOLOCK)" +
                                    "  JOIN generalResults gr(NOLOCK) ON l.iid=gr.list_id" +
                                    "  JOIN Participants p(NOLOCK) ON gr.climber_id=p.iid" +
                                " LEFT JOIN teamsLink tl(nolock) on tl.climber_id = p.iid" +
                                    "                           and 1 = CASE p.team_id WHEN @teamID THEN 0 ELSE 1 END" +
                                    "  JOIN Teams t(NOLOCK) ON t.iid = ISNULL(tl.team_id, p.team_id)" +
                                    " WHERE l.style = @style" +
                                    "   AND t.iid = @teamID" +
                                    "   AND l.group_id = @groupID" +
                                    "   AND gr.pts > 0" +
                                 " ORDER BY gr.pts DESC";
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(stRes.dt);
                        }
                    }
                    else
                    {
                        cmd.CommandText =
                            "SELECT TOP " + tmResType.nMale.ToString() + " p.iid AS [№], " +
                            "       p.surname+' '+p.name AS [Фамилия, Имя], t.name AS [Команда]," +
                            "       (CASE p.team_id WHEN @teamID THEN 1.0 ELSE @q2nd END) * gr.pts AS [Балл] " +
                            "  FROM lists l" +
                            "  JOIN generalResults gr(NOLOCK) ON l.iid = gr.list_id" +
                            "  JOIN Participants p(NOLOCK) ON gr.climber_id = p.iid" +
                        " LEFT JOIN teamsLink tl(nolock) ON tl.climber_id = p.iid" +
                            "                           AND 1 = CASE p.team_id WHEN @teamID THEN 0 ELSE 1 END" +
                            "  JOIN Teams t(NOLOCK) ON t.iid = ISNULL(tl.team_id, p.team_id) " +
                            " WHERE l.style = @style" +
                            "   AND t.iid = @teamID" +
                            "   AND gr.pts > 0" +
                         " ORDER BY gr.pts DESC";
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(stRes.dt);
                    }
                    foreach (DataRow dr in stRes.dt.Rows)
                        stRes.res += Math.Round(Convert.ToDouble(dr["Балл"]), 2);
                    tRes.styleResults.Add(stRes);
                }
                foreach (styleRes str in tRes.styleResults)
                    tRes.res += str.res;
                generalResults.Add(tRes);
            }

            TeamComparer tc = new TeamComparer();
            generalResults.Sort(tc);

            dtRes.Columns.Add("Фамилия, Имя", typeof(string));
            dtRes.Columns.Add("Балл", typeof(double));
            dtRes.Columns.Add("Команда", typeof(string));

            foreach (teamRes tr in generalResults)
            {
                foreach (styleRes sr in tr.styleResults)
                {
                    foreach (DataRow dr in sr.dt.Rows)
                    {
                        object[] obj = new object[3];
                        obj[0] = dr["Фамилия, Имя"].ToString();
                        int i = (int)(Convert.ToDouble(dr["Балл"]) * 100);
                        double d = (double)i / 100.0;
                        obj[1] = d;
                        obj[2] = tr.name;
                        dtRes.Rows.Add(obj);
                    }
                }
            }
            dg.DataSource = dtRes;
            if (generalResults.Count < 1)
                return;
            
            teamRes ttRes = (teamRes)generalResults[0];
            double ptsCur = ttRes.res;
            ttRes.pos = 1;
            generalResults[0] = ttRes;
            int curPos = 1;
            for (int i = 1; i < generalResults.Count; i++)
            {
                ttRes = (teamRes)generalResults[i];
                if (ttRes.res != ptsCur)
                {
                    ttRes.pos = (i + 1);
                    curPos = ttRes.pos;
                    ptsCur = ttRes.res;
                }
                else
                    ttRes.pos = curPos;
                generalResults[i] = ttRes;
            }
            cmd.CommandText = "INSERT INTO teamResults(list_id, team_id, pos) VALUES (" + list_id.ToString() + ", @t, @p)";
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@t", SqlDbType.Int);
            cmd.Parameters.Add("@p", SqlDbType.Int);
            foreach (teamRes t in generalResults)
            {
                cmd.Parameters[0].Value = t.iid;
                cmd.Parameters[1].Value = t.pos;
                cmd.ExecuteNonQuery();
            }
        }

        protected virtual void ExcelExport()
        {
            Excel.Workbook wb;
            Excel.Worksheet ws;
            Excel.Application xlApp;

            if (!StaticClass.LaunchExcel(out xlApp, out wb, out ws, true, cn))
                return;
            try
            {

                const int frst_row = 5;

                int row = frst_row + 1;

                char lstC = 'E';

                Excel.Range range;

                string strT = "";
                if (styles.Count == 1)
                {
                    #region OneStyle
                    foreach (teamRes tr in generalResults)
                    {
                        int startRow = row;
                        styleRes sr = (styleRes)tr.styleResults[0];
                        if (strT == "")
                            strT = sr.style;
                        if (sr.dt.Rows.Count > 0)
                            row--;
                        foreach (DataRow dr in sr.dt.Rows)
                        {
                            row++;
                            range = ws.get_Range("C" + row.ToString(), "C" + row.ToString());
                            range.Cells[1, 1] = dr["Фамилия, Имя"].ToString();
                            //range.Style = "Names";

                            range = ws.get_Range("D" + row.ToString(), "D" + row.ToString());
                            range.Cells[1, 1] = dr["Балл"].ToString();
                            //range.Style = "Points";
                        }

                        range = ws.get_Range("C" + startRow.ToString(), "C" + row.ToString());
                        range.Style = "Names";
                        range = ws.get_Range("D" + startRow.ToString(), "D" + row.ToString());
                        range.Style = "Points";

                        range = ws.get_Range("B" + startRow.ToString(), "B" + row.ToString());
                        range.Cells[1, 1] = tr.name;
                        range.Merge(Type.Missing);
                        range.Style = "Teams";

                        range = ws.get_Range("E" + startRow.ToString(), "E" + row.ToString());
                        range.Cells[1, 1] = sr.res.ToString();
                        range.Style = "Teams";
                        range.Merge(Type.Missing);

                        range = ws.get_Range("A" + startRow.ToString(), "A" + row.ToString());
                        range.Cells[1, 1] = tr.pos.ToString();
                        range.Style = "Teams";
                        range.Merge(Type.Missing);

                        row++;
                    }
                    range = ws.get_Range("A" + frst_row.ToString(), "A" + frst_row.ToString());
                    range.Style = "Teams";
                    range.Cells[1, 1] = "Место";

                    range = ws.get_Range("B" + frst_row.ToString(), "B" + frst_row.ToString());
                    range.Style = "Teams";
                    range.Cells[1, 1] = "Команда";

                    range = ws.get_Range("C" + frst_row.ToString(), "D" + frst_row.ToString());
                    range.Style = "Teams";
                    range.Cells[1, 1] = strT;
                    range.Merge(Type.Missing);

                    range = ws.get_Range("E" + frst_row.ToString(), "E" + frst_row.ToString());
                    range.Style = "Teams";
                    range.Cells[1, 1] = "Баллы";
                    range = ws.get_Range("A" + frst_row.ToString(), "E" + row.ToString());
                    range.Columns.AutoFit();
                    #endregion
                }
                else
                {
                    #region Combined
                    lstC = 'C';
                    strT = "Многоборье";
                    ws.Cells[row, 1] = "Место";
                    ws.Cells[row, 2] = "Команда";
                    int k = 3;
                    for (; k < (styles.Count + 3); k++)
                    {
                        lstC++;
                        ws.Cells[row, k] = (string)styles[k - 3];
                    }
                    ws.Cells[row, k] = "Баллы";
                    foreach (teamRes tr in generalResults)
                    {
                        row++;
                        ws.Cells[row, 1] = tr.pos;
                        ws.Cells[row, 2] = tr.name;
                        k = 3;
                        for (; k < (styles.Count + 3); k++)
                            ws.Cells[row, k] = ((styleRes)tr.styleResults[k - 3]).res;
                        ws.Cells[row, k] = tr.res;
                    }
                    range = ws.get_Range("A" + (frst_row + 1).ToString(), lstC + row.ToString());
                    range.Style = "MyStyle";
                    range.Columns.AutoFit();
                    range = ws.get_Range("B" + (frst_row + 1).ToString(), "B" + row.ToString());
                    range.Style = "StyleLA";
                    #endregion
                }



                //try
                //{
                //    style = wb.Styles.Add("CompTitle", Type.Missing);
                //    style.Font.Size = 16;
                //    style.Font.Bold = true;
                //    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                //    style.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                //}
                //catch { }

                range = ws.get_Range("A1", lstC + "1");
                range.Style = "CompTitle";
                range.Merge(Type.Missing);
                range.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[0];

                ws.Cells[2, 1] = StaticClass.ParseCompTitle(competitionTitle)[1];

                range = ws.get_Range("E2", lstC + "2");
                range.Style = "StyleRA";
                range.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[2];
                //try
                //{
                //    style = wb.Styles.Add("Title", Type.Missing);
                //    style.Font.Bold = true;
                //    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                //    style.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                //}
                //catch { }

                range = ws.get_Range("A4", lstC + "4");
                range.Style = "Title";
                range.Merge(Type.Missing);
                range.Cells[1, 1] = "Командный зачёт";

                range = ws.get_Range("A3", lstC + "3");
                range.Style = "Title";
                range.Merge(Type.Missing);
                range.Cells[1, 1] = "ИТОГОВЫЙ ПРОТОКОЛ РЕЗУЛЬТАТОВ";

                try { ws.Name = "Команды_" + strT; }
                catch { }
                ws.PageSetup.Orientation = Excel.XlPageOrientation.xlPortrait;
            }
            catch (Exception ex) { MessageBox.Show("Ошибка экспорта данных в Excel:\r\n" + ex.Message); }
            finally { StaticClass.SetExcelVisible(xlApp); }
        }

        protected void xlExport_Click(object sender, EventArgs e)
        {
            ExcelExport();
        }

        protected virtual void RefreshAll()
        {
        }

    }
    class TeamComparer: IComparer<object>
    {
        #region IComparer Members

        public int Compare(object x, object y)
        {
            if ((x is TeamResults.teamRes) && (y is TeamResults.teamRes))
            {
                TeamResults.teamRes X = (TeamResults.teamRes)x;
                TeamResults.teamRes Y = (TeamResults.teamRes)y;
                return (Y.res.CompareTo(X.res));
            }
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }

}
