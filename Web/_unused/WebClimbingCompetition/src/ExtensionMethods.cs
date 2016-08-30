// <copyright file="ExtensionMethods.cs">
// Copyright © 2016 All Rights Reserved
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
// (Этот файл — часть ClimbingCompetition.
// 
// ClimbingCompetition - свободная программа: вы можете перераспространять ее и/или
// изменять ее на условиях Стандартной общественной лицензии GNU в том виде,
// в каком она была опубликована Фондом свободного программного обеспечения;
// либо версии 3 лицензии, либо (по вашему выбору) любой более поздней
// версии.
// 
// ClimbingCompetition распространяется в надежде, что она будет полезной,
// но БЕЗО ВСЯКИХ ГАРАНТИЙ; даже без неявной гарантии ТОВАРНОГО ВИДА
// или ПРИГОДНОСТИ ДЛЯ ОПРЕДЕЛЕННЫХ ЦЕЛЕЙ. Подробнее см. в Стандартной
// общественной лицензии GNU.
// 
// Вы должны были получить копию Стандартной общественной лицензии GNU
// вместе с этой программой. Если это не так, см. <http://www.gnu.org/licenses/>.)
// </copyright>
// <author>Ivan Kaurov</author>
// <date>30.08.2016 23:51</date>

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Security.Principal;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Web.SessionState;
using ClimbingCompetition;
using ClimbingCompetition.Online;
using System.IO;
using System.Web.UI.WebControls;

namespace WebClimbing.src
{
    public static class ExtensionMethods
    {
        public static void AddDateTime(this Dictionary<string, string> dict, string key, DateTime? value)
        {
            if (value == null || !value.HasValue)
                return;
            dict.Add(key, value.Value.ToUniversalTime().ToString("d MMMM yyyy г.", new CultureInfo("ru-RU")));
        }
        public static void AddString(this Dictionary<string, string> dict, string key, string value)
        {
            if (string.IsNullOrEmpty(value))
                return;
            dict.Add(key, value);
        }

        private static bool GetStyleComp(ONLCompetition comp, string styleIndex)
        {
            try
            {
                string s = comp.GetStringParam(Constants.PDB_COMP_STYLES);
                if (String.IsNullOrEmpty(s))
                    return false;
                return s.IndexOf(styleIndex) > -1;
            }
            catch { return false; }
        }

        public static bool Lead(this ONLCompetition comp) { return GetStyleComp(comp, "L"); }
        public static bool Speed(this ONLCompetition comp) { return GetStyleComp(comp, "S"); }
        public static bool Boulder(this ONLCompetition comp) { return GetStyleComp(comp, "B"); }

        public static bool HasClimbers(this ONLteam team, long compID)
        {
            return (team.ONLClimberCompLinks.Count(l => l.comp_id == compID) > 0);
        }

        public static bool HasClimbers(this ONLteam team, long compID, int groupID)
        {
            return (team.ONLClimberCompLinks.Count(l => (l.comp_id == compID && l.group_id == groupID)) > 0);
        }
        

        public static string GetStringParam(this ONLCompetition comp, string paramName)
        {
            var ps = from p in comp.ONLCompetitionParams
                     where p.ParamName.ToLower() == paramName.ToLower()
                     select p.ParamValue;
            if (ps == null || ps.Count() < 1)
                return String.Empty;
            return ps.First();
        }

        public static long SetStringParam(this ONLCompetition comp, string paramName, string value, long? iidToSet = null)
        {
            return comp.SetObjectParam(paramName, value, (o => (string)o), iidToSet);
        }

        public static long SetObjectParam(this ONLCompetition comp, string paramName,object value, Func<object,string> stringRepFunc, long? iidToSet = null)
        {
            ONLCompetitionParam ps;
            try { ps = comp.ONLCompetitionParams.First(p => p.ParamName.Equals(paramName, StringComparison.InvariantCultureIgnoreCase)); }
            catch { ps = null; }
            if (ps == null)
            {
                if (value == null)
                    return -1;
                long pIid;
                if (iidToSet == null)
                    try { pIid = dc.ONLCompetitionParams.OrderByDescending(c => c.iid).First().iid + 1; }
                    catch { pIid = 1; }
                else
                    pIid = iidToSet.Value;
                ONLCompetitionParam p = ONLCompetitionParam.CreateONLCompetitionParam(pIid, comp.iid, paramName);
                p.ParamValue = stringRepFunc(value);
                comp.ONLCompetitionParams.Add(p);
                return pIid;
            }
            else
            {
                if (value == null)
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandText = "DELETE FROM ONLCompetitionParams WHERE iid=" + ps.iid;
                    cmd.ExecuteNonQuery();
                }
                else
                    ps.ParamValue = stringRepFunc(value);
                return -1;
            }
        }

        public static long SetDateParam(this ONLCompetition comp, string paramName, DateTime? value, long? iidToSet = null)
        {
            return comp.SetObjectParam(paramName, value, (d => ((DateTime?)d).Value.ToString("dd MMMM yyyy", new CultureInfo("ru-RU"))), iidToSet);
        }

        public static long SetEnumParam<T>(this ONLCompetition comp, string paramName, T value, long? iidToSet = null)
            where T : struct
        {
            Type underlyingType = Enum.GetUnderlyingType(typeof(T));
            object typeVal = Convert.ChangeType(value, underlyingType);
            return comp.SetObjectParam(paramName, typeVal, (c => c.ToString()), iidToSet);
        }

        private delegate Nullable<T> GetNullableParam<T>(ONLCompetition comp, string paramName) where T : struct;
        private static T GetParam<T>(ONLCompetition comp, string paramName, GetNullableParam<T> nullParamFunction, T defValue)
            where T : struct
        {
            Nullable<T> nulRes = nullParamFunction(comp, paramName);
            return (nulRes == null ? defValue : nulRes.Value);
        }

        public static DateTime? GetDateParamNullable(this ONLCompetition comp, string paramName)
        {
            string val = comp.GetStringParam(paramName);
            if (String.IsNullOrEmpty(val))
                return null;
            DateTime dt;
            if (DateTime.TryParse(val, new CultureInfo("ru-RU"), DateTimeStyles.AssumeUniversal, out dt))
                return dt;
            else
                return null;
        }
        public static DateTime GetDateParam(this ONLCompetition comp, string paramName)
        {
            return GetParam<DateTime>(comp, paramName, GetDateParamNullable, DefaultDate);
        }

        private static DateTime DefaultDate = DateTime.Parse("01.01.2011", new CultureInfo("ru-RU"), DateTimeStyles.AssumeUniversal);

        public static long? GetLongParamNullable(this ONLCompetition comp, string paramName)
        {
            string val = comp.GetStringParam(paramName);
            if (String.IsNullOrEmpty(val))
                return null;
            long l;
            if (long.TryParse(val, out l))
                return l;
            else
                return null;
        }
        public static long GetLongParam(this ONLCompetition comp, string paramName)
        {
            return GetParam<long>(comp, paramName, GetLongParamNullable, long.MinValue);
        }

        public static int? GetIntParamNullable(this ONLCompetition comp, string paramName)
        {
            long? res = comp.GetLongParamNullable(paramName);
            return (res == null ? null : new int?((int)res.Value));
        }
        public static int GetIntParam(this ONLCompetition comp, string paramName)
        {
            return GetParam<int>(comp, paramName, GetIntParamNullable, int.MinValue);
        }

        public static bool? GetBooleanParamNullable(this ONLCompetition comp, string paramName)
        {
            string val = comp.GetStringParam(paramName);
            if (String.IsNullOrEmpty(val))
                return null;
            bool res;
            if (bool.TryParse(val, out res))
                return res;
            var ip = comp.GetLongParamNullable(paramName);
            return (ip == null ? null : new bool?(ip.Value > 0));
        }
        public static bool GetBooleanParam(this ONLCompetition comp, string paramName)
        {
            return GetParam<bool>(comp, paramName, GetBooleanParamNullable, false);
        }

        public static Nullable<T> GetEnumParamNullable<T>(this ONLCompetition comp, string paramName)
            where T : struct
        {
            int? val = comp.GetIntParamNullable(paramName);
            if (val == null)
                return null;
            try { return new T?((T)Enum.ToObject(typeof(T), val.Value)); }
            catch { return null; }
        }

        //public static T GetEnumParam<T>(this ONLCompetition comp, string paramName, T defaultValue)
        //    where T : struct
        //{
        //    return GetParam<T>(comp, paramName, GetEnumParamNullable<T>, defaultValue);
        //}

        public static T GetEnumParam<T>(this ONLCompetition comp, string paramName, T defaultValue)
        {
            try
            {
                long? l = comp.GetLongParamNullable(paramName);
                if (l == null || !l.HasValue)
                    return defaultValue;
                return (T)Enum.ToObject(typeof(T), l.Value);
            }
            catch { return defaultValue; }
        }

        public static double? GetDoubleParamNullable(this ONLCompetition comp, string paramName)
        {
            string val = comp.GetStringParam(paramName);
            if (String.IsNullOrEmpty(val))
                return null;
            double res;
            if (double.TryParse(val, out res))
                return res;
            else
                return null;
        }

        public static double GetDoubleParam(this ONLCompetition comp, string paramName)
        {
            return GetParam<double>(comp, paramName, GetDoubleParamNullable, 0.0);
        }

        public static long GetLongParam(this HttpRequest request, string paramName)
        {
            try
            {
                string str = request.QueryString[paramName];
                if (String.IsNullOrEmpty(str))
                    return -1;
                long l;
                if (long.TryParse(str, out l))
                    return l;
                else
                    return -1;
            }
            catch { return -1; }
        }

        public static long GetCompID(this System.Web.UI.UserControl page)
        {
            return GetCompID(page.Session, page.Request);
        }

        public static long GetCompID(this System.Web.UI.Page page)
        {
            return GetCompID(page.Session, page.Request);
        }

        private static long GetCompID(HttpSessionState session, HttpRequest request)
        {
            try
            {
                long n = request.GetLongParam(Constants.PARAM_COMP_ID);
                if (n > 0)
                {
                    try { session[Constants.PARAM_COMP_ID] = n; }
                    catch { session.Add(Constants.PARAM_COMP_ID, n); }
                    return n;
                }
            }
            catch { }
            try
            {
                object fromSession = session[Constants.PARAM_COMP_ID];
                if (fromSession != null)
                    return Convert.ToInt64(fromSession);
            }
            catch { }
            return -1;
        }

        public const string DEFAULT_REDIRECT = "~/Default.aspx?" + Constants.PARAM_NO_REDIR + "=true";

        private static SqlConnection _cn = null;

        private static SqlConnection cn
        {
            get
            {
                if (_cn == null)
                    _cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["db"].ConnectionString);
                if (_cn.State != System.Data.ConnectionState.Open)
                    _cn.Open();
                return _cn;
            }
        }

        private static Entities _dc = null;
        public static Entities dc
        {
            get
            {
                if (_dc == null)
                    _dc = new Entities(WebConfigurationManager.ConnectionStrings["db_Entities"].ConnectionString);
                if (_dc.Connection.State != System.Data.ConnectionState.Open)
                    _dc.Connection.Open();
                return _dc;
            }
        }

        public static bool IsInRole(this IPrincipal prnc, string role, long compID)
        {
            var usrRoleListCount = (from ur in dc.ONLuserRoles
                                    where ur.comp_id == compID
                                    && ur.user_id == prnc.Identity.Name
                                    && ur.role_id == role
                                    select ur).Count();
            if (usrRoleListCount > 0)
                return true;
            if (role.Equals(Constants.ROLE_ADMIN) || role.Equals(Constants.ROLE_ADMIN_ROOT))
                return prnc.IsInRole(Constants.ROLE_ADMIN_ROOT);
            else
                return false;
        }

        public static bool IsAuthenticated(this IIdentity identity, long compID)
        {
            if (!identity.IsAuthenticated)
                return false;
            var usrRoleListCount = (from ur in dc.ONLuserRoles
                                    where ur.comp_id == compID
                                    && ur.user_id == identity.Name
                                    select ur).Count();
            if (usrRoleListCount > 0)
                return true;
            usrRoleListCount = (from ur in dc.ONLuserRoles
                                where ur.user_id == identity.Name
                                && ur.role_id == Constants.ROLE_ADMIN_ROOT
                                select ur).Count();
            return (usrRoleListCount > 0);
        }

        public static ONLTeamsCompLink AddTeamToCompetition(this ONLteam team, long compID, int? ranking = null)
        {
            int rToSet = (ranking == null ? int.MaxValue : ranking.Value);
            if (team.ONLTeamsCompLinks.Count(l => l.comp_id == compID) > 0)
            {
                ONLTeamsCompLink lnkF = dc.ONLTeamsCompLinks.First(l => l.team_id == team.iid && l.comp_id == compID);
                if (ranking != null && lnkF.ranking_pos != rToSet)
                {
                    lnkF.ranking_pos = rToSet;
                    dc.SaveChanges();
                }
                return lnkF;
            }
            long newID;
            try { newID = dc.ONLTeamsCompLinks.OrderByDescending(l => l.iid).First().iid + 1; }
            catch { newID = 1; }
            int secretaryID;
            try
            {
                secretaryID = dc.ONLTeamsCompLinks.Where(l => l.comp_id == compID).
                OrderByDescending(li => li.secretary_id).First().secretary_id + 1;
            }
            catch { secretaryID = 1; }
            ONLTeamsCompLink ln = ONLTeamsCompLink.CreateONLTeamsCompLink(newID, team.iid, compID, secretaryID, rToSet);
            dc.ONLTeamsCompLinks.AddObject(ln);
            dc.SaveChanges();
            return ln;
        }

        public static void RemoveTeamFromCompetition(this ONLteam team, long compID)
        {
            var lnkList = from l in dc.ONLTeamsCompLinks
                          where l.team_id == team.iid
                          && l.comp_id == compID
                          select l;
            if (lnkList.Count() < 1)
                return;
            var Arr = lnkList.ToArray();

            for (int i = 0; i < Arr.Length; i++)
                dc.ONLTeamsCompLinks.DeleteObject(Arr[i]);
            dc.SaveChanges();
        }
        public static bool IsInRoleExclusive(this ONLuser user, long compID, string role)
        {
            return dc.ONLuserRoles.Count(r => r.user_id == user.iid
                && r.comp_id == compID
                && r.role_id == role) > 0;
        }
        public static bool IsInRole(this ONLuser user, long compID, string role)
        {
            bool isDBAdmin = (dc.ONLuserRoles.Count(r => r.user_id == user.iid && r.role_id == Constants.ROLE_ADMIN_ROOT) > 0);
            if (isDBAdmin)
                return true;
            string[] roleToCheck;
            if (role == Constants.ROLE_ADMIN)
                roleToCheck = new string[] { Constants.ROLE_ADMIN };
            else
                roleToCheck = new string[] { Constants.ROLE_ADMIN, Constants.ROLE_USER };

            return (dc.ONLuserRoles.Count(r => r.user_id == user.iid
                && r.comp_id == compID
                && roleToCheck.Contains(r.role_id)) > 0);
        }

        public static void AddUserToCompetition(this ONLuser user, long compID, string role = Constants.ROLE_USER)
        {
            bool exists = (dc.ONLuserRoles.Count(r => r.user_id == user.iid
                   && r.comp_id == compID
                   && r.role_id == role) > 0);
            if (!exists)
            {
                long newID;
                try { newID = dc.ONLuserRoles.OrderByDescending(r => r.iid).First().iid + 1; }
                catch { newID = 1; }
                ONLuserRole newRole = ONLuserRole.CreateONLuserRole(newID, user.iid, role);
                newRole.notifSent = String.Empty;
                newRole.comp_id = compID;
                dc.ONLuserRoles.AddObject(newRole);
                dc.SaveChanges();
            }
            var toDelList1 = from r in dc.ONLuserRoles
                             where r.user_id == user.iid
                             && r.comp_id == compID
                             && r.role_id != role
                             select r;
            if (toDelList1.Count() < 1)
                return;
            var td = toDelList1.ToArray();
            for (int i = 0; i < td.Length; i++)
                dc.ONLuserRoles.DeleteObject(td[i]);
            dc.SaveChanges();
        }

        public static void RemoveUserFromCompetition(this ONLuser user, long compID, string role = null)
        {
            var roleList = from r in dc.ONLuserRoles
                           where r.user_id == user.iid
                           && r.comp_id == compID
                           select r;
            if (!String.IsNullOrEmpty(role))
                roleList = roleList.Where(r => r.role_id == role);
            if (roleList.Count() < 1)
                return;
            var rL = roleList.ToArray();
            for (int i = 0; i < rL.Length; i++)
                dc.ONLuserRoles.DeleteObject(rL[i]);
            dc.SaveChanges();
        }

        public static int GetNextNumber(this ONLGroupsCompLink grp)
        {
            int lastSecr;
            var climbers = from c in dc.ONLClimberCompLinks
                           where c.comp_id == grp.comp_id
                           && c.group_id == grp.group_id
                           orderby c.secretary_id
                           select c;
            var compGroups = (from g in dc.ONLGroupsCompLinks
                              where g.comp_id == grp.comp_id
                              orderby g.ONLGroup.oldYear, g.ONLGroup.genderFemale
                              select g).ToArray();
            int curGrPos;
            for (curGrPos = 0; curGrPos < compGroups.Length; curGrPos++)
                if (grp.iid == compGroups[curGrPos].iid)
                    break;

            lastSecr = climbers.Count() < 1 ? 0 : climbers.OrderByDescending(c => c.secretary_id).First().secretary_id;
            if (lastSecr == 0)
                lastSecr = curGrPos * 100 + 1;
            else if (lastSecr % 100 == 99)
            {
                int startSer = lastSecr - (lastSecr % 100);
                int nextSer = (startSer / 100 == curGrPos) ? compGroups.Length * 100 : startSer + 100;
                for (int i = 0; i < 100; i++)
                {
                    if (!grp.ONLCompetition.CheckNumber(nextSer + 1))
                    {
                        lastSecr = nextSer + 1;
                        break;
                    }
                    else
                        nextSer += 100;
                }
            }
            else
            {
                bool refused = true;
                do
                {
                    refused = grp.ONLCompetition.CheckNumber(++lastSecr);
                } while (refused && lastSecr % 100 != 99);
                if (refused)
                    lastSecr = grp.ONLCompetition.GetNextNumberGeneral();
            }
            return grp.ONLCompetition.CheckNumber(lastSecr) ? grp.ONLCompetition.GetNextNumberGeneral() : lastSecr;
        }

        public static int GetNextNumberGeneral(this ONLCompetition comp)
        {
            return comp.ONLClimberCompLinks.Count() < 1 ? 1 :
                comp.ONLClimberCompLinks.OrderByDescending(l => l.secretary_id).First().secretary_id + 1;
        }

        public static bool CheckNumber(this ONLCompetition comp, int number)
        {
            return (comp.ONLClimberCompLinks.Count(lnk => lnk.secretary_id == number) > 0);
        }

        public static string GetBinaryParamLink(this ONLCompetition comp, string paramName)
        {
            try
            {
                var ll = comp.GetBinaryParamLinks(p => p.ParamName.Equals(paramName));
                if (ll == null || ll.Count < 1)
                    return String.Empty;
                else
                    return ll[paramName];
            }
            catch { return String.Empty; }
        }

        public static Dictionary<string, string> GetBinaryParamLinks(this ONLCompetition comp, Func<ONLCompetitionParam, bool> selector)
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            try
            {
                var selectedPList = comp.ONLCompetitionParams.Where(selector).Where(p => p.ONLCompetitionBinaryParam != null).ToArray();

                foreach (var p in selectedPList)
                    retVal.Add(p.ParamName, p.ONLCompetitionBinaryParam.CheckAndSaveBinaryData());
            }
            catch { }
            return retVal;
        }

        public static void SetHyperlinkVisible(this HyperLink hyperlink, ONLCompetition comp, string paramName)
        {
            try
            {
                if (comp == null)
                {
                    hyperlink.Visible = false;
                    return;
                }
                string linkToSet = comp.GetBinaryParamLink(paramName);
                if (String.IsNullOrEmpty(linkToSet))
                    hyperlink.Visible = false;
                else
                {
                    hyperlink.Visible = true;
                    hyperlink.NavigateUrl = linkToSet;
                }
            }
            catch { }
        }

        public static string CheckAndSaveBinaryData(this ONLCompetitionBinaryParam param)
        {
            try
            {
                string baseDir = System.Threading.Thread.GetDomain().BaseDirectory;
                string fileName = "files";
                string fullName = Path.Combine(baseDir, fileName);
                if (!Directory.Exists(fullName))
                    Directory.CreateDirectory(fullName);
                fileName = Path.Combine(fileName, param.ONLCompetitionParam.comp_id.ToString("000"));
                fullName = Path.Combine(baseDir, fileName);
                if (!Directory.Exists(fullName))
                    Directory.CreateDirectory(fullName);
                var pL = (from p in dc.ONLCompetitionParams
                          where p.comp_id == param.ONLCompetitionParam.comp_id
                          && p.ParamName.Equals(Constants.PDB_BINARY_UPDATED)
                          && p.ParamValue.Equals(param.ONLCompetitionParam.ParamName)
                          select p).ToArray();

                bool needRefrsh = pL.Length > 0;
                fileName = Path.Combine(fileName, param.ONLCompetitionParam.ParamValue);

                fullName = Path.Combine(baseDir, fileName);

                if (!needRefrsh)
                    needRefrsh = !File.Exists(fullName);
                if (!needRefrsh)
                    return "~/" + fileName.Replace('\\', '/');
                if (File.Exists(fullName))
                    for (int i = 0; i < 10; i++)
                        try
                        {
                            File.Delete(fullName);
                            break;
                        }
                        catch { }
                if (param.paramValue == null || param.paramValue.Length < 1)
                    return String.Empty;
                FileStream fstr = new FileStream(fullName, FileMode.Create, FileAccess.Write);
                try { fstr.Write(param.paramValue, 0, param.paramValue.Length); }
                finally { fstr.Close(); }

                if (pL.Length > 0)
                {
                    foreach (var v in pL)
                        dc.ONLCompetitionParams.DeleteObject(v);
                    dc.SaveChanges();
                }
                return "~/" + fileName.Replace('\\', '/');
            }
            catch { return String.Empty; }
        }

        public static long? GetClimbersGroup(this ONLCompetition comp, int birthyear, bool genderFemale)
        {
            int age = comp.GetCompYear() - birthyear;
            var groupsList = (from cl in comp.ONLGroupsCompLinks
                              where cl.ONLGroup.genderFemale == genderFemale
                              && cl.ONLGroup.oldYear >= age
                              && cl.ONLGroup.youngYear <= age
                              select cl.iid).ToArray();
            if (groupsList.Length < 1)
                return null;
            else
                return new long?(groupsList[0]);
        }

        public static int GetCompYear(this ONLCompetition comp)
        {
            var dt = comp.GetDateParamNullable(Constants.PDB_COMP_START_DATE);
            if (dt == null || !dt.HasValue)
                return DateTime.UtcNow.Year;
            else
                return dt.Value.Year;
        }
    }
}