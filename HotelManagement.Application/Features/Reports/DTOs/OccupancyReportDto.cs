using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Application.Features.Reports.DTOs
{
    public class OccupancyReportDto
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public int BookedNights { get; set; }
        public int AvailableNights { get; set; }
        public double OccupancyPercentage { get; set; }
    }
}
