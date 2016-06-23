using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using LubCycle.UWP.Models;

namespace LubCycle.UWP
{
    class CacheData
    {
        public static List<Place> Stations { get; set; }
        public static Geoposition Position { get; set; }
    }
}
