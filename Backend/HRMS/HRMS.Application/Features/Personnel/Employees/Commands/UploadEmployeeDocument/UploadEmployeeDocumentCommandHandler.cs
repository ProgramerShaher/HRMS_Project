using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.Employees.Commands.UploadEmployeeDocument;

public class UploadEmployeeDocumentCommandHandler : IRequestHandler<UploadEmployeeDocumentCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileService _fileService;

    public UploadEmployeeDocumentCommandHandler(IApplicationDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<int> Handle(UploadEmployeeDocumentCommand request, CancellationToken cancellationToken)
    {
        // 1. Verify Employee
        var employee = await _context.Employees.FindAsync(new object[] { request.EmployeeId }, cancellationToken);
        if (employee == null)
            throw new KeyNotFoundException($"Employee {request.EmployeeId} not found");

        // 2. Upload File (Best Practice: Infrastructure handles storage logic)
        var folderInfo = $"employees/documents";  
        var savedPath = await _fileService.UploadFileAsync(request.File, folderInfo);

        // 3. Create Document Entity
        var doc = new EmployeeDocument
        {
            EmployeeId = request.EmployeeId,
            DocumentTypeId = request.DocumentTypeId,
            DocumentNumber = request.DocumentNumber,
            ExpiryDate = request.ExpiryDate,
            FileName = request.File.FileName,
            ContentType = request.File.ContentType,
            FileSize = request.File.Length,
            FilePath = savedPath, // Storing relative path
            CreatedAt = DateTime.UtcNow
        };

        _context.EmployeeDocuments.Add(doc);
        await _context.SaveChangesAsync(cancellationToken);

        return doc.DocumentId;
    }
}
