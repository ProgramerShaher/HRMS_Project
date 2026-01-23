# Backend Migration Guide - HRMS Audit Security

## نظرة عامة

هذا الدليل يشرح كيفية تحديث الـ Backend API للعمل مع نظام Audit Security الجديد.

## التغييرات الرئيسية

### ما الذي تغير؟

**قبل:**
- كان الـ API يمرر `created_by` و `updated_by` كـ parameters لكل stored procedure
- المستخدم يمكنه نظرياً التلاعب بهذه القيم

**بعد:**
- تم إزالة جميع audit parameters من الـ stored procedures
- يجب تعيين User Context في بداية كل request
- الـ Database Triggers تتولى تعبئة audit fields تلقائياً

## خطوات الترحيل

### الخطوة 1: إضافة User Context Service

أنشئ service للتعامل مع User Context:

```csharp
// Services/OracleContextService.cs
public interface IOracleContextService
{
    Task SetUserContextAsync(string username, int userId, int? branchId = null, int? deptId = null);
    Task ClearUserContextAsync();
    Task<string> GetCurrentUserAsync();
}

public class OracleContextService : IOracleContextService
{
    private readonly IDbConnection _connection;
    
    public OracleContextService(IDbConnection connection)
    {
        _connection = connection;
    }
    
    public async Task SetUserContextAsync(string username, int userId, int? branchId = null, int? deptId = null)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "PKG_SECURITY_CONTEXT.SET_USER_INFO";
        cmd.CommandType = CommandType.StoredProcedure;
        
        cmd.Parameters.Add(new OracleParameter("p_username", OracleDbType.Varchar2, username, ParameterDirection.Input));
        cmd.Parameters.Add(new OracleParameter("p_user_id", OracleDbType.Int32, userId, ParameterDirection.Input));
        
        if (branchId.HasValue)
            cmd.Parameters.Add(new OracleParameter("p_branch_id", OracleDbType.Int32, branchId.Value, ParameterDirection.Input));
        
        if (deptId.HasValue)
            cmd.Parameters.Add(new OracleParameter("p_dept_id", OracleDbType.Int32, deptId.Value, ParameterDirection.Input));
        
        await cmd.ExecuteNonQueryAsync();
    }
    
    public async Task ClearUserContextAsync()
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "PKG_SECURITY_CONTEXT.CLEAR_CONTEXT";
        cmd.CommandType = CommandType.StoredProcedure;
        
        await cmd.ExecuteNonQueryAsync();
    }
    
    public async Task<string> GetCurrentUserAsync()
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT PKG_SECURITY_CONTEXT.GET_CURRENT_USER() FROM DUAL";
        cmd.CommandType = CommandType.Text;
        
        var result = await cmd.ExecuteScalarAsync();
        return result?.ToString();
    }
}
```

### الخطوة 2: إضافة Middleware

أنشئ middleware لتعيين Context تلقائياً:

```csharp
// Middleware/OracleContextMiddleware.cs
public class OracleContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<OracleContextMiddleware> _logger;
    
    public OracleContextMiddleware(RequestDelegate next, ILogger<OracleContextMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context, IOracleContextService oracleContext)
    {
        try
        {
            // تعيين User Context إذا كان المستخدم مصادق عليه
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var username = context.User.Identity.Name;
                var userIdClaim = context.User.FindFirst("user_id") ?? context.User.FindFirst(ClaimTypes.NameIdentifier);
                var branchIdClaim = context.User.FindFirst("branch_id");
                var deptIdClaim = context.User.FindFirst("dept_id");
                
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    int? branchId = branchIdClaim != null && int.TryParse(branchIdClaim.Value, out int bId) ? bId : null;
                    int? deptId = deptIdClaim != null && int.TryParse(deptIdClaim.Value, out int dId) ? dId : null;
                    
                    await oracleContext.SetUserContextAsync(username, userId, branchId, deptId);
                    
                    _logger.LogDebug("Oracle context set for user: {Username} (ID: {UserId})", username, userId);
                }
            }
            
            // تنفيذ الـ request
            await _next(context);
        }
        finally
        {
            // مسح Context في النهاية
            try
            {
                await oracleContext.ClearUserContextAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to clear Oracle context");
            }
        }
    }
}

// تسجيل الـ Middleware في Program.cs أو Startup.cs
public static class OracleContextMiddlewareExtensions
{
    public static IApplicationBuilder UseOracleContext(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<OracleContextMiddleware>();
    }
}
```

### الخطوة 3: تحديث Program.cs / Startup.cs

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// تسجيل الـ Service
builder.Services.AddScoped<IOracleContextService, OracleContextService>();

// ... باقي الـ services

var app = builder.Build();

// استخدام الـ Middleware (بعد Authentication)
app.UseAuthentication();
app.UseAuthorization();
app.UseOracleContext(); // ⭐ إضافة هنا

app.MapControllers();
app.Run();
```

### الخطوة 4: تحديث Repository Classes

#### مثال: EmployeeRepository

**قبل:**
```csharp
public async Task<EmployeeResult> CreateEmployeeAsync(CreateEmployeeDto dto, string currentUser)
{
    using var connection = await _connectionFactory.CreateConnectionAsync();
    using var command = connection.CreateCommand();
    
    command.CommandText = "HR_PERSONNEL.PKG_EMP_MANAGER.CREATE_NEW_EMPLOYEE";
    command.CommandType = CommandType.StoredProcedure;
    
    // Input parameters
    command.Parameters.Add("p_first_name_ar", OracleDbType.Varchar2, dto.FirstNameAr, ParameterDirection.Input);
    command.Parameters.Add("p_family_name_ar", OracleDbType.Varchar2, dto.FamilyNameAr, ParameterDirection.Input);
    command.Parameters.Add("p_full_name_en", OracleDbType.Varchar2, dto.FullNameEn, ParameterDirection.Input);
    command.Parameters.Add("p_national_id", OracleDbType.Varchar2, dto.NationalId, ParameterDirection.Input);
    command.Parameters.Add("p_nationality_id", OracleDbType.Int32, dto.NationalityId, ParameterDirection.Input);
    command.Parameters.Add("p_birth_date", OracleDbType.Date, dto.BirthDate, ParameterDirection.Input);
    command.Parameters.Add("p_gender", OracleDbType.Varchar2, dto.Gender, ParameterDirection.Input);
    command.Parameters.Add("p_job_id", OracleDbType.Int32, dto.JobId, ParameterDirection.Input);
    command.Parameters.Add("p_dept_id", OracleDbType.Int32, dto.DeptId, ParameterDirection.Input);
    command.Parameters.Add("p_basic_salary", OracleDbType.Decimal, dto.BasicSalary, ParameterDirection.Input);
    command.Parameters.Add("p_joining_date", OracleDbType.Date, dto.JoiningDate, ParameterDirection.Input);
    command.Parameters.Add("p_created_by", OracleDbType.Varchar2, currentUser, ParameterDirection.Input); // ❌ إزالة
    
    // Output parameters
    var empIdParam = command.Parameters.Add("o_employee_id", OracleDbType.Int32, ParameterDirection.Output);
    var empNumParam = command.Parameters.Add("o_employee_number", OracleDbType.Varchar2, 20, ParameterDirection.Output);
    
    await command.ExecuteNonQueryAsync();
    
    return new EmployeeResult
    {
        EmployeeId = Convert.ToInt32(empIdParam.Value),
        EmployeeNumber = empNumParam.Value.ToString()
    };
}
```

**بعد:**
```csharp
public async Task<EmployeeResult> CreateEmployeeAsync(CreateEmployeeDto dto)
{
    // ملاحظة: لا حاجة لـ currentUser parameter
    // الـ Middleware قام بتعيين الـ Context تلقائياً
    
    using var connection = await _connectionFactory.CreateConnectionAsync();
    using var command = connection.CreateCommand();
    
    command.CommandText = "HR_PERSONNEL.PKG_EMP_MANAGER.CREATE_NEW_EMPLOYEE";
    command.CommandType = CommandType.StoredProcedure;
    
    // Input parameters (بدون p_created_by)
    command.Parameters.Add("p_first_name_ar", OracleDbType.Varchar2, dto.FirstNameAr, ParameterDirection.Input);
    command.Parameters.Add("p_family_name_ar", OracleDbType.Varchar2, dto.FamilyNameAr, ParameterDirection.Input);
    command.Parameters.Add("p_full_name_en", OracleDbType.Varchar2, dto.FullNameEn, ParameterDirection.Input);
    command.Parameters.Add("p_national_id", OracleDbType.Varchar2, dto.NationalId, ParameterDirection.Input);
    command.Parameters.Add("p_nationality_id", OracleDbType.Int32, dto.NationalityId, ParameterDirection.Input);
    command.Parameters.Add("p_birth_date", OracleDbType.Date, dto.BirthDate, ParameterDirection.Input);
    command.Parameters.Add("p_gender", OracleDbType.Varchar2, dto.Gender, ParameterDirection.Input);
    command.Parameters.Add("p_job_id", OracleDbType.Int32, dto.JobId, ParameterDirection.Input);
    command.Parameters.Add("p_dept_id", OracleDbType.Int32, dto.DeptId, ParameterDirection.Input);
    command.Parameters.Add("p_basic_salary", OracleDbType.Decimal, dto.BasicSalary, ParameterDirection.Input);
    command.Parameters.Add("p_joining_date", OracleDbType.Date, dto.JoiningDate, ParameterDirection.Input);
    // ✅ لا حاجة لـ p_created_by - الـ Trigger سيتولى ذلك
    
    // Output parameters
    var empIdParam = command.Parameters.Add("o_employee_id", OracleDbType.Int32, ParameterDirection.Output);
    var empNumParam = command.Parameters.Add("o_employee_number", OracleDbType.Varchar2, 20, ParameterDirection.Output);
    
    await command.ExecuteNonQueryAsync();
    
    return new EmployeeResult
    {
        EmployeeId = Convert.ToInt32(empIdParam.Value),
        EmployeeNumber = empNumParam.Value.ToString()
    };
}
```

### الخطوة 5: تحديث Controllers

**قبل:**
```csharp
[HttpPost]
public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto dto)
{
    var currentUser = User.Identity.Name; // ❌ لم يعد مطلوباً
    var result = await _employeeRepository.CreateEmployeeAsync(dto, currentUser);
    return Ok(result);
}
```

**بعد:**
```csharp
[HttpPost]
public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto dto)
{
    // ✅ الـ Middleware قام بتعيين الـ Context تلقائياً
    var result = await _employeeRepository.CreateEmployeeAsync(dto);
    return Ok(result);
}
```

## قائمة التحديثات المطلوبة

### Packages التي تحتاج تحديث

| Package | Procedures تحتاج تعديل | Status |
|---------|------------------------|--------|
| PKG_EMP_MANAGER | CREATE_NEW_EMPLOYEE, UPDATE_EMPLOYEE_INFO | ✅ تم |
| PKG_LEAVE_MANAGER | REQUEST_LEAVE, APPROVE_LEAVE_REQUEST, MONTHLY_LEAVE_ACCRUAL, CARRY_FORWARD_BALANCE, CANCEL_LEAVE_REQUEST | ⏳ قيد التنفيذ |
| PKG_PAYROLL_MANAGER | GENERATE_PAYSLIP, RUN_MONTHLY_PAYROLL, APPROVE_PAYROLL_RUN, GRANT_LOAN, PROCESS_END_OF_SERVICE, ADD_PAYROLL_ADJUSTMENT | ⏳ قيد التنفيذ |
| PKG_ATTENDANCE_MANAGER | جميع الـ procedures | ⏳ قيد التنفيذ |
| PKG_PERFORMANCE_MANAGER | جميع الـ procedures | ⏳ قيد التنفيذ |
| PKG_SECURITY_MANAGER | جميع الـ procedures | ⏳ قيد التنفيذ |

### Repository Classes التي تحتاج تحديث

- `EmployeeRepository.cs` ✅
- `LeaveRepository.cs` ⏳
- `PayrollRepository.cs` ⏳
- `AttendanceRepository.cs` ⏳
- `PerformanceRepository.cs` ⏳
- `SecurityRepository.cs` ⏳

## الاختبار

### Unit Tests

```csharp
[Fact]
public async Task CreateEmployee_ShouldSetAuditFields_Automatically()
{
    // Arrange
    var dto = new CreateEmployeeDto { /* ... */ };
    
    // تعيين Context
    await _oracleContext.SetUserContextAsync("test_user", 999);
    
    // Act
    var result = await _repository.CreateEmployeeAsync(dto);
    
    // Assert
    var employee = await _repository.GetEmployeeByIdAsync(result.EmployeeId);
    Assert.Equal("test_user", employee.CreatedBy); // يجب أن يكون من الـ Context
    Assert.NotNull(employee.CreatedAt);
}
```

### Integration Tests

```csharp
[Fact]
public async Task CreateEmployee_WithMiddleware_ShouldSetContext()
{
    // Arrange
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
    
    var dto = new CreateEmployeeDto { /* ... */ };
    
    // Act
    var response = await client.PostAsJsonAsync("/api/employees", dto);
    
    // Assert
    response.EnsureSuccessStatusCode();
    var result = await response.Content.ReadFromJsonAsync<EmployeeResult>();
    
    // التحقق من الـ audit fields
    var employee = await GetEmployeeFromDb(result.EmployeeId);
    Assert.Equal("test_user", employee.CreatedBy);
}
```

## استكشاف الأخطاء

### المشكلة: "CREATED_BY is NULL في Database"

**السبب المحتمل:**
1. الـ Middleware لم يتم تسجيله
2. الـ User غير مصادق عليه
3. الـ Claims غير موجودة

**الحل:**
```csharp
// تأكد من ترتيب الـ Middleware
app.UseAuthentication(); // أولاً
app.UseAuthorization();  // ثانياً
app.UseOracleContext();  // ثالثاً
```

### المشكلة: "Context not set"

**السبب:** الـ Connection pool قد يعيد استخدام connection قديم

**الحل:**
```csharp
// تأكد من مسح الـ Context دائماً
try
{
    await _next(context);
}
finally
{
    await oracleContext.ClearUserContextAsync();
}
```

### المشكلة: "Multiple users in same session"

**السبب:** Connection pooling

**الحل:** تعيين Context في بداية كل request (الـ Middleware يفعل ذلك تلقائياً)

## Best Practices

1. **دائماً استخدم Middleware**: لا تعتمد على manual context setting
2. **مسح Context**: تأكد من مسح الـ context في finally block
3. **Logging**: سجّل عمليات تعيين الـ context للتدقيق
4. **Error Handling**: تعامل مع أخطاء الـ context بشكل صحيح
5. **Testing**: اختبر الـ context في unit tests و integration tests

## Migration Checklist

- [ ] إضافة `IOracleContextService` و `OracleContextService`
- [ ] إضافة `OracleContextMiddleware`
- [ ] تسجيل الـ Service في DI container
- [ ] تسجيل الـ Middleware في pipeline
- [ ] تحديث جميع Repository methods
- [ ] إزالة `currentUser` parameters من Controllers
- [ ] تحديث Unit Tests
- [ ] تحديث Integration Tests
- [ ] اختبار النظام بالكامل
- [ ] تحديث API Documentation

## الدعم

للمزيد من المعلومات:
- راجع `README.md` في مجلد Security
- راجع `implementation_plan.md` للتفاصيل الفنية
- شغّل `TEST_AUDIT_SECURITY.sql` للتحقق من Database
