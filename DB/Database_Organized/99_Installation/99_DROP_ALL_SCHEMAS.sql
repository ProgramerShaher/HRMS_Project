PROMPT ========================================
PROMPT Dropping all HRMS Schemas
PROMPT WARNING: This will delete all data!
PROMPT ========================================

SET SERVEROUTPUT ON
ALTER SESSION SET CONTAINER = FREEPDB1;

PROMPT
PROMPT Dropping schemas in reverse order...
PROMPT

DROP USER HR_PERFORMANCE CASCADE;
PROMPT Dropped HR_PERFORMANCE

DROP USER HR_RECRUITMENT CASCADE;
PROMPT Dropped HR_RECRUITMENT

DROP USER HR_PAYROLL CASCADE;
PROMPT Dropped HR_PAYROLL

DROP USER HR_LEAVES CASCADE;
PROMPT Dropped HR_LEAVES

DROP USER HR_ATTENDANCE CASCADE;
PROMPT Dropped HR_ATTENDANCE

DROP USER HR_PERSONNEL CASCADE;
PROMPT Dropped HR_PERSONNEL

DROP USER HR_CORE CASCADE;
PROMPT Dropped HR_CORE

PROMPT
PROMPT ========================================
PROMPT All schemas dropped successfully!
PROMPT ========================================
PROMPT
PROMPT You can now reinstall:
PROMPT @00_MASTER_INSTALL.sql
PROMPT ========================================
