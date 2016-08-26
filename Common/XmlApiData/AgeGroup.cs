using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlApiData
{
    [XmlRoot("Groups", Namespace = XmlApiDataConstants.NAMESPACE)]
    [XmlType("AgeGroupCollection", IncludeInSchema = true)]
    public sealed class API_AgeGroupCollection : APIBaseRequest, IAPICollection<Comp_AgeGroupApiModel>
    {
        [XmlElement("AgeGroup")]
        public Comp_AgeGroupApiModel[] Data { get; set; }

        public API_AgeGroupCollection() { this.Data = new Comp_AgeGroupApiModel[0]; }
        public API_AgeGroupCollection(IEnumerable<Comp_AgeGroupApiModel> data) { this.Data = XmlApiDataConstants.ToArray(data); }
    }

    [XmlType("AgeGroup", Namespace = XmlApiDataConstants.NAMESPACE, IncludeInSchema = true)]
    public sealed class Comp_AgeGroupApiModel : APIBaseRequest
    {
        [XmlElement(ElementName = "Iid")]
        public int Iid { get; set; }

        [XmlElement(ElementName = "Name")]
        public String Name { get; set; }

        [XmlElement(ElementName = "YearOld")]
        public int YearOld { get; set; }

        [XmlElement(ElementName = "YearYoung")]
        public int YearYoung { get; set; }

        [XmlElement(ElementName = "Female")]
        public bool Female { get; set; }

        public Comp_AgeGroupApiModel()
        {
            this.Iid = this.YearOld = this.YearYoung = 0;
            this.Female = false;
            this.Name = String.Empty;
        }
    }
}