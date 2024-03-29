using CookingRecipes.Api.Domain.DTOs;
using CookingRecipes.Api.Domain.Models.Responses;

namespace CookingRecipes.Api.Domain.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<string?>> CreateUserAsync(UserRegisterDto userRegisterDto);
        Task<ApiResponse<string?>> LoginAsync(UserLoginDto userLoginDto);
    }
}
