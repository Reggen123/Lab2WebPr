using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lab2WebPr.Models.ViewModels
{
    public class PersonVM
    {
        public System.Guid Id { get; set; }


        [Required]
        [DisplayName("Фамилия")]
        [StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        [DisplayName("Имя")]
        public string FirstName { get; set; }

        [DisplayName("Отчество")]
        public string Patronymic { get; set; }

        [Required]
        [Range(18, 100)]
        [DisplayName("Возраст")]
        public int Age { get; set; }

        [DisplayName("Пол")]
        public string Gender { get; set; }

        [Required]
        [DisplayName("Трудоустроен")]
        public bool HasJob { get; set; }

        [Required]
        [DisplayName("ID Пользователя")]
        public System.Guid UserID { get; set; }

        [DisplayName("Дата рождения")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Birthday { get; set; }

        [DisplayName("Дата добавления")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddThh:mm}", ApplyFormatInEditMode = true)]

        public DateTime IstertDateTime { get; set; }

        [DisplayName("Время подъёма")]
        [DisplayFormat(DataFormatString = "{0:HH:mm:ss}", ApplyFormatInEditMode = true)]

        public DateTime WakeUpTime { get; set; }
    }
}