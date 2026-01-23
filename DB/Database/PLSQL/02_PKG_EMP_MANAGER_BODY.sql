-- =================================================================================
-- الحزمة: PKG_EMP_MANAGER
-- الوصف: تنفيذ المنطق (Implementation Body)
-- المخطط: HR_PERSONNEL
-- =================================================================================

CREATE OR REPLACE PACKAGE BODY HR_PERSONNEL.PKG_EMP_MANAGER AS

    -- دالة مساعدة خاصة (Private) لتوليد الرقم الوظيفي التالي
    FUNCTION GENERATE_EMP_NUMBER RETURN VARCHAR2 IS
        v_next_seq NUMBER;
        v_year VARCHAR2(4);
    BEGIN
        v_year := TO_CHAR(SYSDATE, 'YYYY');
        -- نفترض وجود sequence، أو نأخذ الماكس (للبساطة هنا نأخذ الماكس)
        SELECT NVL(MAX(TO_NUMBER(SUBSTR(EMPLOYEE_NUMBER, 5))), 0) + 1 
        INTO v_next_seq 
        FROM HR_PERSONNEL.EMPLOYEES 
        WHERE EMPLOYEE_NUMBER LIKE v_year || '%';
        
        RETURN v_year || LPAD(v_next_seq, 4, '0'); -- مثال: 20250001
    END;

    -- ==========================================================
    -- تنفيذ: إضافة موظف جديد
    -- ==========================================================
    PROCEDURE CREATE_NEW_EMPLOYEE (
        p_first_name_ar      IN VARCHAR2,
        p_family_name_ar     IN VARCHAR2,
        p_full_name_en       IN VARCHAR2,
        p_national_id        IN VARCHAR2,
        p_nationality_id     IN NUMBER,
        p_birth_date         IN DATE,
        p_gender             IN VARCHAR2,
        p_job_id            IN NUMBER,
        p_dept_id           IN NUMBER,
        p_basic_salary      IN NUMBER,
        p_joining_date      IN DATE,
        o_employee_id       OUT NUMBER,
        o_employee_number   OUT VARCHAR2
    ) IS
        v_emp_id NUMBER;
        v_emp_num VARCHAR2(20);
        v_exists NUMBER;
    BEGIN
        -- 1. التحقق من القواعد (Validation)
        -- التحقق من العمر (لا يقل عن 18)
        IF MONTHS_BETWEEN(SYSDATE, p_birth_date) / 12 < 18 THEN
            RAISE_APPLICATION_ERROR(-20001, 'عذراً، عمر الموظف يجب أن يكون أكبر من 18 سنة.');
        END IF;

        -- 2. توليد الرقم الوظيفي
        v_emp_num := GENERATE_EMP_NUMBER();

        -- 3. إدخال الموظف (Insert Employee)
        -- ملاحظة: CREATED_BY و CREATED_AT سيتم تعبئتهما تلقائياً من الـ Trigger
        INSERT INTO HR_PERSONNEL.EMPLOYEES (
            EMPLOYEE_NUMBER, FIRST_NAME_AR, HIJRI_LAST_NAME_AR, FULL_NAME_EN, 
            NATIONALITY_ID, BIRTH_DATE, GENDER, JOB_ID, DEPT_ID, JOINING_DATE
        ) VALUES (
            v_emp_num, p_first_name_ar, p_family_name_ar, p_full_name_en,
            p_nationality_id, p_birth_date, p_gender, p_job_id, p_dept_id, p_joining_date
        ) RETURNING EMPLOYEE_ID INTO v_emp_id;

        -- 4. إدخال العقد الافتراضي (Insert Contract)
        -- ملاحظة: CREATED_BY سيتم تعبئته تلقائياً من الـ Trigger
        INSERT INTO HR_PERSONNEL.CONTRACTS (
            EMPLOYEE_ID, CONTRACT_TYPE, START_DATE, BASIC_SALARY, 
            HOUSING_ALLOWANCE, TRANSPORT_ALLOWANCE
        ) VALUES (
            v_emp_id, 'FULL_TIME', p_joining_date, p_basic_salary,
            (p_basic_salary * 0.25), -- قاعدة عمل: بدل السكن 25% افتراضيا
            500 -- قاعدة عمل: المواصلات 500 ثابتة افتراضيا
        );

        -- 5. إرجاع القيم
        o_employee_id := v_emp_id;
        o_employee_number := v_emp_num;
        
        -- الحفظ النهائي (Commit) يتم عادة من التطبيق، لكن يمكن وضعه هنا
        -- COMMIT; 
    EXCEPTION
        WHEN OTHERS THEN
            -- في حال حدوث خطأ، التراجع عن كل شيء
            ROLLBACK;
            RAISE; -- إعادة إرسال الخطأ للتطبيق ليظهر للمستخدم
    END CREATE_NEW_EMPLOYEE;

    -- ==========================================================
    -- تنفيذ: تحديث البيانات
    -- ==========================================================
    PROCEDURE UPDATE_EMPLOYEE_INFO (
        p_employee_id       IN NUMBER,
        p_mobile            IN VARCHAR2,
        p_email             IN VARCHAR2,
        p_marital_status    IN VARCHAR2
    ) IS
    BEGIN
        -- ملاحظة: UPDATED_BY, UPDATED_AT, VERSION_NO سيتم تعبئتها تلقائياً من الـ Trigger
        UPDATE HR_PERSONNEL.EMPLOYEES
        SET MOBILE = p_mobile,
            EMAIL = p_email,
            MARITAL_STATUS = p_marital_status
        WHERE EMPLOYEE_ID = p_employee_id;
        
        IF SQL%ROWCOUNT = 0 THEN
            RAISE_APPLICATION_ERROR(-20002, 'الموظف غير موجود');
        END IF;
    END;

    -- ==========================================================
    -- تنفيذ: حساب الراتب
    -- ==========================================================
    FUNCTION GET_TOTAL_SALARY (p_contract_id IN NUMBER) RETURN NUMBER IS
        v_total NUMBER := 0;
    BEGIN
        SELECT NVL(BASIC_SALARY,0) + NVL(HOUSING_ALLOWANCE,0) + NVL(TRANSPORT_ALLOWANCE,0) + NVL(OTHER_ALLOWANCES,0)
        INTO v_total
        FROM HR_PERSONNEL.CONTRACTS
        WHERE CONTRACT_ID = p_contract_id;
        
        RETURN v_total;
    END;

END PKG_EMP_MANAGER;
/
