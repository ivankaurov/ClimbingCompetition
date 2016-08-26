using System.Web;
using System.Web.Mvc;
using WebClimbing.ServiceClasses;

namespace WebClimbing
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new ExceptionLoggerAttribute());
        }
    }
}