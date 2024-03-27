using CookingRecipes.Api.Domain.DTOs;
using CookingRecipes.Api.Domain.Entities;

namespace CookingRecipes.Api.Domain.Interfaces
{
    public interface IUserService
    {
        Task<bool> CreateUserAsync(UserRegisterDto userRegisterDto);
        Task<string> LoginAsync(UserLoginDto userLoginDto);
    }
}
