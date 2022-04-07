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
        public IActionResult Summary(string county, int pageNum = 1)
        {
            int pageSize = 100;

            var x = new CollisionsViewModel
            {
                Collisions = repo.Collisions
                    .Where(c => c.County.CountyName == county || county == null)
                    .Include(c => c.City)
                    .Include(c => c.County)
                    .OrderByDescending(c => c.CrashId)
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


            // run simulations to determine what would have reduced the severity of the collision
            List<SeverityReducer> severityReducers = CalculateSeverityReducers(crashMLInput, crash.CrashSeverityId);
            ViewBag.SeverityReducers = severityReducers;
            return View("Details", crash);
        }

        // this function simulates a crash with slightly different values
        // to determine what could have reduced the severity of the crash
        private List<SeverityReducer> CalculateSeverityReducers(CrashDataInput input, int baselineActual)
        {
            List<SeverityReducer> reducers = new List<SeverityReducer>();
            int baselinePrediction = (int)Math.Round(GetSeverityPrediction(input).PredictedValue);

            // for each boolean variable, simulate an alternate reality where that thing didn't happen
            if (input.BicyclistInvolved)
            {
                CrashDataInput noBike = input.Clone();
                noBike.BicyclistInvolved = false;
                int noBikeSeverity = (int)Math.Round(GetSeverityPrediction(noBike).PredictedValue);
                CrashSeverity severity = repo.CrashSeverities.FirstOrDefault(x => x.CrashSeverityId == noBikeSeverity);
                if (noBikeSeverity < baselineActual && noBikeSeverity < baselinePrediction) reducers.Add(new SeverityReducer { Attribute = "no Bicycle Was Involved in Crash", Severity = severity });
            }

            // for each boolean variable, simulate an alternate reality where that thing didn't happen
            if (input.NightDarkCondition)
            {
                CrashDataInput day = input.Clone();
                day.NightDarkCondition = false;
                int daySeverity = (int)Math.Round(GetSeverityPrediction(day).PredictedValue);
                CrashSeverity severity = repo.CrashSeverities.FirstOrDefault(x => x.CrashSeverityId == daySeverity);
                if (daySeverity < baselineActual && daySeverity < baselinePrediction) reducers.Add(new SeverityReducer { Attribute = "the Crash Occurred Under Better Lighting Conditions", Severity = severity });
            }

            // for each boolean variable, simulate an alternate reality where that thing didn't happen
            if (input.OverturnRollover)
            {
                CrashDataInput noRollover = input.Clone();
                noRollover.OverturnRollover = false;
                int noRolloverSeverity = (int)Math.Round(GetSeverityPrediction(noRollover).PredictedValue);
                CrashSeverity severity = repo.CrashSeverities.FirstOrDefault(x => x.CrashSeverityId == noRolloverSeverity);
                if (noRolloverSeverity < baselineActual && noRolloverSeverity < baselinePrediction) reducers.Add(new SeverityReducer { Attribute = "a Vehicle Did Not Overturn", Severity = severity });
            }

            // for each boolean variable, simulate an alternate reality where that thing didn't happen
            if (input.MotorcycleInvolved)
            {
                CrashDataInput noMotorCycle = input.Clone();
                noMotorCycle.MotorcycleInvolved = false;
                int noMotorCycleSeverity = (int)Math.Round(GetSeverityPrediction(noMotorCycle).PredictedValue);
                CrashSeverity severity = repo.CrashSeverities.FirstOrDefault(x => x.CrashSeverityId == noMotorCycleSeverity);
                if (noMotorCycleSeverity < baselineActual && noMotorCycleSeverity < baselinePrediction) reducers.Add(new SeverityReducer { Attribute = "No Motorcycle Was Involved in Crash", Severity = severity });
            }

            // for each boolean variable, simulate an alternate reality where that thing didn't happen
            if (input.PedestrianInvolved)
            {
                CrashDataInput noPed = input.Clone();
                noPed.BicyclistInvolved = false;
                int noPedSeverity = (int)Math.Round(GetSeverityPrediction(noPed).PredictedValue);
                CrashSeverity severity = repo.CrashSeverities.FirstOrDefault(x => x.CrashSeverityId == noPedSeverity);
                if (noPedSeverity < baselineActual && noPedSeverity < baselinePrediction) reducers.Add(new SeverityReducer { Attribute = "No Pedestrian Was Involved in Crash", Severity = severity });
            }

            // for each boolean variable, simulate an alternate reality where that thing didn't happen
            if (input.TeenageDriverInvolved)
            {
                CrashDataInput noTeen = input.Clone();
                noTeen.BicyclistInvolved = false;
                int noTeenSeverity = (int)Math.Round(GetSeverityPrediction(noTeen).PredictedValue);
                CrashSeverity severity = repo.CrashSeverities.FirstOrDefault(x => x.CrashSeverityId == noTeenSeverity);
                if (noTeenSeverity < baselineActual && noTeenSeverity < baselinePrediction) reducers.Add(new SeverityReducer { Attribute = "No Teenage Driver Was Involved", Severity = severity });
            }

            return reducers;
        }
    }
}
