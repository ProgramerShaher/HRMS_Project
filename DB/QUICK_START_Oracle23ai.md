# 🎯 دليل التثبيت السريع - Oracle 23ai

## ✅ الوضع الحالي

لديك الـ Schemas جاهزة! لا تحتاج لإعادة إنشائها.

---

## 📋 الخطوات (3 ملفات فقط)

### **1. إنشاء الجداول**
```sql
COMPLETE_INSTALL_PART1.sql
```
✅ تم تعديله لـ FREEPDB1

### **2. إضافة Foreign Keys**
```sql
COMPLETE_INSTALL_PART2.sql
```
✅ تم تعديله لـ FREEPDB1

### **3. منح الصلاحيات**
```sql
10_GRANT_PERMISSIONS.sql
```

---

## 🚀 كيفية التنفيذ

### **من SQL Developer:**

1. اتصل بـ **SYS as SYSDBA**:
   - Service Name: `FREEPDB1`
   - Role: `SYSDBA`

2. نفذ الملفات بالترتيب (F5):
   - `COMPLETE_INSTALL_PART1.sql`
   - `COMPLETE_INSTALL_PART2.sql`
   - `10_GRANT_PERMISSIONS.sql`

### **من Command Line:**

```cmd
cd G:\HRMS_Hospital\DB\Database\Oracle_DDL

sqlplus sys@FREEPDB1 as sysdba @COMPLETE_INSTALL_PART1.sql
sqlplus sys@FREEPDB1 as sysdba @COMPLETE_INSTALL_PART2.sql
sqlplus sys@FREEPDB1 as sysdba @10_GRANT_PERMISSIONS.sql
```

---

## ✨ ميزات Oracle 23ai (اختياري - بعد التثبيت)

بعد إنشاء الجداول، يمكنك إضافة:

### **JSON Duality Views للـ API:**
```sql
CREATE OR REPLACE JSON RELATIONAL DUALITY VIEW employee_api AS
SELECT JSON {
    'id': e.EMPLOYEE_ID,
    'name': e.FULL_NAME_EN,
    'email': e.EMAIL,
    'department': d.DEPT_NAME_AR
}
FROM HR_PERSONNEL.EMPLOYEES e
JOIN HR_CORE.DEPARTMENTS d ON e.DEPT_ID = d.DEPT_ID;
```

---

## 🎯 الخلاصة

**لا تحتاج schemas جديدة!**

الموجودة لديك ممتازة وجاهزة للاستخدام مع Oracle 23ai.

فقط نفذ الملفات الثلاثة وابدأ العمل! 🚀
