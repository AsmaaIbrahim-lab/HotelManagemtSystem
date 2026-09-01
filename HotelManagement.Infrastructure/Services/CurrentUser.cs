using HotelManagement.Application.Features.Auth.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace HotelManagement.Infrastructure.Services
{
    public class CurrentUser: ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    //    string ICurrentUser.UserId =>
    //_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true
    //? _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
    //: null;
        public string? UserId =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}



