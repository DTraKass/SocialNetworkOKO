using System.ComponentModel.DataAnnotations;

namespace SocialNetworkOKO.Models
{
    public class UserEditViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string FirstName { get; internal set; }

        [Required]
        public string LastName { get; internal set; }

        [Required]
        public string MiddleName { get; internal set; }

        [Required]
        public DateTime BirthDate { get; internal set; }

        [Required]
        public string Image { get; internal set; }

        [Required]
        public string Status { get; internal set; }
        [Required]
        public string About { get; internal set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }


    }
}
