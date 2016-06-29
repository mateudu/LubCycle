using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI;
using LubCycle.UWP.Models;

namespace LubCycle.UWP
{
    class CacheData
    {
        public static List<Place> Stations { get; set; }
        public static Geoposition Position { get; set; }
        public static List<StationsListViewItem> StationListViewItems { get; set; }
        public static Color CustomColor { get; set; } = Colors.DarkBlue;
        public static Color ContrastColor { get; set; } = Colors.White;
        public static Color SystemAccentColor { get; set; } = Colors.Blue;
    }
}
