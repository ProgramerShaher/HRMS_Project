-- =================================================================================
-- Triggers للتدقيق التلقائي (Audit Triggers)
-- الوصف: تسجيل تلقائي لجميع العمليات على جداول الموظفين
-- المخطط: HR_PERSONNEL
-- =================================================================================

-- ==========================================================
-- 1. Trigger لتسجيل إضافة موظف جديد
-- ==========================================================
CREATE OR REPLACE TRIGGER HR_PERSONNEL.TRG_EMPLOYEE_INSERT_AUDIT
AFTER INSERT ON HR_PERSONNEL.EMPLOYEES
FOR EACH ROW
BEGIN
    INSERT INTO HR_CORE.AUDIT_LOGS (
        TABLE_NAME, RECORD_ID, ACTION_TYPE,
        NEW_VALUE, PERFORMED_BY, PERFORMED_AT
    ) VALUES (
        'EMPLOYEES', :NEW.EMPLOYEE_ID, 'INSERT',
        'رقم الموظف: ' || :NEW.EMPLOYEE_NUMBER || ', الاسم: ' || :NEW.FULL_NAME_EN,
        :NEW.CREATED_BY, CURRENT_TIMESTAMP
    );
END;
/

-- ==========================================================
-- 2. Trigger لتسجيل تحديث بيانات موظف
-- ==========================================================
CREATE OR REPLACE TRIGGER HR_PERSONNEL.TRG_EMPLOYEE_UPDATE_AUDIT
AFTER UPDATE ON HR_PERSONNEL.EMPLOYEES
FOR EACH ROW
DECLARE
    v_changes VARCHAR2(4000);
BEGIN
    v_changes := '';
    
    IF :OLD.EMAIL != :NEW.EMAIL OR (:OLD.EMAIL IS NULL AND :NEW.EMAIL IS NOT NULL) THEN
        v_changes := v_changes || 'البريد: ' || :OLD.EMAIL || ' → ' || :NEW.EMAIL || '; ';
    END IF;
    
    IF :OLD.MOBILE != :NEW.MOBILE OR (:OLD.MOBILE IS NULL AND :NEW.MOBILE IS NOT NULL) THEN
        v_changes := v_changes || 'الجوال: ' || :OLD.MOBILE || ' → ' || :NEW.MOBILE || '; ';
    END IF;
    
    IF :OLD.STATUS != :NEW.STATUS THEN
        v_changes := v_changes || 'الحالة: ' || :OLD.STATUS || ' → ' || :NEW.STATUS || '; ';
    END IF;
    
    IF :OLD.DEPT_ID != :NEW.DEPT_ID THEN
        v_changes := v_changes || 'القسم تغير من ' || :OLD.DEPT_ID || ' إلى ' || :NEW.DEPT_ID || '; ';
    END IF;
    
    IF LENGTH(v_changes) > 0 THEN
        INSERT INTO HR_CORE.AUDIT_LOGS (
            TABLE_NAME, RECORD_ID, ACTION_TYPE,
            OLD_VALUE, NEW_VALUE, PERFORMED_BY, PERFORMED_AT
        ) VALUES (
            'EMPLOYEES', :NEW.EMPLOYEE_ID, 'UPDATE',
            'التغييرات القديمة', v_changes,
            :NEW.UPDATED_BY, CURRENT_TIMESTAMP
        );
    END IF;
END;
/

-- ==========================================================
-- 3. Trigger لمنع الحذف الفعلي (Soft Delete)
-- ==========================================================
CREATE OR REPLACE TRIGGER HR_PERSONNEL.TRG_EMPLOYEE_PREVENT_DELETE
BEFORE DELETE ON HR_PERSONNEL.EMPLOYEES
FOR EACH ROW
BEGIN
    RAISE_APPLICATION_ERROR(-20100, 
        'لا يمكن حذف الموظف. استخدم الحذف المنطقي (IS_DELETED = 1)');
END;
/

-- ==========================================================
-- 4. Trigger لتسجيل إنشاء عقد جديد
-- ==========================================================
CREATE OR REPLACE TRIGGER HR_PERSONNEL.TRG_CONTRACT_INSERT_AUDIT
AFTER INSERT ON HR_PERSONNEL.CONTRACTS
FOR EACH ROW
BEGIN
    INSERT INTO HR_CORE.AUDIT_LOGS (
        TABLE_NAME, RECORD_ID, ACTION_TYPE,
        NEW_VALUE, PERFORMED_BY, PERFORMED_AT
    ) VALUES (
        'CONTRACTS', :NEW.CONTRACT_ID, 'INSERT',
        'موظف: ' || :NEW.EMPLOYEE_ID || ', نوع العقد: ' || :NEW.CONTRACT_TYPE || 
        ', الراتب: ' || :NEW.BASIC_SALARY,
        :NEW.CREATED_BY, CURRENT_TIMESTAMP
    );
END;
/

-- ==========================================================
-- 5. Trigger لتسجيل تحديث العقد
-- ==========================================================
CREATE OR REPLACE TRIGGER HR_PERSONNEL.TRG_CONTRACT_UPDATE_AUDIT
AFTER UPDATE ON HR_PERSONNEL.CONTRACTS
FOR EACH ROW
DECLARE
    v_changes VARCHAR2(4000);
BEGIN
    v_changes := '';
    
    IF :OLD.BASIC_SALARY != :NEW.BASIC_SALARY THEN
        v_changes := v_changes || 'الراتب: ' || :OLD.BASIC_SALARY || ' → ' || :NEW.BASIC_SALARY || '; ';
    END IF;
    
    IF :OLD.CONTRACT_STATUS != :NEW.CONTRACT_STATUS THEN
        v_changes := v_changes || 'الحالة: ' || :OLD.CONTRACT_STATUS || ' → ' || :NEW.CONTRACT_STATUS || '; ';
    END IF;
    
    IF LENGTH(v_changes) > 0 THEN
        INSERT INTO HR_CORE.AUDIT_LOGS (
            TABLE_NAME, RECORD_ID, ACTION_TYPE,
            OLD_VALUE, NEW_VALUE, PERFORMED_BY, PERFORMED_AT
        ) VALUES (
            'CONTRACTS', :NEW.CONTRACT_ID, 'UPDATE',
            'التغييرات', v_changes,
            :NEW.UPDATED_BY, CURRENT_TIMESTAMP
        );
    END IF;
END;
/

-- ==========================================================
-- 6. Trigger لإنشاء رصيد إجازات تلقائي للموظف الجديد
-- ==========================================================
CREATE OR REPLACE TRIGGER HR_PERSONNEL.TRG_CREATE_LEAVE_BALANCES
AFTER INSERT ON HR_PERSONNEL.EMPLOYEES
FOR EACH ROW
DECLARE
    v_year NUMBER;
BEGIN
    v_year := EXTRACT(YEAR FROM SYSDATE);
    
    -- إنشاء رصيد للإجازة السنوية (نوع 1 = إجازة سنوية)
    INSERT INTO HR_LEAVES.EMPLOYEE_LEAVE_BALANCES (
        EMPLOYEE_ID, LEAVE_TYPE_ID, CURRENT_BALANCE, YEAR,
        CREATED_BY, CREATED_AT
    ) VALUES (
        :NEW.EMPLOYEE_ID, 1, 30, v_year,
        :NEW.CREATED_BY, CURRENT_TIMESTAMP
    );
    
    -- إنشاء رصيد للإجازة المرضية (نوع 2 = إجازة مرضية)
    INSERT INTO HR_LEAVES.EMPLOYEE_LEAVE_BALANCES (
        EMPLOYEE_ID, LEAVE_TYPE_ID, CURRENT_BALANCE, YEAR,
        CREATED_BY, CREATED_AT
    ) VALUES (
        :NEW.EMPLOYEE_ID, 2, 15, v_year,
        :NEW.CREATED_BY, CURRENT_TIMESTAMP
    );
    
EXCEPTION
    WHEN OTHERS THEN
        NULL; -- تجاهل الأخطاء إذا لم تكن أنواع الإجازات موجودة
END;
/

-- ==========================================================
-- 7. Trigger لإنشاء هيكل راتب تلقائي
-- ==========================================================
CREATE OR REPLACE TRIGGER HR_PERSONNEL.TRG_CREATE_SALARY_STRUCTURE
AFTER INSERT ON HR_PERSONNEL.CONTRACTS
FOR EACH ROW
BEGIN
    -- الراتب الأساسي (عنصر 1)
    INSERT INTO HR_PAYROLL.EMPLOYEE_SALARY_STRUCTURE (
        EMPLOYEE_ID, ELEMENT_ID, AMOUNT, IS_ACTIVE,
        CREATED_BY, CREATED_AT
    ) VALUES (
        :NEW.EMPLOYEE_ID, 1, :NEW.BASIC_SALARY, 1,
        :NEW.CREATED_BY, CURRENT_TIMESTAMP
    );
    
    -- بدل السكن (عنصر 2)
    IF :NEW.HOUSING_ALLOWANCE > 0 THEN
        INSERT INTO HR_PAYROLL.EMPLOYEE_SALARY_STRUCTURE (
            EMPLOYEE_ID, ELEMENT_ID, AMOUNT, IS_ACTIVE,
            CREATED_BY, CREATED_AT
        ) VALUES (
            :NEW.EMPLOYEE_ID, 2, :NEW.HOUSING_ALLOWANCE, 1,
            :NEW.CREATED_BY, CURRENT_TIMESTAMP
        );
    END IF;
    
    -- بدل النقل (عنصر 3)
    IF :NEW.TRANSPORT_ALLOWANCE > 0 THEN
        INSERT INTO HR_PAYROLL.EMPLOYEE_SALARY_STRUCTURE (
            EMPLOYEE_ID, ELEMENT_ID, AMOUNT, IS_ACTIVE,
            CREATED_BY, CREATED_AT
        ) VALUES (
            :NEW.EMPLOYEE_ID, 3, :NEW.TRANSPORT_ALLOWANCE, 1,
            :NEW.CREATED_BY, CURRENT_TIMESTAMP
        );
    END IF;
    
    -- التأمينات الاجتماعية (عنصر 10 - خصم)
    INSERT INTO HR_PAYROLL.EMPLOYEE_SALARY_STRUCTURE (
        EMPLOYEE_ID, ELEMENT_ID, PERCENTAGE, IS_ACTIVE,
        CREATED_BY, CREATED_AT
    ) VALUES (
        :NEW.EMPLOYEE_ID, 10, 10, 1,  -- 10% من الراتب الأساسي
        :NEW.CREATED_BY, CURRENT_TIMESTAMP
    );
    
EXCEPTION
    WHEN OTHERS THEN
        NULL; -- تجاهل الأخطاء إذا لم تكن عناصر الراتب موجودة
END;
/

-- ==========================================================
-- 8. Trigger للتحقق من صلاحية تاريخ العقد
-- ==========================================================
CREATE OR REPLACE TRIGGER HR_PERSONNEL.TRG_VALIDATE_CONTRACT_DATES
BEFORE INSERT OR UPDATE ON HR_PERSONNEL.CONTRACTS
FOR EACH ROW
BEGIN
    -- التحقق من أن تاريخ البداية قبل تاريخ النهاية
    IF :NEW.END_DATE IS NOT NULL AND :NEW.START_DATE >= :NEW.END_DATE THEN
        RAISE_APPLICATION_ERROR(-20101, 
            'تاريخ بداية العقد يجب أن يكون قبل تاريخ النهاية');
    END IF;
    
    -- التحقق من أن الراتب موجب
    IF :NEW.BASIC_SALARY <= 0 THEN
        RAISE_APPLICATION_ERROR(-20102, 
            'الراتب الأساسي يجب أن يكون أكبر من صفر');
    END IF;
END;
/

-- ==========================================================
-- 9. Trigger لإرسال إشعار عند انتهاء صلاحية وثيقة
-- ==========================================================
CREATE OR REPLACE TRIGGER HR_PERSONNEL.TRG_DOCUMENT_EXPIRY_ALERT
BEFORE UPDATE ON HR_PERSONNEL.EMPLOYEE_DOCUMENTS
FOR EACH ROW
WHEN (NEW.EXPIRY_DATE IS NOT NULL AND NEW.EXPIRY_DATE - SYSDATE <= 30)
DECLARE
    v_emp_name VARCHAR2(200);
    v_doc_name VARCHAR2(100);
BEGIN
    -- الحصول على اسم الموظف
    SELECT FULL_NAME_EN INTO v_emp_name
    FROM HR_PERSONNEL.EMPLOYEES
    WHERE EMPLOYEE_ID = :NEW.EMPLOYEE_ID;
    
    -- الحصول على نوع الوثيقة
    SELECT DOC_NAME_AR INTO v_doc_name
    FROM HR_CORE.DOCUMENT_TYPES
    WHERE DOC_TYPE_ID = :NEW.DOC_TYPE_ID;
    
    -- إنشاء إشعار
    INSERT INTO HR_CORE.NOTIFICATIONS (
        RECIPIENT_ID, NOTIFICATION_TYPE, TITLE_AR, MESSAGE_AR,
        PRIORITY, REFERENCE_TABLE, REFERENCE_ID,
        CREATED_BY, CREATED_AT
    ) VALUES (
        :NEW.EMPLOYEE_ID, 'DOCUMENT_EXPIRY',
        'تنبيه: انتهاء صلاحية وثيقة',
        'وثيقة ' || v_doc_name || ' ستنتهي صلاحيتها في ' || TO_CHAR(:NEW.EXPIRY_DATE, 'DD/MM/YYYY'),
        'HIGH', 'EMPLOYEE_DOCUMENTS', :NEW.DOC_ID,
        'SYSTEM', CURRENT_TIMESTAMP
    );
    
EXCEPTION
    WHEN OTHERS THEN
        NULL; -- تجاهل الأخطاء
END;
/

-- ==========================================================
-- 10. Trigger لمنع تعديل رقم الموظف
-- ==========================================================
CREATE OR REPLACE TRIGGER HR_PERSONNEL.TRG_PREVENT_EMPNO_CHANGE
BEFORE UPDATE OF EMPLOYEE_NUMBER ON HR_PERSONNEL.EMPLOYEES
FOR EACH ROW
BEGIN
    IF :OLD.EMPLOYEE_NUMBER != :NEW.EMPLOYEE_NUMBER THEN
        RAISE_APPLICATION_ERROR(-20103, 
            'لا يمكن تعديل رقم الموظف بعد إنشائه');
    END IF;
END;
/

PROMPT ========================================
PROMPT تم إنشاء جميع Triggers بنجاح!
PROMPT ========================================
