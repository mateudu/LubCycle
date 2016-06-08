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
        [HttpGet]
        public async Task<List<Place>> GetStations()
        {
            await Core.NextBikeHelper.GetNextbikeInfoAsync();
            return await LubCycle.Core.NextBikeHelper.GetStationsAsync(Startup.Configuration["CITY_UIDS"]);
        }
    }
}