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
        public System.Guid IdPerson { get; set; }

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
        [DisplayName("Имя")]
        public int Age { get; set; }

        [DisplayName("Отчество")]
        public string Gender { get; set; }

        [Required]
        [DisplayName("Трудоустроен")]
        public bool HasJob { get; set; }
    }
}