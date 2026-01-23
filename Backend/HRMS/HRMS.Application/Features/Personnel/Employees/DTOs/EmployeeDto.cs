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
        public string HijriLastNameAr { get; set; }

        /// <summary>
        /// الاسم الكامل بالإنجليزية
        /// </summary>
        public string FullNameEn { get; set; }

        /// <summary>
        /// الجنس (M: ذكر، F: أنثى)
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// تاريخ الميلاد
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// الحالة الاجتماعية (SINGLE, MARRIED, DIVORCED, WIDOWED)
        /// </summary>
        public string MaritalStatus { get; set; }
        
        /// <summary>
        /// معرف الجنسية
        /// </summary>
        public int NationalityId { get; set; }

        /// <summary>
        /// معرف الوظيفة
        /// </summary>
        public int JobId { get; set; }

        /// <summary>
        /// معرف القسم
        /// </summary>
        public int DeptId { get; set; }
        
        /// <summary>
        /// اسم الجنسية بالعربية (من جدول الدول)
        /// </summary>
        public string NationalityName { get; set; }

        /// <summary>
        /// المسمى الوظيفي بالعربية (من جدول الوظائف)
        /// </summary>
        public string JobTitle { get; set; }

        /// <summary>
        /// اسم القسم بالعربية (من جدول الأقسام)
        /// </summary>
        public string DepartmentName { get; set; }
        
        /// <summary>
        /// تاريخ الالتحاق بالعمل
        /// </summary>
        public DateTime JoiningDate { get; set; }

        /// <summary>
        /// حالة الموظف (ACTIVE, RESIGNED, TERMINATED)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// البريد الإلكتروني الرسمي
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// رقم الهاتف الجوال
        /// </summary>
        public string Mobile { get; set; }
    }
}
