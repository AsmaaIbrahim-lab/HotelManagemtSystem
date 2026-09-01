using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Application.Features.Auth.Interfaces
{
    public interface ICurrentUser
    {
        string? UserId { get; }

    }
}
