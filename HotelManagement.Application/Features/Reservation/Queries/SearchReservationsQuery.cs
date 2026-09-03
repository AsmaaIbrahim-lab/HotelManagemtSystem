using HotelManagement.Application.Features.Reservation.DTOs;
using HotelManagement.Application.Features.Room.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Application.Features.Reservation.Queries
{
    public record SearchReservationsQuery(
    string? GuestName,
    string? Status,
    DateOnly? CheckInDate,
    DateOnly? CheckOutDate,
    int? RoomId) : IRequest<List<ReservationDto>>;
    public class SearchReservationsQueryHandler
    : IRequestHandler<SearchReservationsQuery, List<ReservationDto>>
    {
        private readonly IAppDbContext _context;

        public SearchReservationsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReservationDto>> Handle(
            SearchReservationsQuery request,
            CancellationToken cancellationToken)
        {
            var query = _context.Reservations
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.GuestName))
            {
                query = query.Where(r =>
                    r.GuestName.Contains(request.GuestName));
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                query = query.Where(r =>
                    r.Status == request.Status);
            }

            if (request.RoomId.HasValue)
            {
                query = query.Where(r =>
                    r.RoomId == request.RoomId.Value);
            }

            if (request.CheckInDate.HasValue)
            {
                query = query.Where(r =>
                    r.CheckInDate >= request.CheckInDate.Value);
            }

            if (request.CheckOutDate.HasValue)
            {
                query = query.Where(r =>
                    r.CheckOutDate <= request.CheckOutDate.Value);
            }

            return await query
                .Select(r => new ReservationDto
                {
                    Id = r.Id,
                    RoomNumber = r.Room.RoomNumber,
                    GuestName = r.GuestName,
                    CheckInDate = r.CheckInDate,
                    CheckOutDate = r.CheckOutDate,
                    TotalPrice = r.TotalPrice,
                    Status = r.Status
                })
                .ToListAsync(cancellationToken);
        }
    }

}
