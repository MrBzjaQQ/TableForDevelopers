using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TableForDevelopers.Models;

namespace TableForDevelopers.Controllers
{
    [RequireHttps]
    public class TableController : Controller
    {
        // GET: Table
        public ActionResult Table(string project = "")
        {
            CheckAuth();
            List<List<CardModel>> table = new List<List<CardModel>>();
            if (ModelState.IsValid)
            {
                table = LoadTable(project);
            }
            return PartialView(table);
        }
        public ActionResult Card(int id)
        {
            CheckAuth();
            CardModel card = new CardModel();
            using (CardContext cards = new CardContext())
            {
                card = cards.Cards.FirstOrDefault(i => i.CardID == id);
            }
            FindProjects();
            FindDevelopers();
            return PartialView(card);
        }
        public ActionResult CreateCard()
        {
            CheckAuth();
            FindProjects();
            FindDevelopers();
            return PartialView();
        }
        [HttpPost]
        public ActionResult CreateCard(CardModel model)
        {
            CheckAuth();
            CardModel card = new CardModel();
            card.Project = string.Empty;
            if (ModelState.IsValid)
            {
                card = new CardModel
                {
                    Header = model.Header,
                    Description = model.Description,
                    AppointedDeveloper = model.AppointedDeveloper,
                    Project = model.Project,
                    Status = model.Status
                };
                using (CardContext cardContext = new CardContext())
                {
                    cardContext.Cards.Add(card);
                    cardContext.SaveChanges();
                }
            }
            return RedirectToAction("Table", "Table", new { project = card.Project });
        }
        [HttpPost]
        public ActionResult Card(CardModel model, int id, string buttonType)
        {
            CheckAuth();
            if (buttonType == "delete")
            {
                using (CardContext cards = new CardContext())
                {
                    cards.Cards.Remove(cards.Cards.FirstOrDefault(i => i.CardID == id));
                    cards.SaveChanges();
                }
                
            }
            if(buttonType == "edit")
            {
                using (CardContext cards = new CardContext())
                {
                    CardModel card = cards.Cards.FirstOrDefault(i => i.CardID == id);
                    card.Header = model.Header;
                    card.Description = model.Description;
                    card.AppointedDeveloper = model.AppointedDeveloper;
                    card.Project = model.Project;
                    card.Status = model.Status;
                    cards.SaveChanges();
                }
            }
            return RedirectToAction("Table", "Table", new { project = model.Project });
        }
        private void FindDevelopers()
        {
            using (ApplicationContext users = ApplicationContext.Create())
            {
                var u = users.Users.Where(i => i.Rights == UserType.Developer).ToList();
                List<string> developers = new List<string>();
                foreach (var dev in u)
                    developers.Add(dev.UserName);
                ViewBag.Developers = developers;
            }
        }
        private void FindProjects()
        {
            using (ProjectContext projects = new ProjectContext())
            {
                var projs = projects.Projects.ToList();
                List<string> prjs = new List<string>();
                foreach (var p in projs)
                    prjs.Add(p.ProjectName);
                ViewBag.Projects = prjs;
            }
        }

        private List<List<CardModel>> LoadTable(string project = "")
        {
            CheckAuth();
            List<List<CardModel>> table = new List<List<CardModel>>();
            using (CardContext context = new CardContext())
            {
                List<CardModel> cards;
                if (project == string.Empty)
                    cards = context.Cards.ToList();
                else
                    cards = context.Cards.Where(i => i.Project == project).ToList();
                Stack<CardModel> backlog = new Stack<CardModel>(cards.Where(i => i.Status == CardStatus.Backlog.ToString()));
                Stack<CardModel> analysis = new Stack<CardModel>(cards.Where(i => i.Status == CardStatus.Analysis.ToString()));
                Stack<CardModel> developing = new Stack<CardModel>(cards.Where(i => i.Status == CardStatus.Developing.ToString()));
                Stack<CardModel> testing = new Stack<CardModel>(cards.Where(i => i.Status == CardStatus.Testing.ToString()));
                Stack<CardModel> done = new Stack<CardModel>(cards.Where(i => i.Status == CardStatus.Done.ToString()));
                int counter = cards.Count;
                do
                {
                    int cardsTaken = 0;
                    List<CardModel> row = new List<CardModel>();
                    if (backlog.Any())
                    {
                        row.Add(backlog.Pop());
                        cardsTaken++;
                    }
                    else row.Add(null);
                    if (analysis.Any())
                    {
                        row.Add(analysis.Pop());
                        cardsTaken++;
                    }
                    else row.Add(null);
                    if (developing.Any())
                    {
                        row.Add(developing.Pop());
                        cardsTaken++;
                    }
                    else row.Add(null);
                    if (testing.Any())
                    {
                        row.Add(testing.Pop());
                        cardsTaken++;
                    }
                    else row.Add(null);
                    if (done.Any())
                    {
                        row.Add(done.Pop());
                        cardsTaken++;
                    }
                    else row.Add(null);
                    table.Add(row);
                    counter -= cardsTaken;

                } while (counter > 0);
                
            }
            return table;
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