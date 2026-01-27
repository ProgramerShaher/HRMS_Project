using MediatR;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Core;

namespace HRMS.Application.Features.Core.DocumentTypes.Commands.CreateDocumentType;

/// <summary>
/// معالج أمر إنشاء نوع وثيقة جديد
/// </summary>
public class CreateDocumentTypeCommandHandler : IRequestHandler<CreateDocumentTypeCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateDocumentTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateDocumentTypeCommand request, CancellationToken cancellationToken)
    {
        var documentType = new DocumentType
        {
            DocumentTypeNameAr = request.DocumentTypeNameAr,
            DocumentTypeNameEn = request.DocumentTypeNameEn,
            Description = request.Description,
            AllowedExtensions = request.AllowedExtensions,
            IsRequired = request.IsRequired,
            HasExpiry = request.HasExpiry,
            DefaultExpiryDays = request.DefaultExpiryDays,
            MaxFileSizeMB = request.MaxFileSizeMB,
            CreatedAt = DateTime.UtcNow
        };

        _context.DocumentTypes.Add(documentType);
        await _context.SaveChangesAsync(cancellationToken);

        return documentType.DocumentTypeId;
    }
}
