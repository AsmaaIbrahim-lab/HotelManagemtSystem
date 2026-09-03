using HotelManagement.Application.Domain.Entities;
using HotelManagement.Application.Features.Reservation.Commands;
using HotelManagement.Application.Features.Room.Commands;
using HotelManagement.Application.Features.Room.DTOs;
using HotelManagement.Application.Features.Room.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class RoomsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoomsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(
            CreateRoomCommand command)
        {
            try
            {
                var id = await _mediator.Send(command);

                return Ok(new { id });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _mediator.Send(
                new GetRoomsQuery());

            return Ok(rooms);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var room = await _mediator.Send(
                new GetRoomByIdQuery(id));

            if (room is null)
                return NotFound();

            return Ok(room);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
           int id,
           UpdateRoomRequest request)
        {
            try
            {
                var command = new UpdateRoomCommand(
                    id,
                    request.RoomNumber,
                    request.RoomType,
                    request.PricePerNight
                );

                await _mediator.Send(command);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _mediator.Send(
                    new DeleteRoomCommand(id));

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailable(
         [FromQuery] GetAvailableRoomsQuery query)
        {
            var rooms = await _mediator.Send(query);

            return Ok(rooms);
        }

    



    }
}
