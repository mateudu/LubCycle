using LubCycle.Core.Models.NextBike;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LubCycle.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/stations")]
    [ResponseCache(CacheProfileName = "StationsCaching")]
    public class ApiStationsController : Controller
    {
        private Core.Helpers.NextBikeHelper _nextBikeHelper;

        public ApiStationsController(Core.Helpers.NextBikeHelper nextBikeHelper)
        {
            this._nextBikeHelper = nextBikeHelper;
        }

        /// <summary>
        /// Returns station.
        /// </summary>
        /// <param name="number">Station number</param>
        /// <remarks>Returns station</remarks>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("station-number/{number}")]
        [ProducesResponseType(typeof(Place), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetStationByStationNumberAsync(int number)
        {
            var obj = (await GetStationsAsync()).FirstOrDefault(x => x.Number == number);
            if (obj == null)
            {
                return BadRequest(new { error_message = "Station not found" });
            }
            else
            {
                return Ok(obj);
            }
        }

        /// <summary>
        /// Returns all stations.
        /// </summary>
        /// <remarks>Returns all stations</remarks>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("station-uid/{number}")]
        [ProducesResponseType(typeof(Place), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetStationByStationUidAsync(string number)
        {
            var obj = (await GetStationsAsync()).FirstOrDefault(x => x.Uid == number);
            if (obj == null)
            {
                return BadRequest(new { error_message = "Station not found" });
            }
            else
            {
                return Ok(obj);
            }
        }

        [HttpGet]
        public async Task<List<Place>> GetStationsAsync()
        {
            return await _nextBikeHelper.GetStationsAsync();
        }
    }
}