using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace LubCycle.UWP.Models
{
    internal class StationsListViewItem
    {
        public Place Station { get; set; }
        public double Distance { get; set; }
        public Geopoint Geopoint { get; set; }
        public Point AnchorPoint { get; set; } = new Point(0, 1.0);
    }
}