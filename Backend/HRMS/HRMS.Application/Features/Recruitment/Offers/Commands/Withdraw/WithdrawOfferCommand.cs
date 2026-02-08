using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Offers.Commands.Withdraw;

public class WithdrawOfferCommand : IRequest<Result<bool>>
{
    public int OfferId { get; set; }
    public string? Reason { get; set; }
}

public class WithdrawOfferCommandHandler : IRequestHandler<WithdrawOfferCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public WithdrawOfferCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(WithdrawOfferCommand request, CancellationToken cancellationToken)
    {
        var offer = await _context.JobOffers
            .FirstOrDefaultAsync(o => o.OfferId == request.OfferId && o.IsDeleted == 0, cancellationToken);

        if (offer == null)
            return Result<bool>.Failure("عرض العمل غير موجود");

        if (offer.Status == "ACCEPTED")
            return Result<bool>.Failure("لا يمكن سحب عرض تم قبوله بالفعل");

        offer.Status = "WITHDRAWN";
        // offer.Notes = request.Reason; // If notes field exists
        offer.UpdatedBy = _currentUserService.UserId;
        offer.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم سحب عرض العمل بنجاح");
    }
}
