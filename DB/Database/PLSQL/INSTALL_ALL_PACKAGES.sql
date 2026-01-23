-- =================================================================================
-- تثبيت جميع الـ PL/SQL Packages على Oracle 23ai
-- يجب التنفيذ بحساب SYS as SYSDBA
-- =================================================================================

ALTER SESSION SET CONTAINER = FREEPDB1;

SET SERVEROUTPUT ON;
SET DEFINE OFF;

PROMPT ========================================
PROMPT تثبيت PL/SQL Packages (6 packages)
PROMPT ========================================

-- =================================================================================
-- 1. PKG_EMP_MANAGER - إدارة الموظفين
-- =================================================================================

PROMPT 
PROMPT [1/6] تثبيت PKG_EMP_MANAGER...

@@01_PKG_EMP_MANAGER_SPEC.sql
@@02_PKG_EMP_MANAGER_BODY.sql

PROMPT ✅ تم تثبيت PKG_EMP_MANAGER

-- =================================================================================
-- 2. PKG_LEAVE_MANAGER - إدارة الإجازات
-- =================================================================================

PROMPT 
PROMPT [2/6] تثبيت PKG_LEAVE_MANAGER...

@@03_PKG_LEAVE_MANAGER_SPEC.sql
@@04_PKG_LEAVE_MANAGER_BODY.sql

PROMPT ✅ تم تثبيت PKG_LEAVE_MANAGER

-- =================================================================================
-- 3. PKG_PAYROLL_MANAGER - إدارة الرواتب
-- =================================================================================

PROMPT 
PROMPT [3/6] تثبيت PKG_PAYROLL_MANAGER...

@@05_PKG_PAYROLL_MANAGER_SPEC.sql
@@06_PKG_PAYROLL_MANAGER_BODY.sql

PROMPT ✅ تم تثبيت PKG_PAYROLL_MANAGER

-- =================================================================================
-- 4. PKG_ATTENDANCE_MANAGER - إدارة الحضور
-- =================================================================================

PROMPT 
PROMPT [4/6] تثبيت PKG_ATTENDANCE_MANAGER...

@@07_PKG_ATTENDANCE_MANAGER_SPEC.sql
@@08_PKG_ATTENDANCE_MANAGER_BODY.sql

PROMPT ✅ تم تثبيت PKG_ATTENDANCE_MANAGER

-- =================================================================================
-- 5. PKG_PERFORMANCE_MANAGER - إدارة الأداء
-- =================================================================================

PROMPT 
PROMPT [5/6] تثبيت PKG_PERFORMANCE_MANAGER...

@@09_PKG_PERFORMANCE_MANAGER_SPEC.sql
@@10_PKG_PERFORMANCE_MANAGER_BODY.sql

PROMPT ✅ تم تثبيت PKG_PERFORMANCE_MANAGER

-- =================================================================================
-- 6. PKG_SECURITY_MANAGER - إدارة الأمان
-- =================================================================================

PROMPT 
PROMPT [6/6] تثبيت PKG_SECURITY_MANAGER...

@@11_PKG_SECURITY_MANAGER_SPEC.sql
@@12_PKG_SECURITY_MANAGER_BODY.sql

PROMPT ✅ تم تثبيت PKG_SECURITY_MANAGER

-- =================================================================================
-- التحقق من التثبيت
-- =================================================================================

PROMPT 
PROMPT ========================================
PROMPT التحقق من الـ Packages المثبتة
PROMPT ========================================

SELECT 
    object_name AS package_name,
    object_type,
    status,
    TO_CHAR(last_ddl_time, 'YYYY-MM-DD HH24:MI:SS') AS last_compiled
FROM all_objects
WHERE object_type IN ('PACKAGE', 'PACKAGE BODY')
  AND owner IN ('HR_PERSONNEL', 'HR_LEAVES', 'HR_PAYROLL', 'HR_ATTENDANCE', 'HR_PERFORMANCE', 'HR_CORE')
  AND object_name LIKE 'PKG_%'
ORDER BY owner, object_name, object_type;

PROMPT 
PROMPT ========================================
PROMPT ✅ اكتمل تثبيت جميع الـ Packages
PROMPT ========================================

-- عرض الأخطاء إن وجدت
PROMPT 
PROMPT التحقق من الأخطاء:
PROMPT ========================================

SELECT 
    owner || '.' || name AS package_name,
    type,
    line,
    position,
    text AS error_message
FROM all_errors
WHERE owner IN ('HR_PERSONNEL', 'HR_LEAVES', 'HR_PAYROLL', 'HR_ATTENDANCE', 'HR_PERFORMANCE', 'HR_CORE')
  AND name LIKE 'PKG_%'
ORDER BY owner, name, sequence;

PROMPT 
PROMPT إذا لم تظهر أخطاء، فالتثبيت ناجح!
PROMPT ========================================
