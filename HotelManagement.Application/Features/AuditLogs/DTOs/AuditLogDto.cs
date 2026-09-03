using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HotelManagement.Application.Features.AuditLogs.DTOs
{
    public record AuditLogDto(
    int Id,
    string Action,
    string EntityName,
    string? EntityId,
    string? UserId,
    string Details,
    DateTime ActionDate
);
}
