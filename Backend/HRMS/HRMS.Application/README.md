# 🏗️ Application Layer - طبقة التطبيق

## 📋 الوصف
طبقة التطبيق تحتوي على **جميع منطق العمل (Business Logic)** باستخدام نمط CQRS.

---

## 📂 الهيكل التنظيمي

```
HRMS.Application/
├── Features/                    # الميزات (حسب الموديول)
│   ├── Core/                   # الميزات الأساسية
│   │   └── Branches/
│   │       ├── Commands/       # عمليات الكتابة
│   │       │   └── CreateBranch/
│   │       │       ├── CreateBranchCommand.cs          # البيانات
│   │       │       ├── CreateBranchCommandHandler.cs   # المنطق 🔥
│   │       │       └── CreateBranchCommandValidator.cs # التحقق ✅
│   │       └── Queries/        # عمليات القراءة
│   │           ├── GetAllBranches/
│   │           │   ├── GetAllBranchesQuery.cs
│   │           │   └── GetAllBranchesQueryHandler.cs
│   │           └── GetBranchById/
│   │               ├── GetBranchByIdQuery.cs
│   │               └── GetBranchByIdQueryHandler.cs
│   │
│   └── Personnel/              # شؤون الموظفين
│       └── Employees/
│           ├── Commands/
│           │   └── CreateEmployee/
│           │       ├── CreateEmployeeCommand.cs
│           │       ├── CreateEmployeeCommandHandler.cs
│           │       └── CreateEmployeeCommandValidator.cs
│           └── Queries/
│               └── GetEmployeeById/
│
└── Mappings/                   # AutoMapper Profiles
    └── EmployeeMappingProfile.cs
```

---

## 🎯 أين يكون المنطق؟

### 1. **Handlers** (المعالجات) - 🔥 المنطق الرئيسي

```csharp
public class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, int>
{
    private readonly HRMSDbContext _context;

    public async Task<int> Handle(CreateBranchCommand request, CancellationToken ct)
    {
        // ✅ هنا يكون المنطق:
        // 1. التحقق من البيانات (إذا لزم)
        // 2. إنشاء الكائن
        // 3. حفظ في قاعدة البيانات
        // 4. إرجاع النتيجة
        
        var branch = new Branch
        {
            BranchNameAr = request.BranchNameAr,
            // ... منطق إضافي
        };

        _context.Branches.Add(branch);
        await _context.SaveChangesAsync(ct);

        return branch.BranchId;
    }
}
```

### 2. **Validators** (المُحققات) - ✅ التحقق من البيانات

```csharp
public class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchCommandValidator()
    {
        // ✅ هنا قواعد التحقق:
        RuleFor(x => x.BranchNameAr)
            .NotEmpty().WithMessage("اسم الفرع مطلوب")
            .MaximumLength(100);

        RuleFor(x => x.CityId)
            .GreaterThan(0).When(x => x.CityId.HasValue);
    }
}
```

---

## 🔄 تدفق العمل (Workflow)

```
1. Controller يستقبل الطلب
   ↓
2. يرسله إلى MediatR
   ↓
3. MediatR يشغل Validator أولاً ✅
   ↓
4. إذا نجح التحقق → يشغل Handler 🔥
   ↓
5. Handler ينفذ المنطق ويحفظ في DB
   ↓
6. يرجع النتيجة للـ Controller
   ↓
7. Controller يرجعها للعميل
```

---

## 📝 مثال كامل

### Command (البيانات):
```csharp
public class CreateBranchCommand : IRequest<int>
{
    public string BranchNameAr { get; set; }
    public string BranchNameEn { get; set; }
}
```

### Validator (التحقق):
```csharp
public class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchCommandValidator()
    {
        RuleFor(x => x.BranchNameAr).NotEmpty();
    }
}
```

### Handler (المنطق):
```csharp
public class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, int>
{
    public async Task<int> Handle(CreateBranchCommand request, CancellationToken ct)
    {
        // المنطق هنا
        var branch = new Branch { ... };
        _context.Add(branch);
        await _context.SaveChangesAsync(ct);
        return branch.BranchId;
    }
}
```

### Controller (نظيف):
```csharp
[HttpPost]
public async Task<ActionResult<int>> Create([FromBody] CreateBranchCommand command)
{
    var id = await _mediator.Send(command); // فقط!
    return CreatedAtAction(nameof(GetById), new { id }, id);
}
```

---

## ✅ الفوائد

1. **Controller نظيف** - لا يوجد أي منطق
2. **Validation منفصل** - سهل الاختبار
3. **Business Logic مركزي** - في الـ Handlers
4. **قابل للتوسع** - سهل إضافة ميزات جديدة

---

## 🚀 الخطوة التالية

هل تريد:
1. إضافة **Validation Pipeline** لتشغيل Validators تلقائياً؟
2. إنشاء باقي الـ Features بنفس النمط؟
