﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LubCycle.Core.Mobile.Models
{
    public class LocationResponse
    {
        public LocationResponseStatus Status { get; set; }
        public string Message { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }

    public enum LocationResponseStatus : int
    {
        Ok = 200,
        BadRequest = 400,
        Error = 500
    }
}
