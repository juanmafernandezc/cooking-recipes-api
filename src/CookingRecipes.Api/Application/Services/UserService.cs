using CommunityToolkit.Diagnostics;
using CookingRecipes.Api.Common;
using CookingRecipes.Api.Domain.DTOs;
using CookingRecipes.Api.Domain.Entities;
using CookingRecipes.Api.Domain.Interfaces;
using CookingRecipes.Api.Domain.Models.Responses;
using CookingRecipes.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CookingRecipes.Api.Application.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        private const string UserExistsMessage = "User already exists.";
        private const string UserCreatedMessage = "User created successfully.";
        private const string UserNotRegisteredOrIncorrectPasswordMessage = "User not registered or password is incorrect.";
        private const string NotChangedDatabaseMessage = "No changes were made to the database.";

        public UserService(DataContext context, ITokenService tokenService)
        {
            Guard.IsNotNull(context);
            Guard.IsNotNull(tokenService);

            _context = context;
            _tokenService = tokenService;
        }

        public async Task<ApiResponse<string?>> CreateUserAsync(UserRegisterDto userRegisterDto)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Username == userRegisterDto.Username).ConfigureAwait(false);
            if (userExists) return new ApiResponse<string?>(null, false, UserExistsMessage, HttpStatusCodes.Conflict);

            var hmac = new HMACSHA512();
            var user = new User
            {
                Username = userRegisterDto.Username,
                Email = userRegisterDto.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userRegisterDto.Password)),
                PasswordSalt = hmac.Key,
            };

            _context.Users.Add(user);
            var result = await _context.SaveChangesAsync().ConfigureAwait(false);

            if (result != 0) return new ApiResponse<string?>(UserCreatedMessage);

            return new ApiResponse<string?>(null, false, NotChangedDatabaseMessage, HttpStatusCodes.InternalServerError);
        }
        public async Task<ApiResponse<string?>> LoginAsync(UserLoginDto userLoginDto)
        {
            var user = await ValidateLoginAsync(userLoginDto).ConfigureAwait(false);

            if (user == null) return new ApiResponse<string?>(null, false, UserNotRegisteredOrIncorrectPasswordMessage, HttpStatusCodes.Unauthorized);

            return _tokenService.CreateToken(user)!;
        }

        private async Task<User?> ValidateLoginAsync(UserLoginDto userLoginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userLoginDto.Username).ConfigureAwait(false);

            if (user == null) return null;

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userLoginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return null;
            }

            return user;
        }
    }
}
