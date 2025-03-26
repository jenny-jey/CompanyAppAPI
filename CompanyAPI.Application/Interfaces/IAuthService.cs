using CompanyAPI.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyAPI.Application
{
    public interface IAuthService
    {
        //string GenerateJwtToken(User user);
        //User? ValidateUser(string username, string password);
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerRequest);
    }
}
