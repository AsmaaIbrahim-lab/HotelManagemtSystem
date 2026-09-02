using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Application.Features.Room.DTOs
{
    public class RoomDto
    {

        public string RoomNumber { get; set; } = string.Empty;

        public string RoomType { get; set; } = string.Empty;

        public decimal PricePerNight { get; set; }

       
    }
}
