using System.Threading;
using System.Threading.Tasks;
using HotelManagement.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace HotelManagement.Application.Features.Room.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Domain.Entities.Room> Rooms { get; }
        DbSet<AuditLog> AuditLogs { get; }
        DatabaseFacade Database { get; }
        public DbSet<Reservation> Reservations { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
