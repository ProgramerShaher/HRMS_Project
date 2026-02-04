using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.Employees.Commands.DeleteEmployeeDocument;

public class DeleteEmployeeDocumentCommand : IRequest<Result<bool>>
{
    public int EmployeeId { get; set; }
    public int DocumentId { get; set; }
}

public class DeleteEmployeeDocumentCommandHandler : IRequestHandler<DeleteEmployeeDocumentCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileService _fileService;

    public DeleteEmployeeDocumentCommandHandler(IApplicationDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<Result<bool>> Handle(DeleteEmployeeDocumentCommand request, CancellationToken cancellationToken)
    {
        var doc = await _context.EmployeeDocuments
            .FirstOrDefaultAsync(d => d.DocumentId == request.DocumentId && d.EmployeeId == request.EmployeeId, cancellationToken);

        if (doc == null)
            return Result<bool>.Failure("Document not found");

        if (!string.IsNullOrEmpty(doc.FilePath))
        {
            await _fileService.DeleteFileAsync(doc.FilePath);
        }

        _context.EmployeeDocuments.Remove(doc);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Document deleted successfully");
    }
}
