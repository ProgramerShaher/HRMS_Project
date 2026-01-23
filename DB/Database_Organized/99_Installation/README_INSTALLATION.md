# 📘 دليل التنصيب الكامل - HRMS Database

## 🎯 نظرة عامة

هذا الدليل يشرح كيفية تنصيب قاعدة بيانات HRMS بالكامل على Oracle 23ai Free.

---

## 📋 المتطلبات

- ✅ Oracle Database 23ai Free
- ✅ SQL*Plus أو SQL Developer
- ✅ صلاحيات SYSDBA

---

## 🚀 خطوات التنصيب

### الخطوة 1: الاتصال بقاعدة البيانات

```bash
sqlplus sys/password@FREEPDB1 as sysdba
```

### الخطوة 2: تشغيل سكربت التنصيب الرئيسي

```sql
@G:/HRMS_Hospital/DB/Database_Organized/99_Installation/00_MASTER_INSTALL.sql
```

**هذا السكربت سيقوم بـ:**
1. إنشاء جميع Schemas (7 schemas)
2. إنشاء جميع الجداول (75 جدول)
3. إنشاء جميع Foreign Keys (92 علاقة)
4. منح الصلاحيات بين Schemas
5. إنشاء جميع Packages (7 packages)
6. إنشاء جميع Views (52 view)
7. إنشاء جميع Triggers (30 trigger)

**الوقت المتوقع:** 5-10 دقائق

---

## 🔧 التنصيب التدريجي (اختياري)

إذا كنت تريد التنصيب خطوة بخطوة:

### 1. إنشاء Schemas

```sql
@01_Schemas/00_CREATE_ALL_SCHEMAS.sql
```

### 2. تنصيب HR_CORE (البيانات الأساسية)

```sql
@99_Installation/01_INSTALL_CORE.sql
```

### 3. تنصيب HR_PERSONNEL (الموظفين)

```sql
@99_Installation/02_INSTALL_PERSONNEL.sql
```

### 4. تنصيب HR_ATTENDANCE (الحضور)

```sql
@99_Installation/03_INSTALL_ATTENDANCE.sql
```

### 5. تنصيب HR_LEAVES (الإجازات)

```sql
@99_Installation/04_INSTALL_LEAVES.sql
```

### 6. تنصيب HR_PAYROLL (الرواتب)

```sql
@99_Installation/05_INSTALL_PAYROLL.sql
```

### 7. تنصيب HR_RECRUITMENT (التوظيف) - اختياري

```sql
@99_Installation/06_INSTALL_RECRUITMENT.sql
```

### 8. تنصيب HR_PERFORMANCE (الأداء) - اختياري

```sql
@99_Installation/07_INSTALL_PERFORMANCE.sql
```

### 9. منح الصلاحيات

```sql
@09_Permissions/01_GRANT_CORE_PERMISSIONS.sql
@09_Permissions/02_GRANT_PERSONNEL_PERMISSIONS.sql
@09_Permissions/03_GRANT_ATTENDANCE_PERMISSIONS.sql
@09_Permissions/04_GRANT_LEAVES_PERMISSIONS.sql
@09_Permissions/05_GRANT_PAYROLL_PERMISSIONS.sql
```

### 10. إدخال بيانات تجريبية (اختياري)

```sql
@10_Sample_Data/01_CORE_DATA.sql
@10_Sample_Data/02_EMPLOYEES_DATA.sql
@10_Sample_Data/03_OPERATIONAL_DATA.sql
```

---

## ✅ التحقق من التنصيب

### 1. التحقق من Schemas

```sql
SELECT username, account_status 
FROM dba_users 
WHERE username LIKE 'HR_%'
ORDER BY username;
```

**النتيجة المتوقعة:** 7 schemas

### 2. التحقق من الجداول

```sql
SELECT owner, COUNT(*) as table_count
FROM all_tables
WHERE owner LIKE 'HR_%'
GROUP BY owner
ORDER BY owner;
```

**النتيجة المتوقعة:** 75 جدول موزعة على 7 schemas

### 3. التحقق من Packages

```sql
SELECT owner, object_name, status
FROM all_objects
WHERE owner LIKE 'HR_%'
  AND object_type = 'PACKAGE'
ORDER BY owner, object_name;
```

**النتيجة المتوقعة:** 7 packages (كلها VALID)

### 4. التحقق من Views

```sql
SELECT owner, COUNT(*) as view_count
FROM all_views
WHERE owner LIKE 'HR_%'
GROUP BY owner
ORDER BY owner;
```

**النتيجة المتوقعة:** 52 view

### 5. التحقق من Triggers

```sql
SELECT owner, COUNT(*) as trigger_count
FROM all_triggers
WHERE owner LIKE 'HR_%'
GROUP BY owner
ORDER BY owner;
```

**النتيجة المتوقعة:** 30 trigger

---

## 🔍 استكشاف الأخطاء

### مشكلة: ORA-01920: user name 'HR_CORE' conflicts

**الحل:**
```sql
-- حذف Schema القديم
DROP USER HR_CORE CASCADE;

-- إعادة التنصيب
@01_Schemas/00_CREATE_ALL_SCHEMAS.sql
```

### مشكلة: ORA-00942: table or view does not exist

**الحل:**
تأكد من تنصيب Schemas بالترتيب الصحيح:
1. HR_CORE أولاً
2. HR_PERSONNEL ثانياً
3. باقي Schemas

### مشكلة: ORA-04043: object PKG_EMP_MANAGER does not exist

**الحل:**
```sql
-- التحقق من حالة Package
SELECT object_name, status 
FROM user_objects 
WHERE object_name = 'PKG_EMP_MANAGER';

-- إعادة compile إذا كان INVALID
ALTER PACKAGE PKG_EMP_MANAGER COMPILE;
```

---

## 📊 ملخص التنصيب

| المكون | العدد | الحالة |
|--------|------|---------|
| Schemas | 7 | ✅ |
| Tables | 75 | ✅ |
| Foreign Keys | 92 | ✅ |
| Packages | 7 | ✅ |
| Views | 52 | ✅ |
| Triggers | 30 | ✅ |

---

## 🎯 الخطوات التالية

بعد التنصيب الناجح:

1. ✅ **اختبار النظام** - إضافة موظف تجريبي
2. ✅ **إنشاء واجهات APEX** - للموظفين والحضور والرواتب
3. ✅ **إعداد REST APIs** - للتكامل مع التطبيقات الخارجية

---

## 📞 الدعم

لأي استفسارات:
- راجع ملف `README.md` في كل مجلد
- تحقق من `AUDIT_LOGS` للأخطاء
- راجع Oracle Alert Log

---

**النظام جاهز 100% للاستخدام!** ✅
