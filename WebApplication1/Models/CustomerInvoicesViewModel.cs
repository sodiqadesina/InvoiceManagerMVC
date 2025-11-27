namespace WebApplication1.Models
{
    public class CustomerInvoicesViewModel
    {
        public CustomerInvoicesViewModel()
        {
            Invoices = new List<InvoiceViewModel>();
        }

        public List<PaymentTermsViewModel> AllPaymentTerms { get; set; }

        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyCity { get; set; }

        // This should be a list because there could be multiple payment terms across invoices
        public List<PaymentTermsViewModel> PaymentTerms { get; set; }

        // To store the ID of the currently selected invoice
        public int? SelectedInvoiceId { get; set; }

        // The list of all invoices for the customer
        public List<InvoiceViewModel> Invoices { get; set; }

        // The line items for the currently selected invoice
        public List<InvoiceLineItemViewModel> SelectedInvoiceLineItems { get; set; }
    }

   
}
