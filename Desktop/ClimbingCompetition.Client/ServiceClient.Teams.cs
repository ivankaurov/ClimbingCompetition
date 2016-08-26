using ClimbingCompetition.Common.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClimbingCompetition.Client
{
    partial class ServiceClient
    {
        public IAsyncResult BeginGetTeams(AsyncCallback callback, object state)
        {
            return this.BeginProcessRequest(this.PrepareWebRequest("ApiCalendar", "GetTeams", new Dictionary<string, string> { { "id", this.comp.CompId } }), callback, state);
        }

        public ApiTeam[] EndGetTeams(IAsyncResult asyncResult)
        {
            return this.EndProcessJsonRequest<ApiTeam[]>(asyncResult);
        }

        public ApiTeam[] GetTeams()
        {
            return this.EndGetTeams(this.BeginGetTeams(null, null));
        }
    }
}
