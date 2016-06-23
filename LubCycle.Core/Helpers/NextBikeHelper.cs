using LubCycle.Core.Models.NextBike;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LubCycle.Core.Helpers
{
    public class NextBikeHelper
    {
        private Marker _lastResponseInfo;
        private DateTime _lastResponseTime = new DateTime(2000, 1, 1);
        private readonly IEnumerable<string> _cityUids;
        private readonly TimeSpan _updateSpan;

        public NextBikeHelper(IEnumerable<string> cityUids, double updateSpan = 15)
        {
            this._cityUids = cityUids;
            this._updateSpan = TimeSpan.FromSeconds(updateSpan);
        }

        public NextBikeHelper(string cityUids, double updateSpan = 15) : this(cityUids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries), updateSpan)
        { }

        /// <summary>
        ///  Returns NextBike info.
        /// </summary>
        private async Task<Marker> GetNextbikeInfoAsync()
        {
            try
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
            catch (Exception exc)
            {
                // TODO: Log NextBike API connection error.
                Console.WriteLine($"{DateTime.Now} NextBike API connection error.");
                return null;
            }
        }

        /// <summary>
        ///  Returns stations
        /// </summary>
        public async Task<List<Place>> GetStationsAsync()
        {
            return (await GetNextbikeInfoAsync())?.GetStations(_cityUids);
        }
    }
}