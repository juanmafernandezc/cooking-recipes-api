using CommunityToolkit.Diagnostics;
using CookingRecipes.Api.Application.Services;
using CookingRecipes.Api.Domain.Interfaces;
using CookingRecipes.Api.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CookingRecipes.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddJwtAuthentication(configuration);
            services.AddDbContext(configuration);
            services.AddServices();

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IIngredientService, IngredientService>();
            services.AddScoped<IRecipeService, RecipeService>();

            return services;
        }

        private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtIssuer = Environment.GetEnvironmentVariable("JwtIssuer");
            var jwtAudience = Environment.GetEnvironmentVariable("JwtAudience");
            var jwtKey = Environment.GetEnvironmentVariable("JwtKey");

            Guard.IsNotNullOrEmpty(jwtIssuer);
            Guard.IsNotNullOrEmpty(jwtAudience);
            Guard.IsNotNullOrEmpty(jwtKey);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                };
            });

            return services;
        }

        private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var databaseConnection = Environment.GetEnvironmentVariable("DatabaseConnectionString");

            Guard.IsNotNullOrEmpty(databaseConnection);

            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(databaseConnection));

            return services;
        }
    }
}
