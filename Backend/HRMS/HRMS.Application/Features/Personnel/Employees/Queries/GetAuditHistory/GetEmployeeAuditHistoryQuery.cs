using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.Employees.Queries.GetAuditHistory;

/// <summary>
/// استعلام جلب سجل التدقيق للموظف
/// Get Employee Audit History Query
/// </summary>
public class GetEmployeeAuditHistoryQuery : IRequest<Result<List<AuditHistoryDto>>>
{
    /// <summary>
    /// معرف الموظف
    /// </summary>
    public int EmployeeId { get; set; }
}

/// <summary>
/// كائن نقل بيانات سجل التدقيق
/// </summary>
public class AuditHistoryDto
{
    /// <summary>
    /// نوع العملية (Added, Modified, Deleted)
    /// </summary>
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// اسم الجدول
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// القيم القديمة (JSON)
    /// </summary>
    public string? OldValues { get; set; }

    /// <summary>
    /// القيم الجديدة (JSON)
    /// </summary>
    public string? NewValues { get; set; }

    /// <summary>
    /// المستخدم الذي قام بالتعديل
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// عنوان IP
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// تاريخ ووقت التعديل
    /// </summary>
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// معالج استعلام جلب سجل التدقيق
/// </summary>
public class GetEmployeeAuditHistoryQueryHandler : IRequestHandler<GetEmployeeAuditHistoryQuery, Result<List<AuditHistoryDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetEmployeeAuditHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<AuditHistoryDto>>> Handle(GetEmployeeAuditHistoryQuery request, CancellationToken cancellationToken)
    {
        // 1. التحقق من وجود الموظف
        var employeeExists = await _context.Employees
            .AnyAsync(e => e.EmployeeId == request.EmployeeId, cancellationToken);

        if (!employeeExists)
            return Result<List<AuditHistoryDto>>.Failure($"الموظف برقم {request.EmployeeId} غير موجود");

        // 2. جلب سجلات التدقيق من جدول AUDIT_LOGS
        var auditLogs = await _context.AuditLogs
            .Where(a => a.TableName == "EMPLOYEES" && a.RecordId == (long)request.EmployeeId)
            .OrderByDescending(a => a.PerformedAt)
            .Select(a => new AuditHistoryDto
            {
                ActionType = a.ActionType ?? "Unknown",
                TableName = a.TableName,
                OldValues = a.OldValue,
                NewValues = a.NewValue,
                UserId = a.PerformedBy,
                IpAddress = a.IpAddress,
                Timestamp = a.PerformedAt
            })
            .ToListAsync(cancellationToken);

        return Result<List<AuditHistoryDto>>.Success(
            auditLogs,
            $"تم جلب {auditLogs.Count} سجل تدقيق للموظف");
    }
}
