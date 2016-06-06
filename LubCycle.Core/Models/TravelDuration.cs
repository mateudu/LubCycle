﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LubCycle.Core.Models
{
    public class TravelDuration
    {
        [Key]
        public int Id { get; set; }
        public string Station1Uid { get; set; }
        public string Station2Uid { get; set; }
        public double Duration { get; set; }
        public double Distance { get; set; }
    }
}
