-- =================================================================================
-- الحزمة: PKG_SECURITY_MANAGER
-- الوصف: تنفيذ منطق إدارة الأمان (Body)
-- المخطط: HR_CORE
-- =================================================================================

CREATE OR REPLACE PACKAGE BODY HR_CORE.PKG_SECURITY_MANAGER AS

    -- ==========================================================
    -- دالة خاصة: تشفير كلمة المرور (Hash)
    -- ==========================================================
    FUNCTION HASH_PASSWORD (p_password IN VARCHAR2) RETURN VARCHAR2 IS
    BEGIN
        -- استخدام DBMS_UTILITY.GET_HASH_VALUE مع تحويل إلى نص
        RETURN TO_CHAR(DBMS_UTILITY.GET_HASH_VALUE(p_password, 1, POWER(2, 30)));
    END;

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
    ) IS
        v_user_id NUMBER;
        v_password_hash VARCHAR2(256);
        v_existing_count NUMBER;
    BEGIN
        -- التحقق من عدم وجود اسم مستخدم مكرر
        SELECT COUNT(*) INTO v_existing_count
        FROM HR_CORE.SYSTEM_USERS
        WHERE UPPER(USERNAME) = UPPER(p_username);

        IF v_existing_count > 0 THEN
            RAISE_APPLICATION_ERROR(-20001, 'اسم المستخدم موجود بالفعل');
        END IF;

        -- التحقق من قوة كلمة المرور
        IF LENGTH(p_password) < 8 THEN
            RAISE_APPLICATION_ERROR(-20002, 'كلمة المرور يجب أن تكون 8 أحرف على الأقل');
        END IF;

        -- تشفير كلمة المرور
        v_password_hash := HASH_PASSWORD(p_password);

        -- إنشاء المستخدم
        INSERT INTO HR_CORE.SYSTEM_USERS (
            USERNAME, PASSWORD_HASH, EMPLOYEE_ID, EMAIL, IS_ACTIVE, CREATED_BY
        ) VALUES (
            p_username, v_password_hash, p_employee_id, p_email, 1, p_created_by
        ) RETURNING USER_ID INTO v_user_id;

        o_user_id := v_user_id;
        o_message := 'تم إنشاء المستخدم بنجاح';

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END CREATE_USER;

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
    ) IS
        v_password_hash VARCHAR2(256);
        v_stored_hash VARCHAR2(256);
        v_is_active NUMBER;
        v_failed_attempts NUMBER;
        v_locked_until TIMESTAMP;
    BEGIN
        -- تشفير كلمة المرور المدخلة
        v_password_hash := HASH_PASSWORD(p_password);

        -- جلب بيانات المستخدم
        BEGIN
            SELECT USER_ID, EMPLOYEE_ID, PASSWORD_HASH, IS_ACTIVE, 
                   FAILED_LOGIN_ATTEMPTS, ACCOUNT_LOCKED_UNTIL
            INTO o_user_id, o_employee_id, v_stored_hash, v_is_active,
                 v_failed_attempts, v_locked_until
            FROM HR_CORE.SYSTEM_USERS
            WHERE UPPER(USERNAME) = UPPER(p_username);
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                o_is_success := 0;
                o_message := 'اسم المستخدم أو كلمة المرور غير صحيحة';
                RETURN;
        END;

        -- التحقق من حالة الحساب
        IF v_is_active = 0 THEN
            o_is_success := 0;
            o_message := 'الحساب معطل. يرجى الاتصال بالمسؤول';
            RETURN;
        END IF;

        -- التحقق من القفل
        IF v_locked_until IS NOT NULL AND v_locked_until > SYSTIMESTAMP THEN
            o_is_success := 0;
            o_message := 'الحساب مقفل حتى ' || TO_CHAR(v_locked_until, 'YYYY-MM-DD HH24:MI');
            RETURN;
        END IF;

        -- التحقق من كلمة المرور
        IF v_password_hash = v_stored_hash THEN
            -- نجح تسجيل الدخول
            UPDATE HR_CORE.SYSTEM_USERS
            SET LAST_LOGIN = SYSTIMESTAMP,
                FAILED_LOGIN_ATTEMPTS = 0,
                ACCOUNT_LOCKED_UNTIL = NULL
            WHERE USER_ID = o_user_id;

            o_is_success := 1;
            o_message := 'تم تسجيل الدخول بنجاح';
        ELSE
            -- فشل تسجيل الدخول
            v_failed_attempts := v_failed_attempts + 1;

            IF v_failed_attempts >= 5 THEN
                -- قفل الحساب لمدة 30 دقيقة
                UPDATE HR_CORE.SYSTEM_USERS
                SET FAILED_LOGIN_ATTEMPTS = v_failed_attempts,
                    ACCOUNT_LOCKED_UNTIL = SYSTIMESTAMP + INTERVAL '30' MINUTE
                WHERE USER_ID = o_user_id;

                o_message := 'تم قفل الحساب لمدة 30 دقيقة بسبب المحاولات الفاشلة';
            ELSE
                UPDATE HR_CORE.SYSTEM_USERS
                SET FAILED_LOGIN_ATTEMPTS = v_failed_attempts
                WHERE USER_ID = o_user_id;

                o_message := 'كلمة المرور غير صحيحة. المحاولات المتبقية: ' || (5 - v_failed_attempts);
            END IF;

            o_is_success := 0;
        END IF;

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_is_success := 0;
            o_message := 'خطأ: ' || SQLERRM;
    END LOGIN;

    -- ==========================================================
    -- إجراء: تغيير كلمة المرور
    -- ==========================================================
    PROCEDURE CHANGE_PASSWORD (
        p_user_id           IN NUMBER,
        p_old_password      IN VARCHAR2,
        p_new_password      IN VARCHAR2,
        o_message           OUT VARCHAR2
    ) IS
        v_old_hash VARCHAR2(256);
        v_stored_hash VARCHAR2(256);
        v_new_hash VARCHAR2(256);
    BEGIN
        -- التحقق من قوة كلمة المرور الجديدة
        IF LENGTH(p_new_password) < 8 THEN
            RAISE_APPLICATION_ERROR(-20003, 'كلمة المرور الجديدة يجب أن تكون 8 أحرف على الأقل');
        END IF;

        -- جلب كلمة المرور الحالية
        SELECT PASSWORD_HASH INTO v_stored_hash
        FROM HR_CORE.SYSTEM_USERS
        WHERE USER_ID = p_user_id;

        -- التحقق من كلمة المرور القديمة
        v_old_hash := HASH_PASSWORD(p_old_password);

        IF v_old_hash != v_stored_hash THEN
            RAISE_APPLICATION_ERROR(-20004, 'كلمة المرور القديمة غير صحيحة');
        END IF;

        -- تشفير كلمة المرور الجديدة
        v_new_hash := HASH_PASSWORD(p_new_password);

        -- تحديث كلمة المرور
        UPDATE HR_CORE.SYSTEM_USERS
        SET PASSWORD_HASH = v_new_hash,
            UPDATED_AT = SYSTIMESTAMP
        WHERE USER_ID = p_user_id;

        o_message := 'تم تغيير كلمة المرور بنجاح';

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END CHANGE_PASSWORD;

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
    ) IS
        v_role_id NUMBER;
    BEGIN
        INSERT INTO HR_CORE.SYSTEM_ROLES (
            ROLE_NAME, ROLE_NAME_AR, DESCRIPTION, CREATED_BY
        ) VALUES (
            p_role_name, p_role_name_ar, p_description, p_created_by
        ) RETURNING ROLE_ID INTO v_role_id;

        o_role_id := v_role_id;
        o_message := 'تم إنشاء الدور بنجاح';

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END CREATE_ROLE;

    -- ==========================================================
    -- إجراء: منح دور لمستخدم
    -- ==========================================================
    PROCEDURE ASSIGN_ROLE_TO_USER (
        p_user_id           IN NUMBER,
        p_role_id           IN NUMBER,
        p_assigned_by       IN VARCHAR2,
        o_message           OUT VARCHAR2
    ) IS
    BEGIN
        INSERT INTO HR_CORE.USER_ROLES (
            USER_ID, ROLE_ID, ASSIGNED_BY, CREATED_BY
        ) VALUES (
            p_user_id, p_role_id, p_assigned_by, p_assigned_by
        );

        o_message := 'تم منح الدور للمستخدم بنجاح';

        COMMIT;

    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
            o_message := 'المستخدم لديه هذا الدور بالفعل';
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END ASSIGN_ROLE_TO_USER;

    -- ==========================================================
    -- إجراء: منح صلاحية لدور
    -- ==========================================================
    PROCEDURE ASSIGN_PERMISSION_TO_ROLE (
        p_role_id           IN NUMBER,
        p_permission_id     IN NUMBER,
        p_created_by        IN VARCHAR2,
        o_message           OUT VARCHAR2
    ) IS
    BEGIN
        INSERT INTO HR_CORE.ROLE_PERMISSIONS (
            ROLE_ID, PERMISSION_ID, CREATED_BY
        ) VALUES (
            p_role_id, p_permission_id, p_created_by
        );

        o_message := 'تم منح الصلاحية للدور بنجاح';

        COMMIT;

    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
            o_message := 'الدور لديه هذه الصلاحية بالفعل';
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END ASSIGN_PERMISSION_TO_ROLE;

    -- ==========================================================
    -- دالة: التحقق من صلاحية المستخدم
    -- ==========================================================
    FUNCTION CHECK_USER_PERMISSION (
        p_user_id           IN NUMBER,
        p_permission_name   IN VARCHAR2
    ) RETURN NUMBER IS
        v_count NUMBER;
    BEGIN
        SELECT COUNT(*)
        INTO v_count
        FROM HR_CORE.USER_ROLES ur
        JOIN HR_CORE.ROLE_PERMISSIONS rp ON ur.ROLE_ID = rp.ROLE_ID
        JOIN HR_CORE.SYSTEM_PERMISSIONS sp ON rp.PERMISSION_ID = sp.PERMISSION_ID
        WHERE ur.USER_ID = p_user_id
          AND sp.PERMISSION_NAME = p_permission_name;

        RETURN CASE WHEN v_count > 0 THEN 1 ELSE 0 END;
    END;

    -- ==========================================================
    -- إجراء: تعطيل/تفعيل مستخدم
    -- ==========================================================
    PROCEDURE TOGGLE_USER_STATUS (
        p_user_id           IN NUMBER,
        p_is_active         IN NUMBER,
        p_updated_by        IN VARCHAR2,
        o_message           OUT VARCHAR2
    ) IS
    BEGIN
        UPDATE HR_CORE.SYSTEM_USERS
        SET IS_ACTIVE = p_is_active,
            UPDATED_BY = p_updated_by,
            UPDATED_AT = SYSTIMESTAMP
        WHERE USER_ID = p_user_id;

        o_message := CASE WHEN p_is_active = 1 THEN 'تم تفعيل المستخدم' ELSE 'تم تعطيل المستخدم' END;

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END TOGGLE_USER_STATUS;

END PKG_SECURITY_MANAGER;
/
