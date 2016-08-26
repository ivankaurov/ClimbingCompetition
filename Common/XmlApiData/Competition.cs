using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace XmlApiData
{
    [XmlType("CompetitionRules", Namespace = XmlApiDataConstants.NAMESPACE, IncludeInSchema = true)]
    public enum CompetitionRules
    {
        Russian, International
    }

    [XmlRoot("Competitions", Namespace = XmlApiDataConstants.NAMESPACE)]
    [XmlType("CompetitionCollection", IncludeInSchema = true)]
    public sealed class API_CompetitionCollection :APIBaseRequest, IAPICollection<CompetitionApiModel>
    {
        [XmlElement("Competition")]
        public CompetitionApiModel[] Data { get; set; }

        public API_CompetitionCollection() { this.Data = new CompetitionApiModel[0]; }
        public API_CompetitionCollection(IEnumerable<CompetitionApiModel> data) { this.Data = XmlApiDataConstants.ToArray(data); }
    }

    [XmlType("Competition", Namespace = XmlApiDataConstants.NAMESPACE, IncludeInSchema = true)]
    public sealed class CompetitionApiModel : APIBaseRequest
    {
        [XmlElement("Iid")]
        public long Iid { get; set; }

        [XmlElement("FullName")]
        public String FullName { get; set; }

        [XmlElement("ShortName")]
        public String ShortName { get; set; }

        [XmlElement("StartDate")]
        public DateTime DateStart { get; set; }

        [XmlElement("EndDate")]
        public DateTime DateEnd { get; set; }

        [XmlElement("Rules")]
        public CompetitionRules Rules { get; set; }

        public override string ToString()
        {
            return this.FullName;
        }

        public CompetitionApiModel()
        {
            this.Iid = 0;
            this.DateStart = this.DateEnd = DateTime.MinValue;
            this.FullName = this.ShortName = String.Empty;
            this.Rules = CompetitionRules.International;
        }
    }
}
