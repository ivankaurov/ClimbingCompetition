using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Configuration;
using ClimbingCompetition.Online;

namespace WebClimbing.src
{
    public enum InsertingResult { SUCCESS, ERROR, QUOTA_EXCEED, TO_QUEUE, DUPLICATE }
    public sealed class ClimbingConfirmations
    {
        public const string RAZR_NOTE = "Разряд участника не соответствует Положению";
        private readonly SqlConnection cn;
        private readonly Entities dc;
        private static Random rnd = new Random();
        private bool closeInDestr;

        public ClimbingConfirmations()
        {
            this.cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["db"].ConnectionString);
            if (cn.State != ConnectionState.Open)
                cn.Open();

            this.dc = new Entities(WebConfigurationManager.ConnectionStrings["db_Entities"].ConnectionString);
            if (this.dc.Connection.State != ConnectionState.Open)
                this.dc.Connection.Open();

            closeInDestr = true;
        }

        ~ClimbingConfirmations()
        {
            if (closeInDestr)
            {
                try { cn.Close(); }
                catch { }
                try { this.dc.Connection.Close(); }
                catch { }
            }
        }

        public InsertingResult ConfirmClimber(long uncClmIid, out bool isDel,out bool isEdit, out bool qfErr, out string message)
        {
            message = String.Empty;
            isDel = false;
            qfErr = false;
            isEdit = false;
            if (cn.State != ConnectionState.Open)
                cn.Open();
            ONLClimberCompLink clm;
            try { clm = dc.ONLClimberCompLinks.First(lop => lop.iid == uncClmIid); }
            catch { return InsertingResult.ERROR; }
            
            clm.ONLclimber.surname = clm.ONLclimber.surname.Trim();
            while (clm.ONLclimber.surname.IndexOf("  ") > -1)
                clm.ONLclimber.surname = clm.ONLclimber.surname.Replace("  ", " ");

            clm.ONLclimber.name = clm.ONLclimber.name.Trim();
            while (clm.ONLclimber.name.IndexOf("  ") > -1)
                clm.ONLclimber.name = clm.ONLclimber.name.Replace("  ", " ");

            try
            {
                if (clm.state == Constants.CLIMBER_PENDING_DELETE)
                {
                    isDel = true;
                    return DeleteClimber(clm);
                }
                else if (clm.ONLClimberCompLink1.Count < 1)
                    return InsertClimber(clm, out qfErr, out message);
                else
                {
                    isEdit = true;
                    return UpdateClimber(clm, out qfErr, out message);
                }
            }
            finally { DeleteDeadClimbers(dc); }
        }

        private InsertingResult DeleteClimber(ONLClimberCompLink unc)
        {
            foreach (var v in unc.ONLClimberCompLink1.ToArray())
                dc.ONLClimberCompLinks.DeleteObject(v);
            dc.SaveChanges();
            dc.ONLClimberCompLinks.DeleteObject(unc);
            dc.SaveChanges();

            return InsertingResult.SUCCESS;
        }
        private InsertingResult UpdateClimber(ONLClimberCompLink unc, out bool qfErr, out string message)
        {
            message = String.Empty;
            qfErr = false;
            int nCount = (from l in dc.ONLClimberCompLinks
                          where l.comp_id == unc.comp_id
                          && l.climber_id == unc.climber_id
                          && l.state == Constants.CLIMBER_CONFIRMED
                          && l.team_id == unc.team_id
                          && l.replacementID != unc.iid
                          select l).Count();
            if (nCount > 0)
                return InsertingResult.DUPLICATE;

            int quota;
            try { quota = unc.ONLteam.ONLTeamGroup.ONLTeamGroupCompLinks.First(l => l.comp_id == unc.comp_id).quota_style_total; }
            catch { quota = 0; }
            if (quota > 0)
            {
                var otherClimbers = unc.ONLCompetition.ONLClimberCompLinks.Where(lnk =>
                       lnk.team_id == unc.team_id
                       && lnk.group_id == unc.group_id
                       && lnk.state == Constants.CLIMBER_CONFIRMED
                       && lnk.climber_id != unc.climber_id).ToArray();
                int newCnt;
                newCnt = otherClimbers.Count(l => l.lead == 1 && l.queue_Lead < 1);
                foreach (var v in unc.ONLClimberCompLink1)
                    if (v.lead == 1)
                        newCnt--;
                if (unc.lead == 1)
                    newCnt++;
                if (newCnt > quota)
                {
                    message = "Квота для Вашего региона в трудности заполнена.";
                    return InsertingResult.QUOTA_EXCEED;
                }

                newCnt = otherClimbers.Count(l => l.speed == 1 && l.queue_Speed < 1);
                foreach (var v in unc.ONLClimberCompLink1)
                    if (v.speed == 1)
                        newCnt--;
                if (unc.speed == 1)
                    newCnt++;
                if (newCnt > quota)
                {
                    message = "Квота для Вашего региона в скорости заполнена.";
                    return InsertingResult.QUOTA_EXCEED;
                }

                newCnt = otherClimbers.Count(l => l.boulder == 1 && l.queue_Boulder < 1);
                foreach (var v in unc.ONLClimberCompLink1)
                    if (v.boulder == 1)
                        newCnt--;
                if (unc.boulder == 1)
                    newCnt++;
                if (newCnt > quota)
                {
                    message = "Квота для Вашего региона в боулдеринге заполнена.";
                    return InsertingResult.QUOTA_EXCEED;
                }
            }
            foreach (var v in unc.ONLClimberCompLink1.ToArray())
                dc.ONLClimberCompLinks.DeleteObject(v);
            unc.state = Constants.CLIMBER_CONFIRMED;
            qfErr = !checkQf(dc, unc);
            if (qfErr)
                unc.appl_type = "Разряд не соответствует положению";
            unc.sys_date_update = DateTime.UtcNow;
            dc.SaveChanges();
            return InsertingResult.SUCCESS;
        }
        //private InsertingResult UpdateClimberOld(ONLClimberCompLink unc, out bool qfErr)
        //{

        //    qfErr = false;
        //    if (cn.State != ConnectionState.Open)
        //        cn.Open();

        //    ClimbingClassesDataContext dcC = new ClimbingClassesDataContext(cn);
        //    int nCount = (from c in dcC.ONLclimbers
        //                  where c.surname.ToLower() + c.name.ToLower() ==
        //                  unc.surname.ToLower() + unc.name.ToLower() &&
        //                  c.team_id == unc.team_id &&
        //                  c.age == unc.age
        //                  select c).Count();
        //    if (nCount > 1)
        //        return InsertingResult.DUPLICATE;

        //    SqlTransaction tran = null;

        //    bool tranSuccess = false;
        //    try
        //    {

        //        ONLclimbersTableAdapter ta = new ONLclimbersTableAdapter();
        //        ta.Connection = cn;


        //        var dtClm = ta.GetDataByIID(unc.ONLclimber.iid);
        //        if (dtClm.Rows.Count < 1)
        //            throw new ArgumentException("Участник не найден");

        //        tran = cn.BeginTransaction();
        //        ta.Transaction = tran;
        //        var row = dtClm[0];
        //        bool newRanking = ((row.surname + row.name).ToLower().Replace(" ", "") != (unc.surname + unc.name).ToLower().Replace(" ", ""));
        //        int res = ta.Update(
        //        unc.surname,
        //        unc.name,
        //        unc.age,
        //        unc.genderFemale,
        //        unc.qf,
        //        unc.team_id,
        //        unc.group_id,
        //        unc.lead,
        //        unc.speed,
        //        unc.boulder,
        //        (newRanking || row.IsrankingLeadNull() ? null : (int?)row.rankingLead),
        //        (newRanking || row.IsrankingSpeedNull() ? null : (int?)row.rankingSpeed),
        //        (newRanking || row.IsrankingBoulderNull() ? null : (int?)row.rankingBoulder),
        //        (newRanking ? false : row.vk),
        //        newRanking ? false : row.nopoints,
        //        newRanking ? null : (row.IsphotoNull() ? null : row.photo),
        //        newRanking ? false : row.phLoaded,
        //        row.is_del,
        //        row.sys_date_update,
        //        row.appl_type,
        //         row.is_changeble,
        //        row.queue_pos,
        //        row.iid
        //       );
        //        if (newRanking)
        //            LoadRanking(row.iid, cn, tran);
        //        if (!CheckQuota(unc, tran))
        //            return InsertingResult.QUOTA_EXCEED;

        //        return FinalizeInsertUpdate(unc, ref qfErr, tran, ref tranSuccess, unc.climber_id.Value);
        //    }
        //    finally
        //    {
        //        if (tran != null)
        //        {
        //            if (tranSuccess)
        //                tran.Commit();
        //            else
        //                tran.Rollback();
        //        }
        //    }
        //}

        //private static bool checkQf(int climberID, SqlConnection cn, SqlTransaction tran)
        //{
        //    bool qfErr, isDel;
        //    checkQf(climberID, cn, tran, out qfErr, out isDel);
        //    return qfErr;
        //}

        public static bool DeleteDeadClimbers(Entities dc)
        {
            try
            {
                var lst = from c in dc.ONLclimbers
                          where c.ONLClimberCompLinks.Count < 1
                          select c;
                if (lst.Count() < 1)
                    return false;
                List<ONLclimber> cListToDel = new List<ONLclimber>();
                foreach (var c in lst)
                    cListToDel.Add(c);
                foreach (var c in lst)
                    dc.ONLclimbers.DeleteObject(c);
                dc.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        private static bool checkQf(Entities dc, ONLClimberCompLink clm)
        {
            string qfToCheck = clm.qf;
            int qfToCheckID;
            try { qfToCheckID = dc.ONLqfLists.First(q => q.qf == qfToCheck).iid; }
            catch { qfToCheckID = dc.ONLqfLists.OrderByDescending(c => c.iid).First().iid; }
            try
            {
                int groupToCheck = (from d in dc.ONLGroupsCompLinks
                                    where d.comp_id == clm.comp_id
                                    && d.group_id == clm.group_id
                                    select d.minQf).First();
                return (qfToCheckID <= groupToCheck);
            }
            catch { return true; }
        }

        //private static bool checkQf(int climberID, SqlConnection cn, SqlTransaction tran, out bool qfErr, out bool isDel)
        //{
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.Connection = cn;
        //    cmd.Transaction = tran;
        //    cmd.CommandText = "SELECT q.iid, g.minQf, c.is_del " +
        //                     "  FROM ONLclimbers c(NOLOCK)" +
        //                     "  JOIN ONLqfList q(NOLOCK) ON q.qf = c.qf" +
        //                     "  JOIN ONLgroups g(NOLOCK) ON g.iid = c.group_id" +
        //                     " WHERE c.iid = " + climberID.ToString();
        //    SqlDataReader rdr = cmd.ExecuteReader();
        //    try
        //    {
        //        if (rdr.Read())
        //        {
        //            isDel = Convert.ToBoolean(rdr["is_del"]);
        //            qfErr= (Convert.ToInt32(rdr["iid"]) > Convert.ToInt32(rdr["minQf"]));
        //            return true;
        //        }
        //    }
        //    finally { rdr.Close(); }
        //    qfErr = isDel = true;
        //    return false;
        //}

        //private InsertingResult FinalizeInsertUpdate(ONLClimberCompLink unc, ref bool qfErr, SqlTransaction tran, ref bool tranSuccess, int climberID)
        //{
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.Connection = cn;
        //    cmd.Transaction = tran;
        //    cmd.CommandText = "DELETE FROM ONLclimbersUnc WHERE iid = " + unc.iid.ToString();
        //    cmd.ExecuteNonQuery();

        //    RecalculateQueue(unc.group_id.Value, climberID, cn, tran);

        //    bool isDel;
        //    tranSuccess = checkQf(climberID, cn, tran, out qfErr, out isDel);
            
            
        //    if (isDel)
        //        return InsertingResult.TO_QUEUE;
        //    else
        //        return InsertingResult.SUCCESS;
        //}

        private InsertingResult InsertClimber(ONLClimberCompLink unc, out bool qfErr, out string message)
        {
            message = String.Empty;
            qfErr = false;

            int quota;
            try { quota = unc.ONLteam.ONLTeamGroup.ONLTeamGroupCompLinks.First(l => l.comp_id == unc.comp_id).quota_style_total; }
            catch { quota = 0; }
            if (quota > 0)
            {
                var otherClimbers = unc.ONLCompetition.ONLClimberCompLinks.Where(lnk =>
                    lnk.team_id == unc.team_id
                    && lnk.group_id == unc.group_id
                    && lnk.state == Constants.CLIMBER_CONFIRMED
                    && lnk.climber_id != unc.climber_id).ToArray();

                if (unc.lead == 1 && otherClimbers.Count(l => l.lead == 1) >= quota)
                {
                    message = "Квота для Вашего региона в трудности заполнена.";
                    return InsertingResult.QUOTA_EXCEED;
                }

                if (unc.speed == 1 && otherClimbers.Count(l => l.speed == 1) >= quota)
                {
                    message = "Квота для Вашего региона в скорости заполнена.";
                    return InsertingResult.QUOTA_EXCEED;
                }

                if (unc.boulder == 1 && otherClimbers.Count(l => l.boulder == 1) >= quota)
                {
                    message = "Квота для Вашего региона в боулдеринге заполнена.";
                    return InsertingResult.QUOTA_EXCEED;
                }
            }

            int nCount = (from l in dc.ONLClimberCompLinks
                          where l.comp_id == unc.comp_id
                          && l.climber_id == unc.climber_id
                          && l.team_id == unc.team_id
                          && l.state == Constants.CLIMBER_CONFIRMED
                          select l).Count();
            if (nCount > 0)
                return InsertingResult.DUPLICATE;
            unc.state = Constants.CLIMBER_CONFIRMED;
            qfErr = !checkQf(dc, unc);
            unc.sys_date_update = DateTime.UtcNow;
            if (qfErr)
                unc.appl_type = "Разряд не соответствует положению.";
            dc.SaveChanges();
            return InsertingResult.SUCCESS;
        }

        //private InsertingResult InsertClimber(ONLClimberCompLink unc, out bool qfErr)
        //{
        //    qfErr = false;
        //    if (cn.State != ConnectionState.Open)
        //        cn.Open();
        //    ClimbingClassesDataContext dcC = new ClimbingClassesDataContext(cn);
        //    int nCount = (from c in dcC.ONLclimbers
        //                  where c.surname.ToLower() + c.name.ToLower() ==
        //                  unc.surname.ToLower() + unc.name.ToLower() &&
        //                  c.team_id == unc.team_id &&
        //                  c.age == unc.age
        //                  select c).Count();
        //    if (nCount > 0)
        //        return InsertingResult.DUPLICATE;
        //    SqlTransaction tran = cn.BeginTransaction();
        //    int insId = -1;
        //    bool tranSuccess = false;
        //    try
        //    {
        //        ONLclimbersTableAdapter ta = new ONLclimbersTableAdapter();
        //        ta.Connection = cn;
        //        ta.Transaction = tran;
        //        insId = SortingClass.GetNextNumber(unc.group_id.Value, cn, tran, AutoNum.GRP, true);
        //        int res =
        //            ta.Insert(
        //            insId,
        //            unc.surname,
        //            unc.name,
        //            unc.age,
        //            unc.genderFemale,
        //            unc.qf,
        //            unc.team_id,
        //            unc.group_id,
        //            unc.lead,
        //            unc.speed,
        //            unc.boulder,
        //            null,
        //            null,
        //            null,
        //            false,
        //            false,
        //            null,
        //            false,
        //            true,
        //            DateTime.Now.ToUniversalTime(),
        //            "",
        //            true,
        //            int.MaxValue);
        //        unc.climber_id = insId;
        //        LoadRanking(insId, cn, tran);
        //        if (!CheckQuota(unc, tran))
        //            return InsertingResult.QUOTA_EXCEED;

        //        return FinalizeInsertUpdate(unc, ref qfErr, tran, ref tranSuccess, insId);
        //    }
        //    finally
        //    {
        //        if (tranSuccess)
        //            tran.Commit();
        //        else
        //            tran.Rollback();
        //    }
        //}

        //public static DateTime GetDeadLine(bool isMode)
        //{
        //    string deadline = WebConfigurationManager.AppSettings["DeadLine" + (isMode ? "Mode" : "")];
        //    if (String.IsNullOrEmpty(deadline))
        //        return new DateTime(3000, 12, 31);

        //    DateTime dtDeadLine = DateTime.Parse(deadline,
        //        new CultureInfo("ru-RU"), DateTimeStyles.AssumeUniversal).ToUniversalTime();
        //    dtDeadLine = dtDeadLine.AddDays(1.0);
        //    int timeDiff;
        //    try
        //    {
        //        if (!int.TryParse(WebConfigurationManager.AppSettings["TimeDiff"], out timeDiff))
        //            timeDiff = 0;
        //    }
        //    catch { timeDiff = 0; }
        //    dtDeadLine = dtDeadLine.AddHours(-timeDiff);
        //    return dtDeadLine;
        //}

        //private static int remainingRegions(int groupID, SqlConnection cn, SqlTransaction tran, bool UseDeadline)
        //{
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.Connection = cn;
        //    cmd.Transaction = tran;
        //    cmd.CommandText = "SELECT COUNT(*) cnt" +
        //                      "  FROM ONLTeams t(NOLOCK)" +
        //                      " WHERE EXISTS(SELECT * " +
        //                      "                FROM ONLclimbers c(NOLOCK)" +
        //                      "               WHERE c.team_id = t.iid" +
        //                      "                 AND c.group_id = " + groupID.ToString() +
        //                      "                 AND c.is_del = 1";
        //    if (UseDeadline)
        //    {
        //        cmd.CommandText += " AND c.sys_date_update <= @ddl ";
        //        cmd.Parameters.Add("@ddl", SqlDbType.DateTime);
        //        cmd.Parameters[0].Value = GetDeadLine(false);
        //    }
        //    cmd.CommandText += ")";
        //    return Convert.ToInt32(cmd.ExecuteScalar());
        //}

        private const int TOTAL_QUOTA = 60;
        private const int HOST_REGION = 22;
        public static void RecalculateQueue(long groupID, Entities dc)
        {
           
        }

        //public static void RecalculateQueue(long groupID)
        //{
        //    RecalculateQueue(groupID, 0, null, null);
        //}

        //class pStr
        //{
        //    public int iid;
        //    public int oldPos;
        //    public int newPos;
        //    public int teamID;
        //    public pStr(int iid, int oldPos, int newPos, int teamID)
        //    {
        //        this.oldPos = oldPos;
        //        this.newPos = newPos;
        //        this.iid = iid;
        //        this.teamID = teamID;
        //    }
        //}

        //public static void RecalculateQueue(long groupID, int lastClimber, SqlConnection cnP, SqlTransaction tranP)
        //{
        //    int maxGroup;
        //    string sM = WebConfigurationManager.AppSettings["GroupQuote"];
        //    if (!int.TryParse(sM, out maxGroup))
        //        maxGroup = int.MaxValue;


        //    SqlConnection cn;
        //    SqlTransaction tran;
        //    if (cnP == null)
        //        cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["db"].ConnectionString);
        //    else
        //        cn = cnP;
        //    bool tranSuccess = false;

        //    try
        //    {
        //        if (cn.State != ConnectionState.Open)
        //            cn.Open();
        //        if (tranP == null)
        //            tran = cn.BeginTransaction();
        //        else
        //            tran = tranP;
        //        try
        //        {
        //            SqlCommand cmd = new SqlCommand();
        //            cmd.Connection = cn;
        //            cmd.Transaction = tran;

        //            rnd.Next();
        //            string tableName;
        //            //string queueTable;
        //            int falseTables = 0;
        //            do
        //            {
        //                try
        //                {
        //                    tableName = "#clm" + rnd.Next();
        //                    //queueTable = "#q" + rnd.Next();

        //                    //cmd.CommandText = " SELECT iid,team_id,queue_pos,0 newPos" +
        //                    //                "     INTO " + queueTable +
        //                    //                "     FROM ONLclimbers(NOLOCK)" +
        //                    //                "    WHERE group_id = " + groupID.ToString() +
        //                    //                "      AND iid <> " + lastClimber.ToString();
        //                    //cmd.ExecuteNonQuery();

        //                    cmd.CommandText = "SELECT iid, is_del" +
        //                                      "  INTO " + tableName +
        //                                      "  FROM ONLclimbers(NOLOCK) " +
        //                                      " WHERE group_id = " + groupID.ToString() +
        //                                      "  AND iid <> " + lastClimber.ToString();
        //                    cmd.ExecuteNonQuery();
        //                    break;
        //                }
        //                catch
        //                {
        //                    tableName = "";
        //                    //queueTable = "";
        //                    falseTables++;
        //                }
        //            } while (falseTables < 5000);
        //            cmd.CommandText = "UPDATE ONLclimbers SET is_del = 1, is_changeble = 0 WHERE group_id = " + groupID.ToString();
        //            cmd.ExecuteNonQuery();
        //            //Если квота на группу меньше, чем число регионов, то увеличим квоту на группу
        //            int nTmp = remainingRegions(groupID, cn, tran, true);
        //            if (nTmp > maxGroup)
        //                maxGroup = nTmp;
        //            RecalculateFreePlaces(groupID, cn, tran, maxGroup, true, true, true);
        //            //do
        //            //{
        //            //    //посчитаем сколько есть свободных мест за вычетом обязательных региональных
        //            //    freePlaces = maxGroup - remainingRegions(groupID, cn, tran);
        //            //} while (RecalculateFreePlaces(groupID, cn, tran, freePlaces, false));
        //            ////Добавим минимум по 1 человеку от региона

        //            //RecalculateFreePlaces(groupID, cn, tran, maxGroup, true);

        //            CheckQfGroup(groupID, cn, tran);
        //            cmd.Parameters.Clear();

        //            if (!String.IsNullOrEmpty(tableName))
        //            {
        //                cmd.CommandText = " SELECT c.iid, c.surname + ' ' + c.name climber, t.name team, g.name grp, u.email, c.is_del " +
        //                                "     FROM ONLclimbers         c(NOLOCK) " +
        //                                "     JOIN " + tableName + " tmp(NOLOCK) ON tmp.iid   = c.iid AND tmp.is_del <> c.is_del " +
        //                                "     JOIN ONLteams            t(NOLOCK) ON t.iid     = c.team_id " +
        //                                "     JOIN ONLgroups           g(NOLOCK) ON g.iid     = c.group_id " +
        //                                "     JOIN ONLusers            u(NOLOCK) ON u.team_id = t.iid " +
        //                                "    WHERE u.email IS NOT NULL " +
        //                                "      AND u.email <> '' " +
        //                                " ORDER BY u.email, t.name, c.is_del, c.surname, c.name ";
        //                SqlDataReader rdr = cmd.ExecuteReader();
        //                try
        //                {
        //                    string sCurEmail = "", sToSend = "", sTmp;
        //                    int nNum = 0;
        //                    while (rdr.Read())
        //                    {
        //                        if (rdr["email"].ToString() != sCurEmail)
        //                        {
        //                            if (!String.IsNullOrEmpty(sCurEmail))
        //                                MailService.SendMail(sCurEmail, "Изменение статуса заявки", sToSend, MailPriority.Normal, out sTmp);
        //                            sToSend = "";
        //                            sCurEmail = rdr["email"].ToString();
        //                            nNum = 0;
        //                        }
        //                        if (!String.IsNullOrEmpty(sToSend))
        //                            sToSend += "\r\n";
        //                        sToSend += (++nNum).ToString() + ". " + rdr["climber"].ToString() + "|" + rdr["team"].ToString() +
        //                            "|" + rdr["grp"].ToString() + "|" +
        //                            (Convert.ToBoolean(rdr["is_del"]) ? "Перемещён в очередь" : "Перемещён в список подтверждённых участников");
        //                    }
        //                    if (!String.IsNullOrEmpty(sCurEmail))
        //                        MailService.SendMail(sCurEmail, "Изменение статуса заявки", sToSend, MailPriority.Normal, out sTmp);
        //                }
        //                finally { rdr.Close(); }
        //                cmd.CommandText = "DROP TABLE " + tableName;
        //                cmd.ExecuteNonQuery();
        //            }

        //            tranSuccess = true;

        //        }
        //        finally
        //        {
        //            if (tranP == null)
        //                try
        //                {
        //                    if (tranSuccess)
        //                        tran.Commit();
        //                    else
        //                        tran.Rollback();
        //                }
        //                catch { }
        //        }
        //    }
        //    finally
        //    {
        //        try
        //        {
        //            if (cnP == null && cn.State != ConnectionState.Closed)
        //                cn.Close();
        //        }
        //        catch { }
        //    }
        //}

        //private static void CheckQfGroup(int groupID, SqlConnection cn, SqlTransaction tran)
        //{
        //    ClimbingClassesDataContext dc = new ClimbingClassesDataContext(cn);
        //    dc.Transaction = tran;
        //    var clmLstV = from cc in dc.ONLclimbers
        //                  join q in dc.ONLqfLists on cc.qf equals q.qf
        //                  where cc.group_id == groupID &&
        //                  q.iid > cc.ONLGroup.minQf
        //                  select cc.iid;
        //    List<int> clmList = new List<int>();
        //    foreach (int n in clmLstV)
        //        clmList.Add(n);
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.Connection = cn;
        //    cmd.Transaction = tran;
        //    cmd.CommandText = "UPDATE ONLclimbers SET appl_type = appl_type + '; ' + @at WHERE iid=@iid";
        //    cmd.Parameters.Add("@at", SqlDbType.VarChar, 4000);
        //    cmd.Parameters[0].Value = RAZR_NOTE;
        //    cmd.Parameters.Add("@iid", SqlDbType.Int);
        //    foreach (int n in clmList)
        //    {
        //        cmd.Parameters[1].Value = n;
        //        cmd.ExecuteNonQuery();
        //    }

        //}

        //public class ClimTeamPos
        //{
        //    public int ClimberID { get; private set; }
        //    public int TeamID { get; private set; }
        //    public int TeamPos { get; private set; }
        //    public int Ranking { get; private set; }
        //    public int Index { get; set; }
        //    public int QueuePos { get; private set; }
        //    public DateTime DateUpd { get; private set; }
        //    public ClimTeamPos(ONLclimber climber)
        //    {
        //        this.Index = 0;
        //        this.ClimberID = climber.iid;
        //        this.TeamID = climber.ONLteam.iid;
        //        this.TeamPos = climber.ONLteam.pos;
        //        this.QueuePos = climber.queue_pos;
        //        this.DateUpd = climber.sys_date_update;
        //        ONLclimber c = climber;
        //        this.Ranking = Math.Min(c.rankingLead == null ? int.MaxValue : c.rankingLead.Value,
        //                                Math.Min(c.rankingSpeed == null ? int.MaxValue : c.rankingSpeed.Value,
        //                                    c.rankingBoulder == null ? int.MaxValue : c.rankingBoulder.Value));
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="groupID"></param>
        ///// <param name="cn"></param>
        ///// <param name="tran"></param>
        ///// <param name="rT"></param>
        ///// <param name="maxGroup">Скольковсего человек может быть в группе</param>
        ///// <returns></returns>
        //private static bool RecalculateFreePlaces(int groupID, SqlConnection cn, SqlTransaction tran, int maxGroup, bool ranking, bool team, bool late)
        //{
        //    bool hasInserted = false;
        //    List<int> lstT = new List<int>();
        //    List<int> loosers = new List<int>();
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.Connection = cn;
        //    cmd.Transaction = tran;
        //    ClimbingClassesDataContext dc = new ClimbingClassesDataContext(cn);
        //    dc.Transaction = tran;

        //    //Число свободных мест
        //    int freePlaces;

        //    DateTime ddl = GetDeadLine(false);

        //    if (ranking)
        //    {
        //        //Заявим тех, кто проходит по рейтингу (на свободные места (из расчёта одно место на команду))
        //        const int RANKING_THRESHOLD = 30;
        //        bool smtEntered;
        //        do
        //        {
        //            freePlaces = maxGroup - remainingRegions(groupID, cn, tran, true) - (from c in dc.ONLclimbers
        //                                                                                 where c.group_id == groupID && !c.is_del
        //                                                                                 select c).Count();
        //            smtEntered = false;

        //            //РЕйтинговые кто вовремя заявился и те, кого тренера насильно в очередь сунули
        //            var l1 = from c in dc.ONLclimbers
        //                     where c.group_id == groupID && c.is_del && (
        //                     c.rankingLead != null && c.rankingLead <= RANKING_THRESHOLD ||
        //                     c.rankingSpeed != null && c.rankingSpeed <= RANKING_THRESHOLD ||
        //                     c.rankingBoulder != null && c.rankingBoulder <= RANKING_THRESHOLD)
        //                     && c.sys_date_update <= ddl
        //                     orderby Math.Min(c.rankingLead == null ? int.MaxValue : c.rankingLead.Value,
        //                                    Math.Min(c.rankingSpeed == null ? int.MaxValue : c.rankingSpeed.Value,
        //                                        c.rankingBoulder == null ? int.MaxValue : c.rankingBoulder.Value))
        //                     select new
        //                     {
        //                         iid = c.iid,
        //                         Ranking = Math.Min(c.rankingLead == null ? int.MaxValue : c.rankingLead.Value,
        //                                    Math.Min(c.rankingSpeed == null ? int.MaxValue : c.rankingSpeed.Value,
        //                                        c.rankingBoulder == null ? int.MaxValue : c.rankingBoulder.Value)),
        //                         dateUpd = c.sys_date_update
        //                     };
        //            lstT.Clear();
        //            int prevPlace = -1;
        //            foreach (var q in l1)
        //            {
        //                if (freePlaces > 0 || q.Ranking == prevPlace)
        //                {
        //                    loosers.Remove(q.iid);
        //                    lstT.Add(q.iid);
        //                    prevPlace = q.Ranking;
        //                    freePlaces--;
        //                }
        //                else
        //                {
        //                    if (loosers.IndexOf(q.iid) < 0)
        //                        loosers.Add(q.iid);
        //                }
        //            }
        //            foreach (var n in lstT)
        //            {
        //                cmd.CommandText = "UPDATE ONLclimbers SET is_del = 0, is_changeble=0, appl_type='Рейтинг', queue_pos = 0 WHERE iid = " + n.ToString();
        //                cmd.ExecuteNonQuery();
        //                if (!hasInserted)
        //                    hasInserted = true;
        //                if (!smtEntered)
        //                    smtEntered = true;
        //            }
        //        } while (smtEntered);
        //    }


        //    freePlaces = maxGroup - (from c in dc.ONLclimbers
        //                             where c.group_id == groupID && !c.is_del
        //                             select c).Count();

        //    #region Распределение командных квот
            
        //    cmd.Parameters.Clear();
        //    cmd.CommandText = "   SELECT t.iid, t.pos, count(*) cnt" +
        //                      "     FROM ONLteams    t(nolock) " +
        //                      "     JOIN ONLclimbers c(nolock) ON c.team_id = t.iid " +
        //                      "    WHERE c.group_id = " + groupID.ToString() +
        //                      "      AND c.sys_date_update <= @ddl" +
        //                      " GROUP BY t.iid, t.pos";
        //    cmd.Parameters.Add("@ddl", SqlDbType.DateTime);
        //    cmd.Parameters[0].Value = ddl;
        //    TeamsStrCCollection teamList = new TeamsStrCCollection();
        //    SqlDataReader drd = cmd.ExecuteReader();
        //    try
        //    {
        //        while (drd.Read())
        //        {
        //            int pos = (drd["pos"] == DBNull.Value || drd["pos"] == null ? int.MaxValue : Convert.ToInt32(drd["pos"]));
        //            teamList.Add(new TeamStrC(Convert.ToInt32(drd["iid"]), pos, Convert.ToInt32(drd["cnt"])));
        //        }
        //    }
        //    finally { drd.Close(); }

        //    teamList.Sort();
        //    #endregion

        //    //Нерейтинговые кто вовремя заявился на оставшиеся места (минимум 1 на команду оставлено)
        //    if (team)
        //    {

        //        var lst2 = from c in dc.ONLclimbers
        //                   where c.group_id == groupID && c.is_del
        //                   && c.sys_date_update <= ddl
        //                   orderby c.ONLteam.pos, c.team_id, c.queue_pos, c.sys_date_update
        //                   select c;
        //        List<ClimTeamPos> l3 = new List<ClimTeamPos>();
        //        foreach (var wwwp in lst2)
        //            l3.Add(new ClimTeamPos(wwwp));
        //        l3.Sort(new Comparison<ClimTeamPos>(delegate(ClimTeamPos c1, ClimTeamPos c2)
        //        {
        //            if (c1.ClimberID == c2.ClimberID)
        //                return 0;
        //            if (c1.TeamPos != c2.TeamPos)
        //                return c1.TeamPos.CompareTo(c2.TeamPos);
        //            if (c1.TeamID != c2.TeamID)
        //                return c1.TeamID.CompareTo(c2.TeamID);
        //            if (c1.QueuePos != c2.QueuePos)
        //                return c1.QueuePos.CompareTo(c2.QueuePos);
        //            if (c1.Ranking != c2.Ranking)
        //                return c1.Ranking.CompareTo(c2.Ranking);
        //            return c1.DateUpd.CompareTo(c2.DateUpd);
        //        }));
        //        int cTeam = -1;
        //        int cInd = 0;
        //        foreach (var q in l3)
        //        {
        //            if (q.TeamID != cTeam)
        //            {
        //                cTeam = q.TeamID;
        //                cInd = 0;
        //            }
        //            q.Index = (++cInd);
        //        }
        //        l3.Sort(new Comparison<ClimTeamPos>(delegate(ClimTeamPos c1, ClimTeamPos c2)
        //        {
        //            if (c1.ClimberID== c2.ClimberID)
        //                return 0;
        //            if (c1.Index != c2.Index)
        //                return c1.Index.CompareTo(c2.Index);
        //            if (c1.TeamPos != c2.TeamPos)
        //                return c1.TeamPos.CompareTo(c2.TeamPos);
        //            if (c1.TeamID != c2.TeamID)
        //                return c1.TeamID.CompareTo(c2.TeamID);
        //            if (c1.QueuePos != c2.QueuePos)
        //                return c1.QueuePos.CompareTo(c2.QueuePos);
        //            if (c1.Ranking != c2.Ranking)
        //                return c1.Ranking.CompareTo(c2.Ranking);
        //            return c1.DateUpd.CompareTo(c2.DateUpd);
        //        }));

        //        lstT.Clear();
        //        int prevTeamInd = -1;
        //        int curTeamInd;
        //        foreach (var nnb in l3)
        //        {
        //            if (freePlaces > 0)
        //            {
        //                lstT.Add(nnb.ClimberID);
        //                freePlaces--;
        //                loosers.Remove(nnb.ClimberID);
        //                curTeamInd = teamList.GetIndexByIid(nnb.TeamID);
        //                if (curTeamInd < 0)
        //                    continue;
        //                if (prevTeamInd >= curTeamInd)
        //                {
        //                    for (int i = prevTeamInd + 1; i < teamList.Count; i++)
        //                        teamList[i].ShouldHave++;
        //                    prevTeamInd = -1;
        //                }
        //                for (int i = prevTeamInd + 1; i <= curTeamInd && i < teamList.Count; i++)
        //                    teamList[i].ShouldHave++;
        //                prevTeamInd = curTeamInd;
        //            }
        //            else
        //                if (loosers.IndexOf(nnb.ClimberID) < 0)
        //                    loosers.Add(nnb.ClimberID);
        //        }
        //        if (freePlaces > 0)
        //            foreach (var u in teamList)
        //                u.ShouldHave = int.MaxValue;
        //        foreach (var n in lstT)
        //        {
        //            cmd.CommandText = "UPDATE ONLclimbers SET is_del = 0, is_changeble = 1, appl_type = 'Команда', queue_pos = 0 WHERE iid = " + n.ToString();
        //            cmd.ExecuteNonQuery();
        //            if (!hasInserted)
        //                hasInserted = true;
        //        }
        //    }

        //    #region заполним квоту
        //    cmd.Parameters.Clear();
        //    cmd.CommandText = "   SELECT t.iid, count(*) cnt" +
        //                      "     FROM ONLteams t(NOLOCK)" +
        //                      "     JOIN ONLclimbers c(NOLOCK) ON c.team_id = t.iid" +
        //                      "    WHERE c.group_id = " + groupID.ToString() +
        //                      "      AND c.is_changeble = 1" +
        //                      "      AND c.is_del = 0 " +
        //                      "      AND c.sys_date_update <= @ddl " +
        //                      " GROUP BY t.iid";
        //    cmd.Parameters.Add("@ddl", SqlDbType.DateTime);
        //    cmd.Parameters[0].Value = ddl;
        //    drd = cmd.ExecuteReader();
        //    try
        //    {
        //        while (drd.Read())
        //        {
        //            int tId = Convert.ToInt32(drd["iid"]);
        //            int cnt = Convert.ToInt32(drd["cnt"]);
        //            var t = teamList.GetByIid(tId);
        //            if (t != null)
        //                t.AlreadyHave = cnt;
        //        }
        //    }
        //    finally { drd.Close(); }
        //    //посчитали сколько уже заполнено
        //    cmd.Parameters.Clear();
        //    cmd.CommandText = "UPDATE ONLclimbers SET is_changeble = 1 WHERE iid = @iid";
        //    cmd.Parameters.Add("@iid", SqlDbType.Int);
        //    foreach (var t in teamList)
        //    {
        //        if (t.AlreadyHave >= t.ShouldHave)
        //            continue;
        //        // если ещё есть незаполненная квота
        //        int toSet = t.ShouldHave - t.AlreadyHave;
        //        var clmLst = from c in dc.ONLclimbers
        //                     where c.team_id == t.Team
        //                     && c.group_id == groupID
        //                     && !c.is_changeble
        //                     && !c.is_del
        //                     orderby Math.Min(c.rankingLead == null ? int.MaxValue : c.rankingLead.Value,
        //                                Math.Min(c.rankingSpeed == null ? int.MaxValue : c.rankingSpeed.Value,
        //                                    c.rankingBoulder == null ? int.MaxValue : c.rankingBoulder.Value)) descending,
        //                            c.queue_pos descending, c.sys_date_update descending
        //                     select c;
        //        List<int> cLTS = new List<int>();
        //        foreach (var cccc in clmLst)
        //            cLTS.Add(cccc.iid);
        //        for (int i = 0; i < cLTS.Count && toSet > 0; i++)
        //        {
        //            toSet--;
        //            cmd.Parameters[0].Value = cLTS[i];
        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //    cmd.Parameters.Clear();

        //    #endregion

        //    if (late)
        //    {
        //        freePlaces = maxGroup - (from c in dc.ONLclimbers
        //                                 where c.group_id == groupID && !c.is_del
        //                                 select c).Count();
        //        //остатки в порядке подачи заявок
        //        var l4 = from u in dc.ONLclimbers
        //                 where u.is_del &&
        //                 u.group_id == groupID
        //                 orderby u.sys_date_update
        //                 select u.iid;
        //        lstT.Clear();
        //        foreach (var p in l4)
        //            if (freePlaces > 0)
        //            {
        //                lstT.Add(p);
        //                freePlaces--;
        //                loosers.Remove(p);
        //            }
        //            else
        //                if (loosers.IndexOf(p) < 0)
        //                    loosers.Add(p);
        //        foreach (var n in lstT)
        //        {
        //            cmd.CommandText = "UPDATE ONLclimbers SET is_del = 0, is_changeble = 0, appl_type = 'Поздняя заявка', queue_pos = 0 WHERE iid = " + n.ToString();
        //            cmd.ExecuteNonQuery();
        //            if (!hasInserted)
        //                hasInserted = true;
        //        }
        //    }

        //    //обработаем очередь
        //    if (team && !late && !ranking)
        //    {
        //        loosers.Clear();
        //        var lL = from d in dc.ONLclimbers
        //                 where d.group_id == groupID &&
        //                    d.is_del
        //                 orderby d.queue_pos
        //                 select d.iid;
        //        foreach (var nw in lL)
        //            loosers.Add(nw);

        //    }
        //    for (int i = 0; i < loosers.Count; i++)
        //    {
        //        cmd.CommandText = "UPDATE ONLclimbers SET is_del=1,is_changeble=0,appl_type='Очередь',queue_pos=" +
        //            (i + 1).ToString() + " WHERE iid=" + loosers[i].ToString();
        //        cmd.ExecuteNonQuery();
        //    }
        //    cmd.CommandText = "UPDATE ONLclimbers SET queue_pos = 0 WHERE is_del = 0";
        //    cmd.ExecuteNonQuery();
        //    return hasInserted;
        //}

        //private bool CheckQuota(ONLclimbersUnc clm, SqlTransaction tran)
        //{
        //    return true;
        //    /*
        //    string qGroupString = WebConfigurationManager.AppSettings["TeamQuoteGroup"];
        //    int qGr;
        //    if (!int.TryParse(qGroupString, out qGr))
        //        return true;

        //    SqlCommand cmd = new SqlCommand();
        //    cmd.Connection = cn;
        //    cmd.Transaction = tran;
        //    cmd.CommandText = "SELECT ISNULL(MIN(r.pos)," + int.MaxValue + ") pos" +
        //                      "  FROM ONLrankings r(NOLOCK)" +
        //                      "  JOIN ONLclimbers c(NOLOCK) ON r.climber = (c.surname + ' ' + c.name)" +
        //                      "  JOIN ONLgroups g(NOLOCK) ON g.iid = c.group_id AND r.group_name = g.name" +
        //                      " WHERE c.iid = " + clm.climber_id.ToString();
        //    int mPos;
        //    try { mPos = Convert.ToInt32(cmd.ExecuteScalar()); }
        //    catch { mPos = int.MaxValue; }

        //    ClimbingClassesDataContext dc = new ClimbingClassesDataContext(cn);


        //    string strRank = WebConfigurationManager.AppSettings["RankingLimit"];
        //    int rTr;
        //    if (!int.TryParse(strRank, out rTr))
        //        rTr = 0;
        //    if (mPos <= rTr)
        //        return true;

        //    cmd.CommandText = "SELECT COUNT(*) cnt" +
        //                      "  FROM ONLclimbers c(NOLOCK)" +
        //                      " WHERE c.team_id=" + clm.team_id.ToString() +
        //                      "   AND c.group_id=" + clm.group_id.ToString() +
        //                      "   AND ISNULL(c.rankingLead,9999) > " + rTr.ToString() +
        //                      "   AND ISNULL(c.rankingBoulder,9999) > " + rTr.ToString() +
        //                      "   AND ISNULL(c.rankingSpeed,9999) > " + rTr.ToString();
        //    int cnt = Convert.ToInt32(cmd.ExecuteScalar());
        //    return (cnt <= qGr);
        //     * */
        //}

        public static void LoadAllRankings(Entities dc)
        {
        }

        //public static void LoadAllRankings(SqlConnection cn)
        //{
        //    if (cn.State != ConnectionState.Open)
        //        cn.Open();
        //    SqlTransaction tran = cn.BeginTransaction();
        //    bool tranSuccess = false;
        //    try
        //    {
        //        ClimbingClassesDataContext dc = new ClimbingClassesDataContext(cn);
        //        dc.Transaction = tran;
        //        var grList = from d in dc.ONLGroups
        //                     select d.iid;
        //        List<int> lst = new List<int>();
        //        foreach (int n in grList)
        //            lst.Add(n);
        //        foreach (int i in lst)
        //            ReloadRankings(i, cn, tran);
        //        tranSuccess = true;
        //    }
        //    finally
        //    {
        //        if (tranSuccess)
        //            tran.Commit();
        //        else
        //            tran.Rollback();
        //    }
        //}

        //public static void ReloadRankings(int groupID, SqlConnection cn, SqlTransaction tran)
        //{
        //    if (cn.State != ConnectionState.Open)
        //        cn.Open();
        //    SqlTransaction tranL = (tran == null ? cn.BeginTransaction() : tran);
        //    bool tranSuccess = false;
        //    try
        //    {
        //        ClimbingClassesDataContext dc = new ClimbingClassesDataContext(cn);
        //        dc.Transaction = tranL;
        //        var clmList = from q in dc.ONLclimbers
        //                      where q.group_id == groupID
        //                      select q;
        //        List<ONLclimber> lst = new List<ONLclimber>();
        //        foreach (var qwq in clmList)
        //            lst.Add(qwq);
        //        foreach (var qq in lst)
        //            LoadRanking(qq.iid, cn, tranL);
        //        tranSuccess = true;
        //    }
        //    finally
        //    {
        //        if (tran == null)
        //        {
        //            if (tranSuccess)
        //                tranL.Commit();
        //            else
        //                tranL.Rollback();
        //        }
        //    }
        //}

        //private static void LoadRanking(int climber_id, SqlConnection cn, SqlTransaction tran)
        //{
        //    if (cn.State != ConnectionState.Open)
        //        cn.Open();
        //    SqlTransaction tranL = (tran == null ? cn.BeginTransaction() : tran);
        //    bool tranSuccess = false;
        //    try
        //    {
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.Connection = cn;
        //        cmd.Transaction = tranL;
        //        cmd.CommandText = "UPDATE ONLclimbers SET rankingLead = @l," +
        //            "rankingSpeed = @s,rankingBoulder = @b WHERE iid=@iid";
        //        cmd.Parameters.Add("@l", SqlDbType.Int);
        //        cmd.Parameters.Add("@s", SqlDbType.Int);
        //        cmd.Parameters.Add("@b", SqlDbType.Int);
        //        cmd.Parameters.Add("@iid", SqlDbType.Int);
        //        string[] styles = { "Трудность", "Скорость", "Боулдеринг" };
                
        //        ClimbingClassesDataContext dc = new ClimbingClassesDataContext(cn);
        //        dc.Transaction = tranL;
        //        var clmL = from c in dc.ONLclimbers
        //                   where c.iid == climber_id
        //                   select c;
        //        List<ONLclimber> lst = new List<ONLclimber>();
        //        foreach (ONLclimber u in clmL)
        //            lst.Add(u);

        //        foreach (ONLclimber clm in lst)
        //        {
        //            cmd.Parameters[3].Value = clm.iid;
        //            for (int i = 0; i < styles.Length && i < 3; i++)
        //            {
        //                object val;
        //                var rL = from d in dc.ONLrankings
        //                         where d.climber.Replace('ё', 'е') == clm.surname.Replace('ё', 'е') + " " + clm.name.Replace('ё', 'е') &&
        //                         d.group_id == clm.group_id &&
        //                         d.team_id == clm.team_id &&
        //                         d.style == styles[i]
        //                         select d.pos;
        //                if (rL.Count() < 1)
        //                    val = DBNull.Value;
        //                else
        //                    val = rL.First();
        //                cmd.Parameters[i].Value = val;
        //            }
        //            cmd.ExecuteNonQuery();
        //        }
        //        tranSuccess = true;
        //    }
        //    finally
        //    {
        //        if (tran == null)
        //        {
        //            if (tranSuccess)
        //                tranL.Commit();
        //            else
        //                tranL.Rollback();
        //        }
        //    }
        //}

        

        //private sealed class TeamStrC
        //{
        //    public int Team { get; private set; }
        //    public int posR { get; private set; }
        //    public int ClimbersCount { get; private set; }
        //    public int ShouldHave { get; set; }
        //    public int AlreadyHave { get; set; }
        //    public TeamStrC(int team_id, int rankingPos, int climbersCount)
        //    {
        //        this.Team = team_id;
        //        this.ShouldHave = this.AlreadyHave = 0;
        //        this.posR = rankingPos;
        //        this.ClimbersCount = climbersCount;
        //    }
        //}

        //private sealed class TeamsStrCCollection : List<TeamStrC>
        //{
        //    public TeamStrC GetByIid(int team_id)
        //    {
        //        foreach (var v in this)
        //            if (v.Team == team_id)
        //                return v;
        //        return null;
        //    }
        //    public int GetIndexByIid(int team_id)
        //    {
        //        for (int i = 0; i < this.Count; i++)
        //            if (this[i].Team == team_id)
        //                return i;
        //        return -1;
        //    }
        //    public new void Sort()
        //    {
        //        this.Sort(new Comparison<TeamStrC>(delegate(TeamStrC c1, TeamStrC c2)
        //        {
        //            if (c1.Team == c2.Team)
        //                return 0;
        //            if (c1.posR != c2.posR)
        //                return c1.posR.CompareTo(c2.posR);
        //            return c1.Team.CompareTo(c2.Team);
        //        }));
        //    }
        //}
    }
}