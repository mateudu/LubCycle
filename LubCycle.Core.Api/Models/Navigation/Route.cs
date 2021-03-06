﻿using LubCycle.Core.Api.Models.NextBike;
using System;
using System.Collections.Generic;

namespace LubCycle.Core.Api.Models.Navigation
{
    public class Route
    {
        public RouteStatus Status { get; set; }
        public string Message { get; set; }
        public Place Start { get; set; }
        public Place Destination { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        //public int Bikes { get; set; }
        public double? Duration { get; set; }

        public double? Distance { get; set; }
        public List<Place> Stations { get; set; }
    }

    public enum RouteStatus : int
    {
        Ok = 200,
        IncorrectArguments = 400,
        Error = 500
    }
}