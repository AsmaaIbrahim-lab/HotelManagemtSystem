using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Application.Common
{
    public interface IHotelHubContext
    {
        Task SendReservationCreatedAsync(object data, CancellationToken cancellationToken = default);
        Task SendReservationCancelledAsync(object data, CancellationToken cancellationToken = default);
        Task SendRoomUpdatedAsync(object data, CancellationToken cancellationToken = default);
        Task SendRoomCreatedAsync(object data, CancellationToken cancellationToken = default);
        Task SendRoomDeletedAsync(object data, CancellationToken cancellationToken = default);


    }
}
