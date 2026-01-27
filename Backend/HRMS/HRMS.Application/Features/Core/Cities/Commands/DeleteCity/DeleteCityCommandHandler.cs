using MediatR;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Core.Cities.Commands.DeleteCity;

public class DeleteCityCommandHandler : IRequestHandler<DeleteCityCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteCityCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteCityCommand request, CancellationToken cancellationToken)
    {
        var city = await _context.Cities
            .FirstOrDefaultAsync(c => c.CityId == request.CityId, cancellationToken);

        if (city == null)
            throw new KeyNotFoundException($"المدينة برقم {request.CityId} غير موجودة");

        _context.Cities.Remove(city);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
