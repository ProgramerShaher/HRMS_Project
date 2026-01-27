using MediatR;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Core;

namespace HRMS.Application.Features.Core.Cities.Commands.CreateCity;

/// <summary>
/// معالج أمر إنشاء مدينة جديدة
/// </summary>
/// <remarks>
/// يقوم بإضافة مدينة جديدة إلى قاعدة البيانات بعد التحقق من صحة البيانات
/// </remarks>
public class CreateCityCommandHandler : IRequestHandler<CreateCityCommand, int>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// المنشئ
    /// </summary>
    /// <param name="context">سياق قاعدة البيانات</param>
    public CreateCityCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// معالجة أمر إنشاء المدينة
    /// </summary>
    /// <param name="request">بيانات الأمر</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>معرف المدينة الجديدة</returns>
    public async Task<int> Handle(CreateCityCommand request, CancellationToken cancellationToken)
    {
        var city = new City
        {
            CountryId = request.CountryId,
            CityNameAr = request.CityNameAr,
            CityNameEn = request.CityNameEn,
            CreatedAt = DateTime.UtcNow
        };

        _context.Cities.Add(city);
        await _context.SaveChangesAsync(cancellationToken);

        return city.CityId;
    }
}
