-- =================================================================================
-- إنشاء جميع الـ Schemas لنظام HRMS على Oracle 23ai Free (9 Schemas)
-- متوافق مع APEX Workspaces
-- المنفذ: 1522 | PDB: FREEPDB1
-- =================================================================================

-- الاتصال بـ FREEPDB1
ALTER SESSION SET CONTAINER = FREEPDB1;

PROMPT ========================================
PROMPT إنشاء جميع الـ Schemas (9 schemas)
PROMPT ========================================

-- =================================================================================
-- 1. HR_CORE - الأساس والإعدادات العامة
-- =================================================================================

CREATE USER HR_CORE IDENTIFIED BY "Core@2026"
    DEFAULT TABLESPACE USERS
    TEMPORARY TABLESPACE TEMP
    QUOTA UNLIMITED ON USERS;

GRANT CONNECT, RESOURCE, CREATE VIEW TO HR_CORE;
GRANT CREATE SESSION, CREATE TABLE, CREATE SEQUENCE TO HR_CORE;
GRANT CREATE TRIGGER, CREATE PROCEDURE, CREATE SYNONYM TO HR_CORE;
GRANT APEX_ADMINISTRATOR_ROLE TO HR_CORE;

PROMPT ✅ 1/9 - تم إنشاء HR_CORE

-- =================================================================================
-- 2. HR_PERSONNEL - شؤون الموظفين
-- =================================================================================

CREATE USER HR_PERSONNEL IDENTIFIED BY "Personnel@2026"
    DEFAULT TABLESPACE USERS
    TEMPORARY TABLESPACE TEMP
    QUOTA UNLIMITED ON USERS;

GRANT CONNECT, RESOURCE, CREATE VIEW TO HR_PERSONNEL;
GRANT CREATE SESSION, CREATE TABLE, CREATE SEQUENCE TO HR_PERSONNEL;
GRANT CREATE TRIGGER, CREATE PROCEDURE, CREATE SYNONYM TO HR_PERSONNEL;

PROMPT ✅ 2/9 - تم إنشاء HR_PERSONNEL

-- =================================================================================
-- 3. HR_ATTENDANCE - الحضور والانصراف
-- =================================================================================

CREATE USER HR_ATTENDANCE IDENTIFIED BY "Attend@2026"
    DEFAULT TABLESPACE USERS
    TEMPORARY TABLESPACE TEMP
    QUOTA UNLIMITED ON USERS;

GRANT CONNECT, RESOURCE, CREATE VIEW TO HR_ATTENDANCE;
GRANT CREATE SESSION, CREATE TABLE, CREATE SEQUENCE TO HR_ATTENDANCE;
GRANT CREATE TRIGGER, CREATE PROCEDURE TO HR_ATTENDANCE;

PROMPT ✅ 3/9 - تم إنشاء HR_ATTENDANCE

-- =================================================================================
-- 4. HR_LEAVES - الإجازات
-- =================================================================================

CREATE USER HR_LEAVES IDENTIFIED BY "Leaves@2026"
    DEFAULT TABLESPACE USERS
    TEMPORARY TABLESPACE TEMP
    QUOTA UNLIMITED ON USERS;

GRANT CONNECT, RESOURCE, CREATE VIEW TO HR_LEAVES;
GRANT CREATE SESSION, CREATE TABLE, CREATE SEQUENCE TO HR_LEAVES;
GRANT CREATE TRIGGER, CREATE PROCEDURE TO HR_LEAVES;

PROMPT ✅ 4/9 - تم إنشاء HR_LEAVES

-- =================================================================================
-- 5. HR_PAYROLL - الرواتب والأجور
-- =================================================================================

CREATE USER HR_PAYROLL IDENTIFIED BY "Payroll@2026"
    DEFAULT TABLESPACE USERS
    TEMPORARY TABLESPACE TEMP
    QUOTA UNLIMITED ON USERS;

GRANT CONNECT, RESOURCE, CREATE VIEW TO HR_PAYROLL;
GRANT CREATE SESSION, CREATE TABLE, CREATE SEQUENCE TO HR_PAYROLL;
GRANT CREATE TRIGGER, CREATE PROCEDURE TO HR_PAYROLL;

PROMPT ✅ 5/9 - تم إنشاء HR_PAYROLL

-- =================================================================================
-- 6. HR_RECRUITMENT - التوظيف
-- =================================================================================

CREATE USER HR_RECRUITMENT IDENTIFIED BY "Recruit@2026"
    DEFAULT TABLESPACE USERS
    TEMPORARY TABLESPACE TEMP
    QUOTA UNLIMITED ON USERS;

GRANT CONNECT, RESOURCE, CREATE VIEW TO HR_RECRUITMENT;
GRANT CREATE SESSION, CREATE TABLE, CREATE SEQUENCE TO HR_RECRUITMENT;
GRANT CREATE TRIGGER, CREATE PROCEDURE TO HR_RECRUITMENT;

PROMPT ✅ 6/9 - تم إنشاء HR_RECRUITMENT

-- =================================================================================
-- 7. HR_PERFORMANCE - الأداء والجزاءات
-- =================================================================================

CREATE USER HR_PERFORMANCE IDENTIFIED BY "Perform@2026"
    DEFAULT TABLESPACE USERS
    TEMPORARY TABLESPACE TEMP
    QUOTA UNLIMITED ON USERS;

GRANT CONNECT, RESOURCE, CREATE VIEW TO HR_PERFORMANCE;
GRANT CREATE SESSION, CREATE TABLE, CREATE SEQUENCE TO HR_PERFORMANCE;
GRANT CREATE TRIGGER, CREATE PROCEDURE TO HR_PERFORMANCE;

PROMPT ✅ 7/9 - تم إنشاء HR_PERFORMANCE

-- =================================================================================
-- 8. HR_System_Admin - إدارة النظام
-- =================================================================================

CREATE USER HR_System_Admin IDENTIFIED BY "SysAdmin@2026"
    DEFAULT TABLESPACE USERS
    TEMPORARY TABLESPACE TEMP
    QUOTA UNLIMITED ON USERS;

GRANT CONNECT, RESOURCE, CREATE VIEW TO HR_System_Admin;
GRANT CREATE SESSION, CREATE TABLE, CREATE SEQUENCE TO HR_System_Admin;
GRANT CREATE TRIGGER, CREATE PROCEDURE, CREATE SYNONYM TO HR_System_Admin;
GRANT CREATE USER TO HR_System_Admin;  -- صلاحيات إدارية إضافية

-- صلاحيات القراءة من جميع الـ Schemas
GRANT SELECT ANY TABLE TO HR_System_Admin;
GRANT SELECT ANY DICTIONARY TO HR_System_Admin;

PROMPT ✅ 8/9 - تم إنشاء HR_System_Admin

-- =================================================================================
-- 9. HR_System_PDB - نظام قاعدة البيانات
-- =================================================================================

CREATE USER HR_System_PDB IDENTIFIED BY "SysPDB@2026"
    DEFAULT TABLESPACE USERS
    TEMPORARY TABLESPACE TEMP
    QUOTA UNLIMITED ON USERS;

GRANT CONNECT, RESOURCE TO HR_System_PDB;
GRANT CREATE SESSION, CREATE TABLE, CREATE SEQUENCE TO HR_System_PDB;
GRANT CREATE TRIGGER, CREATE PROCEDURE TO HR_System_PDB;

-- صلاحيات للمراقبة والتدقيق
GRANT SELECT_CATALOG_ROLE TO HR_System_PDB;
GRANT SELECT ANY DICTIONARY TO HR_System_PDB;

PROMPT ✅ 9/9 - تم إنشاء HR_System_PDB

-- =================================================================================
-- منح صلاحيات الوصول المتبادل (Cross-Schema Access)
-- =================================================================================

PROMPT ========================================
PROMPT منح صلاحيات الوصول المتبادل
PROMPT ========================================

-- صلاحيات CORE لجميع الـ Schemas
DECLARE
    v_schemas SYS.ODCIVARCHAR2LIST := SYS.ODCIVARCHAR2LIST(
        'HR_PERSONNEL', 'HR_ATTENDANCE', 'HR_LEAVES', 
        'HR_PAYROLL', 'HR_RECRUITMENT', 'HR_PERFORMANCE'
    );
    v_sql VARCHAR2(500);
BEGIN
    FOR i IN 1..v_schemas.COUNT LOOP
        -- SELECT privileges
        v_sql := 'GRANT SELECT ON HR_CORE.COUNTRIES TO ' || v_schemas(i);
        EXECUTE IMMEDIATE v_sql;
        v_sql := 'GRANT SELECT ON HR_CORE.CITIES TO ' || v_schemas(i);
        EXECUTE IMMEDIATE v_sql;
        v_sql := 'GRANT SELECT ON HR_CORE.DEPARTMENTS TO ' || v_schemas(i);
        EXECUTE IMMEDIATE v_sql;
        v_sql := 'GRANT SELECT ON HR_CORE.JOBS TO ' || v_schemas(i);
        EXECUTE IMMEDIATE v_sql;
        v_sql := 'GRANT SELECT ON HR_CORE.BANKS TO ' || v_schemas(i);
        EXECUTE IMMEDIATE v_sql;
        
        -- REFERENCES privileges (for Foreign Keys)
        v_sql := 'GRANT REFERENCES ON HR_CORE.COUNTRIES TO ' || v_schemas(i);
        EXECUTE IMMEDIATE v_sql;
        v_sql := 'GRANT REFERENCES ON HR_CORE.CITIES TO ' || v_schemas(i);
        EXECUTE IMMEDIATE v_sql;
        v_sql := 'GRANT REFERENCES ON HR_CORE.DEPARTMENTS TO ' || v_schemas(i);
        EXECUTE IMMEDIATE v_sql;
        v_sql := 'GRANT REFERENCES ON HR_CORE.JOBS TO ' || v_schemas(i);
        EXECUTE IMMEDIATE v_sql;
        
        DBMS_OUTPUT.PUT_LINE('✅ منح صلاحيات لـ ' || v_schemas(i));
    END LOOP;
END;
/

-- صلاحيات PERSONNEL للـ Schemas الأخرى
BEGIN
    EXECUTE IMMEDIATE 'GRANT SELECT ON HR_PERSONNEL.EMPLOYEES TO HR_ATTENDANCE';
    EXECUTE IMMEDIATE 'GRANT SELECT ON HR_PERSONNEL.EMPLOYEES TO HR_LEAVES';
    EXECUTE IMMEDIATE 'GRANT SELECT ON HR_PERSONNEL.EMPLOYEES TO HR_PAYROLL';
    EXECUTE IMMEDIATE 'GRANT SELECT ON HR_PERSONNEL.EMPLOYEES TO HR_PERFORMANCE';
    EXECUTE IMMEDIATE 'GRANT SELECT ON HR_PERSONNEL.EMPLOYEES TO HR_RECRUITMENT';
    
    EXECUTE IMMEDIATE 'GRANT REFERENCES ON HR_PERSONNEL.EMPLOYEES TO HR_ATTENDANCE';
    EXECUTE IMMEDIATE 'GRANT REFERENCES ON HR_PERSONNEL.EMPLOYEES TO HR_LEAVES';
    EXECUTE IMMEDIATE 'GRANT REFERENCES ON HR_PERSONNEL.EMPLOYEES TO HR_PAYROLL';
    EXECUTE IMMEDIATE 'GRANT REFERENCES ON HR_PERSONNEL.EMPLOYEES TO HR_PERFORMANCE';
    EXECUTE IMMEDIATE 'GRANT REFERENCES ON HR_PERSONNEL.EMPLOYEES TO HR_RECRUITMENT';
END;
/

PROMPT ✅ تم منح جميع الصلاحيات المتبادلة

-- =================================================================================
-- التحقق من الـ Schemas المنشأة
-- =================================================================================

PROMPT ========================================
PROMPT التحقق من الـ Schemas المنشأة
PROMPT ========================================

SELECT 
    username, 
    account_status, 
    TO_CHAR(created, 'YYYY-MM-DD HH24:MI:SS') as created_date,
    default_tablespace
FROM dba_users
WHERE username LIKE 'HR_%'
ORDER BY username;

PROMPT ========================================
PROMPT ✅ اكتمل إنشاء جميع الـ Schemas (9/9)
PROMPT ========================================

PROMPT 
PROMPT 📋 بيانات الاتصال من VS Code:
PROMPT ===================================
PROMPT Host: localhost
PROMPT Port: 1522
PROMPT Service Name: FREEPDB1
PROMPT 
PROMPT 🔐 Schemas & Passwords:
PROMPT ===================================
PROMPT 1. HR_CORE         : Core@2026
PROMPT 2. HR_PERSONNEL    : Personnel@2026
PROMPT 3. HR_ATTENDANCE   : Attend@2026
PROMPT 4. HR_LEAVES       : Leaves@2026
PROMPT 5. HR_PAYROLL      : Payroll@2026
PROMPT 6. HR_RECRUITMENT  : Recruit@2026
PROMPT 7. HR_PERFORMANCE  : Perform@2026
PROMPT 8. HR_System_Admin : SysAdmin@2026
PROMPT 9. HR_System_PDB   : SysPDB@2026
PROMPT ===================================
PROMPT 
PROMPT 🎯 الخطوة التالية:
PROMPT - نفذ السكربتات لإنشاء الجداول
PROMPT - أو قم بإعداد APEX Workspaces
PROMPT ===================================
