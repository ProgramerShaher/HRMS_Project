-- =================================================================================
-- إنشاء Application Context
-- الوصف: إنشاء namespace للـ context الخاص بالنظام
-- =================================================================================

-- ملاحظة: يجب تنفيذ هذا الـ script بصلاحيات SYSDBA أو DBA

-- إنشاء الـ Context
CREATE OR REPLACE CONTEXT HRMS_USER_CTX USING PKG_SECURITY_CONTEXT;

-- منح الصلاحيات اللازمة
GRANT EXECUTE ON DBMS_SESSION TO PUBLIC;

-- رسالة تأكيد
BEGIN
    DBMS_OUTPUT.PUT_LINE('✅ تم إنشاء Application Context بنجاح: HRMS_USER_CTX');
    DBMS_OUTPUT.PUT_LINE('✅ الـ Context مرتبط بـ Package: PKG_SECURITY_CONTEXT');
END;
/
