using Extensions;
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
