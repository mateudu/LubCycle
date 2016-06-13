using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using LubCycle.Api.Data;
using LubCycle.Core;
using LubCycle.Core.Helpers;
using LubCycle.Core.Models;
using LubCycle.Core.Models.Navigation;
using LubCycle.Core.Models.NextBike;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LubCycle.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Route")]
    public class RouteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly NavigationHelper _navHelper;

        public RouteController(ApplicationDbContext context, NavigationHelper navHelper)
        {
            _context = context;
            _navHelper = navHelper;
        }


        //private async Task LoadGraph()
        //{
        //    if (Core.GeoHelper.RouteStatistics == null || Core.GeoHelper.RouteStatistics.Count == 0)
        //    {
        //        var obj = _context.TravelDurations.ToList();
        //        Core.GeoHelper.RouteStatistics = obj.Cast<Core.Models.Navigation.RouteStatistic>().ToList();
        //    }
        //    if (Core.GeoHelper.Stations == null)
        //    {
        //        Core.GeoHelper.Stations = await LubCycle.Core.NextBikeHelper.GetStationsAsync(Startup.Configuration["CITY_UIDS"]);
        //    }
        //}

        [HttpGet("{startUid}/{destUid}")]
        [ProducesResponseType(typeof(Core.Models.Navigation.Route),(int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRoute(string startUid, string destUid)
        {
            // Load travel times between stations
            //await LoadGraph();

            var result = _navHelper.GetRoute(startUid, destUid);

            //var result = Core.GeoHelper.GetRoute(startUid, destUid);

            if (result.Status == RouteStatus.IncorrectArguments)
                return BadRequest(result);

            return Ok(result);
        }
    }
}