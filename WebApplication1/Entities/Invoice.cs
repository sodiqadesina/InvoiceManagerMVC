namespace WebApplication1.Entities
{
    public class Invoice
    {
        public int InvoiceId { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public DateTime? InvoiceDueDate
        {
            get
            {
                return InvoiceDate?.AddDays(Convert.ToDouble(PaymentTerms?.DueDays));
            }
        }

        public double? PaymentTotal { get; set; } = 0.0;

        public DateTime? PaymentDate { get; set; }

        // FK:
        public int PaymentTermsId { get; set; }

        // FK:
        public int CustomerId { get; set; }


        // Each invoice belongs to one customer
        public Customer Customer { get; set; }

        // Each invoice has many line items
        public ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();

        // Each invoice has one set of payment terms
        public PaymentTerms PaymentTerms { get; set; }
    }
}
