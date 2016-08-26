using System;
using System.Globalization;
using XmlApiData;
using System.Net;
namespace XmlApiClient
{
    partial class XmlClient
    {
        #region Competitions
        public API_CompetitionCollection GetCompetitions(int year)
        {
            return GetSerializableT<API_CompetitionCollection>("Calendar", "Competitions", String.Format(CultureInfo.InvariantCulture, "/{0}", year));
        }

        public AsyncRequestResult BeginGetCompetitions(int year, RequestCompleted<API_CompetitionCollection> callback, Object asyncState)
        {
            return BeginGetSerializableT("Calendar", "Competitions",
                String.Format(CultureInfo.InvariantCulture, "/{0}", year), callback, asyncState);
        }

        public bool ValidateCompetition()
        {
            try
            {
                PostSerializableT<APISimpleRequest, bool>("Calendar", "TestDSA", String.Empty, new APISimpleRequest(), false);
                return true;
            }
            catch (WebException ex)
            {
                HttpWebResponse response = ex.Response as HttpWebResponse;
                if (response != null)
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                        case HttpStatusCode.InternalServerError:
                        case HttpStatusCode.MethodNotAllowed:
                        case HttpStatusCode.BadGateway:
                        case HttpStatusCode.BadRequest:
                            throw;
                    }
                return false;
            }
        }
        #endregion
    }
}