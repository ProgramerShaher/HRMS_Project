using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Personnel
{
    /// <summary>
    /// كيان الشهادات المهنية والتراخيص - يحتوي على الشهادات الطبية والمهنية
    /// </summary>
    [Table("EMPLOYEE_CERTIFICATIONS", Schema = "HR_PERSONNEL")]
    public class EmployeeCertification : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للشهادة
        /// </summary>
        [Key]
        [Column("CERT_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CertId { get; set; }

        /// <summary>
        /// معرف الموظف
        /// </summary>
        [Required(ErrorMessage = "الموظف مطلوب")]
        [Column("EMPLOYEE_ID")]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        /// <summary>
        /// اسم الشهادة بالعربية (مثل: BLS, ACLS، رخصة الهيئة)
        /// </summary>
        [Required(ErrorMessage = "اسم الشهادة مطلوب")]
        [MaxLength(200)]
        [Column("CERT_NAME_AR")]
        public string CertNameAr { get; set; } = string.Empty;

        /// <summary>
        /// الجهة المانحة
        /// </summary>
        [MaxLength(200)]
        [Column("ISSUING_AUTHORITY")]
        public string? IssuingAuthority { get; set; }

        /// <summary>
        /// تاريخ الإصدار
        /// </summary>
        [Column("ISSUE_DATE")]
        public DateTime? IssueDate { get; set; }

        /// <summary>
        /// تاريخ الانتهاء
        /// </summary>
        [Column("EXPIRY_DATE")]
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// رقم الشهادة
        /// </summary>
        [MaxLength(100)]
        [Column("CERT_NUMBER")]
        public string? CertNumber { get; set; }

        /// <summary>
        /// هل هي إلزامية لمزاولة المهنة (1=نعم، 0=لا)
        /// </summary>
        [Column("IS_MANDATORY")]
        public byte IsMandatory { get; set; } = 0;

        /// <summary>
        /// مسار صورة الشهادة
        /// </summary>
        [MaxLength(500)]
        [Column("ATTACHMENT_PATH")]
        public string? AttachmentPath { get; set; }

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// الموظف صاحب الشهادة
        /// </summary>
        public virtual Employee Employee { get; set; } = null!;
    }
}
