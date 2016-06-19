using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LubCycle.UWP.Models;
using Newtonsoft.Json;

namespace LubCycle.UWP.Helpers
{
    class LubCycleHelper
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
            catch (Exception exc)
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
            catch (Exception exc)
            {
                return null;
            }
        }
    }
}
