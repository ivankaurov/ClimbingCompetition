using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using XmlApiClient;
using XmlApiData;
using ClimbingCompetition.dsClimbingTableAdapters;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace ClimbingCompetition
{
    public sealed class OnlineUpdater : IDisposable
    {
        public void Dispose()
        {
            this.cn.Dispose();
        }

        private XmlClient client;
        private SqlConnection cn;
        private OnlineUpdater(SqlConnection cn, XmlClient client)
        {
            this.cn = new SqlConnection(cn.ConnectionString);
            if (this.cn.State != ConnectionState.Open)
                this.cn.Open();
            this.client = client;
        }

        #region Templates

        #region Delegates
        private delegate void FinalizeUpdate<T, TOut>(List<T> oldData, List<TOut> newData, SqlTransaction tran);

        private delegate AsyncRequestResult BeginAsyncRequestDelegate<in TIn, out TOut>(TIn data, RequestCompleted<TOut> callback, object asyncState);

        public delegate void UpdateCompleted(RequestResult result, Exception ex, object asyncState);

        private delegate TOut KeySelector<in TIn, out TOut>(TIn source);

        private delegate AsyncRequestResult BeginAsyncRefresh(bool fullRefresh, BeginAsyncRefresh nextDelegates, UpdateCompleted clientCallback, object clientAsyncState);

        private delegate List<T> GetRefrehData<T>(bool fullRefresh, SqlTransaction tran);

        #endregion
        
        #region Elements

        private sealed class ParallelUploadData<T, TOut>
        {
            public T OldItem { get; set; }
            public TOut NewItem { get; set; }
            public RequestResult Result { get; set; }
            public Exception Ex { get; set; }
            public ParallelUploadData() { }
            public ParallelUploadData(T oldItem, TOut newItem)
            {
                this.OldItem = oldItem;
                this.NewItem = newItem;
            }
        }

        sealed class PostOneElementArgs<T, TOut>
            where T : XmlApiData.APIBaseRequest
        {
            public Semaphore Pool { get; set; }
            public Int32 Index { get; set; }
            public List<ParallelUploadData<T, TOut>> Data { get; set; }
            public T ExistingApi { get; set; }
        }

        sealed class ParallelSequenceUpdateCompleted<T, TOut>
        {
            public bool FullRefresh { get; set; }
            public UpdateCompleted ClientCallback { get; set; }
            public object ClientAsyncState { get; set; }
            public FinalizeUpdate<T, TOut> FinalizeUpdate { get; set; }
            public Semaphore Pool { get; set; }
            public List<ParallelUploadData<T, TOut>> Data { get; set; }
            public BeginAsyncRefresh DelegateChain { get; set; }
        }

        public class MultipleAsyncRequest : AsyncRequestResult
        {
            private List<AsyncRequestResult> baseList = new List<AsyncRequestResult>();
            public void Add(AsyncRequestResult item) { this.baseList.Add(item); }

            public MultipleAsyncRequest() : base(null) { }

            public override void Abort()
            {
                foreach (var asr in baseList)
                    asr.Abort();
            }
        }

        private AsyncRequestResult BeginAsyncCollectionParallelRequest<T, TOut>(bool fullRefresh, List<T> dataToSend, UpdateCompleted clientCallback, object clientAsyncState,
            BeginAsyncRequestDelegate<T, TOut> delegat, FinalizeUpdate<T, TOut> finalizeUpdation, BeginAsyncRefresh delegateChain)
            where T : XmlApiData.APIBaseRequest
        {
            List<ParallelUploadData<T,TOut>> newTupleArgs;
            Semaphore pool;
            MultipleAsyncRequest result = new MultipleAsyncRequest();
            if (dataToSend.Count > 0)
            {
                newTupleArgs = new List<ParallelUploadData<T, TOut>>(dataToSend.Count);
                foreach (var t in dataToSend)
                    newTupleArgs.Add(new ParallelUploadData<T, TOut>(t, default(TOut)));
                int nCnt = dataToSend.Count;
                pool = new Semaphore(0, nCnt);
                for (int i = 0; i < nCnt; i++)
                {
                    var t = newTupleArgs[i].OldItem;
                    PostOneElementArgs<T, TOut> argOne = new PostOneElementArgs<T,TOut>
                    {
                        Data = newTupleArgs,
                        ExistingApi = t,
                        Pool = pool,
                        Index = i
                    };
                    result.Add(delegat(t, PostOneElementCompleted<T, TOut>, argOne));
                }
            }
            else
            {
                newTupleArgs = null;
                pool = null;
            }
            ParallelSequenceUpdateCompleted<T, TOut> arg = new ParallelSequenceUpdateCompleted<T, TOut>
            {
                FullRefresh = fullRefresh,
                ClientAsyncState = clientAsyncState,
                ClientCallback = clientCallback,
                Data = newTupleArgs,
                FinalizeUpdate = finalizeUpdation,
                Pool = pool,
                DelegateChain = delegateChain
            };
            Thread thr = new Thread(PostElementListCompleted<T, TOut>);
            thr.Start(arg);
            return result;
        }


        private void PostOneElementCompleted<T, TOut>(TOut r, RequestResult completed, HttpStatusCode statusCode, Exception ex, object res)
            where T : XmlApiData.APIBaseRequest
        {

            var arg = (PostOneElementArgs<T, TOut>)res;
            try
            {
                lock (arg.Data)
                {
                    var d = arg.Data[arg.Index];
                    d.NewItem = r;
                    d.Result = completed;
                    d.Ex = ex;
                }
            }
            finally { arg.Pool.Release(); }
        }

        private void PostElementListCompleted<T, TOut>(object obj)
            where T : APIBaseRequest
        {
            var arg = (ParallelSequenceUpdateCompleted<T, TOut>)obj;

            int n;
            if (arg.Pool != null)
            {
                lock (arg.Data)
                {
                    n = arg.Data.Count;
                }
                try
                {
                    for (int i = 0; i < n; i++)
                        arg.Pool.WaitOne();
                }
                finally { arg.Pool.Release(n); }
                List<T> oldTeams = new List<T>();
                List<TOut> newTeams = new List<TOut>();
                foreach (var r in arg.Data)
                {
                    if (r.Result != RequestResult.Success)
                    {
                        if (arg.ClientCallback != null)
                            arg.ClientCallback(r.Result, r.Ex, arg.ClientAsyncState);
                        else if (r.Ex != null)
                            throw new Exception("See inner exception for details", r.Ex);
                        return;
                    }
                    oldTeams.Add(r.OldItem);
                    newTeams.Add(r.NewItem);
                }
                try { arg.FinalizeUpdate(oldTeams, newTeams, null); }
                catch (Exception ex)
                {
                    if (arg.ClientCallback == null)
                        throw;
                    else
                        arg.ClientCallback(RequestResult.Error, ex, arg.ClientAsyncState);
                    return;
                }
            }

            if (arg.DelegateChain != null)
            {
                var invocationList = arg.DelegateChain.GetInvocationList();
                BeginAsyncRefresh nextStage;
                if (invocationList.Length > 0)
                    nextStage = (BeginAsyncRefresh)invocationList[0];
                else
                    nextStage = arg.DelegateChain;
                nextStage(arg.FullRefresh, arg.DelegateChain, arg.ClientCallback, arg.ClientAsyncState);
            }
            else
            {
                if (arg.ClientCallback != null)
                    arg.ClientCallback(RequestResult.Success, null, arg.ClientAsyncState);
            }
        }
        
        #endregion

        #region Collection

        private sealed class AsyncPackage<Tdata, TOut>
           where Tdata : APIBaseRequest
        {
            public UpdateCompleted ClientCallback { get; set; }
            public object ClientAsyncState { get; set; }
            public List<Tdata> InitialList { get; set; }
            public FinalizeUpdate<Tdata, TOut> FinalizeUpdation { get; set; }
            public BeginAsyncRefresh DelegateChain { get; set; }
            public bool FullRefresh { get; set; }
        }

        private void PostCollectionCompleted<T, TOut>(TOut r, RequestResult completed, HttpStatusCode statusCode, Exception ex, object res)
            where T:APIBaseRequest
        {
            var args = (AsyncPackage<T, TOut>)res;
            if (completed != RequestResult.Success)
            {
                if (args.ClientCallback != null)
                    args.ClientCallback(completed, ex, args.ClientAsyncState);
                else if (ex != null)
                    throw new Exception("See inner exception for details", ex);
                return;
            }
            if (args.FinalizeUpdation != null)
                args.FinalizeUpdation(args.InitialList, null, null);
            InvokeNextRequest(args.DelegateChain, args.FullRefresh, args.ClientCallback, args.ClientAsyncState, completed, ex);
        }

        private void PostFullCollectionCompleted<T>(IAPICollection<T> r, RequestResult completed, HttpStatusCode statusCode, Exception ex, object res)
            where T : APIBaseRequest
        {
            var args = (AsyncPackage<T, T>)res;
            if (completed != RequestResult.Success)
            {
                if (args.ClientCallback != null)
                    args.ClientCallback(completed, ex, args.ClientAsyncState);
                else if (ex != null)
                    throw new Exception("See inner exception for details", ex);
                return;
            }
            var newTeams = new List<T>(r.Data);
            try
            {
                args.FinalizeUpdation(args.InitialList, newTeams, null);
            }
            catch (Exception e)
            {
                if (args.ClientCallback == null)
                    throw;
                else
                    args.ClientCallback(RequestResult.Error, e, args.ClientAsyncState);
                return;
            }
            InvokeNextRequest(args.DelegateChain, args.FullRefresh, args.ClientCallback, args.ClientAsyncState, completed, ex);
        }

        private static void InvokeNextRequest(BeginAsyncRefresh delegateChain, bool fullRefresh, UpdateCompleted clientCallback,
            object ClientAsyncState, RequestResult previousResult, Exception previousException)
        {
            if (delegateChain == null)
            {
                if (clientCallback != null)
                    clientCallback(previousResult, previousException, ClientAsyncState);
            }
            else
            {
                var invocationList = delegateChain.GetInvocationList();
                BeginAsyncRefresh nextStep;
                if (invocationList.Length > 0)
                    nextStep = (BeginAsyncRefresh)invocationList[0];
                else
                    nextStep = delegateChain;
                nextStep(fullRefresh, delegateChain, clientCallback, ClientAsyncState);
            }
        }
        #endregion

        #region ServiceFunctions

        private AsyncRequestResult BeginRefreshData<Tsimple, TCollection, TOut, TCollectionOut>(bool fullRefresh, BeginAsyncRefresh delegateChain, UpdateCompleted clientCallback, object clientAsyncState,
            FinalizeUpdate<Tsimple, TOut> finalizeFunction,
            BeginAsyncRequestDelegate<TCollection, TCollectionOut> packageRefreshFunction,
            XmlApiClient.RequestCompleted<TCollectionOut> packageCallbackFunction,
            KeySelector<IEnumerable<Tsimple>, TCollection> collectionConverter,
            BeginAsyncRequestDelegate<Tsimple, TOut> elementRefreshFunction,
            GetRefrehData<Tsimple> getRefreshData
            )
            where Tsimple : APIBaseRequest
            where TCollection : APIBaseRequest, IAPICollection<Tsimple>
        {
            var data = getRefreshData(fullRefresh, null);
            if (data == null || data.Count < 1)
            {
                InvokeNextRequest(delegateChain, fullRefresh, clientCallback, clientAsyncState, RequestResult.Success, null);
                return null;
            }
            if (fullRefresh)
            {
                AsyncPackage<Tsimple, TOut> args = new AsyncPackage<Tsimple, TOut>
                {
                    ClientAsyncState = clientAsyncState,
                    ClientCallback = clientCallback,
                    DelegateChain = delegateChain,
                    FinalizeUpdation = finalizeFunction,
                    FullRefresh = fullRefresh,
                    InitialList = data
                };
                return packageRefreshFunction(collectionConverter(data), packageCallbackFunction, args);
                //return packageRefreshFunction(collectionConverter(data), this.PostFullCollectionCompleted, args);
            }
            else
            {
                return this.BeginAsyncCollectionParallelRequest<Tsimple, TOut>(
                    fullRefresh,
                    data,
                    clientCallback,
                    clientAsyncState,
                    elementRefreshFunction,
                    finalizeFunction,
                    delegateChain);
            }
        }

        private void FinalizePostSelector<T>(List<T> oldValues, List<T> newValues, KeySelector<T, int> selector, String tableName, SqlTransaction tran)
        {
            List<int> oldL = new List<int>(oldValues.Count), newL = new List<int>(newValues.Count);
            foreach (var t in oldValues)
                oldL.Add(selector(t));
            foreach (var t in newValues)
                newL.Add(selector(t));
            FinalizePostIidLists(tableName, oldL, newL, tran);
        }

        private long changeTableIid(long oldIid, long newIid, String tableName, SqlTransaction tran = null)
        {
            if (oldIid == newIid)
                return 0;
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = tran };
            cmd.CommandText = String.Format("SELECT COUNT(*) FROM {0} (NOLOCK) WHERE iid=@iid", tableName);
            cmd.Parameters.Add("@iid", SqlDbType.Int).Value = newIid;
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
                return newIid;
        }

        private void chageIidsBack(Dictionary<int, int> iids, String tableName, SqlTransaction tran = null)
        {
            if (iids == null || iids.Count < 1)
                return;
            SqlCommand cmdCheck = new SqlCommand { Connection = cn, Transaction = tran };
            cmdCheck.CommandText = String.Format("SELECT COUNT(*) FROM {0} (NOLOCK) WHERE iid=@iid", tableName);
            cmdCheck.Parameters.Add("@iid", SqlDbType.Int);

            SqlCommand cmdUpdate = new SqlCommand { Connection = cn, Transaction = tran };
            cmdUpdate.CommandText = String.Format("UPDATE {0} SET iid=@iidNew WHERE iid=@iidOld", tableName);
            cmdUpdate.Parameters.Add("@iidNew", SqlDbType.Int);
            cmdUpdate.Parameters.Add("@iidOld", SqlDbType.Int);

            Random rand = new Random();
            foreach (var kvp in iids)
            {
                cmdCheck.Parameters["@iid"].Value = kvp.Value;
                int n = Convert.ToInt32(cmdCheck.ExecuteScalar());
                int temporaryiid;
                if (n > 0)
                {
                    do
                    {
                        temporaryiid = rand.Next(1, int.MaxValue);
                        cmdCheck.Parameters["@iid"].Value = temporaryiid;
                        n = Convert.ToInt32(cmdCheck.ExecuteScalar());
                    } while (n > 0);
                    cmdUpdate.Parameters["@iidOld"].Value = kvp.Value;
                    cmdUpdate.Parameters["@iidNew"].Value = temporaryiid;
                    cmdUpdate.ExecuteNonQuery();
                }
                else
                    temporaryiid = -1;
                cmdUpdate.Parameters["@iidOld"].Value = kvp.Key;
                cmdUpdate.Parameters["@iidNew"].Value = kvp.Value;
                cmdUpdate.ExecuteNonQuery();
                if (temporaryiid > 0)
                {
                    cmdUpdate.Parameters["@iidOld"].Value = temporaryiid;
                    cmdUpdate.Parameters["@iidNew"].Value = kvp.Key;
                    cmdUpdate.ExecuteNonQuery();
                }
            }
        }

        private void FinalizePostIidLists(String tableName, List<int> oldTeams, List<int> newTeams, SqlTransaction tran)
        {
            if (oldTeams.Count != newTeams.Count)
                throw new ArgumentException("Arrays\' dimensions differ");
            Dictionary<int, int> changedIids = new Dictionary<int, int>();
            SqlCommand cmd = new SqlCommand
            {
                Connection = cn,
                CommandText = "UPDATE " + tableName + " SET iid=@newIid, changed=0 WHERE iid=@oldIid",
                Transaction = tran ?? cn.BeginTransaction()
            };
            try
            {
                cmd.Parameters.Add("@newIid", SqlDbType.Int);
                cmd.Parameters.Add("@oldIid", SqlDbType.Int);
                for (int i = 0; i < oldTeams.Count; i++)
                {
                    var newTeam = newTeams[i];
                    var oldTeam = oldTeams[i];
                    int iidToSet = newTeam;
                    if (iidToSet != oldTeam)
                    {
                        iidToSet = (int)changeTableIid(oldTeam, newTeam, tableName, cmd.Transaction);
                        if (iidToSet != newTeam)
                            changedIids.Add(iidToSet, newTeam);
                    }
                    cmd.Parameters["@newIid"].Value = iidToSet;
                    cmd.Parameters["@oldIid"].Value = oldTeam;
                    cmd.ExecuteNonQuery();
                }
                chageIidsBack(changedIids, tableName, cmd.Transaction);
                if (tran == null)
                    cmd.Transaction.Commit();
            }
            catch
            {
                if (tran == null)
                    cmd.Transaction.Rollback();
                throw;
            }
        }
        #endregion

        #endregion

        #region Climbers
        
        private long[] GetTeamsForClimber(int climberId, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction=tran };
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

        private List<Comp_CompetitorRegistrationApiModel> ClimbersForRefresh(bool fullRefresh, SqlTransaction tran)
        {
            List<Comp_CompetitorRegistrationApiModel> existingClimbers = new List<Comp_CompetitorRegistrationApiModel>();
            climbersToUpdateTableAdapter cta = new climbersToUpdateTableAdapter() { Connection = cn, Transaction = tran };
            dsClimbing.climbersToUpdateDataTable table;
            if (fullRefresh)
                table = cta.GetData();
            else
                table = cta.GetDataByChanged(true);
            if (table.Rows.Count < 1)
                return existingClimbers;
            
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
                    Teams = GetTeamsForClimber(row.iid, tran)
                });
            return existingClimbers;
        }

        private void RefreshClimbers(bool fullRefresh, bool singleTransaction)
        {
            SqlTransaction tran = singleTransaction ? cn.BeginTransaction() : null;
            try
            {
                var teams = TeamsForRefresh(fullRefresh, tran);
                List<RegionApiModel> newTeams;
                if (fullRefresh)
                    newTeams = new List<RegionApiModel>(client.PostRegionCollection(new API_RegionCollection(teams)).Data);
                else
                {
                    newTeams = new List<RegionApiModel>();
                    foreach (var t in teams)
                        newTeams.Add(client.PostRegion(t));
                }
                FinalizePostTeam(teams, newTeams, tran);

                List<Comp_AgeGroupApiModel> newGroups;
                var groups = GroupsForRefresh(fullRefresh, tran);
                if (fullRefresh)
                    newGroups = new List<Comp_AgeGroupApiModel>(client.PostGroupCollection(new API_AgeGroupCollection(groups)).Data);
                else
                {
                    newGroups = new List<Comp_AgeGroupApiModel>(groups.Count);
                    foreach (var t in groups)
                        newGroups.Add(client.PostGroup(t));
                }
                FinalizePostGroup(groups, newGroups, tran);

                List<Comp_CompetitorRegistrationApiModel> newClimbers;
                var climbers = ClimbersForRefresh(fullRefresh, tran);
                if (fullRefresh)
                    newClimbers = new List<Comp_CompetitorRegistrationApiModel>(client.PostClimberCollection(new API_ClimbersCollection(climbers)).Data);
                else
                {
                    newClimbers = new List<Comp_CompetitorRegistrationApiModel>();
                    foreach (var c in climbers)
                        newClimbers.Add(client.PostClimber(c));
                }
                FinalizePostClimbers(climbers, newClimbers, tran);
                if (tran != null)
                    tran.Commit();
            }
            catch
            {
                if (tran != null)
                    tran.Rollback();
                throw;
            }
        }

        private AsyncRequestResult BeginRefreshClimbers(bool fullRefresh, BeginAsyncRefresh delegateChain, UpdateCompleted clientCallback, object clientAsyncState)
        {
            if (delegateChain != null)
                delegateChain -= BeginRefreshClimbers;
            return BeginRefreshData<Comp_CompetitorRegistrationApiModel, API_ClimbersCollection, Comp_CompetitorRegistrationApiModel, API_ClimbersCollection>(
                fullRefresh,
                delegateChain,
                clientCallback,
                clientAsyncState,
                FinalizePostClimbers,
                client.BeginPostClimberCollection,
                this.PostFullCollectionCompleted,
                (a =>
                {
                    List<Comp_CompetitorRegistrationApiModel> r = new List<Comp_CompetitorRegistrationApiModel>();
                    foreach (var t in a)
                        r.Add(t);
                    return new API_ClimbersCollection(r);
                }),
                client.BeginPostClimber,
                ClimbersForRefresh);
        }

        private void FinalizePostClimbers(List<Comp_CompetitorRegistrationApiModel> oldClimbers, List<Comp_CompetitorRegistrationApiModel> newClimbers, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = (tran ?? cn.BeginTransaction()) };
            try
            {
                cmd.CommandText = "UPDATE Participants SET license = @lic, changed=0 WHERE iid = @iid";
                cmd.Parameters.Add("@lic", SqlDbType.BigInt);
                cmd.Parameters.Add("@iid", SqlDbType.Int);
                foreach (var oldClm in oldClimbers)
                {
                    if (oldClm.Bib == null)
                        continue;
                    Comp_CompetitorRegistrationApiModel cModel = null;
                    foreach (var m in newClimbers)
                        if (m.Bib == oldClm.Bib)
                        {
                            cModel = m;
                            break;
                        }
                    if (cModel == null)
                        continue;
                    newClimbers.Remove(cModel);

                    cmd.Parameters[0].Value = cModel.License;
                    cmd.Parameters[1].Value = oldClm.Bib;
                    cmd.ExecuteNonQuery();

                }
                if (tran == null)
                    cmd.Transaction.Commit();
            }
            catch
            {
                if (tran == null)
                    cmd.Transaction.Rollback();
                throw;
            }
        }

        #endregion

        #region Teams

        private List<RegionApiModel> TeamsForRefresh(bool fullRefresh, SqlTransaction tran)
        {
            List<RegionApiModel> result = new List<RegionApiModel>();
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = tran };
            cmd.CommandText = "SELECT iid, name FROM Teams(NOLOCK)";
            if (!fullRefresh)
                cmd.CommandText += " WHERE changed=1";
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                    result.Add(new RegionApiModel { Iid = Convert.ToInt32(rdr["iid"]), Name = rdr["name"].ToString() });
            }
            return result;
        }

        private void FinalizePostTeam(List<RegionApiModel> oldTeams, List<RegionApiModel> newTeams, SqlTransaction tran)
        {
            FinalizePostSelector(oldTeams, newTeams, (a => (int)a.Iid), "Teams", tran);
        }

        private AsyncRequestResult BeginRefreshTeams(bool fullRefresh, BeginAsyncRefresh delegateChain, UpdateCompleted clientCallback, object clientAsyncState)
        {
            if (delegateChain != null)
                delegateChain -= BeginRefreshTeams;

            return BeginRefreshData<RegionApiModel, API_RegionCollection, RegionApiModel, API_RegionCollection>(
                fullRefresh,
                delegateChain,
                clientCallback,
                clientAsyncState,
                FinalizePostTeam,
                client.BeginPostRegionCollection,
                this.PostFullCollectionCompleted,
                (a => new API_RegionCollection(a)),
                client.BeginPostRegion,
                TeamsForRefresh);
        }

        #endregion

        #region AgeGroups
        private List<Comp_AgeGroupApiModel> GroupsForRefresh(bool fullRefresh, SqlTransaction tran)
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
            return grpList;
        }

        class GroupData
        {
            public int OldIid { get; set; }
            public int NewIid { get; set; }
            public int TempIid { get; set; }
            public String OldName { get; set; }
            public String TempName { get; set; }
        }

        private void FinalizePostGroup(List<Comp_AgeGroupApiModel> oldGroups, List<Comp_AgeGroupApiModel> newGroups, SqlTransaction tran)
        {
            int nCnt;
            if ((nCnt = oldGroups.Count) != newGroups.Count)
                throw new ArgumentException("Array\'s dimensions differ", "newGroups");

            Dictionary<int, GroupData> temporaryIids = new Dictionary<int, GroupData>();
            SqlCommand cmd = new SqlCommand
            {
                Connection = cn,
                Transaction = tran ?? cn.BeginTransaction(),
                CommandText = "UPDATE groups SET changed=0 WHERE iid=@iid"
            };
            try
            {
                var iidPar = cmd.Parameters.Add("@iid", SqlDbType.Int);
                int maxNewIid = 0;
                for (int i = 0; i < nCnt; i++)
                    if (newGroups[i].Iid > maxNewIid)
                        maxNewIid = newGroups[i].Iid;
                for (int i = 0; i < nCnt; i++)
                    if (oldGroups[i].Iid != newGroups[i].Iid)
                    {
                        int tempIid = getTemporaryIid("groups", "iid", maxNewIid, cmd.Transaction);
                        GroupData gd;
                        temporaryIids.Add(tempIid, (gd = new GroupData
                        {
                            OldIid = oldGroups[i].Iid,
                            TempIid = tempIid,
                            NewIid = newGroups[i].Iid,
                            OldName = oldGroups[i].Name,
                            TempName = String.Format("GROUP_{0}", tempIid)
                        }));
                        updateGroupId(oldGroups[i].Iid, tempIid, gd.TempName, 3000, cmd.Transaction);
                    }
                foreach (var kvp in temporaryIids)
                    updateGroupId(kvp.Key, kvp.Value.NewIid, kvp.Value.OldName, -3000, cmd.Transaction);
                for (int i = 0; i < nCnt; i++)
                {
                    iidPar.Value = newGroups[i].Iid;
                    cmd.ExecuteNonQuery();
                }
                if (tran == null)
                    cmd.Transaction.Commit();
            }
            catch
            {
                if (tran == null)
                    cmd.Transaction.Rollback();
                throw;
            }

        }

        private int getTemporaryIid(String tableName, String iidColumn, int minPossible = 0, SqlTransaction tran = null)
        {
            SqlCommand cmd = new SqlCommand
            {
                Connection = cn,
                Transaction = tran,
                CommandText = String.Format("SELECT ISNULL(MAX({1}),0) + 1" +
                                          "  FROM {0} (NOLOCK)", tableName, iidColumn)
            };
            int value = Convert.ToInt32(cmd.ExecuteScalar());
            if (value <= minPossible)
                value = minPossible + 1;
            return value;
        }

        private void updateGroupId(int oldIid, int newIid, String newName, int modify, SqlTransaction _tran)
        {
            if (oldIid == newIid)
                return;
            SqlCommand cmd = new SqlCommand
            {
                Connection = cn,
                Transaction = _tran ?? cn.BeginTransaction(),
                CommandText = String.Format("INSERT INTO Groups (iid, name, oldYear, youngYear, minQf, genderFemale, changed)" +
                              "SELECT {1} iid, @name, oldYear+({2}), youngYear+({2}), minQf, genderFemale, changed" +
                              "  FROM Groups(nolock)" +
                              " WHERE iid={0}", oldIid, newIid, modify)
            };
            try
            {
                cmd.Parameters.Add("@name", SqlDbType.VarChar, 255).Value = newName;
                cmd.ExecuteNonQuery();
                cmd.CommandText = String.Format("UPDATE lists SET group_id={1} WHERE group_id={0}", oldIid, newIid);
                cmd.ExecuteNonQuery();
                cmd.CommandText = String.Format("UPDATE Participants SET group_id={1} WHERE group_id={0}", oldIid, newIid);
                cmd.ExecuteNonQuery();
                cmd.CommandText = String.Format("DELETE FROM Groups WHERE iid={0}", oldIid);
                cmd.ExecuteNonQuery();
                if (_tran == null)
                    cmd.Transaction.Commit();
            }
            catch
            {
                if (_tran == null)
                    cmd.Transaction.Rollback();
                throw;
            }
        }

        private AsyncRequestResult BeginRefreshGroups(bool fullRefresh, BeginAsyncRefresh delegateChain, UpdateCompleted clientCallback, object clientAsyncState)
        {
            if (delegateChain != null)
                delegateChain -= BeginRefreshGroups;

            return BeginRefreshData<Comp_AgeGroupApiModel, API_AgeGroupCollection, Comp_AgeGroupApiModel, API_AgeGroupCollection>(
                fullRefresh, delegateChain, clientCallback, clientAsyncState, FinalizePostGroup,
                client.BeginPostGroupCollection,this.PostFullCollectionCompleted,
                (a => new API_AgeGroupCollection(a)),
                client.BeginPostGroup, GroupsForRefresh);
        }

        #endregion

        #region ListHeaders

        private static int? GetNullableInt(object value){
            if(value==null || value==DBNull.Value)
                return null;
            return Convert.ToInt32(value);
        }
        private static int GetIntValue(object value, int defaultValue = 0){
            var nVal = GetNullableInt(value);
            return nVal??defaultValue;
        }
        private List<ApiListHeader> ListHeadersForRefresh(int listID, SqlTransaction tran)
        {
            return ListHeadersForRefresh(false, tran, listID);
        }
        private List<ApiListHeader> ListHeadersForRefresh(bool fullRefresh, SqlTransaction tran)
        {
            return ListHeadersForRefresh(fullRefresh, tran, 0);
        }
        private List<ApiListHeader> ListHeadersForRefresh(bool fullRefresh, SqlTransaction tran, int listID)
        {
            List<ApiListHeader> res = new List<ApiListHeader>();
            SqlCommand cmd = new SqlCommand
            {
                Connection = cn,
                Transaction = tran
            };
            cmd.CommandText = "SELECT iid, group_id, listType, allowView, iid_parent, prev_round, quote, round," +
                              "       routeNumber, start, style" +
                              "  FROM lists(nolock)" +
                              " WHERE style IN('Трудность','Скорость','Боулдеринг')";
            if (listID > 0)
                cmd.CommandText += String.Format(" AND iid={0}", listID);
            else
            {
                cmd.CommandText += " AND online=1 ";
                if (!fullRefresh)
                    cmd.CommandText += " AND changed=1";
            }
            SpeedRules spRules = SettingsForm.GetSpeedRules(cn, tran);
            CompetitionRules rules = ((spRules & SpeedRules.InternationalRules) == SpeedRules.InternationalRules) ? CompetitionRules.International : CompetitionRules.Russian;
            bool bestQf = ((spRules & SpeedRules.BestResultFromTwoQfRounds) == SpeedRules.BestResultFromTwoQfRounds);
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    var header = new ApiListHeader
                    {
                        GroupId = rdr["group_id"] == DBNull.Value ? null : new int?(Convert.ToInt32(rdr["group_id"])),
                        Iid = Convert.ToInt32(rdr["iid"]),
                        LastRefresh = DateTime.UtcNow,
                        ListType = (ListTypeEnum)Enum.Parse(typeof(ListTypeEnum), rdr["listType"].ToString(), true),
                        Live = Convert.ToBoolean(rdr["allowView"]),
                        ParentList = GetNullableInt(rdr["iid_parent"]),
                        PreviousRound = GetNullableInt(rdr["prev_round"]),
                        Quota = GetIntValue(rdr["quote"]),
                        Round = rdr["round"].ToString(),
                        RouteQuantity = GetNullableInt(rdr["routeNumber"]),
                        StartTime = rdr["start"] == DBNull.Value ? String.Empty : rdr["start"].ToString(),
                        Style = rdr["style"].ToString(),
                        Rules = rules, 
                        BestQf = bestQf
                    };
                    res.Add(header);
                }
            }
            return res;
        }

        private void FinalizePostListHeaders(List<ApiListHeader> oldData, List<object> notUsed, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand
            {
                Connection = cn,
                Transaction = tran ?? cn.BeginTransaction()
            };
            try
            {
                cmd.CommandText = "UPDATE lists SET changed=0 WHERE iid=@iid";
                cmd.Parameters.Add("@iid", SqlDbType.Int);
                foreach (var l in oldData)
                {
                    cmd.Parameters[0].Value = l.Iid;
                    cmd.ExecuteNonQuery();
                }
                if (tran == null)
                    cmd.Transaction.Commit();
            }
            catch
            {
                if (tran == null)
                    cmd.Transaction.Rollback();
                throw;
            }
        }

        private AsyncRequestResult BeginPostLists(bool fullRefresh, BeginAsyncRefresh delegateChain, UpdateCompleted clientCallback, object clientAsyncState)
        {
            if (delegateChain != null)
                delegateChain -= BeginPostLists;
            return BeginRefreshData<ApiListHeader, ApiListHeaderCollection, object, object>(
                fullRefresh,
                delegateChain,
                clientCallback,
                clientAsyncState,
                FinalizePostListHeaders,
                client.BeginPostListHeaderCollection,
                this.PostCollectionCompleted<XmlApiData.ApiListHeader, object>,
                (a => new ApiListHeaderCollection(a)),
                client.BeginPostListHeader,
                ListHeadersForRefresh);
        }
        
        #endregion

        #region ListLines

        private AsyncRequestResult BeginPostListLines(bool fullRefresh, BeginAsyncRefresh delegateChain, UpdateCompleted clientCallback, object clientAsyncState)
        {
            if (delegateChain != null)
                delegateChain -= BeginPostListLines;
            var data = ListLinesForRefresh(fullRefresh, null);
            if (data == null || data.Count < 1)
            {
                InvokeNextRequest(delegateChain, fullRefresh, clientCallback, clientAsyncState, RequestResult.Success, null);
                return null;
            }
            AsyncPackage<ApiListLine, object> args = new AsyncPackage<ApiListLine, object>
            {
                ClientAsyncState = clientAsyncState,
                ClientCallback = clientCallback,
                DelegateChain = delegateChain,
                FinalizeUpdation = FinalizePostResultCollection,
                FullRefresh = fullRefresh,
                InitialList = data
            };
            if (fullRefresh)
                return client.BeginReloadResultList(new ApiListLineCollection(data), this.PostCollectionCompleted<ApiListLine, object>, args);
            else
                return client.BeginLoadResultsPackage(new ApiListLineCollection(data), this.PostCollectionCompleted<ApiListLine, object>, args);
        }

        private List<ApiListLine> ListLinesForRefresh(int listID, SqlTransaction tran)
        {
            return ListLinesForRefresh(listID, true, tran);
        }

        private List<ApiListLine> ListLinesForRefresh(bool fullRefresh, SqlTransaction tran)
        {
            return ListLinesForRefresh(0, fullRefresh, tran);
        }

        private List<ApiListLine> ListLinesForRefresh(int listID, bool fullRefrersh, SqlTransaction tran)
        {
            List<ApiListLine> result = new List<ApiListLine>();
            result.AddRange(GetSpeedRefreshData(listID, fullRefrersh, tran));
            result.AddRange(GetLeadRefreshData(listID, fullRefrersh, tran));
            result.AddRange(GetBoulderRefreshData(listID, fullRefrersh, tran));
            return result;
        }

        private ApiListLineSpeed[] GetSpeedRefreshData(int listId, bool fullReload, SqlTransaction tran = null)
        {
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = tran };
            StringBuilder sb = new StringBuilder(
                "SELECT r.iid, r.climber_id, r.start, r.route1, r.route1_text, r.route2, r.route2_text, r.res, r.resText, r.list_id, ISNULL(r.preQf,0) preQf, " +
                "       r.pos, r.posText, r.qf " +
                "  FROM speedResults R(NOLOCK)" + //CreateWHEREclause(listId, fullReload);
                "  JOIN lists L(nolock) on L.iid = R.list_id" +
                " WHERE ");
            if (listId > 0)
                sb.AppendFormat("L.iid={0} ", listId);
            else
                sb.Append("L.online=1 ");
            if (!fullReload)
                sb.AppendFormat("AND (   L.changed=1" +
                                "     OR (L.listType='{0}' AND EXISTS(SELECT 1" +
                                "                                       FROM speedResults SRC(NOLOCK)" +
                                "                                      WHERE SRC.list_id = L.iid" +
                                "                                        AND SRC.changed=1)" +
                                "         OR R.changed=1" +
                                "        )" +
                                "    )", ListTypeEnum.SpeedFinal);
            cmd.CommandText = sb.ToString();
            
            List<ApiListLineSpeed> result = new List<ApiListLineSpeed>();
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    result.Add(new ApiListLineSpeed
                    {
                        ClimberID = Convert.ToInt32(rdr["climber_id"]),
                        ListID = Convert.ToInt32(rdr["list_id"]),
                        ResText = rdr["resText"] == DBNull.Value ? null : rdr["resText"].ToString(),
                        Result = rdr["res"] == DBNull.Value ? null : new long?(Convert.ToInt64(rdr["res"])),
                        Route1Data = rdr["route1"] == DBNull.Value ? null : new long?(Convert.ToInt64(rdr["route1"])),
                        Route1Text = rdr["route1_text"] == DBNull.Value ? null : rdr["route1_text"].ToString(),
                        Route2Data = rdr["route2"] == DBNull.Value ? null : new long?(Convert.ToInt64(rdr["route2"])),
                        Route2Text = rdr["route2_text"] == DBNull.Value ? null : rdr["route2_text"].ToString(),
                        PreQf = Convert.ToBoolean(rdr["preQf"]),
                        StartNumber = Convert.ToInt32(rdr["start"]),
                        ResultID = Convert.ToInt32(rdr["iid"]),
                        Pos = (rdr["pos"] == DBNull.Value) ? null : new int?(Convert.ToInt32(rdr["pos"])),
                        PosText = rdr["posText"] == DBNull.Value ? String.Empty : rdr["posText"].ToString(),
                        Qf = rdr["qf"] == DBNull.Value ? String.Empty : rdr["qf"].ToString().Trim()
                    });
                }
            }
            return result.ToArray();
        }

        private static String CreateWHEREclause(int listId, bool fullReload)
        {
            StringBuilder sb = new StringBuilder(" JOIN lists L(nolock) on L.iid = R.list_id ");
            if (listId > 0)
                sb.AppendFormat("WHERE L.iid={0} ", listId);
            else
            {
                sb.Append("WHERE L.online=1 ");
                if (!fullReload)
                    sb.Append("AND (R.changed=1 OR L.changed=1) ");
            }
            String sResult = sb.ToString();
            return sResult;
        }

        private ApiListLineLead[] GetLeadRefreshData(int listId, bool fullReload, SqlTransaction tran = null)
        {
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = tran };
            cmd.CommandText = "SELECT r.iid, r.climber_id, r.start,r.res, r.resText, r.timeText, r.timeValue, r.list_id, ISNULL(r.preQf,0) preQf" +
                "  FROM routeResults R(NOLOCK)" + CreateWHEREclause(listId, fullReload);
            List<ApiListLineLead> result = new List<ApiListLineLead>();
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    result.Add(new ApiListLineLead
                    {
                        ClimberID = Convert.ToInt32(rdr["climber_id"]),
                        ListID = Convert.ToInt32(rdr["list_id"]),
                        ResText = rdr["resText"] == DBNull.Value ? null : rdr["resText"].ToString(),
                        Result = rdr["res"] == DBNull.Value ? null : new long?(Convert.ToInt64(rdr["res"])),
                        StartNumber = Convert.ToInt32(rdr["start"]),
                        Time = rdr["timeValue"] == DBNull.Value ? null : new int?(Convert.ToInt32(rdr["timeValue"])),
                        TimeText = rdr["timeText"] == DBNull.Value ? String.Empty : rdr["timeText"].ToString(),
                        PreQf = Convert.ToBoolean(rdr["preQf"]),
                        ResultID = Convert.ToInt32(rdr["iid"])
                    });
                }
            }
            return result.ToArray();
        }

        private ApiListLineBoulder[] GetBoulderRefreshData(int listId, bool fullReload, SqlTransaction tran = null)
        {
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = tran };
            cmd.CommandText =
                "SELECT r.iid, r.list_id, r.climber_id, r.start,ISNULL(nya,0) nya, ISNULL(disq, 0) disq, ISNULL(r.preQf, 0) preQf" +
                "  FROM boulderResults R(NOLOCK)" + CreateWHEREclause(listId, fullReload);
            List<ApiListLineBoulder> result = new List<ApiListLineBoulder>();
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    ResultLabel lbl;
                    if (Convert.ToInt32(rdr["nya"]) > 0)
                        lbl = ResultLabel.DNS;
                    else if (Convert.ToInt32(rdr["disq"]) > 0)
                        lbl = ResultLabel.DSQ;
                    else
                        lbl = ResultLabel.RES;
                    result.Add(new ApiListLineBoulder
                    {
                        ClimberID = Convert.ToInt32(rdr["climber_id"]),
                        ListID = Convert.ToInt32(rdr["list_id"]),
                        ResultCode = lbl,
                        StartNumber = Convert.ToInt32(rdr["start"]),
                        ResultID = Convert.ToInt32(rdr["iid"]),
                        PreQf = Convert.ToBoolean(rdr["preQf"])
                    });
                }
            }
            if (result.Count > 0)
            {
                cmd.CommandText = "SELECT R.routeN, R.topA, R.bonusA" +
                                  "  FROM boulderRoutes R(nolock)" +
                                  " WHERE R.iid_parent = @iid";
                if (!fullReload)
                    cmd.CommandText += " AND R.changed=1";
                cmd.Parameters.Add("@iid", SqlDbType.Int);
                foreach (var res in result)
                {
                    List<ApiBoulderResultRoute> routes = new List<ApiBoulderResultRoute>();
                    cmd.Parameters[0].Value = res.ResultID;
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

        private void FinalizePostResultCollection(List<ApiListLine> data, List<object> notUsed, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = tran ?? cn.BeginTransaction() };
            try
            {
                String sTable;
                foreach (var l in data)
                {
                    if (l is ApiListLineBoulder)
                    {
                        sTable = "boulderResults";
                        cmd.CommandText = String.Format("UPDATE boulderRoutes SET changed=0 WHERE iid_parent={0}", l.ResultID);
                        cmd.ExecuteNonQuery();
                    }
                    else if (l is ApiListLineLead)
                        sTable = "routeResults";
                    else if (l is ApiListLineSpeed)
                        sTable = "speedResults";
                    else
                        throw new InvalidCastException("Invalid result type");
                    cmd.CommandText = String.Format("UPDATE {0} SET changed=0 WHERE iid={1}",
                        sTable, l.ResultID);
                    cmd.ExecuteNonQuery();
                }

                if (tran == null)
                    cmd.Transaction.Commit();
            }
            catch
            {
                if (tran == null)
                    cmd.Transaction.Rollback();
                throw;
            }
        }

        #endregion

        #region public

        private sealed class baseWrapper
        {
            public int RequestId { get; private set; }
            public UpdateCompleted ClientCallback { get; set; }
            public object ClientAsyncState { get; set; }
            public OnlineUpdater Updater { get; set; }
            public AsyncRequestResult ReqRes { get; set; }
            public DateTime StartTime { get; private set; }
            public String StartTimeString { get { return StartTime.ToString("T"); } }
            public baseWrapper()
            {
                int n;
                lock (locker)
                {
                    do
                    {
                        n = random.Next();
                    } while (startDict.ContainsKey(n));
                    this.RequestId = n;
                    startDict.Add(n, this);
                }
                this.StartTime = DateTime.Now;
            }
        }

        private static Random random = new Random();
        private static Dictionary<int, baseWrapper> startDict = new Dictionary<int, baseWrapper>();
        private static object locker = new object();

        private static void ClientRequestCompleted(RequestResult result, Exception ex, object asyncState)
        {
            baseWrapper args = (baseWrapper)asyncState;
            lock (locker)
            {
                startDict.Remove(args.RequestId);
            }
            if (args.ClientCallback != null)
                args.ClientCallback(result, ex, args.ClientAsyncState);
            else if (result != RequestResult.Success && ex != null)
                throw new Exception("See inner exception for details", ex);
        }

        public static void ReloadOneList(int listID, SqlConnection cn, XmlClient client, SqlTransaction tran = null)
        {
            OnlineUpdater updater = new OnlineUpdater(cn, client);
            var lst = updater.ListHeadersForRefresh(listID, null);
            var cmd = new SqlCommand { Connection = cn, Transaction = tran, CommandText = "UPDATE lists SET online=1, changed=0 WHERE iid=@iid" };
            cmd.Parameters.Add("@iid", SqlDbType.Int);
            foreach (var l in lst)
            {
                client.PostListHeader(l);
                var lines = new ApiListLineCollection(updater.ListLinesForRefresh(l.Iid, null));
                client.LoadResultsPackage(lines);
                cmd.Parameters[0].Value = l.Iid;
                cmd.ExecuteNonQuery();
            }
        }

        public static void RefreshLists(bool fullRefresh, SqlConnection cn, XmlClient client)
        {
            OnlineUpdater updater = new OnlineUpdater(cn, client);
            var data = updater.ListHeadersForRefresh(fullRefresh, null);
            foreach (var l in data)
                client.PostListHeader(l);
            updater.FinalizePostListHeaders(data, null, null);
            var lines = updater.ListLinesForRefresh(fullRefresh, null);
            if (fullRefresh)
                client.ReloadResultList(new ApiListLineCollection(lines));
            else
                client.LoadResultsPackage(new ApiListLineCollection(lines));
            updater.FinalizePostResultCollection(lines, null, null);
        }

        public static AsyncRequestResult BeginRefreshLists(bool fullRefresh, SqlConnection cn, XmlClient client, UpdateCompleted clientCallback, object clientAsyncState)
        {
            OnlineUpdater updater = new OnlineUpdater(cn, client);
            baseWrapper args = new baseWrapper
            {
                ClientAsyncState = clientAsyncState,
                ClientCallback = clientCallback,
                Updater=updater
            };
            BeginAsyncRefresh delegateChain = updater.BeginPostLists;
            delegateChain += updater.BeginPostListLines;
            var res= updater.BeginPostLists(fullRefresh, delegateChain, ClientRequestCompleted, args);
            args.ReqRes = res;
            return res;
        }

        public static void RefreshClimbers(bool fullRefresh, SqlConnection cn, XmlClient client, bool singleTransaction)
        {
            OnlineUpdater upd = new OnlineUpdater(cn, client);
            upd.RefreshClimbers(fullRefresh, singleTransaction);
        }

        public static AsyncRequestResult BeginRefreshClimbers(bool fullRefresh, SqlConnection cn, XmlClient client, UpdateCompleted callback, object asyncState)
        {
            OnlineUpdater updater = new OnlineUpdater(cn, client);
            baseWrapper args = new baseWrapper
            {
                ClientAsyncState = asyncState,
                ClientCallback = callback,
                Updater = updater
            };

            BeginAsyncRefresh delegateChain = updater.BeginRefreshTeams;
            delegateChain += updater.BeginRefreshGroups;
            delegateChain += updater.BeginRefreshClimbers;
            var res = updater.BeginRefreshTeams(fullRefresh, delegateChain, ClientRequestCompleted, args);
            args.ReqRes = res;
            return res;
        }

        public enum UpdateStartMode { StartAlways, AskForWait, CancelIfUpdating }

        public static AsyncRequestResult BeginFullUpdate(bool fullRefresh, SqlConnection cn, XmlClient client, UpdateCompleted callback, object asyncState, UpdateStartMode startMode)
        {
            if (startMode == UpdateStartMode.AskForWait || startMode == UpdateStartMode.CancelIfUpdating)
            {
                bool wait;
                do
                {
                    lock (locker)
                    {
                        if (startDict.Count > 0)
                        {
                            if (startMode == UpdateStartMode.CancelIfUpdating)
                                return null;
                            var dgRes = MessageBox.Show("Идёт загрузка данных на сайт. Подождать 5 секунд до завершения операции",
                                String.Empty, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                            switch (dgRes)
                            {
                                case DialogResult.Yes:
                                    wait = true;
                                    break;
                                case DialogResult.No:
                                    wait = false;
                                    break;
                                default:
                                    return null;
                            }
                        }
                        else
                            wait = false;
                    }
                } while (wait);
            }
            OnlineUpdater updater = new OnlineUpdater(cn, client);
            baseWrapper args = new baseWrapper
            {
                ClientAsyncState = asyncState,
                ClientCallback = callback,
                Updater = updater
            };
            BeginAsyncRefresh delegateChain = updater.BeginRefreshTeams;
            delegateChain += updater.BeginRefreshGroups;
            delegateChain += updater.BeginRefreshClimbers;
            delegateChain += updater.BeginPostLists;
            delegateChain += updater.BeginPostListLines;
            var res = updater.BeginRefreshTeams(fullRefresh, delegateChain, ClientRequestCompleted, args);
            args.ReqRes = res;
            return res;
        }

        #endregion
    }

}
