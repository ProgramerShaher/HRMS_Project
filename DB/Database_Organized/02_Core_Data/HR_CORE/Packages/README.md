# 📦 دليل Packages - HR_CORE

## 📂 الهيكل

```
Packages/
├── Specifications/          ← تعريفات الحزم (Specs)
│   ├── 01_PKG_SECURITY_MANAGER_SPEC.sql
│   └── 03_PKG_CORE_MANAGER_SPEC.sql
│
└── Bodies/                  ← تنفيذ الحزم (Bodies)
    ├── 02_PKG_SECURITY_MANAGER_BODY.sql
    └── PKG_CORE_MANAGER_BODY.sql
```

---

## 🎯 ترتيب التنفيذ

### الخطوة 1: تنفيذ جميع Specifications
```bash
sqlplus HR_CORE/Pwd_Core_123@FREEPDB1
@Specifications/01_PKG_SECURITY_MANAGER_SPEC.sql
@Specifications/03_PKG_CORE_MANAGER_SPEC.sql
```

### الخطوة 2: تنفيذ جميع Bodies
```bash
@Bodies/02_PKG_SECURITY_MANAGER_BODY.sql
@Bodies/PKG_CORE_MANAGER_BODY.sql
```

---

## 📋 الحزم المتاحة

### 1. **PKG_SECURITY_MANAGER**
- إدارة الأمان والمستخدمين
- Security Context
- Audit Logging

### 2. **PKG_CORE_MANAGER**
- إدارة الأقسام (Departments)
- إدارة الوظائف (Jobs)
- إدارة الفروع (Branches)
- إدارة المدن (Cities)
- إدارة الدول (Countries)
- إدارة البنوك (Banks)
- إدارة الدرجات الوظيفية (Job Grades)
- إدارة أنواع الوثائق (Document Types)

---

## ✅ Best Practice

هذا الهيكل يتبع أفضل الممارسات:
- ✅ فصل واضح بين التعريف والتنفيذ
- ✅ سهولة الصيانة
- ✅ ترتيب منطقي للتنفيذ
- ✅ احترافي ومنظم

---

**جاهز للاستخدام!** 🚀
