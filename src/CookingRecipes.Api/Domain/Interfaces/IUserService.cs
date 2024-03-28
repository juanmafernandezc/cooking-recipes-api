using CookingRecipes.Api.Domain.DTOs;

namespace CookingRecipes.Api.Domain.Interfaces
{
    public interface IUserService
    {
        Task<bool> CreateUserAsync(UserRegisterDto userRegisterDto);
        Task<string> LoginAsync(UserLoginDto userLoginDto);
    }
}
