using System;
using System.Globalization;
using XmlApiData;
namespace XmlApiClient
{
    partial class XmlClient
    {
        #region AgeGroups
        public API_AgeGroupCollection AgeGroups
        {
            get
            {
                return GetSerializableT<API_AgeGroupCollection>("Calendar", "Groups", String.Format(CultureInfo.InvariantCulture, "/{0}", this.CompId));
            }
        }

        public AsyncRequestResult BeginGetAgeGroups(RequestCompleted<API_AgeGroupCollection> callback, Object asyncState)
        {
            return BeginGetSerializableT("Calendar", "Groups", String.Format(CultureInfo.InvariantCulture, "/{0}", this.CompId),
                callback, asyncState);
        }

        public Comp_AgeGroupApiModel PostGroup(Comp_AgeGroupApiModel ageGroup)
        {
            return PostSerializableT("Calendar", "PostGroup", String.Empty, ageGroup);
        }

        public AsyncRequestResult BeginPostGroup(Comp_AgeGroupApiModel ageGroup, RequestCompleted<Comp_AgeGroupApiModel> callback, Object asyncState)
        {
            return BeginPostSerializableT("Calendar", "PostGroup", String.Empty, ageGroup, callback, asyncState, true);
        }

        public API_AgeGroupCollection PostGroupCollection(API_AgeGroupCollection ageGroups)
        {
            return PostSerializableT("Calendar", "PostGroupCollection", String.Empty, ageGroups);
        }

        public AsyncRequestResult BeginPostGroupCollection(API_AgeGroupCollection ageGroups, RequestCompleted<API_AgeGroupCollection> callback, Object asyncState)
        {
            return BeginPostSerializableT("Calendar", "PostGroupCollection", String.Empty, ageGroups, callback, asyncState, true);
        }

        #endregion
    }
}