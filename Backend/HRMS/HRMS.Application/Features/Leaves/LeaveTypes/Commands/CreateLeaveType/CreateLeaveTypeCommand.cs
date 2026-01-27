using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Leaves;
using MediatR;
using AutoMapper;

namespace HRMS.Application.Features.Leaves.LeaveTypes.Commands.CreateLeaveType;

/// <summary>
/// أمر إنشاء نوع إجازة جديد
/// </summary>
public record CreateLeaveTypeCommand(LeaveTypeDto Data) : IRequest<int>;

public class CreateLeaveTypeCommandHandler : IRequestHandler<CreateLeaveTypeCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateLeaveTypeCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateLeaveTypeCommand request, CancellationToken cancellationToken)
    {
        var leaveType = _mapper.Map<LeaveType>(request.Data);
        _context.LeaveTypes.Add(leaveType);
        await _context.SaveChangesAsync(cancellationToken);
        return leaveType.LeaveTypeId;
    }
}
