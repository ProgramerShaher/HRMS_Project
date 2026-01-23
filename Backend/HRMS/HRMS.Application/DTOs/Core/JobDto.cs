namespace HRMS.Application.DTOs.Core
{
    public class JobDto
    {
        public int JobId { get; set; }
        public string? JobTitleAr { get; set; }
        public string? JobTitleEn { get; set; }
        public int? DefaultGradeId { get; set; }
        public int? IsMedical { get; set; }
    }

    public class CreateJobDto
    {
        public required string JobTitleAr { get; set; }
        public string? JobTitleEn { get; set; }
        public int? DefaultGradeId { get; set; }
        public int? IsMedical { get; set; }
    }

    public class UpdateJobDto
    {
        public required string JobTitleAr { get; set; }
        public string? JobTitleEn { get; set; }
        public int? DefaultGradeId { get; set; }
        public int? IsMedical { get; set; }
    }
}
