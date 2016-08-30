// <copyright file="XmlClient.Competitions.cs">
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
using System.Globalization;
using XmlApiData;
using System.Net;
namespace XmlApiClient
{
    partial class XmlClient
    {
        #region Competitions
        public API_CompetitionCollection GetCompetitions(int year)
        {
            return GetSerializableT<API_CompetitionCollection>("Calendar", "Competitions", String.Format(CultureInfo.InvariantCulture, "/{0}", year));
        }

        public AsyncRequestResult BeginGetCompetitions(int year, RequestCompleted<API_CompetitionCollection> callback, Object asyncState)
        {
            return BeginGetSerializableT("Calendar", "Competitions",
                String.Format(CultureInfo.InvariantCulture, "/{0}", year), callback, asyncState);
        }

        public bool ValidateCompetition()
        {
            try
            {
                PostSerializableT<APISimpleRequest, bool>("Calendar", "TestDSA", String.Empty, new APISimpleRequest(), false);
                return true;
            }
            catch (WebException ex)
            {
                HttpWebResponse response = ex.Response as HttpWebResponse;
                if (response != null)
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                        case HttpStatusCode.InternalServerError:
                        case HttpStatusCode.MethodNotAllowed:
                        case HttpStatusCode.BadGateway:
                        case HttpStatusCode.BadRequest:
                            throw;
                    }
                return false;
            }
        }
        #endregion
    }
}