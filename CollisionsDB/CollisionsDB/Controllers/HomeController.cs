using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CollisionsDB.Models;
using CollisionsDB.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CollisionsDB.Controllers
{
    public class HomeController : Controller
    {
        private ICollisionRepository repo { get; set; }

        public HomeController (ICollisionRepository temp)
        {
            repo = temp;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Summary(int pageNum = 1)
        {
            int pageSize = 100;

            var x = new CollisionsViewModel
            {
                Collisions = repo.Collisions
                    //.Where(c => c.Category == category || category == null)
                    .Include(c => c.City)
                    .Include(c => c.County)
                    .OrderBy(c => c.CrashId)
                    .Skip((pageNum - 1) * pageSize)
                    .Take(pageSize),

                PageInfo = new PageInfo
                {
                    TotalNumCrashes =
                        (repo.Collisions.Count()),
                    CrashesPerPage = pageSize,
                    CurrentPage = pageNum
                }

            };

            return View(x);
        }

        [HttpGet]
        public IActionResult Details(int collisionid)
        {
            var crash = repo.Collisions
                .Include(c => c.City)
                .Include(c => c.County)
                .FirstOrDefault(x => x.CrashId == collisionid);
            return View("Details", crash);
        }
    }
}
