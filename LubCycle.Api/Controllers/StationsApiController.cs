using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LubCycle.Core.Models.NextBike;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Template;

namespace LubCycle.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Stations")]
    public class StationsApiController : Controller
    {
        private Core.Helpers.NextBikeHelper _nextBikeHelper;
        public StationsApiController(Core.Helpers.NextBikeHelper nextBikeHelper)
        {
            this._nextBikeHelper = nextBikeHelper;
        }

        [HttpGet]
        public async Task<List<Place>> GetStations()
        {
            return await _nextBikeHelper.GetStationsAsync();
        }
    }
}