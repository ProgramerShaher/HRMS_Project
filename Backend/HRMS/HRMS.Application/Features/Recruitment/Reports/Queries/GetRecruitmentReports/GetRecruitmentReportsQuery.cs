using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Reports.Queries.GetRecruitmentReports;

public class GetRecruitmentReportsQuery : IRequest<Result<RecruitmentReportsDto>> { }

public class RecruitmentReportsDto
{
    public RecruitmentSummaryDto Summary { get; set; } = new();
    public List<ChartItemDto> CandidatesByStatus { get; set; } = new();
    public List<ChartItemDto> CandidatesBySource { get; set; } = new();
    public List<ChartItemDto> VacanciesByDepartment { get; set; } = new();
    public List<ChartItemDto> ApplicationPipeline { get; set; } = new();
}

public class RecruitmentSummaryDto
{
    public int OpenVacancies { get; set; }
    public int ActiveApplications { get; set; }
    public int HiredThisMonth { get; set; }
    public int UpcomingInterviews { get; set; }
    public int TotalCandidates { get; set; }
}

public class ChartItemDto
{
    public string Label { get; set; } = string.Empty;
    public int Value { get; set; }
    public string? Color { get; set; }
}

public class GetRecruitmentReportsQueryHandler : IRequestHandler<GetRecruitmentReportsQuery, Result<RecruitmentReportsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRecruitmentReportsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<RecruitmentReportsDto>> Handle(GetRecruitmentReportsQuery request, CancellationToken cancellationToken)
    {
        var report = new RecruitmentReportsDto();
        var now = DateTime.Now;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);

        // 1. Summary Metrics
        report.Summary.TotalCandidates = await _context.Candidates.CountAsync(c => c.IsDeleted == 0, cancellationToken);
        
        report.Summary.OpenVacancies = await _context.JobVacancies
            .CountAsync(v => (v.Status == "OPEN" || v.Status == "Active" || v.Status == "نشطة") && v.IsDeleted == 0, cancellationToken);
        
        report.Summary.ActiveApplications = await _context.JobApplications
            .CountAsync(a => a.Status != "REJECTED" && a.Status != "HIRED" && a.IsDeleted == 0, cancellationToken);
        
        report.Summary.HiredThisMonth = await _context.JobApplications
            .CountAsync(a => a.Status == "HIRED" && a.UpdatedAt >= startOfMonth && a.IsDeleted == 0, cancellationToken);

        report.Summary.UpcomingInterviews = await _context.Interviews
            .CountAsync(i => i.ScheduledTime >= now && i.Status != "CANCELLED" && i.IsDeleted == 0, cancellationToken);

        // 2. Candidates by Status
        report.CandidatesByStatus = await _context.Candidates
            .Where(c => c.IsDeleted == 0)
            .GroupBy(c => c.Status ?? "ACTIVE")
            .Select(g => new ChartItemDto { Label = g.Key, Value = g.Count() })
            .ToListAsync(cancellationToken);

        // 3. Candidates by Source (Logic: Check ApplicationSource then LinkedinProfile)
        var candidates = await _context.Candidates.Where(c => c.IsDeleted == 0).ToListAsync(cancellationToken);
        report.CandidatesBySource = candidates
            .GroupBy(c => GetSourceLabel(c.LinkedinProfile ?? c.ApplicationSource))
            .Select(g => new ChartItemDto { Label = g.Key, Value = g.Count() })
            .ToList();

        // 4. Vacancies by Department
        report.VacanciesByDepartment = await _context.JobVacancies
            .Include(v => v.Department)
            .Where(v => v.IsDeleted == 0)
            .GroupBy(v => v.Department != null ? v.Department.DeptNameAr : "بدون قسم")
            .Select(g => new ChartItemDto { Label = g.Key, Value = g.Count() })
            .ToListAsync(cancellationToken);

        // 5. Application Pipeline
        report.ApplicationPipeline = await _context.JobApplications
            .Where(a => a.IsDeleted == 0)
            .GroupBy(a => a.Status)
            .Select(g => new ChartItemDto { Label = g.Key, Value = g.Count() })
            .ToListAsync(cancellationToken);

        return Result<RecruitmentReportsDto>.Success(report);
    }

    private string GetSourceLabel(string? source)
    {
        if (string.IsNullOrEmpty(source)) return "مباشر";
        var s = source.ToLower();
        if (s.Contains("linkedin")) return "LinkedIn";
        if (s.Contains("github")) return "GitHub";
        if (s.Contains("facebook")) return "Facebook";
        if (s.Contains("instagram")) return "Instagram";
        if (s.Contains("x.com") || s.Contains("twitter")) return "X / Twitter";
        return "أخرى";
    }
}
