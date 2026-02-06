using HRMS.Application.Features.Personnel.Contracts.Helpers;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.Contracts.Commands.SyncExistingContracts;

/// <summary>
/// أمر لمزامنة جميع العقود النشطة الموجودة مع هيكل الراتب
/// </summary>
/// <remarks>
/// يستخدم هذا الأمر لمرة واحدة لمزامنة العقود التي تم إنشاؤها قبل تطبيق نظام المزامنة التلقائية
/// </remarks>
public record SyncExistingContractsCommand : IRequest<Result<string>>;

public class SyncExistingContractsCommandHandler : IRequestHandler<SyncExistingContractsCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;

    public SyncExistingContractsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<string>> Handle(SyncExistingContractsCommand request, CancellationToken cancellationToken)
    {
        // Get all ACTIVE contracts
        var activeContracts = await _context.Contracts
            .Where(c => c.ContractStatus == "ACTIVE")
            .ToListAsync(cancellationToken);

        if (!activeContracts.Any())
        {
            return Result<string>.Success("لا توجد عقود نشطة للمزامنة.");
        }

        int syncedCount = 0;
        var errors = new List<string>();

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var contract in activeContracts)
            {
                try
                {
                    // Sync all contract components to salary structure
                    await SalaryStructureSyncHelper.SyncAllContractComponentsAsync(
                        _context,
                        contract.EmployeeId,
                        basicSalary: contract.BasicSalary,
                        housingAllowance: contract.HousingAllowance,
                        transportAllowance: contract.TransportAllowance,
                        medicalAllowance: 0, // Not stored in contract
                        otherAllowances: contract.OtherAllowances,
                        cancellationToken
                    );

                    // Also sync EmployeeCompensation for backward compatibility
                    var compensation = await _context.EmployeeCompensations
                        .FirstOrDefaultAsync(c => c.EmployeeId == contract.EmployeeId, cancellationToken);

                    if (compensation != null)
                    {
                        compensation.BasicSalary = contract.BasicSalary;
                        compensation.HousingAllowance = contract.HousingAllowance;
                        compensation.TransportAllowance = contract.TransportAllowance;
                        compensation.OtherAllowances = contract.OtherAllowances;
                    }

                    syncedCount++;
                }
                catch (Exception ex)
                {
                    errors.Add($"فشل مزامنة العقد {contract.ContractId} للموظف {contract.EmployeeId}: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var message = $"تمت مزامنة {syncedCount} من أصل {activeContracts.Count} عقد نشط.";
            if (errors.Any())
            {
                message += $"\n\nالأخطاء:\n{string.Join("\n", errors)}";
            }

            return Result<string>.Success(message);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<string>.Failure($"فشلت عملية المزامنة: {ex.Message}");
        }
    }
}
