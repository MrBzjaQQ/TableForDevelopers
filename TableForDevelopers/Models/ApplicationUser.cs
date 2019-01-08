using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TableForDevelopers.Models
{
    public class ApplicationUser : IdentityUser
    {
        public UserRights rights { get; private set; }
        public ApplicationUser()
        {
            
        }
    }

    public enum UserRights
    {
        Customer,
        Developer,
        TeamLeader
    }
}