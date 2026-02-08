using AutoMapper;
using HRMS.Application.DTOs.Payroll;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Loans.Queries.GetById;

/// <summary>
/// Get Loan By ID Query
/// </summary>
public class GetLoanByIdQuery : IRequest<Result<LoanDto>>
{
    public int LoanId { get; set; }
}

/// <summary>
/// Get Loan By ID Query Handler
/// </summary>
public class GetLoanByIdQueryHandler : IRequestHandler<GetLoanByIdQuery, Result<LoanDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetLoanByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<LoanDto>> Handle(GetLoanByIdQuery request, CancellationToken cancellationToken)
    {
        // جلب القرض مع بيانات الموظف والأقساط
        var loan = await _context.Loans
            .Include(l => l.Employee)
            .Include(l => l.Installments.OrderBy(i => i.InstallmentNumber))
            .FirstOrDefaultAsync(l => l.LoanId == request.LoanId && l.IsDeleted == 0, cancellationToken);

        if (loan == null)
            return Result<LoanDto>.Failure("القرض غير موجود");

        var dto = _mapper.Map<LoanDto>(loan);

        return Result<LoanDto>.Success(dto);
    }
}
