-- =================================================================================
-- الحزمة: PKG_ATTENDANCE_MANAGER
-- الوصف: إدارة الحضور والانصراف (Specification)
-- المخطط: HR_ATTENDANCE
-- الوظائف: معالجة البصمة، حساب التأخير، الساعات الإضافية، التقارير
-- =================================================================================

CREATE OR REPLACE PACKAGE HR_ATTENDANCE.PKG_ATTENDANCE_MANAGER AS

    -- ==========================================================
    -- إجراء: معالجة سجلات البصمة اليومية
    -- ==========================================================
    PROCEDURE PROCESS_DAILY_PUNCHES (
        p_attendance_date   IN DATE,
        p_employee_id       IN NUMBER DEFAULT NULL, -- NULL = جميع الموظفين
        p_created_by        IN VARCHAR2,
        o_records_processed OUT NUMBER,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- دالة: حساب دقائق التأخير
    -- ==========================================================
    FUNCTION CALCULATE_LATE_MINUTES (
        p_planned_time      IN TIMESTAMP,
        p_actual_time       IN TIMESTAMP,
        p_grace_period      IN NUMBER DEFAULT 15
    ) RETURN NUMBER;

    -- ==========================================================
    -- دالة: حساب ساعات العمل الفعلية
    -- ==========================================================
    FUNCTION CALCULATE_WORKING_HOURS (
        p_in_time           IN TIMESTAMP,
        p_out_time          IN TIMESTAMP
    ) RETURN NUMBER;

    -- ==========================================================
    -- إجراء: طلب ساعات إضافية
    -- ==========================================================
    PROCEDURE REQUEST_OVERTIME (
        p_employee_id       IN NUMBER,
        p_work_date         IN DATE,
        p_hours_requested   IN NUMBER,
        p_reason            IN VARCHAR2,
        p_created_by        IN VARCHAR2,
        o_request_id        OUT NUMBER,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- إجراء: اعتماد طلب ساعات إضافية
    -- ==========================================================
    PROCEDURE APPROVE_OVERTIME (
        p_request_id        IN NUMBER,
        p_approved_hours    IN NUMBER,
        p_approved_by       IN NUMBER,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- إجراء: إنشاء جدول العمل الشهري
    -- ==========================================================
    PROCEDURE GENERATE_MONTHLY_ROSTER (
        p_year              IN NUMBER,
        p_month             IN NUMBER,
        p_dept_id           IN NUMBER DEFAULT NULL,
        p_default_shift_id  IN NUMBER,
        p_created_by        IN VARCHAR2,
        o_records_created   OUT NUMBER,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- دالة: الحصول على حالة الحضور
    -- ==========================================================
    FUNCTION GET_ATTENDANCE_STATUS (
        p_employee_id       IN NUMBER,
        p_attendance_date   IN DATE
    ) RETURN VARCHAR2;

    -- ==========================================================
    -- إجراء: تصحيح سجل حضور يدوياً
    -- ==========================================================
    PROCEDURE CORRECT_ATTENDANCE (
        p_record_id         IN NUMBER,
        p_actual_in_time    IN TIMESTAMP,
        p_actual_out_time   IN TIMESTAMP,
        p_reason            IN VARCHAR2,
        p_corrected_by      IN VARCHAR2,
        o_message           OUT VARCHAR2
    );

END PKG_ATTENDANCE_MANAGER;
/
