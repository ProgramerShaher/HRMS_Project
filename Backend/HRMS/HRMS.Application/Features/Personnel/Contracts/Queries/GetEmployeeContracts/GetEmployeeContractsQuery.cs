using AutoMapper;
using AutoMapper.QueryableExtensions;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.Contracts.Queries.GetEmployeeContracts;

public record GetEmployeeContractsQuery(int EmployeeId) : IRequest<List<ContractDto>>;

public class GetEmployeeContractsQueryHandler : IRequestHandler<GetEmployeeContractsQuery, List<ContractDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeContractsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ContractDto>> Handle(GetEmployeeContractsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Contracts
            .Include(c => c.Employee)
                .ThenInclude(e => e.Job)
                    .ThenInclude(j => j.DefaultGrade)
            .Where(c => c.EmployeeId == request.EmployeeId)
            .Select(c => new ContractDto
            {
                ContractId = c.ContractId,
                EmployeeId = c.EmployeeId,
                ContractType = c.ContractType,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                IsRenewable = c.IsRenewable,
                BasicSalary = c.BasicSalary,
                HousingAllowance = c.HousingAllowance,
                TransportAllowance = c.TransportAllowance,
                OtherAllowances = c.OtherAllowances,
                VacationDays = c.VacationDays,
                WorkingHoursDaily = c.WorkingHoursDaily,
                ContractStatus = c.ContractStatus,
                JobGradeAr = c.Employee.Job.DefaultGrade.GradeNameAr,
                IsActive = c.ContractStatus == "ACTIVE" && (c.EndDate == null || c.EndDate >= DateTime.Now)
            })
            .ToListAsync(cancellationToken);
    }
}
