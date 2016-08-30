// <copyright file="ServiceClient.Lists.cs">
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

﻿using ClimbingCompetition.Common.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClimbingCompetition.Client
{
    partial class ServiceClient
    {
        public ApiListHeader PostListHeader(ApiListHeader list)
        {
            if (list == null)
                return list;
            var request = this.PreparePostRequest("ApiLists", "PostListHeader",
                new Dictionary<string, string> { { "id", this.Competition.CompId }, { "password", this.Password } },
                list);
            return this.EndProcessJsonRequest<ApiListHeader>(this.BeginProcessRequest(request, null, null));
        }

        public void ClearUnneededLists(IEnumerable<string> listsToRemain)
        {
            var request = this.PreparePostRequest("ApiLists", "RemoveListsExceptFor",
                new Dictionary<string, string> { { "id", this.Competition.CompId }, { "password", this.Password } },
                listsToRemain.ToArray());

            this.EndProcessRequest(this.BeginProcessRequest(request, null, null));
        }

        private T[] PostListLines<T>(IEnumerable<T> data, string action) where T :ApiListLine
        {
            var request = this.PreparePostRequest("ApiLists", action,
                new Dictionary<string, string> { { "id", this.Competition.CompId }, { "password", this.Password } },
                data.ToArray());
            return this.EndProcessJsonRequest<T[]>(this.BeginProcessRequest(request, null, null));
        }

        public ApiListLineLead[] PostLeadResults(IEnumerable<ApiListLineLead> data)
        {
            return this.PostListLines(data, "PostLeadResults");
        }

        public ApiListLineSpeed[] PostSpeedResults(IEnumerable<ApiListLineSpeed> data)
        {
            return this.PostListLines(data, "PostSpeedResults");
        }

        public ApiListLineBoulder[] PostBoulderResuls(IEnumerable<ApiListLineBoulder> data)
        {
            return this.PostListLines(data, "PostBoulderResults");
        }

        public void ClearUnneededListResults(IEnumerable<string> neededLines, string listId)
        {
            var request = this.PreparePostRequest("ApiLists", "RemoveDeadResults",
                new Dictionary<string, string> { { "id", this.Competition.CompId }, { "password", this.Password }, { "listId", listId } },
                neededLines.ToArray());
            this.EndProcessRequest(this.BeginProcessRequest(request, null, null));
        }
    }
}
