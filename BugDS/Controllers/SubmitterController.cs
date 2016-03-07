using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugDS.Controllers
{
    [RequireHttps]
    [Authorize(Roles = "Administrator, Project Manager, Developer, Submitter")]
    public class SubmitterController : Controller
    {
        // GET: Submitter
        public ActionResult Index()
        {
            return View();
        }
    }
}