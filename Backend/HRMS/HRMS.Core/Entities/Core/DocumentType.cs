using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Core
{
    /// <summary>
    /// كيان أنواع الوثائق - يحدد أنواع الوثائق المطلوبة من الموظفين
    /// </summary>
    [Table("DOCUMENT_TYPES", Schema = "HR_CORE")]
    public class DocumentType : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد لنوع الوثيقة
        /// </summary>
        [Key]
        [Column("DOCUMENT_TYPE_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocumentTypeId { get; set; }

        /// <summary>
        /// اسم نوع الوثيقة بالعربية
        /// </summary>
        [Required(ErrorMessage = "اسم نوع الوثيقة بالعربية مطلوب")]
        [MaxLength(100)]
        [Column("DOCUMENT_TYPE_NAME_AR")]
        public string DocumentTypeNameAr { get; set; } = string.Empty;

        /// <summary>
        /// اسم نوع الوثيقة بالإنجليزية
        /// </summary>
        [Required(ErrorMessage = "اسم نوع الوثيقة بالإنجليزية مطلوب")]
        [MaxLength(100)]
        [Column("DOCUMENT_TYPE_NAME_EN")]
        public string DocumentTypeNameEn { get; set; } = string.Empty;

        /// <summary>
        /// وصف نوع الوثيقة
        /// </summary>
        [MaxLength(500)]
        [Column("DESCRIPTION")]
        public string? Description { get; set; }

        /// <summary>
        /// الامتدادات المسموحة للملف (مفصولة بفاصلة) مثل: ".pdf,.jpg,.png"
        /// </summary>
        [MaxLength(200)]
        [Column("ALLOWED_EXTENSIONS")]
        public string? AllowedExtensions { get; set; }

        /// <summary>
        /// هل الوثيقة مطلوبة أم اختيارية
        /// </summary>
        [Column("IS_REQUIRED")]
        public bool IsRequired { get; set; }

        /// <summary>
        /// هل للوثيقة تاريخ انتهاء صلاحية
        /// </summary>
        [Column("HAS_EXPIRY")]
        public bool HasExpiry { get; set; }

        /// <summary>
        /// عدد الأيام الافتراضية لصلاحية الوثيقة
        /// </summary>
        [Column("DEFAULT_EXPIRY_DAYS")]
        public int? DefaultExpiryDays { get; set; }

        /// <summary>
        /// الحد الأقصى لحجم الملف بالميجابايت
        /// </summary>
        [Column("MAX_FILE_SIZE_MB")]
        public int? MaxFileSizeMB { get; set; }
    }
}
