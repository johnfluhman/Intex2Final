﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CollisionsDB.Models;
using CollisionsDB.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace CollisionsDB.Controllers
{
    public class HomeController : Controller
    {
        private ICollisionRepository repo { get; set; }
        private InferenceSession session { get; set; }

        public HomeController (ICollisionRepository temp, InferenceSession tempSession)
        {
            repo = temp;
            session = tempSession;
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
        public IActionResult Summary(string county, int severity, int pageNum = 1)
        {
            int pageSize = 100;

            var x = new CollisionsViewModel
            {
                Collisions = repo.Collisions
                    .Where(c => c.County.CountyName == county || county == null)                    
                    .Include(c => c.City)
                    .Include(c => c.County)
                    .OrderBy(c => c.CrashId)
                    .Skip((pageNum - 1) * pageSize)
                    .Take(pageSize),

                PageInfo = new PageInfo
                {
                    TotalNumCrashes =
                        (county == null
                            ? repo.Collisions.Count()
                            : repo.Collisions.Where(x => x.County.CountyName == county).Count()),
                    CrashesPerPage = pageSize,
                    CurrentPage = pageNum
                }

            };

            return View(x);
        }

        [HttpGet]
        public IActionResult Calculator()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Calculator(CrashDataInput data)
        {
            var result = session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", data.AsTensor())
            });
            Tensor<float> score = result.First().AsTensor<float>();
            var prediction = new SeverityPrediction { PredictedValue = score.First() };
            result.Dispose();
            ViewBag.Result = prediction;
            return View();
        }

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
