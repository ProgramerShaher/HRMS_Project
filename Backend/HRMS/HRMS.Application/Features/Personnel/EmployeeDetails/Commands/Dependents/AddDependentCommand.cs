using FluentValidation;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Dependents;

public class AddDependentCommand : IRequest<Result<int>>
{
    public int EmployeeId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string Relation { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public bool HasMedicalInsurance { get; set; }
}

public class AddDependentCommandHandler : IRequestHandler<AddDependentCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public AddDependentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(AddDependentCommand request, CancellationToken cancellationToken)
    {
        var dependent = new Dependent
        {
            EmployeeId = request.EmployeeId,
            NameAr = request.NameAr,
            Relationship = request.Relation,
            NationalId = request.NationalId,
            BirthDate = request.BirthDate,
            IsEligibleForInsurance = request.HasMedicalInsurance ? (byte)1 : (byte)0
        };

        _context.Dependents.Add(dependent);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(dependent.DependentId, "Dependent added successfully");
    }
}

public class AddDependentCommandValidator : AbstractValidator<AddDependentCommand>
{
    public AddDependentCommandValidator()
    {
        RuleFor(x => x.EmployeeId).GreaterThan(0);
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Relation).NotEmpty();
        // NationalId might be optional
    }
}
