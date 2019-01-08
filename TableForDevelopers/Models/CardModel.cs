using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TableForDevelopers.Models
{
    public class CardModel
    {
        public CardModel()
        {
            CardID = 0;
            Header = "Создать доску для управления проектом\n";
            AppointedDeveloper = "Ершов Илья\n";
            Status = CardStatus.Developing.ToString();
            Description = "Сделать формы, потом припилить логику.\n";
            Project = "Курсач по WEB\n";
            _project = new ProjectModel();
            _project.CssStyle = CSSClassModel.Info;
            //load bitmap
        }
        //Будет отображено на карточке на доске
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
        public string Project { get; set; } //TODO найти проект по названию
        public string CardClass => cssClass.Item1; //классы стилей вынесены в модель, чтобы была зависимость от проекта
        public string CardBodyClass => cssClass.Item2;
        private Tuple<string, string> cssClass => _project.CssStyle;
        private CardStatus _status;
        private ProjectModel _project;
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