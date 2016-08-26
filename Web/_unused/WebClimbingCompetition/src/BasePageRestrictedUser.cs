using System;
using ClimbingCompetition.Online;

namespace WebClimbing.src
{
    public abstract class BasePageRestricted : BasePage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!AuthenticateUser())
                Response.Redirect("~/no_access.aspx");
        }

        protected abstract bool AuthenticateUser();
    }

    

    public class BasePageRestrictedUser : BasePageRestricted
    {
        protected override bool AuthenticateUser()
        {
            return (User.Identity.IsAuthenticated(compID));
        }
    }

    public class BasePageRestrictedAdminComp : BasePageRestricted
    {
        protected override bool AuthenticateUser()
        {
            return (User.IsInRole(Constants.ROLE_ADMIN, compID));
        }
    }
}