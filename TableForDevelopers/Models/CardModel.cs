using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TableForDevelopers.Models
{
    public class CardModel
    {
        public CardModel()
        {

        }
        //Будет отображено на карточке на доске
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int CardID { get; set; }
        [Required]
        public string Header { get; set; } //Заголовок
        [Required]
        public string AppointedDeveloper { get; set; } //Назначенный разработчик: Можно создать тип Developer
        //public Tuple<string, Bitmap> CardType { get; set; } - TODO - если будет время
        //Будет дополнительно отображено на карточке в модальном окне
        [Required]
        public string Status
        {
            get
            {
                return _status.ToString();
            }
            set
            {
                switch (value)
                {
                    case "Analysis": { _status = CardStatus.Analysis; break; }
                    case "Developing": { _status = CardStatus.Developing; break; }
                    case "Testing": { _status = CardStatus.Testing; break; }
                    case "Done": { _status = CardStatus.Done; break; }
                    case "Backlog": { _status = CardStatus.Backlog; break; }
                }

            }
        }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Project
        {
            get
            {
                return _projectName;
            }
            set
            {
                _projectName = value;
                using (ProjectContext projects = new ProjectContext())
                {
                    _project = projects.Projects.FirstOrDefault(i => i.ProjectName == value);
                }
            }
        } //TODO найти проект по названию
        public string CardClass => cssClass.Item1; //классы стилей вынесены в модель, чтобы была зависимость от проекта
        public string CardBodyClass => cssClass.Item2;
        private Tuple<string, string> cssClass => _project.CssStyle;
        private CardStatus _status;
        private ProjectModel _project;
        private string _projectName;
        //public string Comments { get; set; } - TODO - если будет время
    }

    public enum CardStatus
    {
        Analysis,
        Developing,
        Testing,
        Done,
        Backlog
    }

}