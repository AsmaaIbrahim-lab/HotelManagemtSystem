using HotelManagement.Application.Features.Room.DTOs;
using HotelManagement.Application.Features.Room.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Application.Features.Room.Queries
{
    
    public record GetRoomByIdQuery(int Id) : IRequest<RoomDto?>;

    public class GetRoomByIdQueryHandler
        : IRequestHandler<GetRoomByIdQuery, RoomDto?>
    {
        private readonly IAppDbContext _context;

        public GetRoomByIdQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<RoomDto?> Handle(
            GetRoomByIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Rooms
                .AsNoTracking()
                .Where(r => r.Id == request.Id && r.IsDeleted ==false)
                .Select(r => new RoomDto
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    RoomType = r.Type,
                    PricePerNight = r.PricePerNight,
                    IsAvailable = r.IsAvailable,
                    CreatedAt = r.CreatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
