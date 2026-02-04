using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Payroll;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Configuration.Structure.Commands.SetEmployeeSalaryStructure;

public class EmployeeStructureItemInput
{
    public int ElementId { get; set; }
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
}

public class SetEmployeeSalaryStructureCommand : IRequest<Result<bool>>
{
    public int EmployeeId { get; set; }
    public List<EmployeeStructureItemInput> Items { get; set; } = new();
}

public class SetEmployeeSalaryStructureCommandHandler : IRequestHandler<SetEmployeeSalaryStructureCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public SetEmployeeSalaryStructureCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(SetEmployeeSalaryStructureCommand request, CancellationToken cancellationToken)
    {
        // 1. Validation: Check for duplicates
        if (request.Items.GroupBy(x => x.ElementId).Any(g => g.Count() > 1))
            return Result<bool>.Failure("لا يمكن إضافة نفس العنصر مرتين للموظف");

        // 2. Fetch existing Active Structure
        var existingStructures = await _context.SalaryStructures
            .Where(s => s.EmployeeId == request.EmployeeId)
            .ToListAsync(cancellationToken);

        // 3. Remove Old Structure (Physical Delete or Soft Delete - converting to Inactive is safer but requirements imply 'Set')
        // For simplicity and 'Set' semantics, we will remove existing and add new ones to avoid drift.
        // OR better: Update existing matches, Add new, Remove missing.
        // Let's go with: Remove all active for this employee and re-add.
        _context.SalaryStructures.RemoveRange(existingStructures);
        
        // 4. Calculate Amounts from Percentages if needed
        // New Logic: Fetch all elements involved to find the Basic Salary element
        var elementIds = request.Items.Select(i => i.ElementId).Distinct().ToList();
        var elements = await _context.SalaryElements
            .Where(e => elementIds.Contains(e.ElementId))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var basicElement = elements.FirstOrDefault(e => e.IsBasic == 1);
        decimal basicSalaryAmount = 0;

        // Find the input item corresponding to the Basic Salary Element
        if (basicElement != null)
        {
            var basicItem = request.Items.FirstOrDefault(i => i.ElementId == basicElement.ElementId);
            if (basicItem != null)
            {
                basicSalaryAmount = basicItem.Amount;
            }
        }

        // Error Handling: If any item needs percentage calc but Basic Salary is missing/zero
        if (request.Items.Any(i => i.Percentage > 0) && basicSalaryAmount <= 0)
        {
             return Result<bool>.Failure("يجب تحديد الراتب الأساسي وقيمته لحساب البدلات المئوية.");
        }

        // --- Validation: Job Grade Limits ---
        var employee = await _context.Employees
            .Include(e => e.Job)
            .ThenInclude(j => j.DefaultGrade)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId, cancellationToken);

        // Validate if Employee has a Grade and we identified a Basic Element context
        if (employee?.Job?.DefaultGrade != null && basicElement != null)
        {
            var grade = employee.Job.DefaultGrade;
            // Check if amount is strictly within range (inclusive)
            if (basicSalaryAmount < grade.MinSalary || basicSalaryAmount > grade.MaxSalary)
            {
               return Result<bool>.Failure($"الراتب الأساسي ({basicSalaryAmount}) خارج النطاق المسموح للدرجة الوظيفية ({grade.GradeNameAr}: {grade.MinSalary} - {grade.MaxSalary})");
            }
        }
        // ------------------------------------

        var newStructures = request.Items.Select(item => new EmployeeSalaryStructure
        {
            EmployeeId = request.EmployeeId,
            ElementId = item.ElementId,
            Amount = item.Amount > 0 ? item.Amount : (basicSalaryAmount * item.Percentage / 100), 
            Percentage = item.Percentage,
            IsActive = 1
        }).ToList();

        _context.SalaryStructures.AddRange(newStructures);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم تحديث هيكل الراتب بنجاح");
    }
}
