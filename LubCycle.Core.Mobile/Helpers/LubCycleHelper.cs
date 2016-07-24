using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LubCycle.Core.Mobile.Models;
using Newtonsoft.Json;

namespace LubCycle.Core.Mobile.Helpers
{
    public class LubCycleHelper
    {
        private readonly string _serviceUrl = @"http://lubcycle.cloudapp.net";
        private readonly HttpClient _client = new HttpClient();

        public async Task<List<Place>> GetStationsAsync()
        {
            try
            {
                var url = new Uri($"{_serviceUrl}/api/stations");
                var result = await _client.GetAsync(url);
                var response = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<List<Place>>(response);
                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Route> GetRouteAsync(int startNumber, int destNumber)
        {
            try
            {
                var url = new Uri($"{_serviceUrl}/api/route/station-number/{startNumber}/{destNumber}");
                var result = await _client.GetAsync(url);
                var response = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<Route>(response);
                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<LocationResponse> GetLocationAsync(string query)
        {
            try
            {
                var url = new Uri($"{_serviceUrl}/api/location/query/{Uri.EscapeDataString(query)}");
                var result = await _client.GetAsync(url);
                var response = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<LocationResponse>(response);
                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
