# 📦 دليل تثبيت PL/SQL Packages

## 🎯 الـ Packages المتوفرة (6 packages)

### **1. PKG_EMP_MANAGER** - إدارة الموظفين
- **Schema:** HR_PERSONNEL
- **الوظائف:**
  - إضافة موظف جديد مع عقده
  - تحديث بيانات الموظف
  - حساب الراتب الإجمالي

### **2. PKG_LEAVE_MANAGER** - إدارة الإجازات
- **Schema:** HR_LEAVES
- **الوظائف:**
  - طلب إجازة جديدة
  - الموافقة/رفض الإجازات
  - حساب الرصيد المتبقي

### **3. PKG_PAYROLL_MANAGER** - إدارة الرواتب
- **Schema:** HR_PAYROLL
- **الوظائف:**
  - تشغيل دورة رواتب
  - حساب الاستقطاعات
  - إنشاء كشوف الرواتب

### **4. PKG_ATTENDANCE_MANAGER** - إدارة الحضور
- **Schema:** HR_ATTENDANCE
- **الوظائف:**
  - تسجيل الحضور/الانصراف
  - حساب ساعات العمل
  - إدارة الورديات

### **5. PKG_PERFORMANCE_MANAGER** - إدارة الأداء
- **Schema:** HR_PERFORMANCE
- **الوظائف:**
  - إنشاء تقييمات الأداء
  - تسجيل المخالفات
  - حساب التقييم النهائي

### **6. PKG_SECURITY_MANAGER** - إدارة الأمان
- **Schema:** HR_CORE
- **الوظائف:**
  - تشفير كلمات المرور
  - التحقق من الصلاحيات
  - إدارة الجلسات

---

## 🚀 طريقة التثبيت

### **الطريقة 1: تثبيت شامل (موصى به)**

```sql
-- من مجلد PLSQL
cd G:\HRMS_Hospital\DB\Database\PLSQL

-- نفذ السكربت الشامل
sqlplus sys@localhost:1522/FREEPDB1 as sysdba
@INSTALL_ALL_PACKAGES.sql
```

---

### **الطريقة 2: تثبيت يدوي**

نفذ الملفات بالترتيب:

```sql
-- 1. Employee Manager
@01_PKG_EMP_MANAGER_SPEC.sql
@02_PKG_EMP_MANAGER_BODY.sql

-- 2. Leave Manager
@03_PKG_LEAVE_MANAGER_SPEC.sql
@04_PKG_LEAVE_MANAGER_BODY.sql

-- 3. Payroll Manager
@05_PKG_PAYROLL_MANAGER_SPEC.sql
@06_PKG_PAYROLL_MANAGER_BODY.sql

-- 4. Attendance Manager
@07_PKG_ATTENDANCE_MANAGER_SPEC.sql
@08_PKG_ATTENDANCE_MANAGER_BODY.sql

-- 5. Performance Manager
@09_PKG_PERFORMANCE_MANAGER_SPEC.sql
@10_PKG_PERFORMANCE_MANAGER_BODY.sql

-- 6. Security Manager
@11_PKG_SECURITY_MANAGER_SPEC.sql
@12_PKG_SECURITY_MANAGER_BODY.sql
```

---

## ✅ التحقق من التثبيت

```sql
-- عرض جميع الـ Packages
SELECT 
    object_name, 
    object_type, 
    status
FROM all_objects
WHERE object_type IN ('PACKAGE', 'PACKAGE BODY')
  AND owner LIKE 'HR_%'
  AND object_name LIKE 'PKG_%'
ORDER BY object_name, object_type;
```

**النتيجة المتوقعة:** 12 سطر (6 PACKAGE + 6 PACKAGE BODY)

---

## 🔧 حل المشاكل

### **إذا ظهرت أخطاء:**

```sql
-- عرض الأخطاء
SELECT name, type, line, text
FROM all_errors
WHERE owner LIKE 'HR_%'
  AND name LIKE 'PKG_%'
ORDER BY name, sequence;
```

### **الأخطاء الشائعة:**

1. **ORA-01031: insufficient privileges**
   - الحل: تأكد من الاتصال بـ SYS as SYSDBA

2. **PLS-00201: identifier must be declared**
   - الحل: تأكد من تنفيذ SPEC قبل BODY

3. **ORA-00942: table or view does not exist**
   - الحل: تأكد من إنشاء الجداول أولاً

---

## 📞 استدعاء الـ Packages

### **مثال: إضافة موظف جديد**

```sql
DECLARE
    v_emp_id NUMBER;
    v_emp_number VARCHAR2(20);
BEGIN
    HR_PERSONNEL.PKG_EMP_MANAGER.CREATE_NEW_EMPLOYEE(
        p_first_name_ar => 'محمد',
        p_family_name_ar => 'أحمد',
        p_full_name_en => 'Mohammed Ahmed',
        p_national_id => '1234567890',
        p_nationality_id => 1,
        p_birth_date => TO_DATE('1990-01-01', 'YYYY-MM-DD'),
        p_gender => 'M',
        p_job_id => 1,
        p_dept_id => 1,
        p_basic_salary => 10000,
        p_joining_date => SYSDATE,
        p_created_by => 'ADMIN',
        o_employee_id => v_emp_id,
        o_employee_number => v_emp_number
    );
    
    DBMS_OUTPUT.PUT_LINE('Employee ID: ' || v_emp_id);
    DBMS_OUTPUT.PUT_LINE('Employee Number: ' || v_emp_number);
END;
/
```

---

## 🎯 الخطوات التالية

بعد تثبيت الـ Packages:
1. ✅ اختبر الـ Procedures الأساسية
2. ✅ أنشئ REST APIs في APEX
3. ✅ ابنِ واجهات APEX تستخدم الـ Packages

---

**جاهز للتثبيت!** 🚀
