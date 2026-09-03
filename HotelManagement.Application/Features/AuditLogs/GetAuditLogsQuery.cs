using HotelManagement.Application.Features.AuditLogs.DTOs;
using HotelManagement.Application.Features.Room.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Application.Features.AuditLogs
{
    public record GetAuditLogsQuery : IRequest<List<AuditLogDto>>;
    public class GetAuditLogsQueryHandler
    : IRequestHandler<GetAuditLogsQuery, List<AuditLogDto>>
    {
        private readonly IAppDbContext _context;

        public GetAuditLogsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AuditLogDto>> Handle(
            GetAuditLogsQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.AuditLogs
                .AsNoTracking()
                .OrderByDescending(x => x.ActionDate)
                .Select(x => new AuditLogDto(
                    x.Id,
                    x.Action,
                    x.EntityName,
                    x.EntityId,
                    x.UserId,
                    x.Details,
                    x.ActionDate
                    
                ))
                .ToListAsync(cancellationToken);
        }
    }
}
