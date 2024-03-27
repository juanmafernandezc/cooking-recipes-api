using CommunityToolkit.Diagnostics;
using CookingRecipes.Api.Domain.DTOs;
using CookingRecipes.Api.Domain.Entities;
using CookingRecipes.Api.Domain.Interfaces;
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

        public UserService(DataContext context, ITokenService tokenService)
        {
            Guard.IsNotNull(context);
            Guard.IsNotNull(tokenService);

            _context = context;
            _tokenService = tokenService;
        }

        public async Task<bool> CreateUserAsync(UserRegisterDto userRegisterDto)
        {

            var userExists = await _context.Users.AnyAsync(u => u.Username == userRegisterDto.Username).ConfigureAwait(false);
            Guard.IsFalse(userExists);

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

            return result == 1;
        }
        public async Task<string> LoginAsync(UserLoginDto userLoginDto)
        {
            var user = await ValidateLoginAsync(userLoginDto).ConfigureAwait(false);
            Guard.IsNotNull(user);

            return _tokenService.CreateToken(user);
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
