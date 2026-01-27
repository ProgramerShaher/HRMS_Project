using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Leaves;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Application.Exceptions;
using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Leaves;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Application.Exceptions;

using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Collections.Generic;

namespace HRMS.Application.Features.Leaves.Requests.Commands.CreateLeaveRequest;

// 1. Command
/// <summary>
/// أمر تقديم طلب إجازة جديد
/// </summary>
public record CreateLeaveRequestCommand(int EmployeeId, int LeaveTypeId, DateTime StartDate, DateTime EndDate, string Reason) : IRequest<int>;

// 2. Validator
public class CreateLeaveRequestValidator : AbstractValidator<CreateLeaveRequestCommand>
{
    public CreateLeaveRequestValidator()
    {
        RuleFor(x => x.EmployeeId).GreaterThan(0).WithMessage("معرف الموظف مطلوب");

        RuleFor(x => x.Reason).NotEmpty().WithMessage("سبب الإجازة مطلوب")
            .MaximumLength(200).WithMessage("السبب طويل جداً");
        
        RuleFor(x => x.StartDate).NotEmpty().WithMessage("تاريخ البداية مطلوب")
            .GreaterThan(DateTime.Today.AddDays(-30)).WithMessage("لا يمكن تقديم طلب إجازة لتاريخ قديم جداً");

        RuleFor(x => x.EndDate).NotEmpty().WithMessage("تاريخ النهاية مطلوب")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("تاريخ النهاية يجب أن يكون بعد البداية");
    }
}

// 3. Handler
public class CreateLeaveRequestCommandHandler : IRequestHandler<CreateLeaveRequestCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateLeaveRequestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        // 1. Verify Employee Exists & Get Info
        var employee = await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId, cancellationToken);
            
        if (employee == null)
            throw new NotFoundException("Employee", request.EmployeeId);
        
        int employeeId = request.EmployeeId;

        // 2. Fetch Leave Type
        var leaveType = await _context.LeaveTypes.FindAsync(new object[] { request.LeaveTypeId }, cancellationToken);
        if (leaveType == null)
            throw new NotFoundException("Leave Type", request.LeaveTypeId);

        // 3. PROBATION PERIOD CHECK (Annual Leave Only)
        // If it's Annual Leave, they must have served the probation period (Default: 3 Months)
        if (leaveType.LeaveNameEn != null && leaveType.LeaveNameEn.Contains("Annual", StringComparison.OrdinalIgnoreCase))
        {
            int probationMonths = 3; // Default value if not set in DB
            
            var probationSetting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.SettingKey == "PROBATION_PERIOD_MONTHS", cancellationToken);
            
            if (probationSetting != null && int.TryParse(probationSetting.SettingValue, out int outputMonths))
            {
                probationMonths = outputMonths;
            }

            var monthsEmployed = ((DateTime.Today.Year - employee.HireDate.Year) * 12) + DateTime.Today.Month - employee.HireDate.Month;
            // Adjust for exact day of month
            if (DateTime.Today.Day < employee.HireDate.Day) monthsEmployed--;

            if (monthsEmployed < probationMonths)
            {
                throw new FluentValidation.ValidationException(
                    $"عذراً، لا يمكنك طلب إجازة سنوية خلال فترة التجربة. يجب إكمال {probationMonths} أشهر، وأنت أكملت {monthsEmployed} أشهر فقط.");
            }
        }

        // 4. OVERLAP VALIDATION
        var hasOverlap = await _context.LeaveRequests
            .AnyAsync(r => 
                r.EmployeeId == employeeId && 
                r.Status != "REJECTED" &&
                r.StartDate <= request.EndDate && 
                r.EndDate >= request.StartDate, 
                cancellationToken);

        if (hasOverlap)
            throw new FluentValidation.ValidationException("عذراً، لديك إجازة أخرى متداخلة في نفس الفترة");

        // 5. SMART CALCULATION (Actual Days)
        // Fetch Public Holidays for the year(s) covering the request
        var years = new[] { request.StartDate.Year, request.EndDate.Year }.Distinct();
        var publicHolidays = await _context.PublicHolidays
            .Where(h => years.Contains(h.Year))
            .ToListAsync(cancellationToken);

        // Expand holidays to individual dates
        var holidayDates = new HashSet<DateTime>();
        foreach (var ph in publicHolidays)
        {
            for (var date = ph.StartDate; date <= ph.EndDate; date = date.AddDays(1))
            {
                holidayDates.Add(date.Date);
            }
        }

        int daysCount = CalculateActualLeaveDays(request.StartDate, request.EndDate, holidayDates);

        if (daysCount <= 0) 
            throw new FluentValidation.ValidationException("مدة الإجازة الصافية هي صفر (ربما اخترت أيام عطل فقط).");

        // 6. Balance Check (If Deductible)
        if (leaveType.IsDeductible)
        {
            var year = (short)request.StartDate.Year;
            var balance = await _context.LeaveBalances
                .FirstOrDefaultAsync(b => b.EmployeeId == employeeId && b.LeaveTypeId == request.LeaveTypeId && b.Year == year, cancellationToken);

            if (balance == null || balance.CurrentBalance < daysCount)
                throw new FluentValidation.ValidationException($"رصيد الإجازة غير كافٍ. المتوفر: {(balance?.CurrentBalance ?? 0)}، المطلوب (الصافي): {daysCount}");
        }

        // 7. Create Request
        var leaveRequest = new LeaveRequest
        {
            EmployeeId = employeeId,
            LeaveTypeId = request.LeaveTypeId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            DaysCount = daysCount, // Store ACTUAL calculated days
            Reason = request.Reason,
            Status = "PENDING",
            IsPostedToBalance = 0 // Not posted yet
        };

        _context.LeaveRequests.Add(leaveRequest);
        await _context.SaveChangesAsync(cancellationToken);

        return leaveRequest.RequestId;
    }

    private int CalculateActualLeaveDays(DateTime start, DateTime end, HashSet<DateTime> holidays)
    {
        int count = 0;
        for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
        {
            // Exclude ONLY Fridays
            if (date.DayOfWeek == DayOfWeek.Friday) continue;

            // Exclude Public Holidays
            if (holidays.Contains(date)) continue;

            // Include Saturdays (Work Days) - Already included since we only skipped Friday
            count++;
        }
        return count;
    }
}
