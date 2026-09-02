using System;
using System.Threading;
using System.Threading.Tasks;
using HotelManagement.Application.Features.Auth.Interfaces;
using HotelManagement.Application.Domain.Entities;
using HotelManagement.Application.Features.Room.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Room.Commands
{
    public record CreateRoomCommand(
        string RoomNumber,
        string RoomType,
        decimal PricePerNight) : IRequest<int>;

    public class CreateRoomCommandHandler
        : IRequestHandler<CreateRoomCommand, int>
    {
        private readonly IAppDbContext _context;
        private readonly ICurrentUser _currentUserService;

        public CreateRoomCommandHandler(IAppDbContext context, ICurrentUser currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(
            CreateRoomCommand request,
            CancellationToken cancellationToken)
        {
            if (request.PricePerNight <= 0)
            {
                throw new ArgumentException("Price per night must be greater than 0.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var roomNumberExists = await _context.Rooms
                    .AnyAsync(r => r.RoomNumber == request.RoomNumber, cancellationToken);

                if (roomNumberExists)
                {
                    throw new InvalidOperationException("Room number already exists.");
                }

                var now = DateTime.UtcNow;

                var room = new Domain.Entities.Room
                {
                    RoomNumber = request.RoomNumber,
                    Type = request.RoomType,
                    PricePerNight = request.PricePerNight,
                    IsAvailable = true,
                    CreatedAt = now
                };

                _context.Rooms.Add(room);

                await _context.SaveChangesAsync(cancellationToken);

                var auditLog = new AuditLog
                {
                    Action = "Create",
                    EntityName = nameof(Domain.Entities.Room),
                    EntityId = room.Id.ToString(),
                    UserId = _currentUserService.UserId ?? "System",
                    ActionDate = now,
                    Details = $"Created room #{room.RoomNumber} ({room.Type}) at {room.PricePerNight:C}/night"
                };

                _context.AuditLogs.Add(auditLog);

                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return room.Id;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
