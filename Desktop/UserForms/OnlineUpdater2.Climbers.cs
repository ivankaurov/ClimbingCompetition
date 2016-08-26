using ClimbingCompetition.Client;
using ClimbingCompetition.Common.API;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ClimbingCompetition
{
    public partial class OnlineUpdater2
    {
        private ApiAgeGroup GetApiAgeGroup(int iid)
        {
            using (var cmd = this.CreateCommand())
            {
                cmd.CommandText = "select name, oldYear, youngYear, genderFemale, " + ServiceHelper.REMOTE_ID_COLUMN+
                                  "  from Groups(nolock)" +
                                  " where iid = @iid";
                cmd.Parameters.Add("@iid", SqlDbType.Int).Value = iid;

                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        return new ApiAgeGroup
                        {
                            AgeGroupInCompId = rdr[ServiceHelper.REMOTE_ID_COLUMN] as string,
                            Name = rdr["name"] as string,
                            Gender = Convert.ToBoolean(rdr["genderFemale"]) ? Common.Gender.Female : Common.Gender.Male,
                            Tag = iid.ToString(),
                            YearOld = Convert.ToInt32(rdr["oldYear"]),
                            YearYoung = Convert.ToInt32(rdr["youngYear"])
                        };
                    }

                    return null;
                }
            }
        }
        private void PostGroups(IEnumerable<int> groupIDs, bool fullRefresh)
        {
            ServiceHelper.CheckRemoteIdColumn("Groups", this.cn, this.currentTransaction);
            var groupsToPost = groupIDs.ToList()
                                     .Select(n => this.GetApiAgeGroup(n))
                                     .Where(t => t != null)
                                     .ToArray();

            if (groupsToPost.Length < 1 && !fullRefresh)
                return;

            foreach(var g in  ServiceClient.Instance.PostAgeGroups(groupsToPost, fullRefresh))
            {
                this.FinalizeGroup(g);
            }
        }

        private void FinalizeGroup(ApiAgeGroup returned)
        {
            int iid;
            if (returned == null || string.IsNullOrEmpty(returned.Tag))
                return;
            if (!int.TryParse(returned.Tag, out iid))
                return;
            using(var cmd = this.CreateCommand())
            {
                cmd.CommandText = "UPDATE Groups SET oldYear=@yO, youngYear=@yY, name=@name, genderFemale=@gf, " +
                    ServiceHelper.REMOTE_ID_COLUMN + "=@remoteId, changed = 0" +
                                " WHERE iid=@iid";
                cmd.Parameters.Add("@yO", SqlDbType.Int).Value = returned.YearOld;
                cmd.Parameters.Add("@yY", SqlDbType.Int).Value = returned.YearYoung;
                cmd.Parameters.Add("@name", SqlDbType.VarChar, 255).Value = returned.Name;
                cmd.Parameters.Add("@gf", SqlDbType.Bit).Value = returned.Gender == Common.Gender.Female;
                cmd.Parameters.Add("@remoteId", SqlDbType.VarChar, ServiceHelper.REMOTE_ID_COL_SIZE).Value = returned.AgeGroupInCompId;
                cmd.Parameters.Add("@iid", SqlDbType.Int).Value = iid;
                cmd.ExecuteNonQuery();
            }
        }

        public void PostAllGroups()
        {
            var groups = new LinkedList<int>();
            using (var cmd = this.CreateCommand())
            {
                cmd.CommandText = "SELECT iid FROM Groups(nolock)";
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                        groups.AddLast(Convert.ToInt32(rdr["iid"]));
                }
            }

            this.PostGroups(groups, true);
        }

        public void PostUpdatedGroups()
        {
            var groups = new LinkedList<int>();
            using (var cmd = this.CreateCommand())
            {
                cmd.CommandText = "SELECT iid FROM Groups(nolock) WHERE changed = 1";
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                        groups.AddLast(Convert.ToInt32(rdr["iid"]));
                }
            }

            this.PostGroups(groups, false);
        }

        private ApiParticipant GetApiParticipant(int iid)
        {
            using(var cmd = this.CreateCommand())
            {
                cmd.CommandText = "select P.name, P.surname, P.age, P.genderFemale, P.qf, T.name team, G." + ServiceHelper.REMOTE_ID_COLUMN + " group_id, " +
                                  "       P.rankingLead, P.rankingSpeed, P.rankingBoulder, P.vk, P.lead, P.speed, P.boulder, P." + ServiceHelper.REMOTE_ID_COLUMN +
                                  "  from Participants P(nolock)" +
                                  "  join Groups G(nolock) on G.iid = P.group_id" +
                                  "  join Teams T(nolock) on T.iid = P.team_id" +
                                  " where P.iid = @iid";
                cmd.Parameters.Add("@iid", SqlDbType.Int).Value = iid;

                using(var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        Common.ClimbingStyles styles = 0;
                        if (Convert.ToInt32(rdr["lead"]) > 0)
                            styles = Common.ClimbingStyles.Lead;

                        if (Convert.ToInt32(rdr["speed"]) > 0)
                            styles = styles | Common.ClimbingStyles.Speed;

                        if (Convert.ToInt32(rdr["boulder"]) > 0)
                            styles = styles | Common.ClimbingStyles.Bouldering;

                        return new ApiParticipant
                        {
                            AgeGroupInCompId = rdr["group_id"].ToString(),
                            Gender = Convert.ToBoolean(rdr["genderFemale"]) ? Common.Gender.Female : Common.Gender.Male,
                            Name = rdr["name"].ToString().Trim(),
                            OldBib = iid,
                            Qf = Extensions.StringExtensions.GetEnumByStringValue<Common.ClimberQf>(rdr["qf"].ToString(), Common.ClimberQf.Empty, DefaultUICulture),
                            RecordUniqueId = rdr[ServiceHelper.REMOTE_ID_COLUMN].ToString(),
                            Styles = styles,
                            Surname = rdr["surname"].ToString().Trim(),
                            TeamNames = new[] { rdr["team"].ToString() },
                            YearOfBirth = Convert.ToInt32(rdr["age"])
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        private void FinaliseParticipant(ApiParticipant participant)
        {
            if (participant == null || string.IsNullOrEmpty(participant.RecordUniqueId) || !participant.OldBib.HasValue)
                return;

            using(var cmd = this.CreateCommand())
            {
                cmd.CommandText = "UPDATE Participants SET " + ServiceHelper.REMOTE_ID_COLUMN + " = @remoteIid, changed = 0 WHERE iid = @iid";
                cmd.Parameters.Add("@remoteIid", SqlDbType.VarChar, ServiceHelper.REMOTE_ID_COL_SIZE).Value = participant.RecordUniqueId;
                cmd.Parameters.Add("@iid", SqlDbType.Int).Value = participant.OldBib;
                cmd.ExecuteNonQuery();
            }
        }

        private ICollection<string> ProcessParticipants(IEnumerable<int> iidList)
        {
            ServiceHelper.CheckRemoteIdColumn("Participants", this.cn, this.currentTransaction);
           
            var climersToPost = iidList.ToList()
                                       .Select(n => this.GetApiParticipant(n))
                                       .Where(p => p != null)
                                       .ToArray();
            if (climersToPost.Length < 1)
                return new string[0];

            List<ApiParticipant> block = new List<ApiParticipant>(10);
            List<ApiParticipant> queryResults = new List<ApiParticipant>();
            foreach(var p in climersToPost)
            {
                block.Add(p);
                if (block.Count >= 10)
                {
                    queryResults.AddRange(ServiceClient.Instance.PostParticipants(block, false));
                    block.Clear();
                }
            }
            if (block.Count > 0)
                queryResults.AddRange(ServiceClient.Instance.PostParticipants(block, false));

            foreach(var p in queryResults)
            {
                this.FinaliseParticipant(p);
            }

            return queryResults.Select(r => r.RecordUniqueId).ToList();
        }

        public void PostAllClimbers()
        {
            List<int> climbers = new List<int>();
            using(var cmd = this.CreateCommand())
            {
                cmd.CommandText = "select iid from Participants(nolock)";
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                        climbers.Add(Convert.ToInt32(rdr["iid"]));
                }
            }

            ServiceClient.Instance.ClearNotNeededClimbers(this.ProcessParticipants(climbers));
        }

        public void PostChangedClimbers()
        {
            List<int> climbers = new List<int>();
            using (var cmd = this.CreateCommand())
            {
                cmd.CommandText = "select iid from Participants(nolock) where changed = 1";
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                        climbers.Add(Convert.ToInt32(rdr["iid"]));
                }
            }

            this.ProcessParticipants(climbers);
        }
    }
}