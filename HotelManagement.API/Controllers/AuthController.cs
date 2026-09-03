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
            try
            {
                RegisterCommand command = new RegisterCommand( RegisterDTO.FullName, RegisterDTO.Email, RegisterDTO.Password);
                var userResponse = await _mediator.Send(command);
                return Ok(new { UserId = userResponse.UserId, Message = "User registered successfully." });
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("already registered"))
                    return Conflict(new { message = ex.Message });

                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            try
            {
                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }
        }

    }
}
