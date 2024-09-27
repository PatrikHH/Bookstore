using System.ComponentModel.DataAnnotations;

namespace Bookstore.ViewModels
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "The Name field is required.")]
        public string Name { get; set; }
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid.")]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
