
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace SocialNetworkOKO.Models
{
    public class UserEditViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Image { get; internal set; }
        public string LastName { get; internal set; }
        public string MiddleName { get; internal set; }
        public string FirstName { get; internal set; }
        public DateTime BirthDate { get; internal set; }
        public string Status { get; internal set; }
        public string About { get; internal set; }
    }
}
