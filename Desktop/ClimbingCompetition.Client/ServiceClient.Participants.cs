using ClimbingCompetition.Common.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClimbingCompetition.Client
{
    partial class ServiceClient
    {
        public IAsyncResult BeginGetParticipants(AsyncCallback callback, object state)
        {
            return this.BeginProcessRequest(this.PrepareWebRequest("ApiCalendar", "GetParticipants", new Dictionary<string, string> { { "id", this.comp.CompId } }), callback, state);
        }

        public ApiParticipant[] EndGetParticipants(IAsyncResult asyncResult)
        {
            return this.EndProcessJsonRequest<ApiParticipant[]>(asyncResult);
        }

        public ApiParticipant[] GetParticipants()
        {
            return this.EndGetParticipants(this.BeginGetParticipants(null, null));
        }

        public ApiParticipant[] PostParticipants(IEnumerable<ApiParticipant> participants, bool fullRefresh)
        {
            var request = this.PreparePostRequest("ApiCalendar", "SaveParticipants",
                new Dictionary<string, string> { { "id", this.Competition.CompId }, { "password", this.Password }, { "refreshAll", fullRefresh.ToString() } },
                participants.ToArray());

            return this.EndProcessJsonRequest<ApiParticipant[]>(this.BeginProcessRequest(request, null, null));
        }

        public void ClearNotNeededClimbers(IEnumerable<string> toClear)
        {
            var request = this.PreparePostRequest("ApiCalendar", "RemoveAllClimbersExceptFor",
                new Dictionary<string, string> { { "id", this.Competition.CompId }, { "password", this.password } },
                toClear.ToArray());

            this.EndProcessRequest(this.BeginProcessRequest(request, null, null));
        }
    }
}
