using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace XmlApiData
{
    [XmlRoot("Teams", Namespace = XmlApiDataConstants.NAMESPACE)]
    [XmlType("TeamCollection", IncludeInSchema = true)]
    public sealed class API_RegionCollection : APIBaseRequest, IAPICollection<RegionApiModel>
    {
        [XmlElement(ElementName = "Team")]
        public RegionApiModel[] Data { get; set; }

        public API_RegionCollection() { this.Data = new RegionApiModel[0]; }
        public API_RegionCollection(IEnumerable<RegionApiModel> data) { this.Data = XmlApiDataConstants.ToArray(data); }
    }

    [XmlType("Team", IncludeInSchema = true, Namespace = XmlApiDataConstants.NAMESPACE)]
    public sealed class RegionApiModel : APIBaseRequest
    {
        [XmlElement(ElementName = "Iid")]
        public long Iid { get; set; }

        [XmlElement(ElementName = "Name")]
        public String Name { get; set; }
        public RegionApiModel()
        {
            this.Iid = 0;
            this.Name = String.Empty;
        }
    }
}
