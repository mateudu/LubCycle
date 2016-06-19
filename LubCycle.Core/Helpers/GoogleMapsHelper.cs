using LubCycle.Core.Models.NextBike;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LubCycle.Core.Helpers
{
    public class GoogleMapsHelper
    {
        private readonly string _apiKey;
        private HttpClient client;

        public GoogleMapsHelper(string apiKey)
        {
            this._apiKey = apiKey;
        }

        public async Task<Core.Models.GoogleMaps.RootObject> GetDistanceAsync(double startLat, double startLng, double destLat, double destLng)
        {
            var uri = new Uri($"https://maps.googleapis.com/maps/api/distancematrix/json" +
                              $"?origins=" +
                              $"{startLat.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                              $"{startLng.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                              $"&destinations=" +
                              $"{destLat.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                              $"{destLng.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                              $"&mode=bicycling" +
                              $"&unites=metrics&key={_apiKey}");
            if (client == null)
            {
                client = new HttpClient();
            }
            var response = (await client.GetAsync(uri));
            var result = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<Core.Models.GoogleMaps.RootObject>(result);
            return obj;
        }

        public async Task<Core.Models.GoogleMaps.RootObject> GetDistanceAsync(Place start, Place dest)
        {
            return await GetDistanceAsync(start.Lat, start.Lng, dest.Lat, dest.Lng);
        }
    }
}