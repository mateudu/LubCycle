using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LubCycle.Core.Models.NextBike;

namespace LubCycle.Core.Models.NextBike
{

    [XmlRoot(ElementName = "markers")]
    public class Marker
    {
        [XmlElement(ElementName = "country")]
        public List<Country> Countries { get; set; }

        public List<Place> GetStations(IEnumerable<string> cityUid)
        {
            var cities = from country in this.Countries
                from city in country.Cities
                where cityUid.Contains(city.Uid)
                select city;
            var list = new List<Place>();
            foreach (var city in cities)
            {
                list.AddRange(city.Places);
            }
            return list;
        }
    }
}
