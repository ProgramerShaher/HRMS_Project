using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Core;

namespace HRMS.Core.Entities.Personnel
{
    /// <summary>
    /// كيان وثائق الموظف - يحتوي على الهويات والجوازات والرخص
    /// </summary>
    [Table("EMPLOYEE_DOCUMENTS", Schema = "HR_PERSONNEL")]
    public class EmployeeDocument : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للوثيقة
        /// </summary>
        [Key]
        [Column("DOC_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocId { get; set; }

        /// <summary>
        /// معرف الموظف صاحب الوثيقة
        /// </summary>
        [Required(ErrorMessage = "الموظف مطلوب")]
        [Column("EMPLOYEE_ID")]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        /// <summary>
        /// معرف نوع الوثيقة
        /// </summary>
        [Required(ErrorMessage = "نوع الوثيقة مطلوب")]
        [Column("DOC_TYPE_ID")]
        [ForeignKey(nameof(DocumentType))]
        public int DocTypeId { get; set; }

        /// <summary>
        /// رقم الوثيقة
        /// </summary>
        [MaxLength(50)]
        [Column("DOC_NUMBER")]
        public string? DocNumber { get; set; }

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
        /// مكان الإصدار
        /// </summary>
        [MaxLength(100)]
        [Column("ISSUE_PLACE")]
        public string? IssuePlace { get; set; }

        /// <summary>
        /// مسار الملف المرفق أو الرابط
        /// </summary>
        [MaxLength(500)]
        [Column("ATTACHMENT_PATH")]
        public string? AttachmentPath { get; set; }

        /// <summary>
        /// هل الوثيقة نشطة (1=نعم، 0=لا)
        /// </summary>
        [Column("IS_ACTIVE")]
        public byte IsActive { get; set; } = 1;

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// الموظف صاحب الوثيقة
        /// </summary>
        public virtual Employee Employee { get; set; } = null!;

        /// <summary>
        /// نوع الوثيقة
        /// </summary>
        public virtual DocumentType DocumentType { get; set; } = null!;
    }
}
