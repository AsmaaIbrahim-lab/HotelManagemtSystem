using HotelManagement.Application.Common;
using HotelManagement.Application.Features.Auth.Interfaces;
using HotelManagement.Application.Features.Reports.DTOs;
using HotelManagement.Application.Features.Room.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace HotelManagement.Application.Features.Reports.Queries
{
    public record GetRevenueReportQuery(DateOnly From, DateOnly To) : IRequest<RevenueReportDto>;
    public class TotalReservationFromToQuery : IRequestHandler<GetRevenueReportQuery, RevenueReportDto>
    {

        private readonly IAppDbContext _context;
        public TotalReservationFromToQuery(IAppDbContext context)
        {
            _context = context;

        }
        public async Task<RevenueReportDto> Handle(GetRevenueReportQuery request, CancellationToken cancellationToken)
        {
            var from = request.From;
            var to = request.To;

            var validReservationsQuery = _context.Reservations
        .Where(r => r.Status != "Cancelled"
                 && r.CheckInDate >= from
                 && r.CheckOutDate <= to)
        .Join(
            _context.Rooms,
            reservation => reservation.RoomId,
            room => room.Id,
            (reservation, room) => new
            {
                ReservationId = reservation.Id,
                RoomType = room.Type,
                Nights = reservation.CheckOutDate.DayNumber - reservation.CheckInDate.DayNumber,
                Revenue = reservation.TotalPrice
            }
        );

            var groupedByRoomType = await validReservationsQuery
                .GroupBy(x => x.RoomType)
                .Select(g => new RoomTypeRevenueDto
                {
                    RoomType = g.Key,
                    TotalReservations = g.Count(),
                    TotalNights = g.Sum(x => x.Nights),
                    TotalRevenue = g.Sum(x => x.Revenue)
                })
                .ToListAsync();

            var totalReservations = groupedByRoomType.Sum(x => x.TotalReservations);
            var totalNights = groupedByRoomType.Sum(x => x.TotalNights);
            var totalRevenue = groupedByRoomType.Sum(x => x.TotalRevenue);

            var result = new RevenueReportDto
            {
                TotalReservations = totalReservations,
                TotalNights = totalNights,
                TotalRevenue = totalRevenue,
                ByRoomType = groupedByRoomType
            };
            return result;
        }
    }
}
