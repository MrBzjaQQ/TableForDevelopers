using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TableForDevelopers.Models
{
    public static class CSSClassModel
    {
       public static Tuple<string, string> Primary => new Tuple<string, string>("card border-primary m-3", "card-body text-primary");
       public static Tuple<string, string> Secondary => new Tuple<string, string>("card border-secondary m-3", "card-body text-secondary");
       public static Tuple<string, string> Success => new Tuple<string, string>("card border-success m-3", "card-body text-success");
       public static Tuple<string, string> Danger => new Tuple<string, string>("card border-danger m-3", "card-body text-danger");
       public static Tuple<string, string> Warning => new Tuple<string, string>("card border-warning m-3", "card-body text-warning");
       public static Tuple<string, string> Info => new Tuple<string, string>("card border-info m-3", "card-body text-info");
       public static Tuple<string, string> Light => new Tuple<string, string>("card border-light m-3", "card-body");
       public static Tuple<string, string> Dark => new Tuple<string, string>("card border-dark m-3", "card-body text-dark");
    }
}