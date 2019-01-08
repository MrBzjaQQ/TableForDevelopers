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
            var card = new CardModel();
            card.CardID = 0;
            card.Description = "Описание";
            List<CardModel> row = new List<CardModel> { card, card, card, card, card };
            List<CardModel> row2 = new List<CardModel> { card, null, card, null, card };
            List<CardModel> row3 = new List<CardModel> { null, card, null, card, null };
            List<List<CardModel>> rows = new List<List<CardModel>> { row, row2, row3, row2, row};
            
            return PartialView(rows);
        }
        public ActionResult Card(int id)
        {
            return PartialView();
        }
        public ActionResult CreateCard()
        {
            return PartialView();
        }
    }
}