using System.Collections.Generic;

namespace LubCycle.Core.Api.Models.BingMaps
{
    public class ActualEnd
    {
        public string Type { get; set; }
        public List<double> Coordinates { get; set; }
    }

    public class ActualStart
    {
        public string Type { get; set; }
        public List<double> Coordinates { get; set; }
    }

    public class Detail
    {
        public int CompassDegrees { get; set; }
        public List<int> EndPathIndices { get; set; }
        public string ManeuverType { get; set; }
        public string Mode { get; set; }
        public List<string> Names { get; set; }
        public string RoadType { get; set; }
        public List<int> StartPathIndices { get; set; }
        public List<string> LocationCodes { get; set; }
    }

    public class Instruction
    {
        public object FormattedText { get; set; }
        public string ManeuverType { get; set; }
        public string Text { get; set; }
    }

    public class ManeuverPoint
    {
        public string Type { get; set; }
        public List<double> Coordinates { get; set; }
    }

    public class Hint
    {
        public string HintType { get; set; }
        public string Text { get; set; }
    }

    public class ItineraryItem
    {
        public string CompassDirection { get; set; }
        public List<Detail> Details { get; set; }
        public string Exit { get; set; }
        public string IconType { get; set; }
        public Instruction Instruction { get; set; }
        public ManeuverPoint ManeuverPoint { get; set; }
        public string SideOfStreet { get; set; }
        public string TollZone { get; set; }
        public string TowardsRoadName { get; set; }
        public string TransitTerminus { get; set; }
        public double TravelDistance { get; set; }
        public int TravelDuration { get; set; }
        public string TravelMode { get; set; }
        public List<Hint> Hints { get; set; }
    }

    public class EndWaypoint
    {
        public string Type { get; set; }
        public List<double> Coordinates { get; set; }
        public string Description { get; set; }
        public bool IsVia { get; set; }
        public string LocationIdentifier { get; set; }
        public int RoutePathIndex { get; set; }
    }

    public class StartWaypoint
    {
        public string Type { get; set; }
        public List<double> Coordinates { get; set; }
        public string Description { get; set; }
        public bool IsVia { get; set; }
        public string LocationIdentifier { get; set; }
        public int RoutePathIndex { get; set; }
    }

    public class RouteSubLeg
    {
        public EndWaypoint EndWaypoint { get; set; }
        public StartWaypoint StartWaypoint { get; set; }
        public double TravelDistance { get; set; }
        public int TravelDuration { get; set; }
    }

    public class RouteLeg
    {
        public ActualEnd ActualEnd { get; set; }
        public ActualStart ActualStart { get; set; }
        public List<object> AlternateVias { get; set; }
        public int Cost { get; set; }
        public string Description { get; set; }
        public List<ItineraryItem> ItineraryItems { get; set; }
        public string RouteRegion { get; set; }
        public List<RouteSubLeg> RouteSubLegs { get; set; }
        public double TravelDistance { get; set; }
        public int TravelDuration { get; set; }
    }

    public class Resource
    {
        public string Type { get; set; }
        public List<double> Bbox { get; set; }
        public string Id { get; set; }
        public string DistanceUnit { get; set; }
        public string DurationUnit { get; set; }
        public List<RouteLeg> RouteLegs { get; set; }
        public string TrafficCongestion { get; set; }
        public string TrafficDataUsed { get; set; }
        public double TravelDistance { get; set; }
        public int TravelDuration { get; set; }
        public int TravelDurationTraffic { get; set; }
    }

    public class ResourceSet
    {
        public int EstimatedTotal { get; set; }
        public List<Resource> Resources { get; set; }
    }

    public class RootObject
    {
        public string AuthenticationResultCode { get; set; }
        public string BrandLogoUri { get; set; }
        public string Copyright { get; set; }
        public List<ResourceSet> ResourceSets { get; set; }
        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string TraceId { get; set; }
    }
}