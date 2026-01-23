-- =================================================================================
-- Package: PKG_HR_CORE_MANAGER (Specification)
-- Description: Core data management for HR_CORE schema
-- Schema: HR_CORE
-- Version: 2.0 (Fixed)
-- =================================================================================

CREATE OR REPLACE PACKAGE HR_CORE.PKG_HR_CORE_MANAGER AS

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
    );
    
    PROCEDURE UPDATE_DEPARTMENT(
        p_dept_id           IN NUMBER,
        p_dept_name_ar      IN VARCHAR2 DEFAULT NULL,
        p_dept_name_en      IN VARCHAR2 DEFAULT NULL,
        p_is_active         IN NUMBER DEFAULT NULL
    );
    
    PROCEDURE DELETE_DEPARTMENT(p_dept_id IN NUMBER);
    
    -- ==========================================================
    -- JOBS MANAGEMENT
    -- ==========================================================
    
    PROCEDURE ADD_JOB(
        p_job_title_ar      IN VARCHAR2,
        p_job_title_en      IN VARCHAR2 DEFAULT NULL,
        p_default_grade_id  IN NUMBER DEFAULT NULL,
        p_is_medical        IN NUMBER DEFAULT 0,
        p_job_id            OUT NUMBER
    );
    
    PROCEDURE UPDATE_JOB(
        p_job_id            IN NUMBER,
        p_job_title_ar      IN VARCHAR2 DEFAULT NULL,
        p_job_title_en      IN VARCHAR2 DEFAULT NULL
    );
    
    PROCEDURE DELETE_JOB(p_job_id IN NUMBER);
    
    -- ==========================================================
    -- BRANCHES MANAGEMENT
    -- ==========================================================
    
    PROCEDURE ADD_BRANCH(
        p_branch_name_ar    IN VARCHAR2,
        p_branch_name_en    IN VARCHAR2 DEFAULT NULL,
        p_city_id           IN NUMBER,
        p_address           IN VARCHAR2 DEFAULT NULL,
        p_branch_id         OUT NUMBER
    );
    
    PROCEDURE UPDATE_BRANCH(
        p_branch_id         IN NUMBER,
        p_branch_name_ar    IN VARCHAR2 DEFAULT NULL,
        p_address           IN VARCHAR2 DEFAULT NULL
    );
    
    PROCEDURE DELETE_BRANCH(p_branch_id IN NUMBER);
    
    -- ==========================================================
    -- CITIES MANAGEMENT
    -- ==========================================================
    
    PROCEDURE ADD_CITY(
        p_city_name_ar      IN VARCHAR2,
        p_city_name_en      IN VARCHAR2 DEFAULT NULL,
        p_country_id        IN NUMBER,
        p_city_id           OUT NUMBER
    );
    
    PROCEDURE UPDATE_CITY(
        p_city_id           IN NUMBER,
        p_city_name_ar      IN VARCHAR2 DEFAULT NULL
    );
    
    PROCEDURE DELETE_CITY(p_city_id IN NUMBER);
    
    -- ==========================================================
    -- COUNTRIES MANAGEMENT
    -- ==========================================================
    
    PROCEDURE ADD_COUNTRY(
        p_country_name_ar       IN VARCHAR2,
        p_country_name_en       IN VARCHAR2,
        p_iso_code              IN CHAR DEFAULT NULL,
        p_country_id            OUT NUMBER
    );
    
    PROCEDURE UPDATE_COUNTRY(
        p_country_id            IN NUMBER,
        p_country_name_ar       IN VARCHAR2 DEFAULT NULL,
        p_iso_code              IN CHAR DEFAULT NULL
    );
    
    PROCEDURE DELETE_COUNTRY(p_country_id IN NUMBER);
    
    -- ==========================================================
    -- BANKS MANAGEMENT
    -- ==========================================================
    
    PROCEDURE ADD_BANK(
        p_bank_name_ar      IN VARCHAR2,
        p_bank_name_en      IN VARCHAR2 DEFAULT NULL,
        p_bank_code         IN VARCHAR2 DEFAULT NULL,
        p_bank_id           OUT NUMBER
    );
    
    PROCEDURE UPDATE_BANK(
        p_bank_id           IN NUMBER,
        p_bank_name_ar      IN VARCHAR2 DEFAULT NULL
    );
    
    PROCEDURE DELETE_BANK(p_bank_id IN NUMBER);

END PKG_HR_CORE_MANAGER;
/

PROMPT ========================================
PROMPT PKG_HR_CORE_MANAGER Specification Created!
PROMPT ========================================
