using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Application.Features.Room.DTOs
{
    public record UpdateRoomRequest(
       string RoomNumber,
       string RoomType,
       decimal PricePerNight
   );

}
