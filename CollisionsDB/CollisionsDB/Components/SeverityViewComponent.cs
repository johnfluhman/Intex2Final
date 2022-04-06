using System;
using Microsoft.AspNetCore.Mvc;
using CollisionsDB.Models;
using System.Linq;

namespace CollisionsDB.Components
{
    public class SeverityViewComponent : ViewComponent
    {
        private ICollisionRepository repo { get; set; }

        public SeverityViewComponent(ICollisionRepository temp)
        {
            repo = temp;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedSeverity = RouteData?.Values["severity"];

            var severities = repo.Collisions
                .Select(x => x.CrashSeverityId)
                .Distinct()
                .OrderBy(x => x);

            return View(severities);
        }
    }
}
