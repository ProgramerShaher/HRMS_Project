-- =================================================================================
-- سكربت إنشاء جميع Schemas
-- الوصف: إنشاء جميع الـ Schemas المطلوبة لنظام HRMS
-- قاعدة البيانات: Oracle 23ai Free
-- =================================================================================

-- الاتصال بـ FREEPDB1
ALTER SESSION SET CONTAINER = FREEPDB1;

PROMPT ========================================
PROMPT إنشاء جميع Schemas لنظام HRMS
PROMPT ========================================

-- 1. HR_CORE: البيانات الأساسية والنظام
CREATE USER HR_CORE IDENTIFIED BY Pwd_Core_123
    DEFAULT TABLESPACE USERS
    TEMPORARY TABLESPACE TEMP
    QUOTA UNLIMITED ON USERS;

GRANT CONNECT, RESOURCE, CREATE VIEW TO HR_CORE;
PROMPT ✅ تم إنشاء HR_CORE

-- 2. HR_PERSONNEL: شؤون الموظفين
CREATE USER HR_PERSONNEL IDENTIFIED BY Pwd_Personnel_123
    DEFAULT TABLESPACE USERS
    TEMPORARY TABLESPACE TEMP
    QUOTA UNLIMITED ON USERS;

GRANT CONNECT, RESOURCE, CREATE VIEW TO HR_PERSONNEL;
PROMPT ✅ تم إنشاء HR_PERSONNEL

-- 3. HR_ATTENDANCE: الحضور والانصراف
CREATE USER HR_ATTENDANCE IDENTIFIED BY Pwd_Attend_123
    DEFAULT TABLESPACE USERS
    TEMPORARY TABLESPACE TEMP
    QUOTA UNLIMITED ON USERS;

GRANT CONNECT, RESOURCE, CREATE VIEW TO HR_ATTENDANCE;
PROMPT ✅ تم إنشاء HR_ATTENDANCE

-- 4. HR_LEAVES: الإجازات
CREATE USER HR_LEAVES IDENTIFIED BY Pwd_Leaves_123
    DEFAULT TABLESPACE USERS
    TEMPORARY TABLESPACE TEMP
    QUOTA UNLIMITED ON USERS;

GRANT CONNECT, RESOURCE, CREATE VIEW TO HR_LEAVES;
PROMPT ✅ تم إنشاء HR_LEAVES

-- 5. HR_PAYROLL: الرواتب
CREATE USER HR_PAYROLL IDENTIFIED BY Pwd_Payroll_123
    DEFAULT TABLESPACE USERS
    TEMPORARY TABLESPACE TEMP
    QUOTA UNLIMITED ON USERS;

GRANT CONNECT, RESOURCE, CREATE VIEW TO HR_PAYROLL;
PROMPT ✅ تم إنشاء HR_PAYROLL

-- 6. HR_RECRUITMENT: التوظيف
CREATE USER HR_RECRUITMENT IDENTIFIED BY Pwd_Recruit_123
    DEFAULT TABLESPACE USERS
    TEMPORARY TABLESPACE TEMP
    QUOTA UNLIMITED ON USERS;

GRANT CONNECT, RESOURCE, CREATE VIEW TO HR_RECRUITMENT;
PROMPT ✅ تم إنشاء HR_RECRUITMENT

-- 7. HR_PERFORMANCE: الأداء والجزاءات
CREATE USER HR_PERFORMANCE IDENTIFIED BY Pwd_Perform_123
    DEFAULT TABLESPACE USERS
    TEMPORARY TABLESPACE TEMP
    QUOTA UNLIMITED ON USERS;

GRANT CONNECT, RESOURCE, CREATE VIEW TO HR_PERFORMANCE;
PROMPT ✅ تم إنشاء HR_PERFORMANCE

PROMPT ========================================
PROMPT تم إنشاء جميع Schemas بنجاح!
PROMPT ========================================

-- التحقق من الإنشاء
SELECT username, account_status, created 
FROM dba_users 
WHERE username LIKE 'HR_%'
ORDER BY username;

PROMPT ========================================
PROMPT معلومات الاتصال:
PROMPT ========================================
PROMPT HR_CORE         : Pwd_Core_123
PROMPT HR_PERSONNEL    : Pwd_Personnel_123
PROMPT HR_ATTENDANCE   : Pwd_Attend_123
PROMPT HR_LEAVES       : Pwd_Leaves_123
PROMPT HR_PAYROLL      : Pwd_Payroll_123
PROMPT HR_RECRUITMENT  : Pwd_Recruit_123
PROMPT HR_PERFORMANCE  : Pwd_Perform_123
PROMPT ========================================
