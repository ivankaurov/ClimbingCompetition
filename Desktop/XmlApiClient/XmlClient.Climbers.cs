using System;
using System.Globalization;
using XmlApiData;
namespace XmlApiClient
{
    partial class XmlClient
    {
        #region Climbers
        public API_ClimbersCollection Climbers
        {
            get
            {
                return GetSerializableT<API_ClimbersCollection>("Calendar", "Climbers", String.Format(CultureInfo.InvariantCulture, "/{0}", this.CompId));
            }
        }

        public AsyncRequestResult BeginGetClimbers(RequestCompleted<API_ClimbersCollection> callback, Object asyncState)
        {
            return BeginGetSerializableT("Calendar", "Climbers", String.Format(CultureInfo.InvariantCulture, "/{0}", this.CompId),
                callback, asyncState);
        }

        public Comp_CompetitorRegistrationApiModel PostClimber(Comp_CompetitorRegistrationApiModel climber)
        {
            return PostSerializableT("Calendar", "PostClimber", String.Empty, climber);
        }

        public AsyncRequestResult BeginPostClimber(Comp_CompetitorRegistrationApiModel climber, RequestCompleted<Comp_CompetitorRegistrationApiModel> callback, Object asyncState)
        {
            return BeginPostSerializableT("Calendar", "PostClimber", String.Empty, climber, callback, asyncState, true);
        }

        public API_ClimbersCollection PostClimberCollection(API_ClimbersCollection climbers)
        {
            return PostSerializableT("Calendar", "PostClimberCollection", String.Empty, climbers);
        }

        public AsyncRequestResult BeginPostClimberCollection(API_ClimbersCollection climbers, RequestCompleted<API_ClimbersCollection> callback, Object asyncState)
        {
            return BeginPostSerializableT("Calendar", "PostClimberCollection", String.Empty, climbers, callback, asyncState, true);
        }

        #endregion
    }
}