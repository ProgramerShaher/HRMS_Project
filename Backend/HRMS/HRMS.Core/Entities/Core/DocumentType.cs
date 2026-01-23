using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Core
{
    /// <summary>
    /// كيان أنواع الوثائق - يحتوي على أنواع الوثائق المطلوبة من الموظفين
    /// </summary>
    [Table("DOCUMENT_TYPES", Schema = "HR_CORE")]
    public class DocumentType : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد لنوع الوثيقة
        /// </summary>
        [Key]
        [Column("DOC_TYPE_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocTypeId { get; set; }

        /// <summary>
        /// اسم نوع الوثيقة بالعربية
        /// </summary>
        [Required(ErrorMessage = "اسم نوع الوثيقة مطلوب")]
        [MaxLength(100, ErrorMessage = "اسم نوع الوثيقة لا يمكن أن يتجاوز 100 حرف")]
        [Column("DOC_NAME_AR")]
        public string DocNameAr { get; set; } = string.Empty;

        /// <summary>
        /// هل الوثيقة إلزامية (1 = نعم، 0 = لا)
        /// </summary>
        [Column("IS_MANDATORY")]
        public byte IsMandatory { get; set; } = 0;

        /// <summary>
        /// هل الوثيقة تتطلب تاريخ انتهاء (1 = نعم، 0 = لا)
        /// </summary>
        [Column("REQUIRES_EXPIRY")]
        public byte RequiresExpiry { get; set; } = 1;

        /// <summary>
        /// عدد الأيام قبل انتهاء الوثيقة للتنبيه
        /// </summary>
        [Column("ALERT_DAYS_BEFORE")]
        public short AlertDaysBefore { get; set; } = 30;
    }
}
