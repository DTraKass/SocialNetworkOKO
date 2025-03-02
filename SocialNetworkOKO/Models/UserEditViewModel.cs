using System.ComponentModel.DataAnnotations;

namespace SocialNetworkOKO.Models
{
    public class UserEditViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string FirstName { get;  set; }

        [Required]
        public string LastName { get;  set; }

        [Required]
        public string MiddleName { get;  set; }

        [Required]
        public DateTime BirthDate { get;  set; }
        public string Image { get;  set; }

        [Required]
        public string Status { get; set; }
        [Required]
        public string About { get; set; }
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }


    }
}
