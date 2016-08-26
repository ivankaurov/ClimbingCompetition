using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using ClimbingCompetition;
using WebClimbing.src;

namespace WebClimbing.Apps
{
    public partial class _PChangePassword : System.Web.UI.Page
    {
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string GetTimeSpan()
        {
            return _MPWebAppl.GetTimeSpan();
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnChange_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if(!Page.IsValid)
                return;
            if (!PasswordWorkingClass.CheckStrength(newPwd.Text))
            {
                ErrorMessage.Text = "Пароль не соответствует требованиям безопасности (не менее 8 символов, не менее 1 строчной буквы, прописной буквы и цифры)";
                return;
            }
            try
            {
                string userId = User.Identity.Name;
                MembershipUser mUsr = Membership.GetUser(userId);
                if (!(mUsr is ClmUser))
                {
                    ErrorMessage.Text = "Ошибка определения пользователя";
                    return;
                }
                ClmUser usr = (ClmUser)mUsr;
                if (usr.ChangePassword(oldPwd.Text, newPwd.Text))
                    ErrorMessage.Text = "Пароль изменён успешно";
                else
                    ErrorMessage.Text = "Ошибка смены пароля";
            }
            catch { ErrorMessage.Text = "Ошибка смены пароля"; }
        }
    }
}
