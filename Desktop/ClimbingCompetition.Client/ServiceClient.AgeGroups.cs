using ClimbingCompetition.Common.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClimbingCompetition.Client
{
    partial class ServiceClient
    {
        public IAsyncResult BeginGetAgeGroups(AsyncCallback callback, object state)
        {
            return this.BeginProcessRequest(this.PrepareWebRequest("ApiCalendar", "GetAgeGroups", new Dictionary<string, string> { { "id", this.comp.CompId } }), callback, state);
        }

        public ApiAgeGroup[] EndGetAgeGroups(IAsyncResult asyncResult)
        {
            return this.EndProcessJsonRequest<ApiAgeGroup[]>(asyncResult);
        }

        public ApiAgeGroup[] GetAgeGroups()
        {
            return this.EndGetAgeGroups(this.BeginGetAgeGroups(null, null));
        }
        public ApiAgeGroup[] PostAgeGroups(IEnumerable<ApiAgeGroup> data, bool fullRefresh)
        {
            var request = this.PreparePostRequest("ApiCalendar", "SaveAgeGroups",
                new Dictionary<string, string> { { "id", this.Competition.CompId }, { "password", this.password }, { "refreshAll", fullRefresh.ToString() } },
                data.ToArray());

            return this.EndProcessJsonRequest<ApiAgeGroup[]>(this.BeginProcessRequest(request, null, null));
        }
    }
}