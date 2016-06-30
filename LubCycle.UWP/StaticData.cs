using Windows.Devices.Geolocation;
using Windows.UI;

namespace LubCycle.UWP
{
    internal static class StaticData
    {
        public static readonly string MapServiceToken =
            @"pYvhKdz5gxUCF2HNVGo3~0mOzxV8x2lXr6tQsDZF8Mg~Atqlf1dFSmCvM1TE8DKbH_b0tAZ6vlAAFlAFFc981m8Kb2WMZDeJmQKv3fmdpauE";

        public static string AppUrl = @"http://lubcycle.pl";

        public static readonly Geopoint DefaultMapCenter = new Geopoint(new BasicGeoposition() { Latitude = 51.2465, Longitude = 22.5684 });
        public static readonly Color[] RouteColors = new Color[] {Color.FromArgb(255, 76, 175, 80), Colors.DodgerBlue};
        public static double DefaultMapZoom = 15.0;
    }
}