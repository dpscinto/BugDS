using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugDS.Models;
using BugDS.Models.CodeFirst;
using BugDS.Helper;
using Microsoft.AspNet.Identity;

namespace BugDS.Controllers
{
    [RequireHttps]
    [Authorize]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        [Authorize(Roles = "Admin, Project Manager, Developer")]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            if (!User.IsInRole("Admin"))
            {
                var user = db.Users.Find(userId);
                return View(user.Projects.ToList());
            }

            return View(db.Projects.ToList());

        }


        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            if (User.IsInRole("Admin"))
            {
                return View();
            }
            return RedirectToAction("Index", "Projects");
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index", "Projects");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
            {
                Project project = db.Projects.Find(id);

                if (project == null)
                {
                    return HttpNotFound();
                }
                return View(project);
            }

            return RedirectToAction("Index", "Projects");
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //
        //GET: SELECT USER FOR PROJECT
        [HttpGet]
        public ActionResult SelectUsers(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var project = db.Projects.Find(id);
            ProjectsViewModel ProjectModel = new ProjectsViewModel();
            ProjectHelper helper = new ProjectHelper();
            var selected = helper.UsersInProject((int)id).Select(z=>z.Id);
            ProjectModel.Users = new MultiSelectList(db.Users, "Id", "FullName", selected);
            ProjectModel.Project = project;

            return View(ProjectModel);

        }

        // POST: SELECT USER FOR PROJECT
        [HttpPost]
        public ActionResult SelectUsers(ProjectsViewModel model)
        {
            ProjectHelper helper = new ProjectHelper();

            if (model.SelectedUsers == null)
            {
                string[] SelectedUsers = new string[] { "" };
            }

            foreach (var user in db.Users.Select(r => r.Id))
            {
                if (model.SelectedUsers.Contains(user))
                {
                    helper.AddUserToProject(user, model.Project.Id);
                }
                else
                {
                    helper.RemoveUserFromProject(user, model.Project.Id);
                }
            }

            return RedirectToAction("Index", "Projects");
        }
    }
}
