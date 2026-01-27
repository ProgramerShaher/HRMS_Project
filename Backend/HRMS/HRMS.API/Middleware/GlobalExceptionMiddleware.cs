using System.Net;
using System.Text.Json;
using FluentValidation;
using HRMS.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using DbUpdateException = Microsoft.EntityFrameworkCore.DbUpdateException;

namespace HRMS.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
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
        _logger.LogError(exception, "Global Exception Caught: {Message}", exception.Message);

        context.Response.ContentType = "application/json";
        
        var responseModel = new { Status = 500, Message = "An error occurred.", Details = exception.Message };
        
        switch (exception)
        {
            case Application.Exceptions.NotFoundException notFoundEx:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                responseModel = new { Status = 404, Message = notFoundEx.Message, Details = "Resource not found" };
                break;

            case Application.Exceptions.ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var details = JsonSerializer.Serialize(validationEx.Errors);
                responseModel = new { Status = 400, Message = "Validation Failed", Details = details };
                break;
                
            case FluentValidation.ValidationException fluentEx: // Catch fluent native too just in case
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                responseModel = new { Status = 400, Message = "Validation Failed", Details = fluentEx.Message };
                break;

            case DbUpdateException dbEx:
                // Foreign Key Constraint Violation (SqlState 23503 in many DBs, or specific error numbers in SQL Server usually 547)
                // Checking for "The INSERT statement conflicted with the FOREIGN KEY constraint"
                if (dbEx.InnerException != null && dbEx.InnerException.Message.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    responseModel = new { Status = 400, Message = "العنصر المرتبط (مثل الموظف) غير موجود في النظام", Details = "Foreign Key Constraint Violation" };
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseModel = new { Status = 500, Message = "Database Error", Details = dbEx.InnerException?.Message ?? dbEx.Message };
                }
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel = new { Status = 500, Message = "Internal Server Error", Details = exception.Message };
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(responseModel);
        await context.Response.WriteAsync(jsonResponse);
    }
}
