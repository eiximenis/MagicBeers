using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticsService.Data
{
    public class StatisticsDbContext : DbContext
    {
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<CardPlayerUsage> CardUsages { get; set; }

        public StatisticsDbContext(DbContextOptions<StatisticsDbContext> options) : base(options)
        { 
        }
    }
}
