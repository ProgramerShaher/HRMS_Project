using AutoMapper;
using HRMS.Application.DTOs.Performance;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

using HRMS.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace HRMS.Application.Features.Performance.Appraisals.Queries.GetAll;

public class GetAppraisalsQuery : IRequest<Result<List<EmployeeAppraisalDto>>>
{
    public int? EmployeeId { get; set; }
    public int? CycleId { get; set; }
    public string? Phase { get; set; }
}

public class GetAppraisalsQueryHandler : IRequestHandler<GetAppraisalsQuery, Result<List<EmployeeAppraisalDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAppraisalsQueryHandler(
        IApplicationDbContext context, 
        IMapper mapper,
        ICurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<Result<List<EmployeeAppraisalDto>>> Handle(GetAppraisalsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.EmployeeAppraisals
            .Include(a => a.Employee)
            .Include(a => a.Cycle)
            .Include(a => a.Evaluator)
            .Where(a => a.IsDeleted == 0)
            .AsQueryable();

        var userIdString = _currentUserService.UserId;
        if (!string.IsNullOrEmpty(userIdString))
        {
            var user = await _userManager.FindByIdAsync(userIdString);
            if (user != null)
            {
                var isAdmin = await _userManager.IsInRoleAsync(user, "System_Admin") || await _userManager.IsInRoleAsync(user, "HR_Manager");
                if (!isAdmin)
                {
                    var employeeId = user.EmployeeId;
                    if (employeeId.HasValue)
                    {
                        // If not admin, you can only see it if you are the Employee (Self) OR the Evaluator (Manager)
                        query = query.Where(a => a.EmployeeId == employeeId.Value || a.EvaluatorId == employeeId.Value);
                    }
                    else
                    {
                        // No employee linked = no access
                        return Result<List<EmployeeAppraisalDto>>.Failure("User is not linked to an employee record", 403);
                    }
                }
            }
        }

        if (request.EmployeeId.HasValue)
        {
            query = query.Where(a => a.EmployeeId == request.EmployeeId);
        }

        if (request.CycleId.HasValue)
        {
            query = query.Where(a => a.CycleId == request.CycleId);
        }

        if (!string.IsNullOrEmpty(request.Phase))
        {
            query = query.Where(a => a.Status == request.Phase);
        }

        var appraisals = await query
            .OrderByDescending(a => a.AppraisalDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<EmployeeAppraisalDto>>(appraisals);

        return Result<List<EmployeeAppraisalDto>>.Success(dtos);
    }
}
