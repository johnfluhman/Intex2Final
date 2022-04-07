using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollisionsDB.Models
{
    public class CollisionDBContext : DbContext 
    {
        public CollisionDBContext(DbContextOptions<CollisionDBContext> options) : base(options)
        {
        }

        public DbSet<Collision> collisions { get; set; }
        public DbSet<CrashSeverity> crashSeverities { get; set; }
        public DbSet<City> cities { get; set; }
        public DbSet<County> counties { get; set; }
    }
}
