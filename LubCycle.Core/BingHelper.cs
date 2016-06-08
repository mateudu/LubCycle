using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LubCycle.Core
{
    public class BingHelper
    {
        /// <summary>
        ///  Returns new instance of BingHelper.
        /// </summary>
        /// <param name="bingMapsApiKey">Bing Maps API key</param>
        public BingHelper(string bingMapsApiKey)
        {
            this._bingMapsApiKey = bingMapsApiKey;
        }

        private readonly string _bingMapsApiKey;

        /// <summary>
        ///  Returns directions between point A and B.
        /// </summary>
        /// <param name="lat1">Point A - Latitude</param>
        /// <param name="lng1">Point A - Longitude</param>
        /// <param name="lat2">Point B - Latitude</param>
        /// <param name="lng2">Point B - Longitude</param>
        public async Task<Models.BingMaps.RootObject> GetDirectionsAsync(double lat1, double lng1, double lat2, double lng2)
        {
            var client = new HttpClient();
            string requestUrl = @"http://dev.virtualearth.net/REST/V1/Routes/Walking?o=json&wp.0=" +
                                lat1.ToString(System.Globalization.CultureInfo.InvariantCulture) + @"," +
                                lng1.ToString(System.Globalization.CultureInfo.InvariantCulture) +
                                @"&wp.1=" +
                                lat2.ToString(System.Globalization.CultureInfo.InvariantCulture) + @"," +
                                lng2.ToString(System.Globalization.CultureInfo.InvariantCulture) +
                                @"&key=" + _bingMapsApiKey;
            var response = await client.GetAsync(new Uri(requestUrl));
            var str = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<Models.BingMaps.RootObject>(str);
            return obj;
        }

        /// <summary>
        ///  Returns directions between station A and B.
        /// </summary>
        /// <param name="start">Station A</param>
        /// <param name="destination">Station B</param>
        public async Task<Models.BingMaps.RootObject> GetDirectionsAsync(Models.NextBike.Place start,
            Models.NextBike.Place destination)
        {
            return await GetDirectionsAsync(start.Lat, start.Lng, destination.Lat, destination.Lng);
        }
    }
}
