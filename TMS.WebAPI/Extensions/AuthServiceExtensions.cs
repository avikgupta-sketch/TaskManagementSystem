using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TMS.ServiceLogic.Implementations;
using TMS.ServiceLogic.Interface;
using TMS.ServiceLogic.Mappings;

namespace TMS.WebAPI.Extensions
{
    public static class AuthServiceExtensions
    {
        public static IServiceCollection AddAuthServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register AuthService
            services.AddScoped<IAuthService, AuthService>();
            // Register AutoMapper with Auth profile
            services.AddAutoMapper(typeof(AuthMappingProfile));  

            
            

            // Configure JWT
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["JwtSettings:Issuer"],
                        ValidAudience = configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!))
                    };
                });

            return services;
        }
    }
}

