using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CollisionsDB.Models;
using CollisionsDB.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CollisionsDB.Controllers
{
    public class AccountController : Controller
    {
        //private UserManager<IdentityUser> userManager;
        //private SignInManager<IdentityUser> signInManager;
        private ICollisionRepository repo { get; set; }

        //public AccountController(UserManager<IdentityUser> um, SignInManager<IdentityUser> sim)
        //{
        //    userManager = um;
        //    signInManager = sim;
        //}

        public AccountController(ICollisionRepository temp)
        {
            repo = temp;
        }

        [HttpGet]
        public IActionResult Login()
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
        public IActionResult Form()
        {
            ViewBag.Cities = repo.Cities.ToList();
            ViewBag.Counties = repo.Counties.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Form(Collision c)
        {
            repo.AddCollision(c);
            return RedirectToAction("Summary");
        }

        [HttpGet]
        public IActionResult EditCollision(int collisionid)
        {
            var edit = repo.Collisions.Single(x => x.CrashId == collisionid);
            return View("Form", edit);
        }

        [HttpPost]
        public IActionResult EditCollision(Collision c)
        {
            repo.EditCollision(c);
            return RedirectToAction("Summary");
        }

        [HttpGet]
        public IActionResult DeleteCollision(int collisionid)
        {
            var edit = repo.Collisions.Single(x => x.CrashId == collisionid);
            return View(edit);
        }

        [HttpPost]
        public IActionResult DeleteCollision(Collision c)
        {
            repo.DeleteCollision(c);
            return RedirectToAction("Summary");
        }

        //public async Task<RedirectResult> Logout(string returnUrl = "/")
        //{
        //    await signInManager.SignOutAsync();

        //    return Redirect(returnUrl);
        //}
    }
}
