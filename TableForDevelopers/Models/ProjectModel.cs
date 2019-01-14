using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TableForDevelopers.Models
{
    public class ProjectModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ProjectID { get; set; }
        [Required]
        public string CustomerName { get; set; } //TODO: здесь выбрать пользователя с указанным именем из базы
        [Required]
        public string ProjectName { get; set; }
        [Required]
        public string Style
        {
            get
            {
                return _style;
            }
            set
            {
                _style = value;
                switch(value)
                {
                    case "Primary": { CssStyle = CSSClassModel.Primary; break; }
                    case "Secondary": { CssStyle = CSSClassModel.Secondary; break; }
                    case "Success": { CssStyle = CSSClassModel.Success; break; }
                    case "Danger": { CssStyle = CSSClassModel.Danger; break; }
                    case "Warning": { CssStyle = CSSClassModel.Warning; break; }
                    case "Info": { CssStyle = CSSClassModel.Info; break; }
                    case "Light": { CssStyle = CSSClassModel.Light; break; }
                    case "Dark": { CssStyle = CSSClassModel.Dark; break; }
                    default: { CssStyle = CSSClassModel.Primary; break; }
                }
            }
        }
        public Tuple<string, string> CssStyle { get; set; }
        private ApplicationUser _customer;
        private string _style;
    }
}