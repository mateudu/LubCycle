using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LubCycle.Core.Helpers;
using LubCycle.Core.Models.IMapHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LubCycle.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/location")]
    public class ApiLocationController : Controller
    {
        private IMapsHelper _mapsHelper;

        public ApiLocationController(IMapsHelper mapsHelper)
        {
            _mapsHelper = mapsHelper;
        }

        /// <summary>
        /// Returns geo-coordinates of requested query.
        /// </summary>
        /// <param name="query">Location query string</param>
        /// <remarks>Returns geo-coordinates of requested query.</remarks>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("query/{query}")]
        [ProducesResponseType(typeof(LocationResponse),(int)HttpStatusCode.OK)]
        [ResponseCache(CacheProfileName = "AddressCaching")]
        public async Task<IActionResult> GetLocationResponseAsync(string query)
        {
            var obj = await _mapsHelper.GetLocationResponseAsync(query);
            switch (obj.Status)
            {
                case LocationResponseStatus.Ok:
                    return StatusCode((int)HttpStatusCode.OK, obj);
                case LocationResponseStatus.BadRequest:
                    return StatusCode((int)HttpStatusCode.BadRequest, obj);
                case LocationResponseStatus.Error:
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError, obj);
            }
        }
    }
}