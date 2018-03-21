using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace ClimbingCompetition.WPF
{
    public interface IWpfControlFactory
    {
        BaseWpfControl CreateControl(SqlConnection cn, string competitionTitle);
    }

    public interface IWpfControlFactory<out T> : IWpfControlFactory
        where T : BaseWpfControl
    {
        T CreateControl(SqlConnection cn, string competitionTitle);
    }
}
