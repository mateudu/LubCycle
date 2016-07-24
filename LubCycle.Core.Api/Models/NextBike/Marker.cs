using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace LubCycle.Core.Api.Models.NextBike
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