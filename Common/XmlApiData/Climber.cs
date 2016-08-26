using System;
using System.Collections.Generic;
using System.Xml.Serialization;
namespace XmlApiData
{
    public enum ApplicationType : short
    {
        NotStart = 0,
        Start = 1,
        NoPoints = 2
    }

    [XmlRoot("Climbers", Namespace = XmlApiDataConstants.NAMESPACE)]
    [XmlType("ClimbersCollection", IncludeInSchema = true)]
    public sealed class API_ClimbersCollection : APIBaseRequest, IAPICollection<Comp_CompetitorRegistrationApiModel>
    {
        [XmlElement("Climber")]
        public Comp_CompetitorRegistrationApiModel[] Data { get; set; }
        public API_ClimbersCollection() { }
        public API_ClimbersCollection(IEnumerable<Comp_CompetitorRegistrationApiModel> data) { this.Data = XmlApiDataConstants.ToArray(data); }
    }

    [XmlInclude(typeof(Comp_MultipleTeamsClimber))]
    [XmlType("Climber", Namespace = XmlApiDataConstants.NAMESPACE, IncludeInSchema = true)]
    public class Comp_CompetitorRegistrationApiModel : APIBaseRequest
    {
        [XmlElement("License")]
        public long License { get; set; }

        [XmlElement("Bib", IsNullable = true)]
        public int? Bib { get; set; }

        [XmlElement("Surname")]
        public String Surname { get; set; }

        [XmlElement("Name")]
        public String Name { get; set; }

        [XmlElement("Female")]
        public bool Female { get; set; }

        [XmlElement("TeamID")]
        public long TeamID { get; set; }

        [XmlElement("GroupID")]
        public int GroupID { get; set; }

        [XmlElement("YearOfBirth")]
        public int YearOfBirth { get; set; }

        [XmlElement("Razryad")]
        public String Razr { get; set; }

        [XmlIgnore]
        public ApplicationType Lead
        {
            get { return (ApplicationType)LeadN; }
            set { LeadN = (short)value; }
        }
        [XmlIgnore]
        public ApplicationType Speed
        {
            get { return (ApplicationType)SpeedN; }
            set { SpeedN = (short)value; }
        }
        [XmlIgnore]
        public ApplicationType Boulder
        {
            get { return (ApplicationType)BoulderN; }
            set { BoulderN = (short)value; }
        }

        [XmlElement("Lead")]
        public short LeadN { get; set; }

        [XmlElement("Speed")]
        public short SpeedN { get; set; }

        [XmlElement("Boulder")]
        public short BoulderN { get; set; }

        [XmlElement("RankingLead", IsNullable = true)]
        public int? RankingLead { get; set; }

        [XmlElement("RankingSpeed", IsNullable = true)]
        public int? RankingSpeed { get; set; }

        [XmlElement("RankingBoulder", IsNullable = true)]
        public int? RankingBoulder { get; set; }

        public Comp_CompetitorRegistrationApiModel()
        {
            this.License = this.TeamID = this.YearOfBirth = 0;
            this.Female = false;
            this.Surname = this.Name = this.Razr = String.Empty;
            this.Bib = null;
        }

    }

    [Serializable]
    [XmlType("MultipleTeamsClimber", Namespace = XmlApiDataConstants.NAMESPACE, IncludeInSchema = true)]
    public sealed class Comp_MultipleTeamsClimber : Comp_CompetitorRegistrationApiModel
    {
        [XmlArray(ElementName = "Teams")]
        [XmlArrayItem(ElementName = "Team")]
        public long[] Teams { get; set; }
    }

    [Serializable]
    [XmlType("ClimberPicture", Namespace = XmlApiDataConstants.NAMESPACE, IncludeInSchema = true)]
    [XmlRoot("ClimberPicture")]
    public sealed class Comp_ClimberPicture
    {
        [XmlElement("License")]
        public long ClimberId { get; set; }

        [XmlElement("Picture")]
        public byte[] Picture { get; set; }

        [XmlElement("PictureDate")]
        public DateTime PictureDate { get; set; }
    }
}