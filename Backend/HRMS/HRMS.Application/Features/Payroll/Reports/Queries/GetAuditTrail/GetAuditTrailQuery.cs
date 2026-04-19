using HRMS.Application.DTOs.Payroll.Processing;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Reports.Queries.GetAuditTrail;

/// <summary>
/// الحصول على سجل التدقيق لعمليات الرواتب
/// Get audit trail for payroll operations
/// </summary>
public class GetAuditTrailQuery : IRequest<Result<List<PayrollAuditDto>>>
{
    public string? EntityName { get; set; }
    public int? EntityId { get; set; }
    public string? Action { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class GetAuditTrailQueryHandler : IRequestHandler<GetAuditTrailQuery, Result<List<PayrollAuditDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAuditTrailQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<PayrollAuditDto>>> Handle(GetAuditTrailQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // استعلام أساسي للتدقيق
            // Base audit query
            var query = _context.AuditLogs.AsQueryable();

            // تطبيق الفلاتر
            // Apply filters
            if (!string.IsNullOrWhiteSpace(request.EntityName))
            {
                query = query.Where(a => a.TableName == request.EntityName);
            }

            if (request.EntityId.HasValue)
            {
                query = query.Where(a => a.RecordId == request.EntityId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Action))
            {
                query = query.Where(a => a.ActionType == request.Action);
            }

            if (request.DateFrom.HasValue)
            {
                query = query.Where(a => a.PerformedAt >= request.DateFrom.Value);
            }

            if (request.DateTo.HasValue)
            {
                query = query.Where(a => a.PerformedAt <= request.DateTo.Value);
            }

            // ترتيب وتطبيق Pagination
            // Order and apply pagination
            var auditLogs = await query
                .OrderByDescending(a => a.PerformedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // تحويل إلى DTO
            // Map to DTO
            var result = auditLogs.Select(a => new PayrollAuditDto
            {
                AuditId = a.LogId,
                EntityName = a.TableName,
                EntityId = (int)a.RecordId,
                Action = a.ActionType ?? "UNKNOWN",
                PerformedBy = a.PerformedBy,
                PerformedAt = a.PerformedAt,
                OldValues = a.OldValue,
                NewValues = a.NewValue,
                Notes = $"IP: {a.IpAddress}"
            }).ToList();

            return Result<List<PayrollAuditDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<List<PayrollAuditDto>>.Failure($"Error retrieving audit trail: {ex.Message}");
        }
    }
}
