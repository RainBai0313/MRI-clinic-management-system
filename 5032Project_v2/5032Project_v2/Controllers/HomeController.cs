using _5032Project_v2.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace _5032Project_v2.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;

        public HomeController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize (Roles ="Admin")]
        public ActionResult Admin(string timeFrame = "all")
        {
            string currentUserId = User.Identity.GetUserId();
            var user = _context.Users.FirstOrDefault(u => u.Id == currentUserId);

            // Define the date range based on the timeFrame
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MaxValue;

            switch (timeFrame)
            {
                case "week":
                    startDate = DateTime.Now.AddDays(-((int)DateTime.Now.DayOfWeek));
                    endDate = startDate.AddDays(7).AddSeconds(-1); // Till end of the week
                    break;

                case "month":
                    startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    endDate = startDate.AddMonths(1).AddSeconds(-1); // Till end of the month
                    break;

                default: // all appointments, no date restrictions
                    break;
            }

            // Use the start and end dates to filter appointments based on the time frame
            int statusZeroCount = _context.Appointments.Where(a => a.DateTime >= startDate && a.DateTime <= endDate).Count(a => a.Status == 0);
            int statusOneTwoCount = _context.Appointments.Where(a => a.DateTime >= startDate && a.DateTime <= endDate).Count(a => a.Status == 1 || a.Status == 2);
            int statusThreeCount = _context.Appointments.Where(a => a.DateTime >= startDate && a.DateTime <= endDate).Count(a => a.Status == 3);

            int oneStarCount = _context.Appointments.Where(r => r.DateTime >= startDate && r.DateTime <= endDate).Count(r => r.FeedbackRating == 1);
            int twoStarsCount = _context.Appointments.Where(r => r.DateTime >= startDate && r.DateTime <= endDate).Count(r => r.FeedbackRating == 2);
            int threeStarsCount = _context.Appointments.Where(r => r.DateTime >= startDate && r.DateTime <= endDate).Count(r => r.FeedbackRating == 3);
            int fourStarsCount = _context.Appointments.Where(r => r.DateTime >= startDate && r.DateTime <= endDate).Count(r => r.FeedbackRating == 4);
            int fiveStarsCount = _context.Appointments.Where(r => r.DateTime >= startDate && r.DateTime <= endDate).Count(r => r.FeedbackRating == 5);
            if (Request.IsAjaxRequest())
            {
                return Json(new
                {
                    StatusZeroCount = statusZeroCount,
                    StatusOneTwoCount = statusOneTwoCount,
                    StatusThreeCount = statusThreeCount,
                    OneStarCount = oneStarCount,
                    TwoStarsCount = twoStarsCount,
                    ThreeStarsCount = threeStarsCount,
                    FourStarsCount = fourStarsCount,
                    FiveStarsCount = fiveStarsCount
                }, JsonRequestBehavior.AllowGet);
            }

            return View();
        }

    }
}
