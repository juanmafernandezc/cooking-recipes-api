using CookingRecipes.Api.Domain.Entities;
using CookingRecipes.Api.Domain.Models.Responses;

namespace CookingRecipes.Api.Domain.Interfaces
{
    public interface ITokenService
    {
        ApiResponse<string> CreateToken(User user);
        int? GetUserIdFromToken(string token);
    }
}
