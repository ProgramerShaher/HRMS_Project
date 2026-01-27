using AutoMapper;
using AutoMapper.QueryableExtensions;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Queries.Experiences;

public record GetEmployeeExperiencesQuery(int EmployeeId) : IRequest<List<EmployeeExperienceDto>>;

public class GetEmployeeExperiencesQueryHandler : IRequestHandler<GetEmployeeExperiencesQuery, List<EmployeeExperienceDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeExperiencesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<EmployeeExperienceDto>> Handle(GetEmployeeExperiencesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Experiences
            .Where(e => e.EmployeeId == request.EmployeeId)
            .ProjectTo<EmployeeExperienceDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
