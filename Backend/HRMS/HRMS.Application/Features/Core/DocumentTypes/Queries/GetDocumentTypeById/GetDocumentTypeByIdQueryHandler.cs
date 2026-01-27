using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Application.Interfaces;
using HRMS.Application.DTOs.Core;

namespace HRMS.Application.Features.Core.DocumentTypes.Queries.GetDocumentTypeById;

/// <summary>
/// معالج استعلام الحصول على نوع وثيقة بمعرفه
/// </summary>
public class GetDocumentTypeByIdQueryHandler : IRequestHandler<GetDocumentTypeByIdQuery, DocumentTypeDto?>
{
    private readonly IApplicationDbContext _context;

    public GetDocumentTypeByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DocumentTypeDto?> Handle(GetDocumentTypeByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.DocumentTypes
            .Where(d => d.DocumentTypeId == request.DocumentTypeId && d.IsDeleted == 0)
            .Select(d => new DocumentTypeDto
            {
                DocumentTypeId = d.DocumentTypeId,
                DocumentTypeNameAr = d.DocumentTypeNameAr,
                DocumentTypeNameEn = d.DocumentTypeNameEn,
                Description = d.Description,
                AllowedExtensions = d.AllowedExtensions,
                IsRequired = d.IsRequired,
                HasExpiry = d.HasExpiry,
                DefaultExpiryDays = d.DefaultExpiryDays,
                MaxFileSizeMB = d.MaxFileSizeMB
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
