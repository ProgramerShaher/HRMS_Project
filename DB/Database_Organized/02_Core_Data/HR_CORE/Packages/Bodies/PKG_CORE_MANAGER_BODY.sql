-- =================================================================================
-- Package: PKG_CORE_MANAGER (Body) - Complete
-- Description: Full implementation with all helper functions and procedures
-- Schema: HR_CORE
-- =================================================================================

CREATE OR REPLACE PACKAGE BODY HR_CORE.PKG_CORE_MANAGER AS

    -- ==========================================================
    
    -- HELPER FUNCTIONS (دوال مساعدة)
    -- ==========================================================
    
    FUNCTION DEPARTMENT_EXISTS(p_dept_id IN NUMBER) RETURN BOOLEAN IS
        v_count NUMBER;
    BEGIN
        SELECT COUNT(*) INTO v_count
        FROM HR_CORE.DEPARTMENTS
        WHERE DEPT_ID = p_dept_id AND IS_DELETED = 0;
        RETURN v_count > 0;
    END;
    
    FUNCTION JOB_EXISTS(p_job_id IN NUMBER) RETURN BOOLEAN IS
        v_count NUMBER;
    BEGIN
        SELECT COUNT(*) INTO v_count
        FROM HR_CORE.JOBS
        WHERE JOB_ID = p_job_id AND IS_DELETED = 0;
        RETURN v_count > 0;
    END;
    
    FUNCTION BRANCH_EXISTS(p_branch_id IN NUMBER) RETURN BOOLEAN IS
        v_count NUMBER;
    BEGIN
        SELECT COUNT(*) INTO v_count
        FROM HR_CORE.BRANCHES
        WHERE BRANCH_ID = p_branch_id AND IS_DELETED = 0;
        RETURN v_count > 0;
    END;
    
    FUNCTION CITY_EXISTS(p_city_id IN NUMBER) RETURN BOOLEAN IS
        v_count NUMBER;
    BEGIN
        SELECT COUNT(*) INTO v_count
        FROM HR_CORE.CITIES
        WHERE CITY_ID = p_city_id AND IS_DELETED = 0;
        RETURN v_count > 0;
    END;
    
    FUNCTION COUNTRY_EXISTS(p_country_id IN NUMBER) RETURN BOOLEAN IS
        v_count NUMBER;
    BEGIN
        SELECT COUNT(*) INTO v_count
        FROM HR_CORE.COUNTRIES
        WHERE COUNTRY_ID = p_country_id AND IS_DELETED = 0;
        RETURN v_count > 0;
    END;
    
    FUNCTION BANK_EXISTS(p_bank_id IN NUMBER) RETURN BOOLEAN IS
        v_count NUMBER;
    BEGIN
        SELECT COUNT(*) INTO v_count
        FROM HR_CORE.BANKS
        WHERE BANK_ID = p_bank_id AND IS_DELETED = 0;
        RETURN v_count > 0;
    END;
    
    FUNCTION DEPT_NAME_EXISTS(p_dept_name_ar IN VARCHAR2, p_exclude_id IN NUMBER DEFAULT NULL) RETURN BOOLEAN IS
        v_count NUMBER;
    BEGIN
        SELECT COUNT(*) INTO v_count
        FROM HR_CORE.DEPARTMENTS
        WHERE UPPER(DEPT_NAME_AR) = UPPER(p_dept_name_ar)
          AND IS_DELETED = 0
          AND (p_exclude_id IS NULL OR DEPT_ID != p_exclude_id);
        RETURN v_count > 0;
    END;
    
    FUNCTION JOB_TITLE_EXISTS(p_job_title_ar IN VARCHAR2, p_exclude_id IN NUMBER DEFAULT NULL) RETURN BOOLEAN IS
        v_count NUMBER;
    BEGIN
        SELECT COUNT(*) INTO v_count
        FROM HR_CORE.JOBS
        WHERE UPPER(JOB_TITLE_AR) = UPPER(p_job_title_ar)
          AND IS_DELETED = 0
          AND (p_exclude_id IS NULL OR JOB_ID != p_exclude_id);
        RETURN v_count > 0;
    END;
    
    -- ==========================================================
    -- DEPARTMENTS MANAGEMENT
    -- ==========================================================
    
    PROCEDURE ADD_DEPARTMENT(
        p_dept_name_ar      IN VARCHAR2,
        p_dept_name_en      IN VARCHAR2 DEFAULT NULL,
        p_branch_id         IN NUMBER,
        p_parent_dept_id    IN NUMBER DEFAULT NULL,
        p_cost_center_code  IN VARCHAR2 DEFAULT NULL,
        p_dept_id           OUT NUMBER
    ) IS
    BEGIN
        IF p_dept_name_ar IS NULL THEN
            RAISE_APPLICATION_ERROR(-20001, 'اسم القسم (عربي) مطلوب');
        END IF;
        
        IF NOT BRANCH_EXISTS(p_branch_id) THEN
            RAISE_APPLICATION_ERROR(-20002, 'الفرع المحدد غير موجود');
        END IF;
        
        IF p_parent_dept_id IS NOT NULL AND NOT DEPARTMENT_EXISTS(p_parent_dept_id) THEN
            RAISE_APPLICATION_ERROR(-20003, 'القسم الأب المحدد غير موجود');
        END IF;
        
        IF DEPT_NAME_EXISTS(p_dept_name_ar) THEN
            RAISE_APPLICATION_ERROR(-20004, 'اسم القسم موجود مسبقاً');
        END IF;
        
        INSERT INTO HR_CORE.DEPARTMENTS (
            DEPT_NAME_AR, DEPT_NAME_EN, BRANCH_ID, 
            PARENT_DEPT_ID, COST_CENTER_CODE, IS_ACTIVE
        ) VALUES (
            p_dept_name_ar, p_dept_name_en, p_branch_id,
            p_parent_dept_id, p_cost_center_code, 1
        ) RETURNING DEPT_ID INTO p_dept_id;
        
        COMMIT;
    END;
    
    PROCEDURE UPDATE_DEPARTMENT(
        p_dept_id           IN NUMBER,
        p_dept_name_ar      IN VARCHAR2 DEFAULT NULL,
        p_dept_name_en      IN VARCHAR2 DEFAULT NULL,
        p_parent_dept_id    IN NUMBER DEFAULT NULL,
        p_cost_center_code  IN VARCHAR2 DEFAULT NULL,
        p_is_active         IN NUMBER DEFAULT NULL
    ) IS
    BEGIN
        IF NOT DEPARTMENT_EXISTS(p_dept_id) THEN
            RAISE_APPLICATION_ERROR(-20005, 'القسم المحدد غير موجود');
        END IF;
        
        IF p_dept_name_ar IS NOT NULL AND DEPT_NAME_EXISTS(p_dept_name_ar, p_dept_id) THEN
            RAISE_APPLICATION_ERROR(-20006, 'اسم القسم موجود مسبقاً');
        END IF;
        
        UPDATE HR_CORE.DEPARTMENTS
        SET DEPT_NAME_AR = NVL(p_dept_name_ar, DEPT_NAME_AR),
            DEPT_NAME_EN = NVL(p_dept_name_en, DEPT_NAME_EN),
            PARENT_DEPT_ID = NVL(p_parent_dept_id, PARENT_DEPT_ID),
            COST_CENTER_CODE = NVL(p_cost_center_code, COST_CENTER_CODE),
            IS_ACTIVE = NVL(p_is_active, IS_ACTIVE)
        WHERE DEPT_ID = p_dept_id;
        
        COMMIT;
    END;
    
    PROCEDURE DELETE_DEPARTMENT(p_dept_id IN NUMBER) IS
        v_has_children NUMBER;
    BEGIN
        IF NOT DEPARTMENT_EXISTS(p_dept_id) THEN
            RAISE_APPLICATION_ERROR(-20007, 'القسم المحدد غير موجود');
        END IF;
        
        SELECT COUNT(*) INTO v_has_children
        FROM HR_CORE.DEPARTMENTS
        WHERE PARENT_DEPT_ID = p_dept_id AND IS_DELETED = 0;
        
        IF v_has_children > 0 THEN
            RAISE_APPLICATION_ERROR(-20008, 'لا يمكن حذف قسم له أقسام فرعية');
        END IF;
        
        UPDATE HR_CORE.DEPARTMENTS
        SET IS_DELETED = 1
        WHERE DEPT_ID = p_dept_id;
        
        COMMIT;
    END;
    
    FUNCTION GET_DEPARTMENT_NAME(p_dept_id IN NUMBER) RETURN VARCHAR2 IS
        v_name VARCHAR2(100);
    BEGIN
        SELECT DEPT_NAME_AR INTO v_name
        FROM HR_CORE.DEPARTMENTS
        WHERE DEPT_ID = p_dept_id AND IS_DELETED = 0;
        RETURN v_name;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            RETURN NULL;
    END;
    
    -- ==========================================================
    -- JOBS MANAGEMENT
    -- ==========================================================
    
    PROCEDURE ADD_JOB(
        p_job_title_ar      IN VARCHAR2,
        p_job_title_en      IN VARCHAR2 DEFAULT NULL,
        p_default_grade_id  IN NUMBER DEFAULT NULL,
        p_is_medical        IN NUMBER DEFAULT 0,
        p_job_id            OUT NUMBER
    ) IS
    BEGIN
        IF p_job_title_ar IS NULL THEN
            RAISE_APPLICATION_ERROR(-20010, 'المسمى الوظيفي (عربي) مطلوب');
        END IF;
        
        IF JOB_TITLE_EXISTS(p_job_title_ar) THEN
            RAISE_APPLICATION_ERROR(-20011, 'المسمى الوظيفي موجود مسبقاً');
        END IF;
        
        INSERT INTO HR_CORE.JOBS (
            JOB_TITLE_AR, JOB_TITLE_EN, 
            DEFAULT_GRADE_ID, IS_MEDICAL
        ) VALUES (
            p_job_title_ar, p_job_title_en,
            p_default_grade_id, p_is_medical
        ) RETURNING JOB_ID INTO p_job_id;
        
        COMMIT;
    END;
    
    PROCEDURE UPDATE_JOB(
        p_job_id            IN NUMBER,
        p_job_title_ar      IN VARCHAR2 DEFAULT NULL,
        p_job_title_en      IN VARCHAR2 DEFAULT NULL,
        p_default_grade_id  IN NUMBER DEFAULT NULL,
        p_is_medical        IN NUMBER DEFAULT NULL
    ) IS
    BEGIN
        IF NOT JOB_EXISTS(p_job_id) THEN
            RAISE_APPLICATION_ERROR(-20012, 'الوظيفة المحددة غير موجودة');
        END IF;
        
        IF p_job_title_ar IS NOT NULL AND JOB_TITLE_EXISTS(p_job_title_ar, p_job_id) THEN
            RAISE_APPLICATION_ERROR(-20013, 'المسمى الوظيفي موجود مسبقاً');
        END IF;
        
        UPDATE HR_CORE.JOBS
        SET JOB_TITLE_AR = NVL(p_job_title_ar, JOB_TITLE_AR),
            JOB_TITLE_EN = NVL(p_job_title_en, JOB_TITLE_EN),
            DEFAULT_GRADE_ID = NVL(p_default_grade_id, DEFAULT_GRADE_ID),
            IS_MEDICAL = NVL(p_is_medical, IS_MEDICAL)
        WHERE JOB_ID = p_job_id;
        
        COMMIT;
    END;
    
    PROCEDURE DELETE_JOB(p_job_id IN NUMBER) IS
    BEGIN
        IF NOT JOB_EXISTS(p_job_id) THEN
            RAISE_APPLICATION_ERROR(-20014, 'الوظيفة المحددة غير موجودة');
        END IF;
        
        UPDATE HR_CORE.JOBS
        SET IS_DELETED = 1
        WHERE JOB_ID = p_job_id;
        
        COMMIT;
    END;
    
    -- ==========================================================
    -- BRANCHES MANAGEMENT
    -- ==========================================================
    
    PROCEDURE ADD_BRANCH(
        p_branch_name_ar    IN VARCHAR2,
        p_branch_name_en    IN VARCHAR2 DEFAULT NULL,
        p_city_id           IN NUMBER,
        p_address           IN VARCHAR2 DEFAULT NULL,
        p_branch_id         OUT NUMBER
    ) IS
    BEGIN
        IF p_branch_name_ar IS NULL THEN
            RAISE_APPLICATION_ERROR(-20020, 'اسم الفرع (عربي) مطلوب');
        END IF;
        
        IF NOT CITY_EXISTS(p_city_id) THEN
            RAISE_APPLICATION_ERROR(-20021, 'المدينة المحددة غير موجودة');
        END IF;
        
        INSERT INTO HR_CORE.BRANCHES (
            BRANCH_NAME_AR, BRANCH_NAME_EN, CITY_ID, ADDRESS
        ) VALUES (
            p_branch_name_ar, p_branch_name_en, p_city_id, p_address
        ) RETURNING BRANCH_ID INTO p_branch_id;
        
        COMMIT;
    END;
    
    PROCEDURE UPDATE_BRANCH(
        p_branch_id         IN NUMBER,
        p_branch_name_ar    IN VARCHAR2 DEFAULT NULL,
        p_branch_name_en    IN VARCHAR2 DEFAULT NULL,
        p_city_id           IN NUMBER DEFAULT NULL,
        p_address           IN VARCHAR2 DEFAULT NULL
    ) IS
    BEGIN
        IF NOT BRANCH_EXISTS(p_branch_id) THEN
            RAISE_APPLICATION_ERROR(-20022, 'الفرع المحدد غير موجود');
        END IF;
        
        UPDATE HR_CORE.BRANCHES
        SET BRANCH_NAME_AR = NVL(p_branch_name_ar, BRANCH_NAME_AR),
            BRANCH_NAME_EN = NVL(p_branch_name_en, BRANCH_NAME_EN),
            CITY_ID = NVL(p_city_id, CITY_ID),
            ADDRESS = NVL(p_address, ADDRESS)
        WHERE BRANCH_ID = p_branch_id;
        
        COMMIT;
    END;
    
    PROCEDURE DELETE_BRANCH(p_branch_id IN NUMBER) IS
        v_has_departments NUMBER;
    BEGIN
        IF NOT BRANCH_EXISTS(p_branch_id) THEN
            RAISE_APPLICATION_ERROR(-20023, 'الفرع المحدد غير موجود');
        END IF;
        
        SELECT COUNT(*) INTO v_has_departments
        FROM HR_CORE.DEPARTMENTS
        WHERE BRANCH_ID = p_branch_id AND IS_DELETED = 0;
        
        IF v_has_departments > 0 THEN
            RAISE_APPLICATION_ERROR(-20024, 'لا يمكن حذف فرع له أقسام');
        END IF;
        
        UPDATE HR_CORE.BRANCHES
        SET IS_DELETED = 1
        WHERE BRANCH_ID = p_branch_id;
        
        COMMIT;
    END;
    
    -- ==========================================================
    -- CITIES MANAGEMENT
    -- ==========================================================
    
    PROCEDURE ADD_CITY(
        p_city_name_ar      IN VARCHAR2,
        p_city_name_en      IN VARCHAR2 DEFAULT NULL,
        p_country_id        IN NUMBER,
        p_city_id           OUT NUMBER
    ) IS
    BEGIN
        IF p_city_name_ar IS NULL THEN
            RAISE_APPLICATION_ERROR(-20030, 'اسم المدينة (عربي) مطلوب');
        END IF;
        
        IF NOT COUNTRY_EXISTS(p_country_id) THEN
            RAISE_APPLICATION_ERROR(-20031, 'الدولة المحددة غير موجودة');
        END IF;
        
        INSERT INTO HR_CORE.CITIES (
            CITY_NAME_AR, CITY_NAME_EN, COUNTRY_ID
        ) VALUES (
            p_city_name_ar, p_city_name_en, p_country_id
        ) RETURNING CITY_ID INTO p_city_id;
        
        COMMIT;
    END;
    
    PROCEDURE UPDATE_CITY(
        p_city_id           IN NUMBER,
        p_city_name_ar      IN VARCHAR2 DEFAULT NULL,
        p_city_name_en      IN VARCHAR2 DEFAULT NULL
    ) IS
    BEGIN
        IF NOT CITY_EXISTS(p_city_id) THEN
            RAISE_APPLICATION_ERROR(-20032, 'المدينة المحددة غير موجودة');
        END IF;
        
        UPDATE HR_CORE.CITIES
        SET CITY_NAME_AR = NVL(p_city_name_ar, CITY_NAME_AR),
            CITY_NAME_EN = NVL(p_city_name_en, CITY_NAME_EN)
        WHERE CITY_ID = p_city_id;
        
        COMMIT;
    END;
    
    PROCEDURE DELETE_CITY(p_city_id IN NUMBER) IS
        v_has_branches NUMBER;
    BEGIN
        IF NOT CITY_EXISTS(p_city_id) THEN
            RAISE_APPLICATION_ERROR(-20033, 'المدينة المحددة غير موجودة');
        END IF;
        
        SELECT COUNT(*) INTO v_has_branches
        FROM HR_CORE.BRANCHES
        WHERE CITY_ID = p_city_id AND IS_DELETED = 0;
        
        IF v_has_branches > 0 THEN
            RAISE_APPLICATION_ERROR(-20034, 'لا يمكن حذف مدينة لها فروع');
        END IF;
        
        UPDATE HR_CORE.CITIES
        SET IS_DELETED = 1
        WHERE CITY_ID = p_city_id;
        
        COMMIT;
    END;
    
    -- ==========================================================
    -- COUNTRIES MANAGEMENT
    -- ==========================================================
    
    PROCEDURE ADD_COUNTRY(
        p_country_name_ar       IN VARCHAR2,
        p_country_name_en       IN VARCHAR2,
        p_citizenship_name_ar   IN VARCHAR2 DEFAULT NULL,
        p_iso_code              IN CHAR DEFAULT NULL,
        p_country_id            OUT NUMBER
    ) IS
    BEGIN
        IF p_country_name_ar IS NULL OR p_country_name_en IS NULL THEN
            RAISE_APPLICATION_ERROR(-20040, 'اسم الدولة (عربي وإنجليزي) مطلوب');
        END IF;
        
        INSERT INTO HR_CORE.COUNTRIES (
            COUNTRY_NAME_AR, COUNTRY_NAME_EN, 
            CITIZENSHIP_NAME_AR, ISO_CODE
        ) VALUES (
            p_country_name_ar, p_country_name_en,
            p_citizenship_name_ar, p_iso_code
        ) RETURNING COUNTRY_ID INTO p_country_id;
        
        COMMIT;
    END;
    
    PROCEDURE UPDATE_COUNTRY(
        p_country_id            IN NUMBER,
        p_country_name_ar       IN VARCHAR2 DEFAULT NULL,
        p_country_name_en       IN VARCHAR2 DEFAULT NULL,
        p_citizenship_name_ar   IN VARCHAR2 DEFAULT NULL,
        p_iso_code              IN CHAR DEFAULT NULL
    ) IS
    BEGIN
        IF NOT COUNTRY_EXISTS(p_country_id) THEN
            RAISE_APPLICATION_ERROR(-20041, 'الدولة المحددة غير موجودة');
        END IF;
        
        UPDATE HR_CORE.COUNTRIES
        SET COUNTRY_NAME_AR = NVL(p_country_name_ar, COUNTRY_NAME_AR),
            COUNTRY_NAME_EN = NVL(p_country_name_en, COUNTRY_NAME_EN),
            CITIZENSHIP_NAME_AR = NVL(p_citizenship_name_ar, CITIZENSHIP_NAME_AR),
            ISO_CODE = NVL(p_iso_code, ISO_CODE)
        WHERE COUNTRY_ID = p_country_id;
        
        COMMIT;
    END;
    
    PROCEDURE DELETE_COUNTRY(p_country_id IN NUMBER) IS
        v_has_cities NUMBER;
    BEGIN
        IF NOT COUNTRY_EXISTS(p_country_id) THEN
            RAISE_APPLICATION_ERROR(-20042, 'الدولة المحددة غير موجودة');
        END IF;
        
        SELECT COUNT(*) INTO v_has_cities
        FROM HR_CORE.CITIES
        WHERE COUNTRY_ID = p_country_id AND IS_DELETED = 0;
        
        IF v_has_cities > 0 THEN
            RAISE_APPLICATION_ERROR(-20043, 'لا يمكن حذف دولة لها مدن');
        END IF;
        
        UPDATE HR_CORE.COUNTRIES
        SET IS_DELETED = 1
        WHERE COUNTRY_ID = p_country_id;
        
        COMMIT;
    END;
    
    -- ==========================================================
    -- BANKS MANAGEMENT
    -- ==========================================================
    
    PROCEDURE ADD_BANK(
        p_bank_name_ar      IN VARCHAR2,
        p_bank_name_en      IN VARCHAR2 DEFAULT NULL,
        p_bank_code         IN VARCHAR2 DEFAULT NULL,
        p_bank_id           OUT NUMBER
    ) IS
    BEGIN
        IF p_bank_name_ar IS NULL THEN
            RAISE_APPLICATION_ERROR(-20050, 'اسم البنك (عربي) مطلوب');
        END IF;
        
        INSERT INTO HR_CORE.BANKS (
            BANK_NAME_AR, BANK_NAME_EN, BANK_CODE
        ) VALUES (
            p_bank_name_ar, p_bank_name_en, p_bank_code
        ) RETURNING BANK_ID INTO p_bank_id;
        
        COMMIT;
    END;
    
    PROCEDURE UPDATE_BANK(
        p_bank_id           IN NUMBER,
        p_bank_name_ar      IN VARCHAR2 DEFAULT NULL,
        p_bank_name_en      IN VARCHAR2 DEFAULT NULL,
        p_bank_code         IN VARCHAR2 DEFAULT NULL
    ) IS
    BEGIN
        IF NOT BANK_EXISTS(p_bank_id) THEN
            RAISE_APPLICATION_ERROR(-20051, 'البنك المحدد غير موجود');
        END IF;
        
        UPDATE HR_CORE.BANKS
        SET BANK_NAME_AR = NVL(p_bank_name_ar, BANK_NAME_AR),
            BANK_NAME_EN = NVL(p_bank_name_en, BANK_NAME_EN),
            BANK_CODE = NVL(p_bank_code, BANK_CODE)
        WHERE BANK_ID = p_bank_id;
        
        COMMIT;
    END;
    
    PROCEDURE DELETE_BANK(p_bank_id IN NUMBER) IS
    BEGIN
        IF NOT BANK_EXISTS(p_bank_id) THEN
            RAISE_APPLICATION_ERROR(-20052, 'البنك المحدد غير موجود');
        END IF;
        
        UPDATE HR_CORE.BANKS
        SET IS_DELETED = 1
        WHERE BANK_ID = p_bank_id;
        
        COMMIT;
    END;
    
    -- ==========================================================
    -- JOB GRADES MANAGEMENT
    -- ==========================================================
    
    PROCEDURE ADD_JOB_GRADE(
        p_grade_name        IN VARCHAR2,
        p_min_salary        IN NUMBER,
        p_max_salary        IN NUMBER,
        p_ticket_class      IN VARCHAR2 DEFAULT NULL,
        p_grade_id          OUT NUMBER
    ) IS
    BEGIN
        IF p_grade_name IS NULL THEN
            RAISE_APPLICATION_ERROR(-20060, 'اسم الدرجة مطلوب');
        END IF;
        
        IF p_min_salary IS NULL OR p_max_salary IS NULL THEN
            RAISE_APPLICATION_ERROR(-20061, 'الحد الأدنى والأقصى للراتب مطلوبان');
        END IF;
        
        IF p_min_salary >= p_max_salary THEN
            RAISE_APPLICATION_ERROR(-20062, 'الحد الأدنى يجب أن يكون أقل من الحد الأقصى');
        END IF;
        
        INSERT INTO HR_CORE.JOB_GRADES (
            GRADE_NAME, MIN_SALARY, MAX_SALARY, TICKET_CLASS
        ) VALUES (
            p_grade_name, p_min_salary, p_max_salary, p_ticket_class
        ) RETURNING GRADE_ID INTO p_grade_id;
        
        COMMIT;
    END;
    
    PROCEDURE UPDATE_JOB_GRADE(
        p_grade_id          IN NUMBER,
        p_grade_name        IN VARCHAR2 DEFAULT NULL,
        p_min_salary        IN NUMBER DEFAULT NULL,
        p_max_salary        IN NUMBER DEFAULT NULL,
        p_ticket_class      IN VARCHAR2 DEFAULT NULL
    ) IS
    BEGIN
        UPDATE HR_CORE.JOB_GRADES
        SET GRADE_NAME = NVL(p_grade_name, GRADE_NAME),
            MIN_SALARY = NVL(p_min_salary, MIN_SALARY),
            MAX_SALARY = NVL(p_max_salary, MAX_SALARY),
            TICKET_CLASS = NVL(p_ticket_class, TICKET_CLASS)
        WHERE GRADE_ID = p_grade_id;
        
        COMMIT;
    END;
    
    PROCEDURE DELETE_JOB_GRADE(p_grade_id IN NUMBER) IS
    BEGIN
        UPDATE HR_CORE.JOB_GRADES
        SET IS_DELETED = 1
        WHERE GRADE_ID = p_grade_id;
        
        COMMIT;
    END;
    
    -- ==========================================================
    -- DOCUMENT TYPES MANAGEMENT
    -- ==========================================================
    
    PROCEDURE ADD_DOCUMENT_TYPE(
        p_doc_name_ar           IN VARCHAR2,
        p_is_mandatory          IN NUMBER DEFAULT 0,
        p_requires_expiry       IN NUMBER DEFAULT 1,
        p_alert_days_before     IN NUMBER DEFAULT 30,
        p_doc_type_id           OUT NUMBER
    ) IS
    BEGIN
        IF p_doc_name_ar IS NULL THEN
            RAISE_APPLICATION_ERROR(-20070, 'اسم نوع الوثيقة مطلوب');
        END IF;
        
        INSERT INTO HR_CORE.DOCUMENT_TYPES (
            DOC_NAME_AR, IS_MANDATORY, REQUIRES_EXPIRY, ALERT_DAYS_BEFORE
        ) VALUES (
            p_doc_name_ar, p_is_mandatory, p_requires_expiry, p_alert_days_before
        ) RETURNING DOC_TYPE_ID INTO p_doc_type_id;
        
        COMMIT;
    END;
    
    PROCEDURE UPDATE_DOCUMENT_TYPE(
        p_doc_type_id           IN NUMBER,
        p_doc_name_ar           IN VARCHAR2 DEFAULT NULL,
        p_is_mandatory          IN NUMBER DEFAULT NULL,
        p_requires_expiry       IN NUMBER DEFAULT NULL,
        p_alert_days_before     IN NUMBER DEFAULT NULL
    ) IS
    BEGIN
        UPDATE HR_CORE.DOCUMENT_TYPES
        SET DOC_NAME_AR = NVL(p_doc_name_ar, DOC_NAME_AR),
            IS_MANDATORY = NVL(p_is_mandatory, IS_MANDATORY),
            REQUIRES_EXPIRY = NVL(p_requires_expiry, REQUIRES_EXPIRY),
            ALERT_DAYS_BEFORE = NVL(p_alert_days_before, ALERT_DAYS_BEFORE)
        WHERE DOC_TYPE_ID = p_doc_type_id;
        
        COMMIT;
    END;
    
    PROCEDURE DELETE_DOCUMENT_TYPE(p_doc_type_id IN NUMBER) IS
    BEGIN
        UPDATE HR_CORE.DOCUMENT_TYPES
        SET IS_DELETED = 1
        WHERE DOC_TYPE_ID = p_doc_type_id;
        
        COMMIT;
    END;

END PKG_CORE_MANAGER;
/

PROMPT ========================================
PROMPT PKG_CORE_MANAGER Body Created!
PROMPT ========================================
PROMPT
PROMPT Package includes:
PROMPT - Helper Functions (8 functions)
PROMPT - Departments Management (4 procedures)
PROMPT - Jobs Management (3 procedures)
PROMPT - Branches Management (3 procedures)
PROMPT - Cities Management (3 procedures)
PROMPT - Countries Management (3 procedures)
PROMPT - Banks Management (3 procedures)
PROMPT - Job Grades Management (3 procedures)
PROMPT - Document Types Management (3 procedures)
PROMPT
PROMPT Total: 8 Functions + 25 Procedures
PROMPT ========================================




