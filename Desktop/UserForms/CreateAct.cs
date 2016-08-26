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
    /// Форма для создания актов готовности трасс. Акты выкидываются в Excel (хорошо бы переделать на Word
    /// и хранить введённые данные)
    /// </summary>
    public partial class CreateAct : BaseForm
    {
        //SqlConnection cn;
        string judge = "", setter = "", name = "", cName = "", president = "", safety = "", chief = "",
            cStyle = "", gName = "", round = "";
        public CreateAct(SqlConnection baseCon, int listID)
            : base(baseCon, "")
        {
            InitializeComponent();
            //this.cn = cn;
            try
            {
                //if (cn.State != ConnectionState.Open)
                //    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = this.cn;
                cmd.CommandText = "SELECT l.judge_id, l.routesetter_id, l.style, l.round, g.name " +
                                  "  FROM lists l(NOLOCK) " +
                                  "  JOIN groups g(NOLOCK) ON g.iid = l.group_id " +
                                  " WHERE l.iid = " + listID.ToString();
                SqlDataReader rdr = cmd.ExecuteReader();
                int jID = -1, rID = -1;
                while (rdr.Read())
                {
                    try { jID = Convert.ToInt32(rdr["judge_id"]); }
                    catch { judge = ""; }
                    try { rID = Convert.ToInt32(rdr["routesetter_id"]); }
                    catch { setter = ""; }
                    name = rdr["style"].ToString() + ", " + rdr["name"].ToString() + ", " + rdr["round"].ToString();
                    cStyle = rdr["style"].ToString();
                    gName = rdr["name"].ToString();
                    round = rdr["round"].ToString();
                    break;
                }
                rdr.Close();
                if (jID > 0)
                    judge = StaticClass.GetJudgeData(jID, cn, false);
                if (rID > 0)
                    setter = StaticClass.GetJudgeData(rID, cn, false);
                tbName.Text = name;
                cmd.CommandText = "SELECT NAME FROM CompetitionData(NOLOCK)";
                try { cName = cmd.ExecuteScalar().ToString(); }
                catch { cName = ""; }

                cmd.CommandText = "SELECT iid FROM judgeView WHERE pos = 'Главный судья'";
                try { president = StaticClass.GetJudgeData(Convert.ToInt32(cmd.ExecuteScalar()), cn, false); }
                catch { president = ""; }

                cmd.CommandText = "SELECT iid FROM judgeView WHERE pos = 'Зам. гл.судьи по безопасности'";
                try { safety = StaticClass.GetJudgeData(Convert.ToInt32(cmd.ExecuteScalar()), cn, false); }
                catch { safety = ""; }

                cmd.CommandText = "SELECT iid FROM judgeView WHERE pos = 'Зам. гл.судьи по трассам'";
                try { chief = StaticClass.GetJudgeData(Convert.ToInt32(cmd.ExecuteScalar()), cn, false); }
                catch { chief = ""; }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Excel.Application xlApp;
            Excel.Workbook wb;
            Excel.Worksheet ws;
            Excel.Range range;
            Excel.Style style;
            if (!StaticClass.LaunchExcel(out xlApp, out wb, out ws, false, cn))
                return;
            try
            {
                try
                {
                    style = wb.Styles.Add("center_no_border", Type.Missing);
                    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    style.VerticalAlignment = Excel.XlVAlign.xlVAlignBottom;
                    style.Font.Size = 10;
                    style.Font.Name = "Arial";
                    style.Font.Bold = false;
                }
                catch { }

                try
                {
                    style = wb.Styles.Add("center_no_border_small", Type.Missing);
                    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    style.VerticalAlignment = Excel.XlVAlign.xlVAlignBottom;
                    style.Font.Size = 8;
                    style.Font.Name = "Arial";
                    style.Font.Bold = false;
                }
                catch { }

                try
                {
                    style = wb.Styles.Add("left_no_border", Type.Missing);
                    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    style.VerticalAlignment = Excel.XlVAlign.xlVAlignBottom;
                    style.WrapText = true;
                    style.Font.Size = 10;
                    style.Font.Name = "Arial";
                    style.Font.Bold = false;
                }
                catch { }

                try
                {
                    style = wb.Styles.Add("center_bold_no_border", Type.Missing);
                    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    style.Font.Size = 12;
                    style.Font.Name = "Arial";
                    style.Font.Bold = true;
                    style.VerticalAlignment = Excel.XlVAlign.xlVAlignBottom;
                }
                catch { }

                ws.PageSetup.Orientation = Excel.XlPageOrientation.xlPortrait;
                ws.Cells[1, 3] = "УТВЕРЖДАЮ";
                ws.Cells[2, 3] = "Главный судья";
                string month = "";
                switch (DateTime.Now.Month)
                {
                    case 1:
                        month = "января";
                        break;
                    case 2:
                        month = "февраля";
                        break;
                    case 3:
                        month = "марта";
                        break;
                    case 4:
                        month = "апреля";
                        break;
                    case 5:
                        month = "мая";
                        break;
                    case 6:
                        month = "июня";
                        break;
                    case 7:
                        month = "июля";
                        break;
                    case 8:
                        month = "августа";
                        break;
                    case 9:
                        month = "сентября";
                        break;
                    case 10:
                        month = "октября";
                        break;
                    case 11:
                        month = "ноября";
                        break;
                    case 12:
                        month = "декабря";
                        break;
                }
                ws.Cells[3, 3] = "\"" + DateTime.Now.Day + "\" " + month + " " + DateTime.Now.Year.ToString() + " г.";
                if (president == "")
                    ws.Cells[4, 3] = "_____________________________";
                else
                    ws.Cells[4, 3] = president;

                range = ws.get_Range(ws.Cells[1, 3], ws.Cells[4, 3]);
                range.Style = "center_no_border";
                range.ColumnWidth = 29.43;
                range.RowHeight = 12.75;

                range = ws.get_Range(ws.Cells[1, 1], ws.Cells[1, 1]);
                range.ColumnWidth = 27.71;

                range = ws.get_Range(ws.Cells[1, 2], ws.Cells[1, 2]);
                range.ColumnWidth = 26.29;

                range = ws.get_Range(ws.Cells[5, 1], ws.Cells[5, 3]);
                range.Style = "center_bold_no_border";
                range.Merge(Type.Missing);
                range.RowHeight = 41.25;
                range.Cells[1, 1] = "АКТ ГОТОВНОСТИ";

                range = ws.get_Range(ws.Cells[6, 1], ws.Cells[6, 3]);
                range.Style = "center_no_border";
                range.Merge(Type.Missing);
                range.RowHeight = 12.75;
                if (cName == "")
                    range.Cells[1, 1] =
                        "( __________________________________________________________________________________ )";
                else
                    range.Cells[1, 1] = "(" + cName + ")";

                range = ws.get_Range(ws.Cells[7, 1], ws.Cells[7, 3]);
                range.Style = "left_no_border";
                range.Merge(Type.Missing);
                range.RowHeight = 25.50;
                if (tbName.Text == "")
                    range.Cells[1, 1] = "трассы ______________________________________________________________________________";
                else
                    range.Cells[1, 1] = "трассы " + tbName.Text;

                range = ws.get_Range(ws.Cells[8, 1], ws.Cells[8, 3]);
                range.Style = "center_no_border_small";
                range.Merge(Type.Missing);
                range.RowHeight = 12.75;
                range.Cells[1, 1] = "(наименование вида/раунда)";

                range = ws.get_Range(ws.Cells[9, 1], ws.Cells[9, 3]);
                range.Style = "left_no_border";
                range.Merge(Type.Missing);
                range.RowHeight = 25.50;
                if (tbCat.Text == "")
                    range.Cells[1, 1] = "категория трудности: __________________________________________________________________";
                else
                    range.Cells[1, 1] = "категория трудности: " + tbCat.Text;

                range = ws.get_Range(ws.Cells[10, 1], ws.Cells[10, 3]);
                range.Style = "center_no_border_small";
                range.Merge(Type.Missing);
                range.RowHeight = 12.75;
                range.Cells[1, 1] = "(наименование)";

                range = ws.get_Range(ws.Cells[11, 1], ws.Cells[11, 3]);
                range.Style = "left_no_border";
                range.Merge(Type.Missing);
                range.RowHeight = 25.50;
                range.Cells[1, 1] = "1. Трасса соответствует требованиям правил соревнований по скалолазанию для данного вида.";

                range = ws.get_Range(ws.Cells[12, 1], ws.Cells[12, 3]);
                range.Style = "left_no_border";
                range.Merge(Type.Missing);
                range.RowHeight = 12.75;
                range.Cells[1, 1] = "2. Состояние трассы позволяет обеспечить безопасность, организация страховки проверена.";

                range = ws.get_Range(ws.Cells[13, 1], ws.Cells[13, 3]);
                range.Style = "left_no_border";
                range.Merge(Type.Missing);
                range.RowHeight = 12.75;
                range.Cells[1, 1] = "3. Трасса оснащена необходимой маркировкой и стартовой линией.";

                range = ws.get_Range(ws.Cells[14, 1], ws.Cells[14, 3]);
                range.Style = "left_no_border";
                range.Merge(Type.Missing);
                range.RowHeight = 12.75;
                range.Cells[1, 1] = "4. Стартовая площадка обозначена.";

                range = ws.get_Range(ws.Cells[15, 1], ws.Cells[15, 3]);
                range.Style = "left_no_border";
                range.Merge(Type.Missing);
                range.RowHeight = 12.75;
                if (tbMaxD.Text == "")
                    range.Cells[1, 1] = "5. Максимальное расстояние между нижними карабинами соседних оттяжек: _______ м.";
                else
                    range.Cells[1, 1] = "5. Максимальное расстояние между нижними карабинами соседних оттяжек: " + tbMaxD.Text + " м.";

                range = ws.get_Range(ws.Cells[16, 1], ws.Cells[16, 3]);
                range.Style = "left_no_border";
                range.Merge(Type.Missing);
                range.RowHeight = 12.75;
                if (tbQuickdraws.Text == "")
                    range.Cells[1, 1] = "6. Число оттяжек: _______";
                else
                    range.Cells[1, 1] = "6. Число оттяжек: " + tbQuickdraws.Text + ".";

                range = ws.get_Range(ws.Cells[17, 1], ws.Cells[17, 3]);
                range.Style = "left_no_border";
                range.Merge(Type.Missing);
                range.RowHeight = 12.75;
                range.Cells[1, 1] = "7. Параметры трассы:";

                range = ws.get_Range(ws.Cells[18, 1], ws.Cells[18, 3]);
                range.Style = "left_no_border";
                range.Merge(Type.Missing);
                range.RowHeight = 12.75;
                if (tbHeight.Text == "")
                    range.Cells[1, 1] = "     а) высота стены: _______ м.";
                else
                    range.Cells[1, 1] = "     а) высота стены: " + tbHeight.Text + " м.";

                range = ws.get_Range(ws.Cells[19, 1], ws.Cells[19, 3]);
                range.Style = "left_no_border";
                range.Merge(Type.Missing);
                range.RowHeight = 12.75;
                if (tbOverhang.Text == "")
                    range.Cells[1, 1] = "     б) нависание: _______ м.";
                else
                    range.Cells[1, 1] = "     б) нависание: " + tbOverhang.Text + " м.";

                range = ws.get_Range(ws.Cells[20, 1], ws.Cells[20, 3]);
                range.Style = "left_no_border";
                range.Merge(Type.Missing);
                range.RowHeight = 12.75;
                if (tbLength.Text == "")
                    range.Cells[1, 1] = "     в) протяжённость: _______ м.";
                else
                    range.Cells[1, 1] = "     в) протяжённость: " + tbLength.Text + " м.";

                range = ws.get_Range(ws.Cells[21, 1], ws.Cells[21, 3]);
                range.Style = "left_no_border";
                range.Merge(Type.Missing);
                range.RowHeight = 12.75;
                if (tbMoves.Text == "")
                    range.Cells[1, 1] = "     г) число перехватов: _______";
                else
                    range.Cells[1, 1] = "     г) число перехватов: " + tbMoves.Text + ".";

                range = ws.get_Range(ws.Cells[22, 1], ws.Cells[22, 3]);
                range.Style = "left_no_border";
                range.Merge(Type.Missing);
                range.RowHeight = 25.50;
                if (tbTime.Text == "")
                    range.Cells[1, 1] = "8. Лучшее время прохождения трассы судьей-демонстратором до предъявления ее участникам " +
                    "(в соревнованиях на скорость): _______ сек.";
                else
                    range.Cells[1, 1] = "8. Лучшее время прохождения трассы судьей-демонстратором до предъявления ее участникам " +
                        "(в соревнованиях на скорость): " + tbTime.Text + " сек.";

                range = ws.get_Range(ws.Cells[23, 1], ws.Cells[23, 3]);
                range.Style = "left_no_border";
                range.Merge(Type.Missing);
                range.RowHeight = 25.50;
                range.Cells[1, 1] = "К акту прилагается схема трассы";

                range = ws.get_Range(ws.Cells[24, 1], ws.Cells[24, 2]);
                range.Style = "left_no_border";
                range.RowHeight = 25.50;
                range.Cells[1, 1] = "Начальник трассы";
                range.Cells[1, 2] = "__________________________";
                range = ws.get_Range(ws.Cells[24, 3], ws.Cells[24, 3]);
                range.Style = "center_no_border";
                if (setter == "")
                    range.Cells[1, 1] = "__________________________";
                else
                    range.Cells[1, 1] = setter;
                range = ws.get_Range(ws.Cells[25, 2], ws.Cells[25, 3]);
                range.RowHeight = 12.75;
                range.Style = "center_no_border_small";
                range.Cells[1, 1] = "(подпись)";
                range.Cells[1, 2] = "(Ф.И.О.)";

                range = ws.get_Range(ws.Cells[26, 1], ws.Cells[26, 1]);
                range.Style = "left_no_border";
                range.RowHeight = 25.50;
                range.Cells[1, 1] = "ТРАССУ СДАЛ:";
                range = ws.get_Range(ws.Cells[27, 1], ws.Cells[27, 2]);
                range.Style = "left_no_border";
                range.RowHeight = 12.75;
                range.Cells[1, 1] = "Зам. гл. судьи по трассам";
                range.Cells[1, 2] = "__________________________";
                range = ws.get_Range(ws.Cells[27, 3], ws.Cells[27, 3]);
                range.Style = "center_no_border";
                if (chief == "")
                    range.Cells[1, 1] = "__________________________";
                else
                    range.Cells[1, 1] = chief;
                range = ws.get_Range(ws.Cells[28, 2], ws.Cells[28, 3]);
                range.Style = "center_no_border_small";
                range.Cells[1, 1] = "(подпись)";
                range.Cells[1, 2] = "(Ф.И.О.)";

                range = ws.get_Range(ws.Cells[29, 1], ws.Cells[29, 1]);
                range.Style = "left_no_border";
                range.RowHeight = 25.50;
                range.Cells[1, 1] = "ТРАССУ ПРИНЯЛ:";
                range = ws.get_Range(ws.Cells[30, 1], ws.Cells[30, 2]);
                range.Style = "left_no_border";
                range.RowHeight = 12.75;
                range.Cells[1, 1] = "Зам. гл. судьи по виду";
                range.Cells[1, 2] = "__________________________";
                range = ws.get_Range(ws.Cells[30, 3], ws.Cells[30, 3]);
                range.Style = "center_no_border";
                if (judge == "")
                    range.Cells[1, 1] = "__________________________";
                else
                    range.Cells[1, 1] = judge;
                range = ws.get_Range(ws.Cells[31, 2], ws.Cells[31, 3]);
                range.Style = "center_no_border_small";
                range.Cells[1, 1] = "(подпись)";
                range.Cells[1, 2] = "(Ф.И.О.)";

                range = ws.get_Range(ws.Cells[32, 1], ws.Cells[32, 1]);
                range.Style = "left_no_border";
                range.RowHeight = 25.50;
                range.Cells[1, 1] = "ТРАССУ ПРИНЯЛ:";
                range = ws.get_Range(ws.Cells[33, 1], ws.Cells[33, 2]);
                range.Style = "left_no_border";
                range.RowHeight = 12.75;
                range.Cells[1, 1] = "Зам. гл. судьи по безопасности";
                range.Cells[1, 2] = "__________________________";
                range = ws.get_Range(ws.Cells[33, 3], ws.Cells[33, 3]);
                range.Style = "center_no_border";
                if (safety == "")
                    range.Cells[1, 1] = "__________________________";
                else
                    range.Cells[1, 1] = safety;
                range = ws.get_Range(ws.Cells[34, 2], ws.Cells[34, 3]);
                range.Style = "center_no_border_small";
                range.Cells[1, 1] = "(подпись)";
                range.Cells[1, 2] = "(Ф.И.О.)";

                string sheetName = "АКТ_";
                switch (cStyle)
                {
                    case "Трудность":
                        sheetName += "ТР_";
                        break;
                    case "Скорость":
                        sheetName += "СК_";
                        break;
                    case "Боулдеринг":
                        sheetName += "Б_";
                        break;
                }
                sheetName += StaticClass.CreateSheetName(gName, round);
                try { ws.Name = sheetName; }
                catch { }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка экспорта данных в Excel:\r\n" + ex.Message); }
            finally { StaticClass.SetExcelVisible(xlApp); }
        }
    }
}
