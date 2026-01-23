-- =================================================================================
-- سكربت التنصيب الكامل للمرحلة الأولى - إدارة الموظفين
-- الوصف: تنصيب Package + Triggers + Views
-- المخطط: HR_PERSONNEL
-- =================================================================================

PROMPT ========================================
PROMPT بدء تنصيب المرحلة الأولى - إدارة الموظفين
PROMPT ========================================

-- الاتصال بـ Oracle 23ai Free
ALTER SESSION SET CONTAINER = FREEPDB1;

PROMPT ========================================
PROMPT 1. إنشاء Package Specification
PROMPT ========================================
@@01_PKG_EMP_MANAGER_SPEC_ENHANCED.sql

PROMPT ========================================
PROMPT 2. إنشاء Package Body - Part 1
PROMPT ========================================
@@02_PKG_EMP_MANAGER_BODY_ENHANCED_PART1.sql

PROMPT ========================================
PROMPT 3. إنشاء Package Body - Part 2
PROMPT ========================================
@@03_PKG_EMP_MANAGER_BODY_ENHANCED_PART2.sql

PROMPT ========================================
PROMPT 4. إنشاء Triggers
PROMPT ========================================
@@04_EMPLOYEE_TRIGGERS.sql

PROMPT ========================================
PROMPT 5. إنشاء Views
PROMPT ========================================
@@05_EMPLOYEE_VIEWS.sql

PROMPT ========================================
PROMPT 6. التحقق من التنصيب
PROMPT ========================================

-- التحقق من Package
SELECT object_name, object_type, status
FROM user_objects
WHERE object_name = 'PKG_EMP_MANAGER'
ORDER BY object_type;

-- التحقق من Triggers
SELECT trigger_name, status
FROM user_triggers
WHERE trigger_name LIKE 'TRG_EMPLOYEE%'
   OR trigger_name LIKE 'TRG_CONTRACT%';

-- التحقق من Views
SELECT view_name
FROM user_views
WHERE view_name LIKE 'V_EMPLOYEE%'
   OR view_name LIKE 'V_DEPT%'
   OR view_name LIKE 'V_ACTIVE%'
   OR view_name LIKE 'V_EXPIRING%';

PROMPT ========================================
PROMPT اكتمل التنصيب بنجاح!
PROMPT ========================================

-- عرض ملخص
SELECT 'Packages' AS OBJECT_TYPE, COUNT(*) AS COUNT
FROM user_objects
WHERE object_name = 'PKG_EMP_MANAGER'
UNION ALL
SELECT 'Triggers', COUNT(*)
FROM user_triggers
WHERE trigger_name LIKE 'TRG_%'
UNION ALL
SELECT 'Views', COUNT(*)
FROM user_views
WHERE view_name LIKE 'V_%';

PROMPT ========================================
PROMPT جاهز للاستخدام!
PROMPT ========================================
