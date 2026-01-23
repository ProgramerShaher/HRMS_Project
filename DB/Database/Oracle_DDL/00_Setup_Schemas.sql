-- -- =================================================================================
-- -- اسم السكربت: 00_Setup_Schemas.sql
-- -- الوصف: إنشاء مستخدمين (Schemas) أوراكل لفصل منطق نظام الموارد البشرية.
-- -- التشغيل: يجب التشغيل بصلاحية SYSDBA أو مستخدم SYSTEM متصل بـ PDB (xepdb1)
-- -- =================================================================================

-- -- التأكد من أننا في PDB وليس CDB
-- ALTER SESSION SET CONTAINER = XEPDB1;

-- -- 1. إنشاء Tablespace (اختياري ولكنه موصى به)
-- -- يمكنك تفعيل هذا السطر إذا أردت tablespace منفصل
-- -- CREATE TABLESPACE tbs_hrms_data DATAFILE SIZE 100M AUTOEXTEND ON NEXT 10M;

-- -- 2. إنشاء المستخدمين (Schemas)

-- -- HR_CORE: الأساس والإعدادات العامة
-- CREATE USER HR_CORE IDENTIFIED BY Pwd_Core_123
--     DEFAULT TABLESPACE USERS
--     TEMPORARY TABLESPACE TEMP
--     QUOTA UNLIMITED ON USERS;
-- GRANT CONNECT, RESOURCE, CREATE VIEW TO HR_CORE;

-- -- HR_RECRUITMENT: التوظيف
-- CREATE USER HR_RECRUITMENT IDENTIFIED BY Pwd_Recruit_123
--     DEFAULT TABLESPACE USERS
--     TEMPORARY TABLESPACE TEMP
--     QUOTA UNLIMITED ON USERS;
-- GRANT CONNECT, RESOURCE TO HR_RECRUITMENT;

-- -- HR_PERSONNEL: شؤون الموظفين
-- CREATE USER HR_PERSONNEL IDENTIFIED BY Pwd_Personnel_123
--     DEFAULT TABLESPACE USERS
--     TEMPORARY TABLESPACE TEMP
--     QUOTA UNLIMITED ON USERS;
-- GRANT CONNECT, RESOURCE TO HR_PERSONNEL;

-- -- HR_ATTENDANCE: الحضور والانصراف
-- CREATE USER HR_ATTENDANCE IDENTIFIED BY Pwd_Attend_123
--     DEFAULT TABLESPACE USERS
--     TEMPORARY TABLESPACE TEMP
--     QUOTA UNLIMITED ON USERS;
-- GRANT CONNECT, RESOURCE TO HR_ATTENDANCE;

-- -- HR_LEAVES: الإجازات
-- CREATE USER HR_LEAVES IDENTIFIED BY Pwd_Leaves_123
--     DEFAULT TABLESPACE USERS
--     TEMPORARY TABLESPACE TEMP
--     QUOTA UNLIMITED ON USERS;
-- GRANT CONNECT, RESOURCE TO HR_LEAVES;

-- -- HR_PAYROLL: الرواتب والأجور
-- CREATE USER HR_PAYROLL IDENTIFIED BY Pwd_Payroll_123
--     DEFAULT TABLESPACE USERS
--     TEMPORARY TABLESPACE TEMP
--     QUOTA UNLIMITED ON USERS;
-- GRANT CONNECT, RESOURCE TO HR_PAYROLL;

-- -- HR_PERFORMANCE: الأداء والجزاءات
-- CREATE USER HR_PERFORMANCE IDENTIFIED BY Pwd_Perform_123
--     DEFAULT TABLESPACE USERS
--     TEMPORARY TABLESPACE TEMP
--     QUOTA UNLIMITED ON USERS;
-- GRANT CONNECT, RESOURCE TO HR_PERFORMANCE;

-- -- 3. صلاحيات الوصول المتبادل بين المخططات (ضروري للمفاتيح الأجنبية FK)

-- -- ملاحظة: سيتم منح الصلاحيات (GRANT REFERENCES) بعد إنشاء الجداول في كل سكربت
-- -- مثال: GRANT REFERENCES ON HR_CORE.DEPARTMENTS TO HR_PERSONNEL;

-- -- تمكين المستخدمين من الاستعلام من بعضهم البعض للتقارير (اختياري)
-- -- GRANT SELECT ANY TABLE TO ADMIN_USER;

-- -- رسالة نجاح
-- SELECT 'تم إنشاء جميع المستخدمين بنجاح!' AS STATUS FROM DUAL;
