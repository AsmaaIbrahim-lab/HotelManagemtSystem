using HotelManagement.Application.Features.Reports.DTOs;
using HotelManagement.Application.Features.Room.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HotelManagement.Application.Features.Reports.Queries
{

    public record GetOccupancyReport(DateOnly from, DateOnly to) : IRequest<List<OccupancyReportDto>>;
    

public class GetOccupancyReportHandler
    : IRequestHandler<GetOccupancyReport, List<OccupancyReportDto>>
    {

        private readonly IAppDbContext _context;

        public GetOccupancyReportHandler(IAppDbContext db)
        {
            _context = db;

        }

        public async Task<List<OccupancyReportDto>> Handle(
            GetOccupancyReport request, CancellationToken ct)
        {
            var from = request.from;
            var to = request.to;

            if (to <= from)
                throw new ValidationException("'to' must be after 'from'.");

            var totalAvailableNights = to.DayNumber - from.DayNumber;

            // Pull rooms with their reservations that could possibly overlap the window
            var rooms = await _context.Rooms
                .Select(r => new
                {
                    r.Id,
                    r.RoomNumber,
                    r.Type,
                    Reservations = r.Reservations
                        .Where(res => res.Status != "Cancelled"
                                      && res.CheckInDate < to
                                      && res.CheckOutDate > from)
                        .Select(res => new { res.CheckInDate, res.CheckOutDate })
                        .ToList()
                })
                .ToListAsync(ct);

            var result = rooms.Select(r =>
            {
                // Clip each reservation to the [from, to) window, then union the covered nights
                var bookedNights = CountBookedNights(r.Reservations, from, to);

                return new OccupancyReportDto
                {
                    RoomId = r.Id,
                    RoomNumber = r.RoomNumber,
                    RoomType = r.Type,
                    BookedNights = bookedNights,
                    AvailableNights = totalAvailableNights,
                    OccupancyPercentage = totalAvailableNights == 0
                        ? 0
                        : Math.Round(bookedNights * 100.0 / totalAvailableNights, 2)
                };
            }).ToList();

            return result;
        }

        private static int CountBookedNights(
            IEnumerable<dynamic> reservations, DateOnly from, DateOnly to)
        {
         
            var bookedDates = new HashSet<DateOnly>();

            foreach (var res in reservations)
            {
                DateOnly checkIn = res.CheckInDate;
                DateOnly checkOut = res.CheckOutDate;

                var clippedStart = checkIn < from ? from : checkIn;
                var clippedEnd = checkOut > to ? to : checkOut;

                for (var d = clippedStart; d < clippedEnd; d = d.AddDays(1))
                    bookedDates.Add(d);
            }

            return bookedDates.Count;
        }
    }
}
