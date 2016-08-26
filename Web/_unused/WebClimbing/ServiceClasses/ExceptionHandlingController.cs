using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebClimbing.Models;
using WebClimbing.Models.UserAuthentication;
using System.Threading.Tasks;
using System.Text;

namespace WebClimbing.ServiceClasses
{
    public class ExceptionLoggerAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (!(filterContext.ExceptionHandled || (filterContext.Exception is System.Web.Http.HttpResponseException)))
            {
                try
                {
                    DateTime dt = DateTime.UtcNow;
                    StringBuilder exData = new StringBuilder();
                    exData.AppendFormat("Date:           {0}{1}", dt.ToShortDateString(), Environment.NewLine);
                    exData.AppendFormat("Time:           {0}{1}", dt.ToLongTimeString(), Environment.NewLine);
                    exData.AppendFormat("Controller:     {0}{1}", filterContext.Controller.GetType(), Environment.NewLine);
                    exData.AppendFormat("IsChildAction:  {0}{1}", filterContext.IsChildAction, Environment.NewLine);
                    exData.AppendFormat("Exception Data: {1}{0}", GetExceptionData(filterContext.Exception, 0), Environment.NewLine);

                    ClimbingContext db = new ClimbingContext();
                    IEnumerable<String> emailList;
                    try { emailList = db.UserRoles.Where(r => r.CompID == null && r.RegionID == null && r.RoleId >= (int)RoleEnum.Admin).Select(r => r.User.Email ?? String.Empty).ToList().Distinct(); }
                    catch { emailList = new List<string> { "ivan.kaurov@gmail.com" }; }
                    Parallel.ForEach(emailList.Where(e => !String.IsNullOrEmpty(e)),
                        u => MailService.SendMessage(u, "Unhandled Exception",
                            exData.ToString(), true));
                }
                catch { }
            }
            base.OnException(filterContext);
        }

        private static String GetExceptionData(Exception ex, int level)
        {
            StringBuilder exceptionData = new StringBuilder();
            StringBuilder sPrefixBuilder = new StringBuilder();
            for (int i = 0; i < level; i++)
                sPrefixBuilder.Append("  ");
            String sPrefix = sPrefixBuilder.ToString();
            exceptionData.AppendFormat("{2}Exception:      {0}{1}", ex.GetType(), Environment.NewLine, sPrefix);
            exceptionData.AppendFormat("{2}Message:        {0}{1}", ex.Message, Environment.NewLine, sPrefix);
            exceptionData.AppendFormat("{1}Stack Trace:    {0}", ex.StackTrace, sPrefix);
            if (ex.InnerException != null)
                exceptionData.AppendFormat("{0}{2}InnerException:{0}{1}", Environment.NewLine, GetExceptionData(ex.InnerException, level + 1), sPrefix);
            return exceptionData.ToString();
        }
    }
}