using HRMS.Application.DTOs.Recruitment;
using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Recruitment.Config.Queries.GetRejectionReasons;

/// <summary>
/// استعلام للحصول على أسباب الرفض
/// </summary>
public class GetRejectionReasonsQuery : IRequest<Result<List<LookupDto>>>
{
}

/// <summary>
/// معالج استعلام أسباب الرفض
/// </summary>
public class GetRejectionReasonsQueryHandler : IRequestHandler<GetRejectionReasonsQuery, Result<List<LookupDto>>>
{
    public Task<Result<List<LookupDto>>> Handle(GetRejectionReasonsQuery request, CancellationToken cancellationToken)
    {
        // أسباب الرفض الشائعة
        var rejectionReasons = new List<LookupDto>
        {
            new LookupDto { Id = 1, NameAr = "عدم توفر الخبرة المطلوبة", NameEn = "Insufficient Experience" },
            new LookupDto { Id = 2, NameAr = "عدم توفر المؤهل المطلوب", NameEn = "Insufficient Qualifications" },
            new LookupDto { Id = 3, NameAr = "ضعف الأداء في المقابلة", NameEn = "Poor Interview Performance" },
            new LookupDto { Id = 4, NameAr = "عدم التطابق مع متطلبات الوظيفة", NameEn = "Not a Good Fit" },
            new LookupDto { Id = 5, NameAr = "توظيف مرشح آخر", NameEn = "Another Candidate Selected" },
            new LookupDto { Id = 6, NameAr = "إلغاء الشاغر", NameEn = "Position Cancelled" },
            new LookupDto { Id = 7, NameAr = "انسحاب المرشح", NameEn = "Candidate Withdrew" },
            new LookupDto { Id = 8, NameAr = "أسباب أخرى", NameEn = "Other Reasons" }
        };

        return Task.FromResult(Result<List<LookupDto>>.Success(rejectionReasons));
    }
}
