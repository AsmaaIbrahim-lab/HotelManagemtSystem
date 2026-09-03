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
    : IRequestHandler<CancelReservationCommand,string>
    {
        private readonly IAppDbContext _context;

        public CancelReservationCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(
            CancelReservationCommand request,
            CancellationToken cancellationToken)
        {
            var reservation = await _context.Reservations
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
            return "Cancel Done Successfuly";
        }
    }
}
