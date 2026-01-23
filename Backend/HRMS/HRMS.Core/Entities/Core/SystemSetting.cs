using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Core
{
    /// <summary>
    /// كيان إعدادات النظام - تخزين الثوابت والإعدادات العامة
    /// </summary>
    [Table("SYSTEM_SETTINGS", Schema = "HR_CORE")]
    public class SystemSetting : BaseEntity
    {
        [Key]
        [Column("SETTING_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SettingId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("SETTING_KEY")]
        public string SettingKey { get; set; } = string.Empty;

        [MaxLength(500)]
        [Column("SETTING_VALUE")]
        public string? SettingValue { get; set; }

        [MaxLength(20)]
        [Column("SETTING_TYPE")]
        public string? SettingType { get; set; } // STRING, NUMBER, DATE, BOOLEAN

        [MaxLength(200)]
        [Column("DESCRIPTION_AR")]
        public string? DescriptionAr { get; set; }

        [Column("IS_EDITABLE")]
        public byte IsEditable { get; set; } = 1;
    }
}
