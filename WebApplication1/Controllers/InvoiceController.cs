using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.Entities;

namespace WebApplication1.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _service;

        public InvoiceController(IInvoiceService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Index(int customerId, string alphaFilter, int? selectedInvoiceId = null)
        {
            var customer = _service.GetCustomerById(customerId);
            if (customer == null)
            {
                return NotFound();
            }
            var allPaymentTerms = _service.GetAllPaymentTerms();
            var invoices = _service.GetInvoicesForCustomer(customerId).ToList();
            var invoiceViewModels = new List<InvoiceViewModel>();
            var paymentTermsList = new List<PaymentTermsViewModel>();


            foreach (var invoice in invoices)
            {
                var paymentTerms = _service.GetPaymentTermsByInvoiceId(invoice.InvoiceId);
                invoiceViewModels.Add(new InvoiceViewModel
                {
                    InvoiceId = invoice.InvoiceId,
                    DueDate = invoice.InvoiceDate?.AddDays(paymentTerms.DueDays) ?? DateTime.MinValue,
                    AmountPaid = (decimal)(invoice.PaymentTotal ?? 0),
                    PaymentTermsDescription = paymentTerms.DueDays,
                    PaymentTermsId = paymentTerms.PaymentTermsId
                });

                if (!paymentTermsList.Any(pt => pt.PaymentTermsId == paymentTerms.PaymentTermsId))
                {
                    paymentTermsList.Add(new PaymentTermsViewModel
                    {
                        PaymentTermsId = paymentTerms.PaymentTermsId,
                        Description = paymentTerms.Description,
                        DueDays = paymentTerms.DueDays
                    });
                }
            }

            // If selectedInvoiceId is not provided, default to the first invoice in the list
            selectedInvoiceId = selectedInvoiceId ?? invoices.FirstOrDefault()?.InvoiceId;

            var selectedInvoiceLineItems = selectedInvoiceId.HasValue
                ? _service.GetLineItemsByInvoiceId(selectedInvoiceId.Value)
                    .Select(li => new InvoiceLineItemViewModel
                    {
                        Description = li.Description,
                        Amount = (decimal)li.Amount
                    }).ToList()
                : new List<InvoiceLineItemViewModel>();

            var viewModel = new CustomerInvoicesViewModel
            {
                CompanyName = customer.Name,
                CompanyAddress = customer.Address1 + ", " + customer.City,
                CompanyCity = customer.City,
                PaymentTerms = paymentTermsList,
                SelectedInvoiceId = selectedInvoiceId,
                Invoices = invoiceViewModels,
                SelectedInvoiceLineItems = selectedInvoiceLineItems,
                // payment terms
                 AllPaymentTerms = allPaymentTerms.Select(pt => new PaymentTermsViewModel
                 {
                     PaymentTermsId = pt.PaymentTermsId,
                     Description = pt.Description,
                     DueDays = pt.DueDays
                 }).ToList()
            };

            ViewBag.CurrentFilter = alphaFilter;
            ViewBag.CustomerId = customerId;

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetLineItems(int invoiceId)
        {
            var lineItems = _service.GetLineItemsByInvoiceId(invoiceId)
                .Select(li => new InvoiceLineItemViewModel
                {
                    Description = li.Description,
                    Amount = (decimal)li.Amount
                }).ToList();

            var viewModel = new CustomerInvoicesViewModel
            {
                SelectedInvoiceId = invoiceId,
                SelectedInvoiceLineItems = lineItems.Select(li => new InvoiceLineItemViewModel
                {
                    Description = li.Description,
                    Amount = (decimal)li.Amount
                }).ToList()
            };

            return PartialView("_InvoiceLineItems", viewModel);
        }



        [HttpPost]
        public IActionResult AddInvoice(int customerId, DateTime invoiceDate, int paymentTermsId)
        {
            //  logic to create a new invoice
            var newInvoice = new Invoice
            {
                InvoiceDate = invoiceDate,
                PaymentTermsId = paymentTermsId,
                CustomerId = customerId // Assuming this is the foreign key to Customer
            };
            _service.AddInvoice(newInvoice);

            // Redirecting back to the Index with the customerId and newly added invoiceId
            return RedirectToAction("Index", new { customerId = customerId, selectedInvoiceId = newInvoice.InvoiceId });
        }

        [HttpPost]
        public IActionResult AddLineItem(int invoiceId, string description, decimal amount, int customerId)
        {
            // logic to create a new line item
            var newLineItem = new InvoiceLineItem
            {
                InvoiceId = invoiceId,
                Description = description,
                Amount = (double?)amount
            };
            _service.AddLineItem(newLineItem);

        return RedirectToAction("Index", new { customerId = customerId, selectedInvoiceId = invoiceId });
        }

        public object Index(int customerId)
        {
            throw new NotImplementedException();
        }
    }
}
