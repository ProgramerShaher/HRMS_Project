using HRMS.API.Middlewares;
using HRMS.Application;
using HRMS.Application.Interfaces;
using HRMS.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// 1. Setup Serilog (Logging)
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

// 2. Register Layer Services (Clean Architecture)
builder.Services.AddInfrastructure(builder.Configuration);

// 3. Register MediatR (CQRS Pattern)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(HRMS.Application.AssemblyReference).Assembly));

// 4. Register AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// 5. Register Identity
builder.Services.AddIdentity<HRMS.Core.Entities.Identity.ApplicationUser, HRMS.Core.Entities.Identity.ApplicationRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<HRMS.Infrastructure.Data.HRMSDbContext>()
.AddDefaultTokenProviders();

// 6. Register JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<HRMS.Application.Settings.JwtSettings>(jwtSettings);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(jwtSettings["Secret"]!))
    };
});

// 7. Register AuthService
builder.Services.AddScoped<IAuthService, HRMS.Application.Services.AuthService>();


// 3. Swagger Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "HRMS Hospital API",
        Version = "v1",
        Description = "نظام إدارة الموارد البشرية - واجهة برمجية متكاملة",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "فريق التطوير",
            Email = "support@hrms.com"
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// 4. Global Exception Handler
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // هذا هو المسار الذي سيظهر في المتصفح
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "HRMS API V1");

        // إذا أردت أن يفتح Swagger مباشرة عند تشغيل المشروع (بدون كتابة /swagger)
        // options.RoutePrefix = string.Empty; 
    });
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed Default Roles and Admin User
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<HRMS.Core.Entities.Identity.ApplicationUser>>();
        var roleManager = services.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<HRMS.Core.Entities.Identity.ApplicationRole>>();
        
        await HRMS.Infrastructure.Data.Seeders.RoleSeeder.SeedRolesAsync(roleManager);
        await HRMS.Infrastructure.Data.Seeders.RoleSeeder.SeedDefaultAdminAsync(userManager, roleManager);
        
        Log.Information("Default roles and admin user seeded successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while seeding roles");
    }
}


try
{
    Log.Information("Starting Web API...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
