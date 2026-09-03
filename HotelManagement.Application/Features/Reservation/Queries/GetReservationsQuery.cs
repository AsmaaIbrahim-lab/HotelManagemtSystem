using HotelManagement.Application.Features.Reservation.DTOs;
using HotelManagement.Application.Features.Room.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Application.Features.Reservation.Queries
{
    public record GetReservationsQuery : IRequest<List<ReservationDto>>;
    public class GetReservationsQueryHandler
    : IRequestHandler<GetReservationsQuery, List<ReservationDto>>
    {
        private readonly IAppDbContext _context;

        public GetReservationsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReservationDto>> Handle(
            GetReservationsQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Reservations
                .AsNoTracking()
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
                .ToListAsync(cancellationToken);
        }
    }

}
