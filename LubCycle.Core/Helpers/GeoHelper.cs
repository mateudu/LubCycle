using System;

namespace LubCycle.Core.Helpers
{
    public class GeoHelper
    {
        // Invoke 'LoadGraph()' method before calling 'GetRoute()'!
        //private async Task LoadGraph()
        //{
        //    if (Core.GeoHelper.TravelDurations == null || Core.GeoHelper.TravelDurations.Count == 0)
        //    {
        //        var obj = _context.TravelDurations.ToList();
        //        Core.GeoHelper.TravelDurations = obj;
        //    }
        //    if (Core.GeoHelper.Stations == null)
        //    {
        //        Core.GeoHelper.Stations = await LubCycle.Core.NextBikeHelper.GetStationsAsync(Startup.Configuration["CITY_UIDS"]);
        //    }
        //}

        // Calc distance using Geo-coordinates.
        private const double EQuatorialEarthRadius = 6378.1370D;

        private const double D2R = (Math.PI / 180D);

        /// <summary>
        ///  Returns distance from A to B in km.
        /// </summary>
        /// <param name="lat1">Point A - Latitude</param>
        /// <param name="lng1">Point A - Longitude</param>
        /// <param name="lat2">Point B - Latitude</param>
        /// <param name="lng2">Point B - Longitude</param>
        public static double CalcDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double dlong = (lng2 - lng1) * D2R;
            double dlat = (lat2 - lat1) * D2R;
            double a = Math.Pow(Math.Sin(dlat / 2D), 2D) + Math.Cos(lat1 * D2R) * Math.Cos(lat2 * D2R) * Math.Pow(Math.Sin(dlong / 2D), 2D);
            double c = 2D * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1D - a));
            double d = EQuatorialEarthRadius * c;
            return d;
        }

        /// <summary>
        ///  Returns distance between station A and B in km.
        /// </summary>
        /// <param name="start">Station A</param>
        /// <param name="destination">Station B</param>
        public static double CalcDistance(Models.NextBike.Place start, Models.NextBike.Place destination)
        {
            return CalcDistance(start.Lat, start.Lng, destination.Lat, destination.Lng);
        }
    }
}