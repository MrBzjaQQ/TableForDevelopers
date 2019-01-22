using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TableForDevelopers.Models
{
    public class TwoFactorAuthModel
    {
        [Required]
        public string Code { get; set; }
    }
}