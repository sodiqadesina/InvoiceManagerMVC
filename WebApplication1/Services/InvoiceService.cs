using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly CustomerDbContext _context;
        public InvoiceService(CustomerDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Invoice> GetInvoicesForCustomer(int customerId)
        {
            return _context.Invoices.Where(i => i.CustomerId == customerId).ToList();

        }

        public IEnumerable<PaymentTerms> GetAllPaymentTerms()
        {
            return _context.PaymentTerms.ToList();
        }

        public Customer GetCustomerById(int customerId)
        {
            return _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
        }
        public Invoice GetInvoiceById(int invoiceId)
        {
            return _context.Invoices.FirstOrDefault(i => i.InvoiceId == invoiceId);
        }

        public void AddInvoice(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            _context.SaveChanges();
        }

        public void UpdateInvoice(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            _context.SaveChanges();
        }

        public void DeleteInvoice(int invoiceId)
        {
            var invoice = GetInvoiceById(invoiceId);
            if (invoice != null)
            {
                _context.Invoices.Remove(invoice);
                _context.SaveChanges();
            }
        }


        public IEnumerable<InvoiceLineItem> GetLineItemsByInvoiceId(int invoiceId)
        {
            // TO retrieve all line items for a given invoice
            return _context.InvoiceLineItems
                           .Where(li => li.InvoiceId == invoiceId)
                           .ToList();
        }

        public void AddLineItem(InvoiceLineItem lineItem)
        {
            // This will add a new line item to the database
            if (lineItem == null)
            {
                throw new ArgumentNullException(nameof(lineItem));
            }

            _context.InvoiceLineItems.Add(lineItem);
            _context.SaveChanges();
        }

        public PaymentTerms GetPaymentTermsByInvoiceId(int invoiceId)
        {
         
            return _context.Invoices
                           .Where(i => i.InvoiceId == invoiceId)
                           .Include(i => i.PaymentTerms)
                           .Select(i => i.PaymentTerms)
                           .FirstOrDefault();
        }





    }
}
