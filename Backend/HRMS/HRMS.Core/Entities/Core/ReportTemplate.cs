using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Core
{
    /// <summary>
    /// كيان قوالب التقارير - تخزين استعلامات التقارير الديناميكية
    /// </summary>
    [Table("REPORT_TEMPLATES", Schema = "HR_CORE")]
    public class ReportTemplate : BaseEntity
    {
        [Key]
        [Column("TEMPLATE_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TemplateId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("TEMPLATE_NAME_AR")]
        public string TemplateNameAr { get; set; } = string.Empty;

        [MaxLength(50)]
        [Column("REPORT_TYPE")]
        public string? ReportType { get; set; }

        [Column("SQL_QUERY")]
        public string? SqlQuery { get; set; }

        [Column("PARAMETERS")]
        public string? Parameters { get; set; }

        [Column("IS_ACTIVE")]
        public byte IsActive { get; set; } = 1;
    }
}
