using HotelManagement.Application.Features.Room.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoomsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [Authorize]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(
            CreateRoomCommand command)
        {
            var id = await _mediator.Send(command);

            return Ok(new { id }); ;
        }


    }
}
