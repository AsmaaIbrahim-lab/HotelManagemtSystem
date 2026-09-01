using HotelManagement.API.Extensions;
using HotelManagement.Application.Features.Auth;
using HotelManagement.Application.Features.Auth.Commands.Register;
using HotelManagement.Application.Features.Auth.Interfaces;
using HotelManagement.Infrastructure.Persistence;
using HotelManagement.Infrastructure.Services;
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
            builder.Services.AddScoped<ICurrentUser, CurrentUser>();
            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            builder.Services.AddMediatR(cfg =>
              cfg.RegisterServicesFromAssembly(
              typeof(RegisterCommand).Assembly));
            // 2. Database Context
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

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
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    await db.Database.MigrateAsync();

                    // TODO: Add database seed logic here (Demo user, rooms, reservations)
                }

                // OpenAPI & Swagger UI Setup
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel API v1");
                    c.RoutePrefix = "swagger"; // Serves Swagger UI at /swagger
                });
            }

            // 7. Standard Middleware Pipeline (Strict Order Matters)
            app.UseHttpsRedirection();

            app.UseCors(ServiceCollectionExtensions.AngularLocalhostCorsPolicy);

            app.UseAuthentication(); // 1. Identify WHO the user is
            app.UseAuthorization();  // 2. Determine WHAT the user can access

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
