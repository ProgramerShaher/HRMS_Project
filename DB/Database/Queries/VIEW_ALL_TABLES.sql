-- =================================================================================
-- استعلامات لعرض جميع الجداول الـ 75
-- =================================================================================

ALTER SESSION SET CONTAINER = XEPDB1;

-- 1. عرض جميع الجداول مع عدد الأعمدة
SELECT 
    owner AS SCHEMA_NAME,
    table_name AS TABLE_NAME,
    (SELECT COUNT(*) FROM all_tab_columns c WHERE c.owner = t.owner AND c.table_name = t.table_name) AS COLUMN_COUNT,
    num_rows AS ROW_COUNT
FROM all_tables t
WHERE owner IN ('HR_CORE', 'HR_PERSONNEL', 'HR_ATTENDANCE', 'HR_LEAVES', 'HR_PAYROLL', 'HR_RECRUITMENT', 'HR_PERFORMANCE')
ORDER BY owner, table_name;

-- 2. إحصائيات الجداول لكل Schema
SELECT 
    owner AS SCHEMA_NAME,
    COUNT(*) AS TABLE_COUNT
FROM all_tables
WHERE owner IN ('HR_CORE', 'HR_PERSONNEL', 'HR_ATTENDANCE', 'HR_LEAVES', 'HR_PAYROLL', 'HR_RECRUITMENT', 'HR_PERFORMANCE')
GROUP BY owner
ORDER BY owner;

-- 3. الجداول الجديدة فقط (الـ 14 جدول)
SELECT owner, table_name, 'جدول جديد' AS STATUS
FROM all_tables
WHERE owner = 'HR_CORE' AND table_name IN ('SYSTEM_USERS', 'SYSTEM_ROLES', 'SYSTEM_PERMISSIONS', 'USER_ROLES', 'ROLE_PERMISSIONS', 'ASSET_CATEGORIES', 'COMPANY_ASSETS')
UNION ALL
SELECT owner, table_name, 'جدول جديد' AS STATUS
FROM all_tables
WHERE owner = 'HR_PERSONNEL' AND table_name IN ('TRAINING_COURSES', 'EMPLOYEE_TRAINING', 'EMPLOYEE_ASSETS', 'EMPLOYEE_TRANSFERS')
UNION ALL
SELECT owner, table_name, 'جدول جديد' AS STATUS
FROM all_tables
WHERE owner = 'HR_PAYROLL' AND table_name IN ('INSURANCE_PLANS', 'EMPLOYEE_INSURANCE', 'BONUSES')
ORDER BY owner, table_name;

-- 4. عرض أعمدة جدول معين (مثال: SYSTEM_USERS)
SELECT 
    column_name,
    data_type,
    data_length,
    nullable,
    column_id
FROM all_tab_columns
WHERE owner = 'HR_CORE' AND table_name = 'SYSTEM_USERS'
ORDER BY column_id;

-- 5. عرض Foreign Keys للجداول الجديدة
SELECT 
    c.constraint_name,
    c.table_name,
    c.r_constraint_name AS REFERENCES_CONSTRAINT,
    (SELECT table_name FROM all_constraints WHERE constraint_name = c.r_constraint_name AND owner = c.owner) AS REFERENCES_TABLE
FROM all_constraints c
WHERE c.owner IN ('HR_CORE', 'HR_PERSONNEL', 'HR_PAYROLL')
  AND c.constraint_type = 'R'
  AND c.table_name IN ('SYSTEM_USERS', 'USER_ROLES', 'ROLE_PERMISSIONS', 'TRAINING_COURSES', 'EMPLOYEE_TRAINING', 
                       'INSURANCE_PLANS', 'EMPLOYEE_INSURANCE', 'BONUSES', 'ASSET_CATEGORIES', 'COMPANY_ASSETS', 
                       'EMPLOYEE_ASSETS', 'EMPLOYEE_TRANSFERS')
ORDER BY c.table_name;

-- 6. المجموع النهائي
SELECT 'إجمالي الجداول: ' || COUNT(*) || ' جدول' AS SUMMARY
FROM all_tables
WHERE owner IN ('HR_CORE', 'HR_PERSONNEL', 'HR_ATTENDANCE', 'HR_LEAVES', 'HR_PAYROLL', 'HR_RECRUITMENT', 'HR_PERFORMANCE');
