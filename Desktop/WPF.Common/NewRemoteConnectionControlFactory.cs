using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace ClimbingCompetition.WPF
{
    public sealed class NewRemoteConnectionControlFactory : BaseWpfControlFactory<NewRemoteConnectionControl>
    {
        private static readonly NewRemoteConnectionControlFactory instance = new NewRemoteConnectionControlFactory();

        private NewRemoteConnectionControlFactory()
        {
        }

        public static IWpfControlFactory<NewRemoteConnectionControl> Instance
        {
            get
            {
                return instance;
            }
        }

        public override NewRemoteConnectionControl CreateControl(SqlConnection cn, string competitionTitle)
        {
            if (cn == null)
            {
                throw new ArgumentNullException("cn");
            }

            return new NewRemoteConnectionControl(cn);
        }
    }
}
