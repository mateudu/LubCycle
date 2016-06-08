using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LubCycle.Core.Models.NextBike;

namespace LubCycle.Core.Models.Navigation
{
    public class NavigationHelperSettings
    {
        public NavigationHelperSettings(List<Models.Navigation.RouteStatistic> routeStatistics, List<Place> stations)
        {
            this.RouteStatistics = routeStatistics;
            this.Stations = stations;
        }

        public List<Models.Navigation.RouteStatistic> RouteStatistics { get; set; }
        public List<Place> Stations { get; set; }
        public double MaximalSingleDistance { get; set; } = 7.00;
        public double MaximalSingleDuration { get; set; } = 2400;
    }
}
