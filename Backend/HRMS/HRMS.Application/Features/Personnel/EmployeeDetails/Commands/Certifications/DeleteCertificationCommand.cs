using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Certifications;

public class DeleteCertificationCommand : IRequest<Result<bool>>
{
    public int EmployeeId { get; set; }
    public int CertificationId { get; set; }
}

public class DeleteCertificationCommandHandler : IRequestHandler<DeleteCertificationCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileService _fileService;

    public DeleteCertificationCommandHandler(IApplicationDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<Result<bool>> Handle(DeleteCertificationCommand request, CancellationToken cancellationToken)
    {
        var cert = await _context.Certifications
            .FirstOrDefaultAsync(c => c.CertId == request.CertificationId && c.EmployeeId == request.EmployeeId, cancellationToken);

        if (cert == null)
            return Result<bool>.Failure("Certification not found");

        if (!string.IsNullOrEmpty(cert.AttachmentPath))
        {
            await _fileService.DeleteFileAsync(cert.AttachmentPath);
        }

        _context.Certifications.Remove(cert);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Certification deleted successfully");
    }
}
