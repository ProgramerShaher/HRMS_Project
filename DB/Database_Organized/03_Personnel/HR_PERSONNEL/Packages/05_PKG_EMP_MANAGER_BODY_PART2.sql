-- =================================================================================
-- الحزمة: PKG_EMP_MANAGER (Enhanced Body - Part 2)
-- الوصف: الدوال المساعدة والتقارير
-- المخطط: HR_PERSONNEL
-- =================================================================================

CREATE OR REPLACE PACKAGE BODY HR_PERSONNEL.PKG_EMP_MANAGER AS

    -- ... (الجزء الأول موجود في PART1) ...
    
    -- ==========================================================
    -- 12. إضافة مؤهل علمي
    -- ==========================================================
    PROCEDURE ADD_QUALIFICATION (
        p_employee_id           IN NUMBER,
        p_degree_type           IN VARCHAR2,
        p_major_ar              IN VARCHAR2,
        p_university_ar         IN VARCHAR2,
        p_country_id            IN NUMBER,
        p_graduation_year       IN NUMBER,
        p_grade                 IN VARCHAR2 DEFAULT NULL,
        p_attachment_path       IN VARCHAR2 DEFAULT NULL,
        p_created_by            IN VARCHAR2,
        o_qualification_id      OUT NUMBER,
        o_message               OUT VARCHAR2
    ) AS
    BEGIN
        INSERT INTO HR_PERSONNEL.EMPLOYEE_QUALIFICATIONS (
            EMPLOYEE_ID, DEGREE_TYPE, MAJOR_AR, UNIVERSITY_AR,
            COUNTRY_ID, GRADUATION_YEAR, GRADE, ATTACHMENT_PATH,
            CREATED_BY, CREATED_AT
        ) VALUES (
            p_employee_id, p_degree_type, p_major_ar, p_university_ar,
            p_country_id, p_graduation_year, p_grade, p_attachment_path,
            p_created_by, CURRENT_TIMESTAMP
        ) RETURNING QUALIFICATION_ID INTO o_qualification_id;
        
        o_message := 'تم إضافة المؤهل العلمي بنجاح';
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END ADD_QUALIFICATION;
    
    -- ==========================================================
    -- 13. إضافة شهادة مهنية
    -- ==========================================================
    PROCEDURE ADD_CERTIFICATION (
        p_employee_id           IN NUMBER,
        p_cert_name_ar          IN VARCHAR2,
        p_issuing_authority     IN VARCHAR2,
        p_issue_date            IN DATE,
        p_expiry_date           IN DATE DEFAULT NULL,
        p_cert_number           IN VARCHAR2 DEFAULT NULL,
        p_is_mandatory          IN NUMBER DEFAULT 0,
        p_attachment_path       IN VARCHAR2 DEFAULT NULL,
        p_created_by            IN VARCHAR2,
        o_cert_id               OUT NUMBER,
        o_message               OUT VARCHAR2
    ) AS
    BEGIN
        INSERT INTO HR_PERSONNEL.EMPLOYEE_CERTIFICATIONS (
            EMPLOYEE_ID, CERT_NAME_AR, ISSUING_AUTHORITY,
            ISSUE_DATE, EXPIRY_DATE, CERT_NUMBER,
            IS_MANDATORY, ATTACHMENT_PATH,
            CREATED_BY, CREATED_AT
        ) VALUES (
            p_employee_id, p_cert_name_ar, p_issuing_authority,
            p_issue_date, p_expiry_date, p_cert_number,
            p_is_mandatory, p_attachment_path,
            p_created_by, CURRENT_TIMESTAMP
        ) RETURNING CERT_ID INTO o_cert_id;
        
        o_message := 'تم إضافة الشهادة المهنية بنجاح';
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END ADD_CERTIFICATION;
    
    -- ==========================================================
    -- 14. إضافة حساب بنكي
    -- ==========================================================
    PROCEDURE ADD_BANK_ACCOUNT (
        p_employee_id           IN NUMBER,
        p_bank_id               IN NUMBER,
        p_account_number        IN VARCHAR2,
        p_iban                  IN VARCHAR2,
        p_is_primary            IN NUMBER DEFAULT 0,
        p_created_by            IN VARCHAR2,
        o_account_id            OUT NUMBER,
        o_message               OUT VARCHAR2
    ) AS
    BEGIN
        -- إذا كان الحساب رئيسي، إلغاء الحسابات الرئيسية السابقة
        IF p_is_primary = 1 THEN
            UPDATE HR_PERSONNEL.EMPLOYEE_BANK_ACCOUNTS
            SET IS_PRIMARY = 0,
                UPDATED_BY = p_created_by,
                UPDATED_AT = CURRENT_TIMESTAMP
            WHERE EMPLOYEE_ID = p_employee_id;
        END IF;
        
        INSERT INTO HR_PERSONNEL.EMPLOYEE_BANK_ACCOUNTS (
            EMPLOYEE_ID, BANK_ID, ACCOUNT_NUMBER, IBAN,
            IS_PRIMARY, IS_ACTIVE,
            CREATED_BY, CREATED_AT
        ) VALUES (
            p_employee_id, p_bank_id, p_account_number, p_iban,
            p_is_primary, 1,
            p_created_by, CURRENT_TIMESTAMP
        ) RETURNING ACCOUNT_ID INTO o_account_id;
        
        o_message := 'تم إضافة الحساب البنكي بنجاح';
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END ADD_BANK_ACCOUNT;
    
    -- ==========================================================
    -- 15. تحديد الحساب الرئيسي
    -- ==========================================================
    PROCEDURE SET_PRIMARY_ACCOUNT (
        p_account_id            IN NUMBER,
        p_updated_by            IN VARCHAR2,
        o_message               OUT VARCHAR2
    ) AS
        v_employee_id NUMBER;
    BEGIN
        -- الحصول على رقم الموظف
        SELECT EMPLOYEE_ID INTO v_employee_id
        FROM HR_PERSONNEL.EMPLOYEE_BANK_ACCOUNTS
        WHERE ACCOUNT_ID = p_account_id;
        
        -- إلغاء جميع الحسابات الرئيسية
        UPDATE HR_PERSONNEL.EMPLOYEE_BANK_ACCOUNTS
        SET IS_PRIMARY = 0,
            UPDATED_BY = p_updated_by,
            UPDATED_AT = CURRENT_TIMESTAMP
        WHERE EMPLOYEE_ID = v_employee_id;
        
        -- تحديد الحساب الجديد كرئيسي
        UPDATE HR_PERSONNEL.EMPLOYEE_BANK_ACCOUNTS
        SET IS_PRIMARY = 1,
            UPDATED_BY = p_updated_by,
            UPDATED_AT = CURRENT_TIMESTAMP
        WHERE ACCOUNT_ID = p_account_id;
        
        o_message := 'تم تحديد الحساب الرئيسي بنجاح';
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END SET_PRIMARY_ACCOUNT;
    
    -- ==========================================================
    -- 16. إضافة جهة اتصال طوارئ
    -- ==========================================================
    PROCEDURE ADD_EMERGENCY_CONTACT (
        p_employee_id           IN NUMBER,
        p_contact_name_ar       IN VARCHAR2,
        p_relationship          IN VARCHAR2,
        p_phone_primary         IN VARCHAR2,
        p_phone_secondary       IN VARCHAR2 DEFAULT NULL,
        p_is_primary            IN NUMBER DEFAULT 0,
        p_created_by            IN VARCHAR2,
        o_contact_id            OUT NUMBER,
        o_message               OUT VARCHAR2
    ) AS
    BEGIN
        -- إذا كانت جهة اتصال رئيسية، إلغاء الرئيسية السابقة
        IF p_is_primary = 1 THEN
            UPDATE HR_PERSONNEL.EMERGENCY_CONTACTS
            SET IS_PRIMARY = 0,
                UPDATED_BY = p_created_by,
                UPDATED_AT = CURRENT_TIMESTAMP
            WHERE EMPLOYEE_ID = p_employee_id;
        END IF;
        
        INSERT INTO HR_PERSONNEL.EMERGENCY_CONTACTS (
            EMPLOYEE_ID, CONTACT_NAME_AR, RELATIONSHIP,
            PHONE_PRIMARY, PHONE_SECONDARY, IS_PRIMARY,
            CREATED_BY, CREATED_AT
        ) VALUES (
            p_employee_id, p_contact_name_ar, p_relationship,
            p_phone_primary, p_phone_secondary, p_is_primary,
            p_created_by, CURRENT_TIMESTAMP
        ) RETURNING CONTACT_ID INTO o_contact_id;
        
        o_message := 'تم إضافة جهة اتصال الطوارئ بنجاح';
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END ADD_EMERGENCY_CONTACT;
    
    -- ==========================================================
    -- 17. إضافة عنوان
    -- ==========================================================
    PROCEDURE ADD_ADDRESS (
        p_employee_id           IN NUMBER,
        p_address_type          IN VARCHAR2,
        p_city_id               IN NUMBER,
        p_district              IN VARCHAR2,
        p_street                IN VARCHAR2,
        p_building_no           IN VARCHAR2 DEFAULT NULL,
        p_postal_code           IN VARCHAR2 DEFAULT NULL,
        p_created_by            IN VARCHAR2,
        o_address_id            OUT NUMBER,
        o_message               OUT VARCHAR2
    ) AS
    BEGIN
        INSERT INTO HR_PERSONNEL.EMPLOYEE_ADDRESSES (
            EMPLOYEE_ID, ADDRESS_TYPE, CITY_ID,
            DISTRICT, STREET, BUILDING_NO, POSTAL_CODE,
            CREATED_BY, CREATED_AT
        ) VALUES (
            p_employee_id, p_address_type, p_city_id,
            p_district, p_street, p_building_no, p_postal_code,
            p_created_by, CURRENT_TIMESTAMP
        ) RETURNING ADDRESS_ID INTO o_address_id;
        
        o_message := 'تم إضافة العنوان بنجاح';
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END ADD_ADDRESS;
    
    -- ==========================================================
    -- 18. إضافة تابع
    -- ==========================================================
    PROCEDURE ADD_DEPENDENT (
        p_employee_id           IN NUMBER,
        p_name_ar               IN VARCHAR2,
        p_relationship          IN VARCHAR2,
        p_birth_date            IN DATE,
        p_national_id           IN VARCHAR2 DEFAULT NULL,
        p_is_eligible_ticket    IN NUMBER DEFAULT 1,
        p_is_eligible_insurance IN NUMBER DEFAULT 1,
        p_created_by            IN VARCHAR2,
        o_dependent_id          OUT NUMBER,
        o_message               OUT VARCHAR2
    ) AS
    BEGIN
        INSERT INTO HR_PERSONNEL.DEPENDENTS (
            EMPLOYEE_ID, NAME_AR, RELATIONSHIP, BIRTH_DATE,
            NATIONAL_ID, IS_ELIGIBLE_FOR_TICKET, IS_ELIGIBLE_FOR_INSURANCE,
            CREATED_BY, CREATED_AT
        ) VALUES (
            p_employee_id, p_name_ar, p_relationship, p_birth_date,
            p_national_id, p_is_eligible_ticket, p_is_eligible_insurance,
            p_created_by, CURRENT_TIMESTAMP
        ) RETURNING DEPENDENT_ID INTO o_dependent_id;
        
        o_message := 'تم إضافة التابع بنجاح';
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END ADD_DEPENDENT;
    
    -- ==========================================================
    -- 19. توليد رقم وظيفي تلقائي
    -- ==========================================================
    FUNCTION GENERATE_EMPLOYEE_NUMBER RETURN VARCHAR2 AS
        v_year VARCHAR2(4);
        v_seq NUMBER;
        v_emp_number VARCHAR2(20);
    BEGIN
        v_year := TO_CHAR(SYSDATE, 'YYYY');
        
        SELECT NVL(MAX(TO_NUMBER(SUBSTR(EMPLOYEE_NUMBER, 5))), 0) + 1
        INTO v_seq
        FROM HR_PERSONNEL.EMPLOYEES
        WHERE SUBSTR(EMPLOYEE_NUMBER, 1, 4) = v_year;
        
        v_emp_number := v_year || LPAD(v_seq, 4, '0');
        
        RETURN v_emp_number;
    END GENERATE_EMPLOYEE_NUMBER;
    
    -- ==========================================================
    -- 20. حساب العمر
    -- ==========================================================
    FUNCTION CALCULATE_AGE (p_birth_date IN DATE) RETURN NUMBER AS
    BEGIN
        RETURN TRUNC(MONTHS_BETWEEN(SYSDATE, p_birth_date) / 12);
    END CALCULATE_AGE;
    
    -- ==========================================================
    -- 21. حساب سنوات الخدمة
    -- ==========================================================
    FUNCTION CALCULATE_SERVICE_YEARS (
        p_employee_id IN NUMBER,
        p_as_of_date IN DATE DEFAULT SYSDATE
    ) RETURN NUMBER AS
        v_joining_date DATE;
        v_service_years NUMBER;
    BEGIN
        SELECT JOINING_DATE INTO v_joining_date
        FROM HR_PERSONNEL.EMPLOYEES
        WHERE EMPLOYEE_ID = p_employee_id;
        
        v_service_years := MONTHS_BETWEEN(p_as_of_date, v_joining_date) / 12;
        
        RETURN ROUND(v_service_years, 2);
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            RETURN 0;
    END CALCULATE_SERVICE_YEARS;
    
    -- ==========================================================
    -- 22. التحقق من صلاحية البريد الإلكتروني
    -- ==========================================================
    FUNCTION IS_VALID_EMAIL (p_email IN VARCHAR2) RETURN NUMBER AS
    BEGIN
        IF REGEXP_LIKE(p_email, '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}$') THEN
            RETURN 1;
        ELSE
            RETURN 0;
        END IF;
    END IS_VALID_EMAIL;
    
    -- ==========================================================
    -- 23. التحقق من صلاحية رقم الجوال
    -- ==========================================================
    FUNCTION IS_VALID_MOBILE (p_mobile IN VARCHAR2) RETURN NUMBER AS
    BEGIN
        -- التحقق من أن الرقم يبدأ بـ 05 ويتكون من 10 أرقام (سعودي)
        IF REGEXP_LIKE(p_mobile, '^05[0-9]{8}$') THEN
            RETURN 1;
        ELSE
            RETURN 0;
        END IF;
    END IS_VALID_MOBILE;
    
    -- ==========================================================
    -- 24. الحصول على الراتب الإجمالي
    -- ==========================================================
    FUNCTION GET_TOTAL_SALARY (p_contract_id IN NUMBER) RETURN NUMBER AS
        v_total NUMBER;
    BEGIN
        SELECT BASIC_SALARY + HOUSING_ALLOWANCE + TRANSPORT_ALLOWANCE + OTHER_ALLOWANCES
        INTO v_total
        FROM HR_PERSONNEL.CONTRACTS
        WHERE CONTRACT_ID = p_contract_id;
        
        RETURN v_total;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            RETURN 0;
    END GET_TOTAL_SALARY;
    
    -- ==========================================================
    -- 25. الحصول على الراتب الأساسي الحالي
    -- ==========================================================
    FUNCTION GET_CURRENT_BASIC_SALARY (p_employee_id IN NUMBER) RETURN NUMBER AS
        v_salary NUMBER;
    BEGIN
        SELECT BASIC_SALARY
        INTO v_salary
        FROM HR_PERSONNEL.CONTRACTS
        WHERE EMPLOYEE_ID = p_employee_id
          AND CONTRACT_STATUS = 'ACTIVE'
          AND ROWNUM = 1;
        
        RETURN v_salary;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            RETURN 0;
    END GET_CURRENT_BASIC_SALARY;
    
    -- ==========================================================
    -- 26. التحقق من وجود موظف
    -- ==========================================================
    FUNCTION EMPLOYEE_EXISTS (p_employee_id IN NUMBER) RETURN NUMBER AS
        v_count NUMBER;
    BEGIN
        SELECT COUNT(*)
        INTO v_count
        FROM HR_PERSONNEL.EMPLOYEES
        WHERE EMPLOYEE_ID = p_employee_id;
        
        RETURN v_count;
    END EMPLOYEE_EXISTS;
    
    -- ==========================================================
    -- 27. الحصول على حالة الموظف
    -- ==========================================================
    FUNCTION GET_EMPLOYEE_STATUS (p_employee_id IN NUMBER) RETURN VARCHAR2 AS
        v_status VARCHAR2(20);
    BEGIN
        SELECT STATUS
        INTO v_status
        FROM HR_PERSONNEL.EMPLOYEES
        WHERE EMPLOYEE_ID = p_employee_id;
        
        RETURN v_status;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            RETURN NULL;
    END GET_EMPLOYEE_STATUS;
    
    -- ==========================================================
    -- 28. الحصول على قائمة الموظفين النشطين
    -- ==========================================================
    PROCEDURE GET_ACTIVE_EMPLOYEES (
        p_dept_id               IN NUMBER DEFAULT NULL,
        o_cursor                OUT SYS_REFCURSOR
    ) AS
    BEGIN
        IF p_dept_id IS NULL THEN
            OPEN o_cursor FOR
            SELECT 
                e.EMPLOYEE_ID,
                e.EMPLOYEE_NUMBER,
                e.FULL_NAME_EN,
                j.JOB_TITLE_AR,
                d.DEPT_NAME_AR,
                e.EMAIL,
                e.MOBILE,
                e.JOINING_DATE,
                CALCULATE_SERVICE_YEARS(e.EMPLOYEE_ID) AS SERVICE_YEARS
            FROM HR_PERSONNEL.EMPLOYEES e
            JOIN HR_CORE.JOBS j ON e.JOB_ID = j.JOB_ID
            JOIN HR_CORE.DEPARTMENTS d ON e.DEPT_ID = d.DEPT_ID
            WHERE e.STATUS = 'ACTIVE'
            ORDER BY e.EMPLOYEE_NUMBER;
        ELSE
            OPEN o_cursor FOR
            SELECT 
                e.EMPLOYEE_ID,
                e.EMPLOYEE_NUMBER,
                e.FULL_NAME_EN,
                j.JOB_TITLE_AR,
                d.DEPT_NAME_AR,
                e.EMAIL,
                e.MOBILE,
                e.JOINING_DATE,
                CALCULATE_SERVICE_YEARS(e.EMPLOYEE_ID) AS SERVICE_YEARS
            FROM HR_PERSONNEL.EMPLOYEES e
            JOIN HR_CORE.JOBS j ON e.JOB_ID = j.JOB_ID
            JOIN HR_CORE.DEPARTMENTS d ON e.DEPT_ID = d.DEPT_ID
            WHERE e.STATUS = 'ACTIVE'
              AND e.DEPT_ID = p_dept_id
            ORDER BY e.EMPLOYEE_NUMBER;
        END IF;
    END GET_ACTIVE_EMPLOYEES;
    
    -- ==========================================================
    -- 29. الحصول على تفاصيل موظف كاملة
    -- ==========================================================
    PROCEDURE GET_EMPLOYEE_DETAILS (
        p_employee_id           IN NUMBER,
        o_cursor                OUT SYS_REFCURSOR
    ) AS
    BEGIN
        OPEN o_cursor FOR
        SELECT 
            e.*,
            j.JOB_TITLE_AR,
            d.DEPT_NAME_AR,
            c.COUNTRY_NAME_AR AS NATIONALITY,
            mgr.FULL_NAME_EN AS MANAGER_NAME,
            cont.BASIC_SALARY,
            cont.HOUSING_ALLOWANCE,
            cont.TRANSPORT_ALLOWANCE,
            GET_TOTAL_SALARY(cont.CONTRACT_ID) AS TOTAL_SALARY
        FROM HR_PERSONNEL.EMPLOYEES e
        LEFT JOIN HR_CORE.JOBS j ON e.JOB_ID = j.JOB_ID
        LEFT JOIN HR_CORE.DEPARTMENTS d ON e.DEPT_ID = d.DEPT_ID
        LEFT JOIN HR_CORE.COUNTRIES c ON e.NATIONALITY_ID = c.COUNTRY_ID
        LEFT JOIN HR_PERSONNEL.EMPLOYEES mgr ON e.MANAGER_ID = mgr.EMPLOYEE_ID
        LEFT JOIN HR_PERSONNEL.CONTRACTS cont ON e.EMPLOYEE_ID = cont.EMPLOYEE_ID 
            AND cont.CONTRACT_STATUS = 'ACTIVE'
        WHERE e.EMPLOYEE_ID = p_employee_id;
    END GET_EMPLOYEE_DETAILS;
    
    -- ==========================================================
    -- 30. الحصول على العقود المنتهية قريباً
    -- ==========================================================
    PROCEDURE GET_EXPIRING_CONTRACTS (
        p_days_before           IN NUMBER DEFAULT 60,
        o_cursor                OUT SYS_REFCURSOR
    ) AS
    BEGIN
        OPEN o_cursor FOR
        SELECT 
            c.CONTRACT_ID,
            e.EMPLOYEE_NUMBER,
            e.FULL_NAME_EN,
            c.CONTRACT_TYPE,
            c.START_DATE,
            c.END_DATE,
            (c.END_DATE - SYSDATE) AS DAYS_REMAINING
        FROM HR_PERSONNEL.CONTRACTS c
        JOIN HR_PERSONNEL.EMPLOYEES e ON c.EMPLOYEE_ID = e.EMPLOYEE_ID
        WHERE c.CONTRACT_STATUS = 'ACTIVE'
          AND c.END_DATE IS NOT NULL
          AND c.END_DATE BETWEEN SYSDATE AND SYSDATE + p_days_before
        ORDER BY c.END_DATE;
    END GET_EXPIRING_CONTRACTS;
    
    -- ==========================================================
    -- 31. إحصائيات الموظفين
    -- ==========================================================
    PROCEDURE GET_EMPLOYEE_STATISTICS (
        o_total_employees       OUT NUMBER,
        o_active_employees      OUT NUMBER,
        o_on_leave              OUT NUMBER,
        o_terminated            OUT NUMBER
    ) AS
    BEGIN
        SELECT COUNT(*) INTO o_total_employees
        FROM HR_PERSONNEL.EMPLOYEES;
        
        SELECT COUNT(*) INTO o_active_employees
        FROM HR_PERSONNEL.EMPLOYEES
        WHERE STATUS = 'ACTIVE';
        
        SELECT COUNT(*) INTO o_on_leave
        FROM HR_PERSONNEL.EMPLOYEES
        WHERE STATUS = 'ON_LEAVE';
        
        SELECT COUNT(*) INTO o_terminated
        FROM HR_PERSONNEL.EMPLOYEES
        WHERE STATUS = 'TERMINATED';
    END GET_EMPLOYEE_STATISTICS;

END PKG_EMP_MANAGER;
/
