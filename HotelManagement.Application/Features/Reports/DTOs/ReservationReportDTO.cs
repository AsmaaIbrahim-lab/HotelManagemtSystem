using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Application.Features.Reports.DTOs
{
   
        public class RevenueReportDto
        {
            public int TotalReservations { get; set; }
            public int TotalNights { get; set; }
            public decimal TotalRevenue { get; set; }
            public List<RoomTypeRevenueDto> ByRoomType { get; set; } = new();
        }

        public class RoomTypeRevenueDto
        {
            public string RoomType { get; set; } = string.Empty;
            public int TotalReservations { get; set; }
            public int TotalNights { get; set; }
            public decimal TotalRevenue { get; set; }
        }
    
}
