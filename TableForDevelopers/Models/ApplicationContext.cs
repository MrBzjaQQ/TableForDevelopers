using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TableForDevelopers.Models
{
    public class ApplicationContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext() : base("ApplicationDb") { }
        public static ApplicationContext Create()
        {
            return new ApplicationContext();
        }
    }
}