using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TableForDevelopers.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;

namespace TableForDevelopers.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }
        public async Task<ActionResult> Index()
        {
            CheckAuth();
            List<string> projectNames = await LoadProjects();
            return View(projectNames);
        }
        public ActionResult About()
        {
            CheckAuth();

            return View();
        }

        public ActionResult Contact()
        {
            CheckAuth();
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private async Task<List<string>> LoadProjects()
        {
            List<string> projectNames = new List<string>();
            using (ProjectContext context = new ProjectContext())
            {
                List<ProjectModel> projects = await context.Projects.ToListAsync<ProjectModel>();
                if (ViewBag.UserType != "Customer")
                    foreach (var p in projects)
                        projectNames.Add(p.ProjectName);
                else
                    foreach (var p in projects.Where(i => i.CustomerName == ViewBag.Login))
                        projectNames.Add(p.ProjectName);

            }
            return projectNames;
        }
        private void CheckAuth()
        {
            ViewBag.IsAuth = HttpContext.User.Identity.IsAuthenticated; // аутентифицирован ли пользователь
            ViewBag.Login = HttpContext.User.Identity.Name; // логин авторизованного пользователя
            ViewBag.UserType = HttpContext.Cache["UserType"];
            ViewBag.UserId = HttpContext.Cache["UserId"];
        }
    }
}