using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HRMS.Core.Entities.Personnel;
using HRMS.Infrastructure.Data;
using MediatR;

namespace HRMS.Application.Features.Personnel.Employees.Commands.CreateEmployee
{
    /// <summary>
    /// معالج أمر إنشاء موظف جديد
    /// </summary>
    /// <remarks>
    /// يستقبل الأمر ويقوم بتحويله إلى Entity ثم حفظه في قاعدة البيانات.
    /// يستخدم DbContext مباشرة (أفضل ممارسة في CQRS).
    /// </remarks>
    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, int>
    {
        #region Dependencies - التبعيات

        private readonly HRMSDbContext _context;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor - المُنشئ

        /// <summary>
        /// مُنشئ المعالج
        /// </summary>
        /// <param name="context">سياق قاعدة البيانات</param>
        /// <param name="mapper">أداة التحويل بين الكائنات</param>
        public CreateEmployeeCommandHandler(HRMSDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #endregion

        #region Handler Method - دالة المعالجة

        /// <summary>
        /// معالجة أمر إنشاء الموظف
        /// </summary>
        /// <param name="request">بيانات الموظف الجديد</param>
        /// <param name="cancellationToken">رمز الإلغاء</param>
        /// <returns>معرف الموظف الذي تم إنشاؤه</returns>
        public async Task<int> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            // تحويل Command إلى Entity
            var employee = _mapper.Map<Employee>(request);

            // إضافة الموظف إلى قاعدة البيانات
            _context.Employees.Add(employee);
            
            // حفظ التغييرات
            await _context.SaveChangesAsync(cancellationToken);

            return employee.EmployeeId;
        }

        #endregion
    }
}
