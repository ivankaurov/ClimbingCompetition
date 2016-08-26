using ClimbingCompetition.Client;
using ClimbingCompetition.Common;
using ClimbingCompetition.Common.API;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ClimbingCompetition
{
    partial class OnlineUpdater2
    {
        private static readonly CultureInfo defaultUICulture = CultureInfo.GetCultureInfo("ru-RU");

        public static CultureInfo DefaultUICulture { get { return OnlineUpdater2.defaultUICulture; } }

        private ApiListHeader GetListHeader(int iid)
        {
            using(var cmd = this.CreateCommand())
            {
                cmd.CommandText = "select G." + ServiceHelper.REMOTE_ID_COLUMN + " group_id, L.listType, L.style, L.round, isnull(L.quote,0) quote," +
                                  "       L.allowView, isnull(LP." + ServiceHelper.REMOTE_ID_COLUMN + ", '') prevRoundId, isnull(L.routeNumber,0) routeNumber," +
                                  "       isnull(PR." + ServiceHelper.REMOTE_ID_COLUMN + ", '') parentIid," +
                                  "       IsNull(L." + ServiceHelper.REMOTE_ID_COLUMN + ", '') " + ServiceHelper.REMOTE_ID_COLUMN +
                                  "  from Lists L(nolock)" +
                              " left join Lists LP(nolock) on LP.iid = L.prev_round" +
                              " left join Lists PR(nolock) on PR.iid = L.iid_parent" +
                              "      join Groups G(nolock) on G.iid = L.group_id" +
                              "     where L.iid = @iid";
                cmd.Parameters.Add("@iid", SqlDbType.Int).Value = iid;

                bool iRulesLead = SettingsForm.GetLeaveTrains(this.cn, this.currentTransaction);
                var sr = SettingsForm.GetSpeedRules(this.cn, this.currentTransaction);

                using (var rdr = cmd.ExecuteReader())
                {
                    if (!rdr.Read())
                        return null;
                    int routeNumber;
                    var round = ClimbingRoundExtensions.GetByLocalizedValue(rdr["round"].ToString(), out routeNumber, DefaultUICulture);
                    if (routeNumber < 1)
                        routeNumber = Convert.ToInt32(rdr["routeNumber"]);

                    ListType listType;
                    try { listType = (ListType)Enum.Parse(typeof(ListType), rdr["listType"].ToString(), true); }
                    catch { listType = ListType.Unknown; }

                    Rules r;
                    var style = Extensions.StringExtensions.GetEnumByStringValue<ClimbingStyles>(rdr["style"].ToString(), ClimbingStyles.Lead, DefaultUICulture);
                    if (style == ClimbingStyles.Speed)
                    {
                        r = (sr & SpeedRules.InternationalSchema) == SpeedRules.InternationalSchema ? Rules.International : Rules.Russian;
                        if ((sr & SpeedRules.BestResultFromTwoQfRounds) == SpeedRules.BestResultFromTwoQfRounds)
                            r = r | Rules.BestRouteInQf;
                    }
                    else
                    {
                        r = iRulesLead ? Rules.International : Rules.Russian;
                    }
                    return new ApiListHeader
                    {
                        AgeGroupInCompId = rdr["group_id"].ToString(),
                        ClimbingRules = r,
                        IidParent = rdr["parentIid"].ToString(),
                        ListTypeV = listType,
                        Live = Convert.ToInt32(rdr["allowView"]) > 0,
                        PreviousRoundId = rdr["prevRoundId"].ToString(),
                        Quota = Convert.ToInt32(rdr["quote"]),
                        Round = round,
                        RouteNumber = routeNumber,
                        Style = style,
                        Iid = rdr[ServiceHelper.REMOTE_ID_COLUMN].ToString(),
                        SecretaryId = iid
                    };
                }
            }
        }

        private ApiListLineSpeed GetListLineSpeed(long iid)
        {
            using (var cmd = this.CreateCommand())
            {
                cmd.CommandText = "SELECT P." + ServiceHelper.REMOTE_ID_COLUMN + " climberId, isnull(R.start,0) start," +
                                  "       isnull(R.res, 0) res, isnull(R.resText, '') resText," +
                                  "       isnull(R.route1,0) route1, isnull(R.route2,0) route2," +
                                  "       isnull(R.route1_text,'') route1_text, isnull(R.route2_text, '') route2_text," +
                                  "       L." + ServiceHelper.REMOTE_ID_COLUMN + " listId," +
                                  "       isnull(R." + ServiceHelper.REMOTE_ID_COLUMN + ", '') resId" +
                                  "  FROM speedResults R(nolock)" +
                                  "  JOIN Participants P(nolock) on P.iid = R.climber_id" +
                                  "                             and isnull(P." + ServiceHelper.REMOTE_ID_COLUMN + ",'') <> ''" +
                                  "  JOIN Lists L(nolock) on L.iid = R.list_id" +
                                  "                      and isnull(L." + ServiceHelper.REMOTE_ID_COLUMN + ",'') <> ''" +
                                  " WHERE R.iid = @iid";
                cmd.Parameters.Add("@iid", SqlDbType.Int).Value = iid;
                using (var rdr = cmd.ExecuteReader())
                {
                    if (!rdr.Read())
                        return null;
                    var resText = rdr["resText"].ToString().Trim();

                    ApiListLineSpeed.ResultType resType;
                    switch (resText)
                    {
                        case "н/я":
                            resType = ApiListLineSpeed.ResultType.Dns;
                            break;
                        case "срыв":
                            resType = ApiListLineSpeed.ResultType.Fall;
                            break;
                        case "дискв.":
                            resType = ApiListLineSpeed.ResultType.Dsq;
                            break;
                        default:
                            resType = ApiListLineSpeed.ResultType.Time;
                            break;
                    }

                    return new ApiListLineSpeed
                    {
                        ClimberId = rdr["climberId"].ToString(),
                        Iid = rdr["resId"].ToString(),
                        ListId = rdr["listId"].ToString(),
                        ResText = resText,
                        Result = Convert.ToInt64(rdr["res"]),
                        Route1Res = Convert.ToInt64(rdr["route1"]),
                        Route1Text = rdr["route1_text"].ToString(),
                        Route2Res = Convert.ToInt64(rdr["route2"]),
                        Route2Text = rdr["route2_text"].ToString(),
                        SecretaryId = iid,
                        Start = Convert.ToInt32(rdr["start"]),
                        TotalResType = resType
                    };
                }
            }
        }

        private ApiListLineLead GetListLineLead(long iid)
        {
            using(var cmd = this.CreateCommand())
            {
                cmd.CommandText = "SELECT P." + ServiceHelper.REMOTE_ID_COLUMN + " climberId, isnull(R.start,0) start, isnull(R.resText,'') resText, isnull(R.res,0) res," +
                                  "       isnull(R.timeValue, 0) time, isnull(R.timeText, '') timeText, L." + ServiceHelper.REMOTE_ID_COLUMN + " iid_parent" +
                                  "  FROM routeResults R(nolock)" +
                                  "  JOIN Participants P(nolock) on P.iid = R.climber_id and IsNull(P." + ServiceHelper.REMOTE_ID_COLUMN + ", '') <> ''" +
                                  "  JOIN Lists L(nolock) on L.iid = R.list_id and IsNull(L." + ServiceHelper.REMOTE_ID_COLUMN + ",'') <> ''" +
                                  " WHERE R.iid=@iid";
                cmd.Parameters.Add("@iid", SqlDbType.BigInt).Value = iid;
                using(var rdr = cmd.ExecuteReader())
                {
                    if (!rdr.Read())
                        return null;
                    return new ApiListLineLead
                    {
                        ClimberId = rdr["climberId"].ToString(),
                        ListId = rdr["iid_parent"].ToString(),
                        ResText = rdr["resText"].ToString(),
                        Result = Convert.ToInt64(rdr["res"]),
                        SecretaryId = iid,
                        Start = Convert.ToInt32(rdr["start"]),
                        TimeText = rdr["timeText"].ToString(),
                        TimeValue = Convert.ToInt64(rdr["time"])
                    };
                }
            }
        }

        private ApiListLineBoulder GetListLineBoulder(long iid)
        {
            using(var cmd = this.CreateCommand())
            {
                int routeNumber;

                cmd.CommandText = "SELECT P." + ServiceHelper.REMOTE_ID_COLUMN + " climberId, isnull(R.start,0) start," +
                                  "       isnull(R.nya, 0) nya, isnull(R.disq, 0) disq,"+
                                  "       isnull(L.routeNumber,0) routeNumber, L." + ServiceHelper.REMOTE_ID_COLUMN + " iid_parent" +
                                  "  FROM boulderResults R(nolock)" +
                                  "  JOIN Participants P(nolock) on P.iid = R.climber_id" +
                                  "                             and isnull(P." + ServiceHelper.REMOTE_ID_COLUMN + ",'') <> ''" +
                                  "  JOIN Lists L(nolock) on L.iid = R.list_id and isnull(L." + ServiceHelper.REMOTE_ID_COLUMN + ",'') <> ''" +
                                  " WHERE R.iid = @iid";
                cmd.Parameters.Add("@iid", SqlDbType.Int).Value = iid;
                ApiListLineBoulder result;
                using(var rdr = cmd.ExecuteReader())
                {
                    if (!rdr.Read())
                        return null;
                    routeNumber = Convert.ToInt32(rdr["routeNumber"]);

                    result = new ApiListLineBoulder
                    {
                        ClimberId = rdr["climberId"].ToString(),
                        ListId = rdr["iid_parent"].ToString(),
                        SecretaryId = iid,
                        Start = Convert.ToInt32(rdr["start"])
                    };

                    if (Convert.ToInt32(rdr["disq"]) > 0)
                        result.ResultType = ResultType.DSQ;
                    else if (Convert.ToInt32(rdr["nya"]) > 0)
                        result.ResultType = ResultType.DNS;
                    else
                        result.ResultType = ResultType.RES;
                }

                List<ApiListLineBoulderRoute> boulderRoutes = new List<ApiListLineBoulderRoute>();

                if (routeNumber > 0)
                {
                    cmd.CommandText = "SELECT R.routeN, R.topA, R.bonusA" +
                                      "  FROM boulderRoutes R(nolock)" +
                                      " WHERE R.iid_parent = @iid" +
                                      "   AND R.topA is not null" +
                                      "   AND R.bonusA is not null" +
                                      "   AND R.routeN between 1 and @routeN";
                    cmd.Parameters.Add("@routeN", SqlDbType.Int).Value = routeNumber;
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            boulderRoutes.Add(new ApiListLineBoulderRoute
                            {
                                RouteNumber = Convert.ToInt32(rdr["routeN"]),
                                TopAttempt = Convert.ToInt32(rdr["topA"]),
                                BonusAttempt = Convert.ToInt32(rdr["bonusA"])
                            });
                        }
                    }
                }

                result.Routes = boulderRoutes.ToArray();
                return result;
            }
        }

        private void FinalizeList(ApiListHeader result)
        {
            if (result == null || string.IsNullOrEmpty(result.Iid))
                return;
            using(var cmd = this.CreateCommand())
            {
                cmd.CommandText = "UPDATE Lists SET " + ServiceHelper.REMOTE_ID_COLUMN + " = @remoteIid, changed = 0, online = 1 where iid = @iid";
                cmd.Parameters.Add("@remoteIid", SqlDbType.VarChar, ServiceHelper.REMOTE_ID_COL_SIZE).Value = result.Iid;
                cmd.Parameters.Add("@iid", SqlDbType.Int).Value = result.SecretaryId;

                cmd.ExecuteNonQuery();
            }
        }

        private void FinalizeListLine(ApiListLine listLine, string tableName)
        {
            if (listLine == null || string.IsNullOrEmpty(listLine.Iid))
                return;
            using(var cmd = this.CreateCommand())
            {
                cmd.CommandText = string.Format("UPDATE {0} SET " + ServiceHelper.REMOTE_ID_COLUMN + " = @remoteIid, changed = 0 WHERE iid = @iid", tableName);
                cmd.Parameters.Add("@remoteIid", SqlDbType.VarChar, ServiceHelper.REMOTE_ID_COL_SIZE).Value = listLine.Iid;
                cmd.Parameters.Add("@iid", SqlDbType.BigInt).Value = listLine.SecretaryId;
                cmd.ExecuteNonQuery();
            }
        }

        private void LoadUpdatedResults<T>(string tableName, Func<long, T> getApiResult, Func<IEnumerable<T>, T[]> postResults)
            where T : ApiListLine
        {
            ServiceHelper.CheckRemoteIdColumn(tableName, this.cn, this.currentTransaction);
            var toUpdate = new List<long>();
            using (var cmd = this.CreateCommand())
            {
                cmd.CommandText = String.Format("SELECT iid FROM {0}(nolock) WHERE changed = 1", tableName);
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                        toUpdate.Add(Convert.ToInt64(rdr["iid"]));
                }
            }

            var u = toUpdate.Select(getApiResult).Where(r => r != null).ToArray();
            if (u.Length < 1)
                return;
            foreach (var r in postResults(u))
            {
                this.FinalizeListLine(r, tableName);
            }
        }

        private void LoadUpdatedBoulderResults()
        {
            ServiceHelper.CheckRemoteIdColumn("boulderResults", this.cn, this.currentTransaction);
            var toUpdate = new List<long>();
            using (var cmd = this.CreateCommand())
            {
                cmd.CommandText = "SELECT distinct R.iid" +
                                  "  FROM boulderResults R(nolock)" +
                              " LEFT JOIN boulderRoutes BR(nolock) on BR.iid_parent = R.iid and BR.changed = 1" +
                              "     WHERE R.changed = 1" +
                              "        OR BR.iid_parent IS NOT NULL";
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                        toUpdate.Add(Convert.ToInt64(rdr["iid"]));
                }
            }

            var u = toUpdate.Select(l => this.GetListLineBoulder(l)).Where(r => r != null).ToArray();
            this.PostBoulderResults(u);
        }

        private void PostBoulderResults(ICollection<ApiListLineBoulder> resultsToPost)
        {
            if (resultsToPost == null || resultsToPost.Count < 1)
                return;

            using (var cmd = this.CreateCommand())
            {
                cmd.CommandText = "UPDATE boulderRoutes SET changed = 0 WHERE iid_parent = @iid";
                var iidParam = cmd.Parameters.Add("@iid", SqlDbType.Int);
                foreach (var r in ServiceClient.Instance.PostBoulderResuls(resultsToPost))
                {
                    this.FinalizeListLine(r, "boulderRoutes");
                    iidParam.Value = r.SecretaryId;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void LoadUpdatedLeadResults()
        {
            this.LoadUpdatedResults("routeResults", l => this.GetListLineLead(l), u => ServiceClient.Instance.PostLeadResults(u));
        }

        private void LoadUpdatedSpeedResults()
        {
            this.LoadUpdatedResults("speedResults", l => this.GetListLineSpeed(l), u => ServiceClient.Instance.PostSpeedResults(u));
        }

        private void LoadResultsByList<T>(int listId, string tableName, Func<long,T> getResult, Func<IEnumerable<T>, T[]> postResults)
            where T : ApiListLine
        {
            List<long> toLoad = new List<long>();

            string remoteIid;

            ServiceHelper.CheckRemoteIdColumn(tableName, this.cn, this.currentTransaction);

            using (var cmd = this.CreateCommand())
            {
                cmd.CommandText = string.Format("SELECT iid FROM {0}(nolock) WHERE list_id = @iid", tableName);
                cmd.Parameters.Add("@iid", SqlDbType.Int).Value = listId;
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                        toLoad.Add(Convert.ToInt64(rdr["iid"]));
                }

                cmd.CommandText = "SELECT " + ServiceHelper.REMOTE_ID_COLUMN + " FROM lists(nolock) WHERE iid = @iid";
                remoteIid = cmd.ExecuteScalar() as string;
                if (string.IsNullOrEmpty(remoteIid))
                    return;
            }

            var lists = toLoad.Select(getResult)
                              .Where(r => r != null)
                              .ToArray();
            if (lists.Length > 0)
            {
                foreach (var r in lists = postResults(lists))
                    this.FinalizeListLine(r, tableName);
            }

            ServiceClient.Instance.ClearUnneededListResults(lists.Select(l => l.Iid), remoteIid);
        }

        private void LoadLeadResultsByList(int listId)
        {
            this.LoadResultsByList(listId, "routeResults", n => this.GetListLineLead(n), l => ServiceClient.Instance.PostLeadResults(l));
        }

        private void LoadSpeedResultsByList(int listId)
        {
            this.LoadResultsByList(listId, "speedResults", n => this.GetListLineSpeed(n), l => ServiceClient.Instance.PostSpeedResults(l));
        }

        private void LoadBoulderResultsByList(int listId)
        {
            List<long> toLoad = new List<long>();

            string remoteIid;

            ServiceHelper.CheckRemoteIdColumn("boulderResults", this.cn, this.currentTransaction);

            using (var cmd = this.CreateCommand())
            {
                cmd.CommandText ="SELECT iid FROM boulderResults(nolock) WHERE list_id = @iid";
                cmd.Parameters.Add("@iid", SqlDbType.Int).Value = listId;
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                        toLoad.Add(Convert.ToInt64(rdr["iid"]));
                }

                cmd.CommandText = "SELECT " + ServiceHelper.REMOTE_ID_COLUMN + " FROM lists(nolock) WHERE iid = @iid";
                remoteIid = cmd.ExecuteScalar() as string;
                if (string.IsNullOrEmpty(remoteIid))
                    return;
            }

            var lists = toLoad.Select(b => this.GetListLineBoulder(b))
                              .Where(r => r != null)
                              .ToArray();
            this.PostBoulderResults(lists);

            ServiceClient.Instance.ClearUnneededListResults(lists.Select(l => l.Iid), remoteIid);
        }

        public string LoadSingleList(int iid, bool loadLines)
        {
            var apiList = this.GetListHeader(iid);
            if (apiList == null)
                return null;

            apiList = ServiceClient.Instance.PostListHeader(apiList);

            this.FinalizeList(apiList);

            if(loadLines && !string.IsNullOrEmpty(apiList.Iid))
            {
                switch (apiList.Style)
                {
                    case ClimbingStyles.Lead:
                        this.LoadLeadResultsByList(apiList.SecretaryId);
                        break;
                    case ClimbingStyles.Speed:
                        this.LoadSpeedResultsByList(apiList.SecretaryId);
                        break;
                    case ClimbingStyles.Bouldering:
                        this.LoadBoulderResultsByList(apiList.SecretaryId);
                        break;
                }
            }

            return apiList.Iid;
        }

        private int LoadListsByFilter(string filter, bool loadLines)
        {
            ServiceHelper.CheckRemoteIdColumn("Lists", this.cn, this.currentTransaction);

            List<int> toLoad = new List<int>();
            using(var cmd = this.CreateCommand())
            {
                cmd.CommandText = "select L.iid FROM lists L(nolock) where L.online = 1";
                if (!string.IsNullOrEmpty(filter))
                    cmd.CommandText += " and " + filter;
                cmd.CommandText += " order by L.iid";
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                        toLoad.Add(Convert.ToInt32(rdr["iid"]));
                }
            }

            int res = 0;
            foreach (var l in toLoad)
            {
                if (!(string.IsNullOrEmpty(this.LoadSingleList(l, loadLines))))
                    res++;
            }
            return res;
        }

        public int LoadAllLists()
        {
            var res = this.LoadListsByFilter(null, true);
            List<string> resL = new List<string>();
            using (var cmd = this.CreateCommand())
            {
                cmd.CommandText = "SELECT " + ServiceHelper.REMOTE_ID_COLUMN +
                                  "  FROM Lists(nolock)" +
                                  " WHERE " + ServiceHelper.REMOTE_ID_COLUMN + " is not null" +
                                  "   AND " + ServiceHelper.REMOTE_ID_COLUMN + " <> ''";
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        resL.Add(rdr[ServiceHelper.REMOTE_ID_COLUMN].ToString());
                    }
                }
            }

            ServiceClient.Instance.ClearUnneededLists(resL);
            return res;
        }

        public int LoadLiveLists()
        {
            return this.LoadListsByFilter("L.allowView = 1", true);
        }

        public void LoadChangedLists()
        {
            this.LoadListsByFilter("L.changed = 1", false);
            this.LoadUpdatedLeadResults();
            this.LoadUpdatedSpeedResults();
            this.LoadUpdatedBoulderResults();
        }
    }
}
