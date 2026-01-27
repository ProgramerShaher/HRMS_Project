using AutoMapper;
using AutoMapper.QueryableExtensions;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Queries.Qualifications;

public record GetEmployeeQualificationsQuery(int EmployeeId) : IRequest<List<EmployeeQualificationDto>>;

public class GetEmployeeQualificationsQueryHandler : IRequestHandler<GetEmployeeQualificationsQuery, List<EmployeeQualificationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeQualificationsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<EmployeeQualificationDto>> Handle(GetEmployeeQualificationsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Qualifications
            .Where(q => q.EmployeeId == request.EmployeeId)
            .ProjectTo<EmployeeQualificationDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
