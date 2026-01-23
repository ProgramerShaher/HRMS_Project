using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Recruitment
{
    /// <summary>
    /// كيان طلبات التوظيف - يربط المرشح بالوظيفة الشاغرة
    /// </summary>
    [Table("APPLICATIONS", Schema = "HR_RECRUITMENT")]
    public class JobApplication : BaseEntity
    {
        [Key]
        [Column("APP_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppId { get; set; }

        [Required]
        [Column("VACANCY_ID")]
        [ForeignKey(nameof(Vacancy))]
        public int VacancyId { get; set; }

        [Required]
        [Column("CANDIDATE_ID")]
        [ForeignKey(nameof(Candidate))]
        public int CandidateId { get; set; }

        [Column("APPLICATION_DATE")]
        public DateTime ApplicationDate { get; set; } = DateTime.Now;

        [MaxLength(20)]
        [Column("STATUS")]
        public string? Status { get; set; } = "APPLIED";

        [MaxLength(200)]
        [Column("REJECTION_REASON")]
        public string? RejectionReason { get; set; }

        [MaxLength(50)]
        [Column("RX_SOURCE")]
        public string? Source { get; set; }

        // Navigation Properties
        public virtual JobVacancy Vacancy { get; set; } = null!;
        public virtual Candidate Candidate { get; set; } = null!;
        public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();
        public virtual ICollection<JobOffer> Offers { get; set; } = new List<JobOffer>();
    }
}
