using _5032Project_v2.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace _5032Project_v2.Controllers
{
    [Authorize(Roles = "Manager")]
    public class RoleManagerController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleManagerController() : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())), new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext())))
        {
        }

        public RoleManagerController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public async Task<ActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            var model = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user.Id);
                model.Add(new UserRolesViewModel
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    RoleNames = roles.ToList(),
                    AllRoles = allRoles
                });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ChangeUserRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);

            // Remove user from all roles
            var currentRoles = await _userManager.GetRolesAsync(user.Id);
            await _userManager.RemoveFromRolesAsync(user.Id, currentRoles.ToArray());

            // Add user to the new role
            await _userManager.AddToRoleAsync(user.Id, roleName);

            return RedirectToAction("Index");
        }

    }
}
