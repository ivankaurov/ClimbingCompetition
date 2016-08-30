// <copyright file="ServiceClient.Requests.cs">
// Copyright © 2016 All Rights Reserved
// This file is part of ClimbingCompetition.
//
//  ClimbingCompetition is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  ClimbingCompetition is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with ClimbingCompetition.  If not, see <http://www.gnu.org/licenses/>.
//
// (Этот файл — часть ClimbingCompetition.
// 
// ClimbingCompetition - свободная программа: вы можете перераспространять ее и/или
// изменять ее на условиях Стандартной общественной лицензии GNU в том виде,
// в каком она была опубликована Фондом свободного программного обеспечения;
// либо версии 3 лицензии, либо (по вашему выбору) любой более поздней
// версии.
// 
// ClimbingCompetition распространяется в надежде, что она будет полезной,
// но БЕЗО ВСЯКИХ ГАРАНТИЙ; даже без неявной гарантии ТОВАРНОГО ВИДА
// или ПРИГОДНОСТИ ДЛЯ ОПРЕДЕЛЕННЫХ ЦЕЛЕЙ. Подробнее см. в Стандартной
// общественной лицензии GNU.
// 
// Вы должны были получить копию Стандартной общественной лицензии GNU
// вместе с этой программой. Если это не так, см. <http://www.gnu.org/licenses/>.)
// </copyright>
// <author>Ivan Kaurov</author>
// <date>30.08.2016 23:51</date>

﻿using Extensions;
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
