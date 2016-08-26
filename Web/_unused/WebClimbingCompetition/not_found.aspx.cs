using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using WebClimbing.src;

namespace WebClimbing
{
    public partial class _PNotFound : BasePage
    {
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string GetTimeSpan()
        {
            return _MPWebAppl.GetTimeSpan();
        }
        

        protected override void Page_Load(object sender, EventArgs e)
        {
            this.NoRedirecting = true;
            base.Page_Load(sender, e);
        }
    }
}