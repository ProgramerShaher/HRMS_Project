using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Core.Entities.Common
{
    /// <summary>
    /// الكيان الأساسي الذي ترث منه جميع الكيانات في النظام
    /// يحتوي على حقول التدقيق (Audit) والإصدار والحذف المنطقي
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// اسم المستخدم الذي قام بإنشاء السجل
        /// </summary>
        [MaxLength(50)]
        [Column("CREATED_BY")]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// تاريخ ووقت إنشاء السجل
        /// </summary>
        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// اسم المستخدم الذي قام بآخر تعديل على السجل
        /// </summary>
        [MaxLength(50)]
        [Column("UPDATED_BY")]
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// تاريخ ووقت آخر تعديل على السجل
        /// </summary>
        [Column("UPDATED_AT")]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// علامة الحذف المنطقي (0 = نشط، 1 = محذوف)
        /// </summary>
        [Column("IS_DELETED")]
        public byte IsDeleted { get; set; } = 0;

        /// <summary>
        /// رقم الإصدار للتحكم في التزامن المتفائل (Optimistic Concurrency)
        /// </summary>
        [Column("VERSION_NO")]
        [ConcurrencyCheck]
        public int VersionNo { get; set; } = 1;

        /// <summary>
        /// يتحقق من أن السجل غير محذوف
        /// </summary>
        [NotMapped]
        public bool IsActive => IsDeleted == 0;
    }
}
