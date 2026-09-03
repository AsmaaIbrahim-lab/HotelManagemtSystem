using HotelManagement.Application.Features.Room.DTOs;
using HotelManagement.Application.Features.Room.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
namespace HotelManagement.Application.Features.Room.Queries
{
    public record GetRoomsQuery : IRequest<List<RoomDto>>;
    public class GetRoomsQueryHandler
     : IRequestHandler<GetRoomsQuery, List<RoomDto>>
    {
        private readonly IAppDbContext _context;

        public GetRoomsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RoomDto>> Handle(
            GetRoomsQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Rooms
                .AsNoTracking()
                .Where(r=>r.IsDeleted == false)
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
