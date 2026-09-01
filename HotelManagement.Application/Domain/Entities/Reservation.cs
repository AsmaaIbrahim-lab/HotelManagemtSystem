using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Application.Domain.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string GuestName { get; set; }
        public string Status { get; set; }  // Confirmed, Cancelled
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys & Navigations
        public string CreatedBy { get; set; } = string.Empty;
        public User User { get; set; } = null!;

        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;
    }
}
