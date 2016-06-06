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
        public BingHelper(string bingMapsApiKey)
        {
            this._bingMapsApiKey = bingMapsApiKey;
        }

        private readonly string _bingMapsApiKey;

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

        public async Task<Models.BingMaps.RootObject> GetDirectionsAsync(Models.NextBike.Place start,
            Models.NextBike.Place destination)
        {
            return await GetDirectionsAsync(start.Lat, start.Lng, destination.Lat, destination.Lng);
        }
    }
}
