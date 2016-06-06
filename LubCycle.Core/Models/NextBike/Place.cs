using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LubCycle.Core.Models.NextBike
{
    [XmlRoot(ElementName = "place")]
    public class Place
    {
        [XmlAttribute(AttributeName = "uid")]
        public string Uid { get; set; }

        [XmlAttribute(AttributeName = "lat")]
        public double Lat { get; set; }

        [XmlAttribute(AttributeName = "lng")]
        public double Lng { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "spot")]
        public string Spot { get; set; }

        [XmlAttribute(AttributeName = "number")]
        public int Number { get; set; }

        [XmlAttribute(AttributeName = "bikes")]
        public string Bikes { get; set; }

        [XmlAttribute(AttributeName = "terminal_type")]
        public string TerminalType { get; set; }

        [XmlAttribute(AttributeName = "bike_numbers")]
        public string BikeNumbers { get; set; }
    }
}
