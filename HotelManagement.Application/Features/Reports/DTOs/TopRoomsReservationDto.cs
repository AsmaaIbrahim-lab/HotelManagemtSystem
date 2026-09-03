using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Application.Features.Reports.DTOs
{
    public class TopRoomsReservationDto
    {
        public string roomNumber { get; set; }
        public string roomType { get; set; }
        public int reservationCount { get; set; }
        public decimal totalRevenue { get; set; }
    }
}
