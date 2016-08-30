// <copyright file="XmlClient.Ctor.cs">
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

﻿using System;
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
