using MediatR;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Core.Countries.Commands.DeleteCountry;

/// <summary>
/// معالج أمر حذف الدولة
/// </summary>
public class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteCountryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
    {
        // البحث عن الدولة
        var country = await _context.Countries
            .Include(c => c.Cities) // تحميل المدن للتحقق
            .FirstOrDefaultAsync(c => c.CountryId == request.CountryId, cancellationToken);

        if (country == null)
            throw new KeyNotFoundException($"الدولة برقم {request.CountryId} غير موجودة");

        // منع الحذف إذا كانت هناك مدن تابعة
        if (country.Cities.Any())
        {
            throw new InvalidOperationException(
                $"لا يمكن حذف الدولة '{country.CountryNameAr}' لأنها تحتوي على {country.Cities.Count} مدينة. " +
                "يجب حذف المدن أولاً.");
        }

        // الحذف
        _context.Countries.Remove(country);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
