using _5032Project_v2.Models;
using Microsoft.AspNet.Identity;
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

        public ActionResult Admin()
        {
            string currentUserId = User.Identity.GetUserId();
            var user = _context.Users.FirstOrDefault(u => u.Id == currentUserId);

            if (user != null)
            {
                // Assuming your ApplicationUser has FirstName and LastName properties
                ViewBag.FirstName = user.FirstName;
                ViewBag.LastName = user.LastName;
            }

            return View();
        }
    }
}
