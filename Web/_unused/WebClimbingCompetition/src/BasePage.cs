using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web.Configuration;
using ClimbingCompetition.Online;

namespace WebClimbing.src
{
    public class BasePage : System.Web.UI.Page
    {
        private static SqlConnection _staticCN = null;
        private static Mutex _staticCnMutex = new Mutex();

        protected static Entities dcStatic { get { return ExtensionMethods.dc; } }

        protected static SqlConnection staticCn
        {
            get
            {
                _staticCnMutex.WaitOne();
                try
                {
                    if(_staticCN == null)
                        _staticCN = new SqlConnection(WebConfigurationManager.ConnectionStrings["db"].ConnectionString);
                    if (_staticCN.State != ConnectionState.Open)
                        _staticCN.Open();
                    return _staticCN;
                }
                finally { _staticCnMutex.ReleaseMutex(); }
            }
        }

        private ONLCompetition _cCur = null;

        public ONLCompetition CurrentCompetition
        {
            get
            {
                if (compID < 1)
                    return null;
                if (_cCur == null)
                {
                    try { _cCur = dc.ONLCompetitions.First(c => c.iid == compID); }
                    catch { _cCur = null; }
                }
                return _cCur;
            }
        }

        public string CompetitionName
        {
            get { return CurrentCompetition == null ? "соревнования по скалолазанию" : CurrentCompetition.name; }
        }

        protected ONLuser GetUserByIid(string iid)
        {
            try { return dc.ONLusers.First(q => q.iid == iid); }
            catch { return null; }
        }

        private bool _noRedirecting = false;
        protected bool NoRedirecting
        {
            get { return _noRedirecting; }
            set { _noRedirecting = value; }
        }

        public void SetCompID(long newCompID)
        {
            this.compID = newCompID;
            try
            {
                try { Session[Constants.PARAM_COMP_ID] = _compID; }
                catch { Session.Add(Constants.PARAM_COMP_ID, _compID); }
            }
            catch { }
        }

        private long _compID = -1;
        public long compID
        {
            get { return _compID; }
            set
            {
                _compID = value;
                _cCur = null;
                _ms = new MailService(dc, _compID);
            }
        }
        private SqlConnection _cn = null;
        public SqlConnection cn
        {
            get
            {
                if (_cn == null)
                    _cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["db"].ConnectionString);
                if (_cn.State != ConnectionState.Open)
                    _cn.Open();
                return _cn;
            }
        }

        private WebClimbing.Entities _dc = null;
        public WebClimbing.Entities dc
        {
            get
            {
                if (_dc == null)
                    _dc = new Entities(WebConfigurationManager.ConnectionStrings["db_Entities"].ConnectionString);
                if (_dc.Connection.State != ConnectionState.Open)
                    _dc.Connection.Open();
                return _dc;
            }
        }

        private static WebClimbing.Entities _dcStatic = null;
        private static Mutex _dcStaticMutex = new Mutex();
        protected static WebClimbing.Entities staticDc
        {
            get
            {
                _dcStaticMutex.WaitOne();
                try
                {
                    if (_dcStatic == null)
                        _dcStatic = new Entities(WebConfigurationManager.ConnectionStrings["db_Entities"].ConnectionString);
                    if (_dcStatic.Connection.State != ConnectionState.Open)
                        _dcStatic.Connection.Open();
                    return _dcStatic;
                }
                finally { _dcStaticMutex.ReleaseMutex(); }
            }
        }

        private MailService _ms = null;
        public MailService mailService
        {
            get
            {
                if (_ms == null)
                    _ms = new MailService(dc, compID);
                return _ms;
            }
        }

        protected virtual void Page_Load(object sender, EventArgs e)
        {
            Page_Load(sender, e, true);
        }

        protected void Page_Load(object sender, EventArgs e, bool loadCompID)
        {
            if (loadCompID)
                compID = this.GetCompID();
            if (compID < 0 && !_noRedirecting)
                Response.Redirect(ExtensionMethods.DEFAULT_REDIRECT);
            if (!IsPostBack)
            {
                try
                {
                    if (CurrentCompetition != null)
                        Page.Title = CurrentCompetition.name;
                }
                catch { }
            }
        }

        protected bool CheckDeadline(bool modifications)
        {
            if (CurrentCompetition == null)
                return false;
            if (User.IsInRole(Constants.ROLE_ADMIN, compID))
                return (DateTime.Now < CurrentCompetition.GetDateParam(Constants.PDB_COMP_START_DATE));
            string paramToCheck = modifications ? Constants.PDB_COMP_DEADLINE_CHANGE : Constants.PDB_COMP_DEADLINE;
            DateTime lastPossible = CurrentCompetition.GetDateParam(paramToCheck).AddDays(1.0);
            return (DateTime.Now < lastPossible);
        }

        ~BasePage()
        {
            try
            {
                if (_cn != null && _cn.State != ConnectionState.Closed)
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
    }
}