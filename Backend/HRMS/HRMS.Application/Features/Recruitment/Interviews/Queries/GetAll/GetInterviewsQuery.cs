using AutoMapper;
using HRMS.Application.DTOs.Recruitment;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Interviews.Queries.GetAll;

/// <summary>
/// استعلام للحصول على قائمة المقابلات
/// </summary>
public class GetInterviewsQuery : IRequest<Result<List<InterviewDto>>>
{
    /// <summary>
    /// تصفية حسب طلب التوظيف
    /// </summary>
    public int? AppId { get; set; }

    /// <summary>
    /// تصفية حسب المُقابِل
    /// </summary>
    public int? InterviewerId { get; set; }
}

public class GetInterviewsQueryHandler : IRequestHandler<GetInterviewsQuery, Result<List<InterviewDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetInterviewsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<InterviewDto>>> Handle(GetInterviewsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Interviews
            .Include(i => i.Application)
                .ThenInclude(a => a.Candidate)
            .Include(i => i.Interviewer)
            .AsQueryable();

        // تطبيق الفلاتر
        if (request.AppId.HasValue)
        {
            query = query.Where(i => i.AppId == request.AppId.Value);
        }

        if (request.InterviewerId.HasValue)
        {
            query = query.Where(i => i.InterviewerId == request.InterviewerId.Value);
        }

        var interviews = await query
            .OrderBy(i => i.ScheduledTime)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<InterviewDto>>(interviews);

        return Result<List<InterviewDto>>.Success(dtos);
    }
}
