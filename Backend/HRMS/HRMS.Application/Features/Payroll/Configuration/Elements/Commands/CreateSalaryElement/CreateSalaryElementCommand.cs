using AutoMapper;
using HRMS.Application.DTOs.Payroll.Configuration;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Payroll;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Configuration.Elements.Commands.CreateSalaryElement;

public class CreateSalaryElementCommand : IRequest<Result<int>>
{
    public string ElementNameAr { get; set; } = string.Empty;
    public string ElementType { get; set; } = "EARNING"; // EARNING, DEDUCTION
    public bool IsTaxable { get; set; }
    public bool IsGosiBase { get; set; }
    public bool IsRecurring { get; set; }
    public bool IsBasic { get; set; } // NEW
    public decimal? DefaultPercentage { get; set; }
}

public class CreateSalaryElementCommandHandler : IRequestHandler<CreateSalaryElementCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateSalaryElementCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<int>> Handle(CreateSalaryElementCommand request, CancellationToken cancellationToken)
    {
        if (request.ElementType != "EARNING" && request.ElementType != "DEDUCTION")
            return Result<int>.Failure("نوع العنصر يجب أن يكون EARNING أو DEDUCTION");

        var element = new SalaryElement
        {
            ElementNameAr = request.ElementNameAr,
            ElementType = request.ElementType,
            IsTaxable = request.IsTaxable ? (byte)1 : (byte)0,
            IsGosiBase = request.IsGosiBase ? (byte)1 : (byte)0,
            IsRecurring = request.IsRecurring ? (byte)1 : (byte)0,
            IsBasic = request.IsBasic ? (byte)1 : (byte)0,
            DefaultPercentage = request.DefaultPercentage
        };

        if (request.IsBasic)
        {
            var existingBasic = await _context.SalaryElements.AnyAsync(e => e.IsBasic == 1, cancellationToken);
            if (existingBasic) return Result<int>.Failure("يوجد بالفعل عنصر راتب أساسي. لا يمكن إضافة أكثر من عنصر أساسي واحد.");
        }

        _context.SalaryElements.Add(element);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(element.ElementId, "تم إضافة العنصر بنجاح");
    }
}
