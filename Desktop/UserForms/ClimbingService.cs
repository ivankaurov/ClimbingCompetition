using System;
using System.Collections.Generic;
using System.Text;
using ClimbingCompetition.WebServices;

namespace ClimbingCompetition
{
    public sealed class ClimbingService : ClimbingWebService
    {
        private const int DEFAULT_TIMEOUT 
#if DEBUG
            = 600000;
#else
            = 300000;
#endif
        public new int Timeout
        {
            get { return base.Timeout; }
            private set { base.Timeout = value; }
        }

        public new string Url
        {
            get { return base.Url; }
            private set { base.Url = value; }
        }

        public ClimbingService()
#if DEBUG
            : this("http://localhost:1191/ClimbingWebService.asmx")
#else
            : this("http://climbing-competition.org/ClimbingWebService.asmx")
#endif
        { }

        public ClimbingService(string url)
            : base()
        {
            this.Url = url;
            this.Timeout = DEFAULT_TIMEOUT;
        }
    }
}
