using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Application.Features.Dashboard
{
    public class OccupancySummaryDto
    {
        public int TotalRooms { get; set; }
        public int AvailableNow { get; set; }
        public int OccupiedNow { get; set; }
        public double OccupancyPercent { get; set; }
    }
}
