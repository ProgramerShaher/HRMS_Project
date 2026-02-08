using HRMS.Application.DTOs.Recruitment;
using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Recruitment.Config.Queries.GetInterviewTypes;

/// <summary>
/// استعلام للحصول على أنواع المقابلات
/// </summary>
public class GetInterviewTypesQuery : IRequest<Result<List<LookupDto>>>
{
}

/// <summary>
/// معالج استعلام أنواع المقابلات
/// </summary>
public class GetInterviewTypesQueryHandler : IRequestHandler<GetInterviewTypesQuery, Result<List<LookupDto>>>
{
    public Task<Result<List<LookupDto>>> Handle(GetInterviewTypesQuery request, CancellationToken cancellationToken)
    {
        // أنواع المقابلات الثابتة (Enum-based)
        var interviewTypes = new List<LookupDto>
        {
            new LookupDto { Id = 1, NameAr = "مقابلة فنية", NameEn = "Technical Interview" },
            new LookupDto { Id = 2, NameAr = "مقابلة موارد بشرية", NameEn = "HR Interview" },
            new LookupDto { Id = 3, NameAr = "مقابلة إدارية", NameEn = "Managerial Interview" },
            new LookupDto { Id = 4, NameAr = "مقابلة نهائية", NameEn = "Final Interview" }
        };

        return Task.FromResult(Result<List<LookupDto>>.Success(interviewTypes));
    }
}
