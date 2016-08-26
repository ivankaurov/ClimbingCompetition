using Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ClimbingCompetition.Client
{
    partial class ServiceClient
    {
        private readonly Dictionary<IAsyncResult, HttpWebRequest> processingRequests = new Dictionary<IAsyncResult, HttpWebRequest>();

        private readonly object requestLocker = new object();

        private Uri CreateUri(String controller, String action, IDictionary<string, string> parameters)
        {
            if (this.baseUri == null)
                throw new InvalidOperationException("Service URI is not set");

            if (string.IsNullOrEmpty("controller"))
                throw new ArgumentNullException("controller");
            if (string.IsNullOrEmpty("action"))
                throw new ArgumentNullException("action");
            var uriBuilder = new StringBuilder();
            uriBuilder.Append(controller)
                      .Append("/")
                      .Append(action);
            if (parameters != null && parameters.Count > 0)
            {
                var paramBuilder = new StringBuilder();
                foreach (var p in parameters)
                {
                    if (paramBuilder.Length > 0)
                        paramBuilder.Append("&");
                    paramBuilder.Append(p.Key).Append("=").Append(p.Value);
                }
                uriBuilder.Append("?").Append(paramBuilder);
            }

            Uri requestUri;
            if (Uri.TryCreate(this.baseUri, uriBuilder.ToString() /*String.Format("{0}/{1}{2}", controller, action, parameters)*/, out requestUri))
                return requestUri;
            else
                throw new ArgumentException("Incorrect arguments", "parameters");
        }

        private HttpWebRequest PrepareWebRequest(string controller, string action, IDictionary<string, string> parameters, string requestType = "GET")
        {
            var result = (HttpWebRequest)HttpWebRequest.Create(this.CreateUri(controller, action, parameters));
            result.ContentType = "application/json";
            result.Timeout = 10 * 60 * 1000;
            result.ReadWriteTimeout = 60 * 1000;
            result.KeepAlive = true;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            if (!string.IsNullOrEmpty(requestType))
                result.Method = requestType;
            return result;
        }

        private IAsyncResult BeginPreparePostRequest(string controller, string action, IDictionary<string,string> parameters,
            AsyncCallback callback, object state)
        {

            var request = this.PrepareWebRequest(controller, action, parameters, "POST");

            IAsyncResult result;
            lock (this.requestLocker)
            {
                result = request.BeginGetRequestStream(callback, state);
                this.processingRequests.Add(result, request);
            }

            return result;
        }

        private Stream EndPreparePostRequest(IAsyncResult res, out HttpWebRequest request)
        {
            lock (this.requestLocker)
            {
                request = this.processingRequests[res];
                this.processingRequests.Remove(res);
            }
            
            return request.EndGetRequestStream(res);
        }

        private HttpWebRequest PreparePostRequest(string controller, string action, IDictionary<string, string> parameters, object data)
        {
            if (data == null)
                return this.PrepareWebRequest(controller, action, parameters, "POST");

            HttpWebRequest request;
            using (var stream = this.EndPreparePostRequest(this.BeginPreparePostRequest(controller, action, parameters, null, null), out request))
            {
                JsonSerializer s = new JsonSerializer();
                using (var writer = new StreamWriter(stream))
                {
                    s.Serialize(writer, data);
                    writer.Flush();
                }
            }

            return request;
        }

        private IAsyncResult BeginProcessRequest(HttpWebRequest request, AsyncCallback callback, object state)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            IAsyncResult result;
            lock (this.requestLocker)
            {
                if (System.Net.ServicePointManager.Expect100Continue)
                {
                    System.Net.ServicePointManager.Expect100Continue = false;
                }
                result = request.BeginGetResponse(callback, state);
                processingRequests.Add(result, request);
            }
            return result;
        }

        private HttpWebResponse EndProcessRequest(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
                throw new ArgumentNullException("asyncResult");

            HttpWebRequest request;
            lock (this.requestLocker)
            {
                if (!this.processingRequests.ContainsKey(asyncResult))
                    throw new ArgumentException("Invalid asyncResult", "asuncResult");
                request = this.processingRequests[asyncResult];
                this.processingRequests.Remove(asyncResult);
            }

            return (HttpWebResponse)request.EndGetResponse(asyncResult);
        }

        private T EndProcessJsonRequest<T>(IAsyncResult asyncResult)
        {
            var response = this.EndProcessRequest(asyncResult);

            using (var stream = response.GetResponseStream())
            {
                using (var streamReader = new StreamReader(stream))
                {
                    var serializer = new JsonSerializer();

                    return (T)serializer.Deserialize(streamReader, typeof(T));
                }
            }
        }
    }
}
