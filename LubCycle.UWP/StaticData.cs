using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace LubCycle.UWP
{
    static class StaticData
    {
        public static readonly string MapServiceToken =
            @"pYvhKdz5gxUCF2HNVGo3~0mOzxV8x2lXr6tQsDZF8Mg~Atqlf1dFSmCvM1TE8DKbH_b0tAZ6vlAAFlAFFc981m8Kb2WMZDeJmQKv3fmdpauE";
        public static readonly Geopoint DefaultMapCenter = new Geopoint(new BasicGeoposition() { Latitude = 51.2465, Longitude = 22.5684 });
    }
}
