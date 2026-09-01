using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;


namespace HotelManagement.Application.Domain.Entities
{
    public class User: IdentityUser
    {
        public string FullName { get; set; } 
        public bool IsActive { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    }
}
