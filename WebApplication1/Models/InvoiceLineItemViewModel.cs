using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class InvoiceLineItemViewModel
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}
