using AutoMapper;
using HRMS.Application.DTOs.Recruitment;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Applications.Queries.GetById;

/// <summary>
/// Get Job Application By ID Query
/// </summary>
public class GetApplicationByIdQuery : IRequest<Result<JobApplicationDto>>
{
    public int AppId { get; set; }
}

/// <summary>
/// Get Application By ID Query Handler
/// </summary>
public class GetApplicationByIdQueryHandler : IRequestHandler<GetApplicationByIdQuery, Result<JobApplicationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetApplicationByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<JobApplicationDto>> Handle(GetApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        // جلب طلب التوظيف مع كافة البيانات المرتبطة
        var application = await _context.JobApplications
            .Include(a => a.Candidate)
            .Include(a => a.Vacancy)
                .ThenInclude(v => v.Job)
            .Include(a => a.Vacancy)
                .ThenInclude(v => v.Department)
            .FirstOrDefaultAsync(a => a.AppId == request.AppId && a.IsDeleted == 0, cancellationToken);

        if (application == null)
            return Result<JobApplicationDto>.Failure("طلب التوظيف غير موجود");

        var dto = _mapper.Map<JobApplicationDto>(application);

        return Result<JobApplicationDto>.Success(dto);
    }
}
