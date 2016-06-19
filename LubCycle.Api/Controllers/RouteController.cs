using LubCycle.Api.Data;
using LubCycle.Core.Helpers;
using LubCycle.Core.Models.Navigation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        [HttpGet("station-number/{startNumber}/{destNumer}")]
        [ProducesResponseType(typeof(Core.Models.Navigation.Route), (int)HttpStatusCode.OK)]
        public IActionResult GetRouteByStationNumber(string startNumber, string destNumer)
        {
            var result = _navHelper.GetRoute(startNumber, destNumer, StationNumberType.StationNumber);
            if (result.Status == RouteStatus.IncorrectArguments)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("station-uid/{startUid}/{destUid}")]
        [ProducesResponseType(typeof(Core.Models.Navigation.Route), (int)HttpStatusCode.OK)]
        public IActionResult GetRouteByStationUid(string startUid, string destUid)
        {
            var result = _navHelper.GetRoute(startUid, destUid, StationNumberType.StationUid);
            if (result.Status == RouteStatus.IncorrectArguments)
                return BadRequest(result);

            return Ok(result);
        }
    }
}