using System;
using System.Globalization;
using XmlApiData;
namespace XmlApiClient
{
    partial class XmlClient
    {
        #region Teams
        public API_RegionCollection Regions
        {
            get
            {
                return GetSerializableT<API_RegionCollection>("Calendar", "Teams", String.Format(CultureInfo.InvariantCulture, "/{0}", this.CompId));
            }
        }

        public AsyncRequestResult BeginGetRegions(RequestCompleted<API_RegionCollection> callback, Object asyncState)
        {
            return BeginGetSerializableT("Calendar", "Teams", String.Format(CultureInfo.InvariantCulture, "/{0}", this.CompId),
                callback, asyncState);
        }

        public RegionApiModel PostRegion(RegionApiModel region)
        {
            return PostSerializableT("Calendar", "PostTeam", String.Empty, region);
        }

        public AsyncRequestResult BeginPostRegion(RegionApiModel region, RequestCompleted<RegionApiModel> callback, Object asyncState)
        {
            return BeginPostSerializableT("Calendar", "PostTeam", String.Empty, region, callback, asyncState, true);
        }

        public API_RegionCollection PostRegionCollection(API_RegionCollection regions)
        {
            return PostSerializableT("Calendar", "PostTeamCollection", String.Empty, regions);
        }

        public AsyncRequestResult BeginPostRegionCollection(API_RegionCollection regions, RequestCompleted<API_RegionCollection> callback, Object asyncState)
        {
            return BeginPostSerializableT("Calendar", "PostTeamCollection", String.Empty, regions, callback, asyncState, true);
        }
        #endregion
    }
}