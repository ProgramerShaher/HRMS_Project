using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Application.Interfaces;

namespace HRMS.Application.Features.Core.DocumentTypes.Commands.UpdateDocumentType;

/// <summary>
/// معالج أمر تحديث نوع الوثيقة
/// </summary>
public class UpdateDocumentTypeCommandHandler : IRequestHandler<UpdateDocumentTypeCommand, int>
{
    private readonly IApplicationDbContext _context;

    public UpdateDocumentTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(UpdateDocumentTypeCommand request, CancellationToken cancellationToken)
    {
        var documentType = await _context.DocumentTypes
            .FirstOrDefaultAsync(d => d.DocumentTypeId == request.DocumentTypeId, cancellationToken);

        if (documentType == null)
            throw new KeyNotFoundException($"نوع الوثيقة برقم {request.DocumentTypeId} غير موجود");

        documentType.DocumentTypeNameAr = request.DocumentTypeNameAr;
        documentType.DocumentTypeNameEn = request.DocumentTypeNameEn;
        documentType.Description = request.Description;
        documentType.AllowedExtensions = request.AllowedExtensions;
        documentType.IsRequired = request.IsRequired;
        documentType.HasExpiry = request.HasExpiry;
        documentType.DefaultExpiryDays = request.DefaultExpiryDays;
        documentType.MaxFileSizeMB = request.MaxFileSizeMB;
        documentType.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return documentType.DocumentTypeId;
    }
}
