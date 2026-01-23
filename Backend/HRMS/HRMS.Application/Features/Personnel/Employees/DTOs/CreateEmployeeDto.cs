using System;

namespace HRMS.Application.Features.Personnel.Employees.DTOs
{
    /// <summary>
    /// كائن نقل البيانات لإنشاء موظف جديد
    /// </summary>
    /// <remarks>
    /// يُستخدم لاستقبال بيانات الموظف الجديد من العميل عبر الـ API.
    /// يتم التحقق من صحة البيانات باستخدام FluentValidation قبل المعالجة.
    /// </remarks>
    public class CreateEmployeeDto
    {
        /// <summary>
        /// الرقم الوظيفي (يجب أن يكون فريداً)
        /// </summary>
        public string EmployeeNumber { get; set; }

        /// <summary>
        /// الاسم الأول بالعربية (مطلوب)
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
        /// اللقب (اسم العائلة) بالعربية (مطلوب)
        /// </summary>
        public string HijriLastNameAr { get; set; }

        /// <summary>
        /// الاسم الكامل بالإنجليزية كما في الجواز
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
        /// معرف الجنسية (مطلوب)
        /// </summary>
        public int NationalityId { get; set; }

        /// <summary>
        /// معرف الوظيفة (مطلوب)
        /// </summary>
        public int JobId { get; set; }

        /// <summary>
        /// معرف القسم (مطلوب)
        /// </summary>
        public int DeptId { get; set; }

        /// <summary>
        /// معرف المدير المباشر (اختياري)
        /// </summary>
        public int? ManagerId { get; set; }

        /// <summary>
        /// تاريخ المباشرة في العمل
        /// </summary>
        public DateTime JoiningDate { get; set; }

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
