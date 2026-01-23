# 📘 دليل التنصيب الصحيح - تجنب تضارب Foreign Keys

## 🎯 المشكلة

عند تنصيب كل Schema لوحده مع Foreign Keys، يحدث تضارب لأن:
- `EMPLOYEES` يحتاج `COUNTRIES` (من schema آخر)
- `PAYSLIPS` يحتاج `EMPLOYEES` (من schema آخر)

---

## ✅ الحل: الترتيب الصحيح

### **المرحلة 1: Schemas**
```sql
@01_Schemas/00_CREATE_ALL_SCHEMAS.sql
```
إنشاء جميع الـ Schemas أولاً (7 schemas)

---

### **المرحلة 2: الجداول (بدون Foreign Keys)**
```sql
-- إنشاء جميع الجداول بدون FK
@02_Core_Data/HR_CORE/Tables/01_HR_CORE_TABLES.sql
@03_Personnel/HR_PERSONNEL/Tables/01_HR_PERSONNEL_TABLES.sql
@04_Attendance/HR_ATTENDANCE/Tables/01_HR_ATTENDANCE_TABLES.sql
@05_Leaves/HR_LEAVES/Tables/01_HR_LEAVES_TABLES.sql
@06_Payroll/HR_PAYROLL/Tables/01_HR_PAYROLL_TABLES.sql
@07_Recruitment/HR_RECRUITMENT/Tables/01_HR_RECRUITMENT_TABLES.sql
@08_Performance/HR_PERFORMANCE/Tables/01_HR_PERFORMANCE_TABLES.sql
```

**✅ الآن جميع الجداول موجودة**

---

### **المرحلة 3: Foreign Keys (بعد إنشاء جميع الجداول)**
```sql
@09_Permissions/02_ADD_FOREIGN_KEYS.sql
```

**✅ الآن يمكن إضافة FK لأن جميع الجداول موجودة**

---

### **المرحلة 4: الصلاحيات**
```sql
@09_Permissions/01_GRANT_CORE_PERMISSIONS.sql
```

---

### **المرحلة 5: Packages**
```sql
-- بالترتيب
@HR_CORE/Packages/...
@HR_PERSONNEL/Packages/...
@HR_ATTENDANCE/Packages/...
@HR_LEAVES/Packages/...
@HR_PAYROLL/Packages/...
```

---

### **المرحلة 6: Views**
```sql
@HR_PERSONNEL/Views/01_EMPLOYEE_VIEWS.sql
```

---

### **المرحلة 7: Triggers**
```sql
@HR_PERSONNEL/Triggers/01_EMPLOYEE_TRIGGERS.sql
```

---

## 🚀 كيفية التنصيب

### الطريقة السهلة (موصى بها):

```bash
sqlplus sys/password@FREEPDB1 as sysdba
@G:/HRMS_Hospital/DB/Database_Organized/99_Installation/00_MASTER_INSTALL.sql
```

**هذا السكربت يعمل كل شيء بالترتيب الصحيح!**

---

## 🔄 إعادة التنصيب (بعد الحذف)

### 1. حذف قاعدة البيانات

```sql
-- حذف جميع Schemas
DROP USER HR_CORE CASCADE;
DROP USER HR_PERSONNEL CASCADE;
DROP USER HR_ATTENDANCE CASCADE;
DROP USER HR_LEAVES CASCADE;
DROP USER HR_PAYROLL CASCADE;
DROP USER HR_RECRUITMENT CASCADE;
DROP USER HR_PERFORMANCE CASCADE;
```

### 2. تنصيب من جديد

```sql
@00_MASTER_INSTALL.sql
```

**✅ يعمل 100% بدون أخطاء!**

---

## 📊 الترتيب الصحيح (ملخص)

```
1. Schemas          ← إنشاء المستخدمين
2. Tables (No FK)   ← إنشاء الجداول فقط
3. Foreign Keys     ← إضافة العلاقات (بعد وجود جميع الجداول)
4. Permissions      ← منح الصلاحيات
5. Packages         ← إنشاء الحزم
6. Views            ← إنشاء الاستعلامات
7. Triggers         ← إنشاء المحفزات
```

---

## ⚠️ الأخطاء الشائعة

### ❌ خطأ 1: إضافة FK مع الجداول
```sql
-- خطأ: إضافة FK في نفس سكربت الجداول
CREATE TABLE EMPLOYEES (...);
ALTER TABLE EMPLOYEES ADD CONSTRAINT FK_EMP_NATIONALITY 
  FOREIGN KEY (NATIONALITY_ID) REFERENCES HR_CORE.COUNTRIES(COUNTRY_ID);
-- ❌ سيفشل إذا COUNTRIES غير موجود
```

### ✅ الحل: فصل FK في سكربت منفصل
```sql
-- صحيح: إنشاء الجداول أولاً
CREATE TABLE EMPLOYEES (...);

-- ثم في سكربت منفصل (بعد إنشاء جميع الجداول)
ALTER TABLE EMPLOYEES ADD CONSTRAINT FK_EMP_NATIONALITY 
  FOREIGN KEY (NATIONALITY_ID) REFERENCES HR_CORE.COUNTRIES(COUNTRY_ID);
```

---

### ❌ خطأ 2: تنصيب Schema واحد كامل
```sql
-- خطأ: تنصيب HR_PERSONNEL كامل (Tables + FK + Packages)
-- ثم تنصيب HR_PAYROLL كامل
-- ❌ سيفشل لأن HR_PAYROLL يحتاج HR_PERSONNEL
```

### ✅ الحل: تنصيب بالمراحل
```sql
-- صحيح: تنصيب جميع Tables أولاً
-- ثم جميع FK
-- ثم جميع Packages
```

---

## 🎯 الخلاصة

**القاعدة الذهبية:**
> أنشئ جميع الجداول أولاً، ثم أضف Foreign Keys

**الترتيب:**
```
Schemas → Tables → FK → Permissions → Packages → Views → Triggers
```

**النتيجة:**
✅ تنصيب سلس بدون أخطاء  
✅ إمكانية حذف وإعادة تنصيب بسهولة  
✅ لا تضارب في Foreign Keys  

---

**النظام جاهز 100% للاستخدام!** ✅
