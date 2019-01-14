using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace TableForDevelopers.Models
{
    public class ProjectContext: DbContext
    {
        public ProjectContext() : base("ApplicationDb") { }
        public DbSet<ProjectModel> Projects { get; set; }
    }
}