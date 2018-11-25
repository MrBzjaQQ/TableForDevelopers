using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TableForDevelopers.Models;

namespace TableForDevelopers.Controllers
{
    public class TableController : Controller
    {
        List<CardModel> cards = new List<CardModel>
        {
            new CardModel(0, CSSClassModel.Danger),
            new CardModel(1, CSSClassModel.Dark),
            new CardModel(2, CSSClassModel.Light),
            new CardModel(3, CSSClassModel.Success)
        };
        // GET: Table
        public ActionResult Table()
        {
            return PartialView(cards);
        }
        public ActionResult Card(int id)
        {
            return PartialView(cards.ElementAt(id));
        }
    }
}