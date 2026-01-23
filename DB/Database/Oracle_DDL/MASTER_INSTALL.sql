-- =================================================================================
-- سكربت التنفيذ الشامل (Master Execution Script)
-- يقوم بتنفيذ جميع السكربتات بالترتيب الصحيح
-- =================================================================================

-- التأكد من الاتصال بـ PDB
ALTER SESSION SET CONTAINER = XEPDB1;

PROMPT ========================================
PROMPT تنفيذ 01_HR_CORE.sql
PROMPT ========================================
@@01_HR_CORE.sql

PROMPT ========================================
PROMPT تنفيذ 02_HR_PERSONNEL.sql
PROMPT ========================================
@@02_HR_PERSONNEL.sql

PROMPT ========================================
PROMPT تنفيذ 03_HR_ATTENDANCE.sql
PROMPT ========================================
@@03_HR_ATTENDANCE.sql

PROMPT ========================================
PROMPT تنفيذ 04_HR_LEAVES.sql
PROMPT ========================================
@@04_HR_LEAVES.sql

PROMPT ========================================
PROMPT تنفيذ 05_HR_PAYROLL.sql
PROMPT ========================================
@@05_HR_PAYROLL.sql

PROMPT ========================================
PROMPT تنفيذ 06_HR_RECRUITMENT.sql
PROMPT ========================================
@@06_HR_RECRUITMENT.sql

PROMPT ========================================
PROMPT تنفيذ 07_HR_PERFORMANCE.sql
PROMPT ========================================
@@07_HR_PERFORMANCE.sql

PROMPT ========================================
PROMPT تنفيذ 08_SYSTEM_CORE.sql
PROMPT ========================================
@@08_SYSTEM_CORE.sql

PROMPT ========================================
PROMPT اكتمل التنفيذ بنجاح!
PROMPT ========================================

-- التحقق من عدد الجداول المنشأة
SELECT owner, COUNT(*) as table_count 
FROM all_tables 
WHERE owner LIKE 'HR_%' 
GROUP BY owner
ORDER BY owner;
