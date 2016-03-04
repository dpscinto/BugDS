using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugDS.Controllers
{
    [Authorize(Roles = "Administrator, Project Manager, Developer")]
    public class DeveloperController : Controller
    {
        // GET: Developer
        public ActionResult Index()
        {
            return View();
        }
    }
}