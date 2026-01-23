-- =================================================================================
-- الحزمة: PKG_PERFORMANCE_MANAGER
-- الوصف: تنفيذ منطق إدارة الأداء (Body)
-- المخطط: HR_PERFORMANCE
-- =================================================================================

CREATE OR REPLACE PACKAGE BODY HR_PERFORMANCE.PKG_PERFORMANCE_MANAGER AS

    -- ==========================================================
    -- إجراء: إنشاء تقييم سنوي
    -- ==========================================================
    PROCEDURE CREATE_APPRAISAL (
        p_employee_id       IN NUMBER,
        p_cycle_id          IN NUMBER,
        p_evaluator_id      IN NUMBER,
        p_created_by        IN VARCHAR2,
        o_appraisal_id      OUT NUMBER,
        o_message           OUT VARCHAR2
    ) IS
        v_appraisal_id NUMBER;
        v_emp_status VARCHAR2(20);
        v_cycle_active NUMBER;
        v_existing_count NUMBER;
    BEGIN
        -- التحقق من حالة الموظف
        SELECT STATUS INTO v_emp_status
        FROM HR_PERSONNEL.EMPLOYEES
        WHERE EMPLOYEE_ID = p_employee_id;

        IF v_emp_status != 'ACTIVE' THEN
            RAISE_APPLICATION_ERROR(-20801, 'الموظف غير نشط');
        END IF;

        -- التحقق من أن الدورة نشطة
        SELECT IS_ACTIVE INTO v_cycle_active
        FROM HR_PERFORMANCE.APPRAISAL_CYCLES
        WHERE CYCLE_ID = p_cycle_id;

        IF v_cycle_active != 1 THEN
            RAISE_APPLICATION_ERROR(-20802, 'دورة التقييم غير نشطة');
        END IF;

        -- التحقق من عدم وجود تقييم سابق لنفس الدورة
        SELECT COUNT(*) INTO v_existing_count
        FROM HR_PERFORMANCE.EMPLOYEE_APPRAISALS
        WHERE EMPLOYEE_ID = p_employee_id
          AND CYCLE_ID = p_cycle_id;

        IF v_existing_count > 0 THEN
            RAISE_APPLICATION_ERROR(-20803, 'يوجد تقييم سابق لهذا الموظف في نفس الدورة');
        END IF;

        -- إنشاء التقييم
        INSERT INTO HR_PERFORMANCE.EMPLOYEE_APPRAISALS (
            EMPLOYEE_ID, CYCLE_ID, EVALUATOR_ID, APPRAISAL_DATE,
            STATUS, CREATED_BY
        ) VALUES (
            p_employee_id, p_cycle_id, p_evaluator_id, SYSDATE,
            'DRAFT', p_created_by
        ) RETURNING APPRAISAL_ID INTO v_appraisal_id;

        o_appraisal_id := v_appraisal_id;
        o_message := 'تم إنشاء التقييم بنجاح. رقم التقييم: ' || v_appraisal_id;

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END CREATE_APPRAISAL;

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
    ) IS
        v_detail_id NUMBER;
        v_score NUMBER;
        v_achievement_percentage NUMBER;
    BEGIN
        -- حساب نسبة الإنجاز
        IF p_target_value > 0 THEN
            v_achievement_percentage := (p_actual_value / p_target_value) * 100;
        ELSE
            v_achievement_percentage := 0;
        END IF;

        -- حساب الدرجة (من 100)
        -- يمكن تخصيص المعادلة حسب الحاجة
        IF v_achievement_percentage >= 100 THEN
            v_score := 100;
        ELSIF v_achievement_percentage >= 90 THEN
            v_score := 90;
        ELSIF v_achievement_percentage >= 80 THEN
            v_score := 80;
        ELSIF v_achievement_percentage >= 70 THEN
            v_score := 70;
        ELSIF v_achievement_percentage >= 60 THEN
            v_score := 60;
        ELSE
            v_score := v_achievement_percentage;
        END IF;

        -- إدخال التفاصيل
        INSERT INTO HR_PERFORMANCE.APPRAISAL_DETAILS (
            APPRAISAL_ID, KPI_ID, TARGET_VALUE, ACTUAL_VALUE,
            SCORE, COMMENTS, CREATED_BY
        ) VALUES (
            p_appraisal_id, p_kpi_id, p_target_value, p_actual_value,
            v_score, p_comments, p_created_by
        ) RETURNING DETAIL_ID INTO v_detail_id;

        o_detail_id := v_detail_id;
        o_score := v_score;
        o_message := 'تم إضافة التقييم. الدرجة: ' || v_score || ' (إنجاز: ' || ROUND(v_achievement_percentage, 2) || '%)';

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END ADD_KPI_SCORE;

    -- ==========================================================
    -- دالة: حساب الدرجة النهائية
    -- ==========================================================
    FUNCTION CALCULATE_FINAL_SCORE (
        p_appraisal_id      IN NUMBER
    ) RETURN NUMBER IS
        v_avg_score NUMBER;
    BEGIN
        SELECT AVG(SCORE)
        INTO v_avg_score
        FROM HR_PERFORMANCE.APPRAISAL_DETAILS
        WHERE APPRAISAL_ID = p_appraisal_id;

        RETURN ROUND(NVL(v_avg_score, 0), 2);
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            RETURN 0;
    END;

    -- ==========================================================
    -- إجراء: اعتماد التقييم
    -- ==========================================================
    PROCEDURE APPROVE_APPRAISAL (
        p_appraisal_id      IN NUMBER,
        p_approved_by       IN VARCHAR2,
        o_final_score       OUT NUMBER,
        o_grade             OUT VARCHAR2,
        o_message           OUT VARCHAR2
    ) IS
        v_final_score NUMBER;
        v_grade VARCHAR2(20);
        v_status VARCHAR2(20);
    BEGIN
        -- التحقق من الحالة
        SELECT STATUS INTO v_status
        FROM HR_PERFORMANCE.EMPLOYEE_APPRAISALS
        WHERE APPRAISAL_ID = p_appraisal_id;

        IF v_status NOT IN ('DRAFT', 'SUBMITTED') THEN
            RAISE_APPLICATION_ERROR(-20804, 'لا يمكن اعتماد تقييم في هذه الحالة');
        END IF;

        -- حساب الدرجة النهائية
        v_final_score := CALCULATE_FINAL_SCORE(p_appraisal_id);

        -- تحديد التقدير
        IF v_final_score >= 90 THEN
            v_grade := 'ممتاز';
        ELSIF v_final_score >= 80 THEN
            v_grade := 'جيد جداً';
        ELSIF v_final_score >= 70 THEN
            v_grade := 'جيد';
        ELSIF v_final_score >= 60 THEN
            v_grade := 'مقبول';
        ELSE
            v_grade := 'ضعيف';
        END IF;

        -- تحديث التقييم
        UPDATE HR_PERFORMANCE.EMPLOYEE_APPRAISALS
        SET FINAL_SCORE = v_final_score,
            GRADE = v_grade,
            STATUS = 'APPROVED',
            UPDATED_BY = p_approved_by,
            UPDATED_AT = SYSDATE
        WHERE APPRAISAL_ID = p_appraisal_id;

        o_final_score := v_final_score;
        o_grade := v_grade;
        o_message := 'تم اعتماد التقييم. الدرجة: ' || v_final_score || ' (' || v_grade || ')';

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END APPROVE_APPRAISAL;

    -- ==========================================================
    -- دالة: حساب عدد المخالفات
    -- ==========================================================
    FUNCTION GET_VIOLATION_COUNT (
        p_employee_id       IN NUMBER,
        p_from_date         IN DATE,
        p_to_date           IN DATE,
        p_violation_type_id IN NUMBER DEFAULT NULL
    ) RETURN NUMBER IS
        v_count NUMBER;
    BEGIN
        SELECT COUNT(*)
        INTO v_count
        FROM HR_PERFORMANCE.EMPLOYEE_VIOLATIONS
        WHERE EMPLOYEE_ID = p_employee_id
          AND VIOLATION_DATE BETWEEN p_from_date AND p_to_date
          AND (p_violation_type_id IS NULL OR VIOLATION_TYPE_ID = p_violation_type_id)
          AND STATUS != 'CANCELLED';

        RETURN v_count;
    END;

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
    ) IS
        v_violation_id NUMBER;
        v_emp_status VARCHAR2(20);
        v_severity_level NUMBER;
        v_violation_count NUMBER;
    BEGIN
        -- التحقق من حالة الموظف
        SELECT STATUS INTO v_emp_status
        FROM HR_PERSONNEL.EMPLOYEES
        WHERE EMPLOYEE_ID = p_employee_id;

        IF v_emp_status != 'ACTIVE' THEN
            RAISE_APPLICATION_ERROR(-20901, 'الموظف غير نشط');
        END IF;

        -- جلب مستوى خطورة المخالفة
        SELECT SEVERITY_LEVEL INTO v_severity_level
        FROM HR_PERFORMANCE.VIOLATION_TYPES
        WHERE VIOLATION_TYPE_ID = p_violation_type_id;

        -- حساب عدد المخالفات المماثلة في آخر سنة
        v_violation_count := GET_VIOLATION_COUNT(
            p_employee_id,
            ADD_MONTHS(p_violation_date, -12),
            p_violation_date,
            p_violation_type_id
        );

        -- تسجيل المخالفة
        INSERT INTO HR_PERFORMANCE.EMPLOYEE_VIOLATIONS (
            EMPLOYEE_ID, VIOLATION_TYPE_ID, VIOLATION_DATE,
            DESCRIPTION, ACTION_ID, STATUS, CREATED_BY
        ) VALUES (
            p_employee_id, p_violation_type_id, p_violation_date,
            p_description, p_action_id, 'PENDING', p_created_by
        ) RETURNING VIOLATION_ID INTO v_violation_id;

        o_violation_id := v_violation_id;
        o_message := 'تم تسجيل المخالفة. العدد في آخر سنة: ' || (v_violation_count + 1);

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END RECORD_VIOLATION;

    -- ==========================================================
    -- إجراء: تنفيذ الجزاء
    -- ==========================================================
    PROCEDURE EXECUTE_DISCIPLINARY_ACTION (
        p_violation_id      IN NUMBER,
        p_executed_by       IN VARCHAR2,
        o_deduction_days    OUT NUMBER,
        o_message           OUT VARCHAR2
    ) IS
        v_action_id NUMBER;
        v_deduction_days NUMBER;
        v_is_termination NUMBER;
        v_employee_id NUMBER;
        v_status VARCHAR2(20);
    BEGIN
        -- جلب بيانات المخالفة
        SELECT ACTION_ID, EMPLOYEE_ID, STATUS
        INTO v_action_id, v_employee_id, v_status
        FROM HR_PERFORMANCE.EMPLOYEE_VIOLATIONS
        WHERE VIOLATION_ID = p_violation_id;

        IF v_status != 'APPROVED' THEN
            RAISE_APPLICATION_ERROR(-20902, 'يجب اعتماد المخالفة أولاً');
        END IF;

        IF v_action_id IS NULL THEN
            RAISE_APPLICATION_ERROR(-20903, 'لم يتم تحديد إجراء تأديبي');
        END IF;

        -- جلب تفاصيل الإجراء
        SELECT DEDUCTION_DAYS, IS_TERMINATION
        INTO v_deduction_days, v_is_termination
        FROM HR_PERFORMANCE.DISCIPLINARY_ACTIONS
        WHERE ACTION_ID = v_action_id;

        -- تنفيذ الإجراء
        IF v_is_termination = 1 THEN
            -- إنهاء خدمة الموظف
            UPDATE HR_PERSONNEL.EMPLOYEES
            SET STATUS = 'TERMINATED',
                UPDATED_BY = p_executed_by,
                UPDATED_AT = SYSDATE
            WHERE EMPLOYEE_ID = v_employee_id;

            o_message := 'تم إنهاء خدمة الموظف';
        ELSE
            -- خصم أيام من الراتب (يمكن ربطه بنظام الرواتب)
            o_message := 'تم تطبيق خصم ' || v_deduction_days || ' يوم';
        END IF;

        -- تحديث حالة المخالفة
        UPDATE HR_PERFORMANCE.EMPLOYEE_VIOLATIONS
        SET IS_EXECUTED = 1,
            STATUS = 'APPROVED',
            UPDATED_BY = p_executed_by,
            UPDATED_AT = SYSDATE
        WHERE VIOLATION_ID = p_violation_id;

        o_deduction_days := v_deduction_days;

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END EXECUTE_DISCIPLINARY_ACTION;

END PKG_PERFORMANCE_MANAGER;
/
