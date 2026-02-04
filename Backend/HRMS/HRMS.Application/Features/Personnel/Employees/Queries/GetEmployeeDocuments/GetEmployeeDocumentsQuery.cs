using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace HRMS.Application.Features.Personnel.Employees.Queries.GetEmployeeDocuments;

public class GetEmployeeDocumentsQuery : IRequest<Result<List<EmployeeDocumentDto>>>
{
    public int EmployeeId { get; set; }
}

public class GetEmployeeDocumentsQueryHandler : IRequestHandler<GetEmployeeDocumentsQuery, Result<List<EmployeeDocumentDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeDocumentsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<EmployeeDocumentDto>>> Handle(GetEmployeeDocumentsQuery request, CancellationToken cancellationToken)
    {
        var docs = await _context.EmployeeDocuments
            .Include(d => d.DocumentType)
            .Where(d => d.EmployeeId == request.EmployeeId)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<EmployeeDocumentDto>>(docs);
        return Result<List<EmployeeDocumentDto>>.Success(dtos);
    }
}
