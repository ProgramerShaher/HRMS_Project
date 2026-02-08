// using HRMS.Application.Interfaces;
// using HRMS.Core.Utilities;
// using MediatR;
// using Microsoft.EntityFrameworkCore;
// using System.Threading;
// using System.Threading.Tasks;

// namespace HRMS.Application.Features.Leaves.LeaveTypes.Commands.DeleteLeaveType
// {
//     // Command
//     public record DeleteLeaveTypeCommand(int LeaveTypeId) : IRequest<Result<bool>>;

//     // Handler
//     public class DeleteLeaveTypeCommandHandler : IRequestHandler<DeleteLeaveTypeCommand, Result<bool>>
//     {
//         private readonly IApplicationDbContext _context;

//         public DeleteLeaveTypeCommandHandler(IApplicationDbContext context)
//         {
//             _context = context;
//         }

//         public async Task<Result<bool>> Handle(DeleteLeaveTypeCommand request, CancellationToken cancellationToken)
//         {
//             // 1. Get Leave Type
//             var leaveType = await _context.LeaveTypes
//                 .FirstOrDefaultAsync(lt => lt.LeaveTypeId == request.LeaveTypeId && lt.IsDeleted == 0, cancellationToken);

//             if (leaveType == null)
//                 return Result<bool>.Failure("نوع الإجازة غير موجود", 404);

//             // 2. Integrity Check: Check for Existing Balances
//             var hasBalances = await _context.EmployeeLeaveBalances
//                 .AnyAsync(b => b.LeaveTypeId == request.LeaveTypeId && b.IsDeleted == 0, cancellationToken);

//             if (hasBalances)
//                 return Result<bool>.Failure("لا يمكن حذف نوع الإجازة لأنه مرتبط بأرصدة موظفين حالية", 400);

//             // 3. Integrity Check: Check for Existing Requests
//             var hasRequests = await _context.LeaveRequests
//                 .AnyAsync(r => r.LeaveTypeId == request.LeaveTypeId && r.IsDeleted == 0, cancellationToken);

//             if (hasRequests)
//                 return Result<bool>.Failure("لا يمكن حذف نوع الإجازة لأنه مرتبط بطلبات إجازة سابقة", 400);

//             // 4. Soft Delete
//             leaveType.IsDeleted = 1;

//             await _context.SaveChangesAsync(cancellationToken);

//             return Result<bool>.Success(true, "تم حذف نوع الإجازة بنجاح");
//         }
//     }
// }
