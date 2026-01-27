using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Interfaces;
using MediatR;
using AutoMapper;
using HRMS.Application.Exceptions;

namespace HRMS.Application.Features.Leaves.LeaveTypes.Commands.UpdateLeaveType;

/// <summary>
/// أمر تحديث نوع إجازة
/// </summary>
public record UpdateLeaveTypeCommand(int LeaveTypeId, LeaveTypeDto Data) : IRequest<Unit>;

public class UpdateLeaveTypeCommandHandler : IRequestHandler<UpdateLeaveTypeCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateLeaveTypeCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateLeaveTypeCommand request, CancellationToken cancellationToken)
    {
        var leaveType = await _context.LeaveTypes.FindAsync(new object[] { request.LeaveTypeId }, cancellationToken);

        if (leaveType == null)
            throw new NotFoundException(nameof(leaveType), request.LeaveTypeId);

        _mapper.Map(request.Data, leaveType);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
