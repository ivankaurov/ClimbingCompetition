using ClimbingEntities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace WebClimbing2.Controllers
{
    public abstract class __BaseController : Controller
    {
        public __BaseController() { climbingContext = new Lazy<ClimbingContext2>(() => LoadContext()); }

        Lazy<ClimbingContext2> climbingContext;

        private ClimbingContext2 LoadContext()
        {
            string ip, machine, user;
            IPAddress ipA;
            GetHttpContextData(out ip, out machine, out user);

            if (!IPAddress.TryParse(ip, out ipA))
                ipA = null;

            return ClimbingContext2.CreateContextForBrowser(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["RemoteDbConnection"].ConnectionString,
                                                            user,
                                                            ipA,
                                                            machine);
        }
        private void GetHttpContextData(out String ip, out String machine, out String user)
        {
            if (Request == null)
            {
                ip = IPAddress.Loopback.ToString();
                machine = System.Net.Dns.GetHostName();
            }
            else
            {
                ip = Request.UserHostAddress;
                machine = Request.UserHostName;
            }
            if (HttpContext == null)
                user = null;
            else
                try { user = HttpContext.User.Identity.IsAuthenticated ? HttpContext.User.Identity.Name : null; }
                catch (NullReferenceException) { user = null; }
        }

        public ClimbingContext2 Context { get { return climbingContext.Value; } }

        protected override void Dispose(bool disposing)
        {
            if (disposing && climbingContext.IsValueCreated)
                climbingContext.Value.Dispose();
            base.Dispose(disposing);
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            var lang = requestContext.RouteData.Values["lang"] as String;
            if (lang == null && requestContext.HttpContext != null && requestContext.HttpContext.Request != null)
                lang = requestContext.HttpContext.Request.Params["lang"];
            if (lang != null)
                try { Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(lang); }
                catch (CultureNotFoundException) { }
            else
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("ru-RU");

            base.Initialize(requestContext);

            if (climbingContext.IsValueCreated)
            {
                climbingContext.Value.Dispose();
                climbingContext = new Lazy<ClimbingContext2>(() => LoadContext());
            }
        }
    }
}