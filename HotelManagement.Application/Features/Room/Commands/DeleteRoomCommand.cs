using HotelManagement.Application.Common;
using HotelManagement.Application.Domain.Entities;
using HotelManagement.Application.Features.Auth.Interfaces;
using HotelManagement.Application.Features.Room.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HotelManagement.Application.Features.Room.Commands
{
    public record DeleteRoomCommand(int Id) : IRequest<string>;

     internal class DeleteRoomCommandHandler
        :IRequestHandler<DeleteRoomCommand,string>
    {
        private readonly IAppDbContext _context;            
        private readonly ICurrentUser _currentUserService;
        private readonly IHotelHubContext _hubContext;


        public DeleteRoomCommandHandler(IAppDbContext context, ICurrentUser currentUserService, IHotelHubContext hubContext)
        {
            _context = context;
            _currentUserService = currentUserService;
            _hubContext = hubContext;
        }

      public async Task<string> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)

        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(
                    r => r.Id == request.Id,
                    cancellationToken);

            if (room == null)
            {
                throw new KeyNotFoundException("Room not found.");
            }

            var hasFutureReservation = await _context.Reservations
                .AnyAsync(
                    r => r.RoomId == request.Id
                        && r.Status == "Confirmed"
                        && r.CheckInDate >= DateOnly.FromDateTime(DateTime.UtcNow),
                    cancellationToken);

            if (hasFutureReservation)
            {
                throw new InvalidOperationException("Cannot delete room with future confirmed reservations.");
            }

            bool hasAnyHistory = room.Reservations.Any();

            if (hasAnyHistory)
            {
                room.IsDeleted = true;
                _context.Rooms.Update(room);
            }
            else
            {
                _context.Rooms.Remove(room);

            }

            var auditLog = new AuditLog
            {
                Action = "Delete",
                EntityName = nameof(Room),
                EntityId = room.Id.ToString(),
                UserId = _currentUserService.UserId,
                ActionDate = DateTime.UtcNow,
                Details = hasAnyHistory ? "Soft deleted room due to historical reservations" : "Hard deleted room"
            };

            _context.AuditLogs.Add(auditLog);

            await _context.SaveChangesAsync(cancellationToken);

            await _hubContext.SendRoomDeletedAsync(
                new
                {
                    room.RoomNumber
                },
                 cancellationToken
                 );

            return "Deletion Completed Successfully";

        }

       
    }
}
