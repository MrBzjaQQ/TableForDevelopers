using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TableForDevelopers.Models;
using System.Data.Entity;

namespace TableForDevelopers.Controllers
{
    public class ProjectController : Controller
    {
        // GET: Project
        public ActionResult CreateProject()
        {
            List<ApplicationUser> users;
            using (ApplicationContext context = ApplicationContext.Create())
            {
                users = context.Users.Where(i => i.Rights == UserType.Customer).ToList();
            }
            List<string> userNames = new List<string>();
            foreach (ApplicationUser user in users)
                userNames.Add(user.UserName);
            ViewBag.Customers = userNames;
            return PartialView();
        }
        public ActionResult RemoveProject()
        {
            List<string> projectNames = new List<string>();
            using (ProjectContext context = new ProjectContext())
            {
                var projects = context.Projects.ToList();
                foreach(ProjectModel p in projects)
                {
                    projectNames.Add(p.ProjectName);
                }
                ViewBag.Projects = projectNames;
            }
                return PartialView();
        }
        [HttpPost]
        public ActionResult CreateProject(ProjectModel model)
        {
            if (ModelState.IsValid)
            {
                ProjectModel project = new ProjectModel { CustomerName = model.CustomerName, ProjectName = model.ProjectName, Style = model.Style };
                using (ProjectContext projectContext = new ProjectContext())
                {
                    projectContext.Projects.Add(project);
                    projectContext.SaveChanges();
                }
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public ActionResult RemoveProject(ProjectModel model)
        {
            using (ProjectContext context = new ProjectContext())
            {
                context.Projects.Remove(context.Projects.FirstOrDefault(i => i.ProjectName == model.ProjectName));
                context.SaveChanges();
            }
            using (CardContext cards = new CardContext())
            {
                foreach (CardModel c in cards.Cards.Where(i => i.Project == model.ProjectName))
                    cards.Cards.Remove(c);
                cards.SaveChanges();
            }
            return RedirectToAction("Index", "Home");
        }
    }
}