using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace LubCycle.UWP.Models
{
    class StationsListViewItem
    {
        public Place Station { get; set; }
        public double Distance { get; set; }
        public Geopoint Geopoint { get; set; }
        public Point AnchorPoint { get; set; } = new Point(0, 1.0);
    }
}
