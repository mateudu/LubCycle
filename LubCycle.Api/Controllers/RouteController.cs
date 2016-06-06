using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using LubCycle.Api.Data;
using LubCycle.Core;
using LubCycle.Core.Models;
using LubCycle.Core.Models.NextBike;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LubCycle.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Route")]
    public class RouteController : Controller
    {
        //private static List<Core.Models.TravelDuration> _durations;
        private readonly ApplicationDbContext _context;
        //private List<Place> stations;

        public RouteController(ApplicationDbContext context)
        {
            _context = context;
        }


        private async Task LoadGraph()
        {
            if (Core.GeoHelper.TravelDurations == null || Core.GeoHelper.TravelDurations.Count == 0)
            {
                var obj = _context.TravelDurations.ToList();
                Core.GeoHelper.TravelDurations = obj;
            }
            if (Core.GeoHelper.Stations == null)
            {
                Core.GeoHelper.Stations = await LubCycle.Core.NextBikeHelper.GetStationsAsync(Startup.Configuration["CITY_UIDS"]);
            }
            
        }

        public async Task<IActionResult> GetRoute(string startUid, string destinationUid, int bikes = 1)
        {
            // Load travel times between stations
            await LoadGraph();
            //var nextBikeInfo = await LubCycle.Core.NextBikeHelper.GetNextbikeInfoAsync();
            

            var startStation = Core.GeoHelper.Stations.FirstOrDefault(x => x.Uid == startUid);
            var destStation = Core.GeoHelper.Stations.FirstOrDefault(x => x.Uid == destinationUid);
            if (startStation == null || destStation == null)
            {
                return BadRequest(new { error_message = "Incorrect start/destination station uid." });
            }

            var stations = Core.GeoHelper.CalcRoute(startStation.Uid, destStation.Uid);
            double duration = 0, distance = 0;
            for (int i = 1; i < stations.Count; i++)
            {
                var el = Core.GeoHelper.TravelDurations.FirstOrDefault(
                    x => x.Station1Uid == stations[i - 1].Uid && x.Station2Uid == stations[i].Uid);
                if (el != null)
                {
                    duration += el.Duration;
                    distance += el.Distance;
                }
            }

            var result = new Route()
            {
                Start = startStation,
                Destination = destStation,
                StartTime = DateTime.Now,
                Stations = stations,
                EndTime = DateTime.Now.AddSeconds(duration),
                Distance = distance,
                Duration = (int)duration
            };

            return Ok(result);
        }
    }
}