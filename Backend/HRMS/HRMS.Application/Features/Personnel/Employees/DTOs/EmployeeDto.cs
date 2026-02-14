using System;

namespace HRMS.Application.Features.Personnel.Employees.DTOs
{
    /// <summary>
    /// كائن نقل البيانات لعرض بيانات الموظف
    /// </summary>
    /// <remarks>
    /// يُستخدم لإرجاع بيانات الموظف من الـ API إلى العميل.
    /// يحتوي على البيانات الأساسية والمرجعية بشكل مبسط.
    /// </remarks>
    public class EmployeeDto
    {
        /// <summary>
        /// المعرف الفريد للموظف
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// الرقم الوظيفي المميز للموظف
        /// </summary>
        public string EmployeeNumber { get; set; }

        /// <summary>
        /// الاسم الأول بالعربية
        /// </summary>
        public string FirstNameAr { get; set; }

        /// <summary>
        /// الاسم الثاني (اسم الأب) بالعربية
        /// </summary>
        public string SecondNameAr { get; set; }

        /// <summary>
        /// الاسم الثالث (اسم الجد) بالعربية
        /// </summary>
        public string ThirdNameAr { get; set; }

        /// <summary>
        /// اللقب (اسم العائلة) بالعربية
        /// </summary>
        public string LastNameAr { get; set; }

        /// <summary>
        /// الاسم الكامل بالإنجليزية
        /// </summary>
        public string FullNameEn { get; set; }
        
        /// <summary>
        /// الاسم الكامل بالعربية
        /// </summary>
        public string FullNameAr { get; set; }

        /// <summary>
        /// الجنس (M: ذكر، F: أنثى)
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// تاريخ الميلاد
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// الحالة الاجتماعية (Single, Married, Divorced, Widowed)
        /// </summary>
        public string MaritalStatus { get; set; }

        /// <summary>
        /// رقم الهوية الوطنية
        /// </summary>
        public string NationalId { get; set; }

        /// <summary>
        /// رقم الهاتف الجوال
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// البريد الإلكتروني الرسمي
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// تاريخ الالتحاق بالعمل
        /// </summary>
        public DateTime HireDate { get; set; }

        /// <summary>
        /// معرف القسم
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// اسم القسم بالعربية (من جدول الأقسام)
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// معرف الوظيفة
        /// </summary>
        public int JobId { get; set; }

        /// <summary>
        /// معرف الدرجة الوظيفية
        /// </summary>
        public int? JobGradeId { get; set; }

        /// <summary>
        /// المسمى الوظيفي بالعربية (من جدول الوظائف)
        /// </summary>
        public string JobTitle { get; set; }

        /// <summary>
        /// معرف الجنسية
        /// </summary>
        public int? NationalityId { get; set; }

        /// <summary>
        /// اسم الجنسية بالعربية (من جدول الدول)
        /// </summary>
        public string NationalityName { get; set; }

        /// <summary>
        /// رقم الترخيص المهني
        /// </summary>
        public string? LicenseNumber { get; set; }

        /// <summary>
        /// تاريخ انتهاء الترخيص
        /// </summary>
        public DateTime? LicenseExpiryDate { get; set; }

        /// <summary>
        /// التخصص الطبي/المهني
        /// </summary>
        public string? Specialty { get; set; }

        /// <summary>
        /// معرف المدير المباشر
        /// </summary>
        public int? ManagerId { get; set; }

        /// <summary>
        /// معرف المستخدم (للربط مع Identity)
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// مسار صورة الملف الشخصي
        /// </summary>
        public string? ProfilePicturePath { get; set; }

        /// <summary>
        /// حالة الموظف (نشط/غير نشط)
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// تاريخ إنهاء الخدمة
        /// </summary>
        public DateTime? TerminationDate { get; set; }
    }
}
