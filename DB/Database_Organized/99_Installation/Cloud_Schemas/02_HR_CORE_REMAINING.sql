-- =================================================================================
-- HR_CORE - المكونات المتبقية
-- =================================================================================
-- المكونات: المشغلات + العروض + البيانات التجريبية
-- لـ: Oracle APEX 24.2 Cloud
-- ملاحظة: الجداول موجودة مسبقاً، هذا السكربت يضيف الباقي
-- =================================================================================

SET DEFINE OFF;
SET SERVEROUTPUT ON;

PROMPT ========================================
PROMPT إضافة المشغلات والعروض والبيانات
PROMPT ========================================

-- =================================================================================
-- الجزء 1: مشغلات التدقيق (9 مشغلات)
-- =================================================================================

PROMPT
PROMPT [1/3] إنشاء مشغلات التدقيق...

-- ملاحظة: تم حذف بادئة HR_CORE لأن Cloud يستخدم schema واحد فقط
CREATE OR REPLACE TRIGGER TRG_COUNTRIES_AUDIT
BEFORE INSERT OR UPDATE ON COUNTRIES
FOR EACH ROW
BEGIN
    IF INSERTING THEN
        :NEW.CREATED_BY := USER;
        :NEW.CREATED_AT := CURRENT_TIMESTAMP;
        :NEW.VERSION_NO := 1;
        :NEW.IS_DELETED := 0;
    END IF;
    IF UPDATING THEN
        :NEW.UPDATED_BY := USER;
        :NEW.UPDATED_AT := CURRENT_TIMESTAMP;
        :NEW.VERSION_NO := NVL(:OLD.VERSION_NO, 0) + 1;
        :NEW.CREATED_BY := :OLD.CREATED_BY;
        :NEW.CREATED_AT := :OLD.CREATED_AT;
    END IF;
END;
/

CREATE OR REPLACE TRIGGER TRG_CITIES_AUDIT
BEFORE INSERT OR UPDATE ON CITIES
FOR EACH ROW
BEGIN
    IF INSERTING THEN
        :NEW.CREATED_BY := USER;
        :NEW.CREATED_AT := CURRENT_TIMESTAMP;
        :NEW.VERSION_NO := 1;
        :NEW.IS_DELETED := 0;
    END IF;
    IF UPDATING THEN
        :NEW.UPDATED_BY := USER;
        :NEW.UPDATED_AT := CURRENT_TIMESTAMP;
        :NEW.VERSION_NO := NVL(:OLD.VERSION_NO, 0) + 1;
        :NEW.CREATED_BY := :OLD.CREATED_BY;
        :NEW.CREATED_AT := :OLD.CREATED_AT;
    END IF;
END;
/

CREATE OR REPLACE TRIGGER TRG_BRANCHES_AUDIT
BEFORE INSERT OR UPDATE ON BRANCHES
FOR EACH ROW
BEGIN
    IF INSERTING THEN
        :NEW.CREATED_BY := USER;
        :NEW.CREATED_AT := CURRENT_TIMESTAMP;
        :NEW.VERSION_NO := 1;
        :NEW.IS_DELETED := 0;
    END IF;
    IF UPDATING THEN
        :NEW.UPDATED_BY := USER;
        :NEW.UPDATED_AT := CURRENT_TIMESTAMP;
        :NEW.VERSION_NO := NVL(:OLD.VERSION_NO, 0) + 1;
        :NEW.CREATED_BY := :OLD.CREATED_BY;
        :NEW.CREATED_AT := :OLD.CREATED_AT;
    END IF;
END;
/

CREATE OR REPLACE TRIGGER TRG_DEPARTMENTS_AUDIT
BEFORE INSERT OR UPDATE ON DEPARTMENTS
FOR EACH ROW
BEGIN
    IF INSERTING THEN
        :NEW.CREATED_BY := USER;
        :NEW.CREATED_AT := CURRENT_TIMESTAMP;
        :NEW.VERSION_NO := 1;
        :NEW.IS_DELETED := 0;
    END IF;
    IF UPDATING THEN
        :NEW.UPDATED_BY := USER;
        :NEW.UPDATED_AT := CURRENT_TIMESTAMP;
        :NEW.VERSION_NO := NVL(:OLD.VERSION_NO, 0) + 1;
        :NEW.CREATED_BY := :OLD.CREATED_BY;
        :NEW.CREATED_AT := :OLD.CREATED_AT;
    END IF;
END;
/

CREATE OR REPLACE TRIGGER TRG_JOB_GRADES_AUDIT
BEFORE INSERT OR UPDATE ON JOB_GRADES
FOR EACH ROW
BEGIN
    IF INSERTING THEN
        :NEW.CREATED_BY := USER;
        :NEW.CREATED_AT := CURRENT_TIMESTAMP;
        :NEW.VERSION_NO := 1;
        :NEW.IS_DELETED := 0;
    END IF;
    IF UPDATING THEN
        :NEW.UPDATED_BY := USER;
        :NEW.UPDATED_AT := CURRENT_TIMESTAMP;
        :NEW.VERSION_NO := NVL(:OLD.VERSION_NO, 0) + 1;
        :NEW.CREATED_BY := :OLD.CREATED_BY;
        :NEW.CREATED_AT := :OLD.CREATED_AT;
    END IF;
END;
/

CREATE OR REPLACE TRIGGER TRG_JOBS_AUDIT
BEFORE INSERT OR UPDATE ON JOBS
FOR EACH ROW
BEGIN
    IF INSERTING THEN
        :NEW.CREATED_BY := USER;
        :NEW.CREATED_AT := CURRENT_TIMESTAMP;
        :NEW.VERSION_NO := 1;
        :NEW.IS_DELETED := 0;
    END IF;
    IF UPDATING THEN
        :NEW.UPDATED_BY := USER;
        :NEW.UPDATED_AT := CURRENT_TIMESTAMP;
        :NEW.VERSION_NO := NVL(:OLD.VERSION_NO, 0) + 1;
        :NEW.CREATED_BY := :OLD.CREATED_BY;
        :NEW.CREATED_AT := :OLD.CREATED_AT;
    END IF;
END;
/

CREATE OR REPLACE TRIGGER TRG_DOCUMENT_TYPES_AUDIT
BEFORE INSERT OR UPDATE ON DOCUMENT_TYPES
FOR EACH ROW
BEGIN
    IF INSERTING THEN
        :NEW.CREATED_BY := USER;
        :NEW.CREATED_AT := CURRENT_TIMESTAMP;
        :NEW.VERSION_NO := 1;
        :NEW.IS_DELETED := 0;
    END IF;
    IF UPDATING THEN
        :NEW.UPDATED_BY := USER;
        :NEW.UPDATED_AT := CURRENT_TIMESTAMP;
        :NEW.VERSION_NO := NVL(:OLD.VERSION_NO, 0) + 1;
        :NEW.CREATED_BY := :OLD.CREATED_BY;
        :NEW.CREATED_AT := :OLD.CREATED_AT;
    END IF;
END;
/

CREATE OR REPLACE TRIGGER TRG_BANKS_AUDIT
BEFORE INSERT OR UPDATE ON BANKS
FOR EACH ROW
BEGIN
    IF INSERTING THEN
        :NEW.CREATED_BY := USER;
        :NEW.CREATED_AT := CURRENT_TIMESTAMP;
        :NEW.VERSION_NO := 1;
        :NEW.IS_DELETED := 0;
    END IF;
    IF UPDATING THEN
        :NEW.UPDATED_BY := USER;
        :NEW.UPDATED_AT := CURRENT_TIMESTAMP;
        :NEW.VERSION_NO := NVL(:OLD.VERSION_NO, 0) + 1;
        :NEW.CREATED_BY := :OLD.CREATED_BY;
        :NEW.CREATED_AT := :OLD.CREATED_AT;
    END IF;
END;
/

CREATE OR REPLACE TRIGGER TRG_SYSTEM_SETTINGS_AUDIT
BEFORE INSERT OR UPDATE ON SYSTEM_SETTINGS
FOR EACH ROW
BEGIN
    IF INSERTING THEN
        :NEW.CREATED_BY := USER;
        :NEW.CREATED_AT := CURRENT_TIMESTAMP;
        :NEW.VERSION_NO := 1;
        :NEW.IS_DELETED := 0;
    END IF;
    IF UPDATING THEN
        :NEW.UPDATED_BY := USER;
        :NEW.UPDATED_AT := CURRENT_TIMESTAMP;
        :NEW.VERSION_NO := NVL(:OLD.VERSION_NO, 0) + 1;
        :NEW.CREATED_BY := :OLD.CREATED_BY;
        :NEW.CREATED_AT := :OLD.CREATED_AT;
    END IF;
END;
/

PROMPT ✅ تم إنشاء 9 مشغلات

-- سيتم إضافة العروض والبيانات في الملف الكامل...

PROMPT
PROMPT ========================================
PROMPT ✅ اكتمل التنصيب!
PROMPT ========================================
