using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Core
{
    /// <summary>
    /// كيان البنوك - يحتوي على بيانات البنوك المعتمدة لتحويل الرواتب
    /// </summary>
    [Table("BANKS", Schema = "HR_CORE")]
    public class Bank : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للبنك
        /// </summary>
        [Key]
        [Column("BANK_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BankId { get; set; }

        /// <summary>
        /// اسم البنك بالعربية
        /// </summary>
        [Required(ErrorMessage = "اسم البنك بالعربية مطلوب")]
        [MaxLength(100, ErrorMessage = "اسم البنك لا يمكن أن يتجاوز 100 حرف")]
        [Column("BANK_NAME_AR")]
        public string BankNameAr { get; set; } = string.Empty;

        /// <summary>
        /// اسم البنك بالإنجليزية
        /// </summary>
        [MaxLength(100)]
        [Column("BANK_NAME_EN")]
        public string? BankNameEn { get; set; }

        /// <summary>
        /// رمز البنك
        /// </summary>
        [MaxLength(20)]
        [Column("BANK_CODE")]
        public string? BankCode { get; set; }

        /// <summary>
        /// رمز السويفت الدولي للبنك (8 أو 11 حرف)
        /// </summary>
        [MaxLength(11)]
        [Column("SWIFT_CODE")]
        public string? SwiftCode { get; set; }

        /// <summary>
        /// عنوان البنك
        /// </summary>
        [MaxLength(200)]
        [Column("ADDRESS")]
        public string? Address { get; set; }

        /// <summary>
        /// رقم هاتف البنك
        /// </summary>
        [MaxLength(20)]
        [Column("PHONE")]
        public string? Phone { get; set; }

        /// <summary>
        /// البريد الإلكتروني للبنك
        /// </summary>
        [MaxLength(100)]
        [Column("EMAIL")]
        public string? Email { get; set; }
    }
}
