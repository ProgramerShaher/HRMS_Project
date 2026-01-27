using AutoMapper;
using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.PublicHolidays.Queries.GetAllPublicHolidays;

// 1. Query
/// <summary>
/// استعلام جلب كافة العطل الرسمية لسنة معينة (أو السنة الحالية)
/// </summary>
public record GetAllPublicHolidaysQuery(short? Year) : IRequest<List<PublicHolidayDto>>;

// 2. Handler
public class GetAllPublicHolidaysQueryHandler : IRequestHandler<GetAllPublicHolidaysQuery, List<PublicHolidayDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllPublicHolidaysQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<PublicHolidayDto>> Handle(GetAllPublicHolidaysQuery request, CancellationToken cancellationToken)
    {
        var targetYear = request.Year ?? (short)DateTime.Now.Year;
        
        var list = await _context.PublicHolidays
            .AsNoTracking()
            .Where(h => h.Year == targetYear)
            .OrderBy(h => h.StartDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<PublicHolidayDto>>(list);
    }
}
