-- =================================================================================
-- الحزمة: PKG_LEAVE_MANAGER
-- الوصف: إدارة الإجازات (Specification)
-- المخطط: HR_LEAVES
-- الوظائف: طلب إجازة، اعتماد إجازة، حساب الرصيد، ترحيل الرصيد
-- =================================================================================

CREATE OR REPLACE PACKAGE HR_LEAVES.PKG_LEAVE_MANAGER AS

    -- ==========================================================
    -- إجراء: طلب إجازة جديدة
    -- ==========================================================
    PROCEDURE REQUEST_LEAVE (
        p_employee_id       IN NUMBER,
        p_leave_type_id     IN NUMBER,
        p_start_date        IN DATE,
        p_end_date          IN DATE,
        p_reason            IN VARCHAR2,
        p_attachment_path   IN VARCHAR2 DEFAULT NULL,
        o_request_id        OUT NUMBER,
        o_days_count        OUT NUMBER,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- إجراء: اعتماد/رفض طلب إجازة
    -- ==========================================================
    PROCEDURE APPROVE_LEAVE_REQUEST (
        p_request_id        IN NUMBER,
        p_action            IN VARCHAR2, -- 'APPROVE' أو 'REJECT'
        p_approval_level    IN VARCHAR2, -- 'MANAGER' أو 'HR'
        p_rejection_reason  IN VARCHAR2 DEFAULT NULL,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- دالة: حساب رصيد الإجازات المتبقي
    -- ==========================================================
    FUNCTION GET_LEAVE_BALANCE (
        p_employee_id       IN NUMBER,
        p_leave_type_id     IN NUMBER,
        p_year              IN NUMBER DEFAULT EXTRACT(YEAR FROM SYSDATE)
    ) RETURN NUMBER;

    -- ==========================================================
    -- إجراء: ترحيل الرصيد المتبقي للسنة الجديدة
    -- ==========================================================
    PROCEDURE CARRY_FORWARD_BALANCE (
        p_employee_id       IN NUMBER DEFAULT NULL, -- NULL = جميع الموظفين
        p_from_year         IN NUMBER,
        p_to_year           IN NUMBER,
        o_employees_count   OUT NUMBER,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- إجراء: استحقاق الإجازات الشهري (Accrual)
    -- ==========================================================
    PROCEDURE MONTHLY_LEAVE_ACCRUAL (
        p_year              IN NUMBER,
        p_month             IN NUMBER,
        o_employees_count   OUT NUMBER,
        o_total_days        OUT NUMBER,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- دالة: حساب عدد أيام الإجازة (بدون عطل نهاية الأسبوع)
    -- ==========================================================
    FUNCTION CALCULATE_LEAVE_DAYS (
        p_start_date        IN DATE,
        p_end_date          IN DATE,
        p_exclude_weekends  IN NUMBER DEFAULT 1 -- 1 = استثناء الجمعة والسبت
    ) RETURN NUMBER;

    -- ==========================================================
    -- إجراء: إلغاء طلب إجازة (قبل الاعتماد فقط)
    -- ==========================================================
    PROCEDURE CANCEL_LEAVE_REQUEST (
        p_request_id        IN NUMBER,
        o_message           OUT VARCHAR2
    );

END PKG_LEAVE_MANAGER;
/
