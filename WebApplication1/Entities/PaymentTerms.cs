namespace WebApplication1.Entities
{
    public class PaymentTerms
    {
        public int PaymentTermsId { get; set; }

        public string Description { get; set; } = null!;

        public int DueDays { get; set; }

        // Navigation property because Payment terms apply to many invoices
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
