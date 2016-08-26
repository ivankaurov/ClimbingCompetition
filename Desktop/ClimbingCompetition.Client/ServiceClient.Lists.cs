using ClimbingCompetition.Common.API;
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
