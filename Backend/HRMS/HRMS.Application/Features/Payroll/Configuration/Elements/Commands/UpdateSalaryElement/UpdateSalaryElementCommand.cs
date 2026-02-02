using AutoMapper;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Configuration.Elements.Commands.UpdateSalaryElement;

public class UpdateSalaryElementCommand : IRequest<Result<bool>>
{
    public int ElementId { get; set; }
    public string ElementNameAr { get; set; } = string.Empty;
    public string ElementType { get; set; } = "EARNING";
    public bool IsTaxable { get; set; }
    public bool IsGosiBase { get; set; }
    public bool IsRecurring { get; set; }
    public bool IsBasic { get; set; } // NEW
    public decimal? DefaultPercentage { get; set; }
}

public class UpdateSalaryElementCommandHandler : IRequestHandler<UpdateSalaryElementCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateSalaryElementCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateSalaryElementCommand request, CancellationToken cancellationToken)
    {
        if (request.ElementType != "EARNING" && request.ElementType != "DEDUCTION")
            return Result<bool>.Failure("نوع العنصر يجب أن يكون EARNING أو DEDUCTION");

        var element = await _context.SalaryElements.FirstOrDefaultAsync(e => e.ElementId == request.ElementId, cancellationToken);
        if (element == null)
            return Result<bool>.Failure("العنصر غير موجود");

        if (request.IsBasic)
        {
            var existingBasic = await _context.SalaryElements.AnyAsync(e => e.IsBasic == 1 && e.ElementId != request.ElementId, cancellationToken);
            if (existingBasic) return Result<bool>.Failure("يوجد بالفعل عنصر راتب أساسي آخر. لا يمكن إضافة أكثر من عنصر أساسي واحد.");
        }

        element.ElementNameAr = request.ElementNameAr;
        element.ElementType = request.ElementType;
        element.IsTaxable = request.IsTaxable ? (byte)1 : (byte)0;
        element.IsGosiBase = request.IsGosiBase ? (byte)1 : (byte)0;
        element.IsRecurring = request.IsRecurring ? (byte)1 : (byte)0;
        element.IsBasic = request.IsBasic ? (byte)1 : (byte)0;
        element.DefaultPercentage = request.DefaultPercentage;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم تحديث العنصر بنجاح");
    }
}
