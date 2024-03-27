using CommunityToolkit.Diagnostics;
using CookingRecipes.Api.Domain.DTOs;
using CookingRecipes.Api.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CookingRecipes.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUserService _userService;

        public AuthController(ILogger<AuthController> logger, IUserService userService)
        {
            Guard.IsNotNull(logger);
            Guard.IsNotNull(userService);

            _logger = logger;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            try
            {
                var result = await _userService.CreateUserAsync(userRegisterDto).ConfigureAwait(false);

                return result ? Ok(result) : BadRequest("Could not add a new user");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing Register: {ex}", ex);
                throw;
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            try
            {
                var result = await _userService.LoginAsync(userLoginDto).ConfigureAwait(false);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing Login: {ex}", ex);
                throw;
            }
        }
    }
}
