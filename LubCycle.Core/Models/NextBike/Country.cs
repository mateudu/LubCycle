using System.Collections.Generic;
using System.Xml.Serialization;

namespace LubCycle.Core.Models.NextBike
{
    [XmlRoot(ElementName = "country")]
    public class Country
    {
        [XmlAttribute(AttributeName = "lat")]
        public double Lat { get; set; }

        [XmlAttribute(AttributeName = "lng")]
        public double Lng { get; set; }

        [XmlAttribute(AttributeName = "zoom")]
        public double Zoom { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "hotline")]
        public string Hotline { get; set; }

        [XmlAttribute(AttributeName = "domain")]
        public string Domain { get; set; }

        [XmlAttribute(AttributeName = "country")]
        public string _Country { get; set; }

        [XmlAttribute(AttributeName = "country_name")]
        public string CountryName { get; set; }

        [XmlAttribute(AttributeName = "terms")]
        public string Terms { get; set; }

        [XmlAttribute(AttributeName = "policy")]
        public string Policy { get; set; }

        [XmlAttribute(AttributeName = "website")]
        public string Website { get; set; }

        [XmlElement(ElementName = "city")]
        public List<City> Cities { get; set; }
    }
}