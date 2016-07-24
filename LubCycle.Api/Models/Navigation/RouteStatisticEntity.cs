using System.ComponentModel.DataAnnotations;

namespace LubCycle.Api.Models.Navigation
{
    public class RouteStatisticEntity : Core.Api.Models.Navigation.RouteStatistic
    {
        [Key]
        public int Id { get; set; }
    }
}