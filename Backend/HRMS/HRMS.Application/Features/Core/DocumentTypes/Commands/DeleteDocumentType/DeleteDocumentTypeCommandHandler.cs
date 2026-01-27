using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Application.Interfaces;

namespace HRMS.Application.Features.Core.DocumentTypes.Commands.DeleteDocumentType;

/// <summary>
/// معالج أمر حذف نوع الوثيقة (Soft Delete)
/// </summary>
public class DeleteDocumentTypeCommandHandler : IRequestHandler<DeleteDocumentTypeCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteDocumentTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteDocumentTypeCommand request, CancellationToken cancellationToken)
    {
        var documentType = await _context.DocumentTypes
            .FirstOrDefaultAsync(d => d.DocumentTypeId == request.DocumentTypeId, cancellationToken);

        if (documentType == null)
            throw new KeyNotFoundException($"نوع الوثيقة برقم {request.DocumentTypeId} غير موجود");

        // Soft Delete
        documentType.IsDeleted = 1;
        documentType.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
