using System;
using MediatR;

namespace HRMS.Application.Features.Personnel.Employees.Commands.CreateEmployee
{
    /// <summary>
    /// طلب إنشاء موظف جديد (Command)
    /// </summary>
    /// <remarks>
    /// يمثل هذا الكائن نية المستخدم لإنشاء سجل موظف جديد في النظام.
    /// يتم إرسال هذا الطلب عبر MediatR ليتم معالجته بواسطة الـ Handler.
    /// </remarks>
    public class CreateEmployeeCommand : IRequest<int>
    {
        /// <summary>
        /// الرقم الوظيفي الفريد
        /// </summary>
        public string EmployeeNumber { get; set; }

        /// <summary>
        /// الاسم الأول بالعربية
        /// </summary>
        public string FirstNameAr { get; set; }

        /// <summary>
        /// الاسم الثاني بالعربية
        /// </summary>
        public string SecondNameAr { get; set; }

        /// <summary>
        /// الاسم الثالث بالعربية
        /// </summary>
        public string ThirdNameAr { get; set; }

        /// <summary>
        /// اسم العائلة بالعربية
        /// </summary>
        public string HijriLastNameAr { get; set; }

        /// <summary>
        /// الاسم الكامل بالإنجليزية
        /// </summary>
        public string FullNameEn { get; set; }

        /// <summary>
        /// الجنس (M/F)
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// تاريخ الميلاد
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// الحالة الاجتماعية
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
        /// معرف المدير المباشر
        /// </summary>
        public int? ManagerId { get; set; }

        /// <summary>
        /// تاريخ بداية العمل
        /// </summary>
        public DateTime JoiningDate { get; set; }

        /// <summary>
        /// البريد الإلكتروني
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// رقم الجوال
        /// </summary>
        public string Mobile { get; set; }
    }
}
