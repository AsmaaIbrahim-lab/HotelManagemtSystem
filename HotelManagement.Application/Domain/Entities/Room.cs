using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Application.Domain.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // e.g. Single, Double, Suite
        public decimal PricePerNight { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;

        
        // Navigation Properties
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
