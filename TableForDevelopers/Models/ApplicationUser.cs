using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TableForDevelopers.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Login { get; set; }
        public UserType Rights { get; set; }
        public ApplicationUser()
        {
            
        }
    }

    public enum UserType
    {
        Customer,
        Developer,
        TeamLeader
    }
}