using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Queries.BankAccounts;

public class GetEmployeeBankAccountsQuery : IRequest<Result<List<EmployeeBankAccountDto>>>
{
    public int EmployeeId { get; set; }
}

public class GetEmployeeBankAccountsQueryHandler : IRequestHandler<GetEmployeeBankAccountsQuery, Result<List<EmployeeBankAccountDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeBankAccountsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<EmployeeBankAccountDto>>> Handle(GetEmployeeBankAccountsQuery request, CancellationToken cancellationToken)
    {
        var accounts = await _context.EmployeeBankAccounts
            .Include(b => b.Bank)
            .Where(b => b.EmployeeId == request.EmployeeId)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<EmployeeBankAccountDto>>(accounts);
        return Result<List<EmployeeBankAccountDto>>.Success(dtos);
    }
}
