using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Configuration.Elements.Commands.DeleteSalaryElement;

public record DeleteSalaryElementCommand(int Id) : IRequest<Result<bool>>;

public class DeleteSalaryElementCommandHandler : IRequestHandler<DeleteSalaryElementCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteSalaryElementCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteSalaryElementCommand request, CancellationToken cancellationToken)
    {
        var element = await _context.SalaryElements
            .Include(e => e.EmployeeStructures)
            .FirstOrDefaultAsync(e => e.ElementId == request.Id, cancellationToken);

        if (element == null)
            return Result<bool>.Failure("العنصر غير موجود");

        if (element.EmployeeStructures.Any())
            return Result<bool>.Failure("لا يمكن حذف العنصر لأنه مرتبط بهياكل رواتب موظفين");

        _context.SalaryElements.Remove(element);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم حذف العنصر بنجاح");
    }
}
