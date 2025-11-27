namespace WebApplication1.Entities
{
    public class InvoiceLineItem
    {
        public int InvoiceLineItemId { get; set; }

        public double? Amount { get; set; }

        public string? Description { get; set; }

        // FK:
        public int? InvoiceId { get; set; }

        // Navigation property because Each line item belongs to one invoice
        public Invoice Invoice { get; set; }
    }
}
