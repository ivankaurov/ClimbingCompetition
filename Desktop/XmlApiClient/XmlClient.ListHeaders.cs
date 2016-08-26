using System;
using System.Globalization;
using XmlApiData;
namespace XmlApiClient
{
    partial class XmlClient
    {
        #region ListHeaders

        public bool PostListHeader(ApiListHeader listHeader)
        {
            PostSerializableT<ApiListHeader, object>("Results", "PostListHeader", String.Empty, listHeader, false);
            return true;
        }

        public AsyncRequestResult BeginPostListHeader(ApiListHeader region, RequestCompleted<object> callback, Object asyncState)
        {
            return BeginPostSerializableTTOut("Results", "PostListHeader", String.Empty, region, callback, asyncState, false);
        }

        public bool PostListHeaderCollection(ApiListHeaderCollection regions)
        {
            PostSerializableT<ApiListHeaderCollection, object>("Results", "ReloadAllLists", String.Empty, regions, false);
            return true;
        }

        public AsyncRequestResult BeginPostListHeaderCollection(ApiListHeaderCollection regions, RequestCompleted<object> callback, Object asyncState)
        {
            return BeginPostSerializableTTOut("Results", "ReloadAllLists", String.Empty, regions, callback, asyncState, false);
        }
        #endregion
    }
}