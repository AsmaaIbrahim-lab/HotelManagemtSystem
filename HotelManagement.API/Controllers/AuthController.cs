using HotelManagement.Application.DTOs.AuthDTOs;
using HotelManagement.Application.Features.Auth.Commands.Login;
using HotelManagement.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO RegisterDTO)
        {
            RegisterCommand command = new RegisterCommand( RegisterDTO.FullName, RegisterDTO.Email, RegisterDTO.Password);
            var userResponse = await _mediator.Send(command);
            return Ok(new { UserId = userResponse.UserId, Message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

    }
}
