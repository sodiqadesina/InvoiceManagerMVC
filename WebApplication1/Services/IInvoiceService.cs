using WebApplication1.Entities;

namespace WebApplication1.Services
{
    public interface IInvoiceService
    {
        IEnumerable<Invoice> GetInvoicesForCustomer(int customerId);

        IEnumerable<PaymentTerms> GetAllPaymentTerms();
        Customer GetCustomerById(int customerId);
        Invoice GetInvoiceById(int invoiceId);
        void AddInvoice(Invoice invoice);
        void UpdateInvoice(Invoice invoice);
        void DeleteInvoice(int invoiceId);

        IEnumerable<InvoiceLineItem> GetLineItemsByInvoiceId(int invoiceId);
        void AddLineItem(InvoiceLineItem lineItem);

        PaymentTerms GetPaymentTermsByInvoiceId(int invoiceId);
    }
}




