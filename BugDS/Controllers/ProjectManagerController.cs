using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugDS.Controllers
{
    [Authorize(Roles = "Administrator, Project Manager")]
    public class ProjectManagerController : Controller
    {
        // GET: ProjectManager
        public ActionResult Index()
        {
            return View();
        }
    }
}