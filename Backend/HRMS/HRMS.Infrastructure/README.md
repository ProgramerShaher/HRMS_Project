# 🏗️ Infrastructure Layer - طبقة البنية التحتية

## 📋 الوصف
طبقة البنية التحتية مسؤولة عن التعامل مع قاعدة البيانات.
**تم اعتماد أفضل ممارسة: CQRS + DbContext مباشرة (بدون Repository Pattern).**

---

## 📂 الهيكل التنظيمي

```
HRMS.Infrastructure/
├── Data/
│   └── HRMSDbContext.cs          # سياق قاعدة البيانات الرئيسي
└── DependencyInjection.cs         # تسجيل الخدمات
```

---

## 🔧 المكونات الرئيسية

### 1. **HRMSDbContext**
- **الموقع**: `Data/HRMSDbContext.cs`
- **الوصف**: سياق Entity Framework Core الرئيسي
- **المحتوى**:
  - 50+ DbSet لجميع الكيانات
  - إعدادات الاتصال بـ SQL Server

### 2. **DependencyInjection**
- **الموقع**: `DependencyInjection.cs`
- **الوصف**: تسجيل خدمات الطبقة
- **الإعدادات**:
  - تكوين DbContext
  - Retry Logic للأخطاء المؤقتة (5 محاولات)
  - Command Timeout (60 ثانية)

---

## ⚙️ الإعدادات

### Connection String
في `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HRMS_Hospital;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### تسجيل الخدمات
في `Program.cs`:

```csharp
builder.Services.AddInfrastructure(builder.Configuration);
```

---

## 🎯 أفضل الممارسات المطبقة

1. ✅ **CQRS Pattern** - فصل القراءة عن الكتابة
2. ✅ **DbContext مباشرة** - بدون Repository (أبسط وأسرع)
3. ✅ **AsNoTracking** - للقراءة (أداء أفضل)
4. ✅ **Include** - جلب البيانات المرتبطة
5. ✅ **Retry Logic** - التعامل مع الأخطاء المؤقتة
6. ✅ **Regions** - تنظيم الكود
7. ✅ **XML Documentation** - توثيق شامل بالعربية

---

## 🚀 الاستخدام

### Commands (الكتابة):
```csharp
public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, int>
{
    private readonly HRMSDbContext _context;

    public async Task<int> Handle(CreateEmployeeCommand request, CancellationToken ct)
    {
        var employee = _mapper.Map<Employee>(request);
        
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(ct);
        
        return employee.EmployeeId;
    }
}
```

### Queries (القراءة):
```csharp
public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto>
{
    private readonly HRMSDbContext _context;

    public async Task<EmployeeDto> Handle(GetEmployeeByIdQuery request, CancellationToken ct)
    {
        var employee = await _context.Employees
            .AsNoTracking() // أداء أفضل
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.EmployeeId == request.Id, ct);
            
        return _mapper.Map<EmployeeDto>(employee);
    }
}
```

### Transactions (المعاملات المعقدة):
```csharp
using var transaction = await _context.Database.BeginTransactionAsync(ct);
try
{
    _context.Employees.Add(employee);
    _context.Contracts.Add(contract);
    await _context.SaveChangesAsync(ct);
    await transaction.CommitAsync(ct);
}
catch
{
    await transaction.RollbackAsync(ct);
    throw;
}
```

---

## 📊 الإحصائيات

- **عدد الـ DbSets**: 50+
- **عدد الـ Schemas**: 7 (Core, Personnel, Attendance, Leaves, Payroll, Recruitment, Performance)
- **النمط المعماري**: Clean Architecture + CQRS

---

## ✅ الحالة
- [x] DbContext مكتمل
- [x] DependencyInjection محدث
- [x] Repository Pattern محذوف (أفضل ممارسة)
- [x] Handlers محدثة لاستخدام DbContext
- [x] التوثيق مكتمل
