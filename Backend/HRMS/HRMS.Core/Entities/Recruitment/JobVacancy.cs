using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Core;

namespace HRMS.Core.Entities.Recruitment
{
    /// <summary>
    /// كيان الشواغر الوظيفية - إدارة الوظائف الشاغرة
    /// </summary>
    [Table("JOB_VACANCIES", Schema = "HR_RECRUITMENT")]
    public class JobVacancy : BaseEntity
    {
        [Key]
        [Column("VACANCY_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VacancyId { get; set; }

        [Required]
        [Column("JOB_ID")]
        [ForeignKey(nameof(Job))]
        public int JobId { get; set; }

        [Required]
        [Column("DEPT_ID")]
        [ForeignKey(nameof(Department))]
        public int DeptId { get; set; }

        [Column("REQUIRED_COUNT")]
        public short RequiredCount { get; set; } = 1;

        [Column("JOB_DESCRIPTION")]
        public string? JobDescription { get; set; }

        [Column("REQUIREMENTS")]
        public string? Requirements { get; set; }

        [MaxLength(20)]
        [Column("STATUS")]
        public string? Status { get; set; } = "OPEN";

        [Column("PUBLISH_DATE")]
        public DateTime? PublishDate { get; set; }

        [Column("CLOSING_DATE")]
        public DateTime? ClosingDate { get; set; }

        // Navigation Properties
        public virtual Job Job { get; set; } = null!;
        public virtual Department Department { get; set; } = null!;
        public virtual ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}
