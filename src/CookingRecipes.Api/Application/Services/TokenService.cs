using CookingRecipes.Api.Domain.Entities;
using CookingRecipes.Api.Domain.Interfaces;
using CookingRecipes.Api.Domain.Models.Responses;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CookingRecipes.Api.Application.Services
{
    public class TokenService : ITokenService
    {
        public ApiResponse<string> CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JwtKey")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = creds,
                Issuer = Environment.GetEnvironmentVariable("JwtIssuer"),
                Audience = Environment.GetEnvironmentVariable("JwtAudience")
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new ApiResponse<string>(tokenHandler.WriteToken(token));
        }
    }
}
