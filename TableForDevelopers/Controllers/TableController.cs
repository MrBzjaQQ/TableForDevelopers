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
        // GET: Table
        public ActionResult Table()
        {
            List<CardModel> cards = new List<CardModel>();
            cards.Add(new CardModel());
            return PartialView(cards);
        }
        public ActionResult Card()
        {
            CardModel card = new CardModel();
            return PartialView(card);
        }
    }
}