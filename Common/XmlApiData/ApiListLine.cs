using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace XmlApiData
{
    [XmlType("ResultLabel", IncludeInSchema = true, Namespace = XmlApiDataConstants.NAMESPACE)]
    public enum ResultLabel : short
    {
        RES = 0,
        DNS = 1,
        DSQ = 2
    }

    [XmlType("ResultListData", IncludeInSchema = true, Namespace=XmlApiDataConstants.NAMESPACE)]
    public sealed class ApiListLineCollection :APIBaseRequest, IAPICollection<ApiListLine>
    {
        [XmlElement(ElementName = "ClimberResult")]
        public ApiListLine[] Data { get; set; }

        public ApiListLineCollection(IEnumerable<ApiListLine> data) { this.Data = XmlApiDataConstants.ToArray(data); }
        public ApiListLineCollection() { this.Data = new ApiListLine[0]; }
    }

    [XmlType("ResultListLine", IncludeInSchema = true, Namespace = XmlApiDataConstants.NAMESPACE)]
    [XmlInclude(typeof(ApiListLineLead))]
    [XmlInclude(typeof(ApiListLineSpeed))]
    [XmlInclude(typeof(ApiListLineBoulder))]
    public abstract class ApiListLine : APIBaseRequest
    {
        [XmlElement("ListID")]
        public int ListID { get; set; }

        [XmlElement("Climber")]
        public int ClimberID { get; set; }

        [XmlElement("StartNumber")]
        public int StartNumber { get; set; }

        [XmlElement("Result")]
        public long? Result { get; set; }

        [XmlElement("TextResult")]
        public String ResText { get; set; }

        [XmlElement("PreQf")]
        public bool PreQf { get; set; }

        [XmlIgnore]
        public int ResultID { get; set; }
    }

    [XmlType("ResultListLineLead", IncludeInSchema=true, Namespace=XmlApiDataConstants.NAMESPACE)]
    public sealed class ApiListLineLead : ApiListLine {
        [XmlElement("Time")]
        public int? Time { get; set; }

        [XmlElement("TimeText")]
        public String TimeText { get; set; }
    }

    [XmlType("ResultListLineSpeed", IncludeInSchema = true, Namespace = XmlApiDataConstants.NAMESPACE)]
    public sealed class ApiListLineSpeed : ApiListLine
    {
        [XmlElement("Route1")]
        public long? Route1Data { get; set; }

        [XmlElement("Route1Text")]
        public String Route1Text { get; set; }

        [XmlElement("Route2")]
        public long? Route2Data { get; set; }
        [XmlElement("Route2Text")]
        public String Route2Text { get; set; }

        [XmlElement("Place")]
        public int? Pos { get; set; }

        private String posText = String.Empty;
        [XmlElement("PlaceText")]
        public String PosText { get { return posText; } set { posText = value ?? String.Empty; } }

        private String qf = String.Empty;
        [XmlElement("Qf")]
        public String Qf { get { return qf; } set { qf = (String.IsNullOrEmpty(value) ? String.Empty : value.Trim()); } }
    }

    [XmlType("ResultListLineBoudler", IncludeInSchema = true, Namespace = XmlApiDataConstants.NAMESPACE)]
    public sealed class ApiListLineBoulder: ApiListLine
    {
        [XmlElement("ResultCode")]
        public ResultLabel ResultCode { get; set; }

        [XmlArray("Routes")]
        [XmlArrayItem("Route")]
        public ApiBoulderResultRoute[] Routes { get; set; }

        public ApiListLineBoulder() { this.Routes = new ApiBoulderResultRoute[0]; }
    }

    [XmlType("ResultListLineBoudlerRoute", IncludeInSchema = true, Namespace = XmlApiDataConstants.NAMESPACE)]
    public sealed class ApiBoulderResultRoute
    {
        [XmlElement("RouteNumber")]
        public int Route { get; set; }

        [XmlElement("Top")]
        public int? Top { get; set; }

        [XmlElement("Bonus")]
        public int? Bonus { get; set; }
    }
}
