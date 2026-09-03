using HotelManagement.API.Extensions;
using HotelManagement.API.Hubs;
using HotelManagement.API.Services;
using HotelManagement.Application.Common;
using HotelManagement.Application.Domain.Entities;
using HotelManagement.Application.Features.Auth;
using HotelManagement.Application.Features.Auth.Commands.Register;
using HotelManagement.Application.Features.Auth.Interfaces;
using HotelManagement.Application.Features.Room.Commands;
using HotelManagement.Application.Features.Room.Interfaces;
using HotelManagement.Infrastructure.Persistence;
using HotelManagement.Infrastructure.Seed;
using HotelManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API
{
   
     

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Core Services & Controllers
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSignalR();
            builder.Services.AddScoped<ICurrentUser, CurrentUser>();
            builder.Services.AddScoped<IHotelHubContext, HotelHubContext>();
            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            builder.Services.AddMediatR(cfg =>
             cfg.RegisterServicesFromAssembly(
              typeof(RegisterCommand).Assembly));
           
            // 2. Database Context
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IAppDbContext>(provider =>
             provider.GetRequiredService<AppDbContext>());
            
            // 3. Identity Configuration
            builder.Services.AddIdentityServices();
            builder.Services.ConfigureIdentityOptions();

            // 4. Authentication & Authorization Setup
            builder.Services.AddAuthentication(builder.Configuration);
            builder.Services.AddAuthorization();

            // 5. OpenAPI
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // 6. Development Pipeline & Database Migration
            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var db = services.GetRequiredService<AppDbContext>();
                        await db.Database.MigrateAsync();

                        var userManager = services.GetRequiredService<UserManager<User>>();
                        await DbSeeder.SeedAsync(db, userManager);

                        Console.WriteLine("--> Database Seeded Successfully!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"--> ERROR DURING SEEDING: {ex.Message}");
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"--> INNER ERROR: {ex.InnerException.Message}");
                        }
                    }
                
            }
                }

                // OpenAPI & Swagger UI Setup
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel API v1");
                    c.RoutePrefix = "swagger"; // Serves Swagger UI at /swagger
                });
            

            // 7. Standard Middleware Pipeline (Strict Order Matters)
            app.UseHttpsRedirection();

            app.UseCors(ServiceCollectionExtensions.AngularLocalhostCorsPolicy);

            app.UseAuthentication();
            app.UseAuthorization();  
            app.MapControllers();
            app.MapHub<HotelHub>("/hubs/hotel");

            await app.RunAsync();
        }
    }
}
