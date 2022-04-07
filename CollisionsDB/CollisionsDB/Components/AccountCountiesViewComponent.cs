using System;
using Microsoft.AspNetCore.Mvc;
using CollisionsDB.Models;
using System.Linq;

namespace CollisionsDB.Components
{
    public class AccountCountiesViewComponent : ViewComponent
    {
        private ICollisionRepository repo { get; set; }

        public AccountCountiesViewComponent(ICollisionRepository temp)
        {
            repo = temp;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedCounty = RouteData?.Values["county"];

            var counties = repo.Counties
                .Select(x => x.CountyName)
                .Distinct()
                .OrderBy(x => x);

            return View(counties);
        }
    }
}
