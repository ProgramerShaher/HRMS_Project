using FluentValidation;
using HRMS.Application.Behaviors;
using HRMS.Application.Interfaces;
using HRMS.Application.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HRMS.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = typeof(ApplicationServiceRegistration).Assembly;

            // تسجيل MediatR و AutoMapper
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(assembly);
                // تسجيل سلوك التحقق (Validation Behavior)
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });
            
            services.AddAutoMapper(assembly);

            // تسجيل FluentValidation
            services.AddValidatorsFromAssembly(assembly);

            // تسجيل خدمات السياسات والذاكرة المؤقتة
            services.AddScoped<IAttendancePolicyService, AttendancePolicyService>();
            services.AddMemoryCache();

            return services;
        }
    }
}
