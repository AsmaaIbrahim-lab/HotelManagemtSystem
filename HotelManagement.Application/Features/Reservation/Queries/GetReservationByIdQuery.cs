using HotelManagement.Application.Features.Reservation.DTOs;
using HotelManagement.Application.Features.Room.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Application.Features.Reservation.Queries
{
    public record GetReservationByIdQuery(int Id)
    : IRequest<ReservationDto?>;

    public class GetReservationByIdQueryHandler
    : IRequestHandler<GetReservationByIdQuery, ReservationDto?>
    {
        private readonly IAppDbContext _context;

        public GetReservationByIdQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<ReservationDto?> Handle(
            GetReservationByIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Reservations
                .AsNoTracking()
                .Where(r => r.Id == request.Id)
                .Select(r => new ReservationDto
                {
                    Id = r.Id,
                    RoomNumber = r.Room.RoomNumber,
                    GuestName = r.GuestName,
                    CheckInDate = r.CheckInDate,
                    CheckOutDate = r.CheckOutDate,
                    TotalPrice = r.TotalPrice,
                    Status = r.Status
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }

}
