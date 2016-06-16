using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LubCycle.Core.Models.NextBike;
using LubCycle.Core.Models;

namespace LubCycle.Core.Helpers
{
    public class NextBikeHelper
    {
        private Marker _lastResponseInfo;
        private DateTime _lastResponseTime = new DateTime(2000, 1, 1);
        private readonly IEnumerable<string> _cityUids;
        private readonly TimeSpan _updateSpan;

        public NextBikeHelper(IEnumerable<string> cityUids, double updateSpan=15)
        {
            this._cityUids = cityUids;
            this._updateSpan = TimeSpan.FromSeconds(updateSpan);
        }

        public NextBikeHelper(string cityUids, double updateSpan=15) : this(cityUids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries),updateSpan)
        {   }
        
        /// <summary>
        ///  Returns NextBike info. Updates every 15 seconds.
        /// </summary>
        private async Task<Marker> GetNextbikeInfoAsync()
        {
            if (DateTime.Now - _lastResponseTime > _updateSpan)
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
        ///  Returns stations
        /// </summary>
        /// <param name="cityUid">List of cityUids</param>
        public async Task<List<Place>> GetStationsAsync()
        {
            return (await GetNextbikeInfoAsync()).GetStations(_cityUids);
        }
    }
}
