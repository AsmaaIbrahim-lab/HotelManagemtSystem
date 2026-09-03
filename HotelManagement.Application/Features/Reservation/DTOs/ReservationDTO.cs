using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Application.Features.Reservation.DTOs
{

    public class ReservationDto
    {
        public int Id { get; set; }

        public int RoomId { get; set; }

        public string RoomNumber { get; set; }

        public string GuestName { get; set; } = string.Empty;

        public DateOnly CheckInDate { get; set; }

        public DateOnly CheckOutDate { get; set; }

        public decimal TotalPrice { get; set; }

        public string Status { get; set; } = string.Empty;
    
}
}
