-- =================================================================================
-- Package: PKG_HR_CORE_MANAGER (Body)
-- Description: Implementation of core data management
-- Schema: HR_CORE
-- Version: 2.0 (Fixed)
-- =================================================================================

CREATE OR REPLACE PACKAGE BODY HR_CORE.PKG_HR_CORE_MANAGER AS

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
            RAISE_APPLICATION_ERROR(-20001, 'اسم القسم مطلوب');
        END IF;
        
        INSERT INTO HR_CORE.DEPARTMENTS (
            DEPT_NAME_AR, DEPT_NAME_EN, BRANCH_ID, 
            PARENT_DEPT_ID, COST_CENTER_CODE, IS_ACTIVE
        ) VALUES (
            p_dept_name_ar, p_dept_name_en, p_branch_id,
            p_parent_dept_id, p_cost_center_code, 1
        ) RETURNING DEPT_ID INTO p_dept_id;
        
        COMMIT;
    END ADD_DEPARTMENT;
    
    PROCEDURE UPDATE_DEPARTMENT(
        p_dept_id           IN NUMBER,
        p_dept_name_ar      IN VARCHAR2 DEFAULT NULL,
        p_dept_name_en      IN VARCHAR2 DEFAULT NULL,
        p_is_active         IN NUMBER DEFAULT NULL
    ) IS
    BEGIN
        UPDATE HR_CORE.DEPARTMENTS
        SET DEPT_NAME_AR = NVL(p_dept_name_ar, DEPT_NAME_AR),
            DEPT_NAME_EN = NVL(p_dept_name_en, DEPT_NAME_EN),
            IS_ACTIVE = NVL(p_is_active, IS_ACTIVE)
        WHERE DEPT_ID = p_dept_id;
        
        COMMIT;
    END UPDATE_DEPARTMENT;
    
    PROCEDURE DELETE_DEPARTMENT(p_dept_id IN NUMBER) IS
    BEGIN
        UPDATE HR_CORE.DEPARTMENTS
        SET IS_DELETED = 1
        WHERE DEPT_ID = p_dept_id;
        
        COMMIT;
    END DELETE_DEPARTMENT;
    
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
            RAISE_APPLICATION_ERROR(-20010, 'المسمى الوظيفي مطلوب');
        END IF;
        
        INSERT INTO HR_CORE.JOBS (
            JOB_TITLE_AR, JOB_TITLE_EN, 
            DEFAULT_GRADE_ID, IS_MEDICAL
        ) VALUES (
            p_job_title_ar, p_job_title_en,
            p_default_grade_id, p_is_medical
        ) RETURNING JOB_ID INTO p_job_id;
        
        COMMIT;
    END ADD_JOB;
    
    PROCEDURE UPDATE_JOB(
        p_job_id            IN NUMBER,
        p_job_title_ar      IN VARCHAR2 DEFAULT NULL,
        p_job_title_en      IN VARCHAR2 DEFAULT NULL
    ) IS
    BEGIN
        UPDATE HR_CORE.JOBS
        SET JOB_TITLE_AR = NVL(p_job_title_ar, JOB_TITLE_AR),
            JOB_TITLE_EN = NVL(p_job_title_en, JOB_TITLE_EN)
        WHERE JOB_ID = p_job_id;
        
        COMMIT;
    END UPDATE_JOB;
    
    PROCEDURE DELETE_JOB(p_job_id IN NUMBER) IS
    BEGIN
        UPDATE HR_CORE.JOBS
        SET IS_DELETED = 1
        WHERE JOB_ID = p_job_id;
        
        COMMIT;
    END DELETE_JOB;
    
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
            RAISE_APPLICATION_ERROR(-20020, 'اسم الفرع مطلوب');
        END IF;
        
        INSERT INTO HR_CORE.BRANCHES (
            BRANCH_NAME_AR, BRANCH_NAME_EN, CITY_ID, ADDRESS
        ) VALUES (
            p_branch_name_ar, p_branch_name_en, p_city_id, p_address
        ) RETURNING BRANCH_ID INTO p_branch_id;
        
        COMMIT;
    END ADD_BRANCH;
    
    PROCEDURE UPDATE_BRANCH(
        p_branch_id         IN NUMBER,
        p_branch_name_ar    IN VARCHAR2 DEFAULT NULL,
        p_address           IN VARCHAR2 DEFAULT NULL
    ) IS
    BEGIN
        UPDATE HR_CORE.BRANCHES
        SET BRANCH_NAME_AR = NVL(p_branch_name_ar, BRANCH_NAME_AR),
            ADDRESS = NVL(p_address, ADDRESS)
        WHERE BRANCH_ID = p_branch_id;
        
        COMMIT;
    END UPDATE_BRANCH;
    
    PROCEDURE DELETE_BRANCH(p_branch_id IN NUMBER) IS
    BEGIN
        UPDATE HR_CORE.BRANCHES
        SET IS_DELETED = 1
        WHERE BRANCH_ID = p_branch_id;
        
        COMMIT;
    END DELETE_BRANCH;
    
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
            RAISE_APPLICATION_ERROR(-20030, 'اسم المدينة مطلوب');
        END IF;
        
        INSERT INTO HR_CORE.CITIES (
            CITY_NAME_AR, CITY_NAME_EN, COUNTRY_ID
        ) VALUES (
            p_city_name_ar, p_city_name_en, p_country_id
        ) RETURNING CITY_ID INTO p_city_id;
        
        COMMIT;
    END ADD_CITY;
    
    PROCEDURE UPDATE_CITY(
        p_city_id           IN NUMBER,
        p_city_name_ar      IN VARCHAR2 DEFAULT NULL
    ) IS
    BEGIN
        UPDATE HR_CORE.CITIES
        SET CITY_NAME_AR = NVL(p_city_name_ar, CITY_NAME_AR)
        WHERE CITY_ID = p_city_id;
        
        COMMIT;
    END UPDATE_CITY;
    
    PROCEDURE DELETE_CITY(p_city_id IN NUMBER) IS
    BEGIN
        UPDATE HR_CORE.CITIES
        SET IS_DELETED = 1
        WHERE CITY_ID = p_city_id;
        
        COMMIT;
    END DELETE_CITY;
    
    -- ==========================================================
    -- COUNTRIES MANAGEMENT
    -- ==========================================================
    
    PROCEDURE ADD_COUNTRY(
        p_country_name_ar       IN VARCHAR2,
        p_country_name_en       IN VARCHAR2,
        p_iso_code              IN CHAR DEFAULT NULL,
        p_country_id            OUT NUMBER
    ) IS
    BEGIN
        IF p_country_name_ar IS NULL OR p_country_name_en IS NULL THEN
            RAISE_APPLICATION_ERROR(-20040, 'اسم الدولة مطلوب');
        END IF;
        
        INSERT INTO HR_CORE.COUNTRIES (
            COUNTRY_NAME_AR, COUNTRY_NAME_EN, ISO_CODE
        ) VALUES (
            p_country_name_ar, p_country_name_en, p_iso_code
        ) RETURNING COUNTRY_ID INTO p_country_id;
        
        COMMIT;
    END ADD_COUNTRY;
    
    PROCEDURE UPDATE_COUNTRY(
        p_country_id            IN NUMBER,
        p_country_name_ar       IN VARCHAR2 DEFAULT NULL,
        p_iso_code              IN CHAR DEFAULT NULL
    ) IS
    BEGIN
        UPDATE HR_CORE.COUNTRIES
        SET COUNTRY_NAME_AR = NVL(p_country_name_ar, COUNTRY_NAME_AR),
            ISO_CODE = NVL(p_iso_code, ISO_CODE)
        WHERE COUNTRY_ID = p_country_id;
        
        COMMIT;
    END UPDATE_COUNTRY;
    
    PROCEDURE DELETE_COUNTRY(p_country_id IN NUMBER) IS
    BEGIN
        UPDATE HR_CORE.COUNTRIES
        SET IS_DELETED = 1
        WHERE COUNTRY_ID = p_country_id;
        
        COMMIT;
    END DELETE_COUNTRY;
    
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
            RAISE_APPLICATION_ERROR(-20050, 'اسم البنك مطلوب');
        END IF;
        
        INSERT INTO HR_CORE.BANKS (
            BANK_NAME_AR, BANK_NAME_EN, BANK_CODE
        ) VALUES (
            p_bank_name_ar, p_bank_name_en, p_bank_code
        ) RETURNING BANK_ID INTO p_bank_id;
        
        COMMIT;
    END ADD_BANK;
    
    PROCEDURE UPDATE_BANK(
        p_bank_id           IN NUMBER,
        p_bank_name_ar      IN VARCHAR2 DEFAULT NULL
    ) IS
    BEGIN
        UPDATE HR_CORE.BANKS
        SET BANK_NAME_AR = NVL(p_bank_name_ar, BANK_NAME_AR)
        WHERE BANK_ID = p_bank_id;
        
        COMMIT;
    END UPDATE_BANK;
    
    PROCEDURE DELETE_BANK(p_bank_id IN NUMBER) IS
    BEGIN
        UPDATE HR_CORE.BANKS
        SET IS_DELETED = 1
        WHERE BANK_ID = p_bank_id;
        
        COMMIT;
    END DELETE_BANK;

END PKG_HR_CORE_MANAGER;
/

PROMPT ========================================
PROMPT PKG_HR_CORE_MANAGER Body Created!
PROMPT ========================================
PROMPT
PROMPT Package includes:
PROMPT - Departments Management (3 procedures)
PROMPT - Jobs Management (3 procedures)
PROMPT - Branches Management (3 procedures)
PROMPT - Cities Management (3 procedures)
PROMPT - Countries Management (3 procedures)
PROMPT - Banks Management (3 procedures)
PROMPT
PROMPT Total: 18 Procedures
PROMPT ========================================
