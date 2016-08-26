using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace XmlApiClient
{
    public class AsyncRequestResult
    {
        private HttpWebRequest request;

        protected internal AsyncRequestResult(HttpWebRequest request)
        {
            this.request = request;
        }

        public virtual void Abort()
        {
            if (request != null)
                request.Abort();
        }
    }
}
