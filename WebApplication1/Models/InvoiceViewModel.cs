using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class InvoiceViewModel
    {

        // Constructor to initialize the LineItems list
        public InvoiceViewModel()
        {
            LineItems = new List<InvoiceLineItemViewModel>();
        }
        public int InvoiceId { get; set; }

        [Required]
        public DateTime DueDate { get; set; }
        public decimal AmountPaid { get; set; }

        [Required]
        public int PaymentTermsId { get; set; } // To reference PaymentTerms directly
        public int PaymentTermsDescription { get; set; } // To display the payment terms description

        public List<InvoiceLineItemViewModel> LineItems { get; set; } // Line items for this invoice


    }
}
