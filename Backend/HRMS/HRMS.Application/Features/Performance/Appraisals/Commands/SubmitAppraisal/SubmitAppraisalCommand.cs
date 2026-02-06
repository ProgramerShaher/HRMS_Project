using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Performance;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Application.Exceptions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System;

namespace HRMS.Application.Features.Performance.Appraisals.Commands.SubmitAppraisal;

// 1. Command
public class SubmitAppraisalCommand : IRequest<Result<int>>
{
	public int EmployeeId { get; set; }
	public int CycleId { get; set; }
	public List<KpiDetailDto> KpiDetails { get; set; } = new();
	public string? EmployeeComment { get; set; }

	// 🔥 حقل جديد: يسمح لك بتحديد من هو المدير الذي يقيم (اختياري)
	// مفيد جداً للاختبار بحساب الأدمن
	public int? ManualEvaluatorId { get; set; }
}

public class KpiDetailDto
{
	public int KpiId { get; set; }
	public decimal Score { get; set; }
	public string? Comments { get; set; }
}

// 2. Validator
public class SubmitAppraisalCommandValidator : AbstractValidator<SubmitAppraisalCommand>
{
	public SubmitAppraisalCommandValidator()
	{
		RuleFor(x => x.EmployeeId).GreaterThan(0).WithMessage("معرف الموظف مطلوب");
		RuleFor(x => x.CycleId).GreaterThan(0).WithMessage("فترة التقييم مطلوبة");
		RuleFor(x => x.KpiDetails).NotEmpty().WithMessage("يجب إدخال تفاصيل التقييم");

		RuleForEach(x => x.KpiDetails).ChildRules(kpi =>
		{
			kpi.RuleFor(x => x.KpiId).GreaterThan(0);
			kpi.RuleFor(x => x.Score).InclusiveBetween(0, 100);
		});
	}
}

// 3. Handler (المصحح)
public class SubmitAppraisalCommandHandler : IRequestHandler<SubmitAppraisalCommand, Result<int>>
{
	private readonly IApplicationDbContext _context;
	private readonly ICurrentUserService _currentUserService;

	public SubmitAppraisalCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
	{
		_context = context;
		_currentUserService = currentUserService;
	}

	public async Task<Result<int>> Handle(SubmitAppraisalCommand request, CancellationToken cancellationToken)
	{
		using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
		try
		{
			// أ. تحديد هوية المُقيّم (المدير)
			int evaluatorId = 0;

			// الأولوية 1: إذا أرسلت الرقم يدوياً في الـ Swagger (للاختبار السريع)
			if (request.ManualEvaluatorId.HasValue && request.ManualEvaluatorId > 0)
			{
				evaluatorId = request.ManualEvaluatorId.Value;
			}
			// الأولوية 2: البحث عن الموظف المرتبط بالمستخدم الحالي
			else if (!string.IsNullOrEmpty(_currentUserService.UserId))
			{
				// لا نستخدم int.Parse مباشرة لأن UserId قد يكون GUID
				// نبحث في جدول الموظفين عن الموظف الذي يملك هذا UserId
				var managerEmployee = await _context.Employees
					.AsNoTracking()
					.FirstOrDefaultAsync(e => e.UserId == _currentUserService.UserId, cancellationToken);

				if (managerEmployee != null)
				{
					evaluatorId = managerEmployee.EmployeeId;
				}
			}

			// إذا لم نجد مقيماً، نرفض الطلب
			if (evaluatorId == 0)
			{
				return Result<int>.Failure("عفواً، لم يتم التعرف على هوية المُقيّم. يرجى إرسال 'manualEvaluatorId' (رقم المدير - مثلاً 17) أو التأكد من ربط المستخدم بموظف.");
			}

			// ب. التحقق من الموظف والدورة
			var employeeExists = await _context.Employees.AnyAsync(e => e.EmployeeId == request.EmployeeId, cancellationToken);
			if (!employeeExists) return Result<int>.Failure("الموظف غير موجود");

			// ج. الحسابات
			decimal finalScore = request.KpiDetails.Sum(k => k.Score); // أو Average حسب سياستك
			string grade = CalculateGrade(finalScore);

			// د. الحفظ
			var appraisal = new EmployeeAppraisal
			{
				EmployeeId = request.EmployeeId,
				CycleId = request.CycleId,
				EvaluatorId = evaluatorId, // ✅ الآن الرقم صحيح ومضمون
				AppraisalDate = DateTime.UtcNow,
				FinalScore = finalScore,
				Grade = grade,
				Status = "SUBMITTED",
				EmployeeComment = request.EmployeeComment,
				CreatedBy = _currentUserService.UserId,
				CreatedAt = DateTime.UtcNow
			};

			_context.EmployeeAppraisals.Add(appraisal);
			await _context.SaveChangesAsync(cancellationToken); // نحفظ هنا لنحصل على ID

			// هـ. حفظ التفاصيل
			var details = request.KpiDetails.Select(d => new AppraisalDetail
			{
				AppraisalId = appraisal.AppraisalId,
				KpiId = d.KpiId,
				Score = d.Score,
				Comments = d.Comments,
				CreatedBy = _currentUserService.UserId,
				CreatedAt = DateTime.UtcNow
			});
			_context.AppraisalDetails.AddRange(details);
			await _context.SaveChangesAsync(cancellationToken);

			await transaction.CommitAsync(cancellationToken);

			return Result<int>.Success(appraisal.AppraisalId, "تم حفظ التقييم بنجاح");
		}
		catch (Exception ex)
		{
			await transaction.RollbackAsync(cancellationToken);
			// إظهار الخطأ الداخلي لمعرفة السبب
			var errorMsg = ex.InnerException?.Message ?? ex.Message;
			return Result<int>.Failure($"خطأ في النظام: {errorMsg}");
		}
	}

	private string CalculateGrade(decimal score)
	{
		if (score >= 90) return "Excellent";
		if (score >= 80) return "Very Good";
		if (score >= 70) return "Good";
		if (score >= 60) return "Fair";
		return "Poor";
	}
}