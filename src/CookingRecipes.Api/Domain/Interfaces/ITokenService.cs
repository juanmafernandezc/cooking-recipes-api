using CookingRecipes.Api.Domain.Entities;

namespace CookingRecipes.Api.Domain.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
