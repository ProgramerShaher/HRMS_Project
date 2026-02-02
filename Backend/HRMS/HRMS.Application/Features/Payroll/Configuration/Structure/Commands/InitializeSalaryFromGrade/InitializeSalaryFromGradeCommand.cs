using HRMS.Application.Features.Payroll.Configuration.Structure.Commands.SetEmployeeSalaryStructure;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using HRMS.Core.Utilities.Payroll;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Configuration.Structure.Commands.InitializeSalaryFromGrade;

public class InitializeSalaryFromGradeCommand : IRequest<Result<bool>>
{
    public int EmployeeId { get; set; }
}

public class InitializeSalaryFromGradeCommandHandler : IRequestHandler<InitializeSalaryFromGradeCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public InitializeSalaryFromGradeCommandHandler(IApplicationDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<Result<bool>> Handle(InitializeSalaryFromGradeCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch Employee and Grade
        var employee = await _context.Employees
            .Include(e => e.Job)
            .ThenInclude(j => j.DefaultGrade)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId, cancellationToken);

        if (employee == null) return Result<bool>.Failure("الموظف غير موجود");
        if (employee.Job == null) return Result<bool>.Failure("الموظف غير مرتبط بوظيفة");
        if (employee.Job.DefaultGrade == null) return Result<bool>.Failure("الوظيفة غير مرتبطة بدرجة وظيفية");

        var grade = employee.Job.DefaultGrade;

        // 2. Prepare Items List
        var items = new List<EmployeeStructureItemInput>();
        
        // Find the Basic Salary Element in the system
        var basicElement = await _context.SalaryElements.FirstOrDefaultAsync(e => e.IsBasic == 1, cancellationToken);
        if (basicElement == null) return Result<bool>.Failure("لم يتم تعريف الراتب الأساسي في النظام (IsBasic=1). يرجى ضبط الإعدادات.");

        // A. Basic Salary (From MinSalary)
        items.Add(new EmployeeStructureItemInput
        {
            ElementId = basicElement.ElementId, 
            Amount = grade.MinSalary,
            Percentage = 0
        });

        // B. Parse Benefits Config
        var specificBenefits = BenefitsParser.Parse(grade.BenefitsConfig);
        foreach (var benefit in specificBenefits)
        {
            // Skip Basic if defined in JSON to avoid duplication
            if (benefit.ElementId == basicElement.ElementId) continue; 

            items.Add(new EmployeeStructureItemInput
            {
                ElementId = benefit.ElementId,
                Amount = benefit.Amount ?? 0,
                Percentage = benefit.Percentage ?? 0
            });
        }

        // C. Add Default Recurring Elements (if not already added)
        // e.g., Transport, Social Insurance if clearly defined in system defaults not just Grade
        // For now, we rely on Grade Configuration.

        // 3. Delegation: Call SetCommand
        // We reuse the logic in SetCommand (which handles Validation and Saving)
        var setCommand = new SetEmployeeSalaryStructureCommand
        {
            EmployeeId = request.EmployeeId,
            Items = items
        };

        return await _mediator.Send(setCommand, cancellationToken);
    }
}
