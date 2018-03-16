// <copyright file="StaticClass.cs">
// Copyright В© 2016 All Rights Reserved
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
// (Р­С‚РѕС‚ С„Р°Р№Р» вЂ” С‡Р°СЃС‚СЊ ClimbingCompetition.
// 
// ClimbingCompetition - СЃРІРѕР±РѕРґРЅР°СЏ РїСЂРѕРіСЂР°РјРјР°: РІС‹ РјРѕР¶РµС‚Рµ РїРµСЂРµСЂР°СЃРїСЂРѕСЃС‚СЂР°РЅСЏС‚СЊ РµРµ Рё/РёР»Рё
// РёР·РјРµРЅСЏС‚СЊ РµРµ РЅР° СѓСЃР»РѕРІРёСЏС… РЎС‚Р°РЅРґР°СЂС‚РЅРѕР№ РѕР±С‰РµСЃС‚РІРµРЅРЅРѕР№ Р»РёС†РµРЅР·РёРё GNU РІ С‚РѕРј РІРёРґРµ,
// РІ РєР°РєРѕРј РѕРЅР° Р±С‹Р»Р° РѕРїСѓР±Р»РёРєРѕРІР°РЅР° Р¤РѕРЅРґРѕРј СЃРІРѕР±РѕРґРЅРѕРіРѕ РїСЂРѕРіСЂР°РјРјРЅРѕРіРѕ РѕР±РµСЃРїРµС‡РµРЅРёСЏ;
// Р»РёР±Рѕ РІРµСЂСЃРёРё 3 Р»РёС†РµРЅР·РёРё, Р»РёР±Рѕ (РїРѕ РІР°С€РµРјСѓ РІС‹Р±РѕСЂСѓ) Р»СЋР±РѕР№ Р±РѕР»РµРµ РїРѕР·РґРЅРµР№
// РІРµСЂСЃРёРё.
// 
// ClimbingCompetition СЂР°СЃРїСЂРѕСЃС‚СЂР°РЅСЏРµС‚СЃСЏ РІ РЅР°РґРµР¶РґРµ, С‡С‚Рѕ РѕРЅР° Р±СѓРґРµС‚ РїРѕР»РµР·РЅРѕР№,
// РЅРѕ Р‘Р•Р—Рћ Р’РЎРЇРљРРҐ Р“РђР РђРќРўРР™; РґР°Р¶Рµ Р±РµР· РЅРµСЏРІРЅРѕР№ РіР°СЂР°РЅС‚РёРё РўРћР’РђР РќРћР“Рћ Р’РР”Рђ
// РёР»Рё РџР РР“РћР”РќРћРЎРўР Р”Р›РЇ РћРџР Р•Р”Р•Р›Р•РќРќР«РҐ Р¦Р•Р›Р•Р™. РџРѕРґСЂРѕР±РЅРµРµ СЃРј. РІ РЎС‚Р°РЅРґР°СЂС‚РЅРѕР№
// РѕР±С‰РµСЃС‚РІРµРЅРЅРѕР№ Р»РёС†РµРЅР·РёРё GNU.
// 
// Р’С‹ РґРѕР»Р¶РЅС‹ Р±С‹Р»Рё РїРѕР»СѓС‡РёС‚СЊ РєРѕРїРёСЋ РЎС‚Р°РЅРґР°СЂС‚РЅРѕР№ РѕР±С‰РµСЃС‚РІРµРЅРЅРѕР№ Р»РёС†РµРЅР·РёРё GNU
// РІРјРµСЃС‚Рµ СЃ СЌС‚РѕР№ РїСЂРѕРіСЂР°РјРјРѕР№. Р•СЃР»Рё СЌС‚Рѕ РЅРµ С‚Р°Рє, СЃРј. <http://www.gnu.org/licenses/>.)
// </copyright>
// <author>Ivan Kaurov</author>
// <date>30.08.2016 23:51</date>

//#if DEBUG
//#define RECREATE_FUNCTIONS
//#endif

//#define DOWNLOAD

//#if DEBUG
//#undef DEBUG
//#endif
//#define CFR

using ClimbingCompetition.dsClimbingTableAdapters;
using ClimbingCompetition.Online;
using ClimbingCompetition.WebServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using XmlApiClient;
using XmlApiData;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
using ClimbingCompetition.SpeedData;


namespace ClimbingCompetition
{
    public enum ClimbingStyle { Lead, Speed, Bouldering }

    /// <summary>
    /// Статические методы
    /// </summary>
    public static class StaticClass
    {
        public static void ShowExceptionMessageBox(String message, Exception ex = null, Form owner = null)
        {
            String showMsg = (ex == null) ? message : String.Format("{0}:{1}{2}", message, Environment.NewLine, ex.Message);
            var parent = (owner == null || owner.IsDisposed) ? null : owner;
            if (parent == null)
                MessageBox.Show(showMsg);
            else
            {
                if (parent.InvokeRequired)
                    parent.Invoke(new EventHandler(delegate { MessageBox.Show(parent, showMsg); }));
                else
                    MessageBox.Show(parent, showMsg);
            }
        }
        public static void LoadInitialStrings(string ResourceFileRootName, object o, CultureInfo ci = null)
        {
            if (ci == null)
                ci = Thread.CurrentThread.CurrentUICulture;
            Type t = o.GetType();
            ResourceManager rm = new ResourceManager(ResourceFileRootName, t.Assembly);

            foreach (var v in t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
            {
                string val = rm.GetString(v.Name, ci);
                if (val != null)
                {
                    if (val.Contains("%NEWLINE%"))
                        val = val.Replace("%NEWLINE%", "\r\n");
                    try { v.SetValue(o, val); }
                    catch { }
                }
            }
        }

        public static void ApplyResourceToControl(Object o, ComponentResourceManager crm, CultureInfo ci = null)
        {
            Type t = o.GetType();

            var pi = t.GetProperty("Name");
            if (pi == null)
                return;
            string name = pi.GetValue(o, null) as string;
            if (name == null)
                return;

            if (ci == null)
                ci = Thread.CurrentThread.CurrentUICulture;

            pi = t.GetProperty("Controls");
            IEnumerable cArray;
            ArrayConverter ac = new ArrayConverter();
            if (pi != null)
            {
                
                object oTmp = pi.GetValue(o, null);
                cArray = oTmp as IEnumerable;
                if (cArray != null)
                    foreach (var ctr in cArray)
                        ApplyResourceToControl(ctr, crm, ci);
            }

            pi = t.GetProperty("Items");
            if (pi != null)
            {
                cArray = pi.GetValue(o, null) as IEnumerable;
                if (cArray != null)
                    foreach (var ctr in cArray)
                        ApplyResourceToControl(ctr, crm, ci);
            }

            pi = t.GetProperty("DropDownItems");
            if (pi != null)
            {
                cArray = pi.GetValue(o, null) as IEnumerable;
                if (cArray != null)
                    foreach (var ctr in cArray)
                        ApplyResourceToControl(ctr, crm, ci);
            }
            pi = t.GetProperty("MdiChildren");
            if (pi != null)
            {
                cArray = pi.GetValue(o, null) as IEnumerable;
                if (cArray != null)
                    foreach (var ctr in cArray)
                        ApplyResourceToControl(ctr, crm, ci);
            }


            pi = t.GetProperty("Text");
            if (pi != null)
            {
                string text = crm.GetString(name + ".Text", ci);
                if (!String.IsNullOrEmpty(text))
                    pi.SetValue(o, text, null);
            }
            pi = t.GetProperty("Size");
            if (pi != null)
                try
                {
                    Object r = crm.GetObject(name + ".Size", ci);
                    if (r is Size)
                        pi.SetValue(o, r, null);
                }
                catch { }

            IRefreshableAndLocalizable iRef = o as IRefreshableAndLocalizable;
            if (iRef != null)
            {
                try
                {
                    iRef.LoadLocalizedStrings(ci);
                    iRef.RefreshAndReload();
                }
                catch { }
            }
        }

        public static string GetEnumStringNumericValue(object value)
        {
            Type underlyingType = Enum.GetUnderlyingType(value.GetType());
            object newVal = Convert.ChangeType(value, underlyingType);
            return " " + newVal.ToString() + " ";
        }
        public static T GetEnumValueFromStringNumeric<T>(string strNumericValue)
        {
            string sCheck = strNumericValue.Trim();
            Type underlyingType = Enum.GetUnderlyingType(typeof(T));
            MethodInfo parseMethod = underlyingType.GetMethod("Parse", new Type[] { typeof(string) });

            object uTypeVal;
            if (parseMethod != null)
                uTypeVal = parseMethod.Invoke(null, new object[] { strNumericValue });
            else
                uTypeVal = Convert.ChangeType(strNumericValue, underlyingType);
            return (T)Enum.ToObject(typeof(T), uTypeVal);
        }
        public static ClimbingTimer TheOnlyTimer = null;

        public enum AccountInfo { SECRETARY, JUDGE, NONE }

        public static AccountInfo currentAccount = AccountInfo.NONE;

        public static bool WebPasswordEntered = false;

        public static Mutex mSpeed = new Mutex();

        public static bool ListHasLeadTime(int listID, SqlConnection cn, SqlTransaction tran = null)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            cmd.CommandText = "SELECT COUNT(*) cnt" +
                              "  FROM routeResults R(nolock)" +
                              " WHERE R.list_id = @listID" +
                              "   AND ISNULL(R.timeValue,0) > 0";
            cmd.Parameters.Add("@listID", SqlDbType.Int).Value = listID;
            object oTmp = cmd.ExecuteScalar();
            if (oTmp != null && oTmp != DBNull.Value)
                if (Convert.ToInt32(oTmp) > 0)
                    return true;
            return false;
        }

        /// <summary>
        /// Refreshes two-route qualificaion list
        /// </summary>
        /// <param name="list_id">two-route qualificaion list</param>
        /// <param name="cn">sql connection used for this function</param>
        public static DataTable FillResFlash(int list_id, SqlConnection cn, bool bUsePreQf, out int routeNumber, bool showEverybody, out int timeRoutes, out KeyValuePair<int,bool>[] lists)
        {
            return FillResFlash(list_id, cn, bUsePreQf, null, out routeNumber, showEverybody, out timeRoutes, out lists);
        }

        public static DataTable FillResFlash(int list_id, SqlConnection cn, bool bUsePreQf, SqlTransaction tran, out int routeNumber, bool showEverybody, out KeyValuePair<int, bool>[] lists)
        {
            int nTmp;
            return FillResFlash(list_id, cn, bUsePreQf, tran, out routeNumber, showEverybody, out nTmp, out lists);
        }

        public static DataTable FillResFlash(int list_id, SqlConnection cn, bool bUsePreQf, SqlTransaction tran, out int routeNumber, bool showEverybody, out int timeRoutes)
        {
            KeyValuePair<int, bool>[] lists;
            return FillResFlash(list_id, cn, bUsePreQf, tran, out routeNumber, showEverybody, out timeRoutes, out lists);
        }

        /// <summary>
        /// Сводит 2 трассы квалификации трудности (как на детских - все участники лезут обе трассы)
        /// </summary>
        /// <param name="list_id"></param>
        /// <param name="cn"></param>
        /// <param name="bUsePreQf"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public static DataTable FillResFlash(int list_id, SqlConnection cn, bool bUsePreQf, SqlTransaction tran, out int routeNumber, bool showEverybody, out int timeRoutes, out KeyValuePair<int,bool>[] lists)
        {
            bool useLeadTime = SettingsForm.GetLeadTime(cn, tran);
            lists = new KeyValuePair<int,bool>[0];
            timeRoutes = 0;
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = tran;
            cmd.Connection = cn;
            cmd.CommandText = "SELECT routeNumber FROM lists(NOLOCK) WHERE iid = " + list_id.ToString();
            object oTmp = cmd.ExecuteScalar();
            routeNumber = 1;
            if (oTmp == null || oTmp == DBNull.Value)
            {
                try
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM lists(NOLOCK) WHERE iid_parent = " + list_id.ToString();
                    routeNumber = Convert.ToInt32(cmd.ExecuteScalar());
                    if (routeNumber > 0)
                    {
                        cmd.CommandText = "UPDATE lists SET routeNumber = " + routeNumber + " WHERE iid=" + list_id.ToString();
                        cmd.ExecuteNonQuery();
                    }
                    else
                        return new DataTable();
                }
                catch { return new DataTable(); }
            }
            else
                routeNumber = Convert.ToInt32(oTmp);

            cmd.CommandText = "   SELECT DISTINCT r.climber_id" +
                              "     FROM lists l(NOLOCK)" +
                              "     JOIN routeResults r(NOLOCK) ON r.list_id = l.iid" +
                              "    WHERE l.iid_parent = " + list_id.ToString();
            if (showEverybody)
                cmd.CommandText += "      AND (r.res IS NOT NULL " +
                                   "       OR r.preQf =1)";
            if (!bUsePreQf)
                cmd.CommandText += " AND r.preQf = 0 ";
            cmd.CommandText += " ORDER BY r.climber_id";
            List<int> climbers = new List<int>();
            SqlDataReader rdr = cmd.ExecuteReader();
            try
            {
                while (rdr.Read())
                    climbers.Add(Convert.ToInt32(rdr["climber_id"]));
            }
            finally { rdr.Close(); }
            if (climbers.Count < 1)
                return new DataTable();

            cmd.CommandText = "SELECT iid FROM lists(NOLOCK) WHERE iid_parent = " + list_id.ToString() + " ORDER BY iid";
            List<KeyValuePair<int,bool>> listsL = new List<KeyValuePair<int,bool>>();
            rdr = cmd.ExecuteReader();
            try
            {
                while (rdr.Read())
                {
                    KeyValuePair<int, bool> kvp = new KeyValuePair<int, bool>(Convert.ToInt32(rdr["iid"]), false);
                    listsL.Add(kvp);
                }
            }
            finally { rdr.Close(); }
            lists = listsL.ToArray();

            if(useLeadTime)
                for(int iqw = 0; iqw < lists.Length; iqw++)
                    if (ListHasLeadTime(lists[iqw].Key, cmd.Connection, cmd.Transaction))
                    {
                        timeRoutes++;
                        lists[iqw] = new KeyValuePair<int, bool>(lists[iqw].Key, true);
                    }

            if (lists.Length < 1)
                return new DataTable();
            if (routeNumber != lists.Length)
            {
                routeNumber = lists.Length;
                cmd.CommandText = "UPDATE lists SET routeNumber = " + routeNumber.ToString() + " WHERE iid = " + list_id.ToString();
                cmd.ExecuteNonQuery();
            }
            cmd.CommandText = @"SELECT '' Место, p.iid [№], p.surname + ' '+p.name [ФамилияИмя],
                                       p.age [Г.р.], p.qf Разряд, dbo.fn_getTeamName(p.iid, " + list_id.ToString() + @") Команда, p.vk
                                  FROM Participants p(NOLOCK)
                                 WHERE p.iid = @iid";
            cmd.Parameters.Add("@iid", SqlDbType.Int);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.SelectCommand.Parameters[0].Value = climbers[0];
            da.Fill(dt);
            for (int k = 1; k < climbers.Count; k++)
            {
                da.SelectCommand.Parameters[0].Value = climbers[k];
                da.Fill(dt);
            }

            cmd.CommandText = @"SELECT CASE preQf WHEN 0 THEN resText ELSE '' END res,
                                       CASE preQf WHEN 0 THEN ptsText ELSE '' END pts,
                                       CASE preQf WHEN 0 THEN ISNULL(pts,0.1) ELSE 0.1 END work,
                                       preQf, CASE WHEN res IS NULL THEN 1 ELSE 0 END resNull,
                                       ISNULL(timeText,'') timeText
                                  FROM routeResults(NOLOCK)
                                 WHERE list_id = @lid
                                   AND climber_id = @iid";
            cmd.Parameters.Add("@lid", SqlDbType.Int);
            for (int k = 1; k <= lists.Length; k++)
            {
                SqlCommand cmd2 = new SqlCommand();
                cmd2.Connection = cn;
                cmd2.Transaction = tran;
                cmd2.CommandText = "SELECT COUNT(*) FROM routeResults(NOLOCK) WHERE list_id = " +
                    lists[k - 1].Key.ToString();
                double lastPls = Convert.ToInt32(cmd2.ExecuteScalar());
                cmd2.CommandText = @"select count(*)
                                       from routeResults r(nolock)
                                       join Participants p(nolock) on p.iid = r.climber_id
                                      where p.vk = 1
                                        and r.list_id = " + lists[k - 1].Key.ToString();
                lastPls -= Convert.ToInt32(cmd2.ExecuteScalar());
                
                string resCol = "Тр." + k.ToString();
                string timeCol = "Время " + k.ToString();
                string ptsCol = "Балл " + k.ToString();
                string workCol = "work" + k.ToString();
                dt.Columns.Add(resCol, typeof(string));
                if (lists[k - 1].Value)
                    dt.Columns.Add(timeCol, typeof(string));
                dt.Columns.Add(ptsCol, typeof(string));
                dt.Columns.Add(workCol, typeof(double));
                cmd.Parameters[1].Value = lists[k - 1].Key;
                List<DataRow> toDelete = new List<DataRow>();
                foreach (DataRow dr in dt.Rows)
                {
                    cmd.Parameters[0].Value = Convert.ToInt32(dr["№"]);
                    rdr = cmd.ExecuteReader();

                    try
                    {
                        if (rdr.Read())
                        {
                            bool preQf = (Convert.ToInt32(rdr["preQf"]) > 0);
                            bool resNull = (Convert.ToInt32(rdr["resNull"]) > 0);
                            if (resNull && !preQf)
                            {
                                if (showEverybody)
                                {
                                    dr[resCol] = "";
                                    dr[ptsCol] = "";
                                    dr[workCol] = lastPls;
                                }
                                else
                                    toDelete.Add(dr);
                            }
                            else
                            {
                                dr[resCol] = rdr["res"].ToString();
                                dr[ptsCol] = rdr["pts"].ToString();
                                if (rdr["work"] != null && rdr["work"] != DBNull.Value)
                                    dr[workCol] = Convert.ToDouble(rdr["work"]);
                                else
                                    dr[workCol] = lastPls;
                                if (lists[k - 1].Value)
                                    dr[timeCol] = rdr["timeText"].ToString();
                            }
                        }
                        else
                        {
                            dr[resCol] = "";
                            dr[ptsCol] = "";
                            dr[workCol] = lastPls;
                            if (lists[k - 1].Value)
                                dr[timeCol] = String.Empty;
                        }
                    }
                    finally { rdr.Close(); }
                }
                foreach (DataRow dr in toDelete)
                    try { dt.Rows.Remove(dr); }
                    catch { }
            }
            dt.Columns.Add("Рез-т", typeof(string));
            dt.Columns.Add("res", typeof(double));
            dt.Columns.Add("Кв.", typeof(string));
            foreach (DataRow dr in dt.Rows)
            {
                double cRes = 1.0;
                for (int k = 1; k <= routeNumber; k++)
                    cRes *= Convert.ToDouble(dr["work" + k.ToString()]);
                cRes = Math.Pow(cRes, 1.0 / ((double)routeNumber));
                cRes = Math.Round(cRes, 2, MidpointRounding.AwayFromZero);
                dr["res"] = cRes;
            }
            SortingClass.SortByColumn(dt, "res");
            for (int k = 1; k <= routeNumber; k++)
                dt.Columns.Remove("work" + k.ToString());

            cmd.CommandText = "SELECT quote FROM lists WHERE iid = " + list_id.ToString();
            double quote;
            try { quote = Convert.ToDouble(cmd.ExecuteScalar()); }
            catch { quote = -1.0; }
            DataTable dtCreate = dt;
            if (dt.Rows.Count > 0)
            {
                #region RefreshTable

                int i = 0;
                long resPrev = Convert.ToInt64(Convert.ToDouble(dtCreate.Rows[i]["res"]) * 1000);
                long resCur;
                int st = 0;

                dtCreate.Columns.Add("ptsT", typeof(double));
                dtCreate.Columns.Add("pos", typeof(int));
                if (dtCreate.Rows.Count == 1)
                {
                    dtCreate.Rows[0]["pos"] = 1;
                    dtCreate.Rows[0]["ptsT"] = 1.0;
                }

                foreach (DataRow rd in dtCreate.Rows)
                {
                    //int number = Convert.ToInt32(rd["climber_id"]);
                    //cmd.CommandText = "SELECT Surname FROM Participants WHERE Number = " + number;
                    //string name = number + ' ' + cmd.ExecuteScalar().ToString();
                    //cmd.CommandText = "SELECT Name FROM Participants WHERE Number = " + number;
                    //name += ' ' + cmd.ExecuteScalar().ToString();

                    if (i == 0)
                    {
                        i++;
                        continue;
                    }
                    resCur = Convert.ToInt64(Convert.ToDouble(rd["res"]) * 1000);

                    if (resCur == resPrev)
                    {
                        if (i == dtCreate.Rows.Count - 1)
                        {
                            double pts = ((double)(i + st + 2)) / 2.0;
                            for (int k = st; k <= i; k++)
                            {
                                dtCreate.Rows[k]["ptsT"] = pts;
                                dtCreate.Rows[k]["Место"] = (st + 1).ToString();
                                dtCreate.Rows[k]["pos"] = st + 1;
                            }
                        }
                    }
                    else
                    {
                        double pts = ((double)(i + st + 1)) / 2.0;
                        for (int k = st; k < i; k++)
                        {
                            dtCreate.Rows[k]["ptsT"] = pts;
                            dtCreate.Rows[k]["Место"] = (st + 1).ToString();
                            dtCreate.Rows[k]["pos"] = st + 1;
                        }
                        st = i;

                        if (resCur == 0)
                            break;
                        if (i == dtCreate.Rows.Count - 1)
                        {
                            rd["ptsT"] = i + 1;
                            rd["Место"] = (i + 1).ToString();
                            rd["pos"] = i + 1;
                        }

                        resPrev = resCur;
                    }
                    if (resCur == 0)
                        break;
                    i++;
                }

                DataRow rdCur, rdNext;
                int vkTotal = 0, vkGroup = 0;

                for (i = 0; i < dtCreate.Rows.Count; i++)
                {
                    rdCur = dtCreate.Rows[i];
                    if (Convert.ToDouble(rdCur["res"]) < 0.0001)
                        break;
                    int stop = Convert.ToInt32(Convert.ToDouble(rdCur["ptsT"]) * 2.0) - i - 1;
                    vkGroup = 0;
                    for (int k = i; k < stop; k++)
                    {
                        rdNext = dtCreate.Rows[k];
                        cmd.CommandText = "SELECT Vk FROM Participants WHERE iid = " + rdNext["№"].ToString();
                        bool vk = Convert.ToBoolean(cmd.ExecuteScalar());
                        if (vk)
                        {
                            rdNext["Место"] = "в/к";
                            rdNext["Рез-т"] = "в/к";
                            vkGroup++;
                        }
                    }
                    double pts = (i + 1 + stop - 2 * vkTotal - vkGroup) / 2.0;
                    for (int k = i; k < stop; k++)
                    {
                        if (pts < quote + 0.9)
                            dtCreate.Rows[k]["Кв."] = "Q";
                        else
                            dtCreate.Rows[k]["Кв."] = "";
                        dtCreate.Rows[k]["ptsT"] = pts;
                        dtCreate.Rows[k]["pos"] = i + 1 - vkTotal;
                        if (dtCreate.Rows[k]["Рез-т"].ToString() != "в/к")
                        {
                            dtCreate.Rows[k]["Место"] = (i + 1 - vkTotal).ToString();
                            dtCreate.Rows[k]["Рез-т"] = Convert.ToDouble(dtCreate.Rows[k]["res"]).ToString("0.00");
                        }
                    }
                    vkTotal += vkGroup;
                    i = stop - 1;
                }
                //try { daCreate.Update(dtCreate); }
                //catch (Exception ex) { MessageBox.Show(ex.Message); }
                //RefreshData();


                foreach (DataRow dr in dtCreate.Rows)
                {
                    bool fNya = true;
                    bool fDsq = true;
                    for (int k = 1; k <= routeNumber; k++)
                    {
                        string res = dr["Тр." + k.ToString()].ToString().ToLower();
                        if (res == "н/я")
                            continue;
                        else if (res == "дискв.")
                        {
                            if (fNya)
                            {
                                fNya = false;
                                fDsq = true;
                            }
                            else
                                continue;
                        }
                        else
                        {
                            fNya = fDsq = false;
                            break;
                        }
                    }
                    if (fNya || fDsq)
                    {
                        dr["Место"] = dr["Кв."] = "";
                        for (int k = 1; k <= routeNumber; k++)
                            dr["Балл " + k.ToString()] = "";
                        if (fNya)
                            dr["Рез-т"] = "н/я";
                        else
                            dr["Рез-т"] = "дискв.";
                    }
                }

                #endregion
            }
            //foreach (DataRow drr in dtCreate.Rows)
            //drr["vk"] = !Convert.ToBoolean(drr["vk"]);
            SortingClass.SortTwoCases(dtCreate, dtCreate.Columns.IndexOf("res"), dtCreate.Columns.IndexOf("vk"));
            int nn = 0;
            foreach (DataRow drr in dtCreate.Rows)
                if (!(bool)drr["vk"])
                    drr["pos"] = Convert.ToInt32(drr["pos"]) + nn;
                else
                    nn++;
            if (dtCreate.Columns.IndexOf("res") > -1)
                dtCreate.Columns.Remove("res");

            if (dtCreate.Columns.IndexOf("ptsT") > -1)
                dtCreate.Columns.Remove("ptsT");
            if (dtCreate.Columns.IndexOf("vk") > -1)
                dtCreate.Columns.Remove("vk");

            //dtCreate.Columns.Remove("pos");

            return dtCreate;
        }

        public static bool boulder;

        public static bool Trial;

        public static int TrialLeft = 0;

        public static ListTypeEnum GetListType(int listID, SqlConnection cn, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            cmd.CommandText = "SELECT listType FROM lists(NOLOCK) WHERE iid = " + listID.ToString();
            object res = cmd.ExecuteScalar();
            if (res != null && res != DBNull.Value)
                try { return (ListTypeEnum)Enum.Parse(typeof(ListTypeEnum), res.ToString(), true); }
                catch { return ListTypeEnum.Unknown; }
            else
                return ListTypeEnum.Unknown;
        }

        public static DataTable FillResOnsight(int listID, SqlConnection cn, bool bUsePreQf)
        {
            return FillResOnsight(listID, cn, bUsePreQf, null);
        }

        /// <summary>
        /// Сводит 2 трассы квалификации боулдеринга или трудности в случае,
        /// если половина участников лезет одну трассу, а половина - другую
        /// </summary>
        /// <param name="listID"></param>
        /// <param name="cn"></param>
        /// <param name="bUsePreQf"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public static DataTable FillResOnsight(int listID, SqlConnection cn, bool bUsePreQf, SqlTransaction tran)
        {
            bool pts;
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return new DataTable(); }

            SqlCommand cmd = new SqlCommand("SELECT style FROM lists(NOLOCK) WHERE iid=" + listID.ToString(), cn);
            cmd.Transaction = tran;
            try { boulder = (cmd.ExecuteScalar().ToString() == "Боулдеринг"); }
            catch { boulder = false; }
            /*cmd.CommandText = "SELECT usePts FROM lists(NOLOCK) WHERE iid = " + listID.ToString();
            try { pts = Convert.ToBoolean(cmd.ExecuteScalar()); }
            catch { pts = true; }*/
#if FULL
            pts = (SettingsForm.GetOSQfMode(cn, tran) == SettingsForm.OSQfMode.PTS);
#else
            pts = true;
#endif
            DataTable result_route1 = new DataTable();
            DataTable result_route2 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            string sWhere, sPl, sPts;
            if (bUsePreQf)
            {
                sWhere = " OR preQf = 1 ";
                sPl = " CASE preQf WHEN 0 THEN posText ELSE '0' END posText, " +
                    " CASE preQf WHEN 0 THEN posText ELSE '0' END Место, ";
                sPts = "CASE preQf WHEN 0 THEN pts ELSE 0.1 END pts ";
            }
            else
            {
                sWhere = "";
                sPl = " posText, posText Место, ";
                sPts = " CASE r1.preQf WHEN 1 THEN 0.1 ELSE r1.pts END pts";
            }
            if (boulder)
            {
                da.SelectCommand = new SqlCommand(
                    "SELECT " + sPts + ", r1.pos," + sPl + "r1.climber_id AS [№]," +
                    "p.surname+' '+p.name AS [ФамилияИмя], p.age AS [Г.р.]," +
                    "p.qf AS [Разряд], dbo.fn_getTeamName(p.iid, l.iid) AS [Команда]," +
                    "r1.disq disqA, r1.nya nyaA, r1.tops AS [ТА],r1.topAttempts AS [ПтА],r1.bonuses AS [БА],r1.bonusAttempts AS [ПбА]," +
                    "CONVERT(BIT,NULL) disqB, CONVERT(BIT,NULL) nyaB, NULL AS [ТБ],NULL AS [ПтБ],NULL AS [ББ],NULL AS [ПбБ]," +
                    "r1.qf AS [Кв.], p.vk, " +
                    "CASE WHEN (p.vk=1 OR p.nopoints=1) THEN 1 ELSE 0 END nopoints " +
                    "FROM lists l " +
                    " JOIN lists l1 ON ((l1.iid_parent=l.iid) AND (l.style=l1.style) AND (l.group_id=l1.group_id)) " +
                    " JOIN boulderResults r1 ON l1.iid = r1.list_id " +
                    "INNER JOIN Participants p ON r1.climber_id=p.iid INNER JOIN " +
                    "Teams t ON p.team_id = t.iid " +
                    "WHERE (l1.round = 'Квалификация Группа А') AND (l.iid = " + listID.ToString() + ") " +
                    "AND (res IS NOT NULL" + sWhere + ") ORDER BY preQf DESC, [res] DESC, disqA, nyaA", cn);
                da.SelectCommand.Transaction = tran;
                da.Fill(result_route1);
                da.SelectCommand.CommandText =
                    "SELECT " + sPts + ", r1.pos," + sPl + "r1.climber_id AS [№]," +
                    "p.surname+' '+p.name AS [ФамилияИмя], p.age AS [Г.р.]," +
                    "p.qf AS [Разряд], dbo.fn_getTeamName(p.iid, l.iid) AS [Команда]," +
                    "NULL disqA, NULL nyaA, NULL AS [ТА],NULL AS [ПтА],NULL AS [БА],NULL AS [ПбА]," +
                    "r1.disq disqB, r1.nya nyaB, r1.tops AS [ТБ],r1.topAttempts AS [ПтБ],r1.bonuses AS [ББ],r1.bonusAttempts AS [ПбБ]," +
                    "r1.qf AS [Кв.], p.vk, " +
                    "CASE WHEN (p.vk=1 OR p.nopoints=1) THEN 1 ELSE 0 END nopoints " +
                    "FROM lists l " +
                    " JOIN lists l1 ON ((l1.iid_parent=l.iid) AND (l.style=l1.style) AND (l.group_id=l1.group_id)) " +
                    " JOIN boulderResults r1 ON l1.iid = r1.list_id " +
                    "INNER JOIN Participants p ON r1.climber_id=p.iid INNER JOIN " +
                    "Teams t ON p.team_id = t.iid " +
                    "WHERE (l1.round = 'Квалификация Группа Б') AND (l.iid = " + listID.ToString() + ") " +
                    "AND (res IS NOT NULL" + sWhere + ") ORDER BY preQf DESC, [res] DESC, disqB, nyaB";
                da.Fill(result_route2);
            }
            else
            {
                da.SelectCommand = new SqlCommand(
                    "SELECT " + sPts + ", r1.pos," + sPl + "r1.climber_id AS [№]," +
                    "p.surname+' '+p.name AS [ФамилияИмя], p.age AS [Г.р.]," +
                    "p.qf AS [Разряд], dbo.fn_getTeamName(p.iid, l.iid) AS [Команда],r1.resText AS [Трасса 1],'' AS [Трасса 2],r1.qf AS [Кв.], p.vk, " +
                    "CASE WHEN (p.vk=1 OR p.nopoints=1) THEN 1 ELSE 0 END nopoints " +
                    "FROM lists l INNER JOIN lists l1 ON ((l.style=l1.style) AND (l.group_id=" +
                    "l1.group_id)) INNER JOIN routeResults r1 ON l1.iid = r1.list_id " +
                    "INNER JOIN Participants p ON r1.climber_id=p.iid INNER JOIN " +
                    "Teams t ON p.team_id = t.iid " +
                    "WHERE (l1.round = '1/4 финала Трасса 1') AND (l.iid = " + listID.ToString() + ") " +
                    "AND (res IS NOT NULL" + sWhere + ") ORDER BY preQf DESC, [res] DESC", cn);
                da.SelectCommand.Transaction = tran;
                da.Fill(result_route1);
                da.SelectCommand.CommandText =
                     "SELECT " + sPts + ", r1.pos," + sPl + "r1.climber_id AS [№]," +
                    "p.surname+' '+p.name AS [ФамилияИмя], p.age AS [Г.р.]," +
                    "p.qf AS [Разряд], dbo.fn_getTeamName(p.iid, l.iid) AS [Команда], '' AS [Трасса 1],r1.resText AS [Трасса 2],r1.qf AS [Кв.], p.vk, " +
                    "CASE WHEN (p.vk=1 OR p.nopoints=1) THEN 1 ELSE 0 END nopoints " +
                    "FROM lists l INNER JOIN lists l1 ON ((l.style=l1.style) AND (l.group_id=" +
                    "l1.group_id)) INNER JOIN routeResults r1 ON l1.iid = r1.list_id " +
                    "INNER JOIN Participants p ON r1.climber_id=p.iid INNER JOIN " +
                    "Teams t ON p.team_id = t.iid " +
                    "WHERE (l1.round = '1/4 финала Трасса 2') AND (l.iid = " + listID.ToString() + ") " +
                    "AND (res IS NOT NULL" + sWhere + ") ORDER BY preQf DESC, [res] DESC";
                da.Fill(result_route2);
            }
            foreach (DataRow row_route2 in result_route2.Rows)
                result_route1.Rows.Add(row_route2.ItemArray);

            double dTmp;
            DataColumn dtS/*, dtcF = null*/;
            if (pts)
            {
                SortingClass.SortByColumn(result_route1, "pts");
                dtS = result_route1.Columns["pts"];
            }
            else
            {


                //SortingClass.SortByColumn(result_route1, "pos");
                //SortingClass.SortTwoCases(result_route1, "pts", "pos");
                //dtS = result_route1.Columns["pos"];
                if (result_route1.Columns.IndexOf("posText") > -1)
                    dtS = result_route1.Columns["posText"];
                else
                    dtS = result_route1.Columns["Место"];
                DataColumn dcTmp = result_route1.Columns.Add("pos2sort", typeof(double));
                //double dValC;
                foreach (DataRow dr in result_route1.Rows)
                {
                    if (!double.TryParse(dr[dtS].ToString(), out dTmp))
                        dTmp = Convert.ToDouble(dr["pts"]);
                    dr["pos2sort"] = dTmp;
                }
                SortingClass.SortByColumn(result_route1, "pos2sort");
                dtS = dcTmp;
            }
            result_route1.Columns.Add("nPos0", typeof(int));
            int nCp = 1;
            int nCpCnt = 1;
            double dVal;
            /*if (dtcF != null)
            {
                if (!double.TryParse(result_route1.Rows[0][dtcF].ToString(), out dVal))
                    dVal = Convert.ToDouble(result_route1.Rows[0][dtS]);
            }
            else*/
            dVal = Convert.ToDouble(result_route1.Rows[0][dtS]);
            result_route1.Rows[0]["nPos0"] = nCp;
            for (int i = 1; i < result_route1.Rows.Count; i++)
            {

                double dValC;
                /*if (dtcF != null)
                {
                    if (!double.TryParse(result_route1.Rows[i][dtcF].ToString(), out dValC))
                        dValC = Convert.ToDouble(result_route1.Rows[i][dtS]);
                }
                else*/
                dValC = Convert.ToDouble(result_route1.Rows[i][dtS]);
                if (dValC == dVal)
                    nCpCnt++;
                else
                {
                    nCp += nCpCnt;
                    nCpCnt = 1;
                    dVal = dValC;
                }
                if (Convert.ToInt32(result_route1.Rows[i]["pos"]) == 1)
                    result_route1.Rows[i]["nPos0"] = 1;
                else
                    result_route1.Rows[i]["nPos0"] = nCp;
            }
            DataTable dtTmp = SortingClass.CreateStructure();
            dtTmp.Columns.Add("pos0", typeof(int));
            object[] obj = new object[dtTmp.Columns.Count];
            for (int i = 0; i < obj.Length; i++)
                obj[i] = DBNull.Value;
            foreach (DataRow dr in result_route1.Rows)
            {
                DataRow drI = dtTmp.Rows.Add(obj);
                drI["vk"] = Convert.ToBoolean(dr["vk"]);
                drI["iid"] = Convert.ToInt32(dr["№"]);
                int sp = 0;
                foreach (DataColumn dcI in result_route1.Columns)
                {
                    switch (dr[dcI].ToString().ToLower())
                    {
                        case "н/я":
                            sp = 2;
                            break;
                        case "дискв.":
                            sp = 1;
                            break;
                    }
                    if (sp > 0)
                        break;
                }
                drI["sp"] = sp;
                drI["pos0"] = Convert.ToInt32(dr["nPos0"]);
            }
            SortingClass.SortResults(dtTmp, true, false);
            foreach (DataRow dr in dtTmp.Rows)
                foreach (DataRow drI in result_route1.Rows)
                    if (dr["iid"].ToString() == drI["№"].ToString())
                    {
                        if (Convert.ToInt32(drI["pos"]) > 1)
                        {
                            drI["pos"] = Convert.ToInt32(dr["pos"]);
                            drI["Место"] = dr["posText"].ToString();
                            drI["posText"] = dr["posText"].ToString();
                            drI["pts"] = Convert.ToDouble(dr["pts"]);
                        }
                        else
                        {
                            drI["pts"] = 1.0;
                            drI["pos"] = 1;
                        }
                        break;
                    }
            result_route1.Columns.Add("isQ", typeof(double));
            foreach (DataRow dr in result_route1.Rows)
                if (dr["Кв."].ToString().Length > 0)
                    dr["isQ"] = 0.0;
                else
                    dr["isQ"] = 1.0;
            SortingClass.SortByColumn(result_route1, "pts");
            SortingClass.SortTwoCases(result_route1, "pts", "isQ");
            int posQMax = 0, posTQMax = 0, nTmp;
            int cntQ = 0;
            foreach (DataRow dr in result_route1.Rows)
                if (dr["Кв."].ToString().Length > 0)
                {
                    if (int.TryParse(dr["Место"].ToString(), out nTmp))
                        posTQMax = nTmp;
                    posQMax = Convert.ToInt32(dr["pos"]);
                    cntQ++;
                }
                else if (posQMax == 0 || dr["Место"].ToString() == "0")
                    cntQ++;
                else if (posQMax * posTQMax > 0 && Convert.ToInt32(dr["pos"]) == posQMax)
                {
                    dr["pos"] = cntQ + 1;
                    if (int.TryParse(dr["Место"].ToString(), out nTmp))
                    {
                        int posDiff = cntQ + 1 - posQMax;
                        dr["Место"] = (nTmp + posDiff).ToString();
                    }
                }
            result_route1.Columns.Remove("posText");
            result_route1.Columns.Remove("vk");
            result_route1.Columns.Remove("nopoints");
            result_route1.Columns.Remove("nPos0");
            result_route1.Columns.Remove("pts");
            result_route1.Columns.Remove("isQ");
            result_route1.Columns.Add("posT", typeof(double));
            foreach (DataRow dr in result_route1.Rows)
                dr["posT"] = Convert.ToDouble(dr["pos"]);
            if (result_route1.Columns.IndexOf("pos2sort") > -1)
                result_route1.Columns.Remove("pos2sort");
            //result_route1.Columns.Remove("pts");
            return result_route1;
        }

        /// <summary>
        /// Как-то парсит строку с данными о соревнованиях
        /// ХЗ как - писал на 3 курсе
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] ParseCompTitle(string str)
        {
            int tmp = 0;
            string[] res = new string[4];
            for (int i = 0; i < 2; i++)
            {
                int tmpN = str.IndexOf("\r\n", tmp);
                try
                {
                    if (tmpN < 0)
                        throw new ArgumentException("Incorrect string format");
                    res[i] = str.Substring(tmp, tmpN - tmp);
                }
                catch
                {
                    res[i] = "";
                }
                tmp = tmpN + 2;
            }
            int qq = -2;
            try
            {
                qq = str.IndexOf("\r\n", tmp + 1);
                res[2] = str.Substring(tmp, qq - tmp);
            }
            catch
            {
                res[2] = "";
                res[3] = "";
                return res;
            }
            qq += 2;
            try
            {
                res[3] = str.Substring(qq, str.Length - qq);
            }
            catch
            {
                res[3] = "";
            }
            return res;
        }

        /// <summary>
        /// получает следующий iid из таблицы
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="cn"></param>
        /// <param name="field"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        static public long GetNextIID(string tableName, SqlConnection cn, string field, SqlTransaction tran)
        {
            long res;

            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(" + field + ")+1, 1) nxt FROM " + tableName, cn);
            cmd.Transaction = tran;
            try { res = Convert.ToInt64(cmd.ExecuteScalar()); }
            catch { res = 1; }
            return res;
        }

        static public void CreateStartList(int listID, SqlTransaction tran)
        {
            try
            {
                SqlConnection cn = tran.Connection;
                if (cn.State != ConnectionState.Open)
                    cn.Open();

                SqlCommand cmd = new SqlCommand("SELECT L.style, L.round, G.name, L.group_id, L.judge_id, L.routeNumber " +
                                                  "FROM lists L " +
                                                  "JOIN Groups G ON G.iid = L.group_id " +
                                                 "WHERE l.iid = " + listID.ToString(), cn);
                cmd.Transaction = tran;
                SqlDataReader rdr = cmd.ExecuteReader();
                string strGroup = "", strStyle = "", strRound = "";
                int groupID = 0, jiid = 0, routeNumber = -1;
                try
                {
                    if (rdr.Read())
                    {
                        strStyle = rdr[0].ToString();
                        strRound = rdr[1].ToString();
                        strGroup = rdr[2].ToString();
                        groupID = (int)rdr[3];
                        try { jiid = Convert.ToInt32(rdr["judge_id"]); }
                        catch { jiid = 0; }
                        if (rdr["routeNumber"] != null && rdr["routeNumber"] != DBNull.Value)
                            try { routeNumber = Convert.ToInt32(rdr["routeNumber"]); }
                            catch { routeNumber = -1; }
                        else
                            routeNumber = -1;
                    }
                    else
                    {
                        MessageBox.Show("Ошибка создания протокола");
                        return;
                    }
                }
                finally { rdr.Close(); }

                StartListMode sm;
                List<Starter> data = new List<Starter>();

                string tableName, columnName, boolName;
                switch (strStyle)
                {
                    case "Трудность":
                        tableName = "routeResults";
                        columnName = "rankingLead";
                        boolName = "lead";
                        break;
                    case "Боулдеринг":
                        tableName = "boulderResults";
                        columnName = "rankingBoulder";
                        boolName = "boulder";
                        break;
                    case "Скорость":
                        tableName = "speedResults";
                        columnName = "rankingSpeed";
                        boolName = "speed";
                        break;
                    default:
                        return;
                }

                cmd.CommandText = " SELECT iid, lateAppl, ISNULL(" + columnName + ", 0) ranking " +
                                    " FROM Participants(NOLOCK) " +
                                   " WHERE group_id = " + groupID.ToString() +
                                     " AND " + boolName + " > 0";
                rdr = cmd.ExecuteReader();
                Random rn = new Random();
                try
                {
                    while (rdr.Read())
                    {
                        Starter str = new Starter();
                        str.iid = Convert.ToInt32(rdr[0]);
                        str.lateAppl = Convert.ToBoolean(rdr[1]);
                        str.prevPos = 0;
                        str.prevStart = 0;
                        str.random = rn.NextDouble();
                        str.ranking = Convert.ToInt32(rdr[2]);
                        data.Add(str);
                    }
                }
                finally { rdr.Close(); }
                switch (strRound)
                {
                    case "Квалификация (2 трассы)":
                        sm = StartListMode.QualiFlash;
                        break;
                    case "Квалификация (2 группы)":
                        sm = StartListMode.TwoRoutes;
                        break;
                    case "1/4 финала (2 трассы)":
                        sm = StartListMode.TwoRoutes;
                        break;
                    default:
                        if (strRound.IndexOf("Квалификация") > -1 && strStyle == "Трудность")
                        {
                            strRound = "Квалификация (2 трассы)";
                            goto case "Квалификация (2 трассы)";
                        }
                        sm = StartListMode.OneRoute;
                        break;
                }

                Sorting srt = new Sorting(data, sm, routeNumber);
                srt.ShowDialog();
                if (srt.Cancel)
                    return;

                List<string> rounds = new List<string>();
                List<string> listTypes = new List<string>();
                List<int> iids = new List<int>();
                iids.Add(listID);
                switch (strRound)
                {
                    case "Квалификация (2 трассы)":
                        for (int i = 1; i <= routeNumber; i++)
                        {
                            rounds.Add("Квалификация " + i.ToString());
                            listTypes.Add(ListTypeEnum.LeadSimple.ToString());
                        }
                        routeNumber = 0;
                        goto case "INSERTING";
                    case "Квалификация (2 группы)":
                        //routeNumber = 2;
                        rounds.Add("Квалификация Группа А");
                        rounds.Add("Квалификация Группа Б");
                        for (int thth = 0; thth < 2; thth++)
                            listTypes.Add(ListTypeEnum.BoulderSimple.ToString());
                        goto case "INSERTING";
                    case "1/4 финала (2 трассы)":
                        routeNumber = -1;
                        rounds.Add("1/4 финала Трасса 1");
                        rounds.Add("1/4 финала Трасса 2");
                        for (int thth = 0; thth < 2; thth++)
                            listTypes.Add(ListTypeEnum.LeadSimple.ToString());
                        goto case "INSERTING";
                    case "INSERTING":
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@jiid", SqlDbType.Int);
                        if (jiid > 0)
                            cmd.Parameters[0].Value = jiid;
                        else
                            cmd.Parameters[0].Value = DBNull.Value;
                        iids.Clear();
                        cmd.Parameters.Add("@rn", SqlDbType.Int);
                        for (int i = 0; i < rounds.Count; i++)
                        {
                            iids.Add((int)GetNextIID("lists", cn, "iid", tran));
                            cmd.CommandText = "INSERT INTO lists (iid, group_id, style, round, judge_id, iid_parent, routeNumber, listType) " +
                                                   "VALUES (" + iids[i].ToString() + ", " +
                                                            groupID.ToString() + ", '" + strStyle + "', " +
                                                            "'" + rounds[i] + "', @jiid," + listID.ToString() + ",@rn,'" + listTypes[i].ToString() + "')";

                            cmd.Parameters[1].Value = (routeNumber > 0 ? routeNumber : (object)DBNull.Value);
                            cmd.ExecuteNonQuery();
                        }
                        cmd.Parameters.Clear();
                        break;
                }
                cmd.CommandText = "INSERT INTO " + tableName + " (list_id,iid,climber_id,start,pos) VALUES " +
                                                                 "(@list_id,@iid,@ciid,@start,@pos)";
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@list_id", SqlDbType.Int);
                cmd.Parameters.Add("@iid", SqlDbType.BigInt);
                cmd.Parameters.Add("@ciid", SqlDbType.Int);
                cmd.Parameters.Add("@start", SqlDbType.Int);
                cmd.Parameters.Add("@pos", SqlDbType.Int);
                cmd.Parameters[4].Value = int.MaxValue;
                for (int i = 0; i < iids.Count; i++)
                {
                    cmd.Parameters[0].Value = iids[i];
                    int strpos = 1;
                    foreach (Starter st in srt[i])
                    {
                        cmd.Parameters[1].Value = StaticClass.GetNextIID(tableName, cn, "iid", tran);
                        cmd.Parameters[2].Value = st.iid;
                        cmd.Parameters[3].Value = strpos++;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                string str = "Ошибка создания стартового протокола:\r\n" + ex.Message;
                throw new Exception(str, ex);
            }
        }

        /// <summary>
        /// Запускает Excel
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="ws"></param>
        /// <param name="footer"></param>
        /// <param name="cn"></param>
        /// <returns></returns>
        public static bool LaunchExcel(out Excel.Application xlApp, out Excel.Workbook wb, out Excel.Worksheet ws, bool footer, SqlConnection cn)
        {
            return LaunchExcel(out xlApp, out wb, out ws, true, footer, cn);
        }

        /// <summary>
        /// Тоже запускает Excel, но с больши числом параметров
        /// в т.ч. печать колонтитулов, копирайта
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="ws"></param>
        /// <param name="copyright"></param>
        /// <param name="footer"></param>
        /// <param name="cn"></param>
        /// <returns></returns>
        public static bool LaunchExcel(out Excel.Application xlApp, out Excel.Workbook wb, out Excel.Worksheet ws, bool copyright, bool footer, SqlConnection cn, bool askForNew = true, bool inThread = true, string templateName = "Книга.xlt", SqlTransaction _tran = null)
        {
            xlApp = null; ws = null; wb = null;

            try
            {

                Excel.Border borderLeft;
                Excel.Border borderRight;
                Excel.Border borderTop;
                Excel.Border borderBottom;
                Excel.Style style;
                //bool templateOrFileLoaded;
                DialogResult dgr;
                if (askForNew)
                {
                    dgr = MessageBox.Show("Создать новую книгу Excel?\r\n" +
                        "(НЕТ-> выбрать существующую и добавить туда новый лист)",
                        "Вопрос?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (dgr == DialogResult.Cancel)
                        return false;
                }
                else
                    dgr = DialogResult.Yes;

                string fileName;
                if (dgr == DialogResult.No)
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Multiselect = false;
                    ofd.Filter = "Книга Microsoft Excel (*.xls)|*.xls|Книга Microsoft Excel 2007 (*.xlsx)|*.xlsx|Все файлы (*.*)|*.*";
                    ofd.FilterIndex = 0;
                    dgr = ofd.ShowDialog();
                    if ((dgr == DialogResult.Cancel) || (ofd.FileName == ""))
                        return false;
                    fileName = ofd.FileName;
                }
                else
                    fileName = "";

                if (inThread)
                {
                    while (ExcelWorkingThread != null && ExcelWorkingThread.IsAlive)
                        Thread.Sleep(50);
                    if (ExcelWorkingThread != null)
                    {
                        ExcelWorkingThread = null;
                        GC.Collect();
                    }
                    ExcelWorkingThread = new Thread(new ThreadStart(delegate
                    {
                        ExcelWorkingForm wf = new ExcelWorkingForm();
                        wf.ShowDialog();
                    }));
                    ExcelWorkingThread.Start();
                }
                xlApp = new Excel.Application();


                if (xlApp == null)
                {
                    MessageBox.Show("Excel не может быть запущен.");
                    return false;
                }

                
                if (fileName=="")
                {
                    string template = Thread.GetDomain().BaseDirectory + "Templates\\" + templateName;
                    //wb = xlApp.Workbooks.Add(Type.Missing);
                    try
                    {
                        wb = xlApp.Workbooks.Add(template);
                        //templateOrFileLoaded = true;
                    }
                    catch
                    {
                        wb = xlApp.Workbooks.Add(Type.Missing);
                        //templateOrFileLoaded = false;
                    }

                    ws = (Excel.Worksheet)wb.Worksheets[1];
                    if (ws == null)
                    {
                        MessageBox.Show("Лист Excel не может быть создан.");
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        wb = xlApp.Workbooks.Open(fileName, Type.Missing, false, Type.Missing, Type.Missing,
                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                            Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        xlApp.Visible = true;
                        return false;
                    }
                    if (wb == null)
                    {
                        MessageBox.Show("Ошибка открытия файла.");
                        xlApp.Visible = true;
                        return false;
                    }
                    ws = (Excel.Worksheet)wb.Worksheets.Add(Type.Missing, Type.Missing, 1, Type.Missing);

                    if (ws == null)
                    {
                        MessageBox.Show("Лист Excel не может быть создан.");
                        xlApp.Visible = true;
                        return false;
                    }

                    try
                    {
                        Excel.Range rng = ws.get_Range("A1", "BB2000");
                        rng.NumberFormat = "@";
                    }
                    catch { }

                    //templateOrFileLoaded = false;
                    ((Excel._Worksheet)ws).Activate();
                }

#if DEBUG
                xlApp.Visible = true;
#else
                if (inThread)
                {
                    /*!!!!!!!!!!!!!!!!!!!!!!!*/
                    xlApp.Interactive = false;
                    xlApp.ScreenUpdating = false;
                    /*!!!!!!!!!!!!!!!!!!!!!!!*/
                }
#endif
                try
                {
                    style = wb.Styles.Add("Names", Type.Missing);
                    style.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

                    borderTop = style.Borders[Excel.XlBordersIndex.xlEdgeTop];
                    borderTop.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderTop.Weight = Excel.XlBorderWeight.xlThin;

                    borderBottom = style.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                    borderBottom.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderBottom.Weight = Excel.XlBorderWeight.xlThin;

                    borderLeft = style.Borders[Excel.XlBordersIndex.xlEdgeLeft];
                    borderLeft.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderLeft.Weight = Excel.XlBorderWeight.xlThin;

                    borderRight = style.Borders[Excel.XlBordersIndex.xlEdgeRight];
                    borderRight.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderRight.Weight = Excel.XlBorderWeight.xlThin;
                    //templateOrFileLoaded = false;
                }
                catch { /*templateOrFileLoaded = true;*/}
                //if (!templateOrFileLoaded)
                //{
                //Стиль "очки"
                try
                {
                    style = wb.Styles.Add("Points", Type.Missing);

                    borderTop = style.Borders[Excel.XlBordersIndex.xlEdgeTop];
                    borderTop.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderTop.Weight = Excel.XlBorderWeight.xlThin;

                    borderBottom = style.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                    borderBottom.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderBottom.Weight = Excel.XlBorderWeight.xlThin;

                    borderLeft = style.Borders[Excel.XlBordersIndex.xlEdgeLeft];
                    borderLeft.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderLeft.Weight = Excel.XlBorderWeight.xlThin;

                    borderRight = style.Borders[Excel.XlBordersIndex.xlEdgeRight];
                    borderRight.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderRight.Weight = Excel.XlBorderWeight.xlThin;

                    style.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    //Стиль "очки" - конец
                }
                catch { }

                //Стиль "команды"
                try
                {
                    style = wb.Styles.Add("Teams", Type.Missing);

                    borderTop = style.Borders[Excel.XlBordersIndex.xlEdgeTop];
                    borderTop.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderTop.Weight = Excel.XlBorderWeight.xlThin;

                    borderBottom = style.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                    borderBottom.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderBottom.Weight = Excel.XlBorderWeight.xlThin;

                    borderLeft = style.Borders[Excel.XlBordersIndex.xlEdgeLeft];
                    borderLeft.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderLeft.Weight = Excel.XlBorderWeight.xlThin;

                    borderRight = style.Borders[Excel.XlBordersIndex.xlEdgeRight];
                    borderRight.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderRight.Weight = Excel.XlBorderWeight.xlThin;

                    style.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    style.Font.Bold = true;
                    //Стиль "команды" - конец
                }
                catch { }
                try
                {
                    //Стиль "заголовок"
                    style = wb.Styles.Add("CompTitle", Type.Missing);
                    style.Font.Size = 16;
                    style.Font.Bold = true;
                    style.WrapText = true;
                    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    style.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                }
                catch { }
                try
                {

                    style = wb.Styles.Add("Title", Type.Missing);
                    style.Font.Bold = true;
                    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    style.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                }
                catch { }

                try
                {
                    style = wb.Styles.Add("StyleRA", Type.Missing);
                    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                }
                catch { }

                try
                {
                    style = wb.Styles.Add("MyStyle", Type.Missing);

                    borderLeft = style.Borders[Excel.XlBordersIndex.xlEdgeLeft];
                    borderLeft.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderLeft.Weight = Excel.XlBorderWeight.xlThin;

                    borderRight = style.Borders[Excel.XlBordersIndex.xlEdgeRight];
                    borderRight.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderRight.Weight = Excel.XlBorderWeight.xlThin;

                    borderTop = style.Borders[Excel.XlBordersIndex.xlEdgeTop];
                    borderTop.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderTop.Weight = Excel.XlBorderWeight.xlThin;

                    borderBottom = style.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                    borderBottom.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderBottom.Weight = Excel.XlBorderWeight.xlThin;

                    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    style.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                }
                catch { }

                try
                {
                    style = wb.Styles.Add("MyStyle2", Type.Missing);

                    borderLeft = style.Borders[Excel.XlBordersIndex.xlEdgeLeft];
                    borderLeft.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderLeft.Weight = Excel.XlBorderWeight.xlThin;

                    borderRight = style.Borders[Excel.XlBordersIndex.xlEdgeRight];
                    borderRight.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderRight.Weight = Excel.XlBorderWeight.xlThin;

                    borderTop = style.Borders[Excel.XlBordersIndex.xlEdgeTop];
                    borderTop.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderTop.Weight = Excel.XlBorderWeight.xlThin;

                    borderBottom = style.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                    borderBottom.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderBottom.Weight = Excel.XlBorderWeight.xlThin;

                    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    style.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                }
                catch { }

                try
                {
                    style = wb.Styles.Add("StyleLA", Type.Missing);

                    borderLeft = style.Borders[Excel.XlBordersIndex.xlEdgeLeft];
                    borderLeft.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderLeft.Weight = Excel.XlBorderWeight.xlThin;

                    borderRight = style.Borders[Excel.XlBordersIndex.xlEdgeRight];
                    borderRight.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderRight.Weight = Excel.XlBorderWeight.xlThin;

                    borderTop = style.Borders[Excel.XlBordersIndex.xlEdgeTop];
                    borderTop.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderTop.Weight = Excel.XlBorderWeight.xlThin;

                    borderBottom = style.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                    borderBottom.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderBottom.Weight = Excel.XlBorderWeight.xlThin;

                    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    style.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                }
                catch { }

                try
                {
                    style = wb.Styles.Add("al_left", Type.Missing);

                    borderLeft = style.Borders[Excel.XlBordersIndex.xlEdgeLeft];
                    borderLeft.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderLeft.Weight = Excel.XlBorderWeight.xlThin;
                    borderTop = style.Borders[Excel.XlBordersIndex.xlEdgeRight];
                    borderTop.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderTop.Weight = Excel.XlBorderWeight.xlThin;
                    borderBottom = style.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                    borderBottom.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderBottom.Weight = Excel.XlBorderWeight.xlThin;
                }
                catch { }
                try
                {
                    style = wb.Styles.Add("al_right", Type.Missing);

                    borderRight = style.Borders[Excel.XlBordersIndex.xlEdgeRight];
                    borderRight.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderRight.Weight = Excel.XlBorderWeight.xlThin;
                    borderTop = style.Borders[Excel.XlBordersIndex.xlEdgeTop];
                    borderTop.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderTop.Weight = Excel.XlBorderWeight.xlThin;
                    borderBottom = style.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                    borderBottom.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderBottom.Weight = Excel.XlBorderWeight.xlThin;
                }
                catch { }
                try
                {
                    style = wb.Styles.Add("font11", Type.Missing);
                    style.Font.Name = "Tahoma";
                    style.Font.Size = 11;
                    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    style.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    borderLeft = style.Borders[Excel.XlBordersIndex.xlEdgeLeft];
                    borderRight = style.Borders[Excel.XlBordersIndex.xlEdgeRight];
                    borderBottom = style.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                    borderTop = style.Borders[Excel.XlBordersIndex.xlEdgeTop];
                    borderTop.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderBottom.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderRight.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderLeft.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderTop.Weight = Excel.XlBorderWeight.xlThin;
                    borderBottom.Weight = Excel.XlBorderWeight.xlThin;
                    borderRight.Weight = Excel.XlBorderWeight.xlThin;
                    borderLeft.Weight = Excel.XlBorderWeight.xlThin;
                }
                catch { }
                try
                {
                    style = wb.Styles.Add("font10", Type.Missing);
                    style.Font.Name = "Tahoma";
                    style.Font.Size = 10;
                    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    style.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    borderLeft = style.Borders[Excel.XlBordersIndex.xlEdgeLeft];
                    borderRight = style.Borders[Excel.XlBordersIndex.xlEdgeRight];
                    borderBottom = style.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                    borderTop = style.Borders[Excel.XlBordersIndex.xlEdgeTop];
                    borderTop.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderBottom.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderRight.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderLeft.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderTop.Weight = Excel.XlBorderWeight.xlThin;
                    borderBottom.Weight = Excel.XlBorderWeight.xlThin;
                    borderRight.Weight = Excel.XlBorderWeight.xlThin;
                    borderLeft.Weight = Excel.XlBorderWeight.xlThin;
                }
                catch { }
                try
                {
                    style = wb.Styles.Add("font11_CA", Type.Missing);
                    style.Font.Name = "Tahoma";
                    style.Font.Size = 11;
                    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    style.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    borderLeft = style.Borders[Excel.XlBordersIndex.xlEdgeLeft];
                    borderRight = style.Borders[Excel.XlBordersIndex.xlEdgeRight];
                    borderBottom = style.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                    borderTop = style.Borders[Excel.XlBordersIndex.xlEdgeTop];
                    borderTop.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderBottom.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderRight.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderLeft.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderTop.Weight = Excel.XlBorderWeight.xlThin;
                    borderBottom.Weight = Excel.XlBorderWeight.xlThin;
                    borderRight.Weight = Excel.XlBorderWeight.xlThin;
                    borderLeft.Weight = Excel.XlBorderWeight.xlThin;
                }
                catch { }
                try
                {
                    style = wb.Styles.Add("top", Type.Missing);
                    style.Font.Name = "Tahoma";
                    style.Font.Size = 12;
                    style.Font.Bold = true;
                    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    style.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    borderLeft = style.Borders[Excel.XlBordersIndex.xlEdgeLeft];
                    borderRight = style.Borders[Excel.XlBordersIndex.xlEdgeRight];
                    borderBottom = style.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                    borderTop = style.Borders[Excel.XlBordersIndex.xlEdgeTop];
                    borderTop.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderBottom.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderRight.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderLeft.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderTop.Weight = Excel.XlBorderWeight.xlThin;
                    borderBottom.Weight = Excel.XlBorderWeight.xlThin;
                    borderRight.Weight = Excel.XlBorderWeight.xlThin;
                    borderLeft.Weight = Excel.XlBorderWeight.xlThin;
                }
                catch { }
                try
                {
                    style = wb.Styles.Add("font11c", Type.Missing);
                    style.Font.Name = "Tahoma";
                    style.Font.Size = 11;
                    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    style.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    borderLeft = style.Borders[Excel.XlBordersIndex.xlEdgeLeft];
                    borderRight = style.Borders[Excel.XlBordersIndex.xlEdgeRight];
                    borderBottom = style.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                    borderTop = style.Borders[Excel.XlBordersIndex.xlEdgeTop];
                    borderTop.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderBottom.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderRight.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderLeft.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderTop.Weight = Excel.XlBorderWeight.xlThin;
                    borderBottom.Weight = Excel.XlBorderWeight.xlThin;
                    borderRight.Weight = Excel.XlBorderWeight.xlThin;
                    borderLeft.Weight = Excel.XlBorderWeight.xlThin;
                }
                catch { }
                try
                {
                    style = wb.Styles.Add("font10c", Type.Missing);
                    style.Font.Name = "Tahoma";
                    style.Font.Size = 10;
                    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    style.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    borderLeft = style.Borders[Excel.XlBordersIndex.xlEdgeLeft];
                    borderRight = style.Borders[Excel.XlBordersIndex.xlEdgeRight];
                    borderBottom = style.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                    borderTop = style.Borders[Excel.XlBordersIndex.xlEdgeTop];
                    borderTop.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderBottom.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderRight.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderLeft.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderTop.Weight = Excel.XlBorderWeight.xlThin;
                    borderBottom.Weight = Excel.XlBorderWeight.xlThin;
                    borderRight.Weight = Excel.XlBorderWeight.xlThin;
                    borderLeft.Weight = Excel.XlBorderWeight.xlThin;
                }
                catch { }
                try
                {
                    style = wb.Styles.Add("font11_CAc", Type.Missing);
                    style.Font.Name = "Tahoma";
                    style.Font.Size = 11;
                    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    style.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    borderLeft = style.Borders[Excel.XlBordersIndex.xlEdgeLeft];
                    borderRight = style.Borders[Excel.XlBordersIndex.xlEdgeRight];
                    borderBottom = style.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                    borderTop = style.Borders[Excel.XlBordersIndex.xlEdgeTop];
                    borderTop.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderBottom.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderRight.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderLeft.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderTop.Weight = Excel.XlBorderWeight.xlThin;
                    borderBottom.Weight = Excel.XlBorderWeight.xlThin;
                    borderRight.Weight = Excel.XlBorderWeight.xlThin;
                    borderLeft.Weight = Excel.XlBorderWeight.xlThin;
                }
                catch { }
                try
                {
                    style = wb.Styles.Add("font11_CA_uc", Type.Missing);
                    style.Font.Name = "Tahoma";
                    style.Font.Size = 11;
                    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    style.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    borderLeft = style.Borders[Excel.XlBordersIndex.xlEdgeLeft];
                    borderRight = style.Borders[Excel.XlBordersIndex.xlEdgeRight];
                    borderBottom = style.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                    borderTop = style.Borders[Excel.XlBordersIndex.xlEdgeTop];
                    borderTop.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderBottom.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderRight.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderLeft.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderTop.Weight = Excel.XlBorderWeight.xlThin;
                    borderBottom.Weight = Excel.XlBorderWeight.xlThin;
                    borderRight.Weight = Excel.XlBorderWeight.xlThin;
                    borderLeft.Weight = Excel.XlBorderWeight.xlThin;
                    style.Orientation = Excel.XlOrientation.xlUpward;
                }
                catch { }
                try
                {
                    style = wb.Styles.Add("topc", Type.Missing);
                    style.Font.Name = "Tahoma";
                    style.Font.Size = 12;
                    style.Font.Bold = true;
                    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    style.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    borderLeft = style.Borders[Excel.XlBordersIndex.xlEdgeLeft];
                    borderRight = style.Borders[Excel.XlBordersIndex.xlEdgeRight];
                    borderBottom = style.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                    borderTop = style.Borders[Excel.XlBordersIndex.xlEdgeTop];
                    borderTop.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderBottom.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderRight.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderLeft.LineStyle = Excel.XlLineStyle.xlContinuous;
                    borderTop.Weight = Excel.XlBorderWeight.xlThin;
                    borderBottom.Weight = Excel.XlBorderWeight.xlThin;
                    borderRight.Weight = Excel.XlBorderWeight.xlThin;
                    borderLeft.Weight = Excel.XlBorderWeight.xlThin;
                }
                catch { }
                try
                {
                    //Фамилия-Имя
                    style = wb.Styles.Add("StyleName", Type.Missing);
                    style.Font.Name = "Comic Sans MS";
                    style.Font.Size = 11;
                    style.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    style.VerticalAlignment = Excel.XlVAlign.xlVAlignBottom;
                }
                catch { }
                try
                {
                    //Группа
                    Excel.Style styleGroup = wb.Styles.Add("StyleGroup", Type.Missing);
                    styleGroup.Font.Name = "Comic Sans MS";
                    styleGroup.Font.Size = 11;
                    styleGroup.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                    styleGroup.VerticalAlignment = Excel.XlVAlign.xlVAlignBottom;
                }
                catch { }
                try
                {
                    //Номер
                    Excel.Style styleNumber = wb.Styles.Add("StyleNumber", Type.Missing);
                    styleNumber.Font.Name = "Comic Sans MS";
                    styleNumber.Font.Size = 160;
                    styleNumber.Font.Bold = true;
                    styleNumber.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    styleNumber.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                }
                catch { }
                try
                {
                    //Соревнования
                    Excel.Style styleComp = wb.Styles.Add("StyleComp", Type.Missing);
                    styleComp.Font.Name = "Comic Sans MS";
                    styleComp.Font.Size = 24;
                    styleComp.Font.Bold = true;
                    styleComp.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    styleComp.VerticalAlignment = Excel.XlVAlign.xlVAlignBottom;
                }
                catch { }
                //}
                

                if (copyright)
                {
                    PrintCopyright(xlApp, ws);
                }
                ws.PageSetup.Orientation = Excel.XlPageOrientation.xlPortrait;

                if (footer)
                {
                    PrintFooter(ws, cn, _tran);
                }

                ws.PageSetup.CenterHorizontally = true;
                ws.PageSetup.Zoom = false;
                ws.PageSetup.FitToPagesWide = 1;
                if (SettingsForm.GetOneListExcel(cn,_tran ))
                    ws.PageSetup.FitToPagesTall = 1;
                else
                {
                    int im = 32767;
                    while (im > 2)
                        try
                        {
                            ws.PageSetup.FitToPagesTall = im;
                            break;
                        }
                        catch { im /= 2; }
                }
                ws.PageSetup.PrintComments = Excel.XlPrintLocation.xlPrintNoComments;

                //ПОЛЯ ДЛЯ СОЛОВАРОВОЙ
                //try
                //{
                //    ws.PageSetup.LeftMargin = xlApp.InchesToPoints(0.275590551181102);
                //    ws.PageSetup.RightMargin = xlApp.InchesToPoints(0.275590551181102);
                //    ws.PageSetup.TopMargin = xlApp.InchesToPoints(0.47244094488189);
                //    ws.PageSetup.BottomMargin = xlApp.InchesToPoints(1.25984251968504);
                //    ws.PageSetup.HeaderMargin = xlApp.InchesToPoints(0.236220472440945);
                //    ws.PageSetup.FooterMargin = xlApp.InchesToPoints(0.669291338582677);
                //}
                //catch { }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка запуска Excel.\r\n" + ex.Message);
                SetExcelVisible(xlApp);
                return false;
            }
        }

        public static void ExcelUnderlineQf(Excel.Worksheet ws)
        {
#if !DEBUG
            try
            {
#endif
                int lastQfRow = -1, qfColumn = -1, emptyLineCounter = 0;
                Excel.Range r;
                string s;
                for (int row = 1; row < 200; row++)
                {
                    if (qfColumn < 1)
                    {
                        bool lineEmpty = true;
                        for (int col = 1; col < 50; col++)
                        {
                            try
                            {
                                r = (Excel.Range)ws.Cells[row, col];
                                s = r.Text.ToString().Trim();
                                if (s.Equals("Q", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    lastQfRow = row;
                                    qfColumn = col;
                                    break;
                                }
                                else if (lineEmpty && !String.IsNullOrEmpty(s))
                                {
                                    lineEmpty = false;
                                    emptyLineCounter = 0;
                                }
                            }
                            catch { }
                        }
                        if (lineEmpty)
                            emptyLineCounter++;
                        if (emptyLineCounter > 5)
                            return;
                    }
                    else
                    {
                        r = (Excel.Range)ws.Cells[row, qfColumn];
                        s = r.Text.ToString().Trim();
                        if (s.Equals("Q", StringComparison.InvariantCultureIgnoreCase))
                            lastQfRow = row;
                    }
                }
                if (lastQfRow < 1 || qfColumn < 1)
                    return;
                r = ws.get_Range(ws.Cells[lastQfRow, 1], ws.Cells[lastQfRow, qfColumn]);
                r.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlMedium;
#if !DEBUG
            }
            catch { }
#endif
        }

        public static void PrintCopyright(Excel.Application xlApp, Excel.Worksheet ws)
        {
            string asmName;
            string version = GetVersion();
#if FULL
            asmName = "Climbing Competition";
#else
                    asmName = "Speed Competition";
#endif

            if (Trial)
                ws.PageSetup.LeftHeader = "Powered by " + asmName + " " + version + " (пробная версия, осталось " +
                    TrialLeft.ToString() + " дней)";
            else
                ws.PageSetup.LeftHeader = "Powered by " + asmName + " " + version;
            ws.PageSetup.RightHeader = "© Ivan Kaurov 2006-" + DateTime.Now.Year.ToString();
#if CFR
            if (File.Exists("Templates\\fsr_banner.png"))
            {
                FileInfo fi = new FileInfo("Templates\\fsr_banner.png");
                ws.PageSetup.CenterHeaderPicture.Filename = fi.FullName;
                ws.PageSetup.TopMargin = xlApp.InchesToPoints(0.78740157480315);
                ws.PageSetup.HeaderMargin = xlApp.InchesToPoints(0.236220472440945);
                ws.PageSetup.CenterHeader = String.Format("{0}&G", (char)10);
            }
#endif
        }

        //Подписи 83px H
        public static void PrintFooter(Excel.Worksheet ws, SqlConnection cn, SqlTransaction tran = null)
        {
            string pr = "", prCat = "", sec = "", secCat = "";
            int iid;
            try
            {
                ws.PageSetup.CenterFooter = ws.PageSetup.LeftFooter = ws.PageSetup.RightFooter = String.Empty;
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT iid, category FROM judgeView WHERE pos = 'Главный судья'", cn);
                cmd.Transaction = tran;
                try
                {
                    SqlDataReader sdr = cmd.ExecuteReader();
                    iid = 0;
                    while (sdr.Read())
                    {
                        iid = Convert.ToInt32(sdr["iid"]);
                        prCat = sdr["category"].ToString();
                        break;
                    }
                    sdr.Close();
                    if (iid > 0)
                        pr = StaticClass.GetJudgeData(iid, cn, false);
                }
                catch
                { pr = ""; prCat = ""; }

                cmd.CommandText = "SELECT iid, category FROM judgeView(NOLOCK) WHERE pos = 'Главный секретарь'";

                try
                {
                    SqlDataReader sdr = cmd.ExecuteReader();
                    iid = 0;
                    while (sdr.Read())
                    {
                        iid = Convert.ToInt32(sdr["iid"]);
                        secCat = sdr["category"].ToString();
                        break;
                    }
                    sdr.Close();
                    if (iid > 0)
                        sec = StaticClass.GetJudgeData(iid, cn, false);
                }
                catch
                { sec = ""; secCat = ""; }
                if (pr != "")
                {
                    ws.PageSetup.LeftFooter = "Гл. судья";
                    if (prCat != "")
                        ws.PageSetup.LeftFooter += " (" + prCat + "):";
                    else
                        ws.PageSetup.LeftFooter += ":";
                    ws.PageSetup.RightFooter = pr;
                    //ws.PageSetup.CenterFooter = "Гл. судья:                                    " + pr;
                    if (sec != "")
                    {
                        //ws.PageSetup.CenterFooter += "\r\n";
                        ws.PageSetup.LeftFooter += "\r\n";
                        ws.PageSetup.RightFooter += "\r\n";
                    }
                }
                if (sec != "")
                {
                    //ws.PageSetup.CenterFooter += "Гл. секретарь:                                    " + sec;
                    ws.PageSetup.LeftFooter += "Гл. секретарь";
                    if (secCat != "")
                        ws.PageSetup.LeftFooter += " (" + secCat + "):";
                    else
                        ws.PageSetup.LeftFooter += ":";
                    ws.PageSetup.RightFooter += sec;
                }
            }
            catch { }
        }

        public delegate void ExecutionFinishedEventHandler(object sender, EventArgs e);
        public static event ExecutionFinishedEventHandler ExecutionFinished;

        private static Thread ExcelWorkingThread = null;

        public static void SetExcelVisible(Excel.Application xlApp)
        {
            try
            {
                if (xlApp == null)
                    return;
                xlApp.Interactive = true;
                xlApp.ScreenUpdating = true;
                xlApp.Visible = true;
                if (ExecutionFinished != null)
                    ExecutionFinished(null, new EventArgs());
            }
            catch { }
        }

        /// <summary>
        /// Возвращает версию выполняемой программы
        /// </summary>
        /// <returns></returns>
        public static string GetVersion()
        {
            try
            {
                var version = Application.ProductVersion;
                if (!string.IsNullOrWhiteSpace(version))
                {
                    return version;
                }
            }
            catch { }

            try
            {
                System.Reflection.Assembly asm;
                asm = System.Reflection.Assembly.GetEntryAssembly();
                if (asm == null)
                    asm = System.Reflection.Assembly.GetExecutingAssembly();
                
                if (asm != null)
                {
                    Version v = asm.GetName().Version;
                    return v.Major.ToString() + "." + v.Minor.ToString() + "." + v.Build.ToString();
                }
            }
            catch { }
            return string.Empty;
        }

        /// <summary>
        /// Запускает Word с пустым документом
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        public static bool LaunchWord(out Word.Document doc, out Word.Application app)
        {
            return LaunchWord("", out doc, out app);
        }

        /// <summary>
        /// Запускает Word с заданным шаблоном
        /// </summary>
        /// <param name="template"></param>
        /// <param name="doc"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        public static bool LaunchWord(string template, out Word.Document doc, out Word.Application app)
        {
            doc = null;
            app = null;
            try
            {
                app = new Word.Application();
                if (app == null)
                {
                    throw new Exception();
                }
                object miss = Type.Missing;
                if (template.IndexOf('\\') == 0)
                    template = template.Substring(1);
                if (template == "")
                    doc = app.Documents.Add(ref miss, ref miss, ref miss, ref miss);
                else
                {
                    object obj = template;
                    doc = app.Documents.Add(ref obj, ref miss, ref miss, ref miss);
                }
                if (doc == null)
                {
                    MessageBox.Show("Документ Word не может быть создан.");
                    app.Visible = true;
                    return false;
                }

                //if (Trial)
                //    ws.PageSetup.LeftHeader = "Powered by ClimbingCompetition 2.0 (пробная версия, осталось " +
                //        TrialLeft.ToString() + " дней)";
                //else
                //    ws.PageSetup.LeftHeader = "Powered by ClimbingCompetition 2.0";
                //ws.PageSetup.RightHeader = "© Ivan Kaurov 2006-" + DateTime.Now.Year.ToString();

                doc.Activate();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка запуска Microsoft Word.\r\n" + ex.Message);
                app.Visible = true;
                return false;
            }
        }

        /// <summary>
        /// Создаёт имя листа в Экселе
        /// </summary>
        /// <param name="gName"></param>
        /// <param name="round"></param>
        /// <returns></returns>
        public static string CreateSheetName(string gName, string round)
        {
            string strTmp = "";
            if (gName.IndexOf("одрост") > -1)
                gName = gName.Replace("остки", "");
            if (gName.IndexOf("ладш") > -1)
                gName = gName.Replace("адшие", "");
            if (gName.IndexOf("арш") > -1)
                gName = gName.Replace("аршие", "");
            if (gName.IndexOf("ужчи") > -1)
                gName = gName.Replace("ужчины", "");
            if (gName.IndexOf("енщ") > -1)
                gName = gName.Replace("енщины", "");
            if (gName.IndexOf("ноши") > -1)
                gName = gName.Replace("ноши", "");
            if (gName.IndexOf("евуш") > -1)
                gName = gName.Replace("евушки", "");
            if (gName.IndexOf("альч") > -1)
                gName = gName.Replace("альчики", "");
            if (gName.IndexOf("евоч") > -1)
                gName = gName.Replace("евочки", "");
            round = ClimbingCompetition.Online.ListCreator.GetShortRoundName(round);
            strTmp = (gName + "_" + round).Replace('/', '_');
            strTmp = strTmp.Replace('(', '_');
            strTmp = strTmp.Replace(')', '_');
            return strTmp.Replace(' ', '_');
        }

        /// <summary>
        /// Возвращает заголовок для формы по iid'у протокола
        /// </summary>
        /// <param name="iid"></param>
        /// <param name="cn"></param>
        /// <returns></returns>
        public static string GetListName(int iid, SqlConnection cn)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand(
                                 "SELECT ISNULL(G.name,'') name, L.style, L.round " +
                                   "FROM lists L(NOLOCK) " +
                              "LEFT JOIN groups G(NOLOCK) ON G.iid = L.group_id " +
                                  "WHERE L.iid = " + iid.ToString(), cn);
                SqlDataReader rdr = cmd.ExecuteReader();
                string s;
                try
                {
                    if (rdr.Read())
                        s = rdr["name"].ToString() + " " + rdr["style"].ToString() + " " + rdr["round"].ToString();
                    else
                        s = "";
                }
                finally { rdr.Close(); }
                return s;
            }
            catch { return ""; }
        }


        /// <summary>
        /// Строит последовательность предыдущих раундов для заданного протокола
        /// </summary>
        /// <param name="listID"></param>
        /// <param name="cn"></param>
        /// <param name="rounds"></param>
        /// <returns></returns>
        public static DataTable BuildRoundRow(int listID, SqlConnection cn, out List<roundN> rounds)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand(
                "SELECT prev_round, round, style FROM lists(NOLOCK) WHERE iid = @nxt", cn);
            cmd.Parameters.Add("@nxt", SqlDbType.Int);
            bool b;
            int curID = listID;
            rounds = new List<roundN>();
            do
            {
                b = false;
                cmd.Parameters[0].Value = curID;
                SqlDataReader rdr = cmd.ExecuteReader();
                try
                {
                    while (rdr.Read())
                    {
                        string style = rdr["style"].ToString();
                        b = true;
                        roundN rnd = new roundN();
                        rnd.iid = curID;
                        rnd.name = rdr["round"].ToString();
                        rnd.qf = (rnd.name == "Квалификация (2 трассы)") || (style == "Трудность" && rnd.name.IndexOf("Квалификация") > -1);
                        rnd.TwoRoutes = (rnd.name == "Квалификация (2 группы)" ||
                            rnd.name == "1/4 финала (2 трассы)");
                        rnd.havePreQf = false;
                        try { curID = (int)rdr["prev_round"]; }
                        catch { curID = 0; }
                        b = (curID != 0);
                        rounds.Add(rnd);
                        break;
                    }
                }
                finally { rdr.Close(); }

            } while (b);

            for (int iI = 1; iI < rounds.Count; iI++)
            {
                roundN rnd = rounds[iI];
                if (rnd.qf)
                    cmd.CommandText =
                        "SELECT COUNT(RLQ.climber_id) " +
                        "  FROM lists LM(NOLOCK) " +
                        "  JOIN lists LQ(NOLOCK) ON LQ.style = LM.style " +
                        "                       AND LQ.group_id = LM.group_id " +
                        "                       AND LQ.round = 'Квалификация 1' " +
                        "  JOIN routeResults RLQ(NOLOCK) ON RLQ.list_id = LQ.iid " +
                        " WHERE RLQ.preQf = 1 " +
                        "   AND LM.iid = " + rnd.iid.ToString();
                else
                    if (!rnd.TwoRoutes)
                        cmd.CommandText =
                            "   SELECT COUNT(l.climber_id) + COUNT(s.climber_id) + COUNT(b.climber_id) cnt " +
                            "     FROM lists LL(NOLOCK) " +
                            "LEFT JOIN routeResults l(NOLOCK) ON l.list_id = LL.iid AND l.preQf = 1" +
                            "LEFT JOIN speedResults s(NOLOCK) ON s.list_id = LL.iid AND l.preQf = 1" +
                            "LEFT JOIN boulderResults b(NOLOCK) ON b.list_id = LL.iid AND l.preQf = 1" +
                            "    WHERE LL.iid = " + rnd.iid.ToString();
                try { rnd.havePreQf = (Convert.ToInt32(cmd.ExecuteScalar()) > 0); }
                catch { }
                rounds[iI] = rnd;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("vk", typeof(bool));
            dt.Columns.Add("pos", typeof(int));
            dt.Columns.Add("posText", typeof(string));
            dt.Columns.Add("iid", typeof(int));
            dt.Columns.Add("qf", typeof(string));
            dt.Columns.Add("pts", typeof(double));
            dt.Columns.Add("ptsText", typeof(string));
            dt.Columns.Add("sp", typeof(int));

            for (int i = 0; i < rounds.Count; i++)
            {
                roundN rn = rounds[i];
                if (i > 0 && (rn.TwoRoutes || rn.havePreQf))
                    dt.Columns.Add("rNum" + i.ToString(), typeof(int));
                dt.Columns.Add("pos" + i.ToString(), typeof(int));
            }
            return dt;
        }

        /// <summary>
        /// Возвращает данные по судье (Ф.И.О, + если надо, то категория)
        /// </summary>
        /// <param name="judgeID"></param>
        /// <param name="cn"></param>
        /// <param name="inclCat"></param>
        /// <returns></returns>
        public static string GetJudgeData(int judgeID, SqlConnection cn, bool inclCat)
        {
            string retVal = String.Empty;
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT surname, name, patronimic, category " +
                                                "  FROM judges(NOLOCK) " +
                                                " WHERE iid = " + judgeID.ToString(), cn);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    retVal = rdr["surname"].ToString();
                    if (rdr["name"].ToString() != "")
                    {
                        //retVal += " " + rdr["name"].ToString()[0] + ".";
                        if (rdr["patronimic"].ToString() != "")
                            retVal += " " + rdr["name"].ToString()[0] + "." +
                                      rdr["patronimic"].ToString()[0] + ".";
                        else
                            retVal += " " + rdr["name"].ToString();
                    }
                    if (inclCat)
                        if (rdr["category"].ToString() != "")
                            retVal += " (" + rdr["category"].ToString() + ")";
                    break;
                }
                rdr.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            return retVal;
        }

        /// <summary>
        /// Проверяет наличие триггеров, срабатывающих на изменение данных
        /// </summary>
        /// <param name="cn"></param>
        public static void CheckChangedTriggers(SqlConnection cn)
        {
            AccountForm.CreateAllChangedTriggers(cn);
        }

        /// <summary>
        /// кодирует русские символы для передачи по блютусу на мобилу для боулдеринга
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static string EncodeChar(char c)
        {
            string s;
            char cStart;
            if (c == 'ё')
                return "r99";
            if (c == 'Ё')
                return "R99";
            if (c >= 'А' && c <= (char)('Я'))
            {
                s = "R";
                cStart = 'А';
            }
            else if (c >= 'а' && c <= (char)('я'))
            {
                s = "r";
                cStart = 'а';
            }
            else
                return c.ToString();
            for (int i = 0; i < 33; i++)
            {
                if (cStart == c)
                {
                    s += i.ToString();
                    return s;
                }
                cStart++;
            }
            return c.ToString();
        }

        /// <summary>
        /// кодирует строку для передачи по блютусу на мобилу для боулдеринга
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string EncodeString(string s)
        {
            string eS = "";
            bool isEncoding = false;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                string tmp = EncodeChar(c);
                if (tmp.Length > 1 && (!isEncoding))
                {
                    eS += "TRAN:" + tmp;
                    isEncoding = true;
                    continue;
                }
                if (tmp.Length == 1 && isEncoding)
                {
                    eS += ":NART" + tmp;
                    isEncoding = false;
                    continue;
                }
                eS += tmp;
            }
            if (isEncoding)
                eS += ":NART";
            return eS;
        }
        private static string DecodeRusPart(string s)
        {
            char[] cStp = new char[] { 'R', 'r', ':' };
            string dS = "";
            while (s.Length > 0)
            {
                int i = s.IndexOfAny(cStp, 1);
                char cStart;
                if (s[0] == 'R')
                    cStart = 'А';
                else
                    cStart = 'а';
                if (i < 0)
                    i = s.Length;
                string toDecode = s.Substring(1, i - 1);
                int num = int.Parse(toDecode);
                if (num == 99)
                {
                    if (s[0] == 'R')
                        dS += 'Ё';
                    else
                        dS += 'ё';
                }
                else
                {
                    char c = (char)(cStart + num);
                    dS += c;
                }
                if (i < s.Length)
                    s = s.Substring(i);
                else
                    s = "";
            }
            return dS;
        }

        /// <summary>
        /// Декодирует строку при приёме по блютусу с мобилы для боулдеринга
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string DecodeString(string s)
        {
            string dS = "";
            while (s.Length > 0)
            {
                int trInd = s.IndexOf("TRAN:");
                if (trInd < 0)
                    trInd = s.Length;
                dS += s.Substring(0, trInd);
                if (trInd < s.Length - 5)
                    s = s.Substring(trInd + 5);
                else
                    s = "";
                if (s.Length > 0)
                {
                    int iS = s.IndexOf(":NART");
                    if (iS < 0)
                        iS = s.Length;
                    try
                    {
                        string rusPart = s.Substring(0, iS);
                        rusPart = DecodeRusPart(rusPart);
                        dS += rusPart;
                    }
                    catch
                    {
                        dS += s;
                        return dS;
                    }
                    if (iS < s.Length - 5)
                        s = s.Substring(iS + 5);
                    else
                        s = "";
                }
            }
            return dS;
        }

        public static bool IsListOnline(int iid, SqlConnection cn, SqlTransaction tran)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT online FROM lists(NOLOCK) WHERE iid=" + iid.ToString(), cn);
            if (tran != null)
                cmd.Transaction = tran;
            object oTmp = cmd.ExecuteScalar();
            if (oTmp != null && oTmp != DBNull.Value)
                return Convert.ToBoolean(oTmp);
            return false;
        }

        private class UpdListStruct
        {
            public int Iid { get; set; }
            private String style;
            public String Style { get { return style; } set { style = value == null ? String.Empty : value.ToLowerInvariant(); } }
            public int[] Climbers { get; set; }
            public UpdListStruct()
            {
                this.Climbers = new int[0];
                this.style = String.Empty;
            }
        }
        private static bool UpdateListData(bool updateAll, SqlConnection cn, XmlClient client)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand { Connection = cn };
            cmd.CommandText = "SELECT L.iid, L.style" +
                              "  FROM lists L(nolock)" +
                              " WHERE L.online=1 "+
                              "   AND style IN('Трудность','Скорость','Боулдеринг') ";
            if (!updateAll)
                cmd.CommandText +=
                    "AND (L.changed=1" +
                    " OR EXISTS(SELECT * FROM routeResults r(NOLOCK) WHERE r.list_id = l.iid AND r.changed=1) " +
                    " OR EXISTS(SELECT * FROM speedResults s(NOLOCK) WHERE s.list_id = l.iid AND s.changed=1) " +
                    " OR EXISTS(SELECT 1 " +
                    "             FROM boulderResults b(NOLOCK) " +
                    "        LEFT JOIN boulderRoutes br(nolock) on br.iid_parent = b.iid" +
                    "            WHERE b.list_id = l.iid" +
                    "              AND (b.changed=1 OR ISNULL(br.changed,0) = 1)" +
                    "          )";
            List<UpdListStruct> listToReload = new List<UpdListStruct>();
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    listToReload.Add(new UpdListStruct
                    {
                        Iid = Convert.ToInt32(rdr["iid"]),
                        Style = rdr["style"] == DBNull.Value ? null : rdr["style"].ToString()
                    });
                }
            }
            if (listToReload.Count < 1)
                return true;
            List<ApiListLine> dataToUpdate = new List<ApiListLine>();
            foreach (var ltrl in listToReload)
            {
                ApiListLine[] listResult;
                switch (ltrl.Style)
                {
                    case "трудность":
                        listResult = GetLeadRefreshData(ltrl.Iid, cn, updateAll);
                        break;
                    case "скорость":
                        listResult = GetSpeedRefreshData(ltrl.Iid, cn, updateAll);
                        break;
                    case "боулдеринг":
                        listResult = GetBoulderRefreshData(ltrl.Iid, cn, updateAll);
                        break;
                    default:
                        continue;
                }
                ltrl.Climbers = new int[listResult.Length];
                for (int i = 0; i < listResult.Length; i++)
                    ltrl.Climbers[i] = listResult[i].ClimberID;
                dataToUpdate.AddRange(listResult);
            }
            if (dataToUpdate.Count < 1)
                return true;
            return client.LoadResultsPackage(new ApiListLineCollection(dataToUpdate));
        }
        /// <summary>
        /// Обновляет данные на сайте
        /// </summary>
        /// <param name="updateAll"></param>
        /// <param name="local"></param>
        /// <param name="remote"></param>
        /// <param name="fullReload"></param>
        /// <returns></returns>
        public static bool UpdateListData(bool updateAll, SqlConnection local, SqlConnection remote, bool fullReload, long? compIDforService, XmlClient client)
        {
            if (client != null)
                return UpdateListData(updateAll, local, client);
            if (local.State != ConnectionState.Open)
                local.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = local;
            cmd.CommandText = "SELECT iid FROM lists l(NOLOCK) WHERE online=1";
            if (!updateAll)
                cmd.CommandText += " AND (l.allowView=1" +
                    " OR EXISTS(SELECT * FROM routeResults r(NOLOCK) WHERE r.list_id = l.iid AND r.changed=1) " +
                    " OR EXISTS(SELECT * FROM speedResults s(NOLOCK) WHERE s.list_id = l.iid AND s.changed=1) " +
                    " OR EXISTS(SELECT * FROM boulderResults b(NOLOCK) WHERE b.list_id = l.iid AND b.changed=1) " +
                    " OR l.changed=1" +
                    ")";
            List<int> listsToUpdate = new List<int>();
            SqlDataReader dr = cmd.ExecuteReader();
            try
            {
                while (dr.Read())
                    listsToUpdate.Add(Convert.ToInt32(dr["iid"]));
            }
            finally { dr.Close(); }
            bool res = true;
            try
            {
                foreach (int i in listsToUpdate)
                    res = res && UpdateListData(i, local, remote, false, fullReload, compIDforService, client);
            }
            finally
            {
                try { remote.Close(); }
                catch { }
            }
            return res;
        }

        /// <summary>
        /// Обновляет данные на сайте
        /// </summary>
        /// <param name="listID"></param>
        /// <param name="local"></param>
        /// <param name="remote"></param>
        /// <param name="closeRemote"></param>
        /// <param name="fullReload"></param>
        /// <returns></returns>
        public static bool UpdateListData(int listID, SqlConnection local, SqlConnection remote, bool closeRemote, bool fullReload, long? compIDForService, XmlClient client)
        {
            if (local.State != ConnectionState.Open)
                local.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = local;
            cmd.CommandText = "SELECT style, round FROM lists(NOLOCK) WHERE iid = " + listID.ToString();
            string style, round;
            SqlDataReader dr = cmd.ExecuteReader();
            try
            {
                if (dr.Read())
                {
                    style = dr["style"].ToString();
                    round = dr["round"].ToString();
                }
                else
                    return false;
            }
            finally { dr.Close(); }
            int linesUpdated = 0;
            try
            {
                switch (style.ToString())
                {
                    case "Трудность":
                        switch (round)
                        {
                            case "Квалификация (2 трассы)":
                                break;
                            case "1/4 финала (2 трассы)":
                                break;
                            default:
                                linesUpdated = RefreshLead(listID.ToString(), local, remote, fullReload, compIDForService, client);
                                break;
                        }
                        break;
                    case "Боулдеринг":
                        if (round != "Квалификация (2 группы)")
                            linesUpdated = RefreshBoulder(listID.ToString(), local, remote, fullReload, compIDForService, client);
                        break;
                    case "Скорость":
                        linesUpdated = RefreshSpeed(listID.ToString(), local, remote, fullReload, compIDForService, client);
                        break;
                }
                if (linesUpdated > 0)
                {
                    try
                    {
                        if (remote.State != ConnectionState.Open)
                            remote.Open();
                        cmd.Connection = remote;
                        cmd.CommandText = "UPDATE ONLlists SET lastUpd=@lu WHERE iid=" + listID.ToString();
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@lu", SqlDbType.DateTime);
                        cmd.Parameters[0].Value = DateTime.Now.ToUniversalTime();
                        cmd.ExecuteNonQuery();
                    }
                    catch { }
                }
            }
            finally
            {
                if (closeRemote && compIDForService == null)
                    try { remote.Close(); }
                    catch { }
            }
            return true;
        }

        private static void UpdateCompleted(RequestResult result, Exception ex, object data)
        {
            if (result == RequestResult.Error)
                StaticClass.ShowExceptionMessageBox(String.Format("{0:T}. Ошибка обновления сайта", DateTime.Now), ex);
        }

        public static void UpdateOnline(SqlConnection local, SqlConnection remote, long? compForService, XmlClient client)
        {
            try
            {
                if (client != null)
                {
                    OnlineUpdater.BeginFullUpdate(false, local, client, UpdateCompleted, null, OnlineUpdater.UpdateStartMode.CancelIfUpdating);
                    return;
                }

                OnlineUpdater2.GetUpdater(local).PostUpdatedGroups();
                OnlineUpdater2.Instance.PostChangedClimbers();
                OnlineUpdater2.Instance.LoadChangedLists();

                /*
                RefreshClimberList(false, false, false, compForService, local, remote, client);
                //RefreshJudgesList(false, false, false, local, remote);
                ReloadChangedLists(local, remote, compForService, client);
                UpdateListData(false, local, remote, false, compForService, client);*/
            }
            finally
            {
                try { remote.Close(); }
                catch { }
            }
        }

        public static T? GetNullableValue<T>(object obj) where T : struct
        {
            try
            {
                if (obj == null || obj == DBNull.Value)
                    return null;
                return new T?((T)obj);
            }
            catch { return null; }
        }

        public static T GetValueTypeValue<T>(object obj, T defaultValue) where T : struct
        {
            T? retVal = GetNullableValue<T>(obj);
            return (retVal != null && retVal.HasValue) ? retVal.Value : defaultValue;
        }

        public static int RefreshLead(string list_id, SqlConnection localConnection, SqlConnection remoteConnection, bool fullReload, long? compIDForService, XmlClient client)
        {
            if (localConnection.State != ConnectionState.Open)
                localConnection.Open();
            SqlDataAdapter da = new SqlDataAdapter(new SqlCommand(
                @"SELECT r.climber_id, r.resText, r.res, r.preQf, r.start 
                    FROM routeResults r(NOLOCK) 
                   WHERE r.list_id=" + list_id, localConnection));
            if (!fullReload)
                da.SelectCommand.CommandText += " AND changed = 1";
            DataTable dTable = new DataTable();
            da.Fill(dTable);
            if (dTable.Rows.Count < 1)
                return 0;
            ClimbingService service;
            if (compIDForService != null && compIDForService.HasValue)
            {
                service = new ClimbingService();
                int listID = int.Parse(list_id);

                List<int> climbersToGet = new List<int>();
                foreach (DataRow dr in dTable.Rows)
                {
                    int n = GetValueTypeValue<int>(dr["climber_id"], -1);
                    if (n > 0 && !climbersToGet.Contains(n))
                        climbersToGet.Add(n);
                }
                Dictionary<int, LeadResult> resultsDict = new Dictionary<int, LeadResult>();
#if DOWNLOAD
                foreach (var v in service.GetLeadResultsForClimber(listID, climbersToGet.ToArray(), compIDForService.Value))
                    if (!resultsDict.ContainsKey(v.ClimberID_Secretary))
                        resultsDict.Add(v.ClimberID_Secretary, v);
#endif

                climbersToGet.Clear();
                List<LeadResult> results = new List<LeadResult>();
                foreach (DataRow dr in dTable.Rows)
                {
                    LeadResult lRes;
                    int climberid = Convert.ToInt32(dr["climber_id"]);
                    if (resultsDict.ContainsKey(climberid))
                        lRes = resultsDict[climberid];
                    else
                    {
                        lRes = new LeadResult();
                        lRes.ClimberID_Secretary = climberid;
                    }
                    
                    lRes.TextRes = (dr["resText"] == DBNull.Value || dr["resText"] == null) ? null : dr["resText"].ToString();
                    lRes.Res = GetNullableValue<long>(dr["res"]);
                    lRes.PreQf = GetValueTypeValue<bool>(dr["preQf"], false);
                    lRes.Start = GetNullableValue<int>(dr["start"]);
                    lRes.ListID_Secretary = listID;
                    climbersToGet.Add(lRes.ClimberID_Secretary);
                    results.Add(lRes);
                }
                int errCode;
                int nRet= service.PostLeadResultList(results.ToArray(), compIDForService.Value, fullReload, out errCode);
                try { SetChanged("routeResults", localConnection, climbersToGet.ToArray(), listID, false); }
                catch { }
                return nRet;
            }
            else
            {
                service = null;
                if (remoteConnection.State != ConnectionState.Open)
                    remoteConnection.Open();
                da.UpdateCommand = new SqlCommand(
                    "EXEC InsertLead " + list_id.ToString() + ", @cl, @st, @resT, @res, @preQf", remoteConnection);
                da.UpdateCommand.Parameters.Add("@cl", SqlDbType.Int, 4, "climber_id");
                da.UpdateCommand.Parameters.Add("@st", SqlDbType.Int, 4, "start");
                da.UpdateCommand.Parameters.Add("@resT", SqlDbType.VarChar, 50, "resText");
                da.UpdateCommand.Parameters.Add("@res", SqlDbType.BigInt, 8, "res");
                da.UpdateCommand.Parameters.Add("@preQf", SqlDbType.Bit, 1, "preQf");
                //da.Update(dTable);
                return UpdateDbFromDT(list_id, localConnection, da, dTable, "routeResults");
            }
        }

        public static string GetNullableString(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return null;
            else
                return obj.ToString();
        }
        
        private static ApiListLineSpeed[] GetSpeedRefreshData(int listId, SqlConnection cn, bool fullReload, SqlTransaction tran = null)
        {
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = tran };
            StringBuilder cText = new StringBuilder(
                "SELECT r.climber_id, r.start, r.route1, r.route1_text, r.route2, r.route2_text, r.res, r.resText" +
                "  FROM speedResults R(NOLOCK)" +
                " WHERE r.list_id=");
            cText.Append(listId);
            if (!fullReload)
                cText.Append(" AND R.changed=1");
            cmd.CommandText = cText.ToString();
            List<ApiListLineSpeed> result = new List<ApiListLineSpeed>();
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    result.Add(new ApiListLineSpeed
                    {
                        ClimberID = Convert.ToInt32(rdr["climber_id"]),
                        ListID = listId,
                        ResText = rdr["resText"] == DBNull.Value ? null : rdr["resText"].ToString(),
                        Result = rdr["res"] == DBNull.Value ? null : new long?(Convert.ToInt64(rdr["res"])),
                        Route1Data = rdr["route1"] == DBNull.Value ? null : new long?(Convert.ToInt64(rdr["route1"])),
                        Route1Text = rdr["route1_text"] == DBNull.Value ? null : rdr["route1_text"].ToString(),
                        Route2Data = rdr["route2"] == DBNull.Value ? null : new long?(Convert.ToInt64(rdr["route2"])),
                        Route2Text = rdr["route2_text"] == DBNull.Value ? null : rdr["route2_text"].ToString(),
                        StartNumber = Convert.ToInt32(rdr["start"])
                    });
                }
            }
            return result.ToArray();
        }

        private static ApiListLineLead[] GetLeadRefreshData(int listId, SqlConnection cn, bool fullReload, SqlTransaction tran = null)
        {
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = tran };
            StringBuilder cText = new StringBuilder(
                "SELECT r.climber_id, r.start,r.res, r.resText, r.timeText, r.timeValue" +
                "  FROM routeResults R(NOLOCK)" +
                " WHERE r.list_id=");
            cText.Append(listId);
            if (!fullReload)
                cText.Append(" AND R.changed=1");
            cmd.CommandText = cText.ToString();
            List<ApiListLineLead> result = new List<ApiListLineLead>();
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    result.Add(new ApiListLineLead
                    {
                        ClimberID = Convert.ToInt32(rdr["climber_id"]),
                        ListID = listId,
                        ResText = rdr["resText"] == DBNull.Value ? null : rdr["resText"].ToString(),
                        Result = rdr["res"] == DBNull.Value ? null : new long?(Convert.ToInt64(rdr["res"])),
                        StartNumber = Convert.ToInt32(rdr["start"]),
                        Time = rdr["timeText"] == DBNull.Value ? null : new int?(Convert.ToInt32(rdr["timeText"])),
                        TimeText = rdr["timeText"] == DBNull.Value ? String.Empty : rdr["timeText"].ToString()
                    });
                }
            }
            return result.ToArray();
        }

        private static ApiListLineBoulder[] GetBoulderRefreshData(int listId, SqlConnection cn, bool fullReload, SqlTransaction tran = null)
        {
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = tran };
            StringBuilder cText = new StringBuilder(
                "SELECT r.climber_id, r.start,ISNULL(nya,0) nya, ISNULL(disq, 0) disq" +
                "  FROM boulderResults R(NOLOCK)" +
                " WHERE r.list_id=");
            cText.Append(listId);
            if (!fullReload)
                cText.Append(" AND R.changed=1");
            cmd.CommandText = cText.ToString();
            List<ApiListLineBoulder> result = new List<ApiListLineBoulder>();
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    ResultLabel lbl;
                    if (Convert.ToBoolean(rdr["nya"]))
                        lbl = ResultLabel.DNS;
                    else if (Convert.ToBoolean(rdr["disq"]))
                        lbl = ResultLabel.DSQ;
                    else
                        lbl = ResultLabel.RES;
                    result.Add(new ApiListLineBoulder
                    {
                        ClimberID = Convert.ToInt32(rdr["climber_id"]),
                        ListID = listId,
                        ResultCode = lbl,
                        StartNumber = Convert.ToInt32(rdr["start"])
                    });
                }
            }
            if (result.Count > 0)
            {
                cmd.CommandText = String.Format("SELECT R.routeN, R.topA, R.bonusA" +
                                                "  FROM boulderRoutes R(nolock)" +
                                                "  JOIN boulderResults BR(nolock) on BR.iid = R.iid_parent" +
                                                " WHERE BR.list_id={0}" +
                                                "   AND BR.climber_id=@iid", listId);
                if (!fullReload)
                    cmd.CommandText += " AND R.changed=1";
                cmd.Parameters.Add("@iid", SqlDbType.Int);
                foreach (var res in result)
                {
                    List<ApiBoulderResultRoute> routes = new List<ApiBoulderResultRoute>();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            routes.Add(new ApiBoulderResultRoute
                            {
                                Bonus = rdr["bonusA"] == DBNull.Value ? null : new int?(Convert.ToInt32(rdr["bonusA"])),
                                Top = rdr["topA"] == DBNull.Value ? null : new int?(Convert.ToInt32(rdr["topA"])),
                                Route = Convert.ToInt32(rdr["routeN"])
                            });
                        }
                    }
                    res.Routes = routes.ToArray();
                }
            }
            return result.ToArray();
        }

        private static int RefreshSpeed(string list_id, SqlConnection cn, SqlConnection remoteConnection, bool fullReload, long? compIDForService, XmlClient client)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlDataAdapter da = new SqlDataAdapter(new SqlCommand(
                @"SELECT r.climber_id, r.start, r.route1_text, r.route2_text, r.resText,
                         r.res, r.preQf, ISNULL(r.qf,'') qf
                    FROM speedResults r(NOLOCK)
                   WHERE r.list_id = " + list_id, cn));
            if (!fullReload)
                da.SelectCommand.CommandText += " AND changed = 1";
            DataTable dTable = new DataTable();
            da.Fill(dTable);
            if (dTable.Rows.Count < 1)
                return 0;
            if (compIDForService != null && compIDForService.HasValue)
            {
                ClimbingService service = new ClimbingService();
                int listID = int.Parse(list_id);
                List<int> toUpdate = new List<int>();
                foreach (DataRow dr in dTable.Rows)
                {
                    int n = GetValueTypeValue<int>(dr["climber_id"], -1);
                    if (n > 0 && !toUpdate.Contains(n))
                        toUpdate.Add(n);
                }
                Dictionary<int, SpeedResult> resDict = new Dictionary<int, SpeedResult>();
#if DOWNLOAD
                foreach (var v in service.GetSpeedResultsForClimber(listID, toUpdate.ToArray(), compIDForService.Value))
                    if (!resDict.ContainsKey(v.ClimberID_Secretary))
                        resDict.Add(v.ClimberID_Secretary, v);
#endif
                List<SpeedResult> results = new List<SpeedResult>();
                toUpdate.Clear();
                foreach (DataRow dr in dTable.Rows)
                {
                    SpeedResult sr;
                    int nID = Convert.ToInt32(dr["climber_id"]);
                    if (resDict.ContainsKey(nID))
                        sr = resDict[nID];
                    else
                    {
                        sr = new SpeedResult();
                        sr.ClimberID_Secretary = nID;
                    }
                    sr.ListID_Secretary = listID;
                    sr.Start = GetNullableValue<int>(dr["start"]);
                    sr.Route1 = GetNullableString(dr["route1_text"]);
                    sr.Route2 = GetNullableString(dr["route2_text"]);
                    sr.TextRes = GetNullableString(dr["resText"]);
                    sr.Res = GetNullableValue<long>(dr["res"]);
                    sr.PreQf = GetValueTypeValue<bool>(dr["preQf"], false);
                    sr.Qf = GetNullableString(dr["qf"]);
                    toUpdate.Add(sr.ClimberID_Secretary);
                    results.Add(sr);
                }
                int errCode;
                int nRet = service.PostSpeedResultList(results.ToArray(), compIDForService.Value, fullReload, out errCode);
                try { SetChanged("speedResults", cn, toUpdate.ToArray(), listID, false); }
                catch { }
                return nRet;
            }
            else
            {
                if (remoteConnection.State != ConnectionState.Open)
                    remoteConnection.Open();
                da.UpdateCommand = new SqlCommand(
                    "EXEC InsertSpeed " + list_id.ToString() + ", @cl, @st, @r1, @r2, @res, @resI, @preQf, @qf", remoteConnection);
                da.UpdateCommand.Parameters.Add("@cl", SqlDbType.Int, 4, "climber_id");
                da.UpdateCommand.Parameters.Add("@st", SqlDbType.Int, 4, "start");
                da.UpdateCommand.Parameters.Add("@r1", SqlDbType.VarChar, 50, "route1_text");
                da.UpdateCommand.Parameters.Add("@r2", SqlDbType.VarChar, 50, "route2_text");
                da.UpdateCommand.Parameters.Add("@res", SqlDbType.VarChar, 50, "resText");
                da.UpdateCommand.Parameters.Add("@resI", SqlDbType.BigInt, 8, "res");
                da.UpdateCommand.Parameters.Add("@preQf", SqlDbType.Bit, 1, "preQf");
                da.UpdateCommand.Parameters.Add("@qf", SqlDbType.VarChar, 50, "qf");
                return UpdateDbFromDT(list_id, cn, da, dTable, "speedResults");
            }
        }

        private static string SetChanged(string tableName, SqlConnection cn, int[] climbers, int listID, bool value)
        {
            if (climbers == null || climbers.Length < 1)
                return String.Empty;
            string inStr = String.Empty;
            foreach (var v in climbers)
            {
                if (inStr.Equals(String.Empty))
                    inStr = " IN(";
                else
                    inStr += ",";
                inStr += v.ToString();
            }
            inStr += ") ";

            SqlCommand cmd = new SqlCommand();
            if (cn.State != ConnectionState.Open)
                cn.Open();
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE " + tableName + " SET changed = @p WHERE list_id=" + listID.ToString() +
                " AND climber_id " + inStr;
            cmd.Parameters.Add("@p", SqlDbType.Bit).Value = value;
            cmd.ExecuteNonQuery();
            return inStr;
        }

        private static int UpdateDbFromDT(string list_id, SqlConnection cn, SqlDataAdapter da, DataTable dTable, string tableName)
        {
            SqlCommand cmd = new SqlCommand(
                "UPDATE " + tableName + " SET changed=0 WHERE list_id=" + list_id + " AND climber_id=@clm", cn);

            if (cn.State != ConnectionState.Open)
                cn.Open();

            if (da.UpdateCommand.Connection.State != ConnectionState.Open)
                da.UpdateCommand.Connection.Open();
            cmd.Parameters.Add("@clm", SqlDbType.Int);
            List<int> insertedClm = new List<int>();
            try
            {
                foreach (DataRow dr in dTable.Rows)
                {
                    foreach (SqlParameter sp in da.UpdateCommand.Parameters)
                        sp.Value = null;
                    foreach (DataColumn dc in dTable.Columns)
                        foreach (SqlParameter sp in da.UpdateCommand.Parameters)
                            if (sp.SourceColumn == dc.ColumnName)
                            {
                                sp.Value = dr[dc];
                                break;
                            }
                    bool allowE = true;
                    foreach (SqlParameter sp in da.UpdateCommand.Parameters)
                        if (sp.Value == null)
                        {
                            allowE = false;
                            break;
                        }
                    if (allowE)
                    {
                        da.UpdateCommand.ExecuteNonQuery();
                        insertedClm.Add(Convert.ToInt32(da.UpdateCommand.Parameters[0].Value));
                    }
                }
            }
            finally
            {
                SetChanged(tableName, cn, insertedClm.ToArray(), int.Parse(list_id), false);
            }
            return insertedClm.Count;
        }

        private struct clmIdStr
        {
            public long resId;
            public int clmId;
        }
        private static int RefreshBoulder(int listID, DataTable dtRes, SqlConnection cn, bool fullReload, long compIDForService, XmlClient client)
        {
            ClimbingService service = new ClimbingService();

            
            List<int> toUpdate = new List<int>();
            foreach (DataRow dr in dtRes.Rows)
            {
                int n = GetValueTypeValue<int>(dr["climber_id"], -1);
                if (n > 0 && !toUpdate.Contains(n))
                    toUpdate.Add(n);
            }

            Dictionary<int, BoulderResult> resDict = new Dictionary<int, BoulderResult>();
#if DOWNLOAD
            foreach (var v in service.GetBoulderResultsForClimber(listID, toUpdate.ToArray(), compIDForService))
                if (!resDict.ContainsKey(v.ClimberID_Secretary))
                    resDict.Add(v.ClimberID_Secretary, v);
#endif
            toUpdate.Clear();
            List<BoulderResult> results = new List<BoulderResult>();

            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT routeN, topA, bonusA" +
                              "  FROM BoulderRoutes(NOLOCK)" +
                              " WHERE iid_parent=@resID";
            cmd.Parameters.Add("@resID", SqlDbType.BigInt);
            foreach (DataRow dr in dtRes.Rows)
            {
                int climberID = Convert.ToInt32(dr["climber_id"]);
                BoulderResult bRes;
                if (resDict.ContainsKey(climberID))
                    bRes = resDict[climberID];
                else
                {
                    bRes = new BoulderResult();
                    bRes.ClimberID_Secretary = climberID;
                }
                bRes.ListID_Secretary = listID;

                bRes.Start = GetNullableValue<int>(dr["start"]);
                bRes.Tops = GetNullableValue<int>(dr["Tops"]);
                bRes.TopAttempts = GetNullableValue<int>(dr["TopAttempts"]);
                bRes.Bonuses = GetNullableValue<int>(dr["Bonuses"]);
                bRes.BonusAttempts = GetNullableValue<int>(dr["BonusAttempts"]);
                bRes.DNS = GetValueTypeValue<bool>(dr["nya"], false);
                bRes.DSQ = GetValueTypeValue<bool>(dr["disq"], false);
                bRes.PreQf = GetValueTypeValue<bool>(dr["preQf"], false);
                bRes.Res = GetNullableValue<long>(dr["res"]);
                long resIdLine = GetValueTypeValue<long>(dr["res_id"], -1);
                cmd.Parameters[0].Value = resIdLine;
                List<BoulderRouteResult> routeList = new List<BoulderRouteResult>();
                var drd = cmd.ExecuteReader();
                try
                {
                    while (drd.Read())
                    {
                        BoulderRouteResult brr = new BoulderRouteResult();
                        brr.RouteNumber = GetValueTypeValue<int>(drd["routeN"], -1);
                        if (brr.RouteNumber < 1)
                            continue;
                        brr.TopAttempt = GetNullableValue<int>(drd["topA"]);
                        brr.BonusAttempt = GetNullableValue<int>(drd["bonusA"]);
                        routeList.Add(brr);
                    }
                }
                finally { drd.Close(); }
                bRes.RouteData = routeList.ToArray();
                toUpdate.Add(bRes.ClimberID_Secretary);
                results.Add(bRes);
            }
            int errCode;
            int nRet = service.PostBoulderResultList(results.ToArray(), compIDForService, fullReload, out errCode);
            try
            {
                var s = SetChanged("boulderResults", cn, toUpdate.ToArray(), listID, false);
                if (!s.Equals(String.Empty))
                {
                    cmd.CommandText = "UPDATE BoulderRoutes SET changed = 0 WHERE iid_parent IN(" +
                        " SELECT iid FROM boulderResults(NOLOCK)" +
                        "  WHERE list_id = " + listID.ToString() +
                        "    AND climber_id " + s + ")";
                    cmd.ExecuteNonQuery();
                }

            }
            catch { }
            return nRet;
        }
        public static int RefreshBoulder(string list_id, SqlConnection cn, SqlConnection remoteConnection, bool fullReload, long? compIDForService, XmlClient client)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd2 = new SqlCommand();
            cmd2.Connection = cn;
            cmd2.CommandText = "SELECT round FROM lists(NOLOCK) WHERE iid = " + list_id;
            string round = cmd2.ExecuteScalar().ToString().ToLower();
            if (round.IndexOf("супер") > -1)
                return RefreshLead(list_id, cn, remoteConnection, fullReload, compIDForService, client);
            cmd2.CommandText = "SELECT r.start, r.climber_id, ";
            cmd2.CommandText +=
                "Tops, TopAttempts,Bonuses, BonusAttempts,r.res, " +
                "r.nya, r.disq, r.preQf, r.iid res_id FROM boulderResults r(NOLOCK) WHERE " +
                "(r.list_id=" + list_id + ")";
            if (!fullReload)
                cmd2.CommandText += " AND ((r.changed = 1) OR EXISTS(SELECT * FROM boulderRoutes b(NOLOCK) WHERE b.changed = 1 AND b.iid_parent = r.iid))";

            SqlDataAdapter da = new SqlDataAdapter(cmd2);
            DataTable dTable = new DataTable();
            da.Fill(dTable);
            if (dTable.Rows.Count < 1)
                return 0;
            if (compIDForService != null && compIDForService.HasValue)
                return RefreshBoulder(int.Parse(list_id), dTable, cn, fullReload, compIDForService.Value, client);

            List<clmIdStr> idList = new List<clmIdStr>();
            foreach (DataRow rq in dTable.Rows)
            {
                clmIdStr s;
                s.resId = Convert.ToInt64(rq["res_id"]);
                s.clmId = Convert.ToInt32(rq["climber_id"]);
                idList.Add(s);
            }
            dTable.Columns.Remove("res_id");

            da.UpdateCommand = new SqlCommand();
            da.UpdateCommand.Connection = remoteConnection;
            da.UpdateCommand.CommandText = "EXEC InsertBoulder " + list_id.ToString() + ", @climber_id, @start,";
            
            da.UpdateCommand.CommandText += " @T, @Ta, @B, @Ba, @nya, @disq, @res, @preQf";

            da.UpdateCommand.Parameters.Add("@climber_id", SqlDbType.Int, 4, "climber_id");
            da.UpdateCommand.Parameters.Add("@start", SqlDbType.Int, 4, "start");

            da.UpdateCommand.Parameters.Add("@T", SqlDbType.Int, 4, "Tops");
            da.UpdateCommand.Parameters.Add("@Ta", SqlDbType.Int, 4, "TopAttempts");
            da.UpdateCommand.Parameters.Add("@B", SqlDbType.Int, 4, "Bonuses");
            da.UpdateCommand.Parameters.Add("@Ba", SqlDbType.Int, 4, "BonusAttempts");

            da.UpdateCommand.Parameters.Add("@preQf", SqlDbType.Bit, 1, "preQf");

            da.UpdateCommand.Parameters.Add("@nya", SqlDbType.Bit, 1, "nya");
            da.UpdateCommand.Parameters.Add("@disq", SqlDbType.Bit, 1, "disq");
            da.UpdateCommand.Parameters.Add("@res", SqlDbType.BigInt, 8, "res");
            int toRet = UpdateDbFromDT(list_id, cn, da, dTable, "boulderResults");

            cmd2.CommandText = "SELECT routeNumber FROM lists(NOLOCK) WHERE iid=" + list_id;
            int rn;
            try{rn = Convert.ToInt32(cmd2.ExecuteScalar());}
            catch { return toRet; }
            cmd2.CommandText = @"SELECT topA, bonusA
                                   FROM boulderRoutes(NOLOCK)
                                  WHERE iid_parent = @res_id
                                    AND routeN = @rN";
            cmd2.Parameters.Clear();
            cmd2.Parameters.Add("@res_id", SqlDbType.BigInt);
            cmd2.Parameters.Add("@rN", SqlDbType.Int);
            SqlCommand cmdU = new SqlCommand();
            cmdU.Connection = remoteConnection;
            cmdU.CommandText = "EXEC InsertBoulderRoute "+list_id+", @climber_id, @routeN, @topA, @bonusA";
            cmdU.Parameters.Add("@climber_id", SqlDbType.Int);
            cmdU.Parameters.Add("@routeN", SqlDbType.Int);
            cmdU.Parameters.Add("@topA", SqlDbType.Int);
            cmdU.Parameters.Add("@bonusA", SqlDbType.Int);
            SqlCommand cmdUL = new SqlCommand("UPDATE boulderRoutes SET changed = 0 WHERE routeN=@rN AND iid_parent=@ip", cn);
            cmdUL.Parameters.Add("@rN", SqlDbType.Int);
            cmdUL.Parameters.Add("@ip", SqlDbType.Int);
            foreach (var n in idList)
            {
                cmd2.Parameters[0].Value = n.resId;
                cmdU.Parameters[0].Value = n.clmId;
                cmdUL.Parameters[1].Value = n.resId;
                for (int r = 1; r <= rn; r++)
                {
                    cmd2.Parameters[1].Value = r;
                    cmdU.Parameters[1].Value = r;
                    
                    SqlDataReader rdr = cmd2.ExecuteReader();
                    object topA, bonusA;
                    bool dataExists;
                    try
                    {
                        if (rdr.Read())
                        {
                            topA = rdr["topA"];
                            bonusA = rdr["bonusA"];
                            dataExists = true;
                        }
                        else
                        {
                            topA = bonusA = DBNull.Value;
                            dataExists = false;
                        }
                    }
                    finally { rdr.Close(); }
                    cmdU.Parameters[2].Value = topA;
                    cmdU.Parameters[3].Value = bonusA;
                    cmdU.ExecuteNonQuery();
                    if (dataExists)
                    {
                        cmdUL.Parameters[0].Value = r;
                        cmdUL.ExecuteNonQuery();
                    }
                }
            }

            return toRet;
        }

        private static long changeTableIid(long oldIid, long newIid, String tableName, SqlConnection cn, SqlTransaction tran)
        {
            if (oldIid == newIid)
                return 0;
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = tran };
            cmd.CommandText = String.Format("SELECT COUNT(*) FROM {0} (NOLOCK) WHERE iid=@iid", tableName);
            cmd.Parameters.Add("@iid", SqlDbType.BigInt).Value = newIid;
            int n = Convert.ToInt32(cmd.ExecuteScalar());
            if (n > 0)
            {
                Random rnd = new Random();
                int nId;
                do
                {
                    nId = rnd.Next(1, int.MaxValue);
                    cmd.Parameters[0].Value = nId;
                    n = Convert.ToInt32(cmd.ExecuteScalar());
                } while (n > 0);
                return nId;
            }
            else
                return 0;
        }

        private static void RefreshClimberListMVC(bool fullRefresh, bool loadPhoto,
            SqlConnection cn, bool showMessages, XmlClient client)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                bool scs = false;
                try
                {
                    if (!RefreshGroupsMVC(fullRefresh, cn, showMessages, client, tran))
                        return;
                    if (!RefreshTeamsMVC(fullRefresh, cn, showMessages, client, tran))
                        return;
                    if (RefreshClimbersMVC(fullRefresh, cn, showMessages, client, tran) == RefreshResult.Error)
                        return;
                    scs = true;
                }
                finally
                {
                    if (scs)
                        tran.Commit();
                    else
                        tran.Rollback();
                }
                if (showMessages)
                    MessageBox.Show("Список участников успешно загружен");
            }
            catch (SqlException ex)
            {
                if (showMessages)
                    MessageBox.Show(String.Format("Ошибка выборки данных:{0}{1}", Environment.NewLine, ex.Message));
            }
            catch (WebException ex)
            {
                if (showMessages)
                    MessageBox.Show(String.Format("Ошибка обновления сайта:{0}{1}", Environment.NewLine, ex.Message));
            }
            catch (XmlException ex)
            {
                if (showMessages)
                    MessageBox.Show(String.Format("Ответ сайта имеет неверный формат:{0}{1}", Environment.NewLine, ex.Message));
            }
        }
        
        private static bool RefreshTeamsMVC(bool fullRefresh, SqlConnection cn, bool showMessages, XmlClient client, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = tran };
            cmd.CommandText = "SELECT iid, name FROM Teams(NOLOCK)";
            if (!fullRefresh)
                cmd.CommandText += " WHERE changed=1";
            List<RegionApiModel> existingTeams = new List<RegionApiModel>();
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                    existingTeams.Add(new RegionApiModel { Iid = Convert.ToInt32(rdr["iid"]), Name = rdr["name"].ToString() });
            }
            if (existingTeams.Count < 1)
                return true;
            Dictionary<long, RegionApiModel> refreshedList = new Dictionary<long, RegionApiModel>();
            if (fullRefresh)
            {
                var refreshResult = client.PostRegionCollection(new API_RegionCollection { Data = existingTeams.ToArray() });
                if (refreshResult.Data.Length != existingTeams.Count)
                {
                    if (showMessages)
                        MessageBox.Show("Ошибка загрузки списка команд - неверный ответ сервера");
                    return false;
                }
                for (int i = 0; i < refreshResult.Data.Length; i++)
                    refreshedList.Add(existingTeams[i].Iid, refreshResult.Data[i]);
            }
            else
            {
                foreach (var gData in existingTeams)
                    refreshedList.Add(gData.Iid, client.PostRegion(gData));
            }
            cmd.CommandText = "UPDATE Teams SET iid=@iid, changed=0 WHERE iid=@iidOld";
            cmd.Parameters.Add("@iid", SqlDbType.Int);
            cmd.Parameters.Add("@iidOld", SqlDbType.Int);
            Dictionary<long, long> changeIid = new Dictionary<long, long>();
            foreach (var gRecord in refreshedList)
            {
                if (gRecord.Key != gRecord.Value.Iid)
                {
                    long nextId = changeTableIid(gRecord.Key, gRecord.Value.Iid, "Teams", cn, tran);
                    if (nextId == 0)
                        nextId = gRecord.Value.Iid;
                    else
                        changeIid.Add(nextId, gRecord.Value.Iid);
                    cmd.Parameters["@iidOld"].Value = gRecord.Key;
                    cmd.Parameters["@iid"].Value = nextId;
                    cmd.ExecuteNonQuery();
                }
            }
            if (changeIid.Count > 0)
                ChangeIidsBack(changeIid, "Teams", cn, tran);
            return true;
        }

        private static bool RefreshGroupsMVC(bool fullRefresh, SqlConnection cn, bool showMessages, XmlClient client, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = tran };
            cmd.CommandText = "SELECT iid, name, oldYear, youngYear, genderFemale FROM Groups(NOLOCK)";
            if (!fullRefresh)
                cmd.CommandText += " WHERE changed=1";
            List<Comp_AgeGroupApiModel> grpList = new List<Comp_AgeGroupApiModel>();
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    grpList.Add(new Comp_AgeGroupApiModel
                    {
                        Female = Convert.ToBoolean(rdr["genderFemale"]),
                        Iid = Convert.ToInt32(rdr["iid"]),
                        Name = rdr["name"].ToString(),
                        YearOld = Convert.ToInt32(rdr["oldYear"]),
                        YearYoung = Convert.ToInt32(rdr["youngYear"])
                    });
                }
            }
            if (grpList.Count < 1)
                return true;
            Dictionary<int, Comp_AgeGroupApiModel> refreshedList = new Dictionary<int, Comp_AgeGroupApiModel>();
            if (fullRefresh)
            {
                var refreshResult = client.PostGroupCollection(new API_AgeGroupCollection { Data = grpList.ToArray() });
                if (refreshResult.Data.Length != grpList.Count)
                {
                    if (showMessages)
                        MessageBox.Show("Ошибка загрузки списка групп - неверный ответ сервера");
                    return false;
                }
                for (int i = 0; i < refreshResult.Data.Length; i++)
                    refreshedList.Add(grpList[i].Iid, refreshResult.Data[i]);
            }
            else
            {
                foreach (var gData in grpList)
                    refreshedList.Add(gData.Iid, client.PostGroup(gData));
            }
            cmd.CommandText = "UPDATE Groups" +
                              "   SET iid=@iid, name=@name, oldYear=@oldYear, youngYear=@youngYear, genderFemale=@gender, changed=0" +
                              " WHERE iid=@iidOld";
            cmd.Parameters.Add("@iid", SqlDbType.Int);
            cmd.Parameters.Add("@name", SqlDbType.VarChar, 50);
            cmd.Parameters.Add("@oldYear", SqlDbType.Int);
            cmd.Parameters.Add("@youngYear", SqlDbType.Int);
            cmd.Parameters.Add("@gender", SqlDbType.Bit);
            cmd.Parameters.Add("@iidOld", SqlDbType.Int);
            Dictionary<long, long> changedId = new Dictionary<long, long>();
            foreach (var gRecord in refreshedList)
            {
                long nextId;
                if (gRecord.Key != gRecord.Value.Iid)
                {
                    nextId = changeTableIid(gRecord.Key, gRecord.Value.Iid, "Groups", cn, tran);
                    if (nextId == 0)
                        nextId = gRecord.Value.Iid;
                    else
                        changedId.Add(nextId, gRecord.Value.Iid);
                }
                else
                    nextId = gRecord.Key;
                cmd.Parameters["@iidOld"].Value = gRecord.Key;
                cmd.Parameters["@iid"].Value = nextId;
                cmd.Parameters["@name"].Value = gRecord.Value.Name;
                cmd.Parameters["@oldYear"].Value = gRecord.Value.YearOld;
                cmd.Parameters["@youngYear"].Value = gRecord.Value.YearYoung;
                cmd.Parameters["@gender"].Value = gRecord.Value.Female;
                cmd.ExecuteNonQuery();
            }
            if (changedId.Count > 0)
                ChangeIidsBack(changedId, "Groups", cn, tran);
            return true;
        }

        private static void ChangeIidsBack(Dictionary<long, long> iidData, String table, SqlConnection cn, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = tran };
            cmd.CommandText = String.Format("UPDATE {0} SET iid=@iidNew WHERE iid=@iidOld", table);
            cmd.Parameters.Add("@iidNew", SqlDbType.BigInt);
            cmd.Parameters.Add("@iidOld", SqlDbType.BigInt);
            foreach (var kvp in iidData)
            {
                cmd.Parameters["@iidOld"].Value = kvp.Key;
                cmd.Parameters["@iidNew"].Value = kvp.Value;
                cmd.ExecuteNonQuery();
            }
        }

        private enum RefreshResult { Empty, Data, Error }

        private static long[] GetTeamsForClimber(int climberId, SqlConnection cn, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = tran };
            cmd.CommandText = "SELECT team_id FROM Participants(nolock) WHERE iid=@iid";
            cmd.Parameters.Add("@iid", SqlDbType.Int).Value = climberId;
            object oRes = cmd.ExecuteScalar();
            if (oRes == null || oRes == DBNull.Value)
                return new long[0];
            List<long> res = new List<long>();
            res.Add(Convert.ToInt64(oRes));
            cmd.CommandText = "SELECT team_id FROM teamsLink L(nolock) WHERE climber_id=@iid";
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    long l = Convert.ToInt64(rdr[0]);
                    if (!res.Contains(l))
                        res.Add(l);
                }
            }
            return res.ToArray();
        }

        private static RefreshResult RefreshClimbersMVC(bool fullRefresh, SqlConnection cn, bool showMessages, XmlClient client, SqlTransaction tran)
        {
            climbersToUpdateTableAdapter cta = new climbersToUpdateTableAdapter();
            cta.Connection = cn;
            cta.Transaction = tran;
            dsClimbing.climbersToUpdateDataTable table;
            if (fullRefresh)
                table = cta.GetData();
            else
                table = cta.GetDataByChanged(true);
            if (table.Rows.Count < 1)
                return RefreshResult.Empty;
            List<Comp_CompetitorRegistrationApiModel> existingClimbers = new List<Comp_CompetitorRegistrationApiModel>();
            foreach (dsClimbing.climbersToUpdateRow row in table.Rows)
                existingClimbers.Add(new Comp_MultipleTeamsClimber
                {
                    Bib = row.iid,
                    Boulder = (ApplicationType)row.boulder,
                    Female = row.genderFemale,
                    GroupID = row.group_id,
                    Lead = (ApplicationType)row.lead,
                    License = row.IslicenseNull() ? -1 : row.license,
                    Name = row.name,
                    RankingBoulder = row.IsrankingBoulderNull() ? null : new int?(row.rankingBoulder),
                    RankingLead = row.IsrankingLeadNull() ? null : new int?(row.rankingLead),
                    RankingSpeed = row.IsrankingSpeedNull() ? null : new int?(row.rankingSpeed),
                    Razr = (row.qf == null) ? String.Empty : row.qf,
                    Speed = (ApplicationType)row.speed,
                    Surname = row.surname,
                    TeamID = row.team_id,
                    YearOfBirth = row.age,
                    Teams = GetTeamsForClimber(row.iid, cn, tran)
                });
            Dictionary<int, Comp_CompetitorRegistrationApiModel> result = new Dictionary<int, Comp_CompetitorRegistrationApiModel>();
            if (fullRefresh)
            {
                var refreshResult = client.PostClimberCollection(new API_ClimbersCollection { Data = existingClimbers.ToArray() });
                if (refreshResult.Data.Length != existingClimbers.Count)
                {
                    if (showMessages)
                        MessageBox.Show("Ошибка загрузки списка участников - неверный ответ сервера");
                    return RefreshResult.Error;
                }
                for (int i = 0; i < refreshResult.Data.Length; i++)
                    result.Add(existingClimbers[i].Bib.Value, refreshResult.Data[i]);
            }
            else
            {
                foreach (var cData in existingClimbers)
                    result.Add(cData.Bib.Value, client.PostClimber(cData));
            }
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = tran };
            cmd.CommandText = "UPDATE Participants SET changed=0, license=@lic where iid=@iidOld";
            cmd.Parameters.Add("@lic", SqlDbType.BigInt);
            cmd.Parameters.Add("@iidOld", SqlDbType.Int);
            foreach (var clm in result)
            {
                cmd.Parameters["@lic"].Value = clm.Value.License;
                cmd.Parameters["@iidOld"].Value = clm.Key;
                cmd.ExecuteNonQuery();
            }
            return RefreshResult.Data;
        }

        private static void RefreshClimberList(bool fullRefresh, bool loadPhoto,
            long compIDforService, SqlConnection local, bool showMessages)
        {
            try
            {
                if (local.State != ConnectionState.Open)
                    local.Open();
                climbersToUpdateTableAdapter cta = new climbersToUpdateTableAdapter();
                cta.Connection = local;
                dsClimbing.climbersToUpdateDataTable table;
                if (fullRefresh)
                    table = cta.GetData();
                else
                    table = cta.GetDataByChanged(true);
                if (table.Rows.Count < 1)
                    return;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = local;
                cmd.CommandText = "UPDATE Participants SET changed=0 WHERE iid=@iid";
                cmd.Parameters.Add("@iid", SqlDbType.Int);
                ClimbingService service = new ClimbingService();
                List<int> updatedClimbers = new List<int>(), updatedTeams = new List<int>(), updatedGroups = new List<int>();
                foreach (dsClimbing.climbersToUpdateRow row in table.Rows)
                {
                    cmd.Parameters[0].Value = row.iid;
                    ONLClimberLinkSerializable serLink = new ONLClimberLinkSerializable();
                    ONLGroupSerializable grp = new ONLGroupSerializable();
                    grp.GenderFemale = row.group_genderFemale;
                    grp.Name = row.group_name;
                    grp.yearOld = row.group_oldYear;
                    grp.yearYoung = row.group_youngYear;
                    grp.CompID = compIDforService;
                    grp.minQf = row.group_minQf;
                    if (!updatedGroups.Contains(row.group_id))
                        updatedGroups.Add(row.group_id);

                    ONLTeamSerializable team = new ONLTeamSerializable();
                    team.CompID = compIDforService;
                    team.GroupSet = false;
                    team.Name = row.team_name;
                    team.RankingPos = row.Isteam_rankingNull() ? int.MaxValue : row.team_ranking;
                    team.SecretaryId = row.team_id;
                    if (!updatedTeams.Contains(row.team_id))
                        updatedTeams.Add(row.team_id);

                    ONLclimber clm = new ONLclimber();
                    clm.age = row.age;
                    clm.birthdate = new DateTime(row.age, 1, 5);
                    clm.genderFemale = row.genderFemale;
                    clm.name = row.name;
                    clm.surname = row.surname;
                    if (loadPhoto)
                        clm.photo = (row.IsphotoNull() ? null : row.photo);
                    else
                        clm.photo = null;

                    ONLClimberCompLink cd = new ONLClimberCompLink();
                    cd.boulder = row.boulder;
                    cd.comp_id = compIDforService;
                    cd.lead = row.lead;
                    cd.nopoints = row.noPoints;
                    cd.qf = row.qf;
                    cd.rankingBoulder = row.IsrankingBoulderNull() ? null : (int?)row.rankingBoulder;
                    cd.rankingLead = row.IsrankingLeadNull() ? null : (int?)row.rankingLead;
                    cd.rankingSpeed = row.IsrankingSpeedNull() ? null : (int?)row.rankingSpeed;
                    cd.secretary_id = row.iid;
                    cd.speed = row.speed;
                    cd.vk = row.vk;

                    ONLClimberLinkSerializable lnk = new ONLClimberLinkSerializable();
                    lnk.Climber = clm;
                    lnk.ContainsPhoto = loadPhoto;
                    lnk.Group = grp;
                    lnk.Team = team;
                    lnk.Link = cd;

                    if (service.PostClimber(lnk) < 1)
                        throw new Exception("Ошибка Web службы");
                    cmd.ExecuteNonQuery();
                    updatedClimbers.Add(row.iid);
                }
                if (fullRefresh)
                {
                    service.DeleteClimbersNotInIid(updatedClimbers.ToArray(), compIDforService);
                    service.DeleteGroupsNotInIid(updatedGroups.ToArray(), compIDforService);
                    service.DeleteTeamsNotInSecretaryId(updatedTeams.ToArray(), compIDforService);
                }
                if (showMessages)
                    MessageBox.Show("Список участников успешо загружен");
            }
            catch (Exception ex)
            {
                if (showMessages)
                    MessageBox.Show("Ошибка загрузки списка участников:\r\n" + ex.Message);
            }
        }

        public static void RefreshClimberList(bool showMessages, bool fullRefresh, bool loadPhoto,
            long? compIDforservice, SqlConnection localConnection, SqlConnection remoteConnection, XmlClient client)
        {
            if (client != null)
            {
                RefreshClimberListMVC(fullRefresh, loadPhoto, localConnection, showMessages, client);
                return;
            }
            if (compIDforservice != null && compIDforservice.HasValue)
            {
                RefreshClimberList(fullRefresh, loadPhoto, compIDforservice.Value, localConnection, showMessages);
                return;
            }
            try
            {
                if (localConnection.State != ConnectionState.Open)
                    localConnection.Open();
                SqlDataAdapter da = new SqlDataAdapter(new SqlCommand());
                da.SelectCommand.Connection = localConnection;
                da.SelectCommand.CommandText = "SELECT iid, name, surname, team_id, group_id, age, qf, lead, speed, boulder, " +
                                               "       rankingLead, rankingSpeed, rankingBoulder, vk, nopoints, genderFemale " +
                                               "  FROM Participants(NOLOCK) ";
                if (!fullRefresh)
                    da.SelectCommand.CommandText += " WHERE changed = 1";
                DataTable dtl = new DataTable();
                da.Fill(dtl);
                if (dtl.Rows.Count < 1)
                    return;
                if (remoteConnection.State != ConnectionState.Open)
                    remoteConnection.Open();

                List<int> inserted = new List<int>();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = remoteConnection;
                SqlCommand cmdLocal = new SqlCommand();
                cmdLocal.Connection = localConnection;
                cmdLocal.CommandText = "UPDATE Participants SET changed=0 WHERE iid=@iid";
                cmdLocal.Parameters.Add("@iid", SqlDbType.Int);
                
                da.SelectCommand.CommandText = "SELECT iid, name, oldYear, youngYear, genderFemale, minQf FROM groups(NOLOCK)";
                DataTable dtTmp = new DataTable();
                da.Fill(dtTmp);

                cmd.CommandText = "EXEC InsertTeamGroup @iid, @name, 0, @yO, @yY, @gF, @minQf";
                cmd.Parameters.Clear();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add("@iid", SqlDbType.Int, 4, "iid");
                cmd.Parameters.Add("@name", SqlDbType.VarChar, 255, "name");
                cmd.Parameters.Add("@yO", SqlDbType.Int, 4, "oldYear");
                cmd.Parameters.Add("@yY", SqlDbType.Int, 4, "youngYear");
                cmd.Parameters.Add("@gF", SqlDbType.Bit, 1, "genderFemale");
                cmd.Parameters.Add("@minQf", SqlDbType.Int, 4, "minQf");
                
                foreach (DataRow dr in dtTmp.Rows)
                {
                    cmd.Parameters[0].Value = Convert.ToInt32(dr["iid"]);
                    cmd.Parameters[1].Value = dr["name"].ToString();
                    cmd.Parameters[2].Value = Convert.ToInt32(dr["oldYear"]);
                    cmd.Parameters[3].Value = Convert.ToInt32(dr["youngYear"]);
                    cmd.Parameters[4].Value = Convert.ToBoolean(dr["genderFemale"]);
                    cmd.Parameters[5].Value = Convert.ToInt32(dr["minQf"]);
                    cmd.ExecuteNonQuery();
                }

                da.SelectCommand.CommandText = "SELECT iid, name, pos FROM teams(NOLOCK)";
                cmd.CommandText = "EXEC InsertTeamGroup @iid, @name, 1, @pos";
                dtTmp.Rows.Clear();
                da.Fill(dtTmp);
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@iid", SqlDbType.Int, 4, "iid");
                cmd.Parameters.Add("@name", SqlDbType.VarChar, 255, "name");
                cmd.Parameters.Add("@pos", SqlDbType.Int,4,"pos");
                foreach (DataRow dr in dtTmp.Rows)
                {
                    cmd.Parameters[0].Value = dr["iid"];
                    cmd.Parameters[1].Value = dr["name"];
                    cmd.Parameters[2].Value = dr["pos"];
                    cmd.ExecuteNonQuery();
                }
                cmd.Parameters.RemoveAt(cmd.Parameters.Count - 1);


                cmd.CommandText =
                    "EXEC InsertClimber @iid, @s, @name, @age, @qf, @t, @g, @pl, @ps, @pb, @rl, @rs, @rb, @vk, @np, @gF";
                //"INSERT INTO ONLclimbers(iid, name, surname, team_id, group_id, age, qf, " +
                //"                    rankingLead, rankingSpeed, rankingBoulder) VALUES " +
                //"(@iid, @name, @s, @t, @g, @age, @qf, @rl, @rs, @rb)";
                cmd.Parameters.Add("@s", SqlDbType.VarChar, 255, "surname");
                cmd.Parameters.Add("@t", SqlDbType.Int, 8, "team_id");
                cmd.Parameters.Add("@g", SqlDbType.Int, 8, "group_id");
                cmd.Parameters.Add("@age", SqlDbType.Int, 8, "age");
                cmd.Parameters.Add("@qf", SqlDbType.VarChar, 50, "qf");

                cmd.Parameters.Add("@pl", SqlDbType.Bit, 1, "lead");
                cmd.Parameters.Add("@ps", SqlDbType.Bit, 1, "speed");
                cmd.Parameters.Add("@pb", SqlDbType.Bit, 1, "boulder");

                cmd.Parameters.Add("@rl", SqlDbType.Int, 8, "rankingLead");
                cmd.Parameters.Add("@rs", SqlDbType.Int, 8, "rankingSpeed");
                cmd.Parameters.Add("@rb", SqlDbType.Int, 8, "rankingBoulder");
                cmd.Parameters.Add("@vk", SqlDbType.Bit, 1, "vk");
                cmd.Parameters.Add("@np", SqlDbType.Bit, 1, "nopoints");
                cmd.Parameters.Add("@gF", SqlDbType.Bit, 1, "genderFemale");


                foreach (DataRow dr in dtl.Rows)
                {
                    cmd.Parameters[0].Value = dr["iid"];
                    cmd.Parameters[1].Value = dr["name"];
                    cmd.Parameters[2].Value = dr["surname"];
                    cmd.Parameters[3].Value = dr["team_id"];
                    cmd.Parameters[4].Value = dr["group_id"];
                    cmd.Parameters[5].Value = dr["age"];
                    cmd.Parameters[6].Value = dr["qf"];

                    cmd.Parameters[7].Value = dr["lead"];
                    cmd.Parameters[8].Value = dr["speed"];
                    cmd.Parameters[9].Value = dr["boulder"];

                    cmd.Parameters[10].Value = dr["rankingLead"];
                    cmd.Parameters[11].Value = dr["rankingSpeed"];
                    cmd.Parameters[12].Value = dr["rankingBoulder"];
                    cmd.Parameters[13].Value = dr["vk"];
                    cmd.Parameters[14].Value = dr["nopoints"];
                    cmd.Parameters[15].Value = dr["genderFemale"];
                    cmd.ExecuteNonQuery();
                    cmdLocal.Parameters[0].Value = Convert.ToInt32(dr["iid"]);
                    cmdLocal.ExecuteNonQuery();
                    inserted.Add(Convert.ToInt32(dr["iid"]));
                }
                if (inserted.Count > 0)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "UPDATE ONLdata SET lastUpd=@lu";
                    cmd.Parameters.Add("@lu", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = DateTime.Now.ToUniversalTime();
                    cmd.ExecuteNonQuery();
                }
                if (loadPhoto)
                {
                    ClimbingCompetition.dsClimbingTableAdapters.PhotoLocalTableAdapter lta = new ClimbingCompetition.dsClimbingTableAdapters.PhotoLocalTableAdapter();
                    cmd.Parameters.Clear();
                    cmd.CommandText = "UPDATE ONLclimbers SET photo = @photo, phLoaded = 0 WHERE iid=@iid";
                    cmd.Parameters.Add("@iid", SqlDbType.Int);
                    cmd.Parameters.Add("@photo", SqlDbType.Image);
                    lta.Connection = localConnection;
                    foreach (int i in inserted)
                        foreach (dsClimbing.PhotoLocalRow row in lta.GetDataByIid(i))
                        {
                            cmd.Parameters[0].Value = row.iid;
                            if (row.IsphotoNull())
                                cmd.Parameters[1].Value = DBNull.Value;
                            else
                                cmd.Parameters[1].Value = row.photo;
                            cmd.ExecuteNonQuery();
                            break;
                        }
                }
                if (fullRefresh && inserted.Count > 0)
                {
                    try
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = "DELETE FROM ONLclimbers WHERE iid NOT IN (";
                        foreach (int i in inserted)
                            cmd.CommandText += i.ToString() + ",";
                        cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 1) + ")";
                        cmd.ExecuteNonQuery();
                    }
                    catch { }
                }
                if (showMessages)
                    MessageBox.Show("Список участников успешно загружен.");
            }
            catch (Exception ex)
            {
                if (showMessages)
                    MessageBox.Show("Ошибка загрзки списка участников\r\n" + ex.Message);
            }
            finally { remoteConnection.Close(); }
        }

        //public static void RefreshJudgesList(bool showMessages, bool fullRefresh, bool loadPhoto, SqlConnection localConnection, SqlConnection remoteConnection)
        //{
        //    try
        //    {
        //        if (localConnection.State != ConnectionState.Open)
        //            localConnection.Open();
        //        SqlDataAdapter da = new SqlDataAdapter(new SqlCommand());
        //        da.SelectCommand.Connection = localConnection;
        //        /*da.SelectCommand.CommandText = "SELECT iid, name, surname, team_id, group_id, age, qf, " +
        //                                       "       rankingLead, rankingSpeed, rankingBoulder, vk, nopoints, genderFemale " +
        //                                       "  FROM Participants(NOLOCK) ";*/
        //        da.SelectCommand.CommandText = "SELECT iid,surname,name,patronimic,city,pos,category " +
        //                                     "  FROM Judges(NOLOCK) ";
        //        if (!fullRefresh)
        //            da.SelectCommand.CommandText += " WHERE changed = 1";
        //        DataTable dtl = new DataTable();
        //        da.Fill(dtl);
        //        if (dtl.Rows.Count < 1)
        //            return;
        //        if (remoteConnection.State != ConnectionState.Open)
        //            remoteConnection.Open();

        //        List<int> inserted = new List<int>();
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.Connection = remoteConnection;
        //        SqlCommand cmdLocal = new SqlCommand();
        //        cmdLocal.Connection = localConnection;
        //        cmdLocal.CommandText = "UPDATE Judges SET changed=0 WHERE iid=@iid";
        //        cmdLocal.Parameters.Add("@iid", SqlDbType.Int);

        //        cmd.CommandText =
        //            "EXEC InsertJudge @iid, @s, @n, @p, @c, @cat";
        //        //"INSERT INTO ONLclimbers(iid, name, surname, team_id, group_id, age, qf, " +
        //        //"                    rankingLead, rankingSpeed, rankingBoulder) VALUES " +
        //        //"(@iid, @name, @s, @t, @g, @age, @qf, @rl, @rs, @rb)";
        //        cmd.Parameters.Add("@iid", SqlDbType.Int, 4, "iid");
        //        cmd.Parameters.Add("@s", SqlDbType.VarChar, 255, "surname");
        //        cmd.Parameters.Add("@n", SqlDbType.VarChar, 255, "name");
        //        cmd.Parameters.Add("@p", SqlDbType.VarChar, 255, "patronimic");
        //        cmd.Parameters.Add("@c", SqlDbType.VarChar, 255, "city");
        //        cmd.Parameters.Add("@cat", SqlDbType.VarChar, 50, "category");



        //        foreach (DataRow dr in dtl.Rows)
        //        {
        //            cmd.Parameters[0].Value = dr["iid"];
        //            cmd.Parameters[1].Value = dr["surname"];
        //            cmd.Parameters[2].Value = dr["name"];
        //            cmd.Parameters[3].Value = dr["patronimic"];
        //            cmd.Parameters[4].Value = dr["city"];
        //            cmd.Parameters[5].Value = dr["category"];
        //            cmd.ExecuteNonQuery();
        //            cmdLocal.Parameters[0].Value = Convert.ToInt32(dr["iid"]);
        //            cmdLocal.ExecuteNonQuery();
        //            inserted.Add(Convert.ToInt32(dr["iid"]));
        //        }
        //        if (loadPhoto)
        //        {
        //            dsClimbingTableAdapters.judgeDataTableAdapter jta = new ClimbingCompetition.dsClimbingTableAdapters.judgeDataTableAdapter();
        //            jta.Connection = localConnection;
        //            cmd.Parameters.Clear();
        //            cmd.CommandText = "UPDATE ONLjudges SET photo=@photo WHERE iid=@iid";
        //            cmd.Parameters.Add("@iid", SqlDbType.Int);
        //            cmd.Parameters.Add("@photo", SqlDbType.Image);
        //            foreach (int jid in inserted)
        //                foreach (dsClimbing.judgeDataRow row in jta.GetDataByIid(jid).Rows)
        //                {
        //                    cmd.Parameters[0].Value = jid;
        //                    if (row.IsphotoNull())
        //                        cmd.Parameters[1].Value = DBNull.Value;
        //                    else
        //                        cmd.Parameters[1].Value = row.photo;
        //                    cmd.ExecuteNonQuery();
        //                    break;
        //                }
        //        }
        //        if (fullRefresh && inserted.Count > 0)
        //        {
        //            try
        //            {
        //                cmd.Parameters.Clear();
        //                cmd.CommandText = "DELETE FROM ONLjudges WHERE iid NOT IN (";
        //                foreach (int i in inserted)
        //                    cmd.CommandText += i.ToString() + ",";
        //                cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 1) + ")";
        //                cmd.ExecuteNonQuery();
        //            }
        //            catch { }
        //        }

        //        /*должности*/
        //        da.SelectCommand.CommandText = "SELECT iid,name FROM positions(NOLOCK)";
        //        if (!fullRefresh)
        //            da.SelectCommand.CommandText += "WHERE changed = 1";
        //        dtl.Rows.Clear();
        //        dtl.Clear();
        //        dtl.Columns.Clear();
        //        da.Fill(dtl);
        //        cmdLocal.CommandText = "UPDATE positions SET changed = 0 WHERE iid=@iid";
        //        cmd.Parameters.Clear();
        //        cmd.CommandText = "EXEC InsertPos @iid, @name";
        //        cmd.Parameters.Add("@iid", SqlDbType.Int, 4, "iid");
        //        cmd.Parameters.Add("@name", SqlDbType.VarChar, 255, "name");
        //        inserted.Clear();
        //        foreach (DataRow dr in dtl.Rows)
        //        {
        //            cmd.Parameters[0].Value = dr["iid"];
        //            cmd.Parameters[1].Value = dr["name"];
        //            cmd.ExecuteNonQuery();
        //            cmdLocal.Parameters[0].Value = dr["iid"];
        //            cmdLocal.ExecuteNonQuery();
        //            inserted.Add(Convert.ToInt32(dr["iid"]));
        //        }
        //        if (fullRefresh && inserted.Count > 0)
        //        {
        //            try
        //            {
        //                cmd.Parameters.Clear();
        //                cmd.CommandText = "DELETE FROM ONLpositions WHERE iid NOT IN (";
        //                foreach (int i in inserted)
        //                    cmd.CommandText += i.ToString() + ",";
        //                cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 1) + ")";
        //                cmd.ExecuteNonQuery();
        //            }
        //            catch { }
        //        }

        //        /*связки судья-должность*/
        //        da.SelectCommand.CommandText = "SELECT iid, judge_id, pos_id FROM JudgePos(NOLOCK)";
        //        if (!fullRefresh)
        //            da.SelectCommand.CommandText += " WHERE changed = 1";
        //        dtl.Rows.Clear();
        //        dtl.Columns.Clear();
        //        dtl.Clear();
        //        da.Fill(dtl);
        //        inserted.Clear();
        //        cmd.Parameters.Clear();
        //        cmd.CommandText = "EXEC InsertJudgePos @iid, @judge_id, @pos_id";
        //        cmd.Parameters.Add("@iid", SqlDbType.Int, 4, "iid");
        //        cmd.Parameters.Add("@judge_id", SqlDbType.Int, 4, "judge_id");
        //        cmd.Parameters.Add("@pos_id", SqlDbType.Int, 4, "pos_id");
        //        cmdLocal.CommandText = "UPDATE JudgePos SET changed = 0 WHERE iid=@iid";
        //        foreach (DataRow dr in dtl.Rows)
        //        {
        //            cmd.Parameters[0].Value = dr["iid"];
        //            cmd.Parameters[1].Value = dr["judge_id"];
        //            cmd.Parameters[2].Value = dr["pos_id"];
        //            cmd.ExecuteNonQuery();
        //            cmdLocal.Parameters[0].Value = dr["iid"];
        //            cmdLocal.ExecuteNonQuery();
        //            inserted.Add(Convert.ToInt32(dr["iid"]));
        //        }
        //        if (fullRefresh && inserted.Count > 0)
        //        {
        //            try
        //            {
        //                cmd.Parameters.Clear();
        //                cmd.CommandText = "DELETE FROM ONLJudgePos WHERE iid NOT IN (";
        //                foreach (int i in inserted)
        //                    cmd.CommandText += i.ToString() + ",";
        //                cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 1) + ")";
        //                cmd.ExecuteNonQuery();
        //            }
        //            catch { }
        //        }

        //        if (showMessages)
        //            MessageBox.Show("Список судей успешно загружен.");
        //    }
        //    catch (Exception ex)
        //    {
        //        if (showMessages)
        //            MessageBox.Show("Ошибка загрзки списка судей\r\n" + ex.Message);
        //    }
        //    finally { remoteConnection.Close(); }
        //}

        public static T[] GetArray<T>(IEnumerable<T> source)
        {
            if (source == null)
                return new T[0];
            var lst = new List<T>(source);
            return lst.ToArray();
        }

        private static bool ReloadListsFromTable(bool fullReload, List<int> iidList, SqlConnection cn, long compID, XmlClient client)
        {
            iidList.Sort();
            var iids = iidList.ToArray();

            if (cn.State != ConnectionState.Open)
                cn.Open();
            SpeedRules rules = SettingsForm.GetSpeedRules(cn);



            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;

            SortingClass.CheckColumn("lists", "IsLocked", "VARCHAR(255) NULL", cn);

            dsClimbingTableAdapters.listsTableAdapter lta = new listsTableAdapter();
            lta.Connection = cn;
            Dictionary<int, ListHeader> listDict = new Dictionary<int, ListHeader>();
            ClimbingService service;
            if (client == null)
            {
                service = new ClimbingService();
                service.PostCompetitionParam(Constants.PDB_COMP_RULES, ((int)rules).ToString(), compID);
                foreach (var v in service.GetLists(iids, compID))
                    if (!listDict.ContainsKey(v.SecretaryID))
                        listDict.Add(v.SecretaryID, v);
            }
            else
                service = null;

            List<ListHeader> lists = new List<ListHeader>();
            List<int> inserted = new List<int>();

            foreach (int iid in iids)
            {
                var listParams = lta.GetDataByIid(iid);
                if (listParams == null || listParams.Count < 1)
                    continue;
                dsClimbing.listsRow row = listParams[0];
                ListHeader lh;
                if (listDict.ContainsKey(iid))
                    lh = listDict[iid];
                else
                {
                    lh = new ListHeader();
                    lh.SecretaryID = iid;
                }
                lh.GroupID = (row.Isgroup_idNull() ? null : new int?(row.group_id));
                lh.ListType = row.listType;
                lh.Live = row.allowView;
                lh.PrevRound_SecretaryID = (row.Isprev_roundNull() ? null : new int?(row.prev_round));
                lh.Quote = (row.IsquoteNull() ? 0 : row.quote);
                lh.Round = row.round;
                lh.RouteNumber = row.IsrouteNumberNull() ? null : new int?(row.routeNumber);
                lh.SecretaryID_Parent = row.Isiid_parentNull() ? null : new int?(row.iid_parent);
                lh.StartTime = row.IsstartNull() ? String.Empty : row.start;
                lh.Style = row.style;


                inserted.Add(iid);
                lists.Add(lh);
            }
            IList<int> nInserted;

            if (service == null)
            {
                DateTime refreshTime = DateTime.UtcNow;
                nInserted = new List<int>();
                var apiCollection = new List<ApiListHeader>(lists.Count);
                foreach (var l in lists)
                {
                    apiCollection.Add(new ApiListHeader
                    {
                        GroupId = l.GroupID,
                        Iid = l.SecretaryID,
                        ListType = (ListTypeEnum)Enum.Parse(typeof(ListTypeEnum), l.ListType, true),
                        Live = l.Live,
                        ParentList = l.SecretaryID_Parent,
                        PreviousRound = l.PrevRound_SecretaryID,
                        Quota = l.Quote,
                        Round = l.Round,
                        RouteQuantity = l.RouteNumber,
                        StartTime = l.StartTime,
                        Style = l.Style,
                        LastRefresh = refreshTime
                    });
                    nInserted.Add(l.SecretaryID);
                }
                if (fullReload)
                    client.PostListHeaderCollection(new ApiListHeaderCollection(apiCollection));
                else
                    foreach (var a in apiCollection)
                        client.PostListHeader(a);
            }
            else
                nInserted = service.PostLists(lists.ToArray(), compID);


            string inStr = String.Empty;
            if (nInserted != null)
                foreach (var v in nInserted)
                {
                    if (!inStr.Equals(String.Empty))
                        inStr += ",";
                    inStr += v.ToString();
                }
            if (!inStr.Equals(String.Empty))
            {
                try
                {
                    if (fullReload && service != null)
                        service.DeleteListsNotInSecretaryID(GetArray(nInserted), compID);
                    cmd.CommandText = "UPDATE lists SET changed=0, online=1 WHERE iid IN(" + inStr + ")";
                    cmd.ExecuteNonQuery();
                    if (fullReload)
                    {
                        cmd.CommandText = "UPDATE lists SET online=0  WHERE iid NOT IN(" + inStr + ")";
                        cmd.ExecuteNonQuery();
                    }
                }
                catch { }
            }
            return (nInserted != null && nInserted.Count > 0);
        }
        public static bool ReloadListsFromTable(bool fullReload, DataTable dtF, SqlConnection localConnection, SqlConnection remoteConnection, long? compIdForService, XmlClient client)
        {
            try
            {
                if (dtF.Rows.Count < 1)
                    return false;
                if (compIdForService != null && compIdForService.HasValue)
                {
                    List<int> iids = new List<int>();
                    foreach (DataRow dr in dtF.Rows)
                        if (GetValueTypeValue<bool>(dr["Доступ на сайте"], false))
                        {
                            int n = GetValueTypeValue<int>(dr["№"], -1);
                            if (n > 0 && !iids.Contains(n))
                                iids.Add(n);
                        }
                    if (iids.Count < 1)
                        return false;
                    return ReloadListsFromTable(fullReload, iids, localConnection, compIdForService.Value, client);
                }
                if (remoteConnection.State != ConnectionState.Open)
                    remoteConnection.Open();
                if (localConnection.State != ConnectionState.Open)
                    localConnection.Open();

                int cnt = 0;
                SqlCommand cmd;
                List<int> insertedList = new List<int>();

                cmd = new SqlCommand();
                cmd.Connection = remoteConnection;

                //if (fullReload)
                //{
                //    cmd.CommandText = "DELETE FROM ONLlists";
                //    cmd.ExecuteNonQuery();
                //}


                cmd.CommandText = "EXEC InsertList @list_id, @style, @group_id, @round, @routeNumber, @live," +
                                                  "@quote, @prevRound, @iid_parent, @start";
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@list_id", SqlDbType.Int);
                cmd.Parameters.Add("@style", SqlDbType.VarChar, 50);
                cmd.Parameters.Add("@group_id", SqlDbType.Int);
                cmd.Parameters.Add("@round", SqlDbType.VarChar, 255);
                cmd.Parameters.Add("@routeNumber", SqlDbType.Int);
                cmd.Parameters.Add("@live", SqlDbType.Bit);
                cmd.Parameters.Add("@quote", SqlDbType.Int);
                cmd.Parameters.Add("@prevRound", SqlDbType.Int);
                cmd.Parameters.Add("@iid_parent", SqlDbType.Int);
                cmd.Parameters.Add("@start", SqlDbType.VarChar, 255);

                SqlCommand cmdLocal = new SqlCommand(), cmdL2 = new SqlCommand();
                cmdLocal.CommandText = "UPDATE lists SET changed = 0 WHERE iid = @iid";
                cmdL2.CommandText = "SELECT ISNULL(quote,0) quote, prev_round, iid_parent, ISNULL(start,'') start FROM lists(NOLOCK) WHERE iid=@iid";
                cmdL2.Connection = cmdLocal.Connection = localConnection;
                cmdLocal.Parameters.Add("@iid", SqlDbType.Int);
                cmdL2.Parameters.Add("@iid", SqlDbType.Int);


                foreach (DataRow dr in dtF.Rows)
                {
                    if (Convert.ToBoolean(dr["Доступ на сайте"]))
                    {
                        cnt++;
                        //cmd2.Parameters[0].Value = Convert.ToInt32(dr["№"]);
                        //cmd2.ExecuteNonQuery();
                        int id = Convert.ToInt32(dr["№"]);
                        cmdL2.Parameters[0].Value = id;
                        int q = 0;
                        object oPr = DBNull.Value, oIp = DBNull.Value;
                        SqlDataReader drL = cmdL2.ExecuteReader();
                        string start = String.Empty;
                        try
                        {
                            if (drL.Read())
                            {
                                if (drL["prev_round"] != DBNull.Value)
                                    try { oPr = Convert.ToInt32(drL["prev_round"]); }
                                    catch { }
                                if (drL["iid_parent"] != DBNull.Value)
                                    try { oIp = Convert.ToInt32(drL["iid_parent"]); }
                                    catch { }
                                start = drL["start"].ToString();
                                try { q = Convert.ToInt32(drL["quote"]); }
                                catch { }
                            }
                        }
                        finally { drL.Close(); }

                        cmd.Parameters[0].Value = id;
                        cmd.Parameters[1].Value = dr["Вид"].ToString();
                        cmd.Parameters[2].Value = Convert.ToInt32(dr["gr_id"]);
                        cmd.Parameters[3].Value = dr["Раунд"].ToString();
                        try
                        {
                            if (dr["Число трасс"] != DBNull.Value)
                                cmd.Parameters[4].Value = Convert.ToInt32(dr["Число трасс"]);
                            else
                                cmd.Parameters[4].Value = DBNull.Value;
                        }
                        catch { cmd.Parameters[4].Value = DBNull.Value; }
                        try { cmd.Parameters[5].Value = Convert.ToBoolean(dr["Прямая трансляция"]); }
                        catch { cmd.Parameters[5].Value = false; }
                        cmd.Parameters[6].Value = q;
                        cmd.Parameters[7].Value = oPr;
                        cmd.Parameters[8].Value = oIp;
                        cmd.Parameters[9].Value = start;
                        //cmd.CommandText = "INSERT INTO ONLlists (iid,style,group_id," +
                        //    "round,routeNumber, live) VALUES (" + dr["№"].ToString() + ",'" +
                        //    dr["Вид"].ToString() + "'," + dr["gr_id"].ToString() + ",'" +
                        //    dr["Раунд"].ToString() + "', @rn, @lv)";
                        //try { cmd.Parameters[0].Value = (int)dr["Число трасс"]; }
                        //catch { cmd.Parameters[0].Value = DBNull.Value; }
                        //try { cmd.Parameters[1].Value = Convert.ToBoolean(dr["Прямая трансляция"]); }
                        //catch { cmd.Parameters[1].Value = false; }
                        cmd.ExecuteNonQuery();

                        cmdLocal.Parameters[0].Value = id;
                        cmdLocal.ExecuteNonQuery();
                        insertedList.Add(id);
                    }
                }

                try
                {
                    if (fullReload)
                    {

                        cmd.Parameters.Clear();
                        cmd.CommandText = "DELETE FROM ONLlists";
                        if (insertedList.Count > 0)
                        {
                            cmd.CommandText += " WHERE iid NOT IN(";
                            foreach (int i in insertedList)
                                cmd.CommandText += i.ToString() + ",";
                            cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 1) + ")";
                        }
                        cmd.ExecuteNonQuery();
                    }

                }
                catch { }
                return cnt > 0;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return false; }
        }

        public static void ReloadChangedLists(SqlConnection localConnection, SqlConnection remoteConnection, long? compIdForService, XmlClient client)
        {
            if (localConnection.State != ConnectionState.Open)
                localConnection.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = localConnection;
            cmd.CommandText = "SELECT iid [№], style Вид, round Раунд, routeNumber [Число трасс]," +
                              "       online [Доступ на сайте], allowView [Прямая трансляция]," +
                              "       group_id gr_id " +
                              "  FROM lists(NOLOCK) " +
                              " WHERE online=1 AND changed=1";
            DataTable dtL = new DataTable();
            (new SqlDataAdapter(cmd)).Fill(dtL);
            cmd.CommandText = "SELECT iid FROM lists(NOLOCK) WHERE online = 0 AND changed = 1";
            List<int> ofl = new List<int>();
            SqlDataReader dr = cmd.ExecuteReader();
            try
            {
                while (dr.Read())
                {
                    if (dr["iid"] != DBNull.Value && dr["iid"] != null)
                        ofl.Add(Convert.ToInt32(dr["iid"]));
                }
            }
            finally { dr.Close(); }
            StaticClass.ReloadListsFromTable(false, dtL, localConnection, remoteConnection, compIdForService, client);
            if (ofl.Count > 0)
            {
                try
                {
                    string inL = String.Empty;
                    foreach (int i in ofl)
                    {
                        if (!inL.Equals(String.Empty))
                            inL += ",";
                        inL += i.ToString();
                    }
                    inL += ")";
                    if (compIdForService != null && compIdForService.HasValue)
                    {
                        if (client == null)
                        {
                            ClimbingService service = new ClimbingService();
                            service.DeleteListsWhereSecretaryIDIn(ofl.ToArray(), compIdForService.Value);
                        }
                    }
                    else
                    {
                        cmd.CommandText = "DELETE FROM ONLlists WHERE iid IN(";
                        
                        cmd.CommandText += inL;
                        if (remoteConnection.State != ConnectionState.Open)
                            remoteConnection.Open();
                        cmd.Connection = remoteConnection;
                        cmd.ExecuteNonQuery();
                    }
                    cmd.Connection = localConnection;
                    cmd.CommandText = "UPDATE lists SET changed = 0 WHERE iid IN(" + inL;
                    cmd.ExecuteNonQuery();
                }
                catch { }
            }
        }

        class CloseArg
        {
            public CloseArg(string connectionString, int listID)
            {
                this.connectionString = connectionString;
                this.listID = listID;
            }
            public string connectionString { get; private set; }
            public int listID { get; private set; }
        }

        private static void OnClosingThr(object objCn)
        {
            if (!(objCn is CloseArg))
                return;
            CloseArg ca = (CloseArg)objCn;
            if (ca.connectionString.Length < 1)
                return;
            SqlConnection cnl = new SqlConnection(ca.connectionString);

            try
            {
                SqlCommand cmd = new SqlCommand();
                cnl.Open();
                cmd.Connection = cnl;
                cmd.CommandText = "UPDATE lists SET nowClimbing=NULL, nowClimbingTmp = NULL, nowClimbing3 = NULL WHERE iid=" +
                    ca.listID.ToString();
                cmd.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try { cnl.Close(); }
                catch { }
            }
        }

        public static void SetNowClimbingNull(SqlConnection cn, int listID)
        {
            CloseArg ca = new CloseArg((cn == null ? "" : cn.ConnectionString), listID);
            System.Threading.Thread thr = new System.Threading.Thread(OnClosingThr);
            thr.Start(ca);
        }

        public static string GetClimberName(string climberID, SqlConnection cn)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                string cNme;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "SELECT surname + ' ' + name Nme FROM Participants(NOLOCK) " +
                    "WHERE iid = @id";
                cmd.Parameters.Add("@id", SqlDbType.Int);
                int nYY;
                if (!int.TryParse(climberID, out nYY))
                {
                    MessageBox.Show("Неверный номер участника");
                    return "";
                }
                cmd.Parameters[0].Value = nYY;
                cNme = cmd.ExecuteScalar().ToString() + " (№" + climberID + ")";
                return cNme;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при извлечении имени участника\r\n" + ex.Message);
                return "";
            }
        }

        /// <summary>
        /// Базовый класс для главной формы приложения
        /// </summary>
        public class ExtendedForm : System.Windows.Forms.Form, IRefreshableAndLocalizable
        {
            private SystemListener sl= null;
            public ExtendedForm() : base() { this.LoadLocalizedStrings(Thread.CurrentThread.CurrentUICulture); }
            
            private Thread loadingThr = null;
            public Thread LoadingThread
            {
                get { return loadingThr; }
                set { loadingThr = value; }
            }

            public SystemListener SystListener
            {
                get { return sl; }
                set { sl = value; }
            }

            protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
            {
                if(sl != null)
                    try { sl.StopListening(); }
                    catch { }
                base.OnClosing(e);
            }

            public virtual void LoadLocalizedStrings(CultureInfo ci) { }


            public virtual void RefreshAndReload() { }
        }

        public delegate void SimpleDelegate();

        public static void DoSimpleWork(SimpleDelegate func)
        {
            ThreadStart thrS = new ThreadStart(func);
            Thread thr = new Thread(thrS);
            thr.Start();
        }

        public static List<int> GetNumberSeq(string src)
        {
            string cur = "";
            List<int> res = new List<int>();
            for (int i = 0; i < src.Length; i++)
                if (char.IsDigit(src[i]))
                    cur += src[i];
                else if (cur.Length > 0)
                {
                    int nTmp;
                    if (int.TryParse(cur, out nTmp))
                        res.Add(nTmp);
                    cur = "";
                }
            if (cur.Length > 0)
            {
                int nTmp;
                if (int.TryParse(cur, out nTmp))
                    res.Add(nTmp);
                cur = "";
            }
            return res;
        }

        public static void CheckQualyTrigger(SqlConnection cn)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT COUNT(*) FROM sysobjects(NOLOCK) WHERE name = 'checkQualy' AND type='TR'";
            if (Convert.ToInt32(cmd.ExecuteScalar()) < 1)
            {
                cmd.CommandText = @"
  CREATE TRIGGER [checkQualy] ON [dbo].[Lists]
  AFTER INSERT,UPDATE
  AS
  DECLARE @rn INT,
          @round VARCHAR(255),
          @style VARCHAR(255)
  SELECT @rn=routeNumber, @round = round, @style = style FROM inserted
  IF (@style = 'Трудность' AND @round = 'Квалификация' OR 
     (@style = 'Боулдеринг' AND @round <> 'Итоговый протокол' AND @round <> 'Квалификация (2 группы)' AND @round <> 'Суперфинал' ) 
     ) AND (@rn IS NULL OR @rn < 1) BEGIN
    RAISERROR ('Количество трасс не введено', 16, 1)
    ROLLBACK TRANSACTION
  END";
                cmd.ExecuteNonQuery();
            }
        }

        public static void CheckSpeedFunctions(SqlConnection cn)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;

            cmd.CommandText = "SELECT COUNT(*) FROM sysobjects(NOLOCK) WHERE name = 'fn_get1st'";
#if RECREATE_FUNCTIONS
            if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
            {
                cmd.CommandText = "DROP FUNCTION dbo.fn_get1st";
                cmd.ExecuteNonQuery();
            }
#else
            if (Convert.ToInt32(cmd.ExecuteScalar()) < 1)
            {
#endif
                cmd.CommandText = @"
create function dbo.fn_get1st(
  @climber_id int,
  @list_id int
) returns varchar(max)
as begin
  declare @qf1 int
  set @qf1 = null
  select @qf1 = prev_round from lists(nolock) where iid=@list_id
  if @qf1 is null
    return('')
  declare @resToRet varchar(max)
  set @resToRet = null
  
  select @resToRet = resText
    from speedResults(nolock)
   where list_id = @qf1
     and climber_id = @climber_id

  set @resToRet = ISNULL(@resToRet,'')

  return (@resToRet)
end";
                cmd.ExecuteNonQuery();
#if !RECREATE_FUNCTIONS
            }
#endif

            

            cmd.CommandText = "SELECT COUNT(*) FROM sysobjects WHERE name = 'fn_getBestIid'";
#if RECREATE_FUNCTIONS
            if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
            {
                cmd.CommandText = "DROP FUNCTION dbo.fn_getBestIid";
                cmd.ExecuteNonQuery();
            }
#else
            if (Convert.ToInt32(cmd.ExecuteScalar()) < 1)
            {
#endif
                cmd.CommandText = @"
create function dbo.fn_getBestIid (@P_lResIid bigint) RETURNS bigint
AS BEGIN
  declare @qf1 int, @r1 bigint, @r2 bigint, @r1Iid bigint, @res bigint,
          @climber int, @list int

  select @r1 = " + long.MaxValue.ToString() + @",
         @r2 = " + long.MaxValue.ToString() + @",
         @r1Iid = null

  select @r2 = res, @climber = climber_id, @list = list_id
    from speedResults(NOLOCK)
   where iid = @P_lResIid

  set @qf1 = null
  select @qf1 = prev_round
    from lists(NOLOCK)
   where iid = @list

  if (@qf1 is null) begin
    return(@P_lResIid)
  end

  select @r1 = res, @r1Iid = iid
    from speedResults(NOLOCK)
   where list_id = @qf1
     and climber_id = @climber

  if (@r1Iid is null) begin
    return(@P_lResIid)
  end
  
  if(@r1 < @r2 and @r1 is not null) begin
    set @res = @r1Iid
  end
  else begin
    set @res = @P_lResIid
  end

  return(@res)
END";
                cmd.ExecuteNonQuery();
#if !RECREATE_FUNCTIONS
            }
#endif
            

            cmd.CommandText = "SELECT COUNT(*) FROM sysobjects WHERE name = 'fn_getBest'";
#if RECREATE_FUNCTIONS
            if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
            {
                cmd.CommandText = "DROP FUNCTION dbo.fn_getBest";
                cmd.ExecuteNonQuery();
            }
#else
            if (Convert.ToInt32(cmd.ExecuteScalar()) < 1)
            {
#endif
                cmd.CommandText = @"create function dbo.fn_getBest(@P_lResIid bigint) returns varchar(50)
as begin
  declare @resIid bigint, @resText varchar(max), @res varchar(50)

  set @resIid = dbo.fn_getBestIid(@P_lResIid)

  set @resText = null

  select @resText = resText
    from speedresults(NOLOCK)
   where iid = @resIid

  set @resText = isnull(@resText,'')

  set @res = left(@resText,50)

  return (@res)
end
";
                cmd.ExecuteNonQuery();
#if !RECREATE_FUNCTIONS
            }
#endif

            cmd.CommandText = "SELECT COUNT(*) FROM sysobjects WHERE name = 'fn_getBestRes'";
#if RECREATE_FUNCTIONS
            if(Convert.ToInt32(cmd.ExecuteScalar()) > 0)
            {
                cmd.CommandText = "DROP FUNCTION dbo.fn_getBestRes";
                cmd.ExecuteNonQuery();
            }
#else
            if (Convert.ToInt32(cmd.ExecuteScalar()) < 1)
            {
#endif
            cmd.CommandText = @"create function dbo.fn_getBestRes(@P_lResIid bigint) returns bigint
as begin
  declare @resIid bigint, @res bigint

  set @resIid = dbo.fn_getBestIid(@P_lResIid)

  set @res = null

  select @res = res
    from speedResults(NOLOCK)
   where iid = @resIid

  set @res = isnull(@res," + long.MaxValue.ToString() + @")

  return (@res)
end
";
                cmd.ExecuteNonQuery();
#if !RECREATE_FUNCTIONS
            }
#endif
        }


        public static long? ParseServString(string cString, out XmlClientWrapper wrapper)
        {
            wrapper = null;
            if (String.IsNullOrEmpty(cString))
                return null;
            wrapper = XmlClientWrapper.Deserialize(cString);
            if (wrapper != null)
                return null;
            string[] parsed = cString.Split(new char[] { '=', ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (parsed.Length < 2 || !parsed[0].Equals("SERV"))
                return null;
            long l;
            if (long.TryParse(parsed[1], out l))
                return new long?(l);
            else
                return null;
        }

        public static string GenerateObjectValue(object src, Type t = null)
        {
            if (src == null || src == DBNull.Value)
                return " NULL ";
            Type typeToCheck = (t == null) ? src.GetType() : t;
            if (typeToCheck == typeof(string))
                return " '" + src + "' ";
            else if (typeToCheck == typeof(DateTime))
            {
                DateTime dt = Convert.ToDateTime(src);
                return " CONVERT(DATETIME,'" +
                    dt.ToString("dd.MM.yyyy hh:mm:ss") + "',104) ";
            }
            else if (typeToCheck == typeof(double) || typeToCheck == typeof(Single) || typeToCheck == typeof(decimal))
                return " " + Convert.ToDouble(src).ToString(new CultureInfo("en-US")) + " ";
            else if (typeToCheck == typeof(bool))
                return " " + ((bool)src ? "1" : "0") + " ";
            else
                return " " + src.ToString() + " ";
        }

        public static long? GetActiveTransaction(long obj_id, string tableName, SqlConnection cn, SqlTransaction tran = null)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;

            SortingClass.CheckColumn("logic_transactions", "is_ghost", "BIT NOT NULL DEFAULT 0", cn, tran);

            cmd.CommandText = "SELECT LT.iid FROM logic_transactions LT(NOLOCK) " +
                              " WHERE LT.obj_table = @table" +
                              "   AND LT.obj_id = @obj_id" +
                              "   AND LT.is_active = 1" +
                              "   AND LT.is_ghost = 0" +
                           " ORDER BY LT.tran_date DESC";
            cmd.Parameters.Add("@table", SqlDbType.VarChar, 255);
            cmd.Parameters[0].Value = tableName;
            cmd.Parameters.Add("@obj_id", SqlDbType.BigInt);
            cmd.Parameters[1].Value = obj_id;
            object res = cmd.ExecuteScalar();
            if (res != null && res != DBNull.Value)
            {
                long reL = Convert.ToInt64(res);
                cmd.CommandText = "UPDATE logic_transactions " +
                                  "   SET is_active = 0 " +
                                  " WHERE obj_table = @table " +
                                  "   AND obj_id = @obj_id " +
                                  "   AND iid <> " + reL.ToString();
                cmd.ExecuteNonQuery();
                return new long?(reL);
            }
            else
                return null;
        }

        public static bool RollbackTransaction(long tran_id, SqlConnection cn, SqlTransaction tran = null)
        {
            return ExecTranProc("dbo.rollback_logic_transaction", tran_id, cn, tran);
        }

        public static bool RestoreLogicTransaction(long tran_id, SqlConnection cn, SqlTransaction tran = null)
        {
            return ExecTranProc("dbo.restore_logic_transaction", tran_id, cn, tran);
        }

        public struct ParamToInsert
        {
            public string TableName { get; private set; }
            public string ColumnName { get; private set; }
            public string Value { get; private set; }
            public string OldValue { get; set; }
            public ParamToInsert(string tableName, string columnName, string value, string old_value):this()
            {
                this.TableName = tableName;
                this.ColumnName = columnName;
                this.Value = value;
                this.OldValue = old_value;
            }
        }

        public static long AddLogicTransactionHeader(long obj_id, string tableName, SqlConnection cn, SqlTransaction tran = null, bool isActive = true)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            long nextIid = SortingClass.GetNextIID("logic_transactions", "iid", cn, tran);
            cmd.CommandText = "INSERT INTO logic_transactions(iid, tran_date, obj_table, obj_id, is_active)" +
                              " VALUES(@iid, @tran_date, @obj_table, @obj_id, @is_active)";
            cmd.Parameters.Add("@iid", SqlDbType.BigInt).Value = nextIid;
            cmd.Parameters.Add("@tran_date", SqlDbType.DateTime).Value = DateTime.UtcNow;
            cmd.Parameters.Add("@obj_table", SqlDbType.VarChar, 255).Value = tableName;
            cmd.Parameters.Add("@obj_id", SqlDbType.BigInt).Value = obj_id;
            cmd.Parameters.Add("@is_active", SqlDbType.Bit).Value = isActive;

            cmd.ExecuteNonQuery();

            cmd.CommandText = "DELETE FROM logic_transactions" +
                              " WHERE obj_id = @obj_id" +
                              "   AND obj_table = @obj_table" +
                              "   AND tran_date > @tran_date";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "UPDATE logic_transactions SET is_active = 0 WHERE obj_id = @obj_id AND obj_table = @obj_table AND iid <> @iid";
            cmd.ExecuteNonQuery();

            return nextIid;
        }

        private static bool ExecTranProc(string proc_name, long tran_id, SqlConnection cn, SqlTransaction tran = null)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            cmd.CommandText = "EXEC " + proc_name + " @P_nTranID = @tranID, @RP_bSuccess = @res OUTPUT";
            cmd.Parameters.Add("@tranID", SqlDbType.BigInt).Value = tran_id;
            cmd.Parameters.Add("@res", SqlDbType.Bit).Direction = ParameterDirection.Output;

            cmd.ExecuteNonQuery();

            object oRes = cmd.Parameters[1].Value;
            if (oRes == null || oRes == DBNull.Value)
                return false;
            try { return Convert.ToBoolean(oRes); }
            catch { return false; }
        }

        public static long? GetNextTransaction(long obj_id, string tableName, SqlConnection cn, SqlTransaction tran = null)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            long? actTran = GetActiveTransaction(obj_id, tableName, cn, tran);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            if (actTran == null || !actTran.HasValue)
            {
                cmd.CommandText = "  SELECT TOP 1 iid" +
                                  "    FROM logic_transactions(NOLOCK)" +
                                  "   WHERE obj_id = @obj_id" +
                                  "     AND obj_table = @obj_table " +
                                  "     AND is_ghost = 0 " +
                                  "ORDER BY tran_date";
                cmd.Parameters.Add("@obj_id", SqlDbType.BigInt).Value = obj_id;
                cmd.Parameters.Add("@obj_table", SqlDbType.VarChar, 255).Value = tableName;
                object rRes = cmd.ExecuteScalar();
                if (rRes == null || rRes == DBNull.Value)
                    return null;
                return new long?(Convert.ToInt64(rRes));
            }


            cmd.CommandText = "  SELECT TOP 1 NT.iid" +
                              "    FROM logic_transactions CT(NOLOCK)" +
                              "    JOIN logic_transactions NT(NOLOCK) ON NT.obj_id = CT.obj_id" +
                              "                                      AND NT.obj_table = CT.obj_table" +
                              "                                      AND NT.tran_date > CT.tran_date" +
                              "   WHERE CT.iid = " + actTran.Value.ToString() +
                              "     AND NT.is_ghost = 0 " +
                              "ORDER BY NT.tran_date";
            object res = cmd.ExecuteScalar();
            if (res == null || res == DBNull.Value)
                return null;
            return new long?(Convert.ToInt64(res));
        }

        public static void GetTableData(string tableName, string iidCol, long iid, SqlConnection cn,
            out Dictionary<string, string> values, SqlTransaction tran = null, bool excludeIDCol = true)
        {
            values = new Dictionary<string, string>();
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            cmd.CommandText = "SELECT " + iidCol + ", * " +
                              "  FROM " + tableName + " (NOLOCK)" +
                              " WHERE " + iidCol + " = " + iid.ToString();
            var dr = cmd.ExecuteReader();
            string tmp;
            try
            {
                if (dr.Read())
                {
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        tmp = dr.GetName(i);
                        if (values.ContainsKey(tmp))
                            continue;
                        if (excludeIDCol && tmp.Equals(iidCol, StringComparison.InvariantCultureIgnoreCase))
                            continue;
                        values.Add(dr.GetName(i), StaticClass.GenerateObjectValue(dr[i]));
                    }
                }
            }
            finally { dr.Close(); }
        }

        public static long AddTransaction(string tableName, string iidCol,
            long iid, Dictionary<string, string> oldValues, Dictionary<string, string> newValues,
            SqlConnection cn, long existingTran = -1, SqlTransaction tran = null, bool checkData = true)
        {
            bool tranSuccess = false;
            SqlTransaction tranToCheck = (tran == null) ? cn.BeginTransaction() : tran;
            long tranID = -1;
            try
            {
                if (existingTran <= 0)
                    tranID = StaticClass.AddLogicTransactionHeader(iid, tableName, cn, tranToCheck);
                else
                    tranID = existingTran;

                List<string> changedCols = new List<string>();
                if (oldValues == null)
                    oldValues = new Dictionary<string, string>();
                if (newValues == null)
                    newValues = new Dictionary<string, string>();
                foreach (var v in newValues)
                    if (!oldValues.ContainsKey(v.Key) || !oldValues[v.Key].Equals(newValues[v.Key]))
                        changedCols.Add(v.Key);
                if (changedCols.Count > 0)
                {
                    dsClimbingTableAdapters.logic_transaction_dataTableAdapter lta = new dsClimbingTableAdapters.logic_transaction_dataTableAdapter();
                    lta.Connection = cn;
                    lta.Transaction = tranToCheck;
                    int cnt = 0;
                    foreach (var s in changedCols)
                    {
                        string old = oldValues.ContainsKey(s) ? oldValues[s] : " NULL ";
                        long nextId = SortingClass.GetNextIID("logic_transaction_data", "iid_line", cn, tranToCheck);
                        lta.Insert(
                            tranID, nextId,
                            "U", tableName, iid, s, iidCol, old, newValues[s], s.Equals(iidCol, StringComparison.InvariantCultureIgnoreCase),
                            String.Empty,
                            String.Empty, (++cnt));
                    }
                }
                if (checkData)
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    cmd.Transaction = tranToCheck;
                    cmd.CommandText = "SELECT COUNT(*) FROM logic_transaction_data(NOLOCK) WHERE iid=" + tranID.ToString();
                    if (Convert.ToInt32(cmd.ExecuteScalar()) < 1)
                    {
                        cmd.CommandText = "DELETE FROM logic_transactions WHERE iid=" + tranID.ToString();
                        cmd.ExecuteNonQuery();
                        tranID = -1;
                    }
                }
                tranSuccess = true;
            }
            finally
            {
                if (tranSuccess && tran == null)
                    tranToCheck.Commit();
                else if (!tranSuccess)
                    try { tranToCheck.Rollback(); }
                    catch { }
            }

            return tranID;
        }

        public static void AddTableDataToDel(string tableName, string iidCol, long iid, long tranIid,SqlConnection cn, ref int orderInsert,SqlTransaction _tran = null)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            dsClimbingTableAdapters.logic_transaction_dataTableAdapter lta = new logic_transaction_dataTableAdapter();
            lta.Connection = cn;
            lta.Transaction = (_tran == null) ? cn.BeginTransaction() : _tran;
            try
            {
                Dictionary<string, string> values;
                GetTableData(tableName, iidCol, iid, cn, out values, lta.Transaction, false);

                foreach (var v in values)
                {
                    lta.Insert(
                    iid: tranIid,
                    iid_line: StaticClass.GetNextIID("logic_transaction_data", cn, "iid_line", lta.Transaction),
                    action: "D",
                    table_name: tableName,
                    table_id: iid,
                    mod_col: v.Key,
                    iid_col: iidCol,
                    old_value: v.Value,
                    new_value: " NULL ",
                    iid_modified: true,
                    default_conversion: String.Empty,
                    def_conv_new: String.Empty,
                    order_insert: (++orderInsert));
                }

                SortingClass.CheckColumn("logic_transactions", "is_ghost", "BIT NOT NULL DEFAULT 0", cn, lta.Transaction);

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Transaction = lta.Transaction;
                cmd.CommandText = "UPDATE logic_transactions SET is_active = 0, is_ghost = 1 " +
                                  " WHERE iid <> " + tranIid.ToString() +
                                  "   AND is_ghost = 0" +
                                  "   AND EXISTS(SELECT * " +
                                  "                FROM logic_transaction_data D(NOLOCK)" +
                                  "               WHERE D.iid = logic_transactions.iid" +
                                  "                 AND D.table_name = @tableName" +
                                  "                 AND D.table_id = @obj_id)";
                cmd.Parameters.Add("@tableName", SqlDbType.VarChar, 255).Value = tableName;
                cmd.Parameters.Add("@obj_id", SqlDbType.BigInt).Value = iid;
                int n = cmd.ExecuteNonQuery();

                if (_tran == null)
                    lta.Transaction.Commit();
            }
            catch (Exception ex)
            {
                try
                {
                    if (_tran == null)
                        lta.Transaction.Rollback();
                }
                catch { }
                throw ex;
            }
        }

        public static readonly XmlSerializer ClientSerializer = new XmlSerializer(typeof(XmlClientWrapper));
    }



    public class roundN
    {
        public int iid = 0;
        public string name = "";
        public bool TwoRoutes = false;
        public bool qf = false;
        public bool havePreQf = false;
    }


    [Serializable]
    public sealed class XmlClientWrapper
    {
        public String Uri { get; set; }
        public long CompetitionId { get; set; }
        public int CompYear { get; set; }
        public XmlClientWrapper() { }

        public XmlClient CreateClient(SqlConnection cn, int timeout = 600000, IWebProxy proxy = null)
        {
            return new XmlClient(Uri, CompetitionId, SecurityOperations.CreateSigningDelegate(cn), SecurityOperations.GetPassword(cn)) { Timeout = timeout, Proxy = proxy };
        }

        public static XmlClientWrapper Deserialize(String s)
        {
            try
            {
                using (XmlReader rdr = XmlReader.Create(new StringReader(s)))
                {
                    if (StaticClass.ClientSerializer.CanDeserialize(rdr))
                        return StaticClass.ClientSerializer.Deserialize(rdr) as XmlClientWrapper;
                    else
                        return null;
                }
            }
            catch (XmlException) { return null; }
        }
    }
}