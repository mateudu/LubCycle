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
    [Route("api/route")]
    public class RouteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly NavigationHelper _navHelper;

        public RouteController(ApplicationDbContext context, NavigationHelper navHelper)
        {
            _context = context;
            _navHelper = navHelper;
        }

        [HttpGet("station-uid/{startUid}/{destUid}")]
        [ProducesResponseType(typeof(Core.Models.Navigation.Route),(int)HttpStatusCode.OK)]
        public IActionResult GetRouteByStationUid(string startUid, string destUid)
        {
            var result = _navHelper.GetRoute(startUid, destUid, StationNumberType.StationUid);
            if (result.Status == RouteStatus.IncorrectArguments)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("station-number/{startNumber}/{destNumer}")]
        [ProducesResponseType(typeof(Core.Models.Navigation.Route), (int)HttpStatusCode.OK)]
        public IActionResult GetRouteByStationNumber(string startNumber, string destNumer)
        {
            var result = _navHelper.GetRoute(startNumber, destNumer, StationNumberType.StationNumber);
            if (result.Status == RouteStatus.IncorrectArguments)
                return BadRequest(result);

            return Ok(result);
        }
    }
}