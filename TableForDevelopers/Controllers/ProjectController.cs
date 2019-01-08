using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TableForDevelopers.Controllers
{
    public class ProjectController : Controller
    {
        // GET: Project
        public ActionResult CreateProject()
        {
            return PartialView();
        }
        public ActionResult RemoveProject()
        {
            return PartialView();
        }
    }
}