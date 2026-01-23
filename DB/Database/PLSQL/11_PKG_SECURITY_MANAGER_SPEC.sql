-- =================================================================================
-- الحزمة: PKG_SECURITY_MANAGER
-- الوصف: إدارة المستخدمين والصلاحيات (Specification)
-- المخطط: HR_CORE
-- الوظائف: إنشاء مستخدمين، إدارة الأدوار، منح الصلاحيات
-- =================================================================================

CREATE OR REPLACE PACKAGE HR_CORE.PKG_SECURITY_MANAGER AS

    -- ==========================================================
    -- إجراء: إنشاء مستخدم جديد
    -- ==========================================================
    PROCEDURE CREATE_USER (
        p_username          IN VARCHAR2,
        p_password          IN VARCHAR2,
        p_employee_id       IN NUMBER,
        p_email             IN VARCHAR2,
        p_created_by        IN VARCHAR2,
        o_user_id           OUT NUMBER,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- إجراء: تسجيل الدخول
    -- ==========================================================
    PROCEDURE LOGIN (
        p_username          IN VARCHAR2,
        p_password          IN VARCHAR2,
        o_user_id           OUT NUMBER,
        o_employee_id       OUT NUMBER,
        o_is_success        OUT NUMBER,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- إجراء: تغيير كلمة المرور
    -- ==========================================================
    PROCEDURE CHANGE_PASSWORD (
        p_user_id           IN NUMBER,
        p_old_password      IN VARCHAR2,
        p_new_password      IN VARCHAR2,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- إجراء: إنشاء دور جديد
    -- ==========================================================
    PROCEDURE CREATE_ROLE (
        p_role_name         IN VARCHAR2,
        p_role_name_ar      IN VARCHAR2,
        p_description       IN VARCHAR2,
        p_created_by        IN VARCHAR2,
        o_role_id           OUT NUMBER,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- إجراء: منح دور لمستخدم
    -- ==========================================================
    PROCEDURE ASSIGN_ROLE_TO_USER (
        p_user_id           IN NUMBER,
        p_role_id           IN NUMBER,
        p_assigned_by       IN VARCHAR2,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- إجراء: منح صلاحية لدور
    -- ==========================================================
    PROCEDURE ASSIGN_PERMISSION_TO_ROLE (
        p_role_id           IN NUMBER,
        p_permission_id     IN NUMBER,
        p_created_by        IN VARCHAR2,
        o_message           OUT VARCHAR2
    );

    -- ==========================================================
    -- دالة: التحقق من صلاحية المستخدم
    -- ==========================================================
    FUNCTION CHECK_USER_PERMISSION (
        p_user_id           IN NUMBER,
        p_permission_name   IN VARCHAR2
    ) RETURN NUMBER;

    -- ==========================================================
    -- إجراء: تعطيل/تفعيل مستخدم
    -- ==========================================================
    PROCEDURE TOGGLE_USER_STATUS (
        p_user_id           IN NUMBER,
        p_is_active         IN NUMBER,
        p_updated_by        IN VARCHAR2,
        o_message           OUT VARCHAR2
    );

END PKG_SECURITY_MANAGER;
/
