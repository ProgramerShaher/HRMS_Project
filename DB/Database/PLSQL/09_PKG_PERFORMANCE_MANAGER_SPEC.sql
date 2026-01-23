-- =================================================================================
-- الحزمة: PKG_PERFORMANCE_MANAGER
-- الوصف: إدارة الأداء والجزاءات (Specification)
-- المخطط: HR_PERFORMANCE
-- الوظائف: التقييم السنوي، المخالفات، الجزاءات، KPIs
-- =================================================================================

CREATE OR REPLACE PACKAGE HR_PERFORMANCE.PKG_PERFORMANCE_MANAGER AS

    -- ==========================================================
    -- إجراء: إنشاء تقييم سنوي لموظف
    -- ==========================================================
    PROCEDURE CREATE_APPRAISAL (
        p_employee_id       IN NUMBER,
        p_cycle_id          IN NUMBER,
        p_evaluator_id      IN NUMBER,
        p_created_by        IN VARCHAR2,
        o_appraisal_id      OUT NUMBER,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- إجراء: إضافة تقييم لمؤشر أداء
    -- ==========================================================
    PROCEDURE ADD_KPI_SCORE (
        p_appraisal_id      IN NUMBER,
        p_kpi_id            IN NUMBER,
        p_target_value      IN NUMBER,
        p_actual_value      IN NUMBER,
        p_comments          IN VARCHAR2 DEFAULT NULL,
        p_created_by        IN VARCHAR2,
        o_detail_id         OUT NUMBER,
        o_score             OUT NUMBER,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- دالة: حساب الدرجة النهائية للتقييم
    -- ==========================================================
    FUNCTION CALCULATE_FINAL_SCORE (
        p_appraisal_id      IN NUMBER
    ) RETURN NUMBER;

    -- ==========================================================
    -- إجراء: اعتماد التقييم
    -- ==========================================================
    PROCEDURE APPROVE_APPRAISAL (
        p_appraisal_id      IN NUMBER,
        p_approved_by       IN VARCHAR2,
        o_final_score       OUT NUMBER,
        o_grade             OUT VARCHAR2,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- إجراء: تسجيل مخالفة
    -- ==========================================================
    PROCEDURE RECORD_VIOLATION (
        p_employee_id       IN NUMBER,
        p_violation_type_id IN NUMBER,
        p_violation_date    IN DATE,
        p_description       IN VARCHAR2,
        p_action_id         IN NUMBER DEFAULT NULL,
        p_created_by        IN VARCHAR2,
        o_violation_id      OUT NUMBER,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- إجراء: تنفيذ الجزاء
    -- ==========================================================
    PROCEDURE EXECUTE_DISCIPLINARY_ACTION (
        p_violation_id      IN NUMBER,
        p_executed_by       IN VARCHAR2,
        o_deduction_days    OUT NUMBER,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- دالة: حساب عدد المخالفات في فترة
    -- ==========================================================
    FUNCTION GET_VIOLATION_COUNT (
        p_employee_id       IN NUMBER,
        p_from_date         IN DATE,
        p_to_date           IN DATE,
        p_violation_type_id IN NUMBER DEFAULT NULL
    ) RETURN NUMBER;

END PKG_PERFORMANCE_MANAGER;
/
