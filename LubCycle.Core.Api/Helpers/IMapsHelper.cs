using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using LubCycle.Core.Api.Models.IMapHelper;

namespace LubCycle.Core.Helpers
{
    public interface IMapsHelper
    {
        Task<DistanceResponse> GetDistanceResponseAsync(double lat1, double lng1, double lat2, double lng2);
        Task<LocationResponse> GetLocationResponseAsync(string query);
    }
}
