using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace WebClimbing.src
{
    public class BaseControl : System.Web.UI.UserControl
    {
        private SqlConnection _cn = null;
        protected SqlConnection cn
        {
            get
            {
                if (this.BasePage != null)
                    return this.BasePage.cn;
                if (_cn == null)
                    _cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["db"].ConnectionString);
                if (_cn.State != System.Data.ConnectionState.Open)
                    _cn.Open();
                return _cn;
            }
        }

        private Entities _dc = null;
        public Entities dc
        {
            get
            {
                if (this.BasePage != null)
                    return this.BasePage.dc;
                if (_dc == null)
                    _dc = new Entities(WebConfigurationManager.ConnectionStrings["db_Entities"].ConnectionString);
                if (_dc.Connection.State != System.Data.ConnectionState.Open)
                    _dc.Connection.Open();
                return _dc;
            }
        }

        protected virtual void Page_Load(object sender, EventArgs e)
        {
            _compID = this.GetCompID();
        }

        private long _compID = -1;
        public long compID
        {
            get
            {
                if (this.BasePage != null)
                    return this.BasePage.compID;
                return _compID;
            }
        }

        ~BaseControl()
        {
            try
            {
                if (_cn != null && _cn.State != System.Data.ConnectionState.Closed)
                    _cn.Close();
            }
            catch { }
            try
            {
                if (_dc != null && _dc.Connection != null && _dc.Connection.State != System.Data.ConnectionState.Open)
                    _dc.Connection.Close();
            }
            catch { }
            try { Dispose(); }
            catch { }
        }

        public BasePage BasePage { get { return this.Page as BasePage; } }
    }
}