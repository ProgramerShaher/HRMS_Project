using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Features.Personnel.Contracts.Helpers;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.Contracts.Commands.RenewContract;

/// <summary>
/// أمر تجديد عقد موظف
/// </summary>
public record RenewContractCommand(RenewContractDto Data) : IRequest<Result<int>>;

/// <summary>
/// معالج أمر تجديد العقد مع تطبيق قواعد العمل الصارمة والمزامنة المالية الكاملة
/// </summary>
/// <remarks>
/// القواعد المطبقة:
/// 1. التحقق من نطاق الراتب حسب الدرجة الوظيفية
/// 2. إنهاء العقد القديم وإنشاء عقد جديد نشط
/// 3. مزامنة كاملة لجميع مكونات العقد مع هيكل الراتب
/// 4. استخدام Transaction لضمان سلامة البيانات
/// </remarks>
public class RenewContractCommandHandler : IRequestHandler<RenewContractCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public RenewContractCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(RenewContractCommand request, CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════
        // STEP 1: Validation - Contract Existence
        // ═══════════════════════════════════════════════════════════
        var oldContract = await _context.Contracts
            .Include(c => c.Employee)
                .ThenInclude(e => e.Job)
                    .ThenInclude(j => j.DefaultGrade)
            .FirstOrDefaultAsync(c => c.ContractId == request.Data.ContractId, cancellationToken);

        if (oldContract == null)
        {
            return Result<int>.Failure($"العقد رقم {request.Data.ContractId} غير موجود.");
        }

        var employee = oldContract.Employee;

        // ═══════════════════════════════════════════════════════════
        // STEP 2: Calculate New Contract Terms
        // ═══════════════════════════════════════════════════════════
        decimal newBasicSalary = request.Data.NewBasicSalary ?? oldContract.BasicSalary;
        decimal newHousingAllowance = request.Data.NewHousingAllowance ?? oldContract.HousingAllowance;
        decimal newTransportAllowance = request.Data.NewTransportAllowance ?? oldContract.TransportAllowance;
        decimal newOtherAllowances = request.Data.NewOtherAllowances ?? oldContract.OtherAllowances;

        // ═══════════════════════════════════════════════════════════
        // STEP 3: Salary Range Validation Against Job Grade
        // ═══════════════════════════════════════════════════════════
        if (employee.Job?.DefaultGrade != null)
        {
            var grade = employee.Job.DefaultGrade;
            
            if (newBasicSalary < grade.MinSalary || newBasicSalary > grade.MaxSalary)
            {
                return Result<int>.Failure(
                    $"الراتب الأساسي ({newBasicSalary:N2}) خارج نطاق الدرجة الوظيفية " +
                    $"({grade.GradeNameAr}). النطاق المسموح: {grade.MinSalary:N2} - {grade.MaxSalary:N2}"
                );
            }
        }

        // ═══════════════════════════════════════════════════════════
        // STEP 4: Start Database Transaction
        // ═══════════════════════════════════════════════════════════
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // ═══════════════════════════════════════════════════════════
            // STEP 5: Expire ALL Active Contracts for this Employee
            // ═══════════════════════════════════════════════════════════
            var activeContracts = await _context.Contracts
                .Where(c => c.EmployeeId == employee.EmployeeId && c.ContractStatus == "ACTIVE")
                .ToListAsync(cancellationToken);

            foreach (var contract in activeContracts)
            {
                contract.ContractStatus = "EXPIRED";
                contract.EndDate = DateTime.Now.AddDays(-1);
            }

            // ═══════════════════════════════════════════════════════════
            // STEP 6: Create Renewal Record
            // ═══════════════════════════════════════════════════════════
            var renewal = new ContractRenewal
            {
                ContractId = request.Data.ContractId,
                OldEndDate = oldContract.EndDate ?? DateTime.MinValue,
                NewStartDate = request.Data.NewStartDate,
                NewEndDate = request.Data.NewEndDate,
                Notes = request.Data.Notes,
                RenewalDate = DateTime.Now
            };
            _context.ContractRenewals.Add(renewal);

            // ═══════════════════════════════════════════════════════════
            // STEP 7: Create NEW Active Contract
            // ═══════════════════════════════════════════════════════════
            var newContract = new Contract
            {
                EmployeeId = employee.EmployeeId,
                ContractType = oldContract.ContractType,
                StartDate = request.Data.NewStartDate,
                EndDate = request.Data.NewEndDate,
                IsRenewable = oldContract.IsRenewable,
                BasicSalary = newBasicSalary,
                HousingAllowance = newHousingAllowance,
                TransportAllowance = newTransportAllowance,
                OtherAllowances = newOtherAllowances,
                VacationDays = oldContract.VacationDays,
                WorkingHoursDaily = oldContract.WorkingHoursDaily,
                ContractStatus = "ACTIVE"
            };
            _context.Contracts.Add(newContract);

            // ═══════════════════════════════════════════════════════════
            // STEP 8: Sync EmployeeCompensation (Backward Compatibility)
            // ═══════════════════════════════════════════════════════════
            var compensation = await _context.EmployeeCompensations
                .FirstOrDefaultAsync(c => c.EmployeeId == employee.EmployeeId, cancellationToken);

            if (compensation == null)
            {
                compensation = new EmployeeCompensation
                {
                    EmployeeId = employee.EmployeeId,
                    BasicSalary = newBasicSalary,
                    HousingAllowance = newHousingAllowance,
                    TransportAllowance = newTransportAllowance,
                    OtherAllowances = newOtherAllowances
                };
                _context.EmployeeCompensations.Add(compensation);
            }
            else
            {
                compensation.BasicSalary = newBasicSalary;
                compensation.HousingAllowance = newHousingAllowance;
                compensation.TransportAllowance = newTransportAllowance;
                compensation.OtherAllowances = newOtherAllowances;
            }

            // ═══════════════════════════════════════════════════════════
            // STEP 9: ✅ COMPLETE Financial Sync - ALL Contract Components
            // ═══════════════════════════════════════════════════════════
            await SalaryStructureSyncHelper.SyncAllContractComponentsAsync(
                _context,
                employee.EmployeeId,
                basicSalary: newBasicSalary,
                housingAllowance: newHousingAllowance,
                transportAllowance: newTransportAllowance,
                medicalAllowance: 0, // Not in renewal DTO
                otherAllowances: newOtherAllowances,
                cancellationToken
            );

            // ═══════════════════════════════════════════════════════════
            // STEP 10: Save All Changes and Commit Transaction
            // ═══════════════════════════════════════════════════════════
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<int>.Success(renewal.RenewalId);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<int>.Failure($"فشل في تجديد العقد: {ex.Message}");
        }
    }
}
