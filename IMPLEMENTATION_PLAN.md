# 🏥 خطة تطوير نظام HRMS
## ASP.NET Core Web API + Angular

---

## 📋 نظرة عامة

سنقوم ببناء نظام إدارة موارد بشرية احترافي متكامل يتكون من:

### **Backend (ASP.NET Core 8.0)**
- ✅ **Clean Architecture** (موجود بالفعل)
  - HRMS.Core - Entities & Interfaces
  - HRMS.Application - Business Logic
  - HRMS.Infrastructure - Data Access & Oracle DB
  - HRMS.API - RESTful API

### **Frontend (Angular 18+)**
- 🆕 **سيتم إنشاؤه** - تطبيق Angular حديث
  - Material Design / PrimeNG
  - Responsive UI
  - State Management (NgRx أو Signals)
  - Authentication & Authorization

### **Database**
- ✅ **Oracle 23ai** (تم إعداده)
  - 75 جدول
  - Views, Triggers, Packages
  - Sample Data

---

## 🎯 المرحلة الأولى: إعداد Backend

### **1.1 مراجعة وتحديث المشروع الحالي**

#### ✅ **التحقق من البنية الموجودة:**
```
HRMS/
├── HRMS.Core/              # Domain Layer
│   ├── Entities/           # Database Models
│   └── Interfaces/         # Repository Interfaces
├── HRMS.Application/       # Business Logic Layer
│   ├── DTOs/              # Data Transfer Objects
│   ├── Services/          # Business Services
│   └── Validators/        # Validation Logic
├── HRMS.Infrastructure/    # Data Access Layer
│   ├── Data/              # DbContext
│   ├── Repositories/      # Repository Implementation
│   └── Migrations/        # EF Core Migrations
└── HRMS.API/              # Presentation Layer
    ├── Controllers/       # API Endpoints
    ├── Middleware/        # Custom Middleware
    └── Program.cs         # App Configuration
```

#### 🔧 **المهام المطلوبة:**

1. **تحديث Entities في HRMS.Core**
   - مطابقة الـ Entities مع جداول Oracle
   - إضافة Data Annotations
   - إضافة Navigation Properties

2. **إعداد Oracle DbContext**
   - تثبيت `Oracle.EntityFrameworkCore`
   - إنشاء `HRMSDbContext`
   - تكوين Connection String

3. **إنشاء Repository Pattern**
   - Generic Repository
   - Unit of Work
   - Specific Repositories لكل Entity

4. **إنشاء DTOs**
   - Request DTOs
   - Response DTOs
   - Mapping Profiles (AutoMapper)

5. **إنشاء Services**
   - Authentication Service
   - Department Service
   - Employee Service
   - Job Service
   - إلخ...

6. **إنشاء Controllers**
   - AuthController
   - DepartmentsController
   - EmployeesController
   - JobsController
   - إلخ...

7. **إضافة Security**
   - JWT Authentication
   - Authorization Policies
   - Role-based Access Control

8. **إضافة Features**
   - Logging (Serilog)
   - Exception Handling
   - Validation (FluentValidation)
   - CORS Configuration
   - Swagger/OpenAPI

---

## 🎨 المرحلة الثانية: إنشاء Frontend

### **2.1 إنشاء مشروع Angular**

```bash
cd G:/HRMS_Hospital/Frontend
ng new hrms-frontend --routing --style=scss --strict
```

#### **الخيارات:**
- ✅ Routing: Yes
- ✅ Style: SCSS
- ✅ Strict Mode: Yes
- ✅ Standalone Components: Yes (Angular 18+)

### **2.2 تثبيت المكتبات الأساسية**

```bash
# UI Framework - PrimeNG (موصى به للأنظمة الإدارية)
npm install primeng primeicons
npm install primeflex

# أو Material Design
npm install @angular/material @angular/cdk

# HTTP & Forms
npm install @angular/common @angular/forms

# State Management
npm install @ngrx/store @ngrx/effects @ngrx/entity

# Authentication
npm install @auth0/angular-jwt

# Utilities
npm install date-fns lodash-es
npm install --save-dev @types/lodash-es

# Charts
npm install chart.js ng2-charts

# Icons
npm install @fortawesome/fontawesome-free
```

### **2.3 هيكل المشروع**

```
hrms-frontend/
├── src/
│   ├── app/
│   │   ├── core/                    # الخدمات الأساسية
│   │   │   ├── guards/              # Route Guards
│   │   │   ├── interceptors/        # HTTP Interceptors
│   │   │   ├── services/            # API Services
│   │   │   └── models/              # TypeScript Models
│   │   │
│   │   ├── shared/                  # المكونات المشتركة
│   │   │   ├── components/          # Reusable Components
│   │   │   ├── directives/          # Custom Directives
│   │   │   ├── pipes/               # Custom Pipes
│   │   │   └── validators/          # Form Validators
│   │   │
│   │   ├── features/                # الميزات الرئيسية
│   │   │   ├── auth/                # تسجيل الدخول
│   │   │   │   ├── login/
│   │   │   │   └── register/
│   │   │   │
│   │   │   ├── dashboard/           # لوحة التحكم
│   │   │   │   ├── dashboard.component.ts
│   │   │   │   └── widgets/
│   │   │   │
│   │   │   ├── hr-core/             # البيانات الأساسية
│   │   │   │   ├── departments/
│   │   │   │   ├── jobs/
│   │   │   │   ├── branches/
│   │   │   │   ├── banks/
│   │   │   │   └── countries/
│   │   │   │
│   │   │   ├── employees/           # إدارة الموظفين
│   │   │   │   ├── employee-list/
│   │   │   │   ├── employee-form/
│   │   │   │   └── employee-details/
│   │   │   │
│   │   │   ├── attendance/          # الحضور والانصراف
│   │   │   ├── payroll/             # الرواتب
│   │   │   ├── leaves/              # الإجازات
│   │   │   ├── performance/         # تقييم الأداء
│   │   │   └── reports/             # التقارير
│   │   │
│   │   ├── layout/                  # التخطيط العام
│   │   │   ├── header/
│   │   │   ├── sidebar/
│   │   │   ├── footer/
│   │   │   └── main-layout/
│   │   │
│   │   └── app.routes.ts            # Routing Configuration
│   │
│   ├── assets/                      # الموارد الثابتة
│   │   ├── images/
│   │   ├── icons/
│   │   └── i18n/                    # ملفات الترجمة
│   │
│   ├── environments/                # إعدادات البيئة
│   │   ├── environment.ts
│   │   └── environment.prod.ts
│   │
│   └── styles/                      # الأنماط العامة
│       ├── _variables.scss
│       ├── _mixins.scss
│       └── styles.scss
```

---

## 🚀 المرحلة الثالثة: التكامل

### **3.1 ربط Frontend بـ Backend**

1. **إنشاء API Services في Angular**
   - AuthService
   - DepartmentService
   - EmployeeService
   - إلخ...

2. **إعداد HTTP Interceptors**
   - JWT Token Interceptor
   - Error Handling Interceptor
   - Loading Interceptor

3. **إعداد Route Guards**
   - AuthGuard
   - RoleGuard

### **3.2 إنشاء الصفحات الرئيسية**

#### **الأولوية العالية:**
1. ✅ **صفحة تسجيل الدخول** (Login)
2. ✅ **لوحة التحكم** (Dashboard)
3. ✅ **إدارة الأقسام** (Departments CRUD)
4. ✅ **إدارة الوظائف** (Jobs CRUD)
5. ✅ **إدارة الموظفين** (Employees CRUD)

#### **الأولوية المتوسطة:**
6. إدارة الفروع (Branches)
7. إدارة البنوك (Banks)
8. إدارة المدن والدول (Cities & Countries)
9. الحضور والانصراف (Attendance)
10. الإجازات (Leaves)

#### **الأولوية المنخفضة:**
11. الرواتب (Payroll)
12. تقييم الأداء (Performance)
13. التدريب (Training)
14. التقارير (Reports)

---

## 📊 المرحلة الرابعة: الميزات المتقدمة

### **4.1 Dashboard Features**
- 📈 إحصائيات في الوقت الفعلي
- 📊 رسوم بيانية تفاعلية
- 🔔 الإشعارات
- 📅 التقويم
- 🎯 المهام السريعة

### **4.2 Advanced Features**
- 🔍 بحث متقدم وفلترة
- 📤 تصدير البيانات (Excel, PDF)
- 📧 إرسال البريد الإلكتروني
- 📱 Responsive Design
- 🌐 Multi-language (AR/EN)
- 🌙 Dark Mode
- 📸 رفع الصور والملفات

---

## 🛠️ الأدوات والتقنيات

### **Backend Stack:**
- ASP.NET Core 8.0
- Entity Framework Core
- Oracle.EntityFrameworkCore
- AutoMapper
- FluentValidation
- Serilog
- JWT Authentication
- Swagger/OpenAPI

### **Frontend Stack:**
- Angular 18+
- PrimeNG / Angular Material
- RxJS
- NgRx (State Management)
- TypeScript
- SCSS
- Chart.js

### **DevOps:**
- Git & GitHub
- Docker (اختياري)
- CI/CD (اختياري)

---

## ✅ خطة العمل المقترحة

### **الأسبوع الأول: Backend Setup**
- [ ] مراجعة وتحديث Entities
- [ ] إعداد Oracle DbContext
- [ ] إنشاء Repositories
- [ ] إنشاء DTOs & Mapping
- [ ] إنشاء Authentication Service
- [ ] إنشاء Controllers الأساسية

### **الأسبوع الثاني: Frontend Setup**
- [ ] إنشاء مشروع Angular
- [ ] تثبيت المكتبات
- [ ] إنشاء Layout Components
- [ ] إنشاء Auth Module
- [ ] إنشاء Dashboard
- [ ] إنشاء API Services

### **الأسبوع الثالث: Core Features**
- [ ] Departments CRUD
- [ ] Jobs CRUD
- [ ] Employees CRUD
- [ ] Branches CRUD
- [ ] Testing & Debugging

### **الأسبوع الرابع: Advanced Features**
- [ ] Attendance Module
- [ ] Leaves Module
- [ ] Reports
- [ ] Final Testing
- [ ] Deployment

---

## 🎯 الخطوة التالية الموصى بها

**ابدأ بـ:**

1. **فحص Backend الحالي** - التأكد من جاهزية المشروع
2. **إنشاء مشروع Angular** - بناء Frontend من الصفر
3. **إنشاء صفحة Login** - أول صفحة في التطبيق
4. **إنشاء Dashboard** - الصفحة الرئيسية بعد تسجيل الدخول

---

## 📞 هل أنت جاهز؟

**اختر ما تريد البدء به:**

1. 🔍 **فحص وتحديث Backend** (مراجعة المشروع الحالي)
2. 🆕 **إنشاء مشروع Angular** (البدء بـ Frontend)
3. 📚 **شرح تفصيلي للبنية** (فهم Clean Architecture)
4. 🚀 **البدء مباشرة بالتطوير** (كود جاهز)

**ما هو اختيارك؟** 😊
