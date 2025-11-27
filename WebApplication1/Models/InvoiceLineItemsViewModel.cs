namespace WebApplication1.Models
{
    public class InvoiceLineItemsViewModel
    {
        public int? SelectedInvoiceId { get; set; }
        public List<InvoiceLineItemViewModel> LineItems { get; set; }

        public decimal Total => LineItems.Sum(li => li.Amount);
    }
}
