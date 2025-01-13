using System.ComponentModel.DataAnnotations;

namespace SocialNetworkOKO.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email является обязательным")]
        [EmailAddress(ErrorMessage = "Некорректный адрес электронной почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль является обязательным")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
