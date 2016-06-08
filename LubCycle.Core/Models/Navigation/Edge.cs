using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LubCycle.Core.Models.Navigation
{
    public struct Edge
    {
        public int To { get; set; }
        public double Duration { get; set; }
        public double Distance { get; set; }
    }
}
