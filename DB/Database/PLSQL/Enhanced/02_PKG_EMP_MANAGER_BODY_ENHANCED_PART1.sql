-- =================================================================================
-- الحزمة: PKG_EMP_MANAGER (Enhanced Body)
-- الوصف: تنفيذ كامل لإدارة الموظفين - نظام ERP 100%
-- المخطط: HR_PERSONNEL
-- الإصدار: 2.0 - Enhanced
-- =================================================================================

CREATE OR REPLACE PACKAGE BODY HR_PERSONNEL.PKG_EMP_MANAGER AS

    -- ==========================================================
    -- متغيرات عامة (Package Variables)
    -- ==========================================================
    g_min_age CONSTANT NUMBER := 18;
    g_max_age CONSTANT NUMBER := 65;
    
    -- ==========================================================
    -- 1. إضافة موظف جديد (كامل)
    -- ==========================================================
    PROCEDURE CREATE_EMPLOYEE (
        p_first_name_ar         IN VARCHAR2,
        p_second_name_ar        IN VARCHAR2,
        p_third_name_ar         IN VARCHAR2,
        p_last_name_ar          IN VARCHAR2,
        p_full_name_en          IN VARCHAR2,
        p_gender                IN CHAR,
        p_birth_date            IN DATE,
        p_marital_status        IN VARCHAR2,
        p_nationality_id        IN NUMBER,
        p_job_id                IN NUMBER,
        p_dept_id               IN NUMBER,
        p_manager_id            IN NUMBER DEFAULT NULL,
        p_joining_date          IN DATE,
        p_email                 IN VARCHAR2,
        p_mobile                IN VARCHAR2,
        p_contract_type         IN VARCHAR2,
        p_basic_salary          IN NUMBER,
        p_housing_allowance     IN NUMBER DEFAULT 0,
        p_transport_allowance   IN NUMBER DEFAULT 0,
        p_contract_start_date   IN DATE,
        p_contract_end_date     IN DATE DEFAULT NULL,
        p_created_by            IN VARCHAR2,
        o_employee_id           OUT NUMBER,
        o_employee_number       OUT VARCHAR2,
        o_contract_id           OUT NUMBER,
        o_message               OUT VARCHAR2
    ) AS
        v_age NUMBER;
        v_emp_number VARCHAR2(20);
    BEGIN
        -- 1. التحقق من العمر
        v_age := CALCULATE_AGE(p_birth_date);
        IF v_age < g_min_age THEN
            RAISE_APPLICATION_ERROR(-20001, 'عمر الموظف أقل من الحد الأدنى (' || g_min_age || ' سنة)');
        END IF;
        
        IF v_age > g_max_age THEN
            RAISE_APPLICATION_ERROR(-20002, 'عمر الموظف أكبر من الحد الأقصى (' || g_max_age || ' سنة)');
        END IF;
        
        -- 2. التحقق من البريد الإلكتروني
        IF p_email IS NOT NULL AND IS_VALID_EMAIL(p_email) = 0 THEN
            RAISE_APPLICATION_ERROR(-20003, 'البريد الإلكتروني غير صحيح');
        END IF;
        
        -- 3. التحقق من رقم الجوال
        IF p_mobile IS NOT NULL AND IS_VALID_MOBILE(p_mobile) = 0 THEN
            RAISE_APPLICATION_ERROR(-20004, 'رقم الجوال غير صحيح');
        END IF;
        
        -- 4. التحقق من الراتب
        IF p_basic_salary <= 0 THEN
            RAISE_APPLICATION_ERROR(-20005, 'الراتب الأساسي يجب أن يكون أكبر من صفر');
        END IF;
        
        -- 5. توليد رقم وظيفي
        v_emp_number := GENERATE_EMPLOYEE_NUMBER();
        
        -- 6. إدراج الموظف
        INSERT INTO HR_PERSONNEL.EMPLOYEES (
            EMPLOYEE_NUMBER, FIRST_NAME_AR, SECOND_NAME_AR, THIRD_NAME_AR,
            HIJRI_LAST_NAME_AR, FULL_NAME_EN, GENDER, BIRTH_DATE,
            MARITAL_STATUS, NATIONALITY_ID, JOB_ID, DEPT_ID, MANAGER_ID,
            JOINING_DATE, STATUS, EMAIL, MOBILE,
            CREATED_BY, CREATED_AT
        ) VALUES (
            v_emp_number, p_first_name_ar, p_second_name_ar, p_third_name_ar,
            p_last_name_ar, p_full_name_en, p_gender, p_birth_date,
            p_marital_status, p_nationality_id, p_job_id, p_dept_id, p_manager_id,
            p_joining_date, 'ACTIVE', p_email, p_mobile,
            p_created_by, CURRENT_TIMESTAMP
        ) RETURNING EMPLOYEE_ID INTO o_employee_id;
        
        -- 7. إنشاء العقد
        CREATE_CONTRACT(
            p_employee_id => o_employee_id,
            p_contract_type => p_contract_type,
            p_start_date => p_contract_start_date,
            p_end_date => p_contract_end_date,
            p_basic_salary => p_basic_salary,
            p_housing_allowance => p_housing_allowance,
            p_transport_allowance => p_transport_allowance,
            p_other_allowances => 0,
            p_vacation_days => 30,
            p_working_hours_daily => 8,
            p_created_by => p_created_by,
            o_contract_id => o_contract_id,
            o_message => o_message
        );
        
        o_employee_number := v_emp_number;
        o_message := 'تم إضافة الموظف بنجاح - رقم الموظف: ' || v_emp_number;
        
        COMMIT;
        
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END CREATE_EMPLOYEE;
    
    -- ==========================================================
    -- 2. تحديث بيانات الموظف
    -- ==========================================================
    PROCEDURE UPDATE_EMPLOYEE (
        p_employee_id           IN NUMBER,
        p_mobile                IN VARCHAR2 DEFAULT NULL,
        p_email                 IN VARCHAR2 DEFAULT NULL,
        p_marital_status        IN VARCHAR2 DEFAULT NULL,
        p_updated_by            IN VARCHAR2,
        o_message               OUT VARCHAR2
    ) AS
    BEGIN
        -- التحقق من وجود الموظف
        IF EMPLOYEE_EXISTS(p_employee_id) = 0 THEN
            RAISE_APPLICATION_ERROR(-20010, 'الموظف غير موجود');
        END IF;
        
        -- التحقق من البريد الإلكتروني
        IF p_email IS NOT NULL AND IS_VALID_EMAIL(p_email) = 0 THEN
            RAISE_APPLICATION_ERROR(-20011, 'البريد الإلكتروني غير صحيح');
        END IF;
        
        -- التحقق من رقم الجوال
        IF p_mobile IS NOT NULL AND IS_VALID_MOBILE(p_mobile) = 0 THEN
            RAISE_APPLICATION_ERROR(-20012, 'رقم الجوال غير صحيح');
        END IF;
        
        -- التحديث
        UPDATE HR_PERSONNEL.EMPLOYEES
        SET MOBILE = NVL(p_mobile, MOBILE),
            EMAIL = NVL(p_email, EMAIL),
            MARITAL_STATUS = NVL(p_marital_status, MARITAL_STATUS),
            UPDATED_BY = p_updated_by,
            UPDATED_AT = CURRENT_TIMESTAMP,
            VERSION_NO = VERSION_NO + 1
        WHERE EMPLOYEE_ID = p_employee_id;
        
        o_message := 'تم تحديث بيانات الموظف بنجاح';
        COMMIT;
        
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END UPDATE_EMPLOYEE;
    
    -- ==========================================================
    -- 3. نقل موظف لقسم آخر
    -- ==========================================================
    PROCEDURE TRANSFER_EMPLOYEE (
        p_employee_id           IN NUMBER,
        p_new_dept_id           IN NUMBER,
        p_new_job_id            IN NUMBER DEFAULT NULL,
        p_transfer_date         IN DATE,
        p_reason                IN VARCHAR2,
        p_approved_by           IN NUMBER,
        p_created_by            IN VARCHAR2,
        o_transfer_id           OUT NUMBER,
        o_message               OUT VARCHAR2
    ) AS
        v_old_dept_id NUMBER;
        v_old_job_id NUMBER;
    BEGIN
        -- الحصول على القسم والوظيفة الحالية
        SELECT DEPT_ID, JOB_ID
        INTO v_old_dept_id, v_old_job_id
        FROM HR_PERSONNEL.EMPLOYEES
        WHERE EMPLOYEE_ID = p_employee_id;
        
        -- تسجيل النقل
        INSERT INTO HR_PERSONNEL.EMPLOYEE_TRANSFERS (
            EMPLOYEE_ID, FROM_DEPT_ID, TO_DEPT_ID,
            FROM_JOB_ID, TO_JOB_ID, TRANSFER_DATE,
            REASON, APPROVED_BY, STATUS,
            CREATED_BY, CREATED_AT
        ) VALUES (
            p_employee_id, v_old_dept_id, p_new_dept_id,
            v_old_job_id, NVL(p_new_job_id, v_old_job_id), p_transfer_date,
            p_reason, p_approved_by, 'COMPLETED',
            p_created_by, CURRENT_TIMESTAMP
        ) RETURNING TRANSFER_ID INTO o_transfer_id;
        
        -- تحديث بيانات الموظف
        UPDATE HR_PERSONNEL.EMPLOYEES
        SET DEPT_ID = p_new_dept_id,
            JOB_ID = NVL(p_new_job_id, JOB_ID),
            UPDATED_BY = p_created_by,
            UPDATED_AT = CURRENT_TIMESTAMP
        WHERE EMPLOYEE_ID = p_employee_id;
        
        o_message := 'تم نقل الموظف بنجاح';
        COMMIT;
        
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END TRANSFER_EMPLOYEE;
    
    -- ==========================================================
    -- 4. إنهاء خدمة موظف
    -- ==========================================================
    PROCEDURE TERMINATE_EMPLOYEE (
        p_employee_id           IN NUMBER,
        p_termination_date      IN DATE,
        p_reason                IN VARCHAR2,
        p_updated_by            IN VARCHAR2,
        o_eos_amount            OUT NUMBER,
        o_message               OUT VARCHAR2
    ) AS
        v_service_years NUMBER;
        v_basic_salary NUMBER;
    BEGIN
        -- حساب سنوات الخدمة
        v_service_years := CALCULATE_SERVICE_YEARS(p_employee_id, p_termination_date);
        
        -- الحصول على الراتب الأساسي
        v_basic_salary := GET_CURRENT_BASIC_SALARY(p_employee_id);
        
        -- حساب مكافأة نهاية الخدمة (حسب نظام العمل السعودي)
        IF v_service_years < 2 THEN
            o_eos_amount := 0;
        ELSIF v_service_years < 5 THEN
            o_eos_amount := (v_basic_salary / 2) * v_service_years;
        ELSE
            o_eos_amount := (v_basic_salary / 2) * 5 + v_basic_salary * (v_service_years - 5);
        END IF;
        
        -- تسجيل مكافأة نهاية الخدمة
        INSERT INTO HR_PAYROLL.END_OF_SERVICE_CALC (
            EMPLOYEE_ID, TERMINATION_DATE, SERVICE_YEARS,
            LAST_BASIC_SALARY, TOTAL_AMOUNT,
            CALCULATION_NOTES, IS_PAID,
            CREATED_BY, CREATED_AT
        ) VALUES (
            p_employee_id, p_termination_date, v_service_years,
            v_basic_salary, o_eos_amount,
            p_reason, 0,
            p_updated_by, CURRENT_TIMESTAMP
        );
        
        -- تحديث حالة الموظف
        UPDATE HR_PERSONNEL.EMPLOYEES
        SET STATUS = 'TERMINATED',
            UPDATED_BY = p_updated_by,
            UPDATED_AT = CURRENT_TIMESTAMP
        WHERE EMPLOYEE_ID = p_employee_id;
        
        -- إنهاء العقد النشط
        UPDATE HR_PERSONNEL.CONTRACTS
        SET CONTRACT_STATUS = 'TERMINATED',
            UPDATED_BY = p_updated_by,
            UPDATED_AT = CURRENT_TIMESTAMP
        WHERE EMPLOYEE_ID = p_employee_id
          AND CONTRACT_STATUS = 'ACTIVE';
        
        o_message := 'تم إنهاء خدمة الموظف - مكافأة نهاية الخدمة: ' || o_eos_amount;
        COMMIT;
        
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END TERMINATE_EMPLOYEE;
    
    -- ==========================================================
    -- 5. إعادة تفعيل موظف
    -- ==========================================================
    PROCEDURE REACTIVATE_EMPLOYEE (
        p_employee_id           IN NUMBER,
        p_updated_by            IN VARCHAR2,
        o_message               OUT VARCHAR2
    ) AS
    BEGIN
        UPDATE HR_PERSONNEL.EMPLOYEES
        SET STATUS = 'ACTIVE',
            UPDATED_BY = p_updated_by,
            UPDATED_AT = CURRENT_TIMESTAMP
        WHERE EMPLOYEE_ID = p_employee_id;
        
        o_message := 'تم إعادة تفعيل الموظف بنجاح';
        COMMIT;
        
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END REACTIVATE_EMPLOYEE;
    
    -- ==========================================================
    -- 6. إنشاء عقد جديد
    -- ==========================================================
    PROCEDURE CREATE_CONTRACT (
        p_employee_id           IN NUMBER,
        p_contract_type         IN VARCHAR2,
        p_start_date            IN DATE,
        p_end_date              IN DATE DEFAULT NULL,
        p_basic_salary          IN NUMBER,
        p_housing_allowance     IN NUMBER DEFAULT 0,
        p_transport_allowance   IN NUMBER DEFAULT 0,
        p_other_allowances      IN NUMBER DEFAULT 0,
        p_vacation_days         IN NUMBER DEFAULT 30,
        p_working_hours_daily   IN NUMBER DEFAULT 8,
        p_created_by            IN VARCHAR2,
        o_contract_id           OUT NUMBER,
        o_message               OUT VARCHAR2
    ) AS
    BEGIN
        -- إنهاء أي عقد نشط سابق
        UPDATE HR_PERSONNEL.CONTRACTS
        SET CONTRACT_STATUS = 'EXPIRED',
            UPDATED_BY = p_created_by,
            UPDATED_AT = CURRENT_TIMESTAMP
        WHERE EMPLOYEE_ID = p_employee_id
          AND CONTRACT_STATUS = 'ACTIVE';
        
        -- إنشاء العقد الجديد
        INSERT INTO HR_PERSONNEL.CONTRACTS (
            EMPLOYEE_ID, CONTRACT_TYPE, START_DATE, END_DATE,
            IS_RENEWABLE, BASIC_SALARY, HOUSING_ALLOWANCE,
            TRANSPORT_ALLOWANCE, OTHER_ALLOWANCES,
            VACATION_DAYS, WORKING_HOURS_DAILY, CONTRACT_STATUS,
            CREATED_BY, CREATED_AT
        ) VALUES (
            p_employee_id, p_contract_type, p_start_date, p_end_date,
            1, p_basic_salary, p_housing_allowance,
            p_transport_allowance, p_other_allowances,
            p_vacation_days, p_working_hours_daily, 'ACTIVE',
            p_created_by, CURRENT_TIMESTAMP
        ) RETURNING CONTRACT_ID INTO o_contract_id;
        
        o_message := 'تم إنشاء العقد بنجاح';
        COMMIT;
        
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END CREATE_CONTRACT;
    
    -- ==========================================================
    -- 7. تجديد عقد
    -- ==========================================================
    PROCEDURE RENEW_CONTRACT (
        p_contract_id           IN NUMBER,
        p_new_start_date        IN DATE,
        p_new_end_date          IN DATE,
        p_new_basic_salary      IN NUMBER DEFAULT NULL,
        p_notes                 IN VARCHAR2 DEFAULT NULL,
        p_created_by            IN VARCHAR2,
        o_renewal_id            OUT NUMBER,
        o_message               OUT VARCHAR2
    ) AS
        v_old_end_date DATE;
        v_employee_id NUMBER;
        v_old_salary NUMBER;
    BEGIN
        -- الحصول على بيانات العقد القديم
        SELECT END_DATE, EMPLOYEE_ID, BASIC_SALARY
        INTO v_old_end_date, v_employee_id, v_old_salary
        FROM HR_PERSONNEL.CONTRACTS
        WHERE CONTRACT_ID = p_contract_id;
        
        -- تسجيل التجديد
        INSERT INTO HR_PERSONNEL.CONTRACT_RENEWALS (
            CONTRACT_ID, OLD_END_DATE, NEW_START_DATE,
            NEW_END_DATE, RENEWAL_DATE, NOTES,
            CREATED_BY, CREATED_AT
        ) VALUES (
            p_contract_id, v_old_end_date, p_new_start_date,
            p_new_end_date, SYSDATE, p_notes,
            p_created_by, CURRENT_TIMESTAMP
        ) RETURNING RENEWAL_ID INTO o_renewal_id;
        
        -- تحديث العقد
        UPDATE HR_PERSONNEL.CONTRACTS
        SET START_DATE = p_new_start_date,
            END_DATE = p_new_end_date,
            BASIC_SALARY = NVL(p_new_basic_salary, BASIC_SALARY),
            UPDATED_BY = p_created_by,
            UPDATED_AT = CURRENT_TIMESTAMP
        WHERE CONTRACT_ID = p_contract_id;
        
        o_message := 'تم تجديد العقد بنجاح';
        COMMIT;
        
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END RENEW_CONTRACT;
    
    -- ==========================================================
    -- 8. إنهاء عقد
    -- ==========================================================
    PROCEDURE TERMINATE_CONTRACT (
        p_contract_id           IN NUMBER,
        p_updated_by            IN VARCHAR2,
        o_message               OUT VARCHAR2
    ) AS
    BEGIN
        UPDATE HR_PERSONNEL.CONTRACTS
        SET CONTRACT_STATUS = 'TERMINATED',
            UPDATED_BY = p_updated_by,
            UPDATED_AT = CURRENT_TIMESTAMP
        WHERE CONTRACT_ID = p_contract_id;
        
        o_message := 'تم إنهاء العقد بنجاح';
        COMMIT;
        
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END TERMINATE_CONTRACT;
    
    -- ==========================================================
    -- 9. إضافة وثيقة
    -- ==========================================================
    PROCEDURE ADD_DOCUMENT (
        p_employee_id           IN NUMBER,
        p_doc_type_id           IN NUMBER,
        p_doc_number            IN VARCHAR2,
        p_issue_date            IN DATE,
        p_expiry_date           IN DATE DEFAULT NULL,
        p_issue_place           IN VARCHAR2 DEFAULT NULL,
        p_attachment_path       IN VARCHAR2 DEFAULT NULL,
        p_created_by            IN VARCHAR2,
        o_doc_id                OUT NUMBER,
        o_message               OUT VARCHAR2
    ) AS
    BEGIN
        INSERT INTO HR_PERSONNEL.EMPLOYEE_DOCUMENTS (
            EMPLOYEE_ID, DOC_TYPE_ID, DOC_NUMBER,
            ISSUE_DATE, EXPIRY_DATE, ISSUE_PLACE,
            ATTACHMENT_PATH, IS_ACTIVE,
            CREATED_BY, CREATED_AT
        ) VALUES (
            p_employee_id, p_doc_type_id, p_doc_number,
            p_issue_date, p_expiry_date, p_issue_place,
            p_attachment_path, 1,
            p_created_by, CURRENT_TIMESTAMP
        ) RETURNING DOC_ID INTO o_doc_id;
        
        o_message := 'تم إضافة الوثيقة بنجاح';
        COMMIT;
        
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END ADD_DOCUMENT;
    
    -- ==========================================================
    -- 10. تحديث وثيقة
    -- ==========================================================
    PROCEDURE UPDATE_DOCUMENT (
        p_doc_id                IN NUMBER,
        p_expiry_date           IN DATE,
        p_attachment_path       IN VARCHAR2 DEFAULT NULL,
        p_updated_by            IN VARCHAR2,
        o_message               OUT VARCHAR2
    ) AS
    BEGIN
        UPDATE HR_PERSONNEL.EMPLOYEE_DOCUMENTS
        SET EXPIRY_DATE = p_expiry_date,
            ATTACHMENT_PATH = NVL(p_attachment_path, ATTACHMENT_PATH),
            UPDATED_BY = p_updated_by,
            UPDATED_AT = CURRENT_TIMESTAMP
        WHERE DOC_ID = p_doc_id;
        
        o_message := 'تم تحديث الوثيقة بنجاح';
        COMMIT;
        
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END UPDATE_DOCUMENT;
    
    -- ==========================================================
    -- 11. الحصول على الوثائق منتهية الصلاحية
    -- ==========================================================
    PROCEDURE GET_EXPIRING_DOCUMENTS (
        p_days_before           IN NUMBER DEFAULT 30,
        o_cursor                OUT SYS_REFCURSOR
    ) AS
    BEGIN
        OPEN o_cursor FOR
        SELECT 
            ed.DOC_ID,
            ed.EMPLOYEE_ID,
            e.EMPLOYEE_NUMBER,
            e.FULL_NAME_EN,
            dt.DOC_NAME_AR,
            ed.DOC_NUMBER,
            ed.EXPIRY_DATE,
            (ed.EXPIRY_DATE - SYSDATE) AS DAYS_REMAINING
        FROM HR_PERSONNEL.EMPLOYEE_DOCUMENTS ed
        JOIN HR_PERSONNEL.EMPLOYEES e ON ed.EMPLOYEE_ID = e.EMPLOYEE_ID
        JOIN HR_CORE.DOCUMENT_TYPES dt ON ed.DOC_TYPE_ID = dt.DOC_TYPE_ID
        WHERE ed.EXPIRY_DATE IS NOT NULL
          AND ed.EXPIRY_DATE BETWEEN SYSDATE AND SYSDATE + p_days_before
          AND ed.IS_ACTIVE = 1
          AND e.STATUS = 'ACTIVE'
        ORDER BY ed.EXPIRY_DATE;
    END GET_EXPIRING_DOCUMENTS;
    
    -- سأكمل الباقي في الرد التالي...
    
END PKG_EMP_MANAGER;
/
