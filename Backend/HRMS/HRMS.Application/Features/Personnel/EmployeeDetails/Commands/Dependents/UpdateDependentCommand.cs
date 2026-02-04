using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Dependents;

public class UpdateDependentCommand : IRequest<Result<bool>>
{
    public int DependentId { get; set; }
    public int EmployeeId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string Relation { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public bool HasMedicalInsurance { get; set; }
}

public class UpdateDependentCommandHandler : IRequestHandler<UpdateDependentCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateDependentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateDependentCommand request, CancellationToken cancellationToken)
    {
        var dependent = await _context.Dependents
            .FirstOrDefaultAsync(d => d.DependentId == request.DependentId && d.EmployeeId == request.EmployeeId, cancellationToken);

        if (dependent == null)
            return Result<bool>.Failure("Dependent not found");

        dependent.NameAr = request.NameAr;
        dependent.Relationship = request.Relation;
        dependent.NationalId = request.NationalId;
        dependent.BirthDate = request.BirthDate;
        dependent.IsEligibleForInsurance = request.HasMedicalInsurance ? (byte)1 : (byte)0;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Dependent updated successfully");
    }
}
