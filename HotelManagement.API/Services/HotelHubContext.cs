using HotelManagement.API.Hubs;
using HotelManagement.Application.Common;
using Microsoft.AspNetCore.SignalR;

namespace HotelManagement.API.Services
{
 

    
        public class HotelHubContext : IHotelHubContext
        {
            private readonly IHubContext<HotelHub> _hubContext;

            public HotelHubContext(IHubContext<HotelHub> hubContext)
            {
                _hubContext = hubContext;
            }

            public async Task SendReservationCreatedAsync(object data, CancellationToken cancellationToken = default)
            {
                await _hubContext.Clients.All.SendAsync("reservationCreated", data, cancellationToken);
            }

            public async Task SendReservationCancelledAsync(object data, CancellationToken cancellationToken = default)
            {
                await _hubContext.Clients.All.SendAsync("reservationCancelled", data, cancellationToken);
            }

            public async Task SendRoomUpdatedAsync(object data, CancellationToken cancellationToken = default)
            {
                await _hubContext.Clients.All.SendAsync("roomUpdated", data, cancellationToken);
            }
        public async Task SendRoomCreatedAsync(object data, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.All.SendAsync("roomCreated", data, cancellationToken);
        }
        public async Task SendRoomDeletedAsync(object data, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.All.SendAsync("roomDeleted", data, cancellationToken);
        }
    }
    }


