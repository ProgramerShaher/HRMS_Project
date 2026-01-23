-- =================================================================================
-- Script: تثبيت نظام Audit Security
-- الوصف: تثبيت كامل للبنية التحتية لتأمين بيانات الـ Audit
-- الترتيب: يجب تنفيذ هذا الـ script بالترتيب
-- =================================================================================

SET SERVEROUTPUT ON SIZE UNLIMITED
SET ECHO ON
SET FEEDBACK ON

PROMPT ╔════════════════════════════════════════════════════════╗
PROMPT ║   تثبيت نظام Audit Security - HRMS                   ║
PROMPT ╚════════════════════════════════════════════════════════╝
PROMPT

-- =================================================================================
-- الخطوة 1: إنشاء Application Context
-- =================================================================================
PROMPT ========================================
PROMPT الخطوة 1: إنشاء Application Context
PROMPT ========================================
PROMPT

@@00_CREATE_CONTEXT.sql

PROMPT
PROMPT ✅ تم إنشاء Application Context
PROMPT

-- =================================================================================
-- الخطوة 2: إنشاء Security Context Package
-- =================================================================================
PROMPT ========================================
PROMPT الخطوة 2: إنشاء Security Context Package
PROMPT ========================================
PROMPT

PROMPT إنشاء Package Specification...
@@01_PKG_SECURITY_CONTEXT_SPEC.sql

PROMPT إنشاء Package Body...
@@02_PKG_SECURITY_CONTEXT_BODY.sql

PROMPT
PROMPT ✅ تم إنشاء PKG_SECURITY_CONTEXT
PROMPT

-- =================================================================================
-- الخطوة 3: إنشاء Audit Trigger Generator Package
-- =================================================================================
PROMPT ========================================
PROMPT الخطوة 3: إنشاء Trigger Generator Package
PROMPT ========================================
PROMPT

PROMPT إنشاء Package Specification...
@@03_PKG_AUDIT_TRIGGER_GENERATOR_SPEC.sql

PROMPT إنشاء Package Body...
@@04_PKG_AUDIT_TRIGGER_GENERATOR_BODY.sql

PROMPT
PROMPT ✅ تم إنشاء PKG_AUDIT_TRIGGER_GENERATOR
PROMPT

-- =================================================================================
-- الخطوة 4: اختبار Security Context
-- =================================================================================
PROMPT ========================================
PROMPT الخطوة 4: اختبار Security Context
PROMPT ========================================
PROMPT

DECLARE
    v_user VARCHAR2(100);
BEGIN
    -- تعيين معلومات المستخدم
    PKG_SECURITY_CONTEXT.SET_USER_INFO('TEST_USER', 999);
    
    -- الحصول على المستخدم
    v_user := PKG_SECURITY_CONTEXT.GET_CURRENT_USER();
    
    DBMS_OUTPUT.PUT_LINE('المستخدم الحالي: ' || v_user);
    
    IF v_user = 'TEST_USER' THEN
        DBMS_OUTPUT.PUT_LINE('✅ اختبار Security Context نجح');
    ELSE
        DBMS_OUTPUT.PUT_LINE('❌ اختبار Security Context فشل');
    END IF;
    
    -- مسح الـ context
    PKG_SECURITY_CONTEXT.CLEAR_CONTEXT();
END;
/

PROMPT

-- =================================================================================
-- الخطوة 5: توليد جميع الـ Triggers
-- =================================================================================
PROMPT ========================================
PROMPT الخطوة 5: توليد جميع الـ Triggers
PROMPT ========================================
PROMPT
PROMPT ⚠️ ملاحظة: سيتم توليد triggers لجميع الجداول (75 جدول)
PROMPT هذه العملية قد تستغرق بضع دقائق...
PROMPT

-- توليد وتنفيذ جميع الـ triggers
BEGIN
    PKG_AUDIT_TRIGGER_GENERATOR.GENERATE_ALL_TRIGGERS(p_execute => TRUE);
END;
/

PROMPT

-- =================================================================================
-- الخطوة 6: التحقق من التثبيت
-- =================================================================================
PROMPT ========================================
PROMPT الخطوة 6: التحقق من التثبيت
PROMPT ========================================
PROMPT

-- عرض إحصائيات الـ Triggers
SELECT 
    OWNER,
    COUNT(*) AS TRIGGER_COUNT,
    SUM(CASE WHEN STATUS = 'ENABLED' THEN 1 ELSE 0 END) AS ENABLED_COUNT,
    SUM(CASE WHEN STATUS = 'DISABLED' THEN 1 ELSE 0 END) AS DISABLED_COUNT
FROM ALL_TRIGGERS
WHERE TRIGGER_NAME LIKE 'TRG_%_AUDIT'
  AND OWNER IN ('HR_CORE', 'HR_PERSONNEL', 'HR_ATTENDANCE', 'HR_LEAVES', 
                'HR_PAYROLL', 'HR_RECRUITMENT', 'HR_PERFORMANCE')
GROUP BY OWNER
ORDER BY OWNER;

PROMPT

-- عرض معلومات الـ Packages
SELECT 
    OBJECT_NAME,
    OBJECT_TYPE,
    STATUS,
    TO_CHAR(LAST_DDL_TIME, 'YYYY-MM-DD HH24:MI:SS') AS LAST_MODIFIED
FROM USER_OBJECTS
WHERE OBJECT_NAME IN ('PKG_SECURITY_CONTEXT', 'PKG_AUDIT_TRIGGER_GENERATOR')
ORDER BY OBJECT_TYPE, OBJECT_NAME;

PROMPT

-- =================================================================================
-- النتيجة النهائية
-- =================================================================================
PROMPT ╔════════════════════════════════════════════════════════╗
PROMPT ║   ✅ اكتمل تثبيت نظام Audit Security بنجاح          ║
PROMPT ╚════════════════════════════════════════════════════════╝
PROMPT
PROMPT الخطوات التالية:
PROMPT 1. تعديل الـ Packages لإزالة audit parameters
PROMPT 2. تحديث الـ Backend API لاستخدام Security Context
PROMPT 3. اختبار النظام بشكل شامل
PROMPT
PROMPT للمزيد من المعلومات، راجع: BACKEND_MIGRATION_GUIDE.md
PROMPT
