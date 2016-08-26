using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web.Configuration;
using ClimbingCompetition.Online;

namespace WebClimbing.src
{
    public sealed class Tasks
    {
        Entities dc;
        long compID;
        ONLCompetition comp;
        private Tasks(long compID)
        {
            this.compID = compID;
            dc = new Entities(WebConfigurationManager.ConnectionStrings["db_Entities"].ConnectionString);
            if (dc.Connection.State != ConnectionState.Open)
                dc.Connection.Open();
            try { comp = dc.ONLCompetitions.First(c => c.iid == this.compID); }
            catch { comp = null; }
        }
        ~Tasks()
        {
            try
            {
                if (dc.Connection.State != ConnectionState.Closed)
                    dc.Connection.Close();
            }
            catch { }
        }

        public static string SendClimbersListForUser(string userID, long compID)
        {
            try
            {
                Tasks t = new Tasks(compID);
                return t.SendClimbersListForUserF(userID);
            }
            catch (Exception ex)
            {
                return "Ошибка отправки списка: " + ex.Message;
            }
        }

        private bool IsUserAdmin(string userID)
        {
            var r = (from ur in dc.ONLuserRoles
                     where ur.user_id == userID
                     && ur.comp_id == compID
                     && ur.role_id == Constants.ROLE_ADMIN
                     select ur).Count();
            if (r > 0)
                return true;
            r = (from ur in dc.ONLuserRoles
                 where ur.role_id == Constants.ROLE_ADMIN_ROOT
                 && ur.user_id == userID
                 select ur).Count();
            return (r > 0);
        }
        private const string SCCS = "Список успешно отправлен на Ваш email";
        private string SendClimbersListForUserF(string userID)
        {
            
            ONLuser usr;
            try { usr = dc.ONLusers.First(u => u.iid == userID); }
            catch { usr = null; }
            if (usr == null)
                return "У вас нет прав на это действие";
            if (String.IsNullOrEmpty(usr.email))
                return "Контактный email не указан";
            string subj = "Список участников";
            string body = "См. вложение";
            if (IsUserAdmin(userID))
            {
                if (SendClimbersListForTeam(usr.email, null, subj, body))
                    return "Список успешно отправлен на Ваш email";
                else
                    return "Ошибка отправки списка";
            }
            else
            {
                var tL = from u in dc.ONLusers
                         where u.iid == userID
                         select u.team_id;
                if (tL.Count() < 1)
                    return "Регион для данного пользователя не задан";
                var t = tL.First();
                if (SendClimbersListForTeam(usr.email, t.Value, subj, body))
                    return SCCS;
                else
                    return "Ошибка отправки списка";
            }

        }

        private static Mutex m = new Mutex();

        public static void CheckDDL(long compID)
        {
            try
            {
                if (checkingThread != null && checkingThread.IsAlive)
                    return;
                checkingThread = new Thread(CheckDDLStatic);
                checkingThread.Start(compID);
            }
            catch { }
        }
        private static Thread checkingThread = null;

        private static void CheckDDLStatic(object compDO)
        {
            m.WaitOne();
            try
            {
                long compID = Convert.ToInt64(compDO);
                Tasks t = new Tasks(compID);
                t.CheckAndSend();
            }
            catch { }
            finally { m.ReleaseMutex(); }
        }

        private void CheckAndSend()
        {
            try
            {
                if (comp == null)
                    return;
                DateTime now = DateTime.Now;
                DateTime check1 = comp.GetDateParam(Constants.PDB_COMP_DEADLINE_CHANGE);
                DateTime check2 = comp.GetDateParam(Constants.PDB_COMP_DEADLINE);
                string sNotif, sNotifCheck;

                if (now > check1)
                {
                    sNotif = "Добрый день.\r\nСегодня в 24.00 по московскому времени заканчивается прием изменений. " +
                            "Пожалуйста, во избежании недоразумений проверьте список заявленных участников (см. во вложении)\r\n" +
                            "С Уважением, Администратор системы";
                    sNotifCheck = Constants.NOTIF_LAST_CHANGE;
                }
                else if (now > check2)
                {
                    sNotif = "Добрый день.\r\nСегодня в 24.00 по Московскому времени заканчивается прием заявок. " +
                            "Пожалуйста, во избежании недоразумений проверьте список заявленных участников (см. во вложении)\r\n" +
                            "С Уважением, Администратор системы";
                    sNotifCheck = Constants.NOTIF_DEADLINE;
                }
                else
                    return;
                var urList = comp.ONLuserRoles.Where(u => u.role_id == Constants.ROLE_USER).ToArray();
                foreach (var r in urList)
                {
                    try
                    {
                        var uToCheck = dc.ONLuserRoles.First(rt => rt.iid == r.iid);
                        if (uToCheck.notifSent != null && uToCheck.notifSent.IndexOf(sNotifCheck) > -1)
                            continue;
                        if (uToCheck.notifSent == null)
                            uToCheck.notifSent = sNotifCheck;
                        else
                            uToCheck.notifSent += sNotifCheck;
                        dc.SaveChanges();
                        if (String.IsNullOrEmpty(uToCheck.ONLuser.email))
                            continue;
                        if (uToCheck.ONLuser.team_id == null)
                            continue;
                        int teamID = uToCheck.ONLuser.team_id.Value;
                        SendClimbersListForTeam(uToCheck.ONLuser.email, teamID, "Список участников", sNotif);
                    }
                    catch { }
                }
            }
            catch { }
        }

        private static string getShortValue(short par)
        {
            switch (par)
            {
                case 1: return "+";
                case 2: return "Лично";
                default: return "-";
            }
        }

        private bool SendClimbersListForTeam(string email, int? team_id, string subj, string body)
        {

            string fileName;
            if (team_id == null)
                fileName = "LIST";
            else
                fileName = "LIST_" + team_id.Value.ToString();
            fileName += "_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmm", CultureInfo.CurrentCulture) + ".csv";

            MemoryStream mStr = new MemoryStream();
            try
            {
                StreamWriter swr = new StreamWriter(mStr, Encoding.Unicode);
                ONLteam[] teams;
                if (team_id == null)
                {
                    teams = (from t in dc.ONLTeamsCompLinks
                             where t.comp_id == compID
                             select t.ONLteam).Distinct().ToArray();
                }
                else
                    teams = (from tL in dc.ONLteams
                             where tL.iid == team_id
                             select tL).ToArray();
                //teams = (from t in teams
                //         where t.ONLClimberCompLinks.Count(l => l.comp_id == compID) > 0
                //         select t).ToArray();
                bool lead = comp.Lead(), speed = comp.Speed(), boulder = comp.Boulder();

                try
                {
                    var grpLstT = from g in dc.ONLGroups
                                  where g.ONLGroupsCompLinks.Count(gl => gl.comp_id == compID) > 0
                                  orderby g.oldYear, g.genderFemale
                                  select g;
                    List<ONLGroup> grpList = new List<ONLGroup>();
                    foreach (var g in grpLstT)
                        grpList.Add(g);
                    foreach (var t in teams)
                    {
                        int teamID = t.iid;
                        swr.AutoFlush = true;
                        swr.WriteLine(t.name);
                        if (t.HasClimbers(compID))
                        {
                            swr.WriteLine("№\tФамилия, Имя\tГ.р.\tРазр." +
                                (lead ? "\tТр." : String.Empty) +
                                (speed ? "\tСк." : String.Empty) +
                                (boulder ? "\tБоулд." : String.Empty)
                                );
                            foreach (var g in grpList)
                            {
                                var clmLstT = from c in dc.ONLClimberCompLinks
                                              where c.group_id == g.iid
                                              && c.team_id == teamID
                                              && c.state == Constants.CLIMBER_CONFIRMED
                                              && c.comp_id == compID
                                              orderby c.ONLclimber.surname, c.ONLclimber.name
                                              select c;
                                int curClm = 0;
                                swr.WriteLine(g.name + ":");
                                if (t.HasClimbers(compID, g.iid))
                                {
                                    foreach (var c in clmLstT.Where(w => w.queue_pos < 1))
                                        swr.WriteLine((++curClm).ToString() + ".\t" +
                                            c.ONLclimber.surname + " " + c.ONLclimber.name + "\t" +
                                            c.ONLclimber.age.ToString() + "\t" +
                                            c.qf +
                                            (lead ? ("\t" + getShortValue(c.lead)) : String.Empty) +
                                            (speed ? ("\t" + getShortValue(c.speed)) : String.Empty) +
                                            (boulder ? ("\t" + getShortValue(c.boulder)) : String.Empty));

                                    swr.WriteLine();
                                    swr.WriteLine("Очередь:");
                                    foreach (var c in clmLstT.Where(w => w.queue_pos > 0))
                                        swr.WriteLine(c.queue_pos.ToString() + ".\t" +
                                            c.ONLclimber.surname + " " + c.ONLclimber.name + "\t" +
                                            c.ONLclimber.age.ToString() + "\t" +
                                            c.qf);
                                    swr.WriteLine();
                                    swr.WriteLine("Неподтверждённые:");
                                    curClm = 0;
                                    var clmLstU = from c in dc.ONLClimberCompLinks
                                                  where c.group_id == g.iid
                                                  && c.team_id == teamID
                                                  && c.comp_id == compID
                                                  && c.state != Constants.CLIMBER_CONFIRMED
                                                  orderby c.ONLclimber.surname, c.ONLclimber.name
                                                  select c;
                                    foreach (var c in clmLstU)
                                        swr.WriteLine((++curClm).ToString() + "\t" +
                                            c.ONLclimber.surname + " " + c.ONLclimber.name + "\t" +
                                            c.ONLclimber.age.ToString() + "\t" +
                                            c.qf + "\t" +
                                            (c.state == Constants.CLIMBER_PENDING_DELETE ? "Удаление" : (c.ONLClimberCompLink1.Count == 0 ? "Добавление" : "Правка"))
                                            );
                                }
                                else
                                    swr.WriteLine("Участники в данную группу не заявлены");
                                swr.WriteLine();
                                swr.WriteLine();
                            }
                        }
                        else
                            swr.WriteLine("УЧАСТНИКИ НЕ ЗАЯВЛЕНЫ");
                    }
                    string sTmp;
                    MailService ms = new MailService(dc, compID);
                    return ms.SendMail(email, subj, body, MailPriority.Normal, fileName, mStr, out sTmp);
                }
                finally { swr.Close(); }
            }
            finally { mStr.Close(); }
        }
    }
}