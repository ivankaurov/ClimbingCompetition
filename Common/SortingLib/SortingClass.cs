// <copyright file="SortingClass.cs">
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

#if DEBUG
#undef DEBUG
#endif

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using ClimbingCompetition.Online;
using XmlApiData;

namespace ClimbingCompetition
{
    public enum AutoNum { GRP, ROW }

    [Flags]
    public enum SpeedRules : int
    {
        DefaultAll = 0x00,
        BestResultFromTwoQfRounds = 0x01,
        BestRouteInQfRound = 0x02,
        OneRouteToRunInFinal = 0x04,
        InternationalSchema = 0x08,
        InternationalRules = 0x10,
        IFSC_WR = (OneRouteToRunInFinal | BestRouteInQfRound),
        SpeedAdvancedSystem = (IFSC_WR | 0x20 | InternationalSchema)
    }

    /// <summary>
    /// Класс для сортировки результатов с учётом предыдущих раундов и всякой хуйни
    /// </summary>
    public static class SortingClass
    {
        public const int DSQ_POS = int.MaxValue - 5;
        public const int DNS_POS = int.MaxValue - 3;
        public static DataTable CreateStructure()
        {
            DataTable res = new DataTable();
            res.Columns.Add("vk", typeof(bool));
            res.Columns.Add("pos", typeof(int));
            res.Columns.Add("posText", typeof(string));
            res.Columns.Add("iid", typeof(int));
            res.Columns.Add("qf", typeof(string));
            res.Columns.Add("pts", typeof(double));
            res.Columns.Add("ptsText", typeof(string));
            res.Columns.Add("sp", typeof(int));
            return res;
        }

        public static void SortResults(DataTable dt, bool frstRound, bool isQuali)
        {
            if (dt == null || dt.Rows.Count < 1)
                return;
            SortRec(dt, 0, dt.Rows.Count, 8);
            int nVk = 0;
            SortTwoCases(dt, dt.Columns.IndexOf("pos"), dt.Columns.IndexOf("vk"));
            foreach (DataRow dr in dt.Rows)
            {
                bool b = true;
                if (Convert.ToBoolean(dr["vk"]))
                {
                    b = false;
                    nVk++;
                    dr["posText"] = dr["ptsText"] = "в/к";
                }

                switch (Convert.ToInt32(dr["sp"]))
                {
                    case 1:
                        if (frstRound)
                        {
                            if (!isQuali)
                                dr["ptsText"] = "дискв.";
                            if (b)
                                dr["posText"] = "дискв.";
                        }
                        else
                            goto default;
                        break;
                    case 2: if (frstRound)
                        {
                            if (!isQuali)
                                dr["ptsText"] = "н/я";
                            if (b)
                                dr["posText"] = "н/я";
                        }
                        else
                            goto default;
                        break;
                    default:
                        if (b)
                            dr["posText"] = (Convert.ToInt32(dr["pos"]) - nVk).ToString();
                        break;
                }
            }

            List<object[]> vkL = new List<object[]>();
            int iRn = 0;
            dt.Columns.Add("clPs", typeof(int));
            dt.Columns.Add("posDiff", typeof(int));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["clPs"] = i;
                dt.Rows[i]["posDiff"] = 0;
            }
            while (iRn < dt.Rows.Count)
            {
                if (Convert.ToBoolean(dt.Rows[iRn]["vk"]))
                {
                    for (int i = iRn + 1; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["pos"] = Convert.ToInt32(dt.Rows[i]["pos"]) - 1;
                        dt.Rows[i]["posDiff"] = (int)dt.Rows[i]["posDiff"] + 1;
                    }
                    vkL.Add(dt.Rows[iRn].ItemArray);
                    dt.Rows.RemoveAt(iRn);
                }
                else
                    iRn++;
            }

            SetPoints(dt, 0, dt.Rows.Count);





            foreach (object[] p in vkL)
                dt.Rows.Add(p);
            SortByColumn(dt, 0, dt.Rows.Count, dt.Columns.IndexOf("clPs"));
            foreach (DataRow drr in dt.Rows)
                drr["pos"] = Convert.ToInt32(drr["pos"]) + (int)drr["posDiff"];
            dt.Columns.Remove("clPs");
            dt.Columns.Remove("posDiff");

            int iSt = 0, vkCnt = 0, pSt = 0;
            double ptsSt = 0.0;
            bool bVk = false;
            int vkCntT = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int curPos = Convert.ToInt32(dt.Rows[i]["pos"]);
                if (Convert.ToBoolean(dt.Rows[i]["vk"]))
                {
                    vkCntT++;
                    if (curPos == pSt)
                        dt.Rows[i]["pts"] = ptsSt;
                    else
                    {
                        if (bVk)
                            vkCnt++;
                        else
                        {
                            bVk = true;
                            iSt = i;
                            vkCnt = 1;
                        }
                    }
                }
                else
                {
                    try { pSt = Convert.ToInt32(dt.Rows[i]["posText"]); }
                    catch { pSt = Convert.ToInt32(dt.Rows[i]["pos"]); }
                    ptsSt = Convert.ToDouble(dt.Rows[i]["pts"]);
                    if (bVk)
                    {
                        double stp = 1.0 / (double)(vkCnt + 1);
                        SetPoints(dt, iSt, i);
                        for (int j = iSt; j < i; j++)
                        {
                            double dCpts = Convert.ToDouble(dt.Rows[j]["pts"]);
                            dCpts *= stp;
                            dCpts += (double)(pSt - 1);
                            dt.Rows[j]["pts"] = dCpts;
                        }
                        bVk = false;
                    }
                }
            }
            if (bVk)
            {
                pSt = dt.Rows.Count + 1 - vkCntT;
                double stp = 1.0 / (double)(vkCnt + 1);
                SetPoints(dt, iSt, dt.Rows.Count);
                for (int j = iSt; j < dt.Rows.Count; j++)
                {
                    double dCpts = Convert.ToDouble(dt.Rows[j]["pts"]);
                    dCpts *= stp;
                    dCpts += (double)(pSt - 1);
                    dt.Rows[j]["pts"] = (double)(pSt - 1) + Convert.ToDouble(dt.Rows[j]["pts"]) * stp;
                }
            }

            foreach (DataRow dr in dt.Rows)
            {
                if ((Convert.ToInt32(dr["sp"]) == 0 || isQuali) && (!Convert.ToBoolean(dr["vk"])))
                    dr["ptsText"] = Convert.ToDouble(dr["pts"]).ToString("0.#");
                if (frstRound)
                    if (Convert.ToInt32(dr["sp"]) > 0)
                    {
                        dr["posText"] = "";
                        if (!isQuali)
                            dr["ptsText"] = "";
                    }
            }
        }

        private static void SetPoints(DataTable dt, int iSt, int iStp)
        {
            if (dt.Rows.Count < 1)
                return;
            double dSt = (double)iSt;
            int curPos = Convert.ToInt32(dt.Rows[iSt]["pos"]);
            if ((iStp - iSt) == 1)
                dt.Rows[iSt]["pts"] = dSt + 1.0;
            for (int i = iSt + 1; i < iStp; i++)
            {
                double pts;
                int pos = Convert.ToInt32(dt.Rows[i]["pos"]);
                if (pos > curPos)
                {
                    pts = ((double)(curPos + pos - 1)) / 2.0;
                    for (int j = curPos - 1; j < i; j++)
                        dt.Rows[j]["pts"] = pts;
                    curPos = pos;
                    if (i == iStp - 1)
                        dt.Rows[i]["pts"] = (double)curPos;
                }
                else if (i == iStp - 1)
                {
                    pts = ((double)(curPos + dt.Rows.Count)) / 2.0;
                    for (int j = curPos - 1; j <= i; j++)
                        dt.Rows[j]["pts"] = pts;
                }
            }
            for (int i = iSt; i < iStp; i++)
                dt.Rows[i]["pts"] = Convert.ToDouble(dt.Rows[i]["pts"]) - dSt;
        }

        public static void SortResults(DataTable dt, int quote, bool frstRound, bool isQuali, /*SpeedRules*/CompetitionRules cRules, bool ForceQf = false)
        {
            if (dt == null || dt.Rows.Count < 1)
                return;
            SortResults(dt, frstRound, isQuali);
            //bool rulesIntl = (cRules & SpeedRules.InternationalRules) == SpeedRules.InternationalRules;
            if (cRules == CompetitionRules.International)
            {
                if (dt.Columns.IndexOf("posV") < 0)
                    dt.Columns.Add("posV", typeof(double));
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    double d;
                    if (double.TryParse(dr["posText"].ToString(), out d))
                        dr["posV"] = d;
                    else if (i == 0)
                        dr["posV"] = 1.0;
                    else if (Convert.ToInt32(Convert.ToDouble(dr["pts"]) * 100) ==
                            Convert.ToInt32(Convert.ToDouble(dt.Rows[i - 1]["pts"]) * 100))
                        dr["posV"] = dt.Rows[i - 1]["posV"];
                    else
                        dr["posV"] = Convert.ToDouble(dt.Rows[i - 1]["posV"]) + 0.01;
                }
            }
            //int nVk = 0;
            //int curRow = 0;
            //int curPl = -1;
            double qD;
            if (quote > 0)
            {
                if(cRules == CompetitionRules.International)
                    qD = (double)quote + 0.001;
                else
                    qD = (double)quote + 0.65;
            }
            else
                qD = -100.0;

            foreach (DataRow dr in dt.Rows)
            {
                if (cRules == CompetitionRules.Russian)
                {
                    if (Convert.ToDouble(dr["pts"]) < qD && Convert.ToDouble(dr["pts"]) > 0.11)
                        dr["qf"] = "Q";
                }
                else
                {
                    if (Convert.ToDouble(dr["posV"]) < qD && Convert.ToDouble(dr["pts"]) > 0.11)
                        dr["qf"] = "Q";
                }
                try
                {
                    if (frstRound && (!isQuali))
                        switch (dr["sp"].ToString())
                        {
                            case "1":
                                dr["pts"] = dr["pos"] = DSQ_POS;
                                if (!ForceQf)
                                    dr["qf"] = "";
                                break;
                            case "2":
                                dr["pts"] = dr["pos"] = DNS_POS;
                                if (!ForceQf)
                                    dr["qf"] = "";
                                break;
                        }
                }
                catch { }
            }
            int lastQ = -1;
            for (int i = 0; i < dt.Rows.Count; i++)
                if (dt.Rows[i]["qf"].ToString().ToLower().IndexOf("q") > -1)
                    lastQ = i;
                else
                    break;
            
            if (lastQ >= 0 && Convert.ToBoolean(dt.Rows[lastQ]["vk"]))
            {
                double lQpts = Convert.ToDouble(dt.Rows[lastQ]["pts"]);
                if (lQpts > (double)quote)
                {
                    int i = lastQ - 1;
                    if (i >= 0 && Convert.ToDouble(dt.Rows[i]["pts"]) < lQpts)
                        dt.Rows[lastQ]["qf"] = "";
                }
            }
        }

        static void SortRec(DataTable dt, int rowStart, int rowStop, int columnID)
        {
            SortByColumn(dt, rowStart, rowStop, 0);
            if (dt.Columns[columnID].ColumnName.IndexOf("rNum") > -1)
                SortTwoRoutes(dt, rowStart, rowStop, columnID);
            else
                if (dt.Columns[columnID].ColumnName.IndexOf("pos") > -1)
                    SortRound(dt, rowStart, rowStop, columnID);
        }

        static void SortTwoRoutes(DataTable dt, int rowStart, int rowStop, int columnID)
        {
#if !DEBUG
            try
            {
#endif
                SortByColumn(dt, rowStart, rowStop, columnID);
                SortByColumn(dt, rowStart, rowStop, columnID + 1);
#if !DEBUG
            }
            catch { }
#endif
            int curPl = rowStart + 1;

            bool b = true;
            int rNum;
            if (dt.Rows[rowStart][columnID] == null || dt.Rows[rowStart][columnID] == DBNull.Value)
                rNum = 0;
            else
                try { rNum = Convert.ToInt32(dt.Rows[rowStart][columnID]); }
                catch { rNum = 0; }
            for (int i = curPl; i < rowStop; i++)
            {
                int curNum;
                if (dt.Rows[i][columnID] == null || dt.Rows[i][columnID] == DBNull.Value)
                    curNum = 0;
                else
                    try { curNum = Convert.ToInt32(dt.Rows[i][columnID]); }
                    catch { curNum = 0; }
                if (curNum != rNum)
                {
                    b = false;
                    break;
                }
            }
            if (b)
                SortRound(dt, rowStart, rowStop, columnID + 1);
            else
            {
                for (int nR = rowStart; nR < rowStop; nR++)
                {
                    dt.Rows[nR][columnID] = 0;
                    dt.Rows[nR][columnID + 1] = 0;
                }
                SortTwoRoutes(dt, rowStart, rowStop, columnID);
            }
        }

        static void SortRound(DataTable dt, int rowStart, int rowStop, int columnID)
        {
            SortByColumn(dt, rowStart, rowStop, columnID);
#if !DEBUG
            if (columnID < 0 || columnID >= dt.Columns.Count)
                return;
#endif
            int curPl = rowStart + 1;
            dt.Rows[rowStart][1] = curPl;
            for (int i = curPl; i < rowStop; i++)
            {
                int now;
                if (dt.Rows[i][columnID] == DBNull.Value || dt.Rows[i][columnID] == null)
                    now = 0;
                else
                    try { now = Convert.ToInt32(dt.Rows[i][columnID]); }
                    catch { now = 0; }
                int cp;
                if (dt.Rows[curPl-1][columnID] == DBNull.Value || dt.Rows[curPl-1][columnID] == null)
                    cp = 0;
                else
                    try { cp = Convert.ToInt32(dt.Rows[curPl - 1][columnID]); }
                    catch { cp = 0; }
                //if (Convert.ToInt32(dt.Rows[i][columnID]) > Convert.ToInt32(dt.Rows[curPl - 1][columnID]))
                if(now > cp)
                {
                    if (i != curPl && columnID < dt.Columns.Count - 1)
                        SortRec(dt, curPl - 1, i, columnID + 1);
                    curPl = i + 1;
                    dt.Rows[i][1] = curPl;
                }
                else
                {
                    dt.Rows[i][1] = curPl;
                    if (i == rowStop - 1 && columnID < dt.Columns.Count - 1)
                        SortRec(dt, curPl - 1, rowStop, columnID + 1);
                }

            }
        }

        public static void SortByColumn(DataTable dt, string column)
        {
            SortByColumn(dt, 0, dt.Rows.Count, dt.Columns.IndexOf(column));
        }

        public static void SortByColumn(DataTable dt, int rowStart, int rowStop, int columnID)
        {
            if (dt.Rows.Count < 1)
                return;
            if (columnID < 0 || columnID >= dt.Columns.Count)
#if DEBUG
                throw new ArgumentOutOfRangeException("Неверный индекс для сортировки");
#else
                return;
#endif
            for (int i = rowStart; i < rowStop && i < dt.Rows.Count; i++)
            {
                double pMin;
                if (dt.Rows[i][columnID] != null && dt.Rows[i][columnID] != DBNull.Value)
                    try { pMin = Convert.ToDouble(dt.Rows[i][columnID]); }
                    catch { pMin = 0.0; }
                else
                    pMin = 0.0;
                int rStCur = i + 1;
                for (int j = rStCur; j < rowStop; j++)
                {
                    double pCur;
                    try
                    {
                        if (dt.Rows[j][columnID] == null || dt.Rows[j][columnID] == DBNull.Value)
                            pCur = -.0;
                        else
                            pCur = Convert.ToDouble(dt.Rows[j][columnID]);
                    }
                    catch { pCur = 0.0; }
                    if (pCur < pMin)
                    {
                        SwapRows(dt, i, j);
                        pMin = pCur;
                    }
                }
            }
        }

        public static void SortTwoCases(DataTable dt, string col1, string col2)
        {
            SortTwoCases(dt, dt.Columns.IndexOf(col1), dt.Columns.IndexOf(col2));
        }

        public static void SortTwoCases(DataTable dt, int col1, int col2)
        {
            if (dt.Rows.Count < 1)
                return;
            SortByColumn(dt, 0, dt.Rows.Count, col1);
            int i = 0;
            while (i < dt.Rows.Count)
            {
                double dI;
                if (dt.Rows[i][col1] == DBNull.Value)
                    dI = 0.0;
                else
                    try { dI = Convert.ToDouble(dt.Rows[i][col1]); }
                    catch { dI = 0.0; }
                for (int j = i + 1; j <= dt.Rows.Count; j++)
                {
                    double dJ;
                    if (j == dt.Rows.Count)
                        dJ = 0.0;
                    else
                    {
                        if (dt.Rows[j][col1] == DBNull.Value)
                            dJ = 0.0;
                        else
                            try { dJ = Convert.ToDouble(dt.Rows[j][col1]); }
                            catch { dJ = 0.0; }
                    }
                    if (j == dt.Rows.Count || dI != dJ)
                    {
                        SortByColumn(dt, i, j, col2);
                        i = j - 1;
                        break;
                    }
                }
                i++;
            }
        }

        static void SwapRows(DataTable dt, int i, int j)
        {
            object[] obj = (object[])dt.Rows[i].ItemArray.Clone();
            dt.Rows[i].ItemArray = (object[])dt.Rows[j].ItemArray.Clone();
            dt.Rows[j].ItemArray = (object[])obj.Clone();
        }

        private static EnumT GetEnumValueFromObject<EnumT, ValueT>(object value, EnumT defaultValue)
            where EnumT : struct
            where ValueT : struct
        {
            try
            {
                if (value == null)
                    return defaultValue;
                ValueT baseVal = (ValueT)Convert.ChangeType(value, typeof(ValueT));
                return (EnumT)Enum.ToObject(typeof(EnumT), baseVal);
            }
            catch { return defaultValue; }
        }

        private static SpeedRules GetCompRulesONL(SqlConnection cn, long compID)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                cmd.Connection = cn;
                cmd.CommandText = "SELECT ParamValue" +
                                  "  FROM ONLCompetitionParams(NOLOCK)" +
                                  " WHERE comp_id = @compID" +
                                  "   AND ParamName = @paramName";
                cmd.Parameters.Add("@compID", SqlDbType.BigInt).Value = compID;
                cmd.Parameters.Add("@paramName", SqlDbType.VarChar, 255).Value = Constants.PDB_COMP_RULES;

                return GetEnumValueFromObject<SpeedRules, int>(cmd.ExecuteScalar(), SpeedRules.DefaultAll);
            }
        }

        public static SpeedRules GetCompRules(SqlConnection cn, SqlTransaction tran = null)
        {
            SqlConnection cnToUse = (tran == null ? cn : tran.Connection);
            SortingClass.CheckColumn("CompetitionData", "SpeedRulesGeneral",
                String.Format("INT NOT NULL DEFAULT {0}", (int)SpeedRules.DefaultAll), cnToUse, tran);
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = cnToUse;
                cmd.Transaction = tran;
                cmd.CommandText = "SELECT SpeedRulesGeneral FROM CompetitionData(NOLOCK)";
                return GetEnumValueFromObject<SpeedRules, int>(cmd.ExecuteScalar(), SpeedRules.DefaultAll);
            }
        }

        public static bool SetCompRules(SpeedRules value, SqlConnection cn, SqlTransaction tran = null)
        {
            SqlConnection cnToUse = (tran == null) ? cn : tran.Connection;
            SortingClass.CheckColumn("CompetitionData", "SpeedRulesGeneral", "INT NOT NULL DEFAULT " +
                ((int)SpeedRules.DefaultAll).ToString(), cnToUse, tran);
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = cnToUse;
                cmd.Transaction = tran;
                cmd.CommandText = String.Format("UPDATE CompetitionData SET SpeedRulesGeneral = {0}", ((int)value));
                cmd.ExecuteNonQuery();
                return true;
            }
        }

        public static SpeedRules GetCompRules(SqlConnection cn, bool onlBase, long ONLcompID = -1)
        {
            if (onlBase)
                return GetCompRulesONL(cn, ONLcompID);
            else
                return GetCompRules(cn, null);
        }

        /// <summary>
        /// Проверяет наличие заданного столца в таблице, и если такого нет, то добавляет его.
        /// Используется для приведения структуры старых БД к новым
        /// </summary>
        /// <param name="table"></param>
        /// <param name="cName"></param>
        /// <param name="cType"></param>
        /// <param name="cn"></param>
        /// <returns>true если такая колонка уже есть, false если была добавлена при вызове метода</returns>
        public static bool CheckColumn(string table, string cName, string cType, SqlConnection cn, SqlTransaction tran)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            using (SqlCommand cmd = new SqlCommand(
                "SELECT ISNULL(COUNT(*), 0) cnt " +
                "  FROM syscolumns C(NOLOCK) " +
                "  JOIN sysobjects O(NOLOCK) ON O.id = C.id" +
                " WHERE O.name = @table " +
                "   AND O.type = 'U' " +
                "   AND C.name = @column ",cn))
            {
                cmd.Transaction = tran;
                cmd.Parameters.Add("@table", SqlDbType.VarChar, 255).Value = table;
                cmd.Parameters.Add("@column", SqlDbType.VarChar, 255).Value = cName;
                if (Convert.ToInt32(cmd.ExecuteScalar()) < 1)
                {
                    cmd.CommandText = String.Format("ALTER TABLE {0} ADD {1} {2}", table, cName, cType);
                    cmd.ExecuteNonQuery();
                    return false;
                }
                else
                    return true;
            }
        }

        public static void CheckGlobalIDColumns(SqlConnection cn)
        {
            SortingClass.CheckColumn("Participants", "global_climber_id", "NVARCHAR(50) NULL", cn);
            SortingClass.CheckColumn("Participants", "global_app_id", "NVARCHAR(50) NULL", cn);
            SortingClass.CheckColumn("Groups", "global_group_id", "NVARCHAR(50) NULL", cn);
            SortingClass.CheckColumn("Teams", "global_team_id", "NVARCHAR(50) NULL", cn);
        }

        public static bool CheckColumn(string table, string cName, string cType, SqlConnection cn)
        { return CheckColumn(table, cName, cType, cn, null); }

        public static bool TablesDiffer(String permanentTable, String temporaryTable, String idColumn, SqlConnection cn, SqlTransaction tran, String comparisonQuery = null)
        {
            if (String.IsNullOrEmpty(comparisonQuery))
                comparisonQuery = CreateTableComparisonQuery(permanentTable, temporaryTable, idColumn, cn, tran);
            if (comparisonQuery == null)
                throw new ArgumentException("Invalid column data");
            var cmd = new SqlCommand { Connection = cn, Transaction = tran, CommandText = comparisonQuery };
            try
            {
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    return (Convert.ToInt32(result) > 0);
                else
                    return false;
            }
            catch (SqlException ex)
            {
                throw new ArgumentException(ex.Message, ex);
            }
        }

        public static String CreateTableComparisonQuery(String permanentTable, String temporaryTable, String idColumn, SqlConnection cn, SqlTransaction tran)
        {
            CreateTableCompareProc(cn, tran);
            var cmd = new SqlCommand
            {
                Connection = cn,
                Transaction = tran,
                CommandType = System.Data.CommandType.StoredProcedure,
                CommandText = "dbo.createTableCompQuery"
            };
            cmd.Parameters.Add("@P_sTable", SqlDbType.VarChar, 100).Value = permanentTable;
            cmd.Parameters.Add("@P_sTableTmp", SqlDbType.VarChar, 100).Value = temporaryTable;
            cmd.Parameters.Add("@P_sIdColumn", SqlDbType.VarChar, 100).Value = idColumn;
            var returnValue = cmd.Parameters.Add("@RP_sQuery", SqlDbType.VarChar, -1);
            returnValue.Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();
            var result = (returnValue.Value as string);
            if (result != null)
                result = result.Trim();
            return String.IsNullOrEmpty(result) ? null : result;
        }

        public static void CreateTableCompareProc(SqlConnection cn, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = tran };
            cmd.CommandText = "SELECT COUNT(*) cnt" +
                              "  FROM sysobjects" +
                              " WHERE name = 'createTableCompQuery'" +
                              "   AND type = 'P'";
            if (Convert.ToInt32(cmd.ExecuteScalar()) < 1)
            {
                cmd.CommandText = @"
create proc dbo.createTableCompQuery(@P_sTable    varchar(100),
                                     @P_sTableTmp varchar(100),
                                     @P_sIdColumn varchar(100),
                                     @RP_sQuery   varchar(max) out)
as begin
set nocount on
set @RP_sQuery = ''

   select @RP_sQuery = @RP_sQuery +
          'OR(TMP.' + C.name + ' <> H.' + C.name + ' ' +
             'OR(TMP.' + C.name + ' IS NULL AND H.' + C.name + ' IS NOT NULL)' +
             'OR(TMP.' + C.name + ' IS NOT NULL AND H.' + C.name + ' IS NULL)' +
             ')'
     from sysobjects T(nolock)
     join syscolumns C(nolock) on C.id = T.id
left join systypes CT(nolock) on CT.xusertype = C.xusertype
    where T.name = @P_sTable
      and C.name <> @P_sIdColumn
      and (   CT.name is null
           or CT.name not in ('binary', 'text', 'image', 'varbinary', 'sql_variant'))
if(@RP_sQuery <> '')
  set @RP_sQuery = RIGHT(@RP_sQuery, LEN(@RP_sQuery) - 2)
else
  return(0)
  
set @RP_sQuery = 'SELECT 1' +
              '  FROM ' + @P_sTableTmp + ' TMP(NOLOCK)' +
              '  JOIN ' + @P_sTable + ' H(NOLOCK) ON H.' + @P_sIdColumn + ' = TMP.' + @P_sIdColumn +
              ' WHERE ' + @RP_sQuery
              
end";
                cmd.ExecuteNonQuery();
            }
        }

        public static int GetNextNumber(int groupID, SqlConnection cn, SqlTransaction tran, AutoNum aNum, bool isOnl)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = cn;
                cmd.Transaction = tran;
                string tableName = (isOnl ? "ONLclimbers" : "Participants");
                int num;
                switch (aNum)
                {
                    case AutoNum.GRP:
                        cmd.CommandText =
                            String.Format("SELECT ISNULL(MAX(iid),0) iid FROM {0}(NOLOCK) WHERE group_id = {1}", tableName, groupID);
                        object oTmp = cmd.ExecuteScalar();
                        if (oTmp != null && oTmp != DBNull.Value)
                            num = Convert.ToInt32(oTmp);
                        else
                            num = 0;
                        if (num == 0)
                            num = (groupID - 1) * 100 + 1;
                        else if (num % 100 == 99)
                            num = GetNextGrpNum(groupID, num, cn, tran, isOnl);
                        else
                            num++;
                        break;
                    case AutoNum.ROW:
                        num = (int)GetNextIID(tableName, "iid", cn, tran);
                        break;
                    default:
                        goto case AutoNum.ROW;
                }
                while (checkNum(num, tableName, cn, tran))
                    num = (int)GetNextIID(tableName, "iid", cn, tran);
                return num;
            }
        }

        public static bool checkNum(int num, string tableName, SqlConnection cn, SqlTransaction tran)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            using (SqlCommand cmd = new SqlCommand(
                String.Format("SELECT COUNT(*) FROM {0} (NOLOCK) WHERE iid = {1}", tableName, num), cn))
            {
                cmd.Transaction = tran;
                object oTmp = cmd.ExecuteScalar();
                if (oTmp == null || oTmp == DBNull.Value)
                    return false;
                return (Convert.ToInt32(oTmp) > 0);
            }
        }

        private static int GetNextGrpNum(int groupId, int prevNum, SqlConnection cn, SqlTransaction tran, bool isOnl)
        {
            string tableGr = (isOnl ? "ONLgroups" : "groups");
            string tablePart = (isOnl ? "ONLclimbers" : "Participants");
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = cn;
                cmd.Transaction = tran;
                cmd.CommandText = "SELECT MAX(iid) FROM " + tableGr + "(NOLOCK)";
                int maxGr = Convert.ToInt32(cmd.ExecuteScalar());
                int startSer = prevNum - (prevNum % 100);
                int nextSer;
                if ((startSer / 100) == (groupId - 1))
                    nextSer = maxGr * 100;
                else
                    nextSer = startSer + 100;

                for (int i = 0; i < 100; i++)
                {
                    if (!checkNum(nextSer + 1, tablePart, cn, tran))
                        return nextSer + 1;
                    nextSer += 100;
                }
                return (int)GetNextIID(tablePart, "iid", cn, tran);
            }
        }

        public static long GetNextIID(string tableName, string columnName, SqlConnection cn, SqlTransaction tran)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = cn;
                cmd.Transaction = tran;
                cmd.CommandText = String.Format("SELECT (ISNULL(MAX({0}),0) + 1) iid FROM {1}(NOLOCK)", columnName, tableName);
                object res = cmd.ExecuteScalar();
                if (res == null || res == DBNull.Value)
                    return 1;
                return Convert.ToInt64(res);
            }
        }

    }
}
