// using HRMS.Application.Interfaces;
// using HRMS.Core.Entities.Personnel;
// using HRMS.Core.Utilities;
// using MediatR;
// using Microsoft.AspNetCore.Http;
// using Microsoft.EntityFrameworkCore;

// namespace HRMS.Application.Features.Personnel.Employees.Commands.UploadDocument;

// public class UploadEmployeeDocumentCommand : IRequest<Result<int>>
// {
//     public int EmployeeId { get; set; }
//     public IFormFile File { get; set; }
//     public string DocumentTypeName { get; set; } = string.Empty; // e.g. "NationalID", "Passport"
//     public DateTime? ExpiryDate { get; set; }
//     public string? DocumentNumber { get; set; }
// }

// public class UploadEmployeeDocumentCommandHandler : IRequestHandler<UploadEmployeeDocumentCommand, Result<int>>
// {
//     private readonly IApplicationDbContext _context;
//     private readonly IFileService _fileService;

//     public UploadEmployeeDocumentCommandHandler(IApplicationDbContext context, IFileService fileService)
//     {
//         _context = context;
//         _fileService = fileService;
//     }

//     public async Task<Result<int>> Handle(UploadEmployeeDocumentCommand request, CancellationToken cancellationToken)
//     {
//         // 1. Verify Employee
//         var employee = await _context.Employees.FindAsync(new object[] { request.EmployeeId }, cancellationToken);
//         if (employee == null)
//             return Result<int>.Failure("Employee not found");

//         // 2. Resolve Document Type ID
//         // For now, looking up by Name. If not found, creating a new type?? 
//         // Or we assume predefined types.
//         // Let's assume we look up or default to "Other" (1).
        
//         var docType = await _context.DocumentTypes
//             .FirstOrDefaultAsync(t => t.DocumentTypeNameEn == request.DocumentTypeName || t.DocumentTypeNameAr == request.DocumentTypeName, cancellationToken);

//         int docTypeId;
//         if (docType == null)
//         {
//             // Optional: Create dynamic type? Or fail?
//             // User wants "specific place", so ideally we have types.
//             // For now, let's use a default if not found OR return error.
//             // Let's return error to enforce valid types.
//             // But user might send "National ID" text.
//             // I'll assume standard types exist. If not, I'll fetch the first one or throw.
//             // actually, let's try to map string to ID.
//              var defaultType = await _context.DocumentTypes.FirstOrDefaultAsync(cancellationToken);
//              docTypeId = defaultType?.DocumentTypeId ?? 1; // Fallback
//         }
//         else
//         {
//             docTypeId = docType.DocumentTypeId;
//         }

//         // 3. Upload File to designated folder
//         // Folder: employees/{id}/{DocumentTypeName}/
//         // Sanitize DocumentTypeName for folder path
//         var safeTypeName = string.Join("", request.DocumentTypeName.Split(Path.GetInvalidFileNameChars()));
//         var folderPath = $"employees/{request.EmployeeId}/{safeTypeName}";
        
//         string filePath;
//         try
//         {
//             filePath = await _fileService.UploadFileAsync(request.File, folderPath);
//         }
//         catch (Exception ex)
//         {
//             return Result<int>.Failure($"File upload failed: {ex.Message}");
//         }

//         // 4. Create Document Entity
//         var document = new EmployeeDocument
//         {
//             EmployeeId = request.EmployeeId,
//             DocumentTypeId = docTypeId,
//             DocumentNumber = request.DocumentNumber,
//             ExpiryDate = request.ExpiryDate,
//             FileName = request.File.FileName,
//             FilePath = filePath,
//             ContentType = request.File.ContentType,
//             FileSize = request.File.Length,
//             IssueDate = DateTime.Now // Default
//         };

//         _context.EmployeeDocuments.Add(document);
//         await _context.SaveChangesAsync(cancellationToken);

//         return Result<int>.Success(document.DocumentId, "Document uploaded successfully");
//     }
// }
