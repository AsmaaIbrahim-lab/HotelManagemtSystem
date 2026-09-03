using HotelManagement.Application.Features.Reservation.Commands;
using HotelManagement.Application.Features.Reservation.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReservationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(
        [FromBody] CreateReservationCommand command,
        CancellationToken cancellationToken)
        {
            var reservationId = await _mediator.Send(
                command,
                cancellationToken);

            return Ok(reservationId);
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAll()
        {
            var reservation = await _mediator.Send(
                new GetReservationsQuery());

            if (reservation is null)
                return NotFound();

            return Ok(reservation);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var reservation = await _mediator.Send(
                new GetReservationByIdQuery(id));

            if (reservation is null)
                return NotFound();

            return Ok(reservation);
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search(
         [FromQuery] string? guestName,
         [FromQuery] string? status,
         [FromQuery] DateOnly? checkInDate,
         [FromQuery] DateOnly? checkOutDate,
         [FromQuery] int? roomNumber)
        {
            var result = await _mediator.Send(
                new SearchReservationsQuery(
                    guestName,
                    status,
                    checkInDate,
                    checkOutDate,
                    roomNumber));

            return Ok(result);
        }
        [HttpPut("cancel/{id:int}")]
        public async Task<IActionResult> Cancel(int id)
        {
            await _mediator.Send(
                new CancelReservationCommand(id));

            return NoContent();
        }
    }
}
