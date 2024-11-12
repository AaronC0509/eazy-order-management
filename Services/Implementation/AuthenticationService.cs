using CustomerPortal.Data;
using CustomerPortal.Models.Entities;
using CustomerPortal.Models.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CustomerPortal.Services.Implementation
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<Customer> _passwordHasher;

        public AuthenticationService(
            ApplicationDbContext context,
            IConfiguration configuration,
            IPasswordHasher<Customer> passwordHasher)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        public async Task<TokenResponse> LoginAsync(string email, string password)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);

            if (customer == null)
                throw new InvalidOperationException("Invalid email or password");

            var verificationResult = _passwordHasher.VerifyHashedPassword(
                customer,
                customer.PasswordHash,
                password);

            if (verificationResult == PasswordVerificationResult.Failed)
                throw new InvalidOperationException("Invalid email or password");

            var token = GenerateJwtToken(customer);

            return new TokenResponse
            {
                Token = token,
                ExpiresIn = 3600
            };
        }

        private string GenerateJwtToken(Customer customer)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new Claim(ClaimTypes.Email, customer.Email),
            new Claim(ClaimTypes.Name, $"{customer.FirstName} {customer.LastName}"),
            new Claim(ClaimTypes.Role, customer.Role)
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
