using System.ComponentModel.DataAnnotations;
using LubCycle.Core.Api.Models.Navigation;

namespace LubCycle.Core.Api.Models
{
    public class TravelDuration : RouteStatistic
    {
        [Key]
        public int Id { get; set; }
    }
}