using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class CustomerViewModel
    {
        public int? CustomerId { get; set; } // Making nullable to distinguish between add and edit

        [Required(ErrorMessage = "Please enter a name.")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Address Line 1 is required.")]
        [Display(Name = "Address Line 1")]
        public string Address1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string? Address2 { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "State/Province is required.")]
        [Display(Name = "State/Province")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "State/Province code must be 2 characters long.")]
        public string ProvinceOrState { get; set; }

        [Required(ErrorMessage = "Zip/Postal Code is required.")]
        [Display(Name = "Zip/Postal Code")]
        public string ZipOrPostalCode { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string Phone { get; set; }

        [Display(Name = "Contact First Name")]
        public string ContactFirstName { get; set; }

        [Display(Name = "Contact Last Name")]
        public string ContactLastName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Contact Email")]
        public string ContactEmail { get; set; }
    }
}
