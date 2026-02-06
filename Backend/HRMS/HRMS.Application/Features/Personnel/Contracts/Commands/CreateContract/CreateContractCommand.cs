using AutoMapper;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Features.Personnel.Contracts.Helpers;
using HRMS.Application.Interfaces;
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
/// <remarks>
/// القواعد المطبقة:
/// 1. التحقق من نطاق الراتب حسب الدرجة الوظيفية
/// 2. ضمان عقد نشط واحد فقط لكل موظف
/// 3. مزامنة كاملة لجميع مكونات العقد مع هيكل الراتب (Basic, Housing, Transport, Other Allowances)
/// 4. استخدام Transaction لضمان سلامة البيانات
/// </remarks>
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
        // STEP 3: Single Active Contract Rule - REJECT if exists
        // ═══════════════════════════════════════════════════════════
        var hasActiveContract = await _context.Contracts
            .AnyAsync(c => c.EmployeeId == request.Data.EmployeeId && c.ContractStatus == "ACTIVE", cancellationToken);

        if (hasActiveContract)
        {
            return Result<int>.Failure(
                $"لا يمكن إضافة عقد جديد. الموظف رقم {request.Data.EmployeeId} لديه عقد نشط بالفعل. " +
                "يرجى إنهاء العقد الحالي أولاً أو استخدام خاصية التجديد."
            );
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
            contract.ContractStatus = "ACTIVE";
            _context.Contracts.Add(contract);

            // ═══════════════════════════════════════════════════════════
            // STEP 6: Sync EmployeeCompensation (Backward Compatibility)
            // ═══════════════════════════════════════════════════════════
            var compensation = await _context.EmployeeCompensations
                .FirstOrDefaultAsync(c => c.EmployeeId == request.Data.EmployeeId, cancellationToken);

            if (compensation == null)
            {
                compensation = new EmployeeCompensation
                {
                    EmployeeId = request.Data.EmployeeId,
                    BasicSalary = request.Data.BasicSalary,
                    HousingAllowance = request.Data.HousingAllowance,
                    TransportAllowance = request.Data.TransportAllowance,
                    OtherAllowances = request.Data.OtherAllowances
                };
                _context.EmployeeCompensations.Add(compensation);
            }
            else
            {
                compensation.BasicSalary = request.Data.BasicSalary;
                compensation.HousingAllowance = request.Data.HousingAllowance;
                compensation.TransportAllowance = request.Data.TransportAllowance;
                compensation.OtherAllowances = request.Data.OtherAllowances;
            }

            // ═══════════════════════════════════════════════════════════
            // STEP 7: ✅ COMPLETE Financial Sync - ALL Contract Components
            // Syncs: Basic Salary, Housing, Transport, Medical, Other Allowances
            // ═══════════════════════════════════════════════════════════
            await SalaryStructureSyncHelper.SyncAllContractComponentsAsync(
                _context,
                request.Data.EmployeeId,
                basicSalary: request.Data.BasicSalary,
                housingAllowance: request.Data.HousingAllowance,
                transportAllowance: request.Data.TransportAllowance,
                medicalAllowance: 0, // Not in contract DTO
                otherAllowances: request.Data.OtherAllowances, // ✅ Syncs Other Allowances
                cancellationToken
            );

            // ═══════════════════════════════════════════════════════════
            // STEP 8: Save All Changes and Commit Transaction
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
}
