-- =================================================================================
-- الحزمة: PKG_PAYROLL_MANAGER
-- الوصف: إدارة الرواتب (Specification)
-- المخطط: HR_PAYROLL
-- الوظائف: تشغيل مسير الرواتب، حساب الراتب، إنشاء القسائم، التقارير
-- =================================================================================

CREATE OR REPLACE PACKAGE HR_PAYROLL.PKG_PAYROLL_MANAGER AS

    -- ==========================================================
    -- إجراء: تشغيل مسير الرواتب الشهري
    -- ==========================================================
    PROCEDURE RUN_MONTHLY_PAYROLL (
        p_year              IN NUMBER,
        p_month             IN NUMBER,
        o_run_id            OUT NUMBER,
        o_employees_count   OUT NUMBER,
        o_total_payout      OUT NUMBER,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- دالة: حساب الراتب الصافي لموظف
    -- ==========================================================
    FUNCTION CALCULATE_NET_SALARY (
        p_employee_id       IN NUMBER,
        p_year              IN NUMBER,
        p_month             IN NUMBER
    ) RETURN NUMBER;

    -- ==========================================================
    -- إجراء: إنشاء قسيمة راتب لموظف
    -- ==========================================================
    PROCEDURE GENERATE_PAYSLIP (
        p_run_id            IN NUMBER,
        p_employee_id       IN NUMBER,
        o_payslip_id        OUT NUMBER,
        o_net_salary        OUT NUMBER,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- إجراء: اعتماد مسير الرواتب
    -- ==========================================================
    PROCEDURE APPROVE_PAYROLL_RUN (
        p_run_id            IN NUMBER,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- إجراء: إضافة تعديل يدوي على الراتب
    -- ==========================================================
    PROCEDURE ADD_PAYROLL_ADJUSTMENT (
        p_employee_id       IN NUMBER,
        p_payroll_run_id    IN NUMBER,
        p_adjustment_type   IN VARCHAR2, -- 'BONUS', 'DEDUCTION', 'CORRECTION'
        p_amount            IN NUMBER,
        p_reason            IN VARCHAR2,
        p_approved_by       IN NUMBER,
        o_adjustment_id     OUT NUMBER,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- دالة: حساب خصم القرض الشهري
    -- ==========================================================
    FUNCTION CALCULATE_LOAN_DEDUCTION (
        p_employee_id       IN NUMBER,
        p_year              IN NUMBER,
        p_month             IN NUMBER
    ) RETURN NUMBER;

    -- ==========================================================
    -- إجراء: منح قرض لموظف
    -- ==========================================================
    PROCEDURE GRANT_LOAN (
        p_employee_id       IN NUMBER,
        p_loan_amount       IN NUMBER,
        p_installment_count IN NUMBER,
        o_loan_id           OUT NUMBER,
        o_monthly_installment OUT NUMBER,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- دالة: حساب مكافأة نهاية الخدمة
    -- ==========================================================
    FUNCTION CALCULATE_END_OF_SERVICE (
        p_employee_id       IN NUMBER,
        p_termination_date  IN DATE
    ) RETURN NUMBER;

    -- ==========================================================
    -- إجراء: حساب وتسجيل مكافأة نهاية الخدمة
    -- ==========================================================
    PROCEDURE PROCESS_END_OF_SERVICE (
        p_employee_id       IN NUMBER,
        p_termination_date  IN DATE,
        o_eos_id            OUT NUMBER,
        o_total_amount      OUT NUMBER,
        o_message           OUT VARCHAR2
    );

END PKG_PAYROLL_MANAGER;
/
