using System;
using System.Linq;

namespace CollisionsDB.Models
{
    public class EFCollisionRepository : ICollisionRepository
    {
        private CollisionDBContext _context { get; set; }

        public EFCollisionRepository(CollisionDBContext temp)
        {
            _context = temp;
        }

        public IQueryable<Collision> Collisions => _context.collisions;
        public IQueryable<City> Cities => _context.cities;
        public IQueryable<County> Counties => _context.counties;

        public void AddCollision(Collision collision)
        {
            _context.collisions.Add(collision);
            _context.SaveChanges();
        }
        public void DeleteCollision(Collision collision)
        {
            _context.collisions.Remove(collision);
            _context.SaveChanges();
        }
        public void EditCollision(Collision collision)
        {
            _context.collisions.Update(collision);
            _context.SaveChanges();
        }
    }
}
