using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace TableForDevelopers.Models
{
    public class CardContext: DbContext
    {
        public CardContext() : base("ApplicationDb") { }
        public DbSet<CardModel> Cards { get; set; }
    }
}