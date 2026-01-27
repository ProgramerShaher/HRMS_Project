using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Application.Interfaces;

namespace HRMS.Application.Features.Core.JobGrades.Commands.DeleteJobGrade;

public class DeleteJobGradeCommandHandler : IRequestHandler<DeleteJobGradeCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteJobGradeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteJobGradeCommand request, CancellationToken cancellationToken)
    {
        var jobGrade = await _context.JobGrades
            .Include(g => g.Jobs)
            .FirstOrDefaultAsync(g => g.JobGradeId == request.JobGradeId, cancellationToken);

        if (jobGrade == null)
            throw new KeyNotFoundException($"الدرجة الوظيفية برقم {request.JobGradeId} غير موجودة");

        // منع الحذف إذا مرتبطة بوظائف
        if (jobGrade.Jobs.Any())
            throw new InvalidOperationException("لا يمكن حذف الدرجة الوظيفية لأنها مرتبطة بوظائف");

        // Soft Delete
        jobGrade.IsDeleted = 1;
        jobGrade.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
