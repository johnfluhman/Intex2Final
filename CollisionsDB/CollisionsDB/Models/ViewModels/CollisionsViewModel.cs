using System;
using System.Linq;

namespace CollisionsDB.Models.ViewModels
{
    public class CollisionsViewModel
    {
        public IQueryable<Collision> Collisions { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}