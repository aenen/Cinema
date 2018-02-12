using Cinema.Data.Database;
using Cinema.Data.Identity;
using Cinema.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Cinema.Controllers
{
    public class ProfileController : Controller
    {
        DbContext context;
        ApplicationUserManager userMgr;

        public ProfileController(DbContext context)
        {
            this.context = context;
            userMgr = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
        }

        // GET: Profile
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var user = await userMgr.FindByNameAsync(User.Identity.Name);
            return View(user);
        }

        // GET: Profile/Settings
        [Authorize]
        public async Task<ActionResult> Settings()
        {
            var user = await userMgr.FindByNameAsync(User.Identity.Name);
            var model = new ProfileSettingsViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Birthday = user.Birthday
            };

            return View(model);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Settings(ProfileSettingsViewModel model, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userMgr.FindByNameAsync(User.Identity.Name);
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Birthday = model.Birthday;
            var result = await userMgr.UpdateAsync(user);
            if (result.Succeeded)
                return RedirectToAction("Index","Profile");
            return View(model);
        }
    }
}