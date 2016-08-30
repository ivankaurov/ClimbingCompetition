// <copyright file="CombinedResults.cs">
// Copyright ¬© 2016 All Rights Reserved
// This file is part of ClimbingCompetition.
//
//  ClimbingCompetition is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  ClimbingCompetition is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with ClimbingCompetition.  If not, see <http://www.gnu.org/licenses/>.
//
// (–≠—Ç–æ—Ç —Ñ–∞–π–ª ‚Äî —á–∞—Å—Ç—å ClimbingCompetition.
// 
// ClimbingCompetition - —Å–≤–æ–±–æ–¥–Ω–∞—è –ø—Ä–æ–≥—Ä–∞–º–º–∞: –≤—ã –º–æ–∂–µ—Ç–µ –ø–µ—Ä–µ—Ä–∞—Å–ø—Ä–æ—Å—Ç—Ä–∞–Ω—è—Ç—å –µ–µ –∏/–∏–ª–∏
// –∏–∑–º–µ–Ω—è—Ç—å –µ–µ –Ω–∞ —É—Å–ª–æ–≤–∏—è—Ö –°—Ç–∞–Ω–¥–∞—Ä—Ç–Ω–æ–π –æ–±—â–µ—Å—Ç–≤–µ–Ω–Ω–æ–π –ª–∏—Ü–µ–Ω–∑–∏–∏ GNU –≤ —Ç–æ–º –≤–∏–¥–µ,
// –≤ –∫–∞–∫–æ–º –æ–Ω–∞ –±—ã–ª–∞ –æ–ø—É–±–ª–∏–∫–æ–≤–∞–Ω–∞ –§–æ–Ω–¥–æ–º —Å–≤–æ–±–æ–¥–Ω–æ–≥–æ –ø—Ä–æ–≥—Ä–∞–º–º–Ω–æ–≥–æ –æ–±–µ—Å–ø–µ—á–µ–Ω–∏—è;
// –ª–∏–±–æ –≤–µ—Ä—Å–∏–∏ 3 –ª–∏—Ü–µ–Ω–∑–∏–∏, –ª–∏–±–æ (–ø–æ –≤–∞—à–µ–º—É –≤—ã–±–æ—Ä—É) –ª—é–±–æ–π –±–æ–ª–µ–µ –ø–æ–∑–¥–Ω–µ–π
// –≤–µ—Ä—Å–∏–∏.
// 
// ClimbingCompetition —Ä–∞—Å–ø—Ä–æ—Å—Ç—Ä–∞–Ω—è–µ—Ç—Å—è –≤ –Ω–∞–¥–µ–∂–¥–µ, —á—Ç–æ –æ–Ω–∞ –±—É–¥–µ—Ç –ø–æ–ª–µ–∑–Ω–æ–π,
// –Ω–æ –ë–ï–ó–û –í–°–Ø–ö–ò–• –ì–ê–†–ê–ù–¢–ò–ô; –¥–∞–∂–µ –±–µ–∑ –Ω–µ—è–≤–Ω–æ–π –≥–∞—Ä–∞–Ω—Ç–∏–∏ –¢–û–í–ê–†–ù–û–ì–û –í–ò–î–ê
// –∏–ª–∏ –ü–†–ò–ì–û–î–ù–û–°–¢–ò –î–õ–Ø –û–ü–†–ï–î–ï–õ–ï–ù–ù–´–• –¶–ï–õ–ï–ô. –ü–æ–¥—Ä–æ–±–Ω–µ–µ —Å–º. –≤ –°—Ç–∞–Ω–¥–∞—Ä—Ç–Ω–æ–π
// –æ–±—â–µ—Å—Ç–≤–µ–Ω–Ω–æ–π –ª–∏—Ü–µ–Ω–∑–∏–∏ GNU.
// 
// –í—ã –¥–æ–ª–∂–Ω—ã –±—ã–ª–∏ –ø–æ–ª—É—á–∏—Ç—å –∫–æ–ø–∏—é –°—Ç–∞–Ω–¥–∞—Ä—Ç–Ω–æ–π –æ–±—â–µ—Å—Ç–≤–µ–Ω–Ω–æ–π –ª–∏—Ü–µ–Ω–∑–∏–∏ GNU
// –≤–º–µ—Å—Ç–µ —Å —ç—Ç–æ–π –ø—Ä–æ–≥—Ä–∞–º–º–æ–π. –ï—Å–ª–∏ —ç—Ç–æ –Ω–µ —Ç–∞–∫, —Å–º. <http://www.gnu.org/licenses/>.)
// </copyright>
// <author>Ivan Kaurov</author>
// <date>30.08.2016 23:51</date>


using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;

namespace ClimbingCompetition
{
    /// <summary>
    /// ‘ÓÏ‡ ‰Îˇ ÔÓ‰Ò˜∏Ú‡ ÂÁÛÎ¸Ú‡ÚÓ‚ ÏÌÓ„Ó·Ó¸ˇ
    /// </summary>
    public class CombinedResults
#if FULL
        :TeamResults
#endif
    {
#if FULL
        protected int list_id, group_id=-1;
        protected string gName = "";
        protected ListElem[] res;
#endif
        public class ListElem
        {
#if FULL
            public string[] data = null;
            public double[] pos = null;
            public double minPos
            {
                get
                {
                    List<double> pTmp = new List<double>();
                    foreach (double d in pos)
                        pTmp.Add(d);
                    pTmp.Sort();
                    return (double)pTmp[0];
                }
            }
            public double diff
            {
                get
                {
                    double max = pos[0];
                    for (int i = 1; i < pos.Length; i++)
                        if (pos[i] > max)
                            max = pos[i];
                    return max - minPos;
                }
            }
            public double sum
            {
                get
                {
                    double res = 0.0;
                    for (int i = 0; i < pos.Length; i++)
                        res += pos[i];
                    return res;
                }
            }
            public int ps = -1;
            public int iid = -1;
#endif
        }
#if FULL
        public CombinedResults(int group_id, SqlConnection baseCon, string competitionTitle, int list_id)
            : base(baseCon, competitionTitle)
        {
            this.list_id = list_id;
            this.group_id = group_id;
            //this.cn = cn;
            SqlCommand cmd = new SqlCommand("SELECT name FROM groups WHERE iid=" + group_id.ToString(), this.cn);

            try
            { gName = cmd.ExecuteScalar().ToString(); }
            catch (Exception ex) { MessageBox.Show(ex.Message); return; }
            RefreshAll();
            this.Text = "ÃÌÓ„Ó·Ó¸Â " + gName;
        }

        protected override void RefreshAll()
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "SELECT iid,style FROM lists(NOLOCK) WHERE (group_id=" + group_id.ToString() +
                    ") AND (round='»ÚÓ„Ó‚˚È ÔÓÚÓÍÓÎ') AND (style <> 'ÃÌÓ„Ó·Ó¸Â') AND (style NOT LIKE ' ÓÏ‡Ì‰%') ORDER BY iid";

                SqlDataReader rdr = cmd.ExecuteReader();
                generalResults.Clear();
                while (rdr.Read())
                {
                    styleRes stlr = new styleRes();
                    stlr.res = Convert.ToDouble(rdr[0]);
                    stlr.style = rdr[1].ToString();
                    stlr.dt = null;
                    generalResults.Add(stlr);
                }
                rdr.Close();
                if (generalResults.Count < 1)
                    throw new Exception("»ÚÓ„Ó‚˚Â ÔÓÚÓÍÓÎ˚ ÌÂ ÒÓÁ‰‡Ì˚");

                for (int i = 0; i < generalResults.Count; i++)
                {
                    styleRes stl = (styleRes)generalResults[i];
                    ShowFinalResults sfr = new ShowFinalResults("NOT_FOR_SHOW", (int)(stl.res + 0.1), cn);
                    stl.dt = sfr.getTable;
                    if (stl.dt.Columns.IndexOf("‘‡ÏËÎËˇ»Ïˇ") > -1)
                        stl.dt.Columns["‘‡ÏËÎËˇ»Ïˇ"].ColumnName = "‘‡ÏËÎËˇ, »Ïˇ";
                    generalResults[i] = stl;
                }
                CreateTables();
                res = GetResults();
                FillTable();
                try
                {
                    if (dtRes.Columns.IndexOf("iid") > -1)
                        dtRes.Columns.Remove("iid");
                }
                catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Œ¯Ë·Í‡ ÔË ÒÓÁÓ‰‡ÌËË ÔÓÚÓÍÓÎ‡:\r\n" + ex.Message);
            }
        }

        void CreateTables()
        {
            foreach (styleRes stl in generalResults)
            {
                int i = 0;
                while (i < stl.dt.Rows.Count)
                {
                    DataRow rdr = stl.dt.Rows[i];
                    if (rdr["ÃÂÒÚÓ"].ToString() == "‚/Í" || rdr["ÃÂÒÚÓ"].ToString() == "" ||
                        rdr["ÃÂÒÚÓ"].ToString() == "‰ËÒÍ‚." || rdr["ÃÂÒÚÓ"].ToString() == "Ì/ˇ")
                        stl.dt.Rows.RemoveAt(i);
                    else
                        i++;
                }

                //int curPos = Convert.ToInt32(stl.dt.Rows[0]["ÃÂÒÚÓ"]);
                //int nxtPos = curPos + 1;

                stl.dt.Columns.Add("posPt", typeof(double));
                int rowStart = -1, curPos = -1;
                for (i = 0; i < stl.dt.Rows.Count; i++)
                {
                    try
                    {
                        curPos = Convert.ToInt32(stl.dt.Rows[i]["ÃÂÒÚÓ"]);
                        rowStart = i;
                        break;
                    }
                    catch
                    {
                        stl.dt.Rows[i]["posPt"] = 0.0;
                    }
                }
                if (rowStart > -1)
                {
                    int posQuan = 1;
                    int posRowStart = rowStart;
                    int nPos;
                    for (i = rowStart + 1; i < stl.dt.Rows.Count; i++)
                    {
                        try { nPos = Convert.ToInt32(stl.dt.Rows[i]["ÃÂÒÚÓ"]); }
                        catch { continue; }
                        if (nPos == curPos)
                            posQuan++;
                        else
                        {
                            double dPosToSet = (double)(curPos + curPos + posQuan - 1) / 2.0;
                            for (int k = posRowStart; k < i; k++)
                                stl.dt.Rows[k]["posPt"] = dPosToSet;
                            curPos = nPos;
                            posRowStart = i;
                            posQuan = 1;
                        }
                    }
                    double dPToSet = (double)(curPos + curPos + posQuan - 1) / 2.0;
                    for (int k = posRowStart; k < i; k++)
                        stl.dt.Rows[k]["posPt"] = dPToSet;
                }

                //i = 1;
                //for (; i < stl.dt.Rows.Count; i++)
                //{
                //    try { nxtPos = Convert.ToInt32(stl.dt.Rows[i]["ÃÂÒÚÓ"]); }
                //    catch { continue; }
                //    if (nxtPos != curPos)
                //    {
                //        double dPos = (double)(nxtPos + curPos - 1) / 2.0;
                //        for (int k = curPos - 1; k < i; k++)
                //            stl.dt.Rows[k]["posPt"] = dPos;
                //        curPos = nxtPos;
                //    }
                //}
                //nxtPos = stl.dt.Rows.Count;
                //double ddPos = (double)(nxtPos + curPos) / 2.0;
                //for (int k = curPos - 1; k < stl.dt.Rows.Count; k++)
                //    stl.dt.Rows[k]["posPt"] = ddPos;
            }
        }

        ListElem[] GetResults()
        {
            List<ListElem> styles = new List<ListElem>();
            foreach (DataRow drd in ((styleRes)generalResults[0]).dt.Rows)
            {
                ListElem le = new ListElem();
                le.data = new string[4];
                le.data[0] = drd["‘‡ÏËÎËˇ, »Ïˇ"].ToString();
                le.data[1] = drd["√.."].ToString();
                le.data[2] = drd["–‡Áˇ‰"].ToString();
                le.data[3] = drd[" ÓÏ‡Ì‰‡"].ToString();
                le.pos = new double[generalResults.Count];
                try { le.pos[0] = (double)drd["posPt"]; }
                catch { continue; }
                for (int k = 1; k < le.pos.Length; k++)
                    le.pos[k] = -1.0;
                le.ps = 0;
                int iid = Convert.ToInt32(drd["π"]);
                le.iid = iid;
                for (int k = 1; k < generalResults.Count; k++)
                {
                    foreach (DataRow drr in ((styleRes)generalResults[k]).dt.Rows)
                        if ((int)drr["π"] == iid)
                        {
                            le.pos[k] = (double)drr["posPt"];
                            break;
                        }
                }
                bool b = true;
                for (int k = 0; k < le.pos.Length; k++)
                    if (le.pos[k] < 0.0)
                    {
                        b = false;
                        break;
                    }
                if(b)
                    styles.Add(le);
            }
            bool newRules = SettingsForm.GetCombinedNew(cn);
            if (newRules)
            {
                if (styles.Count > 0)
                    for (int i = 0; i < generalResults.Count; i++)
                    {
                        styles.Sort((a, b) => a.pos[i].CompareTo(b.pos[i]));
                        double cPts = styles[0].pos[i];
                        double curPos = 1.0;
                        int cpCount = 1;
                        double[] newPos = new double[styles.Count];
                        newPos[0] = 1.0;
                        for (int k = 1; k < newPos.Length; k++)
                        {
                            if (styles[k].pos[i].Equals(cPts))
                                cpCount++;
                            else
                            {
                                if (cpCount > 1)
                                {
                                    double ptsToSet = (curPos + (double)k) / 2.0;
                                    for (int j = k - cpCount; j < k; j++)
                                        newPos[j] = ptsToSet;
                                }
                                cpCount = 1;
                                curPos = (double)(k + 1);
                                cPts = styles[k].pos[i];
                            }
                            newPos[k] = curPos;
                        }
                        if (cpCount > 1)
                        {
                            int sc = styles.Count;
                            double pSet = (curPos + (double)sc) / 2.0;
                            for (int j = sc - cpCount; j < sc; j++)
                                newPos[j] = pSet;
                        }

                        for (int k = 0; k < newPos.Length; k++)
                            styles[k].pos[i] = newPos[k];
                    }
            }

            styles.Sort(new LEComparer(newRules));

            ListElem[] res = new ListElem[styles.Count];
            for (int i = 0; i < res.Length; i++)
                res[i] = (ListElem)styles[i];

            long curRes = (long)(res[0].minPos*10.0 + res[0].sum * 10000.0);
            int curPts = 1;
            res[0].ps = curPts;
            for (int i = 1; i < res.Length; i++)
            {
                long nxtRes = (long)(res[i].minPos * 10.0 + res[i].sum * 10000.0);
                if (curRes != nxtRes)
                {
                    curPts = i + 1;
                    curRes = nxtRes;
                }
                res[i].ps = curPts;
            }

            return res;
        }

        void FillTable()
        {
            dtRes.Clear();
            dtRes.Columns.Clear();
            //DataTable dtRes = new DataTable();
            dtRes.Columns.Add("ÃÂÒÚÓ", typeof(int));
            dtRes.Columns.Add("‘‡ÏËÎËˇ, »Ïˇ", typeof(string));
            dtRes.Columns.Add("√..", typeof(string));
            dtRes.Columns.Add("–‡Áˇ‰", typeof(string));
            dtRes.Columns.Add(" ÓÏ‡Ì‰‡", typeof(string));
            foreach (styleRes stl in generalResults)
                dtRes.Columns.Add(stl.style, typeof(double));
            dtRes.Columns.Add("—ÛÏÏ‡", typeof(double));
            dtRes.Columns.Add("iid", typeof(int));
            foreach (ListElem le in res)
            {
                object[] obj = new object[7 + generalResults.Count];
                obj[0] = le.ps;
                for (int i = 0; i < le.data.Length; i++)
                    obj[i + 1] = le.data[i];
                for (int i = 0; i < le.pos.Length; i++)
                    obj[i + 5] = le.pos[i];
                obj[obj.Length - 2] = le.sum;
                obj[obj.Length - 1] = le.iid;
                dtRes.Rows.Add(obj);
            }
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM generalResults WHERE list_id = " + list_id.ToString(), cn);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO generalResults(list_id, climber_id, pts, pos) " +
                                  "       VALUES (" + list_id.ToString() + ",@c,0,@p)";
                cmd.Parameters.Add("@c", SqlDbType.Int);
                cmd.Parameters.Add("@p", SqlDbType.Int);
                foreach (DataRow dr in dtRes.Rows)
                {
                    cmd.Parameters[0].Value = Convert.ToInt32(dr["iid"]);
                    try { cmd.Parameters[1].Value = Convert.ToInt32(dr["ÃÂÒÚÓ"]); }
                    catch { cmd.Parameters[1].Value = DBNull.Value; }
                    cmd.ExecuteNonQuery();
                    string sCol = dr[" ÓÏ‡Ì‰‡"] as string;
                    if (!String.IsNullOrEmpty(sCol) && sCol.Length >= 4)
                        if (sCol.Substring(sCol.Length - 4).Equals(" (À)"))
                            dr[" ÓÏ‡Ì‰‡"] = sCol.Substring(0, sCol.Length - 4);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, this.Text); }
            //return dtRes;
        }

        protected override void ExcelExport()
        {
            Excel.Application xlApp;
            Excel.Workbook wb;
            Excel.Worksheet ws;
            if (!StaticClass.LaunchExcel(out xlApp, out wb, out ws, true, cn))
                return;
            try
            {
                int startRow = 6;
                int lastRowMedals = -1, lastRowTot = -1;
                char stp = 'A';
                stp--;
                for (int k = 0; k < dtRes.Columns.Count; k++)
                {
                    if (dtRes.Columns[k].ColumnName == "iid")
                        continue;
                    ws.Cells[startRow, k + 1] = dtRes.Columns[k].ColumnName;
                    stp++;
                }
                int iRow = startRow;
                string strTmp;
                foreach (DataRow rdr in dtRes.Rows)
                {
                    iRow++;
                    int iColumn = 1;
                    foreach (DataColumn dc in dtRes.Columns)
                    {
                        if (dc.ColumnName == "iid")
                            continue;
                        strTmp = rdr[dc].ToString();
                        if (strTmp != "0")
                            ws.Cells[iRow, iColumn] = strTmp;

                        if (iColumn == 1)
                        {
                            int nTmpp;
                            try
                            {
                                if (int.TryParse(rdr[dc].ToString(), out nTmpp))
                                    if (nTmpp <= 3)
                                        lastRowMedals = iRow;
                            }
                            catch { }
                        }

                        iColumn++;
                    }
                    lastRowTot = iRow;
                }
                Excel.Range dt = ws.get_Range("A" + startRow.ToString(), stp + iRow.ToString());
                dt.Style = "MyStyle";
                dt.Columns.AutoFit();
                dt = ws.get_Range("B" + startRow.ToString(), "B" + iRow.ToString());
                dt.Style = "StyleLA";

                dt = ws.get_Range("A1", stp + "1");
                dt.Style = "CompTitle";
                dt.Merge(Type.Missing);
                strTmp = "";
                dt.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[0];
                ws.Cells[2, 1] = StaticClass.ParseCompTitle(competitionTitle)[1];
                //try
                //{
                //    style = wb.Styles.Add("StyleRA", Type.Missing);
                //    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                //}
                //catch { }
                dt = ws.get_Range(stp + "2", stp + "2");
                dt.Style = "StyleRA";
                dt.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[2];
                //try
                //{
                //    style = wb.Styles.Add("Title", Type.Missing);
                //    style.Font.Bold = true;
                //    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                //    style.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                //}
                //catch { }

                dt = ws.get_Range("A4", stp + "4");
                dt.Style = "Title";
                dt.Merge(Type.Missing);
                if (generalResults.Count < 3)
                    strTmp = "ƒ‚ÓÂ·Ó¸Â. " + gName;
                else
                    strTmp = "ÃÌÓ„Ó·Ó¸Â. " + gName;
                dt.Cells[1, 1] = strTmp;

                strTmp = "»“Œ√Œ¬€… œ–Œ“Œ ŒÀ –≈«”À‹“¿“Œ¬";
                dt = ws.get_Range("A3", stp + "3");
                dt.Style = "Title";
                dt.Merge(Type.Missing);
                dt.Cells[1, 1] = strTmp;
                strTmp = "ÃÕ_";

                if (lastRowMedals > startRow)
                {
                    ws.get_Range(ws.Cells[startRow + 1, 1], ws.Cells[lastRowMedals, 255]).Font.Bold = true;
                    ws.get_Range(ws.Cells[startRow, 1], ws.Cells[lastRowTot, 255]).Columns.AutoFit();
                }

                try { ws.Name = strTmp + StaticClass.CreateSheetName(gName, ""); }
                catch { }
            }
            catch (Exception ex) { MessageBox.Show("Œ¯Ë·Í‡ ˝ÍÒÔÓÚ‡ ‰‡ÌÌ˚ı ‚ Excel:\r\n" + ex.Message); }
            finally { StaticClass.SetExcelVisible(xlApp); }
        }

        private void InitializeComponent()
        {
            ((System.ComponentModel.ISupportInitialize)(this.dtRes)).BeginInit();
            this.SuspendLayout();
            // 
            // CombinedResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(597, 420);
            this.Name = "CombinedResults";
            ((System.ComponentModel.ISupportInitialize)(this.dtRes)).EndInit();
            this.ResumeLayout(false);

        }

#endif
    }

    class LEComparer
#if FULL
        : IComparer<CombinedResults.ListElem>
#endif
    {
#if FULL
        #region IComparer Members

        private bool useNewRules;

        public LEComparer(bool useNewRules)
        {
            this.useNewRules = useNewRules;
        }

        public int Compare(CombinedResults.ListElem leX, CombinedResults.ListElem leY)
        {
            if (leX.sum != leY.sum)
                return leX.sum.CompareTo(leY.sum);
            if (useNewRules && leX.diff != leY.diff)
                return leX.diff.CompareTo(leY.diff);
            return leX.minPos.CompareTo(leY.minPos);
        }

        #endregion
#endif
    }
}
