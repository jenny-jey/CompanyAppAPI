using CompanyAPI.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyAPI.Application
{
    public interface ICompanyService 
    {
        Task<bool> AddCompanyAsync(CompanyDto company);
        Task<CompanyDto?> GetCompanyByIdAsync(int id);
        Task<CompanyDto?> GetCompanyByIsinAsync(string isin);
        Task<List<CompanyDto>> GetAllCompaniesAsync();
        Task<bool> UpdateCompanyAsync(int id, CompanyDto company);
    }
}
