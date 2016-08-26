using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Web.Http;
using WebClimbing.Controllers;
using WebClimbing.Models;
using XmlApiData;

namespace WebClimbing.ServiceClasses
{
    public static class APIConverter
    {
        public static CompetitionApiModel ToApi(this CompetitionModel model)
        {
            CompetitionApiModel res = new CompetitionApiModel();
            res.Iid = model.Iid;
            res.FullName = model.Name;
            res.ShortName = model.ShortName;
            res.DateStart = model.Start;
            res.DateEnd = model.End;
            res.Rules = CompetitionRules.International;
            return res;
        }

        public static Comp_AgeGroupApiModel ToApi(this Comp_AgeGroupModel model)
        {
            Comp_AgeGroupApiModel res = new Comp_AgeGroupApiModel();
            res.Iid = model.AgeGroupId;
            int compYear = model.Competition.Start.Year;
            res.YearOld = compYear - (model.AgeGroup.MaxAge ?? int.MaxValue);
            if (res.YearOld < 0)
                res.YearOld = 0;
            res.YearYoung = compYear - (model.AgeGroup.MinAge ?? 0);
            if (res.YearYoung > compYear)
                res.YearYoung = compYear;
            res.Female = (model.AgeGroup.GenderProperty == Gender.Female);
            res.Name = model.AgeGroup.SecretaryName;
            return res;
        }

        private static Comp_CompetitorRegistrationApiModel ToApiSimple(Comp_CompetitiorRegistrationModel model)
        {
            Comp_CompetitorRegistrationApiModel res = new Comp_CompetitorRegistrationApiModel();
            res.License = model.PersonId;
            res.Surname = model.Person.Surname;
            res.Name = model.Person.Name;
            var team = model.Teams.OrderBy(t=>t.RegionOrder).FirstOrDefault();
            res.TeamID = (team == null) ? 0 : team.RegionId;
            res.GroupID = model.CompAgeGroup.AgeGroupId;
            res.YearOfBirth = model.Person.DateOfBirth.Year;
            res.Female = model.Person.GenderFemale;
            res.Razr = model.Qf.GetFriendlyValue();
            res.Bib = model.SecretaryId;
            res.Lead = (ApplicationType)model.Lead;
            res.Speed = (ApplicationType)model.Speed;
            res.Boulder = (ApplicationType)model.Boulder;
            res.RankingLead = model.RankingLead;
            res.RankingSpeed = model.RankingiSpeed;
            res.RankingBoulder = model.RankingBoulder;
            return res;
        }

        public static Comp_CompetitorRegistrationApiModel ToApi(this Comp_CompetitiorRegistrationModel model, bool multipleTeams = false)
        {
            var resSimple = ToApiSimple(model);
            if(!multipleTeams)
                return resSimple;
            Comp_MultipleTeamsClimber res = new Comp_MultipleTeamsClimber();
            var t = resSimple.GetType();
            foreach (var pInfo in t.GetProperties().Where(p => p.CanRead && p.CanWrite))
            {
                var value = pInfo.GetValue(resSimple, null);
                pInfo.SetValue(res, value, null);
            }
            res.Teams = model.Teams.OrderBy(l=>l.RegionOrder).Select(l => l.RegionId).ToArray();
            return res;
        }


        public static RegionApiModel ToApi(this RegionModel model)
        {
            RegionApiModel res = new RegionApiModel();
            res.Iid = model.Iid;
            res.Name = model.Name;
            return res;
        }

        public static Comp_ClimberPicture ToApi(this PersonPictureModel model)
        {
            Comp_ClimberPicture res = new Comp_ClimberPicture();
            res.ClimberId = model.PersonId;
            res.Picture = model.Image;
            res.PictureDate = model.PictureDate;
            return res;
        }

        public static bool ValidateDSASignature(APIBaseRequest value, byte[] signature, byte[] publicKey)
        {
            DSAParameters algParams;
            using (MemoryStream mstr = new MemoryStream(publicKey))
            {
                BinaryFormatter fmt = new BinaryFormatter();
                try { algParams = (DSAParameters)fmt.Deserialize(mstr); }
                catch (SerializationException) { return false; }
            }
            using (var provider = new DSACryptoServiceProvider())
            {
                try { provider.ImportParameters(algParams); }
                catch (CryptographicException) { return false; }
                return provider.VerifyData(SerializingHelper.GetRequestBytes(value), signature);
            }
        }

        public static T GetRequestParameter<T>(this APISignedRequest request, ClimbingContext db, out CompetitionModel comp) where T : APIBaseRequest
        {
            if (request == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            comp = db.Competitions.Find(request.CompID);
            if (comp == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            T retValue = request.Request as T;
            if (retValue == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            byte[] publicKeyData = comp[CompetitionParamType.SignatureKey].BinaryValue;
            if (publicKeyData == null)
                throw new HttpResponseException(HttpStatusCode.NotImplemented);
            if (!ValidateDSASignature(retValue, request.Signature, publicKeyData))
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            return retValue;
        }
    }
}