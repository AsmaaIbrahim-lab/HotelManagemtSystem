using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Application.Domain.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty; // e.g., "CreateReservation", "CancelReservation"
        public string EntityName { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public DateTime ActionDate { get; set; } = DateTime.UtcNow;
        public string Details { get; set; } = string.Empty;

        // Foreign Key & Navigation (Nullable for unauthenticated system events)
        public string? UserId { get; set; }
        public User? User { get; set; }

    }
}
