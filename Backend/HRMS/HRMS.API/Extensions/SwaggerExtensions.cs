using Microsoft.OpenApi.Models;

namespace HRMS.API.Extensions;

/// <summary>
/// امتدادات Swagger Configuration
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// إضافة وتكوين Swagger
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "HRMS Hospital API",
                Version = "v1",
                Description = "نظام إدارة الموارد البشرية - واجهة برمجية متكاملة",
                Contact = new OpenApiContact
                {
                    Name = "HRMS Development Team",
                    Email = "support@hrms-hospital.com"
                }
            });

            // JWT Authentication في Swagger
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "أدخل JWT Token بالصيغة: Bearer {token}"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    /// <summary>
    /// استخدام Swagger في Pipeline
    /// </summary>
    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "HRMS API v1");
            options.RoutePrefix = "swagger";
            options.DocumentTitle = "HRMS Hospital API Documentation";
        });

        return app;
    }
}
