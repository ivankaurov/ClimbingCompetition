using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClimbingCompetition.Common.API
{
    public abstract class ApiListLine
    {
        public string Iid { get; set; }
        public long SecretaryId { get; set; }
        public string ListId { get; set; }
        public string ClimberId { get; set; }
        public string ResText { get; set; }
        public long Result { get; set; }
        public int Start { get; set; }
    }

    public class ApiListLineLead : ApiListLine
    {
        public string TimeText { get; set; }
        public long TimeValue { get; set; }
    }

    public class ApiListLineSpeed : ApiListLine
    {
        public enum ResultType { Time, Fall, Dsq, Dns}

        public string Route1Text { get; set; }
        public string Route2Text { get; set; }
        public long Route1Res { get; set; }
        public long Route2Res { get; set; }
        public ResultType TotalResType { get; set; }
    }

    public class ApiListLineBoulder : ApiListLine
    {
        public ResultType ResultType { get; set; }

        public ApiListLineBoulderRoute[] Routes { get; set; }
    }

    public class ApiListLineBoulderRoute
    {
        public int RouteNumber { get; set; }
        public int TopAttempt { get; set; }
        public int BonusAttempt { get; set; }
    }
}
