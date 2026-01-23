PROMPT ========================================
PROMPT HRMS Database - Complete Installation
PROMPT ========================================
PROMPT
PROMPT This script will install:
PROMPT - 7 Schemas
PROMPT - 75 Tables
PROMPT - 92 Foreign Keys
PROMPT - 7 Packages
PROMPT - 15+ Views
PROMPT - 10+ Triggers
PROMPT
PROMPT Estimated time: 5-10 minutes
PROMPT ========================================

ALTER SESSION SET CONTAINER = FREEPDB1;

PROMPT
PROMPT ========================================
PROMPT Phase 1/7: Creating Schemas
PROMPT ========================================
@@../01_Schemas/00_CREATE_ALL_SCHEMAS.sql

PROMPT
PROMPT ========================================
PROMPT Phase 2/7: Creating Tables (No FK)
PROMPT ========================================

PROMPT [2.1] Creating HR_CORE + HR_PERSONNEL Tables (Part 1)...
@@../02_Core_Data/HR_CORE/Tables/00_ALL_CORE_TABLES.sql

PROMPT [2.2] Creating Other Schema Tables (Part 2)...
@@../03_Personnel/HR_PERSONNEL/Tables/00_ALL_PERSONNEL_TABLES.sql

PROMPT
PROMPT ========================================
PROMPT Phase 3/7: Adding Foreign Keys
PROMPT ========================================
@@../09_Permissions/00_ADD_ALL_FOREIGN_KEYS.sql

PROMPT
PROMPT ========================================
PROMPT Phase 4/7: Granting Permissions
PROMPT ========================================
@@../09_Permissions/01_GRANT_CORE_PERMISSIONS.sql

PROMPT
PROMPT ========================================
PROMPT Phase 5/7: Creating Packages
PROMPT ========================================

PROMPT [5.1] PKG_SECURITY_MANAGER (HR_CORE)...
CONNECT HR_CORE/Pwd_Core_123@FREEPDB1
@@../02_Core_Data/HR_CORE/Packages/01_PKG_SECURITY_MANAGER_SPEC.sql
@@../02_Core_Data/HR_CORE/Packages/02_PKG_SECURITY_MANAGER_BODY.sql

PROMPT [5.2] PKG_EMP_MANAGER (HR_PERSONNEL)...
CONNECT HR_PERSONNEL/Pwd_Personnel_123@FREEPDB1
@@../03_Personnel/HR_PERSONNEL/Packages/01_PKG_EMP_MANAGER_SPEC.sql
@@../03_Personnel/HR_PERSONNEL/Packages/02_PKG_EMP_MANAGER_BODY.sql

PROMPT [5.3] PKG_ATTENDANCE_MANAGER (HR_ATTENDANCE)...
CONNECT HR_ATTENDANCE/Pwd_Attend_123@FREEPDB1
@@../04_Attendance/HR_ATTENDANCE/Packages/01_PKG_ATTENDANCE_MANAGER_SPEC.sql
@@../04_Attendance/HR_ATTENDANCE/Packages/02_PKG_ATTENDANCE_MANAGER_BODY.sql

PROMPT [5.4] PKG_LEAVE_MANAGER (HR_LEAVES)...
CONNECT HR_LEAVES/Pwd_Leaves_123@FREEPDB1
@@../05_Leaves/HR_LEAVES/Packages/01_PKG_LEAVE_MANAGER_SPEC.sql
@@../05_Leaves/HR_LEAVES/Packages/02_PKG_LEAVE_MANAGER_BODY.sql

PROMPT [5.5] PKG_PAYROLL_MANAGER (HR_PAYROLL)...
CONNECT HR_PAYROLL/Pwd_Payroll_123@FREEPDB1
@@../06_Payroll/HR_PAYROLL/Packages/01_PKG_PAYROLL_MANAGER_SPEC.sql
@@../06_Payroll/HR_PAYROLL/Packages/02_PKG_PAYROLL_MANAGER_BODY.sql

PROMPT [5.6] PKG_PERFORMANCE_MANAGER (HR_PERFORMANCE)...
CONNECT HR_PERFORMANCE/Pwd_Perform_123@FREEPDB1
@@../08_Performance/HR_PERFORMANCE/Packages/01_PKG_PERFORMANCE_MANAGER_SPEC.sql
@@../08_Performance/HR_PERFORMANCE/Packages/02_PKG_PERFORMANCE_MANAGER_BODY.sql

PROMPT
PROMPT ========================================
PROMPT Phase 6/7: Creating Views
PROMPT ========================================
CONNECT HR_PERSONNEL/Pwd_Personnel_123@FREEPDB1
@@../03_Personnel/HR_PERSONNEL/Views/01_EMPLOYEE_VIEWS.sql

PROMPT
PROMPT ========================================
PROMPT Phase 7/7: Creating Triggers
PROMPT ========================================
CONNECT HR_PERSONNEL/Pwd_Personnel_123@FREEPDB1
@@../03_Personnel/HR_PERSONNEL/Triggers/01_EMPLOYEE_TRIGGERS.sql

PROMPT
PROMPT ========================================
PROMPT Final Verification
PROMPT ========================================
CONNECT sys/password@FREEPDB1 as sysdba

SELECT owner, COUNT(*) as table_count
FROM all_tables
WHERE owner LIKE 'HR_%'
GROUP BY owner
ORDER BY owner;

SELECT owner, COUNT(*) as fk_count
FROM all_constraints
WHERE owner LIKE 'HR_%'
  AND constraint_type = 'R'
GROUP BY owner
ORDER BY owner;

SELECT owner, object_name, object_type, status
FROM all_objects
WHERE owner LIKE 'HR_%'
  AND object_type IN ('PACKAGE', 'VIEW', 'TRIGGER')
ORDER BY owner, object_type, object_name;

PROMPT
PROMPT ========================================
PROMPT Installation Complete!
PROMPT ========================================
PROMPT
PROMPT Expected Results:
PROMPT - 7 Schemas
PROMPT - 75 Tables
PROMPT - 92 Foreign Keys
PROMPT - 7 Packages (VALID)
PROMPT - 15+ Views
PROMPT - 10+ Triggers (ENABLED)
PROMPT
PROMPT System is ready to use!
PROMPT ========================================
