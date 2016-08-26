using ClimbingCompetition.Common.API;
using System;
using System.Collections.Generic;

namespace ClimbingCompetition.Client
{
    partial class ServiceClient
    {
        public IAsyncResult BeginGetCompetitions(AsyncCallback callback, object state)
        {
            return this.DoFuncWithReadLock(() =>
            {
                if (string.IsNullOrEmpty(password))
                    throw new InvalidOperationException("no password");
                return this.BeginProcessRequest(
                    this.PrepareWebRequest("ApiCalendar", "GetCompetitions", new Dictionary<string, string> { { "password", this.password } }),
                    callback, state
                    );
            });
        }
        
        public ApiCompetition[] EndGetCompetitions(IAsyncResult asyncResult)
        {
            return this.EndProcessJsonRequest<ApiCompetition[]>(asyncResult);
        }

        public IAsyncResult BeginCheckCompetitionPassword(ApiCompetition competition, string password, AsyncCallback callback, object state)
        {
            return this.BeginProcessRequest(
                this.PrepareWebRequest("ApiCalendar",
                                      "CheckPassword",
                                      new Dictionary<string, string> { { "compId", competition == null ? null : competition.CompId }, { "password", password } }),
                callback,
                state);
        }

        public bool EndCheckPassword(IAsyncResult asyncResult)
        {
            return this.EndProcessJsonRequest<bool>(asyncResult);
        }
    }
}
