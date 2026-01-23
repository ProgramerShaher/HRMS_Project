-- =================================================================================
-- الحزمة: PKG_AUDIT_TRIGGER_GENERATOR
-- الوصف: توليد Triggers تلقائياً لجميع الجداول
-- الهدف: أتمتة إنشاء triggers للـ audit logging
-- =================================================================================

CREATE OR REPLACE PACKAGE PKG_AUDIT_TRIGGER_GENERATOR AS

    -- ==========================================================
    -- إجراء: توليد trigger لجدول واحد
    -- ==========================================================
    PROCEDURE GENERATE_TRIGGER_FOR_TABLE (
        p_schema_name   IN VARCHAR2,
        p_table_name    IN VARCHAR2,
        p_execute       IN BOOLEAN DEFAULT FALSE
    );

    -- ==========================================================
    -- إجراء: توليد triggers لجميع جداول schema معين
    -- ==========================================================
    PROCEDURE GENERATE_TRIGGERS_FOR_SCHEMA (
        p_schema_name   IN VARCHAR2,
        p_execute       IN BOOLEAN DEFAULT FALSE
    );

    -- ==========================================================
    -- إجراء: توليد triggers لجميع الجداول في النظام
    -- ==========================================================
    PROCEDURE GENERATE_ALL_TRIGGERS (
        p_execute       IN BOOLEAN DEFAULT FALSE
    );

    -- ==========================================================
    -- دالة: الحصول على DDL لـ trigger معين
    -- ==========================================================
    FUNCTION GET_TRIGGER_DDL (
        p_schema_name   IN VARCHAR2,
        p_table_name    IN VARCHAR2
    ) RETURN CLOB;

END PKG_AUDIT_TRIGGER_GENERATOR;
/
