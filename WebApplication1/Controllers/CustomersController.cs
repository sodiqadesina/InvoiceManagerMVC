using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Entities;
using System.Linq;

namespace WebApplication1.Controllers
{
    [Route("customers")]
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("")]
        public IActionResult Index(string alphaFilter = "A-E")
        {
            var customers = _customerService.GetAllCustomers();

            ViewBag.CurrentFilter = alphaFilter;

            if (!string.IsNullOrEmpty(alphaFilter))
			{
				if (alphaFilter == "A-E")
				{
					customers = customers.Where(c => c.Name[0] >= 'A' && c.Name[0] <= 'E');
				}
				else if (alphaFilter == "F-K")
				{
					customers = customers.Where(c => c.Name[0] >= 'F' && c.Name[0] <= 'K');
				}
				else if (alphaFilter == "L-R")
				{
					customers = customers.Where(c => c.Name[0] >= 'L' && c.Name[0] <= 'R');
				}
				else if (alphaFilter == "S-Z")
				{
					customers = customers.Where(c => c.Name[0] >= 'S' && c.Name[0] <= 'Z');
				}
				
			}


			var model = (customers.Select(c => new CustomerViewModel
            {
                CustomerId = c.CustomerId,
                Name = c.Name,
                Address1 = c.Address1,
                Address2 = c.Address2,
                City = c.City,
                ProvinceOrState = c.ProvinceOrState,
                ZipOrPostalCode = c.ZipOrPostalCode,
                Phone = c.Phone,
                ContactFirstName = c.ContactFirstName,
                ContactLastName = c.ContactLastName,
                ContactEmail = c.ContactEmail
            }));

            ViewBag.CurrentFilter = alphaFilter;

            return View(model);
		}

        [HttpGet("add-or-edit/{id?}")]
        public IActionResult AddOrEdit(int? id)
        {
            if (id.HasValue)
            {
                var customer = _customerService.GetCustomerById(id.Value);
                if (customer == null) return NotFound();

                var model = new CustomerViewModel
                {
                    CustomerId = customer.CustomerId,
                    Name = customer.Name,
                    Address1 = customer.Address1,
                    Address2 = customer.Address2,
                    City = customer.City,
                    ProvinceOrState = customer.ProvinceOrState,
                    ZipOrPostalCode = customer.ZipOrPostalCode,
                    Phone = customer.Phone,
                    ContactFirstName = customer.ContactFirstName,
                    ContactLastName = customer.ContactLastName,
                    ContactEmail = customer.ContactEmail
                };
                return View(model);
            }
            return View(new CustomerViewModel());
        }

        [HttpPost("add-or-edit/{id?}")]
        public IActionResult AddOrEdit(int? id, CustomerViewModel model)
        {
          
            if (ModelState.IsValid)
            {
                Customer customer = id.HasValue ?
                                    _customerService.GetCustomerById(id.Value) ?? new Customer() :
                                    new Customer();

                customer.Name = model.Name;
                customer.Address1 = model.Address1;
                customer.Address2 = model.Address2;
                customer.City = model.City;
                customer.ProvinceOrState = model.ProvinceOrState;
                customer.ZipOrPostalCode = model.ZipOrPostalCode;
                customer.Phone = model.Phone;
                customer.ContactFirstName = model.ContactFirstName;
                customer.ContactLastName = model.ContactLastName;
                customer.ContactEmail = model.ContactEmail;

                if (id.HasValue)
                {
                    _customerService.UpdateCustomer(customer);
                }
                else
                {
                    _customerService.AddCustomer(customer);
                }

                return RedirectToAction(nameof(Index));
            }

            // If we get here, something went wrong; re-displaying the  form.
            return View(model);
        }


        [HttpGet("delete/{id}")]
        public IActionResult Delete(int id, string name)
        {
            _customerService.DeleteCustomer(id);
            TempData["DeletedCustomerId"] = id; // Passing the ID 
            TempData["Message"] = "Customer \""+ name+ "\" was deleted successfully.";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet("undoDelete/{id}")]
        public IActionResult UndoDelete(int id)
        {
            var customer = _customerService.GetCustomerById(id);
            if (customer == null) return NotFound();

            customer.IsDeleted = false;
            _customerService.UpdateCustomer(customer);
            return RedirectToAction(nameof(Index));
        }

    }
}
