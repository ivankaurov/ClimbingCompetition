using System;
using System.Net;

namespace XmlApiClient
{
    public sealed partial class XmlClient
    {
        internal class AsyncGetObject<T>
        {
            public HttpWebRequest Request { get; private set; }
            public RequestCompleted<T> Callback { get; private set; }
            public Object AsyncState { get; private set; }
            public AsyncGetObject(HttpWebRequest request, RequestCompleted<T> callback, Object asyncState)
            {
                this.Request = request;
                this.Callback = callback;
                this.AsyncState = asyncState;
            }
        }

        public Uri BaseUrl { get; private set; }

        public int? Timeout { get; set; }

        public IWebProxy Proxy { get; set; }

        public XmlClient(Uri baseUri, long compId, SignBytes signatureFunction, String password)
        {
            this.Proxy = null;
            this.BaseUrl = baseUri;
            this.Timeout = null;
            this.CompId = compId;
            this.signFunc = signatureFunction;
            this.password = password;
        }

        private String password;
        public long CompId { get; set; }

        public XmlClient(String baseUrl, long compId, SignBytes signatureFunction, String password)
            : this(new Uri(baseUrl), compId, signatureFunction, password)
        {
        }

        private SignBytes signFunc;
    }

    public enum RequestResult
    {
        Success = 0,
        Error = 1,
        Exit = 2
    }

    public delegate void RequestCompleted<in TResult>(TResult result, RequestResult requestResult, HttpStatusCode status, Exception exception, Object asyncState);
    public delegate byte[] SignBytes(byte[] data);
}
