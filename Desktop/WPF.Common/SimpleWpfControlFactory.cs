using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace ClimbingCompetition.WPF
{
    public sealed class SimpleWpfControlFactory<T> : BaseWpfControlFactory<T>
        where T : BaseWpfControl, new()
    {
        private static readonly SimpleWpfControlFactory<T> instance = new SimpleWpfControlFactory<T>();

        private SimpleWpfControlFactory()
        {
        }

        public static IWpfControlFactory<T> Instance
        {
            get
            {
                return instance;
            }
        }

        public override T CreateControl(SqlConnection cn, string competitionTitle)
        {
            return new T();
        }
    }
}
