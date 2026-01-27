using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Leaves
{
    /// <summary>
    /// كيان أنواع الإجازات - يحتوي على تعريفات الإجازات (سنوية، مرضية، طارئة)
    /// </summary>
    [Table("LEAVE_TYPES", Schema = "HR_LEAVES")]
    public class LeaveType : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد لنوع الإجازة
        /// </summary>
        [Key]
        [Column("LEAVE_TYPE_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LeaveTypeId { get; set; }

        /// <summary>
        /// اسم الإجازة بالعربية
        /// </summary>
        [Required(ErrorMessage = "اسم الإجازة مطلوب")]
        [MaxLength(100, ErrorMessage = "اسم الإجازة لا يمكن أن يتجاوز 100 حرف")]
        [Column("LEAVE_NAME_AR")]
        public string LeaveNameAr { get; set; } = string.Empty;

        /// <summary>
        /// الاسم بالإنجليزية
        /// </summary>
        [MaxLength(100)]
        [Column("LEAVE_NAME_EN")]
        public string? LeaveNameEn { get; set; }

        /// <summary>
        /// عدد الأيام الافتراضي
        /// </summary>
        [Column("DEFAULT_DAYS")]
        public int DefaultDays { get; set; } = 0;

        /// <summary>
        /// هل الإجازة مدفوعة الراتب (1=نعم، 0=لا)
        /// </summary>
        [Column("IS_PAID")]
        public byte IsPaid { get; set; } = 1;

        /// <summary>
        /// هل يخصم من الرصيد السنوي (1=نعم، 0=لا)
        /// </summary>
        [Column("IS_DEDUCTIBLE")]
        public bool IsDeductible { get; set; } = true;

        /// <summary>
        /// الحد الأقصى للأيام في السنة
        /// </summary>
        [Column("MAX_DAYS_PER_YEAR")]
        public short? MaxDaysPerYear { get; set; }

        /// <summary>
        /// هل تتطلب ملف مرفق (مثل تقرير طبي) (1=نعم، 0=لا)
        /// </summary>
        [Column("REQUIRES_ATTACHMENT")]
        public byte RequiresAttachment { get; set; } = 0;

        /// <summary>
        /// هل يمكن ترحيل الرصيد للسنة التالية (1=نعم، 0=لا)
        /// </summary>
        [Column("IS_CARRY_FORWARD")]
        public byte IsCarryForward { get; set; } = 0;

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// طلبات الإجازة المرتبطة بهذا النوع
        /// </summary>
        public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    }
}
