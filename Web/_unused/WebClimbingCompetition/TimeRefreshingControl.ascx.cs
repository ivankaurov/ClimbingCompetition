using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace WebClimbing
{
    public partial class _CTimeRefreshingControl : System.Web.UI.UserControl,IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public int Time
        {
            get { return ParseLabel(Label1.Text); }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be greater than zero");
                Label1.Text = "Автоматическое обновление через " + value.ToString() + " секунд";
            }
        }

        public bool TimerEnabled
        {
            get { return Timer1.Enabled; }
            set
            {
                Timer1.Enabled = value;
                if (value)
                    Label1.Text = "Автоматическое обновление через 60 секунд";
                else
                    Label1.Text = "";
                this.Visible = value;
            }
        }

        static int ParseLabel(string str)
        {
            int i1 = str.IndexOf("через");
            int i2 = str.IndexOf("сек");
            if (i1 > 0 && i2 > 0)
            {
                int l = i2 - i1 - 7;
                string strr = str.Substring(i1 + 6, l);
                return int.Parse(strr);
            }
            else
                return -1;
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            int tm = ParseLabel(Label1.Text);
            tm -= 10;
            if (tm <= 0)
            {
                if(RefreshListEvent!= null)
                    RefreshListEvent(this, new EventArgs());
                tm = 180;
            }
            Label1.Text = "Автоматическое обновление через " + tm.ToString() + " секунд";
        }

        public delegate void RefreshListEventHandler(object sender, EventArgs e);
        public event RefreshListEventHandler RefreshListEvent;

        #region Члены IPostBackEventHandler

        public void RaisePostBackEvent(string eventArgument)
        {
            if (RefreshListEvent != null)
                RefreshListEvent(null, new EventArgs());
        }

        #endregion
    }
}