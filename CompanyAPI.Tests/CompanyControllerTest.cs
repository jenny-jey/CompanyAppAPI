using CompanyAPI.API.Controllers;
using CompanyAPI.Application;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Serilog;

namespace CompanyAPI.Tests
{
    public class CompanyControllerTest
    {
        private Mock<ICompanyService> _mockCompanyService;
        private Mock<ILogger> _mockLogger;
        private CompanyController _controller;

        [SetUp]
        public void Setup()
        {
            _mockCompanyService = new Mock<ICompanyService>();
            _mockLogger = new Mock<ILogger>();
            _controller = new CompanyController(_mockCompanyService.Object, _mockLogger.Object);

        }

        [Test]
        public async Task GetAllCompaniesAsync_ReturnsOkResult_WithListOfCompanies()
        {
            // Arrange
            var companies = new List<CompanyDto>
            {
                new CompanyDto { Id = 1, Name = "Company A", Isin = "US1234567890", Exchange = "NASDAQ", Ticker = "COMPA"},
                new CompanyDto { Id = 2, Name = "Company B", Isin = "US0987654321", Exchange = "NASDAQ", Ticker = "COMPB", Website ="http://www.compa.com"}
            };
            _mockCompanyService.Setup(s => s.GetAllCompaniesAsync()).ReturnsAsync(companies);

            // Act
            var result = await _controller.GetAllCompaniesAsync();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            //Assert.AreEqual(200, okResult.StatusCode);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.IsInstanceOf<List<CompanyDto>>(okResult.Value);
            Assert.That(((List<CompanyDto>)okResult.Value).Count, Is.EqualTo(2));
            //Assert.AreEqual(2, ((List<CompanyDto>)okResult.Value).Count);
        }

        [Test]
        public async Task GetCompanyById_ReturnsOkResult_WithCompany()
        {
            // Arrange
            var company = new CompanyDto { Id = 1, Name = "Company A", Isin = "US1234567890", Exchange = "NASDAQ", Ticker = "COMPA" };
            _mockCompanyService.Setup(s => s.GetCompanyByIdAsync(1)).ReturnsAsync(company);

            // Act
            var result = await _controller.GetCompanyById(1);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.IsInstanceOf<CompanyDto>(okResult.Value);
            //Assert.AreEqual(company.Id, ((CompanyDto)okResult.Value).Id);
            Assert.That(((CompanyDto)okResult.Value).Id, Is.EqualTo(company.Id));
        }

        [Test]
        public async Task GetCompanyById_ReturnsBadRequest_OnException()
        {
            // Arrange
            _mockCompanyService.Setup(s => s.GetCompanyByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetCompanyById(1);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task GetCompanyByISIN_ReturnsOkResult_WithCompany()
        {
            // Arrange
            var company = new CompanyDto { Id = 1, Name = "Company A", Isin = "US1234567890", Exchange = "NASDAQ", Ticker = "COMPA" };
            _mockCompanyService.Setup(s => s.GetCompanyByIsinAsync("US1234567890")).ReturnsAsync(company);

            // Act
            var result = await _controller.GetCompanyByISIN("US1234567890");

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.IsInstanceOf<CompanyDto>(okResult.Value);
        }

        [Test]
        public async Task UpdateCompany_ReturnsOkResult_WhenUpdateIsSuccessful()
        {
            // Arrange
            var companyDto = new CompanyDto { Id = 1, Name = "Updated Company", Isin = "US1234567890", Exchange = "NASDAQ", Ticker = "COMPA" };
            _mockCompanyService.Setup(s => s.UpdateCompanyAsync(1, companyDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateCompany(1, companyDto);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public async Task UpdateCompany_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            var companyDto = new CompanyDto { Id = 2, Name = "Updated Company", Isin = "US1234567890", Exchange = "NASDAQ", Ticker = "COMPA" };

            // Act
            var result = await _controller.UpdateCompany(1, companyDto);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task CreateCompany_ReturnsOkResult_WhenCompanyCreated()
        {
            // Arrange
            var companyDto = new CompanyDto { Id = 1, Name = "New Company", Isin = "US1234567890" , Exchange = "NASDAQ", Ticker = "COMPA" };
            _mockCompanyService.Setup(s => s.AddCompanyAsync(companyDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateCompany(companyDto);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public async Task CreateCompany_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var companyDto = new CompanyDto { Id = 1, Name = "New Company", Isin = "", Exchange = "NASDAQ", Ticker = "COMPA" };

            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("ISIN", "ISIN format is invalid.")
            };

            var validationException = new ValidationException(validationFailures);

            _mockCompanyService
                 .Setup(s => s.AddCompanyAsync(It.IsAny<CompanyDto>()))
                 .ThrowsAsync(validationException);

            // Act
            var result = await _controller.CreateCompany(new CompanyDto());

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.InstanceOf<object>());


        }
    }
}