using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LubCycle.DbSeed
{
    public class AppDatabase : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Program.AppDatabaseConnectionString);
        }
        
        public DbSet<LubCycle.Core.Api.Models.TravelDuration> TravelDurations { get; set; }
    }
}
