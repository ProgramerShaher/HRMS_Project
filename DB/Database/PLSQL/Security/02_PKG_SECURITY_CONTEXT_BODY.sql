-- =================================================================================
-- الحزمة: PKG_SECURITY_CONTEXT
-- الوصف: إدارة معلومات المستخدم في الـ Session (Body)
-- =================================================================================

CREATE OR REPLACE PACKAGE BODY PKG_SECURITY_CONTEXT AS

    -- ==========================================================
    -- إجراء: تعيين معلومات المستخدم في الـ Session
    -- ==========================================================
    PROCEDURE SET_USER_INFO (
        p_username      IN VARCHAR2,
        p_user_id       IN NUMBER DEFAULT NULL,
        p_branch_id     IN NUMBER DEFAULT NULL,
        p_dept_id       IN NUMBER DEFAULT NULL
    ) IS
    BEGIN
        -- التحقق من صحة اسم المستخدم
        IF p_username IS NULL OR TRIM(p_username) = '' THEN
            RAISE_APPLICATION_ERROR(-20001, 'اسم المستخدم مطلوب ولا يمكن أن يكون فارغاً');
        END IF;

        -- تعيين اسم المستخدم
        DBMS_SESSION.SET_CONTEXT(
            namespace => C_CONTEXT_NAMESPACE,
            attribute => 'USERNAME',
            value     => UPPER(TRIM(p_username))
        );

        -- تعيين معرف المستخدم
        IF p_user_id IS NOT NULL THEN
            DBMS_SESSION.SET_CONTEXT(
                namespace => C_CONTEXT_NAMESPACE,
                attribute => 'USER_ID',
                value     => TO_CHAR(p_user_id)
            );
        END IF;

        -- تعيين معرف الفرع
        IF p_branch_id IS NOT NULL THEN
            DBMS_SESSION.SET_CONTEXT(
                namespace => C_CONTEXT_NAMESPACE,
                attribute => 'BRANCH_ID',
                value     => TO_CHAR(p_branch_id)
            );
        END IF;

        -- تعيين معرف القسم
        IF p_dept_id IS NOT NULL THEN
            DBMS_SESSION.SET_CONTEXT(
                namespace => C_CONTEXT_NAMESPACE,
                attribute => 'DEPT_ID',
                value     => TO_CHAR(p_dept_id)
            );
        END IF;

        -- تسجيل وقت تعيين الـ context
        DBMS_SESSION.SET_CONTEXT(
            namespace => C_CONTEXT_NAMESPACE,
            attribute => 'SET_TIME',
            value     => TO_CHAR(SYSDATE, 'YYYY-MM-DD HH24:MI:SS')
        );

        -- تسجيل Session ID
        DBMS_SESSION.SET_CONTEXT(
            namespace => C_CONTEXT_NAMESPACE,
            attribute => 'SESSION_ID',
            value     => TO_CHAR(SYS_CONTEXT('USERENV', 'SESSIONID'))
        );

    EXCEPTION
        WHEN OTHERS THEN
            RAISE_APPLICATION_ERROR(-20002, 'خطأ في تعيين معلومات المستخدم: ' || SQLERRM);
    END SET_USER_INFO;

    -- ==========================================================
    -- دالة: الحصول على اسم المستخدم الحالي
    -- ==========================================================
    FUNCTION GET_CURRENT_USER RETURN VARCHAR2 IS
        v_username VARCHAR2(100);
    BEGIN
        v_username := SYS_CONTEXT(C_CONTEXT_NAMESPACE, 'USERNAME');
        
        -- إذا لم يتم تعيين context، نستخدم القيمة الافتراضية
        IF v_username IS NULL THEN
            RETURN C_DEFAULT_USER;
        END IF;
        
        RETURN v_username;
    EXCEPTION
        WHEN OTHERS THEN
            RETURN C_DEFAULT_USER;
    END GET_CURRENT_USER;

    -- ==========================================================
    -- دالة: الحصول على معرف المستخدم الحالي
    -- ==========================================================
    FUNCTION GET_CURRENT_USER_ID RETURN NUMBER IS
        v_user_id VARCHAR2(100);
    BEGIN
        v_user_id := SYS_CONTEXT(C_CONTEXT_NAMESPACE, 'USER_ID');
        
        IF v_user_id IS NULL THEN
            RETURN NULL;
        END IF;
        
        RETURN TO_NUMBER(v_user_id);
    EXCEPTION
        WHEN OTHERS THEN
            RETURN NULL;
    END GET_CURRENT_USER_ID;

    -- ==========================================================
    -- دالة: الحصول على معرف الفرع الحالي
    -- ==========================================================
    FUNCTION GET_CURRENT_BRANCH_ID RETURN NUMBER IS
        v_branch_id VARCHAR2(100);
    BEGIN
        v_branch_id := SYS_CONTEXT(C_CONTEXT_NAMESPACE, 'BRANCH_ID');
        
        IF v_branch_id IS NULL THEN
            RETURN NULL;
        END IF;
        
        RETURN TO_NUMBER(v_branch_id);
    EXCEPTION
        WHEN OTHERS THEN
            RETURN NULL;
    END GET_CURRENT_BRANCH_ID;

    -- ==========================================================
    -- دالة: الحصول على معرف القسم الحالي
    -- ==========================================================
    FUNCTION GET_CURRENT_DEPT_ID RETURN NUMBER IS
        v_dept_id VARCHAR2(100);
    BEGIN
        v_dept_id := SYS_CONTEXT(C_CONTEXT_NAMESPACE, 'DEPT_ID');
        
        IF v_dept_id IS NULL THEN
            RETURN NULL;
        END IF;
        
        RETURN TO_NUMBER(v_dept_id);
    EXCEPTION
        WHEN OTHERS THEN
            RETURN NULL;
    END GET_CURRENT_DEPT_ID;

    -- ==========================================================
    -- إجراء: مسح معلومات المستخدم من الـ Session
    -- ==========================================================
    PROCEDURE CLEAR_CONTEXT IS
    BEGIN
        DBMS_SESSION.CLEAR_CONTEXT(
            namespace => C_CONTEXT_NAMESPACE,
            client_id => NULL,
            attribute => NULL
        );
    EXCEPTION
        WHEN OTHERS THEN
            -- تجاهل الأخطاء عند المسح
            NULL;
    END CLEAR_CONTEXT;

    -- ==========================================================
    -- دالة: التحقق من وجود context نشط
    -- ==========================================================
    FUNCTION IS_CONTEXT_SET RETURN NUMBER IS
        v_username VARCHAR2(100);
    BEGIN
        v_username := SYS_CONTEXT(C_CONTEXT_NAMESPACE, 'USERNAME');
        
        IF v_username IS NULL THEN
            RETURN 0;
        ELSE
            RETURN 1;
        END IF;
    END IS_CONTEXT_SET;

    -- ==========================================================
    -- دالة: الحصول على معلومات كاملة عن الـ Session
    -- ==========================================================
    FUNCTION GET_SESSION_INFO RETURN VARCHAR2 IS
        v_info VARCHAR2(4000);
    BEGIN
        v_info := '{' ||
            '"username":"' || NVL(GET_CURRENT_USER(), 'NULL') || '",' ||
            '"user_id":' || NVL(TO_CHAR(GET_CURRENT_USER_ID()), 'null') || ',' ||
            '"branch_id":' || NVL(TO_CHAR(GET_CURRENT_BRANCH_ID()), 'null') || ',' ||
            '"dept_id":' || NVL(TO_CHAR(GET_CURRENT_DEPT_ID()), 'null') || ',' ||
            '"set_time":"' || NVL(SYS_CONTEXT(C_CONTEXT_NAMESPACE, 'SET_TIME'), 'NULL') || '",' ||
            '"session_id":"' || NVL(SYS_CONTEXT('USERENV', 'SESSIONID'), 'NULL') || '",' ||
            '"db_user":"' || SYS_CONTEXT('USERENV', 'SESSION_USER') || '",' ||
            '"ip_address":"' || SYS_CONTEXT('USERENV', 'IP_ADDRESS') || '",' ||
            '"host":"' || SYS_CONTEXT('USERENV', 'HOST') || '"' ||
        '}';
        
        RETURN v_info;
    END GET_SESSION_INFO;

END PKG_SECURITY_CONTEXT;
/
