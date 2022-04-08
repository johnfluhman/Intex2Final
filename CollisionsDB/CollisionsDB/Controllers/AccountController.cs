using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CollisionsDB.Models;
using CollisionsDB.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CollisionsDB.Controllers
{
    [Authorize]
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
        public IActionResult Form()
        {
            ViewBag.Cities = repo.Cities.ToList().Distinct().OrderBy(x => x.CityName);
            ViewBag.Counties = repo.Counties.ToList().Distinct().OrderBy(x => x.CountyName);
            ViewBag.EditMode = false;
            return View();
        }

        [HttpPost]
        public IActionResult Form(Collision c)
        {
            if (ModelState.IsValid)
            {
                repo.AddCollision(c);
                return RedirectToAction("Summary", "Home");
            }
            else
            {
                ViewBag.Cities = repo.Cities.ToList().Distinct().OrderBy(x => x.CityName);
                ViewBag.Counties = repo.Counties.ToList().Distinct().OrderBy(x => x.CountyName);
                ViewBag.EditMode = false;
                return View(c);
            }
        }

        [HttpGet]
        public IActionResult EditCollision(int collisionid)
        {
            ViewBag.Cities = repo.Cities.ToList().Distinct().OrderBy(x => x.CityName);
            ViewBag.Counties = repo.Counties.ToList().Distinct().OrderBy(x => x.CountyName);
            ViewBag.EditMode = true;
            var edit = repo.Collisions.Single(x => x.CrashId == collisionid);
            return View("Form", edit);
        }

        [HttpPost]
        public IActionResult EditCollision(Collision c)
        {
            if (ModelState.IsValid)
            {
                repo.EditCollision(c);
                return RedirectToAction("Summary", "Home");
            }
            else
            {
                ViewBag.Cities = repo.Cities.ToList().Distinct().OrderBy(x => x.CityName);
                ViewBag.Counties = repo.Counties.ToList().Distinct().OrderBy(x => x.CountyName);
                ViewBag.EditMode = true;
                return View(c);
            }

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
            return RedirectToAction("Summary", "Home");
        }

        public IActionResult TestRestricted()
        {
            return View();
        }

        //public async Task<RedirectResult> Logout(string returnUrl = "/")
        //{
        //    await signInManager.SignOutAsync();

        //    return Redirect(returnUrl);
        //}
    }
}
