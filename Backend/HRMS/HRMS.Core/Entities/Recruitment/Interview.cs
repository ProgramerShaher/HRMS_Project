using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Core.Entities.Recruitment
{
    /// <summary>
    /// كيان المقابلات - إدارة مقابلات التوظيف
    /// </summary>
    [Table("INTERVIEWS", Schema = "HR_RECRUITMENT")]
    public class Interview : BaseEntity
    {
        [Key]
        [Column("INTERVIEW_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InterviewId { get; set; }

        [Required]
        [Column("APP_ID")]
        [ForeignKey(nameof(Application))]
        public int AppId { get; set; }

        [Column("INTERVIEWER_ID")]
        [ForeignKey(nameof(Interviewer))]
        public int? InterviewerId { get; set; }

        [Required]
        [Column("SCHEDULED_TIME")]
        public DateTime ScheduledTime { get; set; }

        [MaxLength(20)]
        [Column("INTERVIEW_TYPE")]
        public string? InterviewType { get; set; } = "IN_PERSON";

        [Column("RATING")]
        public byte? Rating { get; set; }

        [Column("FEEDBACK")]
        public string? Feedback { get; set; }

        [MaxLength(20)]
        [Column("RESULT")]
        public string? Result { get; set; }

        // Navigation Properties
        public virtual JobApplication Application { get; set; } = null!;
        public virtual Employee? Interviewer { get; set; }
    }
}
