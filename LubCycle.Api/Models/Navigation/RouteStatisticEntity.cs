using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LubCycle.Api.Models.Navigation
{
    public class RouteStatisticEntity : Core.Models.Navigation.RouteStatistic
    {
        [Key]
        public int Id { get; set; }
    }
}
