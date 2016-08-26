using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace WebClimbing.Models.UserAuthentication
{
    public class ClimbingMembershipProvider : WebMatrix.WebData.ExtendedMembershipProvider
    {
        //ClimbingContext db = new ClimbingContext();

        public override bool ConfirmAccount(string accountConfirmationToken)
        {
            return ConfirmAccount(null, accountConfirmationToken);
        }

        public override bool ConfirmAccount(string userName, string accountConfirmationToken)
        {
            using (var db = new ClimbingContext())
            {
                UserProfileModel usr;
                if (String.IsNullOrEmpty(userName))
                    usr = db.UserProfiles.Find(GetUserIdFromPasswordResetToken(accountConfirmationToken));
                else
                    usr = db.UserProfiles.FirstOrDefault(u => u.Name.Equals(userName, StringComparison.OrdinalIgnoreCase));
                if (usr == null)
                    return false;
                usr.Inactive = false;
                usr.Token = String.Empty;
                usr.PasswordTokenExpirationTime = null;
                db.SaveChanges();
                return true;
            }
        }

        public override string CreateAccount(string userName, string password, bool requireConfirmationToken)
        {
            return CreateUserAndAccount(userName, password, requireConfirmationToken);
        }

        public override string CreateUserAndAccount(string userName, string password, bool requireConfirmation, IDictionary<string, object> values)
        {
            if (String.IsNullOrEmpty(password))
                password = Membership.GeneratePassword(this.minRequiredPasswordLength + 10, this.minRequiredNonAlphanumericCharacters + 1);
            else
            {
                ValidatePasswordEventArgs e = new ValidatePasswordEventArgs(userName, password, true);
                OnValidatingPassword(e);
                if (e.Cancel)
                    return null;
            }
            using (var db = new ClimbingContext())
            {
                var usr = db.UserProfiles.Create();
                usr.Name = userName;
                usr.SetPassword(password);
                usr.Inactive = true;

                if (requireConfirmation)
                {
                    string token;
                    do
                    {
                        token = GenerateToken();
                    } while (db.UserProfiles.Count(u => u.Token.Equals(token, StringComparison.OrdinalIgnoreCase)) > 0);
                    usr.Token = token;
                }
                else
                    usr.Token = String.Empty;
                db.UserProfiles.Add(usr);
                db.SaveChanges();
                return usr.Token;
            }
        }

        public override bool DeleteAccount(string userName)
        {
            using (var db = new ClimbingContext())
            {
                var usr = db.UserProfiles.SingleOrDefault(u => u.Name.Equals(userName, StringComparison.OrdinalIgnoreCase));
                if (usr == null)
                    return false;
                if (usr.Roles.Count(r => r.RoleId == (int)RoleEnum.Admin) > 0 &&
                    db.UserRoles.Count(ur => ur.UserId != usr.Iid && ur.RoleId == (int)RoleEnum.Admin) < 1)
                    //throw new ArgumentException(String.Format("Can\'t delete the last administrator {0}", userName));
                    return false;
                db.UserProfiles.Remove(usr);
                db.SaveChanges();
                return true;
            }
        }

        private string GenerateToken()
        {
            Random rand = new Random(unchecked((int)DateTime.Now.Ticks));
            char[] buffer = new char[UserProfileModel.TOKEN_LENGTH];
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = (char)rand.Next((int)'a', (int)'z');
            return new String(buffer);
        }

        public override string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow)
        {
            using (var db = new ClimbingContext())
            {
                var usr = db.UserProfiles.SingleOrDefault(u => u.Name.Equals(userName, StringComparison.OrdinalIgnoreCase));
                if (usr == null)
                    return null;
                string token;
                do
                {
                    token = GenerateToken();
                } while (db.UserProfiles.Count(u => u.Token.Equals(token, StringComparison.OrdinalIgnoreCase)) > 0);
                usr.Token = token;
                usr.PasswordTokenExpirationTime = DateTime.UtcNow.AddMinutes(tokenExpirationInMinutesFromNow);
                db.SaveChanges();
                return token;
            }
        }
        #region Unimpl
        public override ICollection<WebMatrix.WebData.OAuthAccountData> GetAccountsForUser(string userName)
        {
            return new List<WebMatrix.WebData.OAuthAccountData>();
        }

        public override DateTime GetCreateDate(string userName)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetLastPasswordFailureDate(string userName)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetPasswordChangedDate(string userName)
        {
            throw new NotImplementedException();
        }

        public override int GetPasswordFailuresSinceLastSuccess(string userName)
        {
            throw new NotImplementedException();
        }
        #endregion

        public override int GetUserIdFromPasswordResetToken(string token)
        {
            using (var db = new ClimbingContext())
            {
                var user = db.UserProfiles.FirstOrDefault(u => u.Token.Equals(token, StringComparison.OrdinalIgnoreCase));
                if (user == null)
                    return -1;
                else
                    return user.Iid;
            }
        }

        public override bool IsConfirmed(string userName)
        {
            return !(new ClimbingContext()).UserProfiles.First(u => u.Name.Equals(userName, StringComparison.OrdinalIgnoreCase)).Inactive;
        }

        public override bool ResetPasswordWithToken(string token, string newPassword)
        {
            using (var db = new ClimbingContext())
            {
                var usr = db.UserProfiles.Find(GetUserIdFromPasswordResetToken(token));
                if (usr == null)
                    return false;
                if (usr.PasswordTokenExpirationTime != null && usr.PasswordTokenExpirationTime.Value < DateTime.UtcNow)
                    return false;
                ValidatePasswordEventArgs e = new ValidatePasswordEventArgs(usr.Name, newPassword, false);
                OnValidatingPassword(e);
                if (e.Cancel)
                    return false;
                usr.Token = null;
                usr.PasswordTokenExpirationTime = null;
                usr.SetPassword(newPassword);
                usr.Inactive = false;
                db.SaveChanges();
                return true;
            }
        }

        public override string ApplicationName { get { return "WebClimbing"; } set { } }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (!ValidateUser(username, oldPassword))
                return false;
            ValidatePasswordEventArgs e = new ValidatePasswordEventArgs(username, newPassword, false);
            OnValidatingPassword(e);
            if (e.Cancel)
                return false;
            using (var db = new ClimbingContext())
            {
                var usr = db.UserProfiles.First(u => u.Name.Equals(username, StringComparison.OrdinalIgnoreCase));

                usr.SetPassword(newPassword);
                db.SaveChanges();
                return true;
            }
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out System.Web.Security.MembershipCreateStatus status)
        {
            bool createInactive = true;
            using (var db = new ClimbingContext())
            {
                if (db.UserProfiles.Count(u => u.Name.Equals(username, StringComparison.OrdinalIgnoreCase)) > 0)
                {
                    status = MembershipCreateStatus.DuplicateUserName;
                    return null;
                }
                if (String.IsNullOrEmpty(password))
                    password = Membership.GeneratePassword(minRequiredPasswordLength + 10, minRequiredNonAlphanumericCharacters + 1);
                else
                {
                    ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, password, true);
                    OnValidatingPassword(args);
                    if (args.Cancel)
                    {
                        status = MembershipCreateStatus.InvalidPassword;
                        return null;
                    }
                }
                if (String.IsNullOrEmpty(email))
                {
                    status = MembershipCreateStatus.InvalidEmail;
                    return null;
                }
                if (requiresUniqueEmail && db.UserProfiles.Count(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)) > 0)
                {
                    status = MembershipCreateStatus.DuplicateEmail;
                    return null;
                }
                try
                {
                    string token = null;
                    if (createInactive)
                    {
                        do
                        {
                            token = GenerateToken();
                        }
                        while (db.UserProfiles.Count(p => p.Token.Equals(token, StringComparison.InvariantCultureIgnoreCase)) > 0);
                    }
                    var upm = db.UserProfiles.Create();
                    upm.Name = username;
                    upm.SetPassword(password);
                    upm.Email = email;
                    upm.Inactive = createInactive;
                    upm.Token = token;
                    upm.PasswordTokenExpirationTime = null;
                    db.UserProfiles.Add(upm);
                    db.SaveChanges();
                    status = MembershipCreateStatus.Success;
                    return GetUser(username, false);
                }
                catch
                {
                    status = MembershipCreateStatus.ProviderError;
                    return null;
                }
            }
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            return DeleteAccount(username);
        }

        private string GetConfigValue(string configValue, string defaultValue)
        {
            return String.IsNullOrEmpty(configValue) ? defaultValue : configValue;
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");
            if (String.IsNullOrEmpty(name))
                name = this.GetType().Name;
            base.Initialize(name, config);

            enablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            enablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "false"));
            maxInvalidPasswordAttempts = Int32.Parse(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            minRequiredNonAlphanumericCharacters = Int32.Parse(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "0"));
            minRequiredPasswordLength = Int32.Parse(GetConfigValue(config["minRequiredPasswordLength"], "1"));
            passwordAttemptWindow = Int32.Parse(GetConfigValue(config["passwordAttemptWindow"], "10"));
            requiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "false"));
        }

        private bool enablePasswordReset = true;
        public override bool EnablePasswordReset { get { return enablePasswordReset; } }
        private bool enablePasswordRetrieval = false;
        public override bool EnablePasswordRetrieval { get { return enablePasswordRetrieval; } }
        private int maxInvalidPasswordAttempts =int.MaxValue;
        public override int MaxInvalidPasswordAttempts { get { return maxInvalidPasswordAttempts; } }
        private int minRequiredNonAlphanumericCharacters = 0;
        public override int MinRequiredNonAlphanumericCharacters { get { return minRequiredNonAlphanumericCharacters; } }
        private int minRequiredPasswordLength = 1;
        public override int MinRequiredPasswordLength { get { return minRequiredPasswordLength; } }
        private int passwordAttemptWindow = 0;
        public override int PasswordAttemptWindow { get { return passwordAttemptWindow; } }
        private bool requiresUniqueEmail = false;
        public override bool RequiresUniqueEmail { get { return requiresUniqueEmail; } }

        #region Unimplemented
        public override System.Web.Security.MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override System.Web.Security.MembershipUser GetUser(string username, bool userIsOnline)
        {
            using (var db = new ClimbingContext())
            {
                var usr = db.UserProfiles.FirstOrDefault(u => u.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
                if (usr == null)
                    return null;
                return new MembershipUser("ClimbingMembershipProvider",
                    username, usr.Iid, usr.Email, null, null, !usr.Inactive, usr.Inactive,
                    new DateTime(), new DateTime(), new DateTime(), new DateTime(), new DateTime());
            }
            //return GetUser(usr.UserId, userIsOnline);
        }

        public override System.Web.Security.MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            using (var db = new ClimbingContext())
            {
                var db_usr = db.UserProfiles.Find(providerUserKey);
                if (db_usr == null)
                    return null;
                MembershipUser mu = new MembershipUser("ClimbingMembershipProvider",
                    db_usr.Name, db_usr.Iid, db_usr.Email, null, null, !db_usr.Inactive, db_usr.Inactive,
                    new DateTime(), new DateTime(), new DateTime(), new DateTime(), new DateTime());
                return mu;
            }
        }

        public override System.Web.Security.MembershipPasswordFormat PasswordFormat { get { return MembershipPasswordFormat.Hashed; } }

        public override string PasswordStrengthRegularExpression { get { return String.Empty; } }

        public override bool RequiresQuestionAndAnswer { get { return false; } }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(System.Web.Security.MembershipUser user)
        {
            using (var db = new ClimbingContext())
            {
                var u = db.UserProfiles.Find(user.ProviderUserKey);
                if (u == null)
                    return;
                /*if (db.UserProfiles.Count(usr => usr.Iid != u.Iid && usr.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)) > 0)
                    throw new MembershipCreateUserException(MembershipCreateStatus.DuplicateEmail);*/
                u.Email = user.Email;
                u.Inactive = user.IsLockedOut;
                db.SaveChanges();
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            using (var db = new ClimbingContext())
            {
                var usr = db.UserProfiles.FirstOrDefault(u => u.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
                if (usr == null || usr.Inactive)
                    return false;
                return usr.CheckPassword(password);
            }
        }

        public override string GetUserNameByEmail(string email)
        {
            return (new ClimbingContext()).UserProfiles.First(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)).Name;
        }

        public override bool HasLocalAccount(int userId)
        {
            using (var db = new ClimbingContext())
            {
                var u = db.UserProfiles.Find(userId);
                return (u != null);
            }
        }
    }
}