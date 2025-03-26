using CompanyAPI.Domain;
using Microsoft.EntityFrameworkCore;
using Serilog;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CompanyAPI.Infrastructure
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public CompanyRepository(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> AddAsync(Company company)
        {
            bool result = false;
            try
            {
                _logger.Information($"Adding {company.Name} - {company.Isin} to the database");
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
                result = true;
                _logger.Information($"{company.Name} - {company.Isin} added successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error while saving company {company.Name} - {company.Isin} to database.");
                throw;
            }
            return result;
        }

        public async Task<List<Company>> GetAllAsync()
        {
            _logger.Information($"Retrieving companies");
            List<Company> result = new List<Company>();

            try
            {
                result = await _context.Companies.ToListAsync();
                _logger.Information($"Companies retrieved successfully");
            }
            catch (Exception ex )
            {
                _logger.Error(ex, $"Error retrieving companies");
                throw;
            }
            return result;
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            _logger.Information($"Retrieving companies");
            Company? company = new Company();

            try
            {
                company = await _context.Companies.FindAsync(id);

                if (company != null)
                {
                    _logger.Information($"Company {company.Name} - {company.Isin} retrieved successfully.");
                }
                else
                {
                    _logger.Information($"Company with Id {id} not found.");
                }
               
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error retrieving companies");
                throw;
            }
            return company;
        }

        public async Task<Company?> GetByIsinAsync(string isin)
        {
            _logger.Information($"Retrieving companies");
            Company? company = new Company();

            try
            {
                company = await _context.Companies.FirstOrDefaultAsync(c => c.Isin == isin);

                if (company != null)
                {
                    _logger.Information($"Company {company.Name} - {company.Isin} retrieved successfully.");
                }
                else
                {
                    _logger.Information($"Company with ISIN {isin} not found.");
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error retrieving companies");
                throw;
            }
            return company;
        }

        public async Task<bool> UpdateAsync(Company company)
        {
            bool result = false;
            try
            {
                _logger.Information($"Updating {company.Name} - {company.Isin} to the database");
                _context.Companies.Update(company);
                await _context.SaveChangesAsync();
                result = true;
                _logger.Information($"{company.Name} - {company.Isin} updated successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error while updating the company {company.Name} - {company.Isin}.");
                throw;
            }
            return result;
        }
    }
}
