-- =================================================================================
-- سكربت تنصيب HR_PERSONNEL
-- الوصف: تنصيب كامل لوحدة شؤون الموظفين
-- =================================================================================

PROMPT ========================================
PROMPT تنصيب HR_PERSONNEL - شؤون الموظفين
PROMPT ========================================

-- 1. إنشاء الجداول
PROMPT
PROMPT [1/4] إنشاء الجداول...
@@../03_Personnel/HR_PERSONNEL/Tables/01_EMPLOYEES.sql
@@../03_Personnel/HR_PERSONNEL/Tables/02_EMPLOYEE_DOCUMENTS.sql
@@../03_Personnel/HR_PERSONNEL/Tables/03_EMPLOYEE_QUALIFICATIONS.sql
@@../03_Personnel/HR_PERSONNEL/Tables/04_EMPLOYEE_EXPERIENCES.sql
@@../03_Personnel/HR_PERSONNEL/Tables/05_EMPLOYEE_CERTIFICATIONS.sql
@@../03_Personnel/HR_PERSONNEL/Tables/06_EMPLOYEE_ADDRESSES.sql
@@../03_Personnel/HR_PERSONNEL/Tables/07_EMERGENCY_CONTACTS.sql
@@../03_Personnel/HR_PERSONNEL/Tables/08_EMPLOYEE_BANK_ACCOUNTS.sql
@@../03_Personnel/HR_PERSONNEL/Tables/09_CONTRACTS.sql
@@../03_Personnel/HR_PERSONNEL/Tables/10_CONTRACT_RENEWALS.sql
@@../03_Personnel/HR_PERSONNEL/Tables/11_DEPENDENTS.sql
@@../03_Personnel/HR_PERSONNEL/Tables/12_EMPLOYEE_TRANSFERS.sql
@@../03_Personnel/HR_PERSONNEL/Tables/13_TRAINING_COURSES.sql
@@../03_Personnel/HR_PERSONNEL/Tables/14_EMPLOYEE_TRAINING.sql
@@../03_Personnel/HR_PERSONNEL/Tables/15_EMPLOYEE_ASSETS.sql

-- 2. إنشاء Packages
PROMPT
PROMPT [2/4] إنشاء Packages...
@@../03_Personnel/HR_PERSONNEL/Packages/01_PKG_EMP_MANAGER_SPEC.sql
@@../03_Personnel/HR_PERSONNEL/Packages/02_PKG_EMP_MANAGER_BODY.sql

-- 3. إنشاء Views
PROMPT
PROMPT [3/4] إنشاء Views...
@@../03_Personnel/HR_PERSONNEL/Views/01_V_EMPLOYEES_FULL.sql
@@../03_Personnel/HR_PERSONNEL/Views/02_V_ACTIVE_EMPLOYEES.sql
@@../03_Personnel/HR_PERSONNEL/Views/03_V_CONTRACTS_FULL.sql
@@../03_Personnel/HR_PERSONNEL/Views/04_V_EXPIRING_CONTRACTS.sql
@@../03_Personnel/HR_PERSONNEL/Views/05_V_EXPIRING_DOCUMENTS.sql
@@../03_Personnel/HR_PERSONNEL/Views/06_V_EMPLOYEE_QUALIFICATIONS.sql
@@../03_Personnel/HR_PERSONNEL/Views/07_V_EMPLOYEE_CERTIFICATIONS.sql
@@../03_Personnel/HR_PERSONNEL/Views/08_V_EMPLOYEE_BANK_ACCOUNTS.sql
@@../03_Personnel/HR_PERSONNEL/Views/09_V_EMERGENCY_CONTACTS.sql
@@../03_Personnel/HR_PERSONNEL/Views/10_V_EMPLOYEE_ADDRESSES.sql
@@../03_Personnel/HR_PERSONNEL/Views/11_V_EMPLOYEE_DEPENDENTS.sql
@@../03_Personnel/HR_PERSONNEL/Views/12_V_EMPLOYEE_TRANSFERS.sql
@@../03_Personnel/HR_PERSONNEL/Views/13_V_DEPT_STATISTICS.sql
@@../03_Personnel/HR_PERSONNEL/Views/14_V_NATIONALITY_STATISTICS.sql
@@../03_Personnel/HR_PERSONNEL/Views/15_V_EMPLOYEE_DASHBOARD.sql

-- 4. إنشاء Triggers
PROMPT
PROMPT [4/4] إنشاء Triggers...
@@../03_Personnel/HR_PERSONNEL/Triggers/01_TRG_EMPLOYEE_INSERT_AUDIT.sql
@@../03_Personnel/HR_PERSONNEL/Triggers/02_TRG_EMPLOYEE_UPDATE_AUDIT.sql
@@../03_Personnel/HR_PERSONNEL/Triggers/03_TRG_EMPLOYEE_PREVENT_DELETE.sql
@@../03_Personnel/HR_PERSONNEL/Triggers/04_TRG_CONTRACT_INSERT_AUDIT.sql
@@../03_Personnel/HR_PERSONNEL/Triggers/05_TRG_CONTRACT_UPDATE_AUDIT.sql
@@../03_Personnel/HR_PERSONNEL/Triggers/06_TRG_CREATE_LEAVE_BALANCES.sql
@@../03_Personnel/HR_PERSONNEL/Triggers/07_TRG_CREATE_SALARY_STRUCTURE.sql
@@../03_Personnel/HR_PERSONNEL/Triggers/08_TRG_VALIDATE_CONTRACT_DATES.sql
@@../03_Personnel/HR_PERSONNEL/Triggers/09_TRG_DOCUMENT_EXPIRY_ALERT.sql
@@../03_Personnel/HR_PERSONNEL/Triggers/10_TRG_PREVENT_EMPNO_CHANGE.sql

PROMPT
PROMPT ========================================
PROMPT ✅ اكتمل تنصيب HR_PERSONNEL
PROMPT ========================================

-- التحقق
SELECT 'Tables' AS TYPE, COUNT(*) AS COUNT
FROM user_tables
UNION ALL
SELECT 'Packages', COUNT(*)
FROM user_objects
WHERE object_type = 'PACKAGE'
UNION ALL
SELECT 'Views', COUNT(*)
FROM user_views
UNION ALL
SELECT 'Triggers', COUNT(*)
FROM user_triggers;
