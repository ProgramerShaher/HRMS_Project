using AutoMapper;
using HRMS.Application.DTOs.Performance;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.KpiLibrary.Queries.GetAll;

public class GetKpisQuery : IRequest<Result<List<KpiDto>>>
{
    /// <summary>اختياري: تصفية حسب الفئة الوظيفية (NURSE, ADMIN, DOCTOR, ALL)</summary>
    public string? TargetJobType { get; set; }
}

public class GetKpisQueryHandler : IRequestHandler<GetKpisQuery, Result<List<KpiDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetKpisQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<KpiDto>>> Handle(GetKpisQuery request, CancellationToken cancellationToken)
    {
        var query = _context.KpiLibraries
            .Where(k => k.IsDeleted == 0) // ✅ FIX: تصفية المحذوفين
            .AsQueryable();

        // تصفية حسب نوع الوظيفة (إذا تم تحديده)
        if (!string.IsNullOrEmpty(request.TargetJobType) && request.TargetJobType != "ALL")
        {
            query = query.Where(k => k.TargetJobType == request.TargetJobType || k.TargetJobType == "ALL");
        }

        var kpis = await query
            .OrderBy(k => k.Category)
            .ThenBy(k => k.KpiNameAr)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<KpiDto>>(kpis);
        return Result<List<KpiDto>>.Success(dtos);
    }
}
