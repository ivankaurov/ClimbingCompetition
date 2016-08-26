using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace XmlApiData
{
    [XmlType("ListTypes", IncludeInSchema = true, Namespace = XmlApiDataConstants.NAMESPACE)]
    public enum ListTypeEnum
    {
        SpeedQualy, SpeedQualy2, SpeedFinal,
        LeadFlash, LeadGroups, LeadSimple,
        BoulderGroups, BoulderSimple, BoulderSuper,
        TeamLead, TeamSpeed, TeamBoulder, TeamGeneral,
        Combined,
        General, Unknown
    }

    [XmlType("ListHeaderCollection", IncludeInSchema = true, Namespace = XmlApiDataConstants.NAMESPACE)]
    public sealed class ApiListHeaderCollection : APIBaseRequest, IAPICollection<ApiListHeader>
    {
        [XmlElement(ElementName = "List")]
        public  ApiListHeader[] Data { get; set; }
        public ApiListHeaderCollection() { this.Data = new ApiListHeader[0]; }
        public ApiListHeaderCollection(IEnumerable<ApiListHeader> data) { this.Data = XmlApiDataConstants.ToArray(data); }
    }

    [XmlType("ListHeader", Namespace = XmlApiDataConstants.NAMESPACE, IncludeInSchema = true)]
    public sealed class ApiListHeader : APIBaseRequest
    {
        [XmlElement("Id")]
        public int Iid { get; set; }

        [XmlElement("ParentList")]
        public int? ParentList { get; set; }

        [XmlElement("PreviousRound")]
        public int? PreviousRound { get; set; }

        [XmlElement("GroupId")]
        public int? GroupId { get; set; }

        [XmlElement("Type")]
        public ListTypeEnum ListType { get; set; }

        [XmlElement("Discipline")]
        public String Style { get; set; }

        [XmlElement("RoundName")]
        public String Round { get; set; }

        [XmlElement("Quota")]
        public int Quota { get; set; }

        [XmlElement("RouteQuantity")]
        public int? RouteQuantity { get; set; }

        [XmlElement("CurrentlyOn")]
        public bool Live { get; set; }

        [XmlElement("StartTime")]
        public String StartTime { get; set; }

        [XmlElement("LastRefresh")]
        public DateTime LastRefresh { get; set; }

        [XmlElement("CompetitionRules")]
        public CompetitionRules Rules { get; set; }

        [XmlElement("BestResultInQf")]
        public bool BestQf { get; set; }
    }
}