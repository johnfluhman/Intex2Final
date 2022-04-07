using System;
using System.Linq;

namespace CollisionsDB.Models
{
    public interface ICollisionRepository
    {
        IQueryable<Collision> Collisions { get; }
        IQueryable<City> Cities { get; }
        IQueryable<County> Counties { get; }

        IQueryable<CrashSeverity> CrashSeverities { get; }
        public void AddCollision(Collision collision);
        public void DeleteCollision(Collision collision);
        public void EditCollision(Collision collision);
    }
}
