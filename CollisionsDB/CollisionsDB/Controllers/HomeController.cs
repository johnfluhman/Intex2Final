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
        public IActionResult Calculator()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Calculator(CrashDataInput data)
        {

            ViewBag.Result = GetSeverityPrediction(data);
            return View();
        }

        private SeverityPrediction GetSeverityPrediction(CrashDataInput data)
        {
            var result = session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", data.AsTensor())
            });
            Tensor<float> score = result.First().AsTensor<float>();
            var prediction = new SeverityPrediction { PredictedValue = score.First() };
            result.Dispose();
            return prediction;
        }

        public IActionResult Details(int collisionid)
        {
            var crash = repo.Collisions
                .Include(c => c.City)
                .Include(c => c.County)
                .Include( c => c.CrashSeverity)
                .FirstOrDefault(x => x.CrashId == collisionid);


            // predict what the crash severity SHOULD have been
            CrashDataInput crashMLInput = CrashDataInput.CollisionToMLInput(crash);
            float predictedSeverityValue = GetSeverityPrediction(crashMLInput).PredictedValue;
            int roundedPrediction = (int)Math.Round(predictedSeverityValue);
            CrashSeverity predictedCrashSeverity = repo.CrashSeverities.Where(cs => cs.CrashSeverityId == roundedPrediction).FirstOrDefault();
            ViewBag.ActualSeverity = crash.CrashSeverityId.ToString() + " - " + crash.CrashSeverity.Description;
            ViewBag.PredictedSeverity = predictedSeverityValue;
            ViewBag.RoundedPrediction = roundedPrediction.ToString() + " - " + predictedCrashSeverity.Description;

            string comparison;
            string comparisonTextClass;
            // generate a line of text that comments on the prediction vs actual
            if(roundedPrediction > crash.CrashSeverityId)
            {
                comparison = "less severe";
                comparisonTextClass = "success";
            }
            else if(roundedPrediction < crash.CrashSeverityId)
            {
                comparison = "more severe";
                comparisonTextClass = "danger";
            }
            else
            {
                comparison = "equally severe";
                comparisonTextClass = "info";
            }
            // add the word "much" if the difference is big
            if(Math.Abs(roundedPrediction - crash.CrashSeverityId) >= 2)
            {
                comparison = "much " + comparison;
            }
            ViewBag.ComparisonTextClass = comparisonTextClass;
            ViewBag.SeverityComparison = comparison;


            // if this crash happened during the daytime, the severity would have decreased by 17%

            return View("Details", crash);
        }
    }
}
