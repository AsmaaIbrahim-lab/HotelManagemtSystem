using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HotelManagement.Application.DTOs.AuthDTOs
{
    public class LoginResponseDto
    {
        public string Email { get; set; } 
        public string Token { get; set; }

    }
}
