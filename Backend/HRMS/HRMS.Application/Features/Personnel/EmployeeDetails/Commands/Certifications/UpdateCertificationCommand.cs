using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // For IFormFile

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Certifications;

public class UpdateCertificationCommand : IRequest<Result<bool>>
{
    public int CertificationId { get; set; }
    public int EmployeeId { get; set; }
    public string CertNameAr { get; set; } = string.Empty;
    public string? IssuingAuthority { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? CertNumber { get; set; }
    public byte IsMandatory { get; set; }
    public IFormFile? Attachment { get; set; }
}

public class UpdateCertificationCommandHandler : IRequestHandler<UpdateCertificationCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileService _fileService;

    public UpdateCertificationCommandHandler(IApplicationDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<Result<bool>> Handle(UpdateCertificationCommand request, CancellationToken cancellationToken)
    {
        var cert = await _context.Certifications
            .FirstOrDefaultAsync(c => c.CertId == request.CertificationId && c.EmployeeId == request.EmployeeId, cancellationToken);

        if (cert == null)
            return Result<bool>.Failure("Certification not found");

        cert.CertNameAr = request.CertNameAr;
        cert.IssuingAuthority = request.IssuingAuthority;
        cert.IssueDate = request.IssueDate;
        cert.ExpiryDate = request.ExpiryDate;
        cert.CertNumber = request.CertNumber;
        cert.IsMandatory = request.IsMandatory;

        if (request.Attachment != null)
        {
            if (!string.IsNullOrEmpty(cert.AttachmentPath))
            {
                await _fileService.DeleteFileAsync(cert.AttachmentPath);
            }
            cert.AttachmentPath = await _fileService.UploadFileAsync(request.Attachment, "certifications");
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Certification updated successfully");
    }
}
