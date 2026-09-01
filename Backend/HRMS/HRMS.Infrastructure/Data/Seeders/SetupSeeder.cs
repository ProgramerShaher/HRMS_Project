using HRMS.Core.Entities.Core;
using HRMS.Core.Entities.Attendance;
using HRMS.Core.Entities.Leaves;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Infrastructure.Data.Seeders
{
    /// <summary>
    /// بذر بيانات التهيئة الأساسية (Setup Data Seeder)
    /// </summary>
    public static class SetupSeeder
    {
        public static async Task SeedSetupDataAsync(HRMSDbContext context)
        {
            // 1. Seed Countries
            if (!await context.Countries.AnyAsync())
            {
                var countries = new List<Country>
                {
                    new Country { CountryNameAr = "اليمن", CountryNameEn = "Yemen", IsoCode = "YE", CitizenshipNameAr = "يمني", CitizenshipNameEn = "Yemeni"},
                    new Country { CountryNameAr = "السعودية", CountryNameEn = "Saudi Arabia", IsoCode = "SA", CitizenshipNameAr = "سعودي", CitizenshipNameEn = "Saudi"},
                    new Country { CountryNameAr = "مصر", CountryNameEn = "Egypt", IsoCode = "EG", CitizenshipNameAr = "مصري", CitizenshipNameEn = "Egyptian"}
                };
                await context.Countries.AddRangeAsync(countries);
                await context.SaveChangesAsync();
            }

            // 2. Seed Cities
            if (!await context.Cities.AnyAsync())
            {
                var yemen = await context.Countries.FirstOrDefaultAsync(c => c.IsoCode == "YE");
                var saudi = await context.Countries.FirstOrDefaultAsync(c => c.IsoCode == "SA");
                var egypt = await context.Countries.FirstOrDefaultAsync(c => c.IsoCode == "EG");

                var cities = new List<City>();
                if (yemen != null)
                {
                    cities.AddRange(new[]
                    {
                        new City { CityNameAr = "صنعاء", CityNameEn = "Sana'a", CountryId = yemen.CountryId},
                        new City { CityNameAr = "عدن", CityNameEn = "Aden", CountryId = yemen.CountryId},
                        new City { CityNameAr = "تعز", CityNameEn = "Taiz", CountryId = yemen.CountryId}
                    });
                }
                if (saudi != null)
                {
                    cities.AddRange(new[]
                    {
                        new City { CityNameAr = "الرياض", CityNameEn = "Riyadh", CountryId = saudi.CountryId},
                        new City { CityNameAr = "جدة", CityNameEn = "Jeddah", CountryId = saudi.CountryId}
                    });
                }
                if (egypt != null)
                {
                    cities.Add(new City { CityNameAr = "القاهرة", CityNameEn = "Cairo", CountryId = egypt.CountryId});
                }
                await context.Cities.AddRangeAsync(cities);
                await context.SaveChangesAsync();
            }

            // 3. Seed Branches
            if (!await context.Branches.AnyAsync())
            {
                var sanaa = await context.Cities.FirstOrDefaultAsync(c => c.CityNameEn == "Sana'a");
                var branches = new List<Branch>
                {
                    new Branch { BranchNameAr = "الفرع الرئيسي", BranchNameEn = "Main Branch", CityId = sanaa?.CityId, Address = "شارع الزبيري"}
                };
                await context.Branches.AddRangeAsync(branches);
                await context.SaveChangesAsync();
            }

            // 4. Seed Departments
            if (!await context.Departments.AnyAsync())
            {
                var departments = new List<Department>
                {
                    new Department { DeptNameAr = "الإدارة العليا", DeptNameEn = "Executive Management"},
                    new Department { DeptNameAr = "الموارد البشرية", DeptNameEn = "Human Resources"},
                    new Department { DeptNameAr = "تقنية المعلومات", DeptNameEn = "Information Technology"},
                    new Department { DeptNameAr = "المالية والحسابات", DeptNameEn = "Finance & Accounting"}
                };
                await context.Departments.AddRangeAsync(departments);
                await context.SaveChangesAsync();
            }

            // 5. Seed Job Grades
            if (!await context.JobGrades.AnyAsync())
            {
                var grades = new List<JobGrade>
                {
                    new JobGrade { GradeCode = "G1", GradeNameAr = "درجة تنفيذية", GradeNameEn = "Executive"},
                    new JobGrade { GradeCode = "G2", GradeNameAr = "درجة عليا", GradeNameEn = "Senior"},
                    new JobGrade { GradeCode = "G3", GradeNameAr = "درجة متوسطة", GradeNameEn = "Mid-Level"},
                    new JobGrade { GradeCode = "G4", GradeNameAr = "درجة مبتدئة", GradeNameEn = "Junior"}
                };
                await context.JobGrades.AddRangeAsync(grades);
                await context.SaveChangesAsync();
            }

            // 6. Seed Jobs
            if (!await context.Jobs.AnyAsync())
            {
                var execGrade = await context.JobGrades.FirstOrDefaultAsync(g => g.GradeCode == "G1");
                var seniorGrade = await context.JobGrades.FirstOrDefaultAsync(g => g.GradeCode == "G2");
                var midGrade = await context.JobGrades.FirstOrDefaultAsync(g => g.GradeCode == "G3");

                var jobs = new List<Job>
                {
                    new Job { JobTitleAr = "مدير عام", JobTitleEn = "General Manager", DefaultGradeId = execGrade?.JobGradeId},
                    new Job { JobTitleAr = "مدير موارد بشرية", JobTitleEn = "HR Manager", DefaultGradeId = seniorGrade?.JobGradeId},
                    new Job { JobTitleAr = "مهندس برمجيات", JobTitleEn = "Software Engineer", DefaultGradeId = midGrade?.JobGradeId},
                    new Job { JobTitleAr = "محاسب", JobTitleEn = "Accountant", DefaultGradeId = midGrade?.JobGradeId}
                };
                await context.Jobs.AddRangeAsync(jobs);
                await context.SaveChangesAsync();
            }

            // 7. Seed Document Types
            if (!await context.DocumentTypes.AnyAsync())
            {
                var docTypes = new List<DocumentType>
                {
                    new DocumentType { DocumentTypeNameAr = "بطاقة هوية وطنية", DocumentTypeNameEn = "National ID"},
                    new DocumentType { DocumentTypeNameAr = "جواز سفر", DocumentTypeNameEn = "Passport"},
                    new DocumentType { DocumentTypeNameAr = "شهادة تخرج", DocumentTypeNameEn = "Graduation Certificate"}
                };
                await context.DocumentTypes.AddRangeAsync(docTypes);
                await context.SaveChangesAsync();
            }

            // 8. Seed Banks
            if (!await context.Banks.AnyAsync())
            {
                var banks = new List<Bank>
                {
                    new Bank { BankNameAr = "بنك الكريمي", BankNameEn = "Kuraimi Islamic Microfinance Bank", BankCode = "KIMB"},
                    new Bank { BankNameAr = "بنك التضامن", BankNameEn = "Tadhamon Bank", BankCode = "TIB"},
                    new Bank { BankNameAr = "بنك اليمن والكويت", BankNameEn = "Yemen Kuwait Bank", BankCode = "YKB"}
                };
                await context.Banks.AddRangeAsync(banks);
                await context.SaveChangesAsync();
            }

            // 9. Seed Leave Types
            if (!await context.LeaveTypes.AnyAsync())
            {
                var leaves = new List<LeaveType>
                {
                    new LeaveType { LeaveNameAr = "إجازة سنوية", LeaveNameEn = "Annual Leave", IsPaid = 1, DefaultDays = 30},
                    new LeaveType { LeaveNameAr = "إجازة مرضية", LeaveNameEn = "Sick Leave", IsPaid = 1, DefaultDays = 15, RequiresAttachment = 1},
                    new LeaveType { LeaveNameAr = "إجازة بدون راتب", LeaveNameEn = "Unpaid Leave", IsPaid = 0, DefaultDays = 0}
                };
                await context.LeaveTypes.AddRangeAsync(leaves);
                await context.SaveChangesAsync();
            }

            // 10. Seed Shift Types
            if (!await context.ShiftTypes.AnyAsync())
            {
                var shifts = new List<ShiftType>
                {
                    new ShiftType { ShiftNameAr = "فترة صباحية", StartTime = "08:00", EndTime = "16:00", HoursCount = 8, IsCrossDay = 0},
                    new ShiftType { ShiftNameAr = "فترة مسائية", StartTime = "16:00", EndTime = "23:59", HoursCount = 8, IsCrossDay = 0}
                };
                await context.ShiftTypes.AddRangeAsync(shifts);
                await context.SaveChangesAsync();
            }
        }
    }
}
