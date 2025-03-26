using CompanyAPI.Domain;
using FluentValidation;
using Serilog;

namespace CompanyAPI.Application
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IValidator<Company> _validator;
        private readonly ILogger _logger;

        public CompanyService(ICompanyRepository companyRepository, IValidator<Company> validator, ILogger logger)
        {
            _companyRepository = companyRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<bool> AddCompanyAsync(CompanyDto companydto)
        {
            _logger.Information("Adding company");
            bool result = false;
            try
            {
                var company = new Company
                {
                    Isin = companydto.Isin,
                    Exchange = companydto.Exchange,
                    Name = companydto.Name,
                    Ticker = companydto.Ticker,
                    Website = companydto.Website
                };
                result = await _companyRepository.AddAsync(company);
                result = true;
                _logger.Information($"Company {company.Name} - {company.Isin} added successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while creating companies");
                throw;
            }
            return result;
        }

        public async Task<List<CompanyDto>> GetAllCompaniesAsync()
        {
            _logger.Information("Retrieving company data");
            try
            {
                var companies = await _companyRepository.GetAllAsync();

                if (companies == null || !companies.Any())
                {
                    return new List<CompanyDto>();
                }

                return companies.Select(c => new CompanyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Exchange = c.Exchange,
                    Isin = c.Isin,
                    Ticker = c.Ticker,
                    Website = c.Website
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while retrieving companies");
                throw;
            }
        }

        public async Task<CompanyDto?> GetCompanyByIdAsync(int id)
        {
            _logger.Information("Retrieving company data");
            try
            {
                var company = await _companyRepository.GetByIdAsync(id);

                return company == null ? null : new CompanyDto
                {
                    Id = company.Id,
                    Name = company.Name,
                    Exchange = company.Exchange,
                    Isin = company.Isin,
                    Ticker = company.Ticker,
                    Website = company.Website
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while retrieving companies");
                throw;
            }
        }

        public async Task<CompanyDto?> GetCompanyByIsinAsync(string isin)
        {
            _logger.Information($"Retrieving company data for the ISIN {isin}");
            try
            {
                var company = await _companyRepository.GetByIsinAsync(isin);

                return company == null ? null : new CompanyDto
                {
                    Id = company.Id,
                    Name = company.Name,
                    Exchange = company.Exchange,
                    Isin = company.Isin,
                    Ticker = company.Ticker,
                    Website = company.Website
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while retrieving companies");
                throw;
            }
        }

        public async Task<bool> UpdateCompanyAsync(int id, CompanyDto companydto)
        {
            _logger.Information("Updating company");
            bool result = false;
            try
            {
                var company = new Company
                {
                    Isin = companydto.Isin,
                    Exchange = companydto.Exchange,
                    Name = companydto.Name,
                    Ticker = companydto.Ticker,
                    Website = companydto.Website
                };
                result = await _companyRepository.UpdateAsync(company);
                result = true;
                _logger.Information($"Company {company.Name} - {company.Isin} updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while updating companies");
                throw;
            }
            return result;
        }
    }
}
