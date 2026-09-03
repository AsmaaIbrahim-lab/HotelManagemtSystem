using HotelManagement.Application.Features.Dashboard;
using HotelManagement.Application.Features.Reservation.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static HotelManagement.Application.Features.Dashboard.GetRecentReservationQuery;

namespace HotelManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GetDashboardSummaryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GetDashboardSummaryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("dashboard/summary")]
        public async Task<ActionResult<OccupancySummaryDto>> GetDashboardSummary()
        {
            var result = await _mediator.Send(new GetDashboardSummaryQuery());
            return Ok(result);
        }
        [HttpGet("recent")]
        public async Task<ActionResult<List<ReservationDto>>> GetRecentReservations([FromQuery] int count = 5)
        {
            var result = await _mediator.Send(new GetRecentReservationsQuery(count));
            return Ok(result);
        }
    }
}
