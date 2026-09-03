using HotelManagement.Application.Features.Room.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HotelManagement.Application.Features.Dashboard
{
    public record GetDashboardSummaryQuery : IRequest<OccupancySummaryDto>;
    public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, OccupancySummaryDto>
    {
        private readonly IAppDbContext _context;

        public GetDashboardSummaryQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<OccupancySummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var Rooms =  _context.Rooms.ToList();
           var  totalRooms= Rooms.Count();

            if (totalRooms == 0)
            {
                return new OccupancySummaryDto
                {
                    TotalRooms = 0,
                    AvailableNow = 0,
                    OccupiedNow = 0,
                    OccupancyPercent = 0
                };
            }


            var occupiedNowCount = _context.Reservations
                .Where(r => r.Status != "Cancelled"
                         && r.CheckInDate <= today
                         && r.CheckOutDate > today)
                .Select(r => r.RoomId).ToList().Count();


            var availableNow = totalRooms - occupiedNowCount;
            var occupancyPercent = Math.Round(((double)occupiedNowCount / totalRooms) * 100, 2);

            return new OccupancySummaryDto
            {
                TotalRooms = totalRooms,
                AvailableNow = availableNow,
                OccupiedNow = occupiedNowCount,
                OccupancyPercent = occupancyPercent
            };
        }
    }
}
