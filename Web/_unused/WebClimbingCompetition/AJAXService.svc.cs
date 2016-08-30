// <copyright file="AJAXService.svc.cs">
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
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Caching;
using WebClimbing.src;

namespace WebClimbing
{
    [ServiceContract(Namespace = "WebClimbing")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AJAXService
    {
        private static Entities dcStatic { get { return ExtensionMethods.dc; } }

        private const string CACHE_PREFIX = "CACHECLIMBERS_";
        private ClimberData[] GetClimberLike(string src, string teamIDStr, long compID)
        {

            string srcToCheck = src.Replace('ё', 'е').ToLower(); ;
            while (srcToCheck.IndexOf("  ") > -1)
                srcToCheck.Replace(" ", String.Empty);
            
            string cacheToCheck = (srcToCheck.Length < 1 ? srcToCheck : srcToCheck.Substring(0, 1));
            List<ClimberData> cacheList;
            if (HttpRuntime.Cache[CACHE_PREFIX + cacheToCheck] == null)
            {
                var wh = dcStatic.ONLclimbers.Where(c => (c.surname + " " + c.name).IndexOf(cacheToCheck) == 0);
                cacheList = new List<ClimberData>();
                foreach (var v in wh)
                    cacheList.Add(new ClimberData(v));
                if (cacheList.Count < 1)
                    return null;
                HttpRuntime.Cache.Add(CACHE_PREFIX + cacheToCheck, cacheList,
                    null, DateTime.Now.AddHours(0.5d), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }
            else
                cacheList = (List<ClimberData>)HttpRuntime.Cache[CACHE_PREFIX + cacheToCheck];

            var clmLst = cacheList.Where(c => 
#if !DEBUG
                !c.comps.Contains(compID) && 
#endif
                c.Name.ToLower().IndexOf(srcToCheck) == 0);
            int teamID;
            if (!String.IsNullOrEmpty(teamIDStr) && int.TryParse(teamIDStr, out teamID))
                clmLst = clmLst.Where(c => c.teamIDs.Contains(teamID));
            return clmLst.OrderBy(c => c.Name).ToArray();
        }


        // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
        // To create an operation that returns XML,
        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
        //     and include the following line in the operation body:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
        [OperationContract]
        public ClimberData GetClimberData(string clmName, string teamID, long compID)
        {
            try
            {
                var v = GetClimberLike(clmName, teamID, compID);
                if (v == null || v.Length < 1)
                    return null;
                else
                    return v[0];
            }
            catch { return null; }
        }

        [OperationContract]
        public string ValidateClimber(string name, int age, bool genderFemale, long compID, string teamIDS, int applThreshold)
        {
            if (String.IsNullOrEmpty(name))
                return String.Empty;

            if (age <= 30)
                age += 2000;
            else if (age <= 99)
                age += 1900;
            try
            {
                var groupLinkID = dcStatic.ONLCompetitions.Where(c => c.iid == compID).First().GetClimbersGroup(age, genderFemale);
                if (groupLinkID == null || !groupLinkID.HasValue)
                    return "Участник \"" + name + "\" не входит ни в одну возрастную группу";
            }
            catch { return "Участник \"" + name + "\" не входит ни в одну возрастную группу"; }

            string clmToCheck = name.Trim().Replace('ё', 'е');
            var listToCheck = from cl in dcStatic.ONLClimberCompLinks
                              where cl.comp_id == compID
                              && cl.ONLclimber.age == age
                              && cl.ONLclimber.genderFemale == genderFemale
                              && clmToCheck.Equals(cl.ONLclimber.surname + " " + cl.ONLclimber.name)
                              select cl;
            int teamID;
            if (!String.IsNullOrEmpty(teamIDS) && int.TryParse(teamIDS, out teamID))
                listToCheck = listToCheck.Where(l => l.team_id == teamID);
            if (listToCheck.Count() > applThreshold)
                return "Участник \"" + name + "\" уже заявлен на данные соревнования";
            else
                return String.Empty;
        }

        public class ClimberData
        {
            public string Name;
            public string Age;
            public int Gender;
            public string Qf;
            public int[] teamIDs;
            public long[] comps;
            public ClimberData()
            {
                Name = Age = string.Empty;
                this.Gender = 0;
                teamIDs = new int[0];
                comps = new long[0];
            }

            public ClimberData(ONLclimber clm)
            {
                this.Name = clm.surname + " " + clm.name;
                this.Age = ((clm.age == null || !clm.age.HasValue) ? clm.birthdate.Year : clm.age.Value).ToString();
                this.Gender = clm.genderFemale ? 1 : 0;
                try
                {
                    var lastCompLink = clm.ONLClimberCompLinks.OrderByDescending(l => l.iid).First();
                    this.Qf = lastCompLink.qf;
                }
                catch { this.Qf = String.Empty; }
                this.teamIDs = (from cl in clm.ONLClimberCompLinks
                                select cl.team_id).Distinct().ToArray();
                this.comps = (from cl in clm.ONLClimberCompLinks
                              select cl.comp_id).Distinct().ToArray();
            }
        }

        // Add more operations here and mark them with [OperationContract]
    }
}
