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
using System.Drawing;
using System.Net.Http;
using System.IO;
using System.Net;
using System.Net.Http.Headers;

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
        public IActionResult Summary(string county, int pageNum = 1, int? severity = null, string? city = null)
        {
            int pageSize = 100;

            var query = repo.Collisions
                    .Where(c => c.County.CountyName == county || county == null)
                    .Where(c => c.CrashSeverityId == severity || severity == null)
                    .Where(c => c.City.CityName == city || city == null)
                    .Include(c => c.City)
                    .Include(c => c.County)
                    .OrderByDescending(c => c.CrashId)
                    .Skip((pageNum - 1) * pageSize)
                    .Take(pageSize);

            var x = new CollisionsViewModel
            {
                Collisions = query,

                PageInfo = new PageInfo
                {
                    TotalNumCrashes = repo.Collisions
                                        .Where(c => c.County.CountyName == county || county == null)
                                        .Where(c => c.CrashSeverityId == severity || severity == null)
                                        .Where(c => c.City.CityName == city || city == null)
                                        .Count(),
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
            if(Math.Abs(roundedPrediction - crash.CrashSeverityId) >= 3)
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

        public IActionResult Analytics()
        {
            // get a sum of all crashes by type for most recent year in dataset
            string year = "2019";
            Dictionary<int, int> numCrashes = new Dictionary<int, int>();
            for(int i = 1; i <= 5; i++)
            {
                // kinda hacky but it works
                numCrashes[i] = repo.Collisions.Where(c => c.CrashDatetime.Contains("/" + year)).Count(c => c.CrashSeverityId == i);
            }
            ViewBag.NumCrashes = numCrashes;
            ViewBag.CrashSeverities = repo.CrashSeverities.ToList<CrashSeverity>();
            return View();
        }


        // returns an image that shows how the ML model converts location into severity
        // we screenshotted this functionality because it breaks on AWS
        // but on local dev it dynamically generates an image to load into the app
        public IActionResult HeatMap(int width=225)
        {
            // these dimensions are roughly the shape of utah
            //int width = 225;
            //int height = 275;
            
            // compute image height automatically (Utah is ~22% taller than it is wide)
            int height = (int)(width * 1.222222f);

            // the UTM boundaries of Utah (roughly)
            float LatMin = 4095217.62f;
            float LatMax = 4651314.56f;
            float LongMin = 234453.51f;
            float LongMax = 672162f;

            // the corner of utah begins at around the 58% mark width and 80% height
            float cornerX = .58f;
            float cornerY = .8f;

            float UtahWidth = LongMax - LongMin;
            float UtahHeight = LatMax - LatMin;

            CrashDataInput input = new CrashDataInput
            {
                LatUtmY = 0,
                LongUtmX = 0,
                // average milepoint in the data
                Milepoint = 70.34f,
                BicyclistInvolved = false,
                OverturnRollover = false,
                NightDarkCondition = false,
                TeenageDriverInvolved = false,
                MotorcycleInvolved = false,
                PedestrianInvolved = false,
            };

            using (Bitmap image = new Bitmap(width, height))
            {
                for(int x = 0; x < width; x++)
                {
                    for(int y = 0; y < height; y++)
                    {
                        // convert x and y into 0-1 range
                        float relX = (float)x / width;
                        float relY = (float)y / height;

                        // convert the 0-1 range into UTM units
                        float lon = UtahWidth * relX + LongMin;
                        float lat = UtahHeight * relY + LatMin;

                        input.LongUtmX = lon;
                        input.LatUtmY = lat;
                        float brightness = GetSeverityPrediction(input).PredictedValue;
                        // convert brightness from 1-5 to 0-255
                        brightness = (brightness - 1) * 64;
                        // scale brightness
                        brightness *= brightness / 4;
                        // clamp
                        if (brightness > 255) brightness = 255;

                        image.SetPixel(x, height-y-1, Color.FromArgb(255, (int)brightness, 0, 0));

                        // auto set to black if outside utah bounds
                        if (relX > cornerX && relY > cornerY)
                        {
                            image.SetPixel(x, height - y - 1, Color.Transparent);
                        }
                    }
                }
                MemoryStream ms = new MemoryStream();

                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                return File(ms.ToArray(), "image/png");
            }
        }
    }
}
