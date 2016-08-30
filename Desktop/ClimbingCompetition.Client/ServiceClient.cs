// <copyright file="ServiceClient.cs">
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using ClimbingCompetition.Common.API;

namespace ClimbingCompetition.Client
{
    public sealed partial class ServiceClient
    {
        private Uri baseUri;
        private string password;
        private readonly ReaderWriterLockSlim slimLock = new ReaderWriterLockSlim();
        private ApiCompetition comp;

        private T DoFuncWithReadLock<T>(Func<T> func)
        {
            if (func == null)
                throw new ArgumentNullException("func");

            bool hasLock = false;
            try
            {
                if (hasLock = this.slimLock.TryEnterReadLock(Timeout.Infinite))
                    return func();
                else
                    throw new TimeoutException();
            }
            finally
            {
                if (hasLock)
                    this.slimLock.ExitReadLock();
            }
        }

        private void DoActionWithWriteLock(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            bool hasLock = false;
            try
            {
                if (hasLock = this.slimLock.TryEnterWriteLock(Timeout.Infinite))
                    action();
                else
                    throw new TimeoutException();
            }
            finally
            {
                if (hasLock)
                    this.slimLock.ExitWriteLock();
            }
        }

        private void SetConnectionModelInternal(ServiceConnectionModel model)
        {
            if (model == null)
            {
                this.baseUri = null;
                this.password = null;
                this.comp = null;
            }
            else
            {
                this.baseUri = string.IsNullOrEmpty(model.Address) ? null : new Uri(model.Address.EndsWith("/") ? model.Address : (model.Address + "/"));
                this.password = model.Password ?? string.Empty;
                this.comp = model.Competition;
            }
        }

        private ServiceConnectionModel GetModelInternal()
        {
            return new ServiceConnectionModel
            {
                Address = this.baseUri.AbsoluteUri,
                Competition = this.comp,
                Password = this.password ?? string.Empty
            };
        }
        
        public void Load(SqlConnection cn)
        {
            this.DoActionWithWriteLock(() => this.SetConnectionModelInternal(ServiceConnectionModel.FromSql(cn)));
        }

        public void Persist(SqlConnection cn)
        {
            var model = this.DoFuncWithReadLock(() => this.GetModelInternal());
            if (model != null)
                model.Persist(cn);
        }

        public Uri BaseUri
        {
            get { return this.DoFuncWithReadLock(() => this.baseUri); }
            set { this.DoActionWithWriteLock(() => this.baseUri = value); }
        }

        public string Password
        {
            get { return this.DoFuncWithReadLock(() => this.password); }
            set { this.DoActionWithWriteLock(() => this.password = value); }
        }

        public ApiCompetition Competition
        {
            get { return this.DoFuncWithReadLock(() => this.comp); }
            set { this.DoActionWithWriteLock(() => this.comp = value); }
        }

        public bool ConnectionSet
        {
            get
            {
                return this.DoFuncWithReadLock(() => this.comp == null ? false : !string.IsNullOrEmpty(this.comp.CompId));
            }
        }

        private ServiceClient()
        {
        }

    }
}
