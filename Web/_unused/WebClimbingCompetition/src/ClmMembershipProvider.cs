using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Configuration;
using System.Web.Security;

namespace WebClimbing.src
{
    public class ClmMembershipProvider : MembershipProvider
    {
        public const string PROVIDER_NAME = "ClmMembershipProvider";
        private string applicationName;
        public override string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        private string connectionString;
        public string ConnectionString { get { return connectionString; } }

        private Entities _dc = null;
        public Entities dc
        {
            get
            {
                if (_dc == null)
                    _dc = new Entities(WebConfigurationManager.ConnectionStrings["db_Entities"].ConnectionString);
                if (_dc.Connection.State != ConnectionState.Open)
                    _dc.Connection.Open();
                return _dc;
            }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            if (String.IsNullOrEmpty(name))
                name = PROVIDER_NAME;
            if (config == null)
                throw new ArgumentNullException("config");
            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "ClimbingCompetition membership provider");
            }

            base.Initialize(name, config);
            connectionString = WebConfigurationManager.ConnectionStrings["db"].ConnectionString;
            //connectionString = ConfigurationManager.ConnectionStrings[config["connectionString"]].ConnectionString;

        }

        private ONLuser getONLUserByIid(string iid)
        {
            var usrList = from u in dc.ONLusers
                          where u.iid == iid
                          select u;
            if (usrList.Count() < 1)
                return null;
            return usrList.First();
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (!ValidateUser(username, oldPassword))
                return false;
            ValidatePasswordEventArgs e = new ValidatePasswordEventArgs(username, newPassword, true);
            OnValidatingPassword(e);
            if (e.Cancel)
                if (e.FailureInformation == null)
                    throw new MembershipPasswordException("Ошибка смены пароля.");
                else
                    throw e.FailureInformation;
            try
            {
                var u = getONLUserByIid(username);
                u.password = newPassword;
                dc.SaveChanges();
                try
                {
                    MembershipUser mUsr = GetUser(username, false);
                    if (mUsr is ClmUser)
                    {
                        string eMail = ((ClmUser)mUsr).Usr.email;
                        string erM;
                        if (!String.IsNullOrEmpty(eMail))
                        {
                            MailService ms = new MailService(dc, -1);
                            ms.SendMail(eMail, "Смена пароля",
                                "Ваш пароль в системе подачи заявок на соренования по скалолазанию " +
                                "был изменён на " + newPassword,
                                 System.Net.Mail.MailPriority.High, out erM);
                        }
                    }
                }
                catch { }
                return true;
            }
            catch (Exception ex)
            {
                throw new MembershipPasswordException("Ошибка смены пароля: " +
                    ex.Message);
            }
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }


        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            int nTmp;
            int? teamId;
            if (int.TryParse(passwordQuestion, out nTmp))
                teamId = nTmp;
            else
                teamId = null;
            return CreateUser(username, password, email, teamId, null, isApproved, providerUserKey, out status);
        }

        public MembershipUser CreateUser(string username, string password, string email, int? teamID, string chief, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            ValidatePasswordEventArgs e = new ValidatePasswordEventArgs(username, password, true);
            OnValidatingPassword(e);
            if (e.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            if (RequiresUniqueEmail && !String.IsNullOrEmpty(GetUserNameByEmail(email)))
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }
            
            if (!(providerUserKey is string))
            {
                status = MembershipCreateStatus.InvalidProviderUserKey;
                return null;
            }
            string newIid = (string)providerUserKey;
            if (newIid.Length > 3)
                newIid = newIid.Substring(0, 3);
            if (GetUser(newIid, false) != null)
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }
            try
            {
                ONLuser u = ONLuser.CreateONLuser(newIid, username, password, String.Empty);
                u.team_id = teamID;
                u.email = email;
                u.chief = chief;
                u.creationDate = DateTime.UtcNow;
                dc.ONLusers.AddObject(u);
                dc.SaveChanges();
                
                status = MembershipCreateStatus.Success;
                try
                {
                    if (!String.IsNullOrEmpty(email))
                    {
                        string str;
                        MailService ms = new MailService(dc, -1);
                        ms.SendMail(email, "Учетная запись",
                            "Создана учётная запись в системе подачи заявок на соревнования по скалолазанию.\r\n" +
                            "Ваш регион: " + username + "\r\n" +
                            "Ваш пароль: " + password + "\r\n\r\n" +
                            "С Уважением, Администратор системы.",
                             System.Net.Mail.MailPriority.Normal,
                             out str);
                    }
                }
                catch { }
                return GetUser(newIid, false);
            }
            catch (Exception)
            {
                status = MembershipCreateStatus.ProviderError;
                return null;
            }
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            ONLuser u = getONLUserByIid(username);
            if (u == null)
                return false;
            dc.ONLusers.DeleteObject(u);
            dc.SaveChanges();
            return true;
        }

        private bool enablePasswordReset = true;
        public override bool EnablePasswordReset { get { return enablePasswordReset; } }

        private bool enablePasswordRetrieval = true;
        public override bool EnablePasswordRetrieval { get { return enablePasswordRetrieval; } }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection res = new MembershipUserCollection();
            var userC = from u in dc.ONLusers
                        orderby u.iid ascending
                        select u;
            totalRecords = userC.Count();
            int recCnt = 0;
            int stIndex = pageSize * pageIndex;
            int endIndex = pageSize * (pageIndex + 1) - 1;
            foreach (ONLuser usr in userC)
            {
                if (recCnt >= stIndex)
                    res.Add(new ClmUser(usr, Name));
                if (recCnt >= endIndex)
                    break;
                recCnt++;
            }
            return res;
        }

        public override int GetNumberOfUsersOnline()
        {
            return 0;
        }

        public override string GetPassword(string username, string answer)
        {
            try
            {
                var pwd = from u in dc.ONLusers
                          where u.iid == username
                          select u.password;
                return pwd.First();
            }
            catch (Exception ex) { throw new ProviderException(ex.Message); }
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            var usrC = from u in dc.ONLusers
                       where u.iid == username
                       select u;
            if (usrC.Count() < 1)
                return null;
            ONLuser usr = usrC.First();
            return new ClmUser(usr, Name);
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (providerUserKey is string)
                return GetUser((string)providerUserKey, userIsOnline);
            else
                return null;
        }

        public override string GetUserNameByEmail(string email)
        {
            var uIdC = from u in dc.ONLusers
                       where u.email == email
                       select u.iid;
            if (uIdC.Count() < 1)
                return null;
            return uIdC.First();
        }

        private int maxInvalidPasswordAttempts = int.MaxValue;
        public override int MaxInvalidPasswordAttempts { get { return maxInvalidPasswordAttempts; } }

        private int minRequiredNonAlphanumericCharacters = 0;
        public override int MinRequiredNonAlphanumericCharacters { get { return minRequiredNonAlphanumericCharacters; } }

        private int minRequiredPasswordLength = 8;
        public override int MinRequiredPasswordLength { get { return minRequiredPasswordLength; } }

        private int passwordAttemptWindow = 0;
        public override int PasswordAttemptWindow { get { return passwordAttemptWindow; } }

        private MembershipPasswordFormat passwordFormat = MembershipPasswordFormat.Clear;
        public override MembershipPasswordFormat PasswordFormat { get { return passwordFormat; } }

        private string passwordStrengthRegularExpression = "";
        public override string PasswordStrengthRegularExpression { get { return passwordStrengthRegularExpression; } }

        private bool requiresQuestionAndAnswer = false;
        public override bool RequiresQuestionAndAnswer { get { return requiresQuestionAndAnswer; } }

        private bool requiresUniqueEmail = false;
        public override bool RequiresUniqueEmail { get { return requiresUniqueEmail; } }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            return true;
        }

        public override void UpdateUser(MembershipUser user)
        {
            if (user is ClmUser)
            {
                ClmUser cUser = (ClmUser)user;
                var u = getONLUserByIid(cUser.UserName);
                if (u == null)
                    return;
                u.name = cUser.Usr.name;
                u.password = cUser.Usr.password;
                u.team_id = cUser.Usr.team_id;
                u.email = cUser.Email;
                u.chief = cUser.Usr.chief;
                dc.SaveChanges();
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            var pL = from u in dc.ONLusers
                     where u.iid == username
                     select u.password;
            return (pL.Count() < 1 ? false : pL.First().Equals(password));
        }
    }

    public sealed class ClmUser : MembershipUser
    {
        private ONLuser usr;
        public ONLuser Usr { get { return usr; } set { usr = value; } }
        public ClmUser(ONLuser usr, string provider) :
            base(provider, usr.iid, usr.iid, usr.email,
             "", "", true, false, DateTime.MinValue, DateTime.MinValue,
            DateTime.MinValue, DateTime.MinValue, DateTime.MinValue)
        {
            this.usr = usr;
        }
    }
}
