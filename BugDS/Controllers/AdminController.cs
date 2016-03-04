using BugDS.Helper;
using BugDS.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugDS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Select Roles
        [HttpGet]
        public ActionResult EditUserRole(string id)
        {

                var user = db.Users.Find(id);
                AdminUserViewModel AdminModel = new AdminUserViewModel();
                UserRolesHelper helper = new UserRolesHelper();
                var selected = helper.ListUserRoles(id);
                AdminModel.Roles = new MultiSelectList(db.Roles, "Name", "Name", selected);
                AdminModel.User = user;

                return View(AdminModel);

        }

        // POST: Select Roles
        [HttpPost]
        public ActionResult EditUserRole(AdminUserViewModel model)
        {
            UserRolesHelper helper = new UserRolesHelper();

            if (model.SelectedRoles == null)
            {
                string[] SelectedRoles = new string[] { "" };
            }

            foreach (var role in db.Roles.Select(r => r.Name))
            {
                if (model.SelectedRoles.Contains(role))
                {
                    helper.AddUserToRole(model.User.Id, role);
                }
                else
                {
                    helper.RemoveUserFromRole(model.User.Id, role);
                }
            }
            return RedirectToAction("Index", "Admin");
        }

    }
}
