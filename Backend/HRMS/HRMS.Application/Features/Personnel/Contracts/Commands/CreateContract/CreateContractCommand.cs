using AutoMapper;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Payroll;
using HRMS.Core.Entities.Personnel;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.Contracts.Commands.CreateContract;

/// <summary>
/// أمر إنشاء عقد جديد للموظف
/// </summary>
public record CreateContractCommand(CreateContractDto Data) : IRequest<Result<int>>;

/// <summary>
/// معالج أمر إنشاء العقد مع تطبيق قواعد العمل الصارمة والمزامنة المالية الكاملة
/// </summary>
public class CreateContractCommandHandler : IRequestHandler<CreateContractCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateContractCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<int>> Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════
        // STEP 1: Validation - Employee Existence
        // ═══════════════════════════════════════════════════════════
        var employee = await _context.Employees
            .Include(e => e.Job)
                .ThenInclude(j => j.DefaultGrade)
            .FirstOrDefaultAsync(e => e.EmployeeId == request.Data.EmployeeId, cancellationToken);

        if (employee == null)
        {
            return Result<int>.Failure($"الموظف رقم {request.Data.EmployeeId} غير موجود.");
        }

        // ═══════════════════════════════════════════════════════════
        // STEP 2: Salary Range Validation Against Job Grade
        // ═══════════════════════════════════════════════════════════
        if (employee.Job?.DefaultGrade != null)
        {
            var grade = employee.Job.DefaultGrade;
            
            if (request.Data.BasicSalary < grade.MinSalary || request.Data.BasicSalary > grade.MaxSalary)
            {
                return Result<int>.Failure(
                    $"الراتب الأساسي ({request.Data.BasicSalary:N2}) خارج نطاق الدرجة الوظيفية " +
                    $"({grade.GradeNameAr}). النطاق المسموح: {grade.MinSalary:N2} - {grade.MaxSalary:N2}"
                );
            }
        }

        // ═══════════════════════════════════════════════════════════
        // STEP 3: Single Active Contract Rule - Check & Expire exists
        // ═══════════════════════════════════════════════════════════
        var activeContracts = await _context.Contracts
            .Where(c => c.EmployeeId == request.Data.EmployeeId && c.ContractStatus == "ACTIVE")
            .ToListAsync(cancellationToken);

        // Auto-expire existing active contracts instead of rejecting (Requirement Update)
        foreach (var activeContract in activeContracts)
        {
            activeContract.ContractStatus = "EXPIRED";
            activeContract.EndDate = request.Data.StartDate.AddDays(-1); // End yesterday
            _context.Contracts.Update(activeContract);
        }

        // ═══════════════════════════════════════════════════════════
        // STEP 4: Start Database Transaction
        // ═══════════════════════════════════════════════════════════
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // ═══════════════════════════════════════════════════════════
            // STEP 5: Create New Contract
            // ═══════════════════════════════════════════════════════════
            var contract = _mapper.Map<Contract>(request.Data);
            contract.ContractStatus = "ACTIVE"; // Validated status
            _context.Contracts.Add(contract);

            // ═══════════════════════════════════════════════════════════
            // STEP 6: Execute Private Sync Logic (Financial Integration)
            // ═══════════════════════════════════════════════════════════
            await SyncSalaryStructure(request.Data, cancellationToken);

            // ═══════════════════════════════════════════════════════════
            // STEP 7: Commit Transaction
            // ═══════════════════════════════════════════════════════════
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<int>.Success(contract.ContractId);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<int>.Failure($"فشل في إنشاء العقد: {ex.Message}");
        }
    }

    /// <summary>
    /// Private Helper to Sync Salary Structure Elements directly from Contract Data
    /// </summary>
    private async Task SyncSalaryStructure(CreateContractDto data, CancellationToken cancellationToken)
    {
        // 1. Clean Up: Remove existing structure for this employee to avoid duplicates
        var existingStructure = await _context.EmployeeSalaryStructures
            .Where(s => s.EmployeeId == data.EmployeeId)
            .ToListAsync(cancellationToken);

        if (existingStructure.Any())
        {
            _context.EmployeeSalaryStructures.RemoveRange(existingStructure);
        }

        // 2. Fetch necessary Salary Elements from DB (Dynamic Lookup)
        var elements = await _context.SalaryElements
            .Where(e => e.IsBasic == 1 
                     || e.ElementNameAr.Contains("سكن") 
                     || e.ElementNameAr.Contains("نقل") 
                     || e.ElementNameAr.Contains("تأمينات") || e.ElementNameAr.Contains("GOSI")
                     || e.ElementType == "EARNING") // General fetch to filter in memory if strictly needed, but let's be specific
            .ToListAsync(cancellationToken);

        var basicElement = elements.FirstOrDefault(e => e.IsBasic == 1) 
                           ?? throw new Exception("عنصر الراتب الأساسي غير معرف في النظام");
        
        var housingElement = elements.FirstOrDefault(e => e.ElementNameAr.Contains("سكن"));
        var transportElement = elements.FirstOrDefault(e => e.ElementNameAr.Contains("نقل"));
        var otherElement = elements.FirstOrDefault(e => e.ElementNameAr.Contains("أخرى") || e.ElementNameAr.Contains("Other"));
        var gosiElement = elements.FirstOrDefault(e => e.ElementNameAr.Contains("تأمينات") || e.ElementNameAr.Contains("GOSI"));

        var newStructures = new List<EmployeeSalaryStructure>();

        // 3. Insert Basic Salary
        newStructures.Add(new EmployeeSalaryStructure
        {
            EmployeeId = data.EmployeeId,
            ElementId = basicElement.ElementId,
            Amount = data.BasicSalary,
            IsActive = 1
        });

        // 4. Insert Housing Allowance (if > 0)
        if (data.HousingAllowance > 0 && housingElement != null)
        {
            newStructures.Add(new EmployeeSalaryStructure
            {
                EmployeeId = data.EmployeeId,
                ElementId = housingElement.ElementId,
                Amount = data.HousingAllowance,
                IsActive = 1
            });
        }

        // 5. Insert Transport Allowance (if > 0)
        if (data.TransportAllowance > 0 && transportElement != null)
        {
            newStructures.Add(new EmployeeSalaryStructure
            {
                EmployeeId = data.EmployeeId,
                ElementId = transportElement.ElementId,
                Amount = data.TransportAllowance,
                IsActive = 1
            });
        }

        // 6. Insert Other Allowances (if > 0)
        if (data.OtherAllowances > 0 && otherElement != null)
        {
            newStructures.Add(new EmployeeSalaryStructure
            {
                EmployeeId = data.EmployeeId,
                ElementId = otherElement.ElementId,
                Amount = data.OtherAllowances,
                IsActive = 1
            });
        }

        // 7. Auto-Calculate GOSI Deduction (9% of Basic)
        // Assume GOSI is a deduction element
        if (gosiElement != null)
        {
            var gosiAmount = data.BasicSalary * 0.09m; // 9% Rule
            newStructures.Add(new EmployeeSalaryStructure
            {
                EmployeeId = data.EmployeeId,
                ElementId = gosiElement.ElementId,
                Amount = gosiAmount,
                IsActive = 1
            });
        }

        // 8. Add range to context
        await _context.EmployeeSalaryStructures.AddRangeAsync(newStructures, cancellationToken);
    }
}
