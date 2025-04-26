using hw2.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using System.Text;

namespace hw2.Services
{
    public class AuthTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly JsonWebTokenHandler _tokenHandler; // Use the newer handler

        public AuthTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            _tokenHandler = new JsonWebTokenHandler(); // Instantiate the handler
        }

        public string GenerateToken(User user)
        {
            // Retrieve JWT settings from configuration
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = jwtSettings["Key"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                // Handle missing configuration appropriately - throw exception or log error
                throw new InvalidOperationException("JWT settings (Key, Issuer, Audience) are not configured properly in appsettings.json.");
            }

            // IMPORTANT: Ensure the key is long enough for the algorithm (HS256 requires at least 128 bits / 16 bytes)
            if (Encoding.UTF8.GetBytes(secretKey).Length < 16)
            {
                 throw new InvalidOperationException("JWT Secret Key must be at least 16 characters long.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create claims for the token payload
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // Subject (usually user ID)
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique token identifier
                // Add other claims as needed (e.g., roles)
                // new Claim(ClaimTypes.Role, "Admin"),
            };

            // Define token expiration (e.g., 1 hour)
            var expires = DateTime.UtcNow.AddHours(1); // Make this configurable if needed

            // Create the token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = credentials
            };

            // CreateToken directly returns the string token with JsonWebTokenHandler
            string token = _tokenHandler.CreateToken(tokenDescriptor);

            // Return the serialized token string
            return token;
        }
    }
}
