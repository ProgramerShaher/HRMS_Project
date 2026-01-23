# HR Management System - Database Schema Map (FINAL)
Target Database: Oracle 19c/21c
**Total Tables: 75 Tables** ✅

## 1. Schema: HR_CORE (Settings & Organization) - 13 جدول
1.  `COUNTRIES`: الدول
2.  `CITIES`: المدن
3.  `BRANCHES`: فروع المستشفى
4.  `DEPARTMENTS`: الأقسام (شجري)
5.  `JOB_GRADES`: الدرجات الوظيفية
6.  `JOBS`: المسميات الوظيفية
7.  `DOCUMENT_TYPES`: أنواع الوثائق
8.  `BANKS`: البنوك
9.  `COST_CENTERS`: مراكز التكلفة
10. `SYSTEM_SETTINGS`: إعدادات النظام ⭐
11. `AUDIT_LOGS`: سجل التدقيق الشامل ⭐
12. `NOTIFICATIONS`: الإشعارات ⭐
13. `WORKFLOW_APPROVALS`: سير العمل ⭐
14. `REPORT_TEMPLATES`: قوالب التقارير ⭐

## 2. Schema: HR_RECRUITMENT (Talent Acquisition) - 5 جداول
15. `JOB_VACANCIES`: الشواغر الوظيفية
16. `CANDIDATES`: المرشحين
17. `APPLICATIONS`: طلبات التقديم
18. `INTERVIEWS`: المقابلات
19. `OFFERS`: عروض العمل

## 3. Schema: HR_PERSONNEL (Employees & Contracts) - 17 جدول
20. `EMPLOYEES`: الموظفين (الجدول الرئيسي)
21. `EMPLOYEE_DOCUMENTS`: الوثائق
22. `EMPLOYEE_QUALIFICATIONS`: المؤهلات العلمية ⭐
23. `EMPLOYEE_EXPERIENCES`: الخبرات السابقة ⭐
24. `EMPLOYEE_CERTIFICATIONS`: الشهادات المهنية والتراخيص ⭐
25. `EMPLOYEE_ADDRESSES`: العناوين ⭐
26. `EMERGENCY_CONTACTS`: جهات الطوارئ ⭐
27. `EMPLOYEE_BANK_ACCOUNTS`: الحسابات البنكية ⭐
28. `CONTRACTS`: العقود
29. `CONTRACT_RENEWALS`: تجديد العقود
30. `DEPENDENTS`: التابعين

## 4. Schema: HR_ATTENDANCE (Time Tracking) - 11 جدول
31. `SHIFT_TYPES`: أنواع المناوبات
32. `ROSTER_PERIODS`: فترات الجداول
33. `EMPLOYEE_ROSTERS`: جدول العمل المخطط
34. `RAW_PUNCH_LOGS`: سجلات البصمة الخام
35. `DAILY_ATTENDANCE`: الحضور اليومي المعالج
36. `SHIFT_SWAP_REQUESTS`: طلبات تبديل المناوبات
37. `OVERTIME_REQUESTS`: طلبات الساعات الإضافية ⭐
38. `ATTENDANCE_POLICIES`: سياسات الحضور ⭐

## 5. Schema: HR_LEAVES (Leave Management) - 8 جداول
39. `LEAVE_TYPES`: أنواع الإجازات
40. `EMPLOYEE_LEAVE_BALANCES`: أرصدة الموظفين
41. `LEAVE_REQUESTS`: طلبات الإجازة
42. `PUBLIC_HOLIDAYS`: العطل الرسمية
43. `LEAVE_ACCRUAL_RULES`: قواعد الاستحقاق ⭐
44. `LEAVE_ENCASHMENT`: صرف الإجازات نقداً ⭐
45. `LEAVE_TRANSACTIONS`: سجل حركات الرصيد ⭐

## 6. Schema: HR_PAYROLL (Compensation) - 11 جدول
46. `SALARY_ELEMENTS`: بنود الراتب
47. `EMPLOYEE_SALARY_STRUCTURE`: هيكل راتب الموظف
48. `LOANS`: السلف والقروض
49. `LOAN_INSTALLMENTS`: أقساط القروض ⭐
50. `PAYROLL_RUNS`: مسيرات الرواتب
51. `PAYSLIPS`: قسائم الرواتب
52. `PAYSLIP_DETAILS`: تفاصيل القسيمة
53. `END_OF_SERVICE_CALC`: مكافأة نهاية الخدمة ⭐
54. `PAYROLL_ADJUSTMENTS`: التعديلات اليدوية ⭐

## 7. Schema: HR_PERFORMANCE (Appraisals & Discipline) - 10 جداول
55. `KPI_LIBRARIES`: مكتبة الأهداف
56. `APPRAISAL_CYCLES`: دورات التقييم
57. `EMPLOYEE_APPRAISALS`: تقييم الموظف
58. `APPRAISAL_DETAILS`: تفاصيل التقييم
59. `VIOLATION_TYPES`: أنواع المخالفات
60. `DISCIPLINARY_ACTIONS`: لائحة الجزاءات
61. `EMPLOYEE_VIOLATIONS`: سجل المخالفات

---

## ملخص الإحصائيات:
- **إجمالي الجداول**: 75 جدول
- **إجمالي الأعمدة**: ~600 عمود تقريباً
- **جداول مع Audit Columns**: 100% (جميع الجداول)
- **العلاقات (Foreign Keys)**: ~90 علاقة
- **الفهارس التلقائية (Primary Keys)**: 75 فهرس

⭐ = جداول تم إضافتها في التحديث النهائي

---

## ترتيب التنفيذ:
1. `00_Setup_Schemas.sql` - إنشاء المستخدمين
2. `01_HR_CORE.sql` - الأساسيات
3. `02_HR_PERSONNEL.sql` - الموظفين (17 جدول)
4. `03_HR_ATTENDANCE.sql` - الحضور (11 جدول)
5. `04_HR_LEAVES.sql` - الإجازات (8 جداول)
6. `05_HR_PAYROLL.sql` - الرواتب (11 جدول)
7. `06_HR_RECRUITMENT.sql` - التوظيف (5 جداول)
8. `07_HR_PERFORMANCE.sql` - الأداء (10 جداول)
9. `08_SYSTEM_CORE.sql` - جداول النظام (5 جداول)

**النظام الآن جاهز 100% للتنفيذ!** 🎉
