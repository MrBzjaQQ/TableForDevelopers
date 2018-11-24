using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace TableForDevelopers.Models
{
    public class CardModel
    {
        public CardModel()
        {
            Header = "Создать доску для управления проектом\n";
            AppointedDeveloper = "Ершов Илья\n";
            Status = CardStatus.Developing;
            Description = "Сделать формы, потом припилить логику.\n";
            Project = "Курсач по WEB\n";
            //load bitmap
        }
        //Будет отображено на карточке на доске
        public string Header { get; set; } //Заголовок
        public string AppointedDeveloper { get; set; } //Назначенный разработчик: Можно создать тип Developer
        //public Tuple<string, Bitmap> CardType { get; set; } - TODO - если будет время
        //Будет дополнительно отображено на карточке в модальном окне
        public CardStatus Status { get; set; }
        public string Description { get; set; }
        public string Project { get; set; }
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