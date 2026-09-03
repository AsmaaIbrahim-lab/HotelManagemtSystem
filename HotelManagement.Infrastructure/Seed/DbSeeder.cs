using HotelManagement.Application.Domain.Entities;
using HotelManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Infrastructure.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context, UserManager<User> userManager)
        {
           


            var user = new User
            {
                UserName = "admin@hotel.com",
                Email = "admin@hotel.com",
                FullName = "Admin User",
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, "Admin@_123");
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to seed demo user: {errors}");
            }

            var rooms = new List<Room>
            {
                new Room { RoomNumber = "101", Type = "Single", PricePerNight = 99, IsAvailable = true },
                new Room { RoomNumber = "102", Type = "Double", PricePerNight = 149, IsAvailable = true },
                new Room { RoomNumber = "103", Type = "Double", PricePerNight = 159, IsAvailable = false },
                new Room { RoomNumber = "104", Type = "Suite", PricePerNight = 249, IsAvailable = true },
                new Room { RoomNumber = "105", Type = "Suite", PricePerNight = 299, IsAvailable = false }
            };

            context.Rooms.AddRange(rooms);
            await context.SaveChangesAsync();

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var reservations = new List<Reservation>
            {
                new Reservation
                {
                    CheckInDate = today.AddDays(-5),
                    CheckOutDate = today.AddDays(2),
                    TotalPrice = 1043,
                    GuestName = "John Smith",
                    Status = "Confirmed",
                    CreatedBy = user.Id,
                    RoomId = rooms[2].Id
                },
                new Reservation
                {
                    CheckInDate = today.AddDays(-10),
                    CheckOutDate = today.AddDays(-7),
                    TotalPrice = 447,
                    GuestName = "Jane Doe",
                    Status = "Confirmed",
                    CreatedBy = user.Id,
                    RoomId = rooms[4].Id
                },
                new Reservation
                {
                    CheckInDate = today.AddDays(3),
                    CheckOutDate = today.AddDays(7),
                    TotalPrice = 996,
                    GuestName = "Bob Johnson",
                    Status = "Confirmed",
                    CreatedBy = user.Id,
                    RoomId = rooms[1].Id
                },
                new Reservation
                {
                    CheckInDate = today.AddDays(-20),
                    CheckOutDate = today.AddDays(-15),
                    TotalPrice = 495,
                    GuestName = "Alice Williams",
                    Status = "Cancelled",
                    CreatedBy = user.Id,
                    RoomId = rooms[0].Id
                }
            };

            context.Reservations.AddRange(reservations);
            await context.SaveChangesAsync();
        }
    }
}
