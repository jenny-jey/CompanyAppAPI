using CompanyAPI.Domain;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CompanyAPI.Application
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        //private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            //_passwordHasher = passwordHasher;
        }


        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            // Retrieve user from the database
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            {
                return null; // Invalid username or password
            }

            var token = GenerateJwtToken(user);
            return new LoginResponseDto { Token = token };
        }

        private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            //var result = _passwordHasher.VerifyHashedPassword(user, storedPasswordHash, inputPassword);
            //return result == PasswordVerificationResult.Success;
            var parts = storedPasswordHash.Split(':');
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            string hashToCompare = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32
            ));

            return parts[1] == hashToCompare;
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
        {
            // Check if the user already exists
            var existingUser = await _userRepository.GetByUsernameAsync(registerRequest.Username);
            if (existingUser != null)
            {
                return new RegisterResponseDto { Message = "User already exists" };
            }

            // Hash the password
            var hashedPassword = HashPassword(registerRequest.Password);

            var user = new User
            {
                Username = registerRequest.Username,
                PasswordHash = hashedPassword,
                Role = "User"
            };

            await _userRepository.AddAsync(user);

            return new RegisterResponseDto { Message = "Registration successful" };
        }

        private string HashPassword(string password)
        {
            // Use PBKDF2 to hash the password
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return Convert.ToBase64String(salt) + ":" + hash;
        }
    }
}
