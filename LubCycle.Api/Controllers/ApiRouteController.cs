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
    [ResponseCache(CacheProfileName = "StationsCaching")]
    public class ApiRouteController : Controller
    {
        private readonly NextBikeHelper _nextBikeHelper;
        private readonly NavigationHelper _navHelper;

        public ApiRouteController(NextBikeHelper nextBikeHelper, NavigationHelper navHelper)
        {
            _nextBikeHelper = nextBikeHelper;
            _navHelper = navHelper;
        }


        /// <summary>
        /// Returns route.
        /// </summary>
        /// <param name="startNumber">Start station number</param>
        /// <param name="destNumer">End station number</param>
        /// <remarks>Returns route between requested stations.</remarks>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal Server Error</response>
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

        /// <summary>
        /// Returns route.
        /// </summary>
        /// <param name="startUid">Start station Uid</param>
        /// <param name="destUid">End station Uid</param>
        /// <remarks>Returns route between requested stations.</remarks>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal Server Error</response>
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