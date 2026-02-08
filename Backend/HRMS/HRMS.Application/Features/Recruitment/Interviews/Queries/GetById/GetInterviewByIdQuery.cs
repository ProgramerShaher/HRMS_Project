using AutoMapper;
using HRMS.Application.DTOs.Recruitment;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Interviews.Queries.GetById;

/// <summary>
/// Get Interview By ID Query
/// </summary>
public class GetInterviewByIdQuery : IRequest<Result<InterviewDto>>
{
    public int InterviewId { get; set; }
}

/// <summary>
/// Get Interview By ID Query Handler
/// </summary>
public class GetInterviewByIdQueryHandler : IRequestHandler<GetInterviewByIdQuery, Result<InterviewDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetInterviewByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<InterviewDto>> Handle(GetInterviewByIdQuery request, CancellationToken cancellationToken)
    {
        // جلب المقابلة مع كافة البيانات المرتبطة
        var interview = await _context.Interviews
            .Include(i => i.Application)
                .ThenInclude(a => a.Candidate)
            .Include(i => i.Application)
                .ThenInclude(a => a.Vacancy)
                    .ThenInclude(v => v.Job)
            .Include(i => i.Interviewer)
            .FirstOrDefaultAsync(i => i.InterviewId == request.InterviewId && i.IsDeleted == 0, cancellationToken);

        if (interview == null)
            return Result<InterviewDto>.Failure("المقابلة غير موجودة");

        var dto = _mapper.Map<InterviewDto>(interview);

        return Result<InterviewDto>.Success(dto);
    }
}
