using MediatR;
using Microsoft.AspNetCore.Http;

namespace HRMS.Application.Features.Personnel.Employees.Commands.UploadEmployeeDocument;

public class UploadEmployeeDocumentCommand : IRequest<int>
{
    public int EmployeeId { get; set; }
    public int DocumentTypeId { get; set; }
    public string? DocumentNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public IFormFile File { get; set; } = null!; // الملف نفسه
}
