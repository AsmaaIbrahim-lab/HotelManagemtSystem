using HotelManagement.Application.Common;
using HotelManagement.Application.Domain.Entities;
using HotelManagement.Application.Features.Auth.Interfaces;
using HotelManagement.Application.Features.Room.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

using System.Text;

namespace HotelManagement.Application.Features.Reservation.Commands
{
    public record CancelReservationCommand(int Id) : IRequest<string>;

    public class CancelReservationCommandHandler
    : IRequestHandler<CancelReservationCommand, string>
    {
        private readonly IAppDbContext _context;
        private readonly IHotelHubContext _hubContext;
        private readonly ICurrentUser _currentUserService;



        public CancelReservationCommandHandler(IAppDbContext context, IHotelHubContext hubContext, ICurrentUser currentUserService)
        {
            _context = context;
            _hubContext = hubContext;
              _currentUserService= currentUserService;
        }

        public async Task<string> Handle(
            CancelReservationCommand request,
            CancellationToken cancellationToken)
        {
            var reservation = await _context.Reservations.Include(r=>r.Room)
                .FirstOrDefaultAsync(
                    r => r.Id == request.Id,
                    cancellationToken);

            if (reservation is null)
                throw new KeyNotFoundException("Reservation not found.");

            if (reservation.Status == "Cancelled")
                throw new InvalidOperationException(
                    "Reservation is already cancelled.");

            reservation.Status = "Cancelled";

            await _context.SaveChangesAsync(cancellationToken);


            var auditLog = new AuditLog
            {
                Action = "Cancel",
                EntityName = nameof(Domain.Entities.Reservation),
                EntityId = reservation.Id.ToString(),
                UserId = _currentUserService.UserId ?? "System",
                ActionDate = DateTime.UtcNow,
                Details =
                        $"Reservation Cancelled for room {reservation.Room.RoomNumber}, " +
                        $"guest {reservation.GuestName}, " +
                        $"from {reservation.CheckInDate} " +
                        $"to {reservation.CheckOutDate}."
            };

            _context.AuditLogs.Add(auditLog);

            await _context.SaveChangesAsync(cancellationToken);


            await _hubContext.SendReservationCancelledAsync(
    new
    {
        reservation.Id,
        reservation.Room.RoomNumber,
        reservation.Status
    },
    cancellationToken
);
            return "Cancel Done Successfuly";
        }
    }
}
