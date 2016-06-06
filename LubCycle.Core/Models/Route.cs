using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LubCycle.Core.Models.NextBike;

namespace LubCycle.Core.Models
{
    public class Route
    {
        public Place Start { get; set; }
        public Place Destination { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        //public int Bikes { get; set; }
        public double Duration { get; set; }
        public double Distance { get; set; }
        public List<Place> Stations { get; set; }
    }
}
