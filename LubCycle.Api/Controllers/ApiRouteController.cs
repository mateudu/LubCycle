using LubCycle.Api.Data;
using LubCycle.Core.Helpers;
using LubCycle.Core.Models.Navigation;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace LubCycle.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/route")]
    public class ApiRouteController : Controller
    {
        private readonly NextBikeHelper _nextBikeHelper;
        private readonly NavigationHelper _navHelper;

        public ApiRouteController(NextBikeHelper nextBikeHelper, NavigationHelper navHelper)
        {
            _nextBikeHelper = nextBikeHelper;
            _navHelper = navHelper;
        }

        [HttpGet("station-number/{startNumber}/{destNumer}")]
        [ProducesResponseType(typeof(Core.Models.Navigation.Route), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRouteByStationNumberAsync(string startNumber, string destNumer)
        {
            _navHelper.UpdateStations(await _nextBikeHelper.GetStationsAsync());
            var result = _navHelper.GetRoute(startNumber, destNumer, StationNumberType.StationNumber);
            if (result.Status == RouteStatus.IncorrectArguments)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("station-uid/{startUid}/{destUid}")]
        [ProducesResponseType(typeof(Core.Models.Navigation.Route), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRouteByStationUidAsync(string startUid, string destUid)
        {
            _navHelper.UpdateStations(await _nextBikeHelper.GetStationsAsync());
            var result = _navHelper.GetRoute(startUid, destUid, StationNumberType.StationUid);
            if (result.Status == RouteStatus.IncorrectArguments)
                return BadRequest(result);

            return Ok(result);
        }
    }
}