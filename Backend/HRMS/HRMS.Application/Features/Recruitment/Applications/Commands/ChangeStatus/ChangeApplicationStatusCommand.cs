using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Applications.Commands.ChangeStatus;

public class ChangeApplicationStatusCommand : IRequest<Result<bool>>
{
	public int AppId { get; set; }
	public string Status { get; set; } = string.Empty;
	public string? Notes { get; set; }
}

public class ChangeApplicationStatusCommandValidator : AbstractValidator<ChangeApplicationStatusCommand>
{
	public ChangeApplicationStatusCommandValidator()
	{
		RuleFor(x => x.AppId).GreaterThan(0);

		// ✅ تم الإصلاح: أضفنا "SHORTLISTED" إلى القائمة المسموحة
		RuleFor(x => x.Status).NotEmpty()
			.Must(s => new[] { "NEW", "SCREENING", "SHORTLISTED", "INTERVIEW", "OFFERED", "HIRED", "REJECTED" }.Contains(s))
			.WithMessage("حالة الطلب غير صالحة. الحالات المسموحة: NEW, SCREENING, SHORTLISTED, INTERVIEW, OFFERED, HIRED, REJECTED");
	}
}

public class ChangeApplicationStatusCommandHandler : IRequestHandler<ChangeApplicationStatusCommand, Result<bool>>
{
	private readonly IApplicationDbContext _context;
	private readonly ICurrentUserService _currentUserService;

	public ChangeApplicationStatusCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
	{
		_context = context;
		_currentUserService = currentUserService;
	}

	public async Task<Result<bool>> Handle(ChangeApplicationStatusCommand request, CancellationToken cancellationToken)
	{
		var application = await _context.JobApplications
			.FirstOrDefaultAsync(a => a.AppId == request.AppId, cancellationToken); // تأكد أن الاسم في الداتابيز ApplicationId وليس AppId

		if (application == null)
			return Result<bool>.Failure("طلب التوظيف غير موجود");

		// تحديث الحالة
		application.Status = request.Status;

		// إذا كان هناك حقل للملاحظات في الجدول، قم بإلغاء التعليق عن السطر التالي
		// application.Notes = request.Notes; 

		// تحديث حقول التدقيق (Audit Fields) إذا كانت مدعومة
		// application.UpdatedBy = _currentUserService.UserId;
		// application.UpdatedAt = DateTime.UtcNow;

		await _context.SaveChangesAsync(cancellationToken);

		return Result<bool>.Success(true, "تم تغيير حالة الطلب بنجاح");
	}
}