using System;

namespace HRMS.Core.Entities.Common
{
    /// <summary>
    /// الكيان الأساسي للتدقيق وتتبع التغييرات
    /// </summary>
    public abstract class AuditableEntity
    {
        // تم استخدام string لأن بعض الجداول قد لا يكون لها Id رقمي موحد، لكن غالباً سيكون موجوداً في الطبقات المشتقة
        // أو يمكننا إضافة public int Id { get; set; } هنا إذا كانت كل الجداول موحدة
        
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? VersionNo { get; set; }
        public int? IsDeleted { get; set; } // Oracle NUMBER(1) maps to int, not bool
    }
}
