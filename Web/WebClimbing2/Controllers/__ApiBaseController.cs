using ClimbingCompetition.Common;
using ClimbingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace WebClimbing2.Controllers
{
    public abstract class __ApiBaseController : ApiController
    {
        public __ApiBaseController() { climbingContext = new Lazy<ClimbingContext2>(() => LoadContext()); }

        Lazy<ClimbingContext2> climbingContext;

        private ClimbingContext2 LoadContext()
        {
            return ClimbingContext2.CreateContextForBrowser(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["RemoteDbConnection"].ConnectionString,
                                                            null,
                                                            null,
                                                            "API_CALL");
        }

        public ClimbingContext2 Context { get { return climbingContext.Value; } }

        protected bool CheckCompetitionsPassword(string compId, string password)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(compId))
                return false;
            var p = Context.CompetitionParameters.FirstOrDefault(pr => pr.CompId == compId && pr.ParamIdString == CompetitionParamId.UpdatePassword.ToString());
            if (p == null)
                return false;
            return string.Equals(p.StringValue, password, StringComparison.Ordinal);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && climbingContext.IsValueCreated)
                climbingContext.Value.Dispose();
            base.Dispose(disposing);
        }
    }
}