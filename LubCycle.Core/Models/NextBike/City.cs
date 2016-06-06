using System.Collections.Generic;
using System.Xml.Serialization;

namespace LubCycle.Core.Models.NextBike
{
    [XmlRoot(ElementName = "city")]
    public class City
    {
        [XmlAttribute(AttributeName = "uid")]
        public string Uid { get; set; }

        [XmlAttribute(AttributeName = "lat")]
        public double Lat { get; set; }

        [XmlAttribute(AttributeName = "lng")]
        public double Lng { get; set; }

        [XmlAttribute(AttributeName = "zoom")]
        public double Zoom { get; set; }

        [XmlAttribute(AttributeName = "maps_icon")]
        public string MapsIcon { get; set; }

        [XmlAttribute(AttributeName = "alias")]
        public string Alias { get; set; }

        [XmlAttribute(AttributeName = "break")]
        public string Break { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "place")]
        public List<Place> Places { get; set; }
    }
}
