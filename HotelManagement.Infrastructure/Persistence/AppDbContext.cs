using HotelManagement.Application.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Room> Rooms ;
        public DbSet<Reservation> Reservations ;
        public DbSet<AuditLog> AuditLogs ;
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>(entity =>
            {
                entity.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(100);

                entity.Property(u => u.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(u => u.CreatedAt)
                    .IsRequired();

            });
            // User -> Reservation (One-To-Many)
            builder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting user from wiping historical records

            // Room -> Reservation (One-To-Many)
            builder.Entity<Reservation>()
                .HasOne(r => r.Room)
                .WithMany(rm => rm.Reservations)
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // User -> AuditLog (One-To-Many, Optional)
            builder.Entity<AuditLog>()
                .HasOne(a => a.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Decimal Precision Setup
            builder.Entity<Room>()
                .Property(r => r.PricePerNight)
                .HasPrecision(18, 2);

            builder.Entity<Room>()
                .HasIndex(r => r.RoomNumber)
                .IsUnique();

            builder.Entity<Reservation>()
                .Property(r => r.TotalPrice)
                .HasPrecision(18, 2);

        }
    }
}
