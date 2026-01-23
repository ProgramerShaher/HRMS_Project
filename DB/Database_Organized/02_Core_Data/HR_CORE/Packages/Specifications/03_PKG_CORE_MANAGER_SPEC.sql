-- =================================================================================
-- Package: PKG_CORE_MANAGER (Specification)
-- Description: Complete business logic for HR_CORE management
-- Schema: HR_CORE
-- =================================================================================

CREATE OR REPLACE PACKAGE HR_CORE.PKG_CORE_MANAGER AS

    -- ==========================================================
    -- DEPARTMENTS MANAGEMENT (إدارة الأقسام)
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
        p_parent_dept_id    IN NUMBER DEFAULT NULL,
        p_cost_center_code  IN VARCHAR2 DEFAULT NULL,
        p_is_active         IN NUMBER DEFAULT NULL
    );
    
    PROCEDURE DELETE_DEPARTMENT(
        p_dept_id IN NUMBER
    );
    
    FUNCTION GET_DEPARTMENT_NAME(
        p_dept_id IN NUMBER
    ) RETURN VARCHAR2;
    
    -- ==========================================================
    -- JOBS MANAGEMENT (إدارة الوظائف)
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
        p_job_title_en      IN VARCHAR2 DEFAULT NULL,
        p_default_grade_id  IN NUMBER DEFAULT NULL,
        p_is_medical        IN NUMBER DEFAULT NULL
    );
    
    PROCEDURE DELETE_JOB(
        p_job_id IN NUMBER
    );
    
    -- ==========================================================
    -- BRANCHES MANAGEMENT (إدارة الفروع)
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
        p_branch_name_en    IN VARCHAR2 DEFAULT NULL,
        p_city_id           IN NUMBER DEFAULT NULL,
        p_address           IN VARCHAR2 DEFAULT NULL
    );
    
    PROCEDURE DELETE_BRANCH(
        p_branch_id IN NUMBER
    );
    
    -- ==========================================================
    -- CITIES MANAGEMENT (إدارة المدن)
    -- ==========================================================
    
    PROCEDURE ADD_CITY(
        p_city_name_ar      IN VARCHAR2,
        p_city_name_en      IN VARCHAR2 DEFAULT NULL,
        p_country_id        IN NUMBER,
        p_city_id           OUT NUMBER
    );
    
    PROCEDURE UPDATE_CITY(
        p_city_id           IN NUMBER,
        p_city_name_ar      IN VARCHAR2 DEFAULT NULL,
        p_city_name_en      IN VARCHAR2 DEFAULT NULL
    );
    
    PROCEDURE DELETE_CITY(
        p_city_id IN NUMBER
    );
    
    -- ==========================================================
    -- COUNTRIES MANAGEMENT (إدارة الدول)
    -- ==========================================================
    
    PROCEDURE ADD_COUNTRY(
        p_country_name_ar       IN VARCHAR2,
        p_country_name_en       IN VARCHAR2,
        p_citizenship_name_ar   IN VARCHAR2 DEFAULT NULL,
        p_iso_code              IN CHAR DEFAULT NULL,
        p_country_id            OUT NUMBER
    );
    
    PROCEDURE UPDATE_COUNTRY(
        p_country_id            IN NUMBER,
        p_country_name_ar       IN VARCHAR2 DEFAULT NULL,
        p_country_name_en       IN VARCHAR2 DEFAULT NULL,
        p_citizenship_name_ar   IN VARCHAR2 DEFAULT NULL,
        p_iso_code              IN CHAR DEFAULT NULL
    );
    
    PROCEDURE DELETE_COUNTRY(
        p_country_id IN NUMBER
    );
    
    -- ==========================================================
    -- BANKS MANAGEMENT (إدارة البنوك)
    -- ==========================================================
    
    PROCEDURE ADD_BANK(
        p_bank_name_ar      IN VARCHAR2,
        p_bank_name_en      IN VARCHAR2 DEFAULT NULL,
        p_bank_code         IN VARCHAR2 DEFAULT NULL,
        p_bank_id           OUT NUMBER
    );
    
    PROCEDURE UPDATE_BANK(
        p_bank_id           IN NUMBER,
        p_bank_name_ar      IN VARCHAR2 DEFAULT NULL,
        p_bank_name_en      IN VARCHAR2 DEFAULT NULL,
        p_bank_code         IN VARCHAR2 DEFAULT NULL
    );
    
    PROCEDURE DELETE_BANK(
        p_bank_id IN NUMBER
    );
    
    -- ==========================================================
    -- JOB GRADES MANAGEMENT (إدارة الدرجات الوظيفية)
    -- ==========================================================
    
    PROCEDURE ADD_JOB_GRADE(
        p_grade_name        IN VARCHAR2,
        p_min_salary        IN NUMBER,
        p_max_salary        IN NUMBER,
        p_ticket_class      IN VARCHAR2 DEFAULT NULL,
        p_grade_id          OUT NUMBER
    );
    
    PROCEDURE UPDATE_JOB_GRADE(
        p_grade_id          IN NUMBER,
        p_grade_name        IN VARCHAR2 DEFAULT NULL,
        p_min_salary        IN NUMBER DEFAULT NULL,
        p_max_salary        IN NUMBER DEFAULT NULL,
        p_ticket_class      IN VARCHAR2 DEFAULT NULL
    );
    
    PROCEDURE DELETE_JOB_GRADE(
        p_grade_id IN NUMBER
    );
    
    -- ==========================================================
    -- DOCUMENT TYPES MANAGEMENT (إدارة أنواع الوثائق)
    -- ==========================================================
    
    PROCEDURE ADD_DOCUMENT_TYPE(
        p_doc_name_ar           IN VARCHAR2,
        p_is_mandatory          IN NUMBER DEFAULT 0,
        p_requires_expiry       IN NUMBER DEFAULT 1,
        p_alert_days_before     IN NUMBER DEFAULT 30,
        p_doc_type_id           OUT NUMBER
    );
    
    PROCEDURE UPDATE_DOCUMENT_TYPE(
        p_doc_type_id           IN NUMBER,
        p_doc_name_ar           IN VARCHAR2 DEFAULT NULL,
        p_is_mandatory          IN NUMBER DEFAULT NULL,
        p_requires_expiry       IN NUMBER DEFAULT NULL,
        p_alert_days_before     IN NUMBER DEFAULT NULL
    );
    
    PROCEDURE DELETE_DOCUMENT_TYPE(
        p_doc_type_id IN NUMBER
    );
    
    -- ==========================================================
    -- HELPER FUNCTIONS (دوال مساعدة)
    -- ==========================================================
    
    FUNCTION DEPARTMENT_EXISTS(p_dept_id IN NUMBER) RETURN BOOLEAN;
    FUNCTION JOB_EXISTS(p_job_id IN NUMBER) RETURN BOOLEAN;
    FUNCTION BRANCH_EXISTS(p_branch_id IN NUMBER) RETURN BOOLEAN;
    FUNCTION CITY_EXISTS(p_city_id IN NUMBER) RETURN BOOLEAN;
    FUNCTION COUNTRY_EXISTS(p_country_id IN NUMBER) RETURN BOOLEAN;
    FUNCTION BANK_EXISTS(p_bank_id IN NUMBER) RETURN BOOLEAN;
    
    FUNCTION DEPT_NAME_EXISTS(p_dept_name_ar IN VARCHAR2, p_exclude_id IN NUMBER DEFAULT NULL) RETURN BOOLEAN;
    FUNCTION JOB_TITLE_EXISTS(p_job_title_ar IN VARCHAR2, p_exclude_id IN NUMBER DEFAULT NULL) RETURN BOOLEAN;
    
END PKG_CORE_MANAGER;
/

PROMPT ========================================
PROMPT PKG_CORE_MANAGER Specification Created!
PROMPT ========================================
