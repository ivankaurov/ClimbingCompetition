using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace ClimbingCompetition.WPF
{
    public abstract class BaseWpfControlFactory<T> : IWpfControlFactory<T>
        where T: BaseWpfControl
    {
        public abstract T CreateControl(SqlConnection cn, string competitionTitle);

        BaseWpfControl IWpfControlFactory.CreateControl(SqlConnection cn, string competitionTitle)
        {
            return this.CreateControl(cn, competitionTitle);
        }
    }
}
