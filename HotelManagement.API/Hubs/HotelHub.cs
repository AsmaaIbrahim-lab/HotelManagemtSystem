using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HotelManagement.API.Hubs
{
    [Authorize]
    public class HotelHub:Hub
    {

    }
}
