using System;
using System.Globalization;
using XmlApiData;
namespace XmlApiClient
{
    partial class XmlClient
    {
        #region ListLines

        public bool PostListLine(ApiListLine listLine)
        {
            PostSerializableT<ApiListLine, object>("Results", "PostListLine", String.Empty, listLine, false);
            return true;
        }

        public AsyncRequestResult BeginPostListLine(ApiListLine region, RequestCompleted<object> callback, Object asyncState)
        {
            return BeginPostSerializableTTOut("Results", "PostListLine", String.Empty, region, callback, asyncState, false);
        }

        public bool ReloadResultList(ApiListLineCollection regions)
        {
            PostSerializableT<ApiListLineCollection, object>("Results", "ReloadResultList", String.Empty, regions, false);
            return true;
        }

        public AsyncRequestResult BeginReloadResultList(ApiListLineCollection regions, RequestCompleted<object> callback, Object asyncState)
        {
            return BeginPostSerializableTTOut("Results", "ReloadResultList", String.Empty, regions, callback, asyncState, false);
        }

        public bool LoadResultsPackage(ApiListLineCollection regions)
        {
            PostSerializableT<ApiListLineCollection, object>("Results", "LoadResultsPackage", String.Empty, regions, false);
            return true;
        }

        public AsyncRequestResult BeginLoadResultsPackage(ApiListLineCollection regions, RequestCompleted<object> callback, Object asyncState)
        {
            return BeginPostSerializableTTOut("Results", "LoadResultsPackage", String.Empty, regions, callback, asyncState, false);
        }
        #endregion
    }
}