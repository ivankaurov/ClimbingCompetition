using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using XmlApiData;
using System.Threading;
using System.IO;
namespace XmlApiClient
{
    partial class XmlClient
    {
        private Uri CreateUri(String controller, String action, String parameters)
        {
            Uri requestUri;
            if (Uri.TryCreate(this.BaseUrl, String.Format(CultureInfo.InvariantCulture, "{0}/{1}{2}", controller, action, parameters), out requestUri))
                return requestUri;
            else
                throw new ArgumentException("Incorrect arguments", "parameters");
        }
        private HttpWebRequest GetApiRequest(String controller, String method, String parameters)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(CreateUri(controller, method, parameters));
            request.Proxy = this.Proxy;
            if (this.Timeout != null)
                request.Timeout = this.Timeout.Value;
            return request;
        }

        private T GetSerializableT<T>(String controller, String action, String parameters)
        {
            var request = GetApiRequest(controller, action, parameters);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            XmlSerializer ser = SerializingHelper.GetSerializer(typeof(T));
            using (var stream = response.GetResponseStream())
            {
                return (T)ser.Deserialize(stream);
            }
        }

        private AsyncRequestResult BeginGetSerializableT<T>(String controller, String action, String parameters, RequestCompleted<T> callback, object asyncState)
        {
            var request = GetApiRequest(controller, action, parameters);
            AsyncGetObject<T> data = new AsyncGetObject<T>(request, callback, asyncState);
            request.BeginGetResponse(EndGetPostSerializableT<T>, data);
            return new AsyncRequestResult(request);
        }
        
        private void EndGetPostSerializableT<T>(IAsyncResult res)
        {
            EndGetPostSerializableT<T>(res, true);
        }

        private void EndGetPostSerializableNoData(IAsyncResult res)
        {
            EndGetPostSerializableT<object>(res, false);
        }
        
        private static void EndGetPostSerializableT<T>(IAsyncResult res, bool hasResultData)
        {
#if DEBUG
            Thread.Sleep(1500);
#endif
            AsyncGetObject<T> state = (AsyncGetObject<T>)res.AsyncState;
            HttpWebResponse response;
            try { response = (HttpWebResponse)state.Request.EndGetResponse(res); }
            catch (ObjectDisposedException)
            {
                if (state.Callback != null)
                    state.Callback(default(T), RequestResult.Exit, HttpStatusCode.InternalServerError, null, state.AsyncState);
                return;
            }
            catch (Exception ex)
            {
                if (state.Callback != null)
                {
                    state.Callback(default(T), RequestResult.Error, HttpStatusCode.InternalServerError, ex, state.AsyncState);
                    return;
                }
                else
                    throw;
            }

            T result = default(T);
            bool scs;
            if (hasResultData)
            {
                XmlSerializer ser = SerializingHelper.GetSerializer(typeof(T));

                scs = false;
                try
                {
                    using (var stream = response.GetResponseStream())
                    {
                        result = (T)ser.Deserialize(stream);
                        scs = true;
                    }
                }
                catch (Exception ex)
                {
                    if (state.Callback != null)
                        state.Callback(default(T), RequestResult.Error, response.StatusCode, ex, state.AsyncState);
                    else
                        throw;
                }
            }
            else
                scs = true;
            if (scs && state.Callback != null)
                state.Callback(result, RequestResult.Success, response.StatusCode, null, state.AsyncState);

        }

        private T PostSerializableT<T>(String controller, String action, String parameters, T value, bool needResponseData = true) where T : APIBaseRequest
        {
            return PostSerializableT<T, T>(controller, action, parameters, value, needResponseData);
        }

        private TOut PostSerializableT<T, TOut>(String controller, String action, String parameters, T value, bool needResponseData = true) where T : APIBaseRequest
        {
            //parameters = "/1";
            byte[] data;
            var request = PreparePostRequest<T>(controller, action, parameters, value, out data);
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            if (needResponseData)
            {
                XmlSerializer s2 = SerializingHelper.GetSerializer(typeof(TOut));
                using (var stream = response.GetResponseStream())
                {
                    return (TOut)s2.Deserialize(stream);
                }
            }
            else
                return default(TOut);
        }

        private HttpWebRequest PreparePostRequest<T>(String controller, String action, String parameters, T value, out byte[] byteData) where T : APIBaseRequest
        {
            APICompIDRequest wrapper;

            SignBytes signFunction = Interlocked.CompareExchange(ref signFunc, null, null);
            if (signFunction != null)
                wrapper = new APISignedRequest(CompId) { Request = value, Signature = signFunction(SerializingHelper.GetRequestBytes(value)) };
            else
                wrapper = new APIPasswordRequest(CompId) { Request = value, Password = (this.password ?? String.Empty) };

            byteData = SerializingHelper.GetRequestBytes(wrapper);
            var request = GetApiRequest(controller, action, parameters);
            request.ContentType = "application/xml;charset=utf-16";
            request.Method = "POST";
            //using (var stream = request.GetRequestStream())
            //{
            //    stream.Write(byteData, 0, byteData.Length);
            //}
            return request;
        }
        
        private AsyncRequestResult BeginPostSerializableT<T>(String controller, String action, String parameters, T value, RequestCompleted<T> callback, object asyncState, bool needResponseData)
            where T : APIBaseRequest
        {
            return BeginPostSerializableTTOut<T, T>(controller, action, parameters, value, callback, asyncState, needResponseData);
        }

        private AsyncRequestResult BeginPostSerializableTTOut<T, TOut>(String controller, String action, String parameters, T value, RequestCompleted<TOut> callback, object asyncState, bool needResponseData)
            where T : APIBaseRequest
        {
            byte[] bytedata;
            var request = PreparePostRequest(controller, action, parameters, value, out bytedata);

            
            
            var data = new AsyncGetObject<TOut>(request, callback, asyncState);
            AsyncCallback asyncCallBack;
            if (needResponseData)
                asyncCallBack = EndGetPostSerializableT<TOut>;
            else
                asyncCallBack = EndGetPostSerializableNoData;
            RequestData<TOut> rqd = new RequestData<TOut>
            {
                Data = bytedata,
                AsyncData = data,
                Callback = asyncCallBack
            };

            request.BeginGetRequestStream(EndGetRequestStream<TOut>, rqd);
            return new AsyncRequestResult(request);
        }

        private class RequestData<T>
        {
            public byte[] Data { get; set; }
            public AsyncCallback Callback { get; set; }
            public AsyncGetObject<T> AsyncData { get; set; }
        }
        private void EndGetRequestStream<T>(IAsyncResult res)
        {
            RequestData<T> rqd = (RequestData<T>)res.AsyncState;
            try
            {
                using (var stream = rqd.AsyncData.Request.EndGetRequestStream(res))
                {
                    stream.Write(rqd.Data, 0, rqd.Data.Length);
                }
                rqd.AsyncData.Request.BeginGetResponse(rqd.Callback, rqd.AsyncData);
            }
            catch (Exception ex)
            {
                if (rqd.AsyncData.Callback == null)
                    throw;
                else
                {
                    rqd.AsyncData.Callback(default(T), RequestResult.Error, HttpStatusCode.NotAcceptable, ex, rqd.AsyncData.AsyncState);
                    return;
                }
            }
        }
    }
}