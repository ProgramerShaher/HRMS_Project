namespace HRMS.Application.DTOs.Performance;

/// <summary>
/// DTO لعرض مخالفة موظف
/// </summary>
public class EmployeeViolationDto
{
    /// <summary>
    /// معرف المخالفة
    /// </summary>
    public int ViolationId { get; set; }

    /// <summary>
    /// معرف الموظف
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// اسم الموظف الكامل
    /// </summary>
    public string EmployeeName { get; set; } = string.Empty;

    /// <summary>
    /// معرف نوع المخالفة
    /// </summary>
    public int ViolationTypeId { get; set; }

    /// <summary>
    /// نوع المخالفة (عربي)
    /// </summary>
    public string ViolationTypeNameAr { get; set; } = string.Empty;

    /// <summary>
    /// معرف الإجراء التأديبي
    /// </summary>
    public int ActionId { get; set; }

    /// <summary>
    /// الإجراء التأديبي (عربي)
    /// </summary>
    public string ActionNameAr { get; set; } = string.Empty;

    /// <summary>
    /// عدد أيام الخصم
    /// </summary>
    public int DeductionDays { get; set; }

    /// <summary>
    /// المبلغ المخصوم
    /// </summary>
    public decimal? DeductionAmount { get; set; }

    /// <summary>
    /// تاريخ المخالفة
    /// </summary>
    public DateTime ViolationDate { get; set; }

    /// <summary>
    /// الوصف
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// الحالة (PENDING, APPROVED, REJECTED)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// هل تم ترحيلها للرواتب
    /// </summary>
    public bool IsExecuted { get; set; }

    /// <summary>
    /// تاريخ التنفيذ
    /// </summary>
    public DateTime? ExecutionDate { get; set; }
}

/// <summary>
/// DTO لعرض تقييم أداء موظف
/// </summary>
public class EmployeeAppraisalDto
{
    /// <summary>
    /// معرف التقييم
    /// </summary>
    public int AppraisalId { get; set; }

    /// <summary>
    /// معرف الموظف
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// اسم الموظف
    /// </summary>
    public string EmployeeName { get; set; } = string.Empty;

    /// <summary>
    /// معرف فترة التقييم
    /// </summary>
    public int CycleId { get; set; }

    /// <summary>
    /// اسم فترة التقييم
    /// </summary>
    public string CycleName { get; set; } = string.Empty;

    /// <summary>
    /// معرف المقيّم
    /// </summary>
    public int EvaluatorId { get; set; }

    /// <summary>
    /// اسم المقيّم
    /// </summary>
    public string EvaluatorName { get; set; } = string.Empty;

    /// <summary>
    /// تاريخ التقييم
    /// </summary>
    public DateTime AppraisalDate { get; set; }

    /// <summary>
    /// الدرجة النهائية
    /// </summary>
    public decimal FinalScore { get; set; }

    /// <summary>
    /// التقدير (ممتاز، جيد جداً، إلخ)
    /// </summary>
    public string Grade { get; set; } = string.Empty;

    /// <summary>
    /// حالة التقييم
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// تفاصيل KPIs
    /// </summary>
    public List<AppraisalDetailDto> Details { get; set; } = new();
}

/// <summary>
/// DTO لتفاصيل تقييم KPI واحد
/// </summary>
public class AppraisalDetailDto
{
    /// <summary>
    /// معرف مؤشر الأداء
    /// </summary>
    public int KpiId { get; set; }

    /// <summary>
    /// اسم مؤشر الأداء
    /// </summary>
    public string KpiName { get; set; } = string.Empty;

    /// <summary>
    /// الوزن النسبي
    /// </summary>
    public decimal? Weight { get; set; }

    /// <summary>
    /// الدرجة المحققة
    /// </summary>
    public decimal Score { get; set; }

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
/// </summary>
public class KpiDto
{
    public int KpiId { get; set; }
    public string KpiNameAr { get; set; } = string.Empty;
    public string KpiDescription { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string MeasurementUnit { get; set; } = string.Empty;
}


/// <summary>
/// DTO لفترة التقييم
/// </summary>
public class AppraisalCycleDto
{
    public int CycleId { get; set; }
    public string CycleName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
