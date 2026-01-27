using HRMS.API.Extensions;
using Serilog;

#region Serilog Configuration

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/hrms-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

Log.Information("🚀 Starting HRMS Hospital API...");

#endregion

try
{
    var builder = WebApplication.CreateBuilder(args);
    
    #region Serilog Integration

    builder.Host.UseSerilog();

    #endregion

    #region Service Registration

    // 1. Database Services
    builder.Services.AddDatabaseServices(builder.Configuration);

    // 2. Application Services (MediatR, AutoMapper, FluentValidation)
    builder.Services.AddApplicationServices();

    // 3. Identity Services
    builder.Services.AddIdentityServices();

    // 4. JWT Authentication
    builder.Services.AddJwtAuthentication(builder.Configuration);

    // 5. Authorization Policies (Development Mode: Disabled)
    builder.Services.AddAuthorizationPolicies(isDevelopment: true);
    // 🔓 لتفعيل Authorization: غير isDevelopment إلى false

    // 6. Custom Application Services
    builder.Services.AddCustomServices();

    // 7. Swagger Documentation
    builder.Services.AddSwaggerDocumentation();

    // 8. Controllers
    builder.Services.AddControllers();

    // 9. CORS Policy
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    #endregion

    var app = builder.Build();

    #region Middleware Pipeline

    // Configure Middleware
    app.UseCustomMiddleware(app.Environment);

    // Swagger Documentation
    app.UseSwaggerDocumentation();

    // Map Controllers
    app.MapControllers();

    #endregion

    #region Database Seeding

    // Seed Default Roles and Admin User
    //await app.SeedDatabaseAsync();

    #endregion

    #region Run Application

    Log.Information("✅ HRMS Hospital API started successfully");
    app.Run();

    #endregion
}
catch (Exception ex)
{
    // 💡 هنا السر: استخراج الخطأ الحقيقي من داخل الـ AggregateException
    var realError = ex;
    if (ex is AggregateException aggEx)
    {
        realError = aggEx.Flatten().InnerExceptions.First();
    }

    Console.WriteLine("====================================================");
    Console.WriteLine("❌ ERROR DETECTED:");
    Console.WriteLine(realError.Message);
    if (realError.InnerException != null)
    {
        Console.WriteLine("🔗 INNER EXCEPTION:");
        Console.WriteLine(realError.InnerException.Message);
    }
    Console.WriteLine("====================================================");

    Log.Fatal(realError, "❌ Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
