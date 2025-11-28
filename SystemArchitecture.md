#   1. High-level purpose
  This is an ASP.NET Core 7 MVC Invoice Tracking web app for a small business to:

  - Maintain a list of customers (with contact info and soft-delete).
  
  - Define payment terms (e.g., Net 30, Net 60).
  
  - Create and manage invoices for each customer.
  
  - Attach line items (description + amount) to each invoice.
  
  - View all invoices and line items for a selected customer in a master-detail UI.
  
  - Supports this logic with EF Core + SQL Server and a small xUnit test project.
  
  So functionally: you pick a customer → see their invoices → pick an invoice → see its line items → add new invoices/line items.

# 2. Solution structure

  At the root we really have two projects:
  
  - WebApplication1/ – the main ASP.NET Core MVC app (the invoice system).
  
  - TestProject1/ – an xUnit test project targeting WebApplication1.

  Everything interesting is under WebApplication1.
  
  - Key folders:
  
    - Entities/ – EF Core entities and CustomerDbContext.
    
    - Services/ – application services (CustomerService, InvoiceService) + interfaces.
    
    - Controllers/ – MVC controllers for customers, invoices, and home.
    
    - Models/ – view models used by the views.
    
    - Views/ – Razor pages for Customers, Invoice, Home, Shared.
    
    - wwwroot/ – static assets: Bootstrap, jQuery, site.css, site.js.
    
    - Migrations/ – EF Core migrations, plus seeding logic in CustomerDbContext.

# 3. Data / domain model (Entities)

  All entities live in WebApplication1.Entities.

  ## 3.1 Customer
  
  - Customer.cs:
  
    - Key: CustomerId
    
    - Core info:
    
    - Name (required)
    
    - Address1 (required), optional Address2
    
    - City (required)
    
    - ProvinceOrState – 2-letter code, validated by regex.
    
    - ZipOrPostalCode
    
    - Country
    
    - Phone – validated via regex (North-American style pattern).
  
  - Contact info:
  
    - ContactLastName
    
    - ContactFirstName
    
    - ContactEmail (with [EmailAddress] validation).
  
  - Soft delete:
  
    - IsDeleted (bool, default false).
  
  - Relationships:
  
    - ICollection<Invoice> Invoices – a customer has many invoices.
  
  So it models a fairly complete customer profile used for invoicing.
  
 ## 3.2 Invoice
  
  - Invoice.cs:
  
    - Key: InvoiceId
  
  - Dates:
  
    - InvoiceDate (nullable DateTime?).
    
    - InvoiceDueDate – computed in the getter as InvoiceDate + PaymentTerms.DueDays.
  
  - Money:
  
    - PaymentTotal (nullable double?, default 0.0).
    
  - Relationships:
  
    - CustomerId (FK).
    
    - Customer Customer – each invoice belongs to one customer.
    
    - ICollection<InvoiceLineItem> LineItems – one invoice → many line items.
    
    - PaymentTerms PaymentTerms – one invoice → one payment terms.
    
    - There is also a PaymentTermsId used by the service/controller when constructing invoices (seen in code using it).
  
## 3.3 InvoiceLineItem
  
  - InvoiceLineItem.cs:
  
    - Key: InvoiceLineItemId
  
  - Fields:
  
    - Amount (double?)
  
    - Description (string?)
  
  - Relationships:
  
    - InvoiceId (nullable int FK).
    
    - Invoice Invoice – each line item belongs to a single invoice.
    
## 3.4 PaymentTerms
  
  PaymentTerms.cs:
  
  - Key: PaymentTermsId
  
  - Fields:
  
    - Description (e.g., “Net 30”).
    
    - DueDays (int).
  
  - Relationship:
  
    - ICollection<Invoice> Invoices – one payment terms used by many invoices.
  
## 3.5 CustomerDbContext
  
  CustomerDbContext.cs:
  
  - Inherits from DbContext.
  
  - DbSet<Customer> Customers
  
  - DbSet<Invoice> Invoices
  
  - DbSet<InvoiceLineItem> InvoiceLineItems
  
  - DbSet<PaymentTerms> PaymentTerms
  
  In OnModelCreating:
  
  - Configures relationships between the four entities.
  
  - Seeds data for:
  
    - Multiple customers with realistic fields.
    
    - Predefined payment terms (e.g., 30, 60 days).
    
    - Several invoices mapped to different customers + payment terms.
    
    - A set of invoice line items (e.g., “Trip 1”, “Service ABC”, etc.) attached to invoices.
  
  So when you run migrations, the database is pre-populated with sample customers, invoices, line items, and terms.

# 4. View models (Models)

These live in WebApplication1.Models and exist to shape data for Razor views, separate from EF entities.

## 4.1 CustomerViewModel

CustomerViewModel.cs:

  - CustomerId (int?) – nullable to distinguish add vs edit.
  
  - Required fields with display names and validation messages:

    - Name
    
    - Address1
    
    - City
    
    - ProvinceOrState (2-letter).
    
    - ZipOrPostalCode
    
    - Country
    
    - Phone – [DataType(DataType.PhoneNumber)] + regex.

  - Optional contact fields:

    - ContactFirstName, ContactLastName, ContactEmail (with [EmailAddress]).

Used by the Add/Edit Customer view and controller actions.

## 4.2 InvoiceLineItemViewModel

InvoiceLineItemViewModel.cs:

  - [Required] string Description
  
  - [Required] decimal Amount
  
  - Used for displaying and posting line items in the invoice UI.

## 4.3 InvoiceViewModel

InvoiceViewModel.cs:

- Constructor initializes LineItems = new List<InvoiceLineItemViewModel>();

- Fields:

  - InvoiceId
  
  - DueDate (DateTime, required).
  
  - AmountPaid (decimal).
  
  - PaymentTermsId (required).
  
  - PaymentTermsDescription (stored as int in code, representing days – conceptually “Net X”).
  
  - List<InvoiceLineItemViewModel> LineItems.

This is the per-invoice representation used in the invoice page.

## 4.4 InvoiceLineItemsViewModel

InvoiceLineItemsViewModel.cs:

  - int? SelectedInvoiceId
  
  - List<InvoiceLineItemViewModel> LineItems
  
  - Total => LineItems.Sum(li => li.Amount) (computed decimal total).

## 4.5 PaymentTermsViewModel

PaymentTermsViewModel.cs:

  - PaymentTermsId
  
  - Description
  
  - int? DueDays

## 4.6 CustomerInvoicesViewModel

CustomerInvoicesViewModel.cs:

This is the big composite model driving the invoice screen:

  - Constructor initializes Invoices = new List<InvoiceViewModel>();
  
  - List<PaymentTermsViewModel> AllPaymentTerms – all available payment terms in the system.
  
  - Company info:
  
    - CompanyName
    
    - CompanyAddress
    
    - CompanyCity
  
  - List<PaymentTermsViewModel> PaymentTerms – unique terms used across this customer’s invoices.
  
  - int? SelectedInvoiceId – which invoice is selected in the UI.
  
  - List<InvoiceViewModel> Invoices – invoices for a given customer.
  
  - List<InvoiceLineItemViewModel> SelectedInvoiceLineItems – line items of the selected invoice.

## 4.7 ErrorViewModel

Standard error model:

- RequestId and ShowRequestId.

# 5. Services (application logic)

Services encapsulate business/data access logic and are registered with DI in Program.cs.

## 5.1 ICustomerService + CustomerService

ICustomerService defines:

  - IEnumerable<Customer> GetAllCustomers()
  
  - Customer GetCustomerById(int customerId)
  
  - void AddCustomer(Customer customer)
  
  - void UpdateCustomer(Customer customer)
  
  - void DeleteCustomer(int customerId) – implemented as soft delete (IsDeleted = true).

CustomerService implementation:

  - Uses CustomerDbContext.
  
  - GetAllCustomers() filters !IsDeleted so deleted customers are hidden.
  
  - DeleteCustomer sets IsDeleted = true and saves.
  
  -AddCustomer and UpdateCustomer do standard EF Core Add / Update + SaveChanges().

## 5.2 IInvoiceService + InvoiceService

IInvoiceService defines:

  - IEnumerable<Invoice> GetInvoicesForCustomer(int customerId)
  
  - IEnumerable<PaymentTerms> GetAllPaymentTerms()
  
  - Customer GetCustomerById(int customerId)
  
  - Invoice GetInvoiceById(int invoiceId)
  
  - void AddInvoice(Invoice invoice)
  
  - void UpdateInvoice(Invoice invoice)
  
  - void DeleteInvoice(int invoiceId)
  
  - IEnumerable<InvoiceLineItem> GetLineItemsByInvoiceId(int invoiceId)
  
  - void AddLineItem(InvoiceLineItem lineItem)
  
  - PaymentTerms GetPaymentTermsByInvoiceId(int invoiceId)

InvoiceService implementation:

  - Uses CustomerDbContext.
  
  - GetInvoicesForCustomer – returns all invoices for given customer.
  
  - GetAllPaymentTerms – list of all payment terms.
  
  - GetCustomerById & GetInvoiceById – simple lookups.
  
  - AddInvoice, UpdateInvoice, DeleteInvoice – EF Core CRUD.
  
  - GetLineItemsByInvoiceId – pulls InvoiceLineItems for an invoice.
  
  - AddLineItem – adds a line item with null checks, then saves.
  
  - GetPaymentTermsByInvoiceId – loads invoice with .Include(i => i.PaymentTerms) and returns the related PaymentTerms.
  
So the InvoiceController never touches EF DbContext directly; it uses this service.

# 6. Web layer (controllers + Program.cs)
## 6.1 Startup / Program

Program.cs:

  - Builds the web app.
  
  - Adds MVC:
  
        builder.Services.AddControllersWithViews();

  - Configures EF Core:
  
        var connectstr = builder.Configuration.GetConnectionString("CourseConnectionString");
        builder.Services.AddDbContext<CustomerDbContext>(option => option.UseSqlServer(connectstr));
        
  
  - Dependency injection:
  
    - AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
    
    - AddScoped<ICustomerService, CustomerService>()
    
    - AddScoped<IInvoiceService, InvoiceService>()
  
  - Pipeline:
  
    - Exception handler and HSTS in non-development.
    
    - UseHttpsRedirection, UseStaticFiles, UseRouting, UseAuthorization.
    
    - Default route: {controller=Home}/{action=Index}/{id?}.

## 6.2 HomeController

HomeController.cs:

  - Index() – returns the home page.
  
  - Privacy() – returns privacy page.
  
  - Error() – returns error view with ErrorViewModel.

This is generic and not domain-specific.

## 6.3 CustomersController

CustomersController.cs (routed as [Route("customers")]):

Injected service: ICustomerService.

Actions

- Index(string alphaFilter = "A-E") (GET at /customers):

  - Fetches all non-deleted customers from CustomerService.
  
  - Applies an alphabetical filter on customer Name[0]:
  
    - "A-E", "F-K", "L-R", "S-Z".
  
  - Maps each Customer to a CustomerViewModel.
  
  - Stores alphaFilter in ViewBag.CurrentFilter for the UI.
  
  - Returns IEnumerable<CustomerViewModel> to Views/Customers/Index.cshtml.

- AddOrEdit(int? id) (GET customers/add-or-edit/{id?}):

  - If id exists:

    - Loads customer.
    
    - If null → NotFound().
    
    - Otherwise maps to CustomerViewModel and returns the edit view.

  - If id is null:

    - Returns an empty CustomerViewModel so the view acts as Add mode.

  - The view uses CustomerId > 0 to decide Add vs Edit.

- AddOrEdit(int? id, CustomerViewModel model) (POST):

  - Validates ModelState.

  - If valid:

    - Either loads existing Customer (edit) or creates new one.
    
    - Copies fields from view model to entity (Name, Address1/2, City, ProvinceOrState, Zip, Phone, ContactFirstName, ContactLastName, ContactEmail).
    
    - If id.HasValue → _customerService.UpdateCustomer(customer).
    
    - Else → _customerService.AddCustomer(customer).
    
    - Redirects to Index.

  - If invalid:
  
    - Redisplays the form.

- Delete(int id, string name) (GET):

  - Calls _customerService.DeleteCustomer(id) (soft delete).
  
  - Uses TempData:

    - TempData["DeletedCustomerId"] = id
    
    - TempData["Message"] = "Customer \"{name}\" was deleted successfully."

  - Redirects to Index.

- UndoDelete(int id) (GET):

  - Fetches customer, if null → NotFound().
  
  - Sets customer.IsDeleted = false.
  
  - Calls _customerService.UpdateCustomer(customer).
  
  - Redirects back to Index.

So the Customers area gives:

  - List with A-Z filters.
  
  - Add / edit form with validation.
  
  - Delete with “undo delete” UX using TempData.

## 6.4 InvoiceController

InvoiceController.cs:

Injected service: IInvoiceService.

Index(int customerId, string alphaFilter, int? selectedInvoiceId = null) (GET)

This is the main invoice screen for a given customer.

Flow:

  1. Get the Customer by customerId. If not found → NotFound().
  
  2. Get all payment terms: _service.GetAllPaymentTerms().
  
  3. Get invoices for that customer: _service.GetInvoicesForCustomer(customerId).ToList().
  
  4. For each invoice:

  - Fetch PaymentTerms for that invoice using _service.GetPaymentTermsByInvoiceId(invoice.InvoiceId).
  
  - Build an InvoiceViewModel:
  
    - InvoiceId
    
    - DueDate = InvoiceDate + PaymentTerms.DueDays
    
    - AmountPaid = invoice.PaymentTotal ?? 0 cast to decimal.
    
    - PaymentTermsDescription = paymentTerms.DueDays (so UI shows “X days”).
    
    - PaymentTermsId
  
  Also build a paymentTermsList of unique PaymentTermsViewModel used across these invoices.

  5. If selectedInvoiceId is null:

  - Default to the first invoice’s id (if any).

  6. Load line items for selectedInvoiceId (if set) using _service.GetLineItemsByInvoiceId → map to InvoiceLineItemViewModel list.

  7. Construct a CustomerInvoicesViewModel:

  - Company info from the customer record.
  
  - PaymentTerms = paymentTermsList.
  
  - SelectedInvoiceId
  
  - Invoices = list of InvoiceViewModel.
  
  - SelectedInvoiceLineItems = line items for selected invoice.
  
  - AllPaymentTerms = all payment terms in the system, mapped to view models.

  8. Set:

  - ViewBag.CurrentFilter = alphaFilter (for “Return to customers filter” link).
  
  - ViewBag.CustomerId = customerId.

  9. Return View(viewModel) → Views/Invoice/Index.cshtml.

GetLineItems(int invoiceId) (GET, async)

  - Uses _service.GetLineItemsByInvoiceId(invoiceId).
  
  - Maps to new InvoiceLineItemViewModel list (Description + Amount).
  
  - Builds a mini CustomerInvoicesViewModel with:
  
    - SelectedInvoiceId
    
    - SelectedInvoiceLineItems = lineItems.
    
  - Returns PartialView("_InvoiceLineItems", viewModel).

This supports the AJAX behavior in the invoice page to reload the line items table when you click a different invoice radio button.

AddInvoice(int customerId, DateTime invoiceDate, int paymentTermsId) (POST)

- Builds a new Invoice object:

  - InvoiceDate = invoiceDate
  
  - PaymentTermsId = paymentTermsId
  
  - CustomerId = customerId

- Calls _service.AddInvoice(newInvoice).

- Redirects to Index with customerId and selectedInvoiceId = newInvoice.InvoiceId so the newly created invoice becomes selected.

AddLineItem(int invoiceId, string description, decimal amount, int customerId) (POST)

- Creates new InvoiceLineItem:

  - InvoiceId = invoiceId
  
  - Description = description
  
  - Amount = (double?)amount

- Calls _service.AddLineItem(newLineItem).

- Redirects back to Index with customerId and selectedInvoiceId = invoiceId to show the updated invoice.

# 7. Views / UI behavior
## 7.1 Layout and shared views

  - _Layout.cshtml – standard Bootstrap layout with navbar, body, scripts.
  
  - _ValidationScriptsPartial.cshtml – includes jQuery validation + unobtrusive scripts.
  
  - Error.cshtml – error display using ErrorViewModel.

## 7.2 Customers area views

Under Views/Customers/:

  - Index.cshtml:
  
    - Model: IEnumerable<CustomerViewModel>.
    
    - Shows a table of customers (Name, city, etc.).
    
    - “Add a new customer” link → Customers/AddOrEdit.
    
    - Shows a TempData message after delete with an “Undo Delete” link calling UndoDelete.
    
    - Has buttons A–E, F–K, L–R, S–Z that call the Index action with alphaFilter route values; uses currentFilter from ViewBag to highlight active filter.
    
    - Includes JS to auto-hide TempData message after 6 seconds.

  - AddOrEdit.cshtml:
  
    - Model: CustomerViewModel.
    
    - Determines isEditMode using CustomerId > 0.
    
    - Renders a form that posts back to AddOrEdit action.
    
    - Uses asp-for helpers for fields with validation messages.
    
    - Shows a generic error banner if any field has a validation error (client-side jQuery script scans .field-validation-error).
  
  - Delete.cshtml and Detials.cshtml exist but are not central — Delete is handled via direct action plus redirect; Detials is likely a customer details view.

## 7.3 Invoice area views

Under Views/Invoice/:

  - Index.cshtml:

    - Model: CustomerInvoicesViewModel.
    
    - Layout: two-column/master-detail UI:

      - Left side:
        
        - List of invoices (Model.Invoices) with radio buttons (one per invoice).
        
        - Each invoice row shows due date, amount, and some payment terms info.
        
        - Add-invoice form at the bottom – fields for InvoiceDate and PaymentTermsId (dropdown from Model.AllPaymentTerms).

      - Right side:

        - The selected invoice’s line items rendered via _InvoiceLineItems partial.
        
        - A form to add new line item (description + amount) that posts to AddLineItem.

  - A “Return to XYZ customers” link uses ViewBag.CurrentFilter to go back to the same customers filter.

  - JavaScript:
  
    - Listens for changes on invoice radio buttons.
    
    - When a radio button is selected:
  
      - Makes an AJAX call to Invoice/GetLineItems to update the _InvoiceLineItems section.
      
      - Updates a hidden InvoiceId input in the add-line-item form.
      
      - Updates a visual label for payment terms (e.g., “Terms: 30 days”) using a data-payment-terms attribute.

  - _InvoiceLineItems.cshtml:
  
    - Model: CustomerInvoicesViewModel (but uses only SelectedInvoiceId + SelectedInvoiceLineItems).
    
    - If there are line items:
    
      - Shows “Line items for invoice #X”.
      
      - Renders a table of Description + Amount (formatted as currency).
      
      - Shows total sum at the bottom.
    
    - Otherwise: displays “No invoice line items!!!”.
   
# 8. Test project (TestProject1)

TestProject1 is an xUnit test project that references WebApplication1.

Key file: WebApllication1-Test.cs (yes, with the typo).

## 8.1 AddorEditFormTest

  - Tests validation on Customer entity and/or view model:
  
    - Valid when all required fields present.
    
    - Invalid when required fields are missing.
    
    - Invalid for misformatted fields (phone, etc.).

## 8.2 CustomersControllerTests

  - Uses Moq to mock ICustomerService.
  
  - Tests include:
  
    - Index_ReturnsViewWithCustomers – ensures correct model returned.
    
    - AddOrEdit_ExistingCustomer_ReturnsViewWithModel – ensures editing an existing customer loads correct data.
    
    - AddOrEdit_NonExistingCustomer_ReturnsNotFound.
    
    - AddOrEdit_Post_ValidModel_RedirectsToIndex – ensures it calls service and redirects.
    
    - Delete behavior: calls service and sets proper redirect.

## 8.3 InvoiceControllerTests

  - Mocks IInvoiceService.
  
  - Verifies:
  
      - Index_ReturnsView_WithCustomerInvoicesViewModel or similar – structure of the view model.
      
      - AddInvoice_AddsInvoiceAndRedirects – ensures _service.AddInvoice is called and redirect uses the right route values.
      
      - AddLineItem_AddsLineItemAndRedirects – ensures _service.AddLineItem is called once and the redirect is correct.

So we have unit tests for both controller logic and form/model validation.
