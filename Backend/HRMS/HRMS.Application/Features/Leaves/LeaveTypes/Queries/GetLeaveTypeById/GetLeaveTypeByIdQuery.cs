using AutoMapper;
using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Leaves.LeaveTypes.Queries.GetLeaveTypeById
{
    // Query
    public record GetLeaveTypeByIdQuery(int LeaveTypeId) : IRequest<Result<LeaveTypeDto>>;

    // Handler
    public class GetLeaveTypeByIdQueryHandler : IRequestHandler<GetLeaveTypeByIdQuery, Result<LeaveTypeDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetLeaveTypeByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<LeaveTypeDto>> Handle(GetLeaveTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var leaveType = await _context.LeaveTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(lt => lt.LeaveTypeId == request.LeaveTypeId && lt.IsDeleted == 0, cancellationToken);

            if (leaveType == null)
                return Result<LeaveTypeDto>.Failure("نوع الإجازة غير موجود", 404);

            var dto = _mapper.Map<LeaveTypeDto>(leaveType);

            return Result<LeaveTypeDto>.Success(dto);
        }
    }
}
