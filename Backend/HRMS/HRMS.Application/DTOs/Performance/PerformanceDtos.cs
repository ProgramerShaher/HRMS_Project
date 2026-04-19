namespace HRMS.Application.DTOs.Performance;

/// <summary>
/// DTO لعرض مخالفة موظف
/// </summary>
public class EmployeeViolationDto
{
    public int ViolationId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int ViolationTypeId { get; set; }
    public string ViolationTypeNameAr { get; set; } = string.Empty;
    public int ActionId { get; set; }
    public string ActionNameAr { get; set; } = string.Empty;
    public int DeductionDays { get; set; }
    public decimal? DeductionAmount { get; set; }
    public DateTime ViolationDate { get; set; }
    public string? Description { get; set; }
    /// <summary>الحالة (PENDING, INVESTIGATION, APPROVED, CANCELLED)</summary>
    public string Status { get; set; } = string.Empty;
    public bool IsExecuted { get; set; }
    public DateTime? ExecutionDate { get; set; }
}

/// <summary>
/// DTO لعرض تقييم أداء موظف
/// </summary>
public class EmployeeAppraisalDto
{
    public int AppraisalId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int CycleId { get; set; }
    /// <summary>✅ FIX #6: اسم الدورة يُعاد من CycleNameAr</summary>
    public string CycleName { get; set; } = string.Empty;
    public DateTime CycleStartDate { get; set; }
    public DateTime CycleEndDate { get; set; }
    public int EvaluatorId { get; set; }
    /// <summary>✅ FIX #4: اسم المُقيّم يظهر الآن بعد إضافة Include(a => a.Evaluator)</summary>
    public string EvaluatorName { get; set; } = string.Empty;
    public DateTime AppraisalDate { get; set; }
    public decimal FinalScore { get; set; }
    public string Grade { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? EmployeeComment { get; set; }
    public string? Comments { get; set; }
    public List<AppraisalDetailDto> Details { get; set; } = new();
}

/// <summary>
/// DTO لتفاصيل تقييم KPI واحد
/// </summary>
public class AppraisalDetailDto
{
    public long DetailId { get; set; }
    public int KpiId { get; set; }
    public string KpiName { get; set; } = string.Empty;
    public string? KpiCategory { get; set; }
    /// <summary>✅ FIX #5: Weight مُضاف لاستعماله في العرض وحساب الصفحة</summary>
    public decimal Weight { get; set; }
    public decimal? TargetValue { get; set; }
    public decimal? ActualValue { get; set; }
    public decimal EmployeeScore { get; set; }
    public decimal ManagerScore { get; set; }

    /// <summary>
    /// الدرجة المحققة النهائية
    /// </summary>
    public decimal FinalScore { get; set; }

    // Compatbility
    public decimal Score => FinalScore;

    /// <summary>
    /// التعليقات
    /// </summary>
    public string? Comments { get; set; }
}

/// <summary>
/// DTO لنوع المخالفة
/// </summary>
public class ViolationTypeDto
{
    public int ViolationTypeId { get; set; }
    public string ViolationNameAr { get; set; } = string.Empty;
    
    /// <summary>
    /// وصف المخالفة
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// مستوى الخطورة: 1=بسيط، 2=متوسط، 3=جسيم
    /// </summary>
    public byte SeverityLevel { get; set; }
}

/// <summary>
/// DTO للإجراء التأديبي
/// </summary>
public class DisciplinaryActionDto
{
    public int ActionId { get; set; }
    public string ActionNameAr { get; set; } = string.Empty;
    public decimal DeductionDays { get; set; }
    public bool IsTermination { get; set; }
}

/// <summary>
/// DTO لمؤشر الأداء (KPI)
/// ✅ FIX #5: إضافة Weight و TargetJobType
/// </summary>
public class KpiDto
{
    public int KpiId { get; set; }
    public string KpiNameAr { get; set; } = string.Empty;
    public string? KpiDescription { get; set; }
    public string? Category { get; set; }
    public string? MeasurementUnit { get; set; }
    /// <summary>الوزن النسبي — مجموع أوزان كل الـ KPIs يجب أن يساوي 100</summary>
    public decimal Weight { get; set; }
    public string? TargetJobType { get; set; }
}

/// <summary>
/// DTO لفترة التقييم
/// ✅ FIX #6: تصحيح أسماء الحقول لتتطابق مع Entity
/// </summary>
public class AppraisalCycleDto
{
    public int CycleId { get; set; }
    /// <summary>✅ FIX: الاسم الصحيح من العربية</summary>
    public string CycleNameAr { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public byte IsActive { get; set; }
}

/// <summary>
/// DTO ملخص نتائج تقييم موظف (للـ Dashboard)
/// </summary>
public class AppraisalResultSummaryDto
{
    public int AppraisalId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string CycleName { get; set; } = string.Empty;
    public decimal FinalScore { get; set; }
    public string Grade { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<KpiScoreChartDto> KpiScores { get; set; } = new();
}

/// <summary>
/// DTO لبيانات الـ Radar Chart (نقاط القوة والضعف)
/// </summary>
public class KpiScoreChartDto
{
    public string KpiName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal Weight { get; set; }
    public decimal EmployeeScore { get; set; }
    public decimal ManagerScore { get; set; }
    public decimal FinalScore { get; set; }
    public decimal WeightedContribution => Math.Round(FinalScore * Weight / 100, 2);
}
