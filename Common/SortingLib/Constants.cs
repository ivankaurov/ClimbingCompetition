using System;
using System.Collections.Generic;
using System.Web;

namespace ClimbingCompetition.Online
{
    public static class Constants
    {
        public const string ROLE_ADMIN = "ADM";
        public const string ROLE_USER = "USR";
        public const string ROLE_ADMIN_ROOT = "ADB";

        public const string PARAM_NO_REDIR = "no_redirect";
        public const string PARAM_IID = "iid";
        public const string PARAM_COMP_ID = "comp_id";
        public const string PARAM_SECRETARY_ID = "secretary_id";

        public const string PDB_COMP_START_DATE = "StartDate";
        public const string PDB_COMP_END_DATE = "EndDate";
        public const string PDB_COMP_DEADLINE = "Deadline";
        public const string PDB_COMP_DEADLINE_CHANGE = "DeadlineChange";
        public const string PDB_COMP_STYLES = "Styles";
        public const string PDB_COMP_ALLOW_AFTER_DEADLINE = "AllowAfterDeadline";
        public const string PDB_COMP_RULES = "CompetitionRules";
        public const string PDB_COMP_VIDEO = "Video";
        public const string PDB_COMP_ADD_APPL_INFO = "AdditionalApplInfo";

        public const string PDB_COMP_BIN_POLOGENIE = "Pologenie";
        public const string PDB_COMP_BIN_POLOGENIE_TITUL = "TitulPologeniya";
        public const string PDB_COMP_BIN_REGLAMENT = "Regalment";
        public const string PDB_COMP_BIN_REGLAMENT_TITUL = "TitulReglamenta";
        public const string PDB_COMP_BIN_RASP = "Timetable";
        public const string PDB_COMP_BIN_ACCOMODATION = "Accomodation";
        public const string PDB_COMP_BIN_RESULTS = "ResultsFile";
        public const string PDB_COMP_SPONSORS = "SponsorID";
        public const string PDB_COMP_PARTNERS = "PartnerID";
        public const string PDB_COMP_BIN_ADD_FILE = "AdditionalFile";
        public const string PDB_COMP_BIN_LOGO_LEFT = "LogoLeft";
        public const string PDB_COMP_BIN_LOGO_RIGHT = "LogoRight";
        public const string PDB_PARAM_ADD_INFO = "_AdditionalParameterInfo";

        public const string PDB_BINARY_UPDATED = "ParamUpdated";

        public const string SCRIPT_CLIMBER_VALIDATE = "ValidateClimberData";
        public const string SCRIPT_TEAM_SEL_ID = "ScriptTeamSel";
 

        public static DateTime NowDate
        {
            get
            {
                DateTime nowDate = DateTime.UtcNow;
                nowDate = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, 0, 0, 0);
                return nowDate;
            }
        }

        public const string CLIMBER_PENDING_UPDATE = "U";
        public const string CLIMBER_PENDING_DELETE = "D";
        public const string CLIMBER_CONFIRMED = "C";

        public const int OP_STATE_NEW = 0;
        public const int OP_STATE_SENT = 1;
        public const int OP_STATE_CONFIRMED = 2;

        public const string NOTIF_LAST_CHANGE = "CNG;";
        public const string NOTIF_DEADLINE = "DDL;";

        public const string REDIRECT_NOACCESS = "~/no_access.aspx";
    }
}