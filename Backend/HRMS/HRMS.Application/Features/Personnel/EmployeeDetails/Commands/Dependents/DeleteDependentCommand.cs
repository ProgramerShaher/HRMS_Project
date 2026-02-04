using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Dependents;

public class DeleteDependentCommand : IRequest<Result<bool>>
{
    public int EmployeeId { get; set; }
    public int DependentId { get; set; }
}

public class DeleteDependentCommandHandler : IRequestHandler<DeleteDependentCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteDependentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteDependentCommand request, CancellationToken cancellationToken)
    {
        var dependent = await _context.Dependents
            .FirstOrDefaultAsync(d => d.DependentId == request.DependentId && d.EmployeeId == request.EmployeeId, cancellationToken);

        if (dependent == null)
            return Result<bool>.Failure("Dependent not found");

        _context.Dependents.Remove(dependent);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Dependent deleted successfully");
    }
}
