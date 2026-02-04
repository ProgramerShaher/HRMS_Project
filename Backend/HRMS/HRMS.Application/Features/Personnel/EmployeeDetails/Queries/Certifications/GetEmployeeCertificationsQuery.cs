using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Features.Personnel.Employees.DTOs;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Queries.Certifications;

public class GetEmployeeCertificationsQuery : IRequest<Result<List<EmployeeCertificationDto>>>
{
    public int EmployeeId { get; set; }
}

public class GetEmployeeCertificationsQueryHandler : IRequestHandler<GetEmployeeCertificationsQuery, Result<List<EmployeeCertificationDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeCertificationsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<EmployeeCertificationDto>>> Handle(GetEmployeeCertificationsQuery request, CancellationToken cancellationToken)
    {
        var certs = await _context.Certifications
            .Where(c => c.EmployeeId == request.EmployeeId)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<EmployeeCertificationDto>>(certs);
        return Result<List<EmployeeCertificationDto>>.Success(dtos);
    }
}
