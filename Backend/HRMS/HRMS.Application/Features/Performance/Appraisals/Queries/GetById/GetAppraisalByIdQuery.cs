using AutoMapper;
using HRMS.Application.DTOs.Performance;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.Appraisals.Queries.GetById;

public class GetAppraisalByIdQuery : IRequest<Result<EmployeeAppraisalDto>>
{
    public int AppraisalId { get; set; }

    public GetAppraisalByIdQuery(int id)
    {
        AppraisalId = id;
    }
}

public class GetAppraisalByIdQueryHandler : IRequestHandler<GetAppraisalByIdQuery, Result<EmployeeAppraisalDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAppraisalByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<EmployeeAppraisalDto>> Handle(GetAppraisalByIdQuery request, CancellationToken cancellationToken)
    {
        var appraisal = await _context.EmployeeAppraisals
            .Include(a => a.Employee)
            .Include(a => a.Cycle)
            .Include(a => a.Employee)
            .Include(a => a.Details)
                .ThenInclude(d => d.Kpi)
            .FirstOrDefaultAsync(a => a.AppraisalId == request.AppraisalId && a.IsDeleted == 0, cancellationToken);

        if (appraisal == null)
            return Result<EmployeeAppraisalDto>.Failure("التقييم غير موجود");

        var dto = _mapper.Map<EmployeeAppraisalDto>(appraisal);

        return Result<EmployeeAppraisalDto>.Success(dto);
    }
}
