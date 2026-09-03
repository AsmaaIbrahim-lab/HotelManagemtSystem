using HotelManagement.Application.Features.Reports.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("occupancy")]
        public async Task<IActionResult> GetOccupancyReport(
            [FromQuery] DateOnly from,
            [FromQuery] DateOnly to)
        {
            var result = await _mediator.Send(
                new GetOccupancyReport(from, to));

            return Ok(result);
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueReport(
            [FromQuery] DateOnly from,
            [FromQuery] DateOnly to)
        {
            var result = await _mediator.Send(
                new GetRevenueReportQuery(from, to));

            return Ok(result);
        }

        [HttpGet("top-rooms")]
        public async Task<IActionResult> GetTopRooms(
            [FromQuery] int count = 10)
        {
            var result = await _mediator.Send(
                new TopRoomsReservation(count));

            return Ok(result);
        }
    }
}
