using HotelManagement.Application.Features.Room.DTOs;
using HotelManagement.Application.Features.Room.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Application.Features.Room.Queries
{
    public record GetAvailableRoomsQuery(
    string? RoomType,
    decimal? MinPrice,
    decimal? MaxPrice,
    DateOnly CheckInDate,
    DateOnly CheckOutDate
) : IRequest<List<RoomDto>>;
    public class GetAvailableRoomsQueryHandler
    : IRequestHandler<GetAvailableRoomsQuery, List<RoomDto>>
    {
        private readonly IAppDbContext _context;

        public GetAvailableRoomsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RoomDto>> Handle(
            GetAvailableRoomsQuery request,
            CancellationToken cancellationToken)
        {
            if (request.CheckOutDate <= request.CheckInDate)
            {
                throw new ArgumentException(
                    "Check-out date must be after check-in date.");
            }

            var query = _context.Rooms
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.RoomType))
            {
                query = query.Where(r =>
                    r.Type == request.RoomType);
            }

            if (request.MinPrice.HasValue)
            {
                query = query.Where(r =>
                    r.PricePerNight >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                query = query.Where(r =>
                    r.PricePerNight <= request.MaxPrice.Value);
            }

            query = query.Where(room =>
                !room.Reservations.Any(reservation =>
                    reservation.Status != "Cancelled"
                    &&
                    reservation.CheckInDate < request.CheckOutDate
                    &&
                    reservation.CheckOutDate > request.CheckInDate
                ));

            return await query
                .Select(r => new RoomDto
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    RoomType = r.Type,
                    PricePerNight = r.PricePerNight,
                    IsAvailable = r.IsAvailable,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }
    }
}
