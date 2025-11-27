using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WebApplication1.Entities
{
    public class Customer
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address1 { get; set; }

        public string? Address2 { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "Province/State is required")]
        [RegularExpression("^[A-Za-z]{2}$", ErrorMessage = "Province/State must be a 2 letter state code")]
        public string ProvinceOrState { get; set; }

        [Required(ErrorMessage = "Zip/Postal code is required")]
        [RegularExpression(@"^(?:\d{5}(-\d{4})?|[A-Za-z]\d[A-Za-z] ?\d[A-Za-z]\d)$", ErrorMessage = "Invalid Zip/Postal code format")]
        public string ZipOrPostalCode { get; set; }



        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; }

        public string? ContactLastName { get; set; }

        public string? ContactFirstName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? ContactEmail { get; set; }

        public bool IsDeleted { get; set; } = false;

        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
