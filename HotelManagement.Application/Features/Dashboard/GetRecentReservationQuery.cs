using HotelManagement.Application.Features.Reservation.DTOs;
using HotelManagement.Application.Features.Room.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Application.Features.Dashboard
{
    public class GetRecentReservationQuery
    {
       
        public record GetRecentReservationsQuery(int Count = 5) : IRequest<List<ReservationDto>>;
        public class GetRecentReservationsQueryHandler : IRequestHandler<GetRecentReservationsQuery, List<ReservationDto>>
        {
            private readonly IAppDbContext _context;

            public GetRecentReservationsQueryHandler(IAppDbContext db)
            {
                _context = db;

            }

            public async Task<List<ReservationDto>> Handle(GetRecentReservationsQuery request, CancellationToken cancellationToken)
            {
                var recentReservations = await _context.Reservations
                    .AsNoTracking()
                    .Join(
                        _context.Rooms,
                        reservation => reservation.RoomId,
                        room => room.Id,
                        (reservation, room) => new { reservation, room }
                    )
                    .OrderByDescending(x => x.reservation.CreatedAt)
                    .Take(request.Count)
                    .Select(x => new ReservationDto
                    {
                        Id = x.reservation.Id,
                        GuestName = x.reservation.GuestName,
                        RoomId = x.reservation.RoomId,
                        RoomNumber = x.room.RoomNumber,
                        CheckInDate = x.reservation.CheckInDate,
                        CheckOutDate = x.reservation.CheckOutDate,
                        TotalPrice = x.reservation.TotalPrice,
                        Status = x.reservation.Status.ToString(),
                    })
                    .ToListAsync(cancellationToken);

                return recentReservations;
            }
        }
    }
}
