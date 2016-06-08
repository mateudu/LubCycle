using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using LubCycle.Api.Data;
using LubCycle.Core;
using LubCycle.Core.Models;
using LubCycle.Core.Models.Geo;
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

        [HttpGet("{startUid}/{destUid}")]
        [ProducesResponseType(typeof(Core.Models.Geo.Route),(int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRoute(string startUid, string destUid)
        {
            // Load travel times between stations
            await LoadGraph();

            var result = Core.GeoHelper.GetRoute(startUid, destUid);

            if (result.Status == RouteStatus.IncorrectArguments)
                return BadRequest(result);

            return Ok(result);
        }
    }
}