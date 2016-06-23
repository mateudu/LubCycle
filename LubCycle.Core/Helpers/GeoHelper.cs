using System;

namespace LubCycle.Core.Helpers
{
    public class GeoHelper
    {
        // Calc distance using Geo-coordinates.
        private const double EQuatorialEarthRadius = 6378.1370D;

        private const double D2R = (Math.PI / 180D);


        private static double CalcDistanceInKilometers(double lat1, double lng1, double lat2, double lng2)
        {
            double dlong = (lng2 - lng1) * D2R;
            double dlat = (lat2 - lat1) * D2R;
            double a = Math.Pow(Math.Sin(dlat / 2D), 2D) + Math.Cos(lat1 * D2R) * Math.Cos(lat2 * D2R) * Math.Pow(Math.Sin(dlong / 2D), 2D);
            double c = 2D * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1D - a));
            double d = EQuatorialEarthRadius * c;
            return d;
        }

        /// <summary>
        ///  Returns distance from A to B in km.
        /// </summary>
        /// <param name="lat1">Point A - Latitude</param>
        /// <param name="lng1">Point A - Longitude</param>
        /// <param name="lat2">Point B - Latitude</param>
        /// <param name="lng2">Point B - Longitude</param>
        public static double CalcDistanceInKilometers(double? lat1, double? lng1, double? lat2, double? lng2)
        {
            if (!lat1.HasValue || !lng1.HasValue || !lat2.HasValue || !lng2.HasValue)
            {
                return 0;
            }
            else
            {
                return CalcDistanceInKilometers(lat1.Value, lng1.Value, lat2.Value, lng2.Value);
            }
        }

        /// <summary>
        ///  Returns distance from A to B in meters.
        /// </summary>
        /// <param name="lat1">Point A - Latitude</param>
        /// <param name="lng1">Point A - Longitude</param>
        /// <param name="lat2">Point B - Latitude</param>
        /// <param name="lng2">Point B - Longitude</param>
        public static double CalcDistanceInMeters(double? lat1, double? lng1, double? lat2, double? lng2)
        {
            return (CalcDistanceInKilometers(lat1, lng1, lat2, lng2) * 1000.0);
        }
    }
}