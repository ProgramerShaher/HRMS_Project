-- =================================================================================
-- إنشاء Views لعرض جميع جداول HR في مكان واحد
-- يتم تنفيذها بمستخدم SYSTEM
-- =================================================================================

ALTER SESSION SET CONTAINER = XEPDB1;

-- View لعرض جميع الموظفين
CREATE OR REPLACE VIEW ALL_HR_EMPLOYEES AS
SELECT * FROM HR_PERSONNEL.EMPLOYEES;

-- View لعرض جميع الأقسام
CREATE OR REPLACE VIEW ALL_HR_DEPARTMENTS AS
SELECT * FROM HR_CORE.DEPARTMENTS;

-- View لعرض جميع الوظائف
CREATE OR REPLACE VIEW ALL_HR_JOBS AS
SELECT * FROM HR_CORE.JOBS;

-- View لعرض الحضور اليومي
CREATE OR REPLACE VIEW ALL_HR_ATTENDANCE AS
SELECT * FROM HR_ATTENDANCE.DAILY_ATTENDANCE;

-- View لعرض طلبات الإجازات
CREATE OR REPLACE VIEW ALL_HR_LEAVE_REQUESTS AS
SELECT * FROM HR_LEAVES.LEAVE_REQUESTS;

-- View لعرض قسائم الرواتب
CREATE OR REPLACE VIEW ALL_HR_PAYSLIPS AS
SELECT * FROM HR_PAYROLL.PAYSLIPS;

SELECT 'تم إنشاء Views بنجاح! ستجدها في قائمة Views' AS STATUS FROM DUAL;
