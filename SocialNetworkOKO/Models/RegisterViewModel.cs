using System.ComponentModel.DataAnnotations;

namespace SocialNetworkOKO.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string EmailReg { get; set; }

        [Required]
        [Display(Name = "Год")]
        public int Year { get; set; }

        [Required]
        [Display(Name = "День")]
        public int Date { get; set; }

        [Required]
        [Display(Name = "Месяц")]
        public int Month { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        [StringLength(100, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 5)]
        public string PasswordReg { get; set; }

        [Required]
        [Compare("PasswordReg", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm {  get; set; }

        [Required]
        [Display(Name = "Никнейм")]
        public string Login { get; set; }

        public enum Months
        {
            [Display(Name = "Январь")]
            Jan = 1,
            [Display(Name = "Февраль")]
            Feb = 2,
            [Display(Name = "Март")]
            Mar = 3,
            [Display(Name = "Апрель")]
            Apr = 4,
            [Display(Name = "Май")]
            May = 5,
            [Display(Name = "Июнь")]
            Jun = 6,
            [Display(Name = "Июль")]
            Jul = 7,
            [Display(Name = "Август")]
            Aug = 8,
            [Display(Name = "Сентябрь")]
            Sep = 9,
            [Display(Name = "Октябрь")]
            Oct = 10,
            [Display(Name = "Ноябрь")]
            Nov = 11,
            [Display(Name = "Декабрь")]
            Dec = 12,
        }
    }
}
