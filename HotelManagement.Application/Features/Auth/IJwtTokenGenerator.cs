using HotelManagement.Application.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Application.Features.Auth
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
