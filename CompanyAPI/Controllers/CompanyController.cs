using CompanyAPI.Application;
using CompanyAPI.Domain;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CompanyAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly Serilog.ILogger _logger;

        public CompanyController(ICompanyService companyService, Serilog.ILogger logger)
        {
            _companyService = companyService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCompaniesAsync()
        {
            try
            {
                var companies = await _companyService.GetAllCompaniesAsync();

                if (companies == null || !companies.Any())
                {
                    _logger.Warning("No companies found.");
                }
                return Ok(companies);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while retrieving all companies.");
                return BadRequest();
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCompanyById(int id)
        {
            try
            {
                var company = await _companyService.GetCompanyByIdAsync(id);
                if (company == null)
                {
                    _logger.Warning("No companies found.");
                }
                return Ok(company);
            }
            catch(Exception ex) 
            {
                _logger.Error(ex, "An error occurred while retrieving company.");
                return BadRequest();
            }
            
        }

        [HttpGet("{isin}")]
        public async Task<IActionResult> GetCompanyByISIN(string isin)
        {
            try
            {
                var company = await _companyService.GetCompanyByIsinAsync(isin);
                if (company == null)
                {
                    _logger.Error("No companies found.");
                }
                return Ok(company);
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "An error occurred while retrieving company.");
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody]CompanyDto companydto)
        {
            try
            {
                if (id != companydto.Id)
                {
                    return BadRequest();
                }
                await _companyService.UpdateCompanyAsync(id, companydto);
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "An error occurred while retrieving company.");
                return BadRequest();
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody]CompanyDto companydto)
        {
            try
            {
                var companyId = await _companyService.AddCompanyAsync(companydto);
                return Ok();
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while retrieving company.");
                return BadRequest();
            }
        }
    }
}
