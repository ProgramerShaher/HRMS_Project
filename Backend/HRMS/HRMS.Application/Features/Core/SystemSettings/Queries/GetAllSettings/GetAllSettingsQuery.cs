using AutoMapper;
using HRMS.Application.DTOs.Core;
using HRMS.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Core.SystemSettings.Queries.GetAllSettings;

// 1. Query
/// <summary>
/// استعلام جلب كافة إعدادات النظام النشطة
/// </summary>
public record GetAllSettingsQuery : IRequest<List<SystemSettingDto>>;

// 2. Handler
public class GetAllSettingsQueryHandler : IRequestHandler<GetAllSettingsQuery, List<SystemSettingDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllSettingsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<SystemSettingDto>> Handle(GetAllSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await _context.SystemSettings
            .AsNoTracking()
            .Where(s => s.IsDeleted == 0)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<SystemSettingDto>>(settings);
    }
}
