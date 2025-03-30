using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Bookstore.DTO
{
    public class StoreManagementDTO
    {
        public int Id { get; set; }
        //[Required(ErrorMessage = "Name is required.")]
        //[StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters long.")]
        //[RegularExpression(@"^[A-Za-z0-9]+(?: [A-Za-z0-9]+)*$", ErrorMessage = "Name is not valid. It must not begin or end with a space and must contain only letters, numbers, and spaces.")]
        public string Name { get; set; }

        //[Required(ErrorMessage = "Street is required.")]
        //[StringLength(50, MinimumLength = 2, ErrorMessage = "Street must be between 2 and 50 characters long.")]
        //[RegularExpression(@"^[A-Za-z0-9]+(?: [A-Za-z0-9]+)*$", ErrorMessage = "Street name is not valid. It must not begin or end with a space and must contain only letters, numbers, and spaces.")]
        public string Street { get; set; }
        //[Required(ErrorMessage = "City is required.")]
        //[StringLength(50, MinimumLength = 2, ErrorMessage = "City must be between 2 and 50 characters long.")]
        //[RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)*$", ErrorMessage = "City is not valid. It must not begin or end with a space and must contain only letters and spaces.")]
        public string City { get; set; }
        //[Required(ErrorMessage = "E-mail address is required.")]
        //[RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,3}$", ErrorMessage = "E-mail is not valid.")]
        public string Email { get; set; }
        //[Required(ErrorMessage = "Phone is required.")]
        //[StringLength(15, MinimumLength = 9, ErrorMessage = " must be between 9 and 13 characters long.")]
        //[RegularExpression("^(\\+?[0-9]{1,3})? ?([0-9]{3} ?[0-9]{3} ?[0-9]{3})$", ErrorMessage = "Phone is not valid. Only country calling code and phone number (9 digits) are allowed.")]
        public string Phone { get; set; }
    }
}
