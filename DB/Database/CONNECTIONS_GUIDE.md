# دليل الاتصالات - HR Management System

## معلومات الاتصال الأساسية
- **Hostname:** localhost
- **Port:** 1521
- **Service Name:** xepdb1

---

## الاتصالات المطلوبة (7 اتصالات)

### 1. HR_CORE (الأساسيات والإعدادات)
```
Connection Name: HR_CORE
Username: HR_CORE
Password: Pwd_Core_123
```
**الجداول:** 13 جدول (COUNTRIES, CITIES, DEPARTMENTS, JOBS, BANKS, إلخ)

---

### 2. HR_PERSONNEL (شؤون الموظفين)
```
Connection Name: HR_PERSONNEL
Username: HR_PERSONNEL
Password: Pwd_Personnel_123
```
**الجداول:** 11 جدول (EMPLOYEES, CONTRACTS, QUALIFICATIONS, إلخ)

---

### 3. HR_ATTENDANCE (الحضور والانصراف)
```
Connection Name: HR_ATTENDANCE
Username: HR_ATTENDANCE
Password: Pwd_Attend_123
```
**الجداول:** 8 جداول (DAILY_ATTENDANCE, SHIFT_TYPES, OVERTIME_REQUESTS, إلخ)

---

### 4. HR_LEAVES (الإجازات)
```
Connection Name: HR_LEAVES
Username: HR_LEAVES
Password: Pwd_Leaves_123
```
**الجداول:** 7 جداول (LEAVE_REQUESTS, LEAVE_TYPES, LEAVE_BALANCES, إلخ)

---

### 5. HR_PAYROLL (الرواتب)
```
Connection Name: HR_PAYROLL
Username: HR_PAYROLL
Password: Pwd_Payroll_123
```
**الجداول:** 10 جداول (PAYSLIPS, PAYROLL_RUNS, LOANS, إلخ)

---

### 6. HR_RECRUITMENT (التوظيف)
```
Connection Name: HR_RECRUITMENT
Username: HR_RECRUITMENT
Password: Pwd_Recruit_123
```
**الجداول:** 5 جداول (JOB_VACANCIES, CANDIDATES, APPLICATIONS, إلخ)

---

### 7. HR_PERFORMANCE (الأداء والجزاءات)
```
Connection Name: HR_PERFORMANCE
Username: HR_PERFORMANCE
Password: Pwd_Perform_123
```
**الجداول:** 7 جداول (EMPLOYEE_APPRAISALS, KPI_LIBRARIES, VIOLATIONS, إلخ)

---

## خطوات إنشاء الاتصال في VS Code

1. افتح لوحة **Oracle SQL Developer** من الشريط الجانبي
2. اضغط على أيقونة **"+"** (Create Connection)
3. املأ البيانات من القائمة أعلاه
4. ✅ فعّل **"Save Password"**
5. اضغط **"Test"** للتأكد من الاتصال
6. اضغط **"Connect"**

---

## الاتصال الإداري (SYSTEM)
```
Connection Name: HR_System_Admin
Username: SYSTEM
Password: [كلمة مرور SYSTEM الخاصة بك]
Service Name: xepdb1
```
**الاستخدام:** تنفيذ السكربتات الإدارية (CREATE USER, GRANT, إلخ)

---

## ملاحظات مهمة:
- 🔒 جميع كلمات المرور يجب تغييرها في بيئة الإنتاج
- 📊 كل Schema يحتوي على جداوله الخاصة فقط
- 🔗 العلاقات (Foreign Keys) تربط الجداول عبر الـ Schemas
- ⚡ استخدم الاتصال المناسب حسب الوحدة التي تعمل عليها

---

## الاستعلامات السريعة

### عرض جميع الجداول في Schema معين:
```sql
SELECT table_name FROM user_tables ORDER BY table_name;
```

### عرض أعمدة جدول:
```sql
DESC EMPLOYEES;
```

### عرض Foreign Keys:
```sql
SELECT constraint_name, r_constraint_name 
FROM user_constraints 
WHERE constraint_type = 'R';
```
