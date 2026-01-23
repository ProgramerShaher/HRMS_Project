namespace HRMS.Application.DTOs.Core
{
    public class BranchDto
    {
        public int BranchId { get; set; }
        public string? BranchNameAr { get; set; }
        public string? BranchNameEn { get; set; }
        public int? CityId { get; set; }
        public string? Address { get; set; }
    }

    public class CreateBranchDto
    {
        public required string BranchNameAr { get; set; }
        public string? BranchNameEn { get; set; }
        public int? CityId { get; set; }
        public string? Address { get; set; }
    }

    public class UpdateBranchDto
    {
        public required string BranchNameAr { get; set; }
        public string? BranchNameEn { get; set; }
        public int? CityId { get; set; }
        public string? Address { get; set; }
    }
}
