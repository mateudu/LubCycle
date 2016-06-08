using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LubCycle.Core.Models.Navigation;

namespace LubCycle.Core.Models
{
    public class TravelDuration : RouteStatistic
    {
        [Key]
        public int Id { get; set; }
        //public override string Station1Uid { get; set; }
        //public override string Station2Uid { get; set; }
        //public override double Duration { get; set; }
        //public override double Distance { get; set; }
    }
}
