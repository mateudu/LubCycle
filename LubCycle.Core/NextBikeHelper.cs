using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LubCycle.Core.Models.NextBike;
using LubCycle.Core.Models;

namespace LubCycle.Core
{
    public static class NextBikeHelper
    {
        static NextBikeHelper()
        {
            _lastResponseTime = new DateTime(2000,1,1);
        }

        /// <summary>
        ///  Returns NextBike info. Updates every 15 seconds.
        /// </summary>
        public static async Task<Marker> GetNextbikeInfoAsync()
        {
            if (DateTime.Now - _lastResponseTime > TimeSpan.FromSeconds(15))
            {
                var client = new HttpClient();
                var response = await client.GetAsync(new Uri(@"http://nextbike.net/maps/nextbike-official.xml"));

                using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(Marker));
                        Marker obj = (Marker)serializer.Deserialize(reader);

                        _lastResponseInfo = obj;
                        _lastResponseTime = DateTime.Now;
                    }
                }
            }
            return _lastResponseInfo;

        }

        /// <summary>
        ///  Returns stations in cityUids.
        /// </summary>
        /// <param name="cityUid">List of cityUids</param>
        public static async Task<List<Place>> GetStationsAsync(IEnumerable<string> cityUid)
        {
            if (_stations == null)
            {
                _stations = (await GetNextbikeInfoAsync()).GetStations(cityUid);
            }
            return _stations;
        }

        /// <summary>
        ///  Returns stations in cityUids.
        /// </summary>
        /// <param name="cityUid">CityUids, i.e "1,2,3,4"</param>
        public static async Task<List<Place>> GetStationsAsync(string cityUid)
        {
            return await GetStationsAsync(cityUid.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries));
        }

        private static List<Place> _stations;
        private static Marker _lastResponseInfo;
        private static DateTime _lastResponseTime;
    }
}
