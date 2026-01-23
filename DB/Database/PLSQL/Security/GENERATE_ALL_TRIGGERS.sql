-- =================================================================================
-- Script: توليد جميع الـ Audit Triggers
-- الوصف: توليد triggers لجميع الجداول في النظام
-- الاستخدام: يمكن تشغيله بشكل منفصل لتوليد الـ triggers فقط
-- =================================================================================

SET SERVEROUTPUT ON SIZE UNLIMITED
SET LINESIZE 200
SET PAGESIZE 1000

PROMPT ╔════════════════════════════════════════════════════════╗
PROMPT ║   توليد Audit Triggers لجميع جداول HRMS             ║
PROMPT ╚════════════════════════════════════════════════════════╝
PROMPT

-- توليد وتنفيذ جميع الـ triggers
BEGIN
    PKG_AUDIT_TRIGGER_GENERATOR.GENERATE_ALL_TRIGGERS(p_execute => TRUE);
EXCEPTION
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('❌ خطأ في توليد الـ Triggers: ' || SQLERRM);
        RAISE;
END;
/

PROMPT
PROMPT ========================================
PROMPT إحصائيات الـ Triggers المُنشأة
PROMPT ========================================
PROMPT

-- عرض إحصائيات مفصلة
SELECT 
    OWNER AS SCHEMA_NAME,
    COUNT(*) AS TOTAL_TRIGGERS,
    SUM(CASE WHEN STATUS = 'ENABLED' THEN 1 ELSE 0 END) AS ENABLED,
    SUM(CASE WHEN STATUS = 'DISABLED' THEN 1 ELSE 0 END) AS DISABLED
FROM ALL_TRIGGERS
WHERE TRIGGER_NAME LIKE 'TRG_%_AUDIT'
  AND OWNER IN ('HR_CORE', 'HR_PERSONNEL', 'HR_ATTENDANCE', 'HR_LEAVES', 
                'HR_PAYROLL', 'HR_RECRUITMENT', 'HR_PERFORMANCE')
GROUP BY OWNER
ORDER BY OWNER;

PROMPT
PROMPT ✅ اكتمل توليد جميع الـ Triggers
PROMPT
