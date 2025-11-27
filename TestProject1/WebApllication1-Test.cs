using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Moq;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Controllers;
using WebApplication1.Entities;
using WebApplication1.Models;
using WebApplication1.Services;
using Xunit;

namespace TestProject1

{
    public class AddorEditFormTest
    // Add or Edit Customer Form  test 
    {
        [Fact]
        public void Customer_WithAllRequiredFieldsIsValid()
        {
            // Arrange
            var customer = new Customer
            {
                Name = "Test Name",
                Address1 = "123 Test Street",
                City = "TestCity",
                ProvinceOrState = "ON",
                ZipOrPostalCode = "12345",
                Phone = "123-456-7890"
            };

            // Act & Assert
            ValidateModel(customer, true);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("ON", true)]
        [InlineData("Ontario", false)]
        public void ProvinceOrState_Validation(string value, bool expectedIsValid)
        {
            // Arrange
            var customer = new Customer
            {
                Name = "Test Name",
                Address1 = "123 Test Street",
                City = "TestCity",
                ProvinceOrState = value,
                ZipOrPostalCode = "12345",
                Phone = "123-456-7890"
            };

            // Act & Assert
            ValidateModel(customer, expectedIsValid);
        }


        private void ValidateModel(Customer customer, bool expectedIsValid)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(customer, null, null);
            var isValid = Validator.TryValidateObject(customer, validationContext, validationResults, true);

            Assert.Equal(expectedIsValid, isValid);
        }


    }
  


    public class CustomersControllerTests
    // Custormer Controller logic test
    {
        private readonly Mock<ICustomerService> _mockService;
        private readonly CustomersController _controller;

        public CustomersControllerTests()
        {
            _mockService = new Mock<ICustomerService>();
            _controller = new CustomersController(_mockService.Object);
        }

        // Testing Index Action
        [Fact]
        public void Index_ReturnsViewWithCustomers()
        {
            // Arrange
            var customers = new List<Customer> { /* Create some test customers */ };
            _mockService.Setup(s => s.GetAllCustomers()).Returns(customers);

            // Act
            var result = _controller.Index(null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<CustomerViewModel>>(viewResult.Model);
            Assert.Equal(customers.Count, model.Count());
        }

        // Testing AddOrEdit GET Action
        [Fact]
        public void AddOrEdit_ExistingCustomer_ReturnsViewWithModel()
        {
            // Arrange
            var customer = new Customer { CustomerId = 1, Name = "Test Customer" };
            _mockService.Setup(s => s.GetCustomerById(1)).Returns(customer);

            // Act
            var result = _controller.AddOrEdit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CustomerViewModel>(viewResult.Model);
            Assert.Equal(customer.CustomerId, model.CustomerId);
        }

        [Fact]
        public void AddOrEdit_NonExistingCustomer_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetCustomerById(1)).Returns((Customer)null);

            // Act
            var result = _controller.AddOrEdit(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // Testing AddOrEdit POST Action

        [Fact]
        public void AddOrEdit_ValidModel_CreatesCustomerAndRedirects()
        {
            // Arrange
            var model = new CustomerViewModel { /* Set properties */ };
            _mockService.Setup(s => s.AddCustomer(It.IsAny<Customer>()));

            // Act
            var result = _controller.AddOrEdit(null, model);

            // Assert
            _mockService.Verify(s => s.AddCustomer(It.IsAny<Customer>()), Times.Once());
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public void AddOrEdit_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var model = new CustomerViewModel();
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = _controller.AddOrEdit(null, model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<CustomerViewModel>(viewResult.Model);
        }

        // Testing UndoDelete Action
        [Fact]
        public void UndoDelete_ValidCustomer_RedirectsToIndex()
        {
            // Arrange
            var customer = new Customer { CustomerId = 1, IsDeleted = true };
            _mockService.Setup(s => s.GetCustomerById(1)).Returns(customer);
            _mockService.Setup(s => s.UpdateCustomer(It.IsAny<Customer>()));

            // Act
            var result = _controller.UndoDelete(1);

            // Assert
            Assert.False(customer.IsDeleted);
            _mockService.Verify(s => s.UpdateCustomer(customer), Times.Once());
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }


    }

    

    public class InvoiceControllerTests
    // Invoice Service logic test
    {
        private readonly Mock<IInvoiceService> _mockService;
        private readonly InvoiceController _controller;

        public InvoiceControllerTests()
        {
            _mockService = new Mock<IInvoiceService>();
            _controller = new InvoiceController(_mockService.Object);
        }

        // Test methods 

        // Testing AddInvoice POST Action
        [Fact]
        public void AddInvoice_ValidData_CreatesInvoiceAndRedirects()
        {
            // Arrange
            var customerId = 1;
            var invoiceDate = DateTime.Now;
            var paymentTermsId = 1;
            _mockService.Setup(s => s.AddInvoice(It.IsAny<Invoice>()));

            // Act
            var result = _controller.AddInvoice(customerId, invoiceDate, paymentTermsId);

            // Assert
            _mockService.Verify(s => s.AddInvoice(It.IsAny<Invoice>()), Times.Once());
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        // Testing AddLineItem POST Action
        [Fact]
        public void AddLineItem_ValidData_CreatesLineItemAndRedirects()
        {
            // Arrange
            var invoiceId = 1;
            var description = "Test Item";
            var amount = 100m;
            var customerId = 1;
            _mockService.Setup(s => s.AddLineItem(It.IsAny<InvoiceLineItem>()));

            // Act
            var result = _controller.AddLineItem(invoiceId, description, amount, customerId);

            // Assert
            _mockService.Verify(s => s.AddLineItem(It.IsAny<InvoiceLineItem>()), Times.Once());
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal(customerId, redirectToActionResult.RouteValues["customerId"]);
        }

        

    }
}






