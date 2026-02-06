using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Recruitment
{
    /// <summary>
    /// كيان عروض العمل - إدارة العروض الوظيفية
    /// </summary>
    [Table("OFFERS", Schema = "HR_RECRUITMENT")]
    public class JobOffer : BaseEntity
    {
        [Key]
        [Column("OFFER_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OfferId { get; set; }

        [Required]
        [Column("APP_ID")]
        [ForeignKey(nameof(Application))]
        public int AppId { get; set; }

        [Column("OFFER_DATE")]
        public DateTime OfferDate { get; set; } = DateTime.Now;

        [Column("BASIC_SALARY", TypeName = "decimal(10, 2)")]
        public decimal? BasicSalary { get; set; }

        [Column("HOUSING_ALLOWANCE", TypeName = "decimal(10, 2)")]
        public decimal? HousingAllowance { get; set; }

        [Column("TRANSPORT_ALLOWANCE", TypeName = "decimal(10, 2)")]
        public decimal? TransportAllowance { get; set; }

        [Column("MEDICAL_ALLOWANCE", TypeName = "decimal(10, 2)")]
        public decimal? MedicalAllowance { get; set; }

        [Column("OTHER_ALLOWANCES", TypeName = "decimal(10, 2)")]
        public decimal? OtherAllowances { get; set; }

        [Column("JOINING_DATE")]
        public DateTime? JoiningDate { get; set; }

        [Column("EXPIRY_DATE")]
        public DateTime? ExpiryDate { get; set; }

        [MaxLength(20)]
        [Column("STATUS")]
        public string? Status { get; set; } = "DRAFT";

        // Navigation Properties
        public virtual JobApplication Application { get; set; } = null!;
    }
}
