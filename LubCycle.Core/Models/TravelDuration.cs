using LubCycle.Core.Models.Navigation;
using System.ComponentModel.DataAnnotations;

namespace LubCycle.Core.Models
{
    public class TravelDuration : RouteStatistic
    {
        [Key]
        public int Id { get; set; }
    }
}