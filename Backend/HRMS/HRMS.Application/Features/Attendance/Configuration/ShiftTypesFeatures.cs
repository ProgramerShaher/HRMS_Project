using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Attendance;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.Features.Attendance.Configuration;

// ═══════════════════════════════════════════════════════════
// 1. DTO
// ═══════════════════════════════════════════════════════════
public class ShiftTypeDto
{
    public int ShiftId { get; set; }
    public string ShiftNameAr { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public decimal HoursCount { get; set; }
    public byte IsCrossDay { get; set; }
    public short GracePeriodMins { get; set; }
}

// ═══════════════════════════════════════════════════════════
// 2. CREATE
// ═══════════════════════════════════════════════════════════
public record CreateShiftTypeCommand(string ShiftNameAr, string StartTime, string EndTime, byte IsCrossDay, short GracePeriodMins = 15) : IRequest<Result<int>>;

public class CreateShiftTypeValidator : AbstractValidator<CreateShiftTypeCommand>
{
    public CreateShiftTypeValidator()
    {
        RuleFor(p => p.ShiftNameAr).NotEmpty().WithMessage("اسم المناوبة مطلوب");
        RuleFor(p => p.StartTime).Matches(@"^([01][0-9]|2[0-3]):[0-5][0-9]$").WithMessage("صيغة وقت البدء غير صحيحة (HH:mm)");
        RuleFor(p => p.EndTime).Matches(@"^([01][0-9]|2[0-3]):[0-5][0-9]$").WithMessage("صيغة وقت النهاية غير صحيحة (HH:mm)");
    }
}

public class CreateShiftTypeHandler : IRequestHandler<CreateShiftTypeCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public CreateShiftTypeHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CreateShiftTypeCommand request, CancellationToken cancellationToken)
    {
        // Calc Hours
        decimal hours = 0;
        if (TimeSpan.TryParse(request.StartTime, out var start) && TimeSpan.TryParse(request.EndTime, out var end))
        {
            if (request.IsCrossDay == 1 && end <= start)
            {
                hours = (decimal)(end.Add(TimeSpan.FromDays(1)) - start).TotalHours;
            }
            else
            {
                hours = (decimal)(end - start).TotalHours;
            }
        }

        var shift = new ShiftType
        {
            ShiftNameAr = request.ShiftNameAr,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            IsCrossDay = request.IsCrossDay,
            GracePeriodMins = request.GracePeriodMins,
            HoursCount = Math.Abs(hours)
        };

        _context.ShiftTypes.Add(shift);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(shift.ShiftId, "تم إضافة المناوبة بنجاح");
    }
}

// ═══════════════════════════════════════════════════════════
// 3. UPDATE
// ═══════════════════════════════════════════════════════════
public record UpdateShiftTypeCommand(int ShiftId, string ShiftNameAr, string StartTime, string EndTime, byte IsCrossDay, short GracePeriodMins) : IRequest<Result<bool>>;

public class UpdateShiftTypeValidator : AbstractValidator<UpdateShiftTypeCommand>
{
    public UpdateShiftTypeValidator()
    {
        RuleFor(p => p.ShiftId).GreaterThan(0);
        RuleFor(p => p.ShiftNameAr).NotEmpty().WithMessage("اسم المناوبة مطلوب");
        RuleFor(p => p.StartTime).Matches(@"^([01][0-9]|2[0-3]):[0-5][0-9]$").WithMessage("صيغة وقت البدء غير صحيحة (HH:mm)");
        RuleFor(p => p.EndTime).Matches(@"^([01][0-9]|2[0-3]):[0-5][0-9]$").WithMessage("صيغة وقت النهاية غير صحيحة (HH:mm)");
    }
}

public class UpdateShiftTypeHandler : IRequestHandler<UpdateShiftTypeCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateShiftTypeHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateShiftTypeCommand request, CancellationToken cancellationToken)
    {
        var shift = await _context.ShiftTypes.FindAsync(new object[] { request.ShiftId }, cancellationToken);
        if (shift == null) return Result<bool>.Failure("المناوبة غير موجودة");

        // Calc Hours
        decimal hours = 0;
        if (TimeSpan.TryParse(request.StartTime, out var start) && TimeSpan.TryParse(request.EndTime, out var end))
        {
            if (request.IsCrossDay == 1 && end <= start)
            {
                hours = (decimal)(end.Add(TimeSpan.FromDays(1)) - start).TotalHours;
            }
            else
            {
                hours = (decimal)(end - start).TotalHours;
            }
        }

        shift.ShiftNameAr = request.ShiftNameAr;
        shift.StartTime = request.StartTime;
        shift.EndTime = request.EndTime;
        shift.IsCrossDay = request.IsCrossDay;
        shift.GracePeriodMins = request.GracePeriodMins;
        shift.HoursCount = Math.Abs(hours);

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true, "تم تعديل المناوبة بنجاح");
    }
}

// ═══════════════════════════════════════════════════════════
// 4. DELETE
// ═══════════════════════════════════════════════════════════
public record DeleteShiftTypeCommand(int ShiftId) : IRequest<Result<bool>>;

public class DeleteShiftTypeHandler : IRequestHandler<DeleteShiftTypeCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteShiftTypeHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteShiftTypeCommand request, CancellationToken cancellationToken)
    {
        var shift = await _context.ShiftTypes.FindAsync(new object[] { request.ShiftId }, cancellationToken);
        if (shift == null) return Result<bool>.Failure("المناوبة غير موجودة");

        shift.IsDeleted = 1; // Soft Delete
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true, "تم حذف المناوبة بنجاح");
    }
}

// ═══════════════════════════════════════════════════════════
// 5. QUERY (GET ALL)
// ═══════════════════════════════════════════════════════════
public record GetShiftTypesQuery : IRequest<Result<List<ShiftTypeDto>>>;

public class GetShiftTypesHandler : IRequestHandler<GetShiftTypesQuery, Result<List<ShiftTypeDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetShiftTypesHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ShiftTypeDto>>> Handle(GetShiftTypesQuery request, CancellationToken cancellationToken)
    {
        var shifts = await _context.ShiftTypes
            .AsNoTracking()
            .Where(s => s.IsDeleted == 0)
            .OrderBy(s => s.ShiftId)
            .Select(s => new ShiftTypeDto
            {
                ShiftId = s.ShiftId,
                ShiftNameAr = s.ShiftNameAr,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                HoursCount = s.HoursCount,
                IsCrossDay = s.IsCrossDay,
                GracePeriodMins = s.GracePeriodMins
            })
            .ToListAsync(cancellationToken);

        return Result<List<ShiftTypeDto>>.Success(shifts);
    }
}
