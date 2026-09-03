using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotelManagement.Application.Domain.Entities;
using HotelManagement.Application.Features.Auth.Interfaces;
using HotelManagement.Application.Features.Room.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Room.Commands
{
    public record UpdateRoomCommand(
        int Id,
        string RoomNumber,
        string RoomType,
        decimal PricePerNight
    ) : IRequest<string>;

    public class UpdateRoomCommandHandler
        : IRequestHandler<UpdateRoomCommand,string>
    {
        private readonly IAppDbContext _context;
        private readonly ICurrentUser _currentUserService;

        public UpdateRoomCommandHandler(IAppDbContext context, ICurrentUser currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<string> Handle(
            UpdateRoomCommand request,
            CancellationToken cancellationToken)
        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(
                    r => r.Id == request.Id,
                    cancellationToken);

            if (room is null)
            {
                throw new KeyNotFoundException("Room not found.");
            }

            if (request.PricePerNight <= 0)
            {
                throw new ArgumentException("Price per night must be greater than 0.");
            }

            var duplicateRoomNumber = await _context.Rooms
                .AnyAsync(
                    r => r.RoomNumber == request.RoomNumber && r.Id != request.Id,
                    cancellationToken);

            if (duplicateRoomNumber)
            {
                throw new InvalidOperationException($"Room number '{request.RoomNumber}' is already in use.");
            }

            room.RoomNumber = request.RoomNumber;
            room.Type = request.RoomType;
            room.PricePerNight = request.PricePerNight;

            var auditLog = new AuditLog
            {
                Action = "Update",
                EntityName = nameof(Domain.Entities.Room),
                EntityId = room.Id.ToString(),
                UserId = _currentUserService.UserId ?? "System",
                ActionDate = DateTime.UtcNow,
                Details = $"Updated RoomNumber to {request.RoomNumber}, Type to {request.RoomType}, Price to {request.PricePerNight:C}"
            };

            _context.AuditLogs.Add(auditLog);

            await _context.SaveChangesAsync(cancellationToken);

            return "Updating Room Completed successfully";
        }
    }
}
