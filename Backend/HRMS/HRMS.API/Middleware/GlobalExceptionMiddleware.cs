using System.Net;
using System.Text.Json;
using FluentValidation;
using HRMS.Core.Utilities; // النمط الموحد للنتائج Result<T>
using Microsoft.Data.SqlClient; // للوصول لأدق تفاصيل SQL Server
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Middleware;

/// <summary>
/// English: The Ultimate Global Exception Handler for HRMS ERP.
/// Handles every possible system failure, including DB Deadlocks, Concurrency, and Identity errors.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // تسجيل الخطأ بكامل تفاصيله للمطورين في الـ Logs
        _logger.LogError(exception, "CRITICAL ERROR CAUGHT: {Message}", exception.Message);

        context.Response.ContentType = "application/json";
        var statusCode = (int)HttpStatusCode.InternalServerError;
        var message = "حدث خطأ غير متوقع في النظام، يرجى مراجعة الإدارة التقنية";
        var errors = new List<string>();

        // منطق التمييز العبقري بين أنواع الأخطاء
        // Comprehensive analysis of system-wide exceptions
        switch (exception)
        {
            // 1. أخطاء التحقق (المدخلات غير الصحيحة)
            case Application.Exceptions.ValidationException valEx:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = "فشل التحقق من البيانات، يرجى تصحيح الأخطاء أدناه";

                // تحويل القاموس إلى قائمة نصوص مسطحة (Flattening)
                // Converts Dictionary<PropertyName, ErrorMessages[]> to a flat List<string>
                errors = valEx.Errors
                    .SelectMany(kvp => kvp.Value.Select(errorMsg => $"{kvp.Key}: {errorMsg}"))
                    .ToList();
                break;
            case FluentValidation.ValidationException fvEx:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = "فشل التحقق من صحة الحقول";
                errors = fvEx.Errors.Select(e => e.ErrorMessage).ToList();
                break;

            // 2. أخطاء عدم الوجود (السجل غير موجود)
            case Application.Exceptions.NotFoundException nfEx:
                statusCode = (int)HttpStatusCode.NotFound;
                message = nfEx.Message;
                break;

            // 3. أخطاء التزامن (تعديل نفس السجل من شخصين في نفس الوقت)
            case DbUpdateConcurrencyException:
                statusCode = (int)HttpStatusCode.Conflict;
                message = "عفواً، قام مستخدم آخر بتعديل هذه البيانات قبل قليل، يرجى تحديث الصفحة";
                break;

            // 4. أخطاء قاعدة البيانات العميقة
            case DbUpdateException dbUpdateEx:
                if (dbUpdateEx.InnerException is SqlException sqlEx)
                {
                    switch (sqlEx.Number)
                    {
                        case 547: // Foreign Key: حذف عنصر مرتبط
                            statusCode = (int)HttpStatusCode.BadRequest;
                            message = "لا يمكن حذف أو تعديل هذا السجل لارتباطه ببيانات أخرى (مثلاً موظف مرتبط بقسم)";
                            break;
                        case 2601: // Unique Index: تكرار قيمة فريدة
                        case 2627: // Primary Key: تكرار رقم الهوية أو الرمز
                            statusCode = (int)HttpStatusCode.BadRequest;
                            message = "هذه البيانات (الرمز أو الاسم أو المعرف) مسجلة مسبقاً في النظام";
                            break;
                        case 1205: // Deadlock: تعليق قاعدة البيانات
                            statusCode = (int)HttpStatusCode.ServiceUnavailable;
                            message = "النظام مزدحم حالياً، يرجى المحاولة بعد ثوانٍ";
                            break;
                        default:
                            message = "خطأ داخلي في معالجة بيانات المستشفى";
                            break;
                    }
                }
                break;

            // 5. أخطاء الصلاحيات والوصول
            case UnauthorizedAccessException:
                statusCode = (int)HttpStatusCode.Forbidden;
                message = "ليس لديك الصلاحيات الكافية لتنفيذ هذا الإجراء";
                break;

            // 6. أخطاء المهلة الزمنية (Timeout)
            case TimeoutException:
                statusCode = (int)HttpStatusCode.RequestTimeout;
                message = "استغرقت العملية وقتاً أطول من المتوقع، يرجى التحقق من جودة الاتصال";
                break;

            // 7. أي خطأ منطقي آخر تم برمجته يدوياً
            case InvalidOperationException invEx:
                message = invEx.Message;
                break;
        }

        context.Response.StatusCode = statusCode;

        // في بيئة التطوير، نظهر تفاصيل الخطأ كاملة للمطور
        if (_env.IsDevelopment() && errors.Count == 0)
        {
            errors.Add($"Technical Detail: {exception.Message}");
            if (exception.InnerException != null)
                errors.Add($"Inner Exception: {exception.InnerException.Message}");
        }

        // استخدام Result الموحد لضمان "المسطرة" مع الأنجولار
        var response = Result<string>.Failure(message, statusCode, errors);

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var jsonResponse = JsonSerializer.Serialize(response, options);

        await context.Response.WriteAsync(jsonResponse);
    }
}