using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;

namespace ClimbingCompetition
{
    public partial class SafetyRepParams : ClimbingCompetition.BaseForm
    {
        int? selectedTeam = null;

        public SafetyRepParams(SqlConnection baseCn, string competitionTitle, int? selectedTeam)
            : base(baseCn, competitionTitle, null)
        {
            InitializeComponent();
            this.selectedTeam = selectedTeam;
        }

        private void SafetyRepParams_Load(object sender, EventArgs e)
        {
            try
            {
                if (cn == null)
                    this.Close();
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                dateToPrint.Value = DateTime.Now;
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = new SqlCommand();
                da.SelectCommand.Connection = cn;
                if (selectedTeam == null)
                {
                    rbOnlyOne.Visible = false;
                    rbAll.Checked = true;
                }
                else
                {
                    da.SelectCommand.CommandText = "SELECT name FROM teams(NOLOCK) WHERE iid=" + selectedTeam.ToString();
                    string res = da.SelectCommand.ExecuteScalar() as string;
                    if (res == null)
                    {
                        rbOnlyOne.Visible = false;
                        rbAll.Checked = true;
                    }
                    else
                    {
                        rbOnlyOne.Text = "Только " + res + " (iid=" + selectedTeam.ToString() + ")";
                        rbOnlyOne.Visible = rbOnlyOne.Checked = true;
                    }
                }
                da.SelectCommand.CommandText =
                    "  SELECT iid AS[№], name AS [Команда], CONVERT(BIT,0) AS [Печатать] " +
                    "    FROM teams(NOLOCK) " +
                    "ORDER BY name";
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgTeams.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных:\r\n" +
                    ex.Message);
                this.Close();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (PrintData<Word.Application>(ReportToPrint.TB_REP))
                this.Close();
        }

        enum ReportToPrint { TB_REP, APPL_REP }

        private bool PrintData<T>(ReportToPrint repToPrint) where T : class
        {
            List<int> ToPrint = new List<int>();
            DataTable dt = dgTeams.DataSource as DataTable;
            if (dt == null)
            {
                MessageBox.Show(this, "Неизвестная ошибка загрузки списка команд");
                return false;
            }
            int lastCol = dt.Columns.Count - 1;
            if (lastCol < 0)
            {
                MessageBox.Show(this, "Неизвестная ошибка загрузки списка команд");
                return false;
            }
            if (rbOnlyOne.Checked)
            {
                if (selectedTeam == null)
                {
                    MessageBox.Show(this, "Неизвестная ошибка загрузки списка команд");
                    return false;
                }
                ToPrint.Add(selectedTeam.Value);
            }
            else if (rbSelected.Checked || rbAll.Checked)
            {
                foreach (DataRow row in dt.Rows)
                    if (Convert.ToBoolean(row[lastCol]) || rbAll.Checked)
                        ToPrint.Add(Convert.ToInt32(row[0]));
            }
            else
            {
                MessageBox.Show(this, "Режим печати отета не выбран");
                return false;
            }
            if (ToPrint.Count < 1)
            {
                MessageBox.Show(this, "Не выбраны команды для печати отчета");
                return false;
            }
            string dir;
            if (ToPrint.Count == 1 || repToPrint == ReportToPrint.APPL_REP)
                dir = String.Empty;
            else
            {
                ClComp def = ClComp.Default;
                dir = String.IsNullOrEmpty(def.DefaultReportFolder) ? MainForm.GetDir() : def.DefaultReportFolder;
                FolderBrowserDialog ofd = new FolderBrowserDialog();
                ofd.SelectedPath = dir;
                ofd.Description = "Выберите папку для сохранения отчетов";
                if (ofd.ShowDialog() == DialogResult.Cancel)
                    return false;
                dir = ofd.SelectedPath;
                def.DefaultReportFolder = dir;
                def.Save();
            }


            TBReports<T> rep = new TBReports<T>(cn, ToPrint, dir, this);
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT MAX(iid) FROM teams(NOLOCK)";
            object res = cmd.ExecuteScalar();
            if (res != null && res != DBNull.Value)
            {
                int lC = res.ToString().Length;
                if (lC > 0)
                {
                    string tpl = String.Empty;
                    for (int i = 0; i < lC; i++)
                        tpl += "0";
                    rep.IdTemplate = tpl;
                }
            }

            ReportCreationBar<T>.CreateRep func;

            switch (repToPrint)
            {
                case ReportToPrint.TB_REP:
                    func = rep.CreateTBReport_DelegateFunction;
                    break;
                case ReportToPrint.APPL_REP:
                    func = rep.CreateApplSecrData;
                    break;
                default:
                    MessageBox.Show(this, "Неизвестный отчет: " + repToPrint.ToString());
                    return false;
            }

            return rep.PrintAllReports(func, cbNoDate.Checked ? null : new DateTime?(dateToPrint.Value));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Закрыть окно не напечатав отчет?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == System.Windows.Forms.DialogResult.Yes)
                this.Close();
        }

        private void dgTeams_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (!rbSelected.Checked)
                rbSelected.Checked = true;
        }

        private void btnPrintAppls_Click(object sender, EventArgs e)
        {
            if (PrintData<Excel.Application>(ReportToPrint.APPL_REP))
                this.Close();
        }

        class TBReports<T> where T:class
        {
            SqlConnection cn;
            List<int> listToPrint;
            int currentIid;
            string dirToSave;
            string dirToLoad;
            string dateToSet;
            string idTemplate = String.Empty;
            public string IdTemplate { get { return idTemplate; } set { idTemplate = value; } }
            IWin32Window parent;


            public TBReports(SqlConnection cn, List<int> listToPrint, string dirToSave,  IWin32Window parent)
            {
                this.cn = cn;
                this.listToPrint = listToPrint;
                this.dirToSave = dirToSave;
                this.parent = parent;
                this.dirToLoad = Path.Combine(MainForm.GetDir(), "Templates");
                this.dirToLoad = Path.Combine(this.dirToLoad, "tb.dot");
            }

            public bool PrintAllReports(ReportCreationBar<T>.CreateRep reportFunction, DateTime? dtDateToSet)
            {
                this.dateToSet = (dtDateToSet == null ? "\"____\" ________________ 20___" :
                    dtDateToSet.Value.ToString("dd MMMM yyyy", new CultureInfo("ru-RU"))) + " г.";

                if (reportFunction == CreateApplSecrData)
                {
                    ReportCreationBar<T> rb = new ReportCreationBar<T>(reportFunction);
                    rb.ShowAfterComplete = rb.AnimatedHeader = true;
                    if (parent == null)
                        rb.ShowDialog();
                    else
                        rb.ShowDialog(parent);
                }
                else
                {

                    for (int i = 0; i < listToPrint.Count; i++)
                    {
                        currentIid = listToPrint[i];
                        ReportCreationBar<T> rb = new ReportCreationBar<T>(reportFunction,
                            "Формирование отчета (" + (i + 1).ToString() + " из " + listToPrint.Count.ToString() + ")");
                        if (listToPrint.Count > 1)
                            rb.ShowAfterComplete = rb.AnimatedHeader = false;
                        if (parent == null)
                            rb.ShowDialog();
                        else
                            rb.ShowDialog(parent);
                    }
                    if (!String.IsNullOrEmpty(dirToSave))
                    {
                        string sToShow = "Формирование файлов завершено, см. папку " + dirToSave;
                        if (parent == null)
                            MessageBox.Show(sToShow);
                        else
                            MessageBox.Show(parent, sToShow);
                    }
                }
                return true;
            }

            public T CreateApplSecrData(BackgroundWorker bw)
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                Excel.Application app = null;
                Excel.Workbook wb = null;
                Excel.Worksheet ws = null;
                if (StaticClass.LaunchExcel(out app, out wb, out ws, false, false, cn, false, false))
                {
                    try
                    {
                        if (app as T == null)
                        {
                            app.Visible = true;
                            ((Excel._Application)app).Quit();
                            bw.ReportProgress(100);
                            return default(T);
                        }
                        bool first = true;
                        foreach (var currentIid in listToPrint)
                        {
                            if (first)
                                first = false;
                            else
                                ws = (Excel.Worksheet)wb.Sheets.Add();
                            bw.ReportProgress(15);

                            ws.PageSetup.CenterHorizontally = true;
                            ws.PageSetup.Zoom = false;
                            ws.PageSetup.FitToPagesTall = 32767;
                            ws.PageSetup.FitToPagesWide = 1;
                            ws.PageSetup.Orientation = Excel.XlPageOrientation.xlLandscape;

                            Excel.Range range = ws.get_Range("A1", "O1");
                            range.MergeCells = true;
                            range.Font.Bold = true;
                            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            cmd.CommandText = "SELECT SHORT_NAME FROM CompetitionData(NOLOCK)";
                            string compName = cmd.ExecuteScalar() as string;
                            if (compName != null)
                                range.Cells[1, 1] = "Заявочный лист \"" + compName + "\"";

                            range = ws.get_Range("H2", "J2");
                            range.MergeCells = true;
                            range.RowHeight = 25.5;
                            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                            range.WrapText = true;
                            range.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                            range.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                            range.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                            range.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                            range.Cells[1, 1] = "Участие в видах";

                            bw.ReportProgress(25);

                            int rowStart = 3;
                            int curRow = rowStart;

                            ws.Cells[curRow, 1] = "№ п/п";
                            ws.Cells[curRow, 11] = "Страховка";
                            ws.Cells[curRow, 12] = "Разр. книжка";
                            ws.Cells[curRow, 13] = "Паспорт";
                            ws.Cells[curRow, 14] = "Мед. допуск";
                            ws.Cells[curRow, 15] = "Ст. взнос";
                            ws.get_Range(ws.Cells[curRow, 11], ws.Cells[curRow, 15]).ColumnWidth = 7.14;

                            range = ws.get_Range(ws.Cells[curRow, 1], ws.Cells[curRow, 15]);
                            range.WrapText = true;
                            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

                            string sTeam = null;

                            bw.ReportProgress(35);


                            cmd.CommandText = "SELECT name FROM Teams(NOLOCK) WHERE iid=" + currentIid.ToString();
                            sTeam = cmd.ExecuteScalar() as string;
                            cmd.CommandText = @"SELECT p.iid AS [Инд.№], p.surname + ' ' + p.name [Фамилия, Имя],
                                               p.age [Г.р.], p.qf [Разр.], t.name [Команда],
                                               CASE p.genderFemale WHEN 1 THEN 'ж' ELSE '' END [Пол],
                                               CASE p.lead WHEN 1 THEN '+' ELSE '' END [Тр.],
                                               CASE p.speed WHEN 1 THEN '+' ELSE '' END [Ск.],
                                               CASE p.boulder WHEN 1 THEN '+' ELSE '' END [Боулд.]
                                          from Participants p(NOLOCK)
                                          join Teams t(NOLOCK) ON t.iid = p.team_id
                                          join Groups g(NOLOCK) ON g.iid = p.group_id
                                         where p.team_id=" + currentIid.ToString() + @"
                                      order by g.oldYear, g.genderFemale, p.surname, p.name, p.iid";
                            DataTable dt = new DataTable();
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(dt);
                            for (int i = 0; i < dt.Columns.Count; i++)
                                ws.Cells[curRow, 2 + i] = dt.Columns[i].ColumnName;

                            int val = 40;
                            bw.ReportProgress(val);

                            int step = dt.Rows.Count > 0 ? (90 - val) / dt.Rows.Count : 1;
                            if (step < 1)
                                step = 1;

                            foreach (DataRow row in dt.Rows)
                            {
                                ws.Cells[++curRow, 1] = curRow - rowStart;
                                for (int i = 0; i < dt.Columns.Count; i++)
                                    ws.Cells[curRow, 2 + i] = row[i];
                                if (String.IsNullOrEmpty(sTeam))
                                    try { sTeam = row["Команда"] as string; }
                                    catch { }
                                val += step;
                                if (val > 90)
                                    val = 90;
                                bw.ReportProgress(val);

                            }

                            sTeam = currentIid.ToString() + (String.IsNullOrEmpty(sTeam) ? String.Empty : "_" + sTeam);
                            ws.Name = sTeam;

                            range = ws.get_Range(ws.Cells[rowStart, 1], ws.Cells[curRow, 15]);
                            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            range.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                            range.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                            range.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                            range.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                            range.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;

                            if (dt.Rows.Count > 0)
                            {
                                range.Borders[Excel.XlBordersIndex.xlInsideHorizontal].LineStyle = Excel.XlLineStyle.xlContinuous;
                                range = ws.get_Range(ws.Cells[rowStart + 1, 3], ws.Cells[curRow, 3]);
                                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

                                range = ws.get_Range(ws.Cells[rowStart, 1], ws.Cells[curRow, 10]);
                                range.Columns.AutoFit();
                            }
                            range = ws.get_Range(ws.Cells[rowStart, 1], ws.Cells[curRow, 15]);
                            range.Rows.AutoFit();
                        }
                        
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show("Ошибка создания отчета:\r\n" + ex2.Message);
                        StaticClass.SetExcelVisible(app);
                        return app as T;
                    }    
                }
                bw.ReportProgress(100);
                return app as T;
            }

            public T CreateTBReport_DelegateFunction(BackgroundWorker bw)
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                
                Word.Application app = null;
                Word.Document doc = null;
                if (StaticClass.LaunchWord(dirToLoad, out doc, out app))
                {
                    if (app as T == null)
                    {
                        app.Visible = true;
                        ((Word._Application)app).Quit();
                        bw.ReportProgress(100);
                        return default(T);
                    }
                    bw.ReportProgress(15);

                    cmd.CommandText = "SELECT NAME FROM CompetitionData(NOLOCK)";
                    WordReportCreationBar.ReplaceLabel(doc, app, "COMP_NAME", cmd.ExecuteScalar() as string, true);
                    bw.ReportProgress(20);

                    cmd.CommandText = "SELECT name FROM teams(NOLOCK) WHERE iid = " + currentIid.ToString();
                    string sTeam = cmd.ExecuteScalar() as string;
                    if (sTeam == null)
                        sTeam = String.Empty;
                    WordReportCreationBar.ReplaceLabel(doc, app, "TEAM_ST", sTeam, true);
                    bw.ReportProgress(25);

                    cmd.CommandText = "SELECT iid FROM judgeView(NOLOCK) WHERE pos LIKE '%Зам%безоп%'";
                    object oTmp = cmd.ExecuteScalar();
                    string sTmp;
                    if (oTmp == null || oTmp == DBNull.Value)
                        sTmp = String.Empty;
                    else
                        sTmp = StaticClass.GetJudgeData(Convert.ToInt32(oTmp), cn, false);
                    if (sTmp == String.Empty)
                        sTmp = " /______________________/";
                    else
                        sTmp = " / " + sTmp + " /";
                    WordReportCreationBar.ReplaceLabel(doc, app, "JUDGE", sTmp, true);
                    bw.ReportProgress(30);

                    WordReportCreationBar.ReplaceLabel(doc, app, "DATE", dateToSet, true);
                    bw.ReportProgress(33);

                    cmd.CommandText = "   SELECT surname, name, age, qf " +
                                      "     FROM Participants(NOLOCK) " +
                                      "    WHERE team_id = " + currentIid.ToString() +
                                      " ORDER BY surname, name, age, qf ";
                    SqlDataReader rdr = cmd.ExecuteReader();
                    int curProgress = 33;
                    int progrStep = 2;
                    try
                    {
                        int curNumber = 0;
                        int row = doc.Tables[1].Rows.Count;
                        while (rdr.Read())
                        {
                            curNumber++;
                            doc.Tables[1].Rows.Add();
                            row++;
                            doc.Tables[1].Rows[row].Cells[1].Range.Text = curNumber.ToString();
                            doc.Tables[1].Rows[row].Cells[2].Range.Text = rdr["surname"].ToString() + " " + rdr["name"].ToString();
                            doc.Tables[1].Rows[row].Cells[3].Range.Text = rdr["age"].ToString();
                            doc.Tables[1].Rows[row].Cells[4].Range.Text = rdr["qf"].ToString();
                            doc.Tables[1].Rows[row].Cells[5].Range.Text = sTeam;
                            if (curProgress < 90)
                            {
                                curProgress += progrStep;
                                bw.ReportProgress(curProgress);
                            }
                        }
                    }
                    finally { rdr.Close(); }
                    if (!String.IsNullOrEmpty(dirToSave))
                    {
                        string fileName = Path.Combine(dirToSave, currentIid.ToString(idTemplate) + "_" + sTeam + ".doc");
                        if (File.Exists(fileName))
                            File.Delete(fileName);
                        object format = Word.WdSaveFormat.wdFormatDocument97;
                        object fName = fileName;

                        Type t = doc.GetType();

                        MethodInfo saveMethod = t.GetMethod("SaveAs");
                        

                        try
                        {
                            var parameters = saveMethod.GetParameters();
                            object[] ps = new object[parameters.Length];
                            for (int i = 0; i < ps.Length; i++)
                                ps[i] = Type.Missing;
                            if (ps.Length > 1)
                            {
                                ps[0] = fName;
                                ps[1] = format;
                            }
                            saveMethod.Invoke(doc, ps);

                            //doc.SaveAs2(ref fName, ref format);
                            ((Word._Application)app).Quit();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка сохранения документа:\r\n\r\n" +
                                ex.ToString());
                            app.Visible = true;
                        }
                    }
                }

                bw.ReportProgress(100);
                return app as T;
            }
        }
    }
}
