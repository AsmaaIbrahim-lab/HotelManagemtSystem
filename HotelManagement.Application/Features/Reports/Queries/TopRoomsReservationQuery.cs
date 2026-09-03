using HotelManagement.Application.Common;
using HotelManagement.Application.Features.Auth.Interfaces;
using HotelManagement.Application.Features.Reports.DTOs;
using HotelManagement.Application.Features.Room.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Application.Features.Reports.Queries
{
    public record TopRoomsReservation(int count) : IRequest<List<TopRoomsReservationDto>>
    {
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
    }
    public class TopRoomsReservationQuery : IRequestHandler<TopRoomsReservation, List<TopRoomsReservationDto>>
    {
        private readonly IAppDbContext _context;
        public TopRoomsReservationQuery(IAppDbContext context)
        {
            _context = context;

        }
        public async Task<List<TopRoomsReservationDto>> Handle(TopRoomsReservation request, CancellationToken cancellationToken)
        {
            var result = await _context.Reservations
     .Where(r => r.Status != "Cancelled")
     .Join(
         _context.Rooms,
         reservation => reservation.RoomId,
         room => room.Id,
         (reservation, room) => new
         {
             RoomNumber = room.RoomNumber,
             RoomType = room.Type,
             TotalPrice = reservation.TotalPrice
         }
     )
     .GroupBy(x => new
     {
         x.RoomNumber,
         x.RoomType
     })
     .Select(g => new TopRoomsReservationDto
     {
         roomNumber = g.Key.RoomNumber,
         roomType = g.Key.RoomType,
         reservationCount = g.Count(),
         totalRevenue = g.Sum(x => x.TotalPrice)
     })
     .OrderByDescending(x => x.reservationCount)
     .Take(request.count)
     .ToListAsync(cancellationToken);

            return result;


        }
    }
}
