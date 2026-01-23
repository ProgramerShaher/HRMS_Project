using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Core;

namespace HRMS.Core.Entities.Recruitment
{
    /// <summary>
    /// كيان المرشحين - مخزون الكفاءات (Talent Pool)
    /// </summary>
    [Table("CANDIDATES", Schema = "HR_RECRUITMENT")]
    public class Candidate : BaseEntity
    {
        [Key]
        [Column("CANDIDATE_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CandidateId { get; set; }

        [MaxLength(50)]
        [Column("FIRST_NAME_AR")]
        public string? FirstNameAr { get; set; }

        [MaxLength(50)]
        [Column("FAMILY_NAME_AR")]
        public string? FamilyNameAr { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("FULL_NAME_EN")]
        public string FullNameEn { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        [Column("EMAIL")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("PHONE")]
        public string? Phone { get; set; }

        [Column("NATIONALITY_ID")]
        [ForeignKey(nameof(Nationality))]
        public int? NationalityId { get; set; }

        [MaxLength(500)]
        [Column("CV_FILE_PATH")]
        public string? CvFilePath { get; set; }

        [MaxLength(200)]
        [Column("LINKEDIN_PROFILE")]
        public string? LinkedinProfile { get; set; }

        // Navigation Properties
        public virtual Country? Nationality { get; set; }
        public virtual ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}
