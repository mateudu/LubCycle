using LubCycle.Core.Api.Models.NextBike;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LubCycle.Core.Api.Models.GoogleMaps;
using LubCycle.Core.Api.Models.IMapHelper;

namespace LubCycle.Core.Helpers
{
    public class GoogleMapsHelper : IMapsHelper
    {
        private readonly string _apiKey;
        private HttpClient _client;
        private readonly string _serviceUrl = @"https://maps.googleapis.com";
        public GoogleMapsHelper(string apiKey)
        {
            this._apiKey = apiKey;
            _client = new HttpClient();
        }

        // IMapHelper implementation

        public async Task<DistanceResponse> GetDistanceResponseAsync(double lat1, double lng1, double lat2, double lng2)
        {
            var response = await GetDistanceAsync(lat1, lng1, lat2, lng2);
            var obj = response?.rows?.FirstOrDefault()?.elements?.FirstOrDefault();
            if (obj != null)
            {
                double distance, duration;
                double.TryParse(obj.duration.value.ToString(), out duration);
                double.TryParse(obj.distance.value.ToString(), out distance);
                return new DistanceResponse
                {
                    Distance = distance,
                    Duration = duration
                };
            }

            return null;
        }

        public async Task<LocationResponse> GetLocationResponseAsync(string query)
        {
            var uri = new Uri($"{_serviceUrl}/maps/api/geocode/json" +
                              $"?address={query}" +
                              $"&key={_apiKey}");

            try
            {
                var response = await _client.GetAsync(uri);
                var result = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<Core.Api.Models.GoogleMaps.GeoCode.RootObject>(result);

                if (obj != null)
                {
                    switch (obj.status)
                    {
                        case "OK":
                            var loc = obj?.results?.FirstOrDefault()?.geometry?.location;
                            return new LocationResponse
                            {
                                Status = LocationResponseStatus.Ok,
                                Message = obj.status,
                                Lat = loc?.lat,
                                Lng = loc?.lng
                            };
                        case "ZERO_RESULTS":
                        case "OVER_QUERY_LIMIT":
                        case "REQUEST_DENIED":
                        case "INVALID_REQUEST":
                        case "UNKNOWN_ERROR":
                            return new LocationResponse
                            {
                                Status = LocationResponseStatus.BadRequest,
                                Message = obj.status
                            };
                    }
                }
            }
            catch (Exception exc)
            {
                // TODO: Log Google Maps API error.
            }
            
            return new LocationResponse
            {
                Status = LocationResponseStatus.Error,
                Message = "Internal error/unknown"
            };
        }

        

        private async Task<Core.Api.Models.GoogleMaps.DistanceMatrix.RootObject> GetDistanceAsync(double startLat, double startLng, double destLat, double destLng)
        {
            var uri = new Uri($"{_serviceUrl}/maps/api/distancematrix/json" +
                              $"?origins=" +
                              $"{startLat.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                              $"{startLng.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                              $"&destinations=" +
                              $"{destLat.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                              $"{destLng.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                              $"&mode=bicycling" +
                              $"&unites=metrics&key={_apiKey}");

            var response = (await _client.GetAsync(uri));
            var result = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<Core.Api.Models.GoogleMaps.DistanceMatrix.RootObject>(result);
            return obj;
        }
    }
}