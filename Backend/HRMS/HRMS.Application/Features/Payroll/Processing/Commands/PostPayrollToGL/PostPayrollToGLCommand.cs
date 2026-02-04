using FluentValidation;
using HRMS.Application.Features.Payroll.Processing.Services;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Payroll.Processing.Commands.PostPayrollToGL;

/// <summary>
/// أمر ترحيل مسير الرواتب إلى دليل الحسابات
/// Post Payroll to General Ledger Command
/// </summary>
public class PostPayrollToGLCommand : IRequest<Result<long>>
{
    /// <summary>
    /// معرف مسير الرواتب
    /// </summary>
    public int RunId { get; set; }
}

/// <summary>
/// معالج أمر ترحيل الرواتب
/// </summary>
public class PostPayrollToGLCommandHandler : IRequestHandler<PostPayrollToGLCommand, Result<long>>
{
    private readonly PayrollAccountingService _accountingService;
    private readonly IApplicationDbContext _context;

    public PostPayrollToGLCommandHandler(
        PayrollAccountingService accountingService,
        IApplicationDbContext context)
    {
        _accountingService = accountingService;
        _context = context;
    }

    public async Task<Result<long>> Handle(PostPayrollToGLCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // استدعاء خدمة الترحيل
            var journalEntryId = await _accountingService.PostPayrollToGLAsync(request.RunId);

            return Result<long>.Success(
                journalEntryId,
                $"Payroll Run {request.RunId} posted successfully to GL. Journal Entry ID: {journalEntryId}");
        }
        catch (Exception ex)
        {
            return Result<long>.Failure(ex.Message);
        }
    }
}

/// <summary>
/// التحقق من صحة الأمر
/// </summary>
public class PostPayrollToGLCommandValidator : AbstractValidator<PostPayrollToGLCommand>
{
    private readonly IApplicationDbContext _context;

    public PostPayrollToGLCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.RunId)
            .GreaterThan(0)
            .WithMessage("RunId must be greater than 0")
            .MustAsync(PayrollRunExists)
            .WithMessage("Payroll Run not found")
            .MustAsync(PayrollRunIsApproved)
            .WithMessage("Payroll Run must be APPROVED before posting to GL")
            .MustAsync(PayrollRunNotAlreadyPosted)
            .WithMessage("Payroll Run is already posted to GL");
    }

    private async Task<bool> PayrollRunExists(int runId, CancellationToken cancellationToken)
    {
        var exists = await _context.PayrollRuns.FindAsync(new object[] { runId }, cancellationToken);
        return exists != null;
    }

    private async Task<bool> PayrollRunIsApproved(int runId, CancellationToken cancellationToken)
    {
        var run = await _context.PayrollRuns.FindAsync(new object[] { runId }, cancellationToken);
        return run?.Status == "APPROVED";
    }

    private async Task<bool> PayrollRunNotAlreadyPosted(int runId, CancellationToken cancellationToken)
    {
        var run = await _context.PayrollRuns.FindAsync(new object[] { runId }, cancellationToken);
        return run?.Status != "POSTED";
    }
}
