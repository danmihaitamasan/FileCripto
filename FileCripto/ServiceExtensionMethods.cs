using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using BusinessLogic;
using DataAccess;
using System;
using System.Linq;
using System.Security.Claims;
using BusinessLogic.Services;

namespace FileCrypto
{
    public static class ServiceCollectionExtensionMethods
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services.AddScoped<ControllerDependencies>();

            return services;
        }

        public static IServiceCollection AddFileCryptoBusinessLogic(this IServiceCollection services)
        {
            services.AddScoped<ServiceDependencies>();
            services.AddScoped<UserAccountService>();
            services.AddScoped<FileManagerService>();
            services.AddScoped<EmailService>();
            return services;
        }

        public static IServiceCollection AddFileCryptoCurrentUser(this IServiceCollection services)
        {
            services.AddScoped(s =>
            {
                var accessor = s.GetService<IHttpContextAccessor>();
                var httpContext = accessor.HttpContext;
                var claims = httpContext.User.Claims;
                var userIdClaim = claims?.FirstOrDefault(c => c.Type == "Id")?.Value;
                var isGood = Guid.TryParse(userIdClaim, out var id);
                return new CurrentUserDto
                {
                    UserId = id,
                    IsAuthenticated = httpContext.User.Identity.IsAuthenticated,
                    Email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                    FirstName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                    UserName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                };
            });

            return services;
        }
    }
}