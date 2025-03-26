using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyAPI.Domain
{
    public interface ICompanyRepository
    {
        Task<bool> AddAsync(Company company);
        Task<Company?> GetByIdAsync(int id);
        Task<Company?> GetByIsinAsync(string isin);
        Task<List<Company>> GetAllAsync();
        Task<bool> UpdateAsync(Company company);
    }
}
