-- =================================================================================
-- الحزمة: PKG_AUDIT_TRIGGER_GENERATOR
-- الوصف: توليد Triggers تلقائياً لجميع الجداول (Body)
-- =================================================================================

CREATE OR REPLACE PACKAGE BODY PKG_AUDIT_TRIGGER_GENERATOR AS

    -- ==========================================================
    -- دالة خاصة: التحقق من وجود عمود في الجدول
    -- ==========================================================
    FUNCTION COLUMN_EXISTS (
        p_schema_name   IN VARCHAR2,
        p_table_name    IN VARCHAR2,
        p_column_name   IN VARCHAR2
    ) RETURN BOOLEAN IS
        v_count NUMBER;
    BEGIN
        SELECT COUNT(*)
        INTO v_count
        FROM ALL_TAB_COLUMNS
        WHERE OWNER = UPPER(p_schema_name)
          AND TABLE_NAME = UPPER(p_table_name)
          AND COLUMN_NAME = UPPER(p_column_name);
        
        RETURN v_count > 0;
    END COLUMN_EXISTS;

    -- ==========================================================
    -- دالة: الحصول على DDL لـ trigger معين
    -- ==========================================================
    FUNCTION GET_TRIGGER_DDL (
        p_schema_name   IN VARCHAR2,
        p_table_name    IN VARCHAR2
    ) RETURN CLOB IS
        v_ddl CLOB;
        v_trigger_name VARCHAR2(128);
        v_has_created_by BOOLEAN;
        v_has_created_at BOOLEAN;
        v_has_updated_by BOOLEAN;
        v_has_updated_at BOOLEAN;
        v_has_version_no BOOLEAN;
    BEGIN
        -- اسم الـ trigger
        v_trigger_name := 'TRG_' || UPPER(p_table_name) || '_AUDIT';
        
        -- التحقق من وجود الأعمدة
        v_has_created_by := COLUMN_EXISTS(p_schema_name, p_table_name, 'CREATED_BY');
        v_has_created_at := COLUMN_EXISTS(p_schema_name, p_table_name, 'CREATED_AT');
        v_has_updated_by := COLUMN_EXISTS(p_schema_name, p_table_name, 'UPDATED_BY');
        v_has_updated_at := COLUMN_EXISTS(p_schema_name, p_table_name, 'UPDATED_AT');
        v_has_version_no := COLUMN_EXISTS(p_schema_name, p_table_name, 'VERSION_NO');
        
        -- بناء الـ DDL
        v_ddl := '-- =============================================================' || CHR(10);
        v_ddl := v_ddl || '-- Trigger: ' || v_trigger_name || CHR(10);
        v_ddl := v_ddl || '-- Table: ' || p_schema_name || '.' || p_table_name || CHR(10);
        v_ddl := v_ddl || '-- Purpose: Auto-populate audit columns' || CHR(10);
        v_ddl := v_ddl || '-- Generated: ' || TO_CHAR(SYSDATE, 'YYYY-MM-DD HH24:MI:SS') || CHR(10);
        v_ddl := v_ddl || '-- =============================================================' || CHR(10);
        v_ddl := v_ddl || CHR(10);
        v_ddl := v_ddl || 'CREATE OR REPLACE TRIGGER ' || p_schema_name || '.' || v_trigger_name || CHR(10);
        v_ddl := v_ddl || 'BEFORE INSERT OR UPDATE ON ' || p_schema_name || '.' || p_table_name || CHR(10);
        v_ddl := v_ddl || 'FOR EACH ROW' || CHR(10);
        v_ddl := v_ddl || 'BEGIN' || CHR(10);
        
        -- INSERT logic
        IF v_has_created_by OR v_has_created_at THEN
            v_ddl := v_ddl || '    -- عند الإدخال' || CHR(10);
            v_ddl := v_ddl || '    IF INSERTING THEN' || CHR(10);
            
            IF v_has_created_by THEN
                v_ddl := v_ddl || '        :NEW.CREATED_BY := PKG_SECURITY_CONTEXT.GET_CURRENT_USER();' || CHR(10);
            END IF;
            
            IF v_has_created_at THEN
                v_ddl := v_ddl || '        :NEW.CREATED_AT := SYSDATE;' || CHR(10);
            END IF;
            
            IF v_has_version_no THEN
                v_ddl := v_ddl || '        :NEW.VERSION_NO := 1;' || CHR(10);
            END IF;
            
            v_ddl := v_ddl || '    END IF;' || CHR(10);
            v_ddl := v_ddl || CHR(10);
        END IF;
        
        -- UPDATE logic
        IF v_has_updated_by OR v_has_updated_at OR v_has_version_no THEN
            v_ddl := v_ddl || '    -- عند التحديث' || CHR(10);
            v_ddl := v_ddl || '    IF UPDATING THEN' || CHR(10);
            
            IF v_has_updated_by THEN
                v_ddl := v_ddl || '        :NEW.UPDATED_BY := PKG_SECURITY_CONTEXT.GET_CURRENT_USER();' || CHR(10);
            END IF;
            
            IF v_has_updated_at THEN
                v_ddl := v_ddl || '        :NEW.UPDATED_AT := SYSDATE;' || CHR(10);
            END IF;
            
            IF v_has_version_no THEN
                v_ddl := v_ddl || '        :NEW.VERSION_NO := NVL(:OLD.VERSION_NO, 0) + 1;' || CHR(10);
            END IF;
            
            -- منع تعديل CREATED_BY و CREATED_AT
            IF v_has_created_by THEN
                v_ddl := v_ddl || '        -- منع تعديل CREATED_BY' || CHR(10);
                v_ddl := v_ddl || '        :NEW.CREATED_BY := :OLD.CREATED_BY;' || CHR(10);
            END IF;
            
            IF v_has_created_at THEN
                v_ddl := v_ddl || '        -- منع تعديل CREATED_AT' || CHR(10);
                v_ddl := v_ddl || '        :NEW.CREATED_AT := :OLD.CREATED_AT;' || CHR(10);
            END IF;
            
            v_ddl := v_ddl || '    END IF;' || CHR(10);
        END IF;
        
        v_ddl := v_ddl || 'END ' || v_trigger_name || ';' || CHR(10);
        v_ddl := v_ddl || '/' || CHR(10);
        
        RETURN v_ddl;
    END GET_TRIGGER_DDL;

    -- ==========================================================
    -- إجراء: توليد trigger لجدول واحد
    -- ==========================================================
    PROCEDURE GENERATE_TRIGGER_FOR_TABLE (
        p_schema_name   IN VARCHAR2,
        p_table_name    IN VARCHAR2,
        p_execute       IN BOOLEAN DEFAULT FALSE
    ) IS
        v_ddl CLOB;
    BEGIN
        -- الحصول على DDL
        v_ddl := GET_TRIGGER_DDL(p_schema_name, p_table_name);
        
        -- طباعة DDL
        DBMS_OUTPUT.PUT_LINE(v_ddl);
        
        -- تنفيذ إذا طُلب ذلك
        IF p_execute THEN
            EXECUTE IMMEDIATE v_ddl;
            DBMS_OUTPUT.PUT_LINE('✅ تم إنشاء Trigger لـ ' || p_schema_name || '.' || p_table_name);
        END IF;
        
    EXCEPTION
        WHEN OTHERS THEN
            DBMS_OUTPUT.PUT_LINE('❌ خطأ في توليد Trigger لـ ' || p_schema_name || '.' || p_table_name || ': ' || SQLERRM);
            RAISE;
    END GENERATE_TRIGGER_FOR_TABLE;

    -- ==========================================================
    -- إجراء: توليد triggers لجميع جداول schema معين
    -- ==========================================================
    PROCEDURE GENERATE_TRIGGERS_FOR_SCHEMA (
        p_schema_name   IN VARCHAR2,
        p_execute       IN BOOLEAN DEFAULT FALSE
    ) IS
        v_count NUMBER := 0;
    BEGIN
        DBMS_OUTPUT.PUT_LINE('========================================');
        DBMS_OUTPUT.PUT_LINE('توليد Triggers لـ Schema: ' || p_schema_name);
        DBMS_OUTPUT.PUT_LINE('========================================');
        DBMS_OUTPUT.PUT_LINE('');
        
        FOR rec IN (
            SELECT DISTINCT TABLE_NAME
            FROM ALL_TAB_COLUMNS
            WHERE OWNER = UPPER(p_schema_name)
              AND (COLUMN_NAME IN ('CREATED_BY', 'CREATED_AT', 'UPDATED_BY', 'UPDATED_AT'))
            ORDER BY TABLE_NAME
        ) LOOP
            BEGIN
                GENERATE_TRIGGER_FOR_TABLE(p_schema_name, rec.TABLE_NAME, p_execute);
                v_count := v_count + 1;
                DBMS_OUTPUT.PUT_LINE('');
            EXCEPTION
                WHEN OTHERS THEN
                    DBMS_OUTPUT.PUT_LINE('⚠️ تخطي جدول: ' || rec.TABLE_NAME);
            END;
        END LOOP;
        
        DBMS_OUTPUT.PUT_LINE('========================================');
        DBMS_OUTPUT.PUT_LINE('✅ تم توليد ' || v_count || ' trigger');
        DBMS_OUTPUT.PUT_LINE('========================================');
    END GENERATE_TRIGGERS_FOR_SCHEMA;

    -- ==========================================================
    -- إجراء: توليد triggers لجميع الجداول في النظام
    -- ==========================================================
    PROCEDURE GENERATE_ALL_TRIGGERS (
        p_execute       IN BOOLEAN DEFAULT FALSE
    ) IS
        TYPE schema_list_type IS TABLE OF VARCHAR2(30);
        v_schemas schema_list_type := schema_list_type(
            'HR_CORE',
            'HR_PERSONNEL',
            'HR_ATTENDANCE',
            'HR_LEAVES',
            'HR_PAYROLL',
            'HR_RECRUITMENT',
            'HR_PERFORMANCE'
        );
    BEGIN
        DBMS_OUTPUT.PUT_LINE('');
        DBMS_OUTPUT.PUT_LINE('╔════════════════════════════════════════════════════════╗');
        DBMS_OUTPUT.PUT_LINE('║   توليد Triggers لجميع جداول نظام HRMS               ║');
        DBMS_OUTPUT.PUT_LINE('╚════════════════════════════════════════════════════════╝');
        DBMS_OUTPUT.PUT_LINE('');
        
        FOR i IN 1..v_schemas.COUNT LOOP
            BEGIN
                GENERATE_TRIGGERS_FOR_SCHEMA(v_schemas(i), p_execute);
                DBMS_OUTPUT.PUT_LINE('');
            EXCEPTION
                WHEN OTHERS THEN
                    DBMS_OUTPUT.PUT_LINE('⚠️ خطأ في Schema: ' || v_schemas(i));
                    DBMS_OUTPUT.PUT_LINE('   ' || SQLERRM);
            END;
        END LOOP;
        
        DBMS_OUTPUT.PUT_LINE('');
        DBMS_OUTPUT.PUT_LINE('╔════════════════════════════════════════════════════════╗');
        DBMS_OUTPUT.PUT_LINE('║   ✅ اكتمل توليد جميع الـ Triggers                    ║');
        DBMS_OUTPUT.PUT_LINE('╚════════════════════════════════════════════════════════╝');
    END GENERATE_ALL_TRIGGERS;

END PKG_AUDIT_TRIGGER_GENERATOR;
/
