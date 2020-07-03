using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SocialMedia.Data;
using SocialMedia.Models;

namespace SocialMedia.Web.Infrastructure
{
    public static class ServiceCollectionExtensions 
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services
                .AddIdentity<User, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredUniqueChars = 6;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<SocialMediaDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
