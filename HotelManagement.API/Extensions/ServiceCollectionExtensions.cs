using HotelManagement.Application.Domain.Entities;
using HotelManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace HotelManagement.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public const string AngularLocalhostCorsPolicy = "AllowAngularLocalhost";

        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services.AddIdentityApiEndpoints<User>()
              .AddEntityFrameworkStores<AppDbContext>();
            return services;
        }
        public static IServiceCollection ConfigureIdentityOptions(this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
             
                options.User.RequireUniqueEmail = true;

            });
            return services;

        }
        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration config)
        {
           services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(y =>
            {
                y.SaveToken = true;
                y.RequireHttpsMetadata = true;
                y.TokenValidationParameters = new TokenValidationParameters
                {

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(config["JwtKey"]!)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = config["ValidIssuer"],
                    ValidAudience = config["ValidAudience"],
                    NameClaimType = ClaimTypes.NameIdentifier


                };
                y.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        // Route hub token automatically
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/hotel"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };

            });

            services.AddCors(options =>
            {
                options.AddPolicy(AngularLocalhostCorsPolicy, policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:4200",
                            "https://localhost:4200",
                            "http://127.0.0.1:4200",
                            "http://192.168.0.43:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            return services;

        }

    }

}
 
