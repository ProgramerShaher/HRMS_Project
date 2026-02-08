using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Offers.Commands.Update;

/// <summary>
/// Update Job Offer Command
/// </summary>
public class UpdateOfferCommand : IRequest<Result<int>>
{
    public int OfferId { get; set; }
    public decimal? BasicSalary { get; set; }
    public decimal? HousingAllowance { get; set; }
    public decimal? TransportAllowance { get; set; }
    public DateTime? JoiningDate { get; set; }
    public string? OfferTerms { get; set; }
}

/// <summary>
/// Update Offer Command Validator
/// </summary>
public class UpdateOfferCommandValidator : AbstractValidator<UpdateOfferCommand>
{
    public UpdateOfferCommandValidator()
    {
        RuleFor(x => x.OfferId).GreaterThan(0);
        RuleFor(x => x.BasicSalary).GreaterThan(0).When(x => x.BasicSalary.HasValue);
        RuleFor(x => x.HousingAllowance).GreaterThanOrEqualTo(0).When(x => x.HousingAllowance.HasValue);
        RuleFor(x => x.TransportAllowance).GreaterThanOrEqualTo(0).When(x => x.TransportAllowance.HasValue);
        RuleFor(x => x.JoiningDate).GreaterThan(DateTime.Today).When(x => x.JoiningDate.HasValue)
            .WithMessage("تاريخ الالتحاق يجب أن يكون في المستقبل");
    }
}

/// <summary>
/// Update Offer Command Handler
/// </summary>
public class UpdateOfferCommandHandler : IRequestHandler<UpdateOfferCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateOfferCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(UpdateOfferCommand request, CancellationToken cancellationToken)
    {
        // البحث عن عرض العمل
        var offer = await _context.JobOffers
            .FirstOrDefaultAsync(o => o.OfferId == request.OfferId && o.IsDeleted == 0, cancellationToken);

        if (offer == null)
            return Result<int>.Failure("عرض العمل غير موجود");

        // Business Rule: Cannot update accepted offers
        if (offer.Status == "ACCEPTED")
            return Result<int>.Failure("لا يمكن تعديل عرض عمل تم قبوله");

        if (offer.Status == "WITHDRAWN")
            return Result<int>.Failure("لا يمكن تعديل عرض عمل تم سحبه");

        // تحديث البيانات المالية
        if (request.BasicSalary.HasValue)
            offer.BasicSalary = request.BasicSalary.Value;

        if (request.HousingAllowance.HasValue)
            offer.HousingAllowance = request.HousingAllowance.Value;

        if (request.TransportAllowance.HasValue)
            offer.TransportAllowance = request.TransportAllowance.Value;

        if (request.JoiningDate.HasValue)
            offer.JoiningDate = request.JoiningDate.Value;

        // if (!string.IsNullOrEmpty(request.OfferTerms))
        //     offer.OfferTerms = request.OfferTerms;

        offer.UpdatedBy = _currentUserService.UserId;
        offer.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(offer.OfferId, "تم تحديث عرض العمل بنجاح");
    }
}
