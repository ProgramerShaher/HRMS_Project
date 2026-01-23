-- =================================================================================
-- الحزمة: PKG_ATTENDANCE_MANAGER
-- الوصف: تنفيذ منطق إدارة الحضور (Body)
-- المخطط: HR_ATTENDANCE
-- =================================================================================

CREATE OR REPLACE PACKAGE BODY HR_ATTENDANCE.PKG_ATTENDANCE_MANAGER AS

    -- ==========================================================
    -- دالة: حساب دقائق التأخير
    -- ==========================================================
    FUNCTION CALCULATE_LATE_MINUTES (
        p_planned_time      IN TIMESTAMP,
        p_actual_time       IN TIMESTAMP,
        p_grace_period      IN NUMBER DEFAULT 15
    ) RETURN NUMBER IS
        v_diff_minutes NUMBER;
    BEGIN
        IF p_actual_time IS NULL OR p_planned_time IS NULL THEN
            RETURN 0;
        END IF;

        -- حساب الفرق بالدقائق
        v_diff_minutes := EXTRACT(DAY FROM (p_actual_time - p_planned_time)) * 24 * 60 +
                          EXTRACT(HOUR FROM (p_actual_time - p_planned_time)) * 60 +
                          EXTRACT(MINUTE FROM (p_actual_time - p_planned_time));

        -- إذا كان الفرق أقل من فترة السماح، لا يوجد تأخير
        IF v_diff_minutes <= p_grace_period THEN
            RETURN 0;
        END IF;

        RETURN v_diff_minutes - p_grace_period;
    END;

    -- ==========================================================
    -- دالة: حساب ساعات العمل الفعلية
    -- ==========================================================
    FUNCTION CALCULATE_WORKING_HOURS (
        p_in_time           IN TIMESTAMP,
        p_out_time          IN TIMESTAMP
    ) RETURN NUMBER IS
        v_hours NUMBER;
    BEGIN
        IF p_in_time IS NULL OR p_out_time IS NULL THEN
            RETURN 0;
        END IF;

        v_hours := EXTRACT(DAY FROM (p_out_time - p_in_time)) * 24 +
                   EXTRACT(HOUR FROM (p_out_time - p_in_time)) +
                   EXTRACT(MINUTE FROM (p_out_time - p_in_time)) / 60;
        
        RETURN GREATEST(v_hours, 0);
    END;

    -- ==========================================================
    -- دالة: الحصول على حالة الحضور
    -- ==========================================================
    FUNCTION GET_ATTENDANCE_STATUS (
        p_employee_id       IN NUMBER,
        p_attendance_date   IN DATE
    ) RETURN VARCHAR2 IS
        v_status VARCHAR2(20);
    BEGIN
        SELECT STATUS INTO v_status
        FROM HR_ATTENDANCE.DAILY_ATTENDANCE
        WHERE EMPLOYEE_ID = p_employee_id
          AND ATTENDANCE_DATE = TRUNC(p_attendance_date);

        RETURN v_status;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            RETURN 'NOT_RECORDED';
    END;

    -- ==========================================================
    -- إجراء: معالجة سجلات البصمة اليومية
    -- ==========================================================
    PROCEDURE PROCESS_DAILY_PUNCHES (
        p_attendance_date   IN DATE,
        p_employee_id       IN NUMBER DEFAULT NULL,
        p_created_by        IN VARCHAR2,
        o_records_processed OUT NUMBER,
        o_message           OUT VARCHAR2
    ) IS
        v_count NUMBER := 0;
        v_first_in TIMESTAMP;
        v_last_out TIMESTAMP;
        v_planned_shift_id NUMBER;
        v_shift_start VARCHAR2(5);
        v_shift_end VARCHAR2(5);
        v_planned_in TIMESTAMP;
        v_planned_out TIMESTAMP;
        v_late_mins NUMBER;
        v_early_leave_mins NUMBER;
        v_overtime_mins NUMBER;
        v_working_hours NUMBER;
        v_status VARCHAR2(20);
    BEGIN
        FOR emp IN (
            SELECT DISTINCT e.EMPLOYEE_ID, r.SHIFT_ID
            FROM HR_PERSONNEL.EMPLOYEES e
            LEFT JOIN HR_ATTENDANCE.EMPLOYEE_ROSTERS r 
                ON e.EMPLOYEE_ID = r.EMPLOYEE_ID 
                AND r.ROSTER_DATE = TRUNC(p_attendance_date)
            WHERE e.STATUS = 'ACTIVE'
              AND (p_employee_id IS NULL OR e.EMPLOYEE_ID = p_employee_id)
        ) LOOP
            BEGIN
                -- جلب أول دخول وآخر خروج من سجلات البصمة
                SELECT 
                    MIN(CASE WHEN PUNCH_TYPE = 'IN' THEN PUNCH_TIME END),
                    MAX(CASE WHEN PUNCH_TYPE = 'OUT' THEN PUNCH_TIME END)
                INTO v_first_in, v_last_out
                FROM HR_ATTENDANCE.RAW_PUNCH_LOGS
                WHERE EMPLOYEE_ID = emp.EMPLOYEE_ID
                  AND TRUNC(PUNCH_TIME) = TRUNC(p_attendance_date)
                  AND IS_PROCESSED = 0;

                -- تحديد الحالة
                IF v_first_in IS NULL AND v_last_out IS NULL THEN
                    -- لم يسجل حضور
                    v_status := 'ABSENT';
                    v_late_mins := 0;
                    v_early_leave_mins := 0;
                    v_overtime_mins := 0;
                ELSIF v_first_in IS NOT NULL AND v_last_out IS NULL THEN
                    -- دخول بدون خروج
                    v_status := 'MISSING_PUNCH';
                    v_late_mins := 0;
                    v_early_leave_mins := 0;
                    v_overtime_mins := 0;
                ELSE
                    -- حضور كامل
                    v_status := 'PRESENT';

                    -- جلب معلومات المناوبة المخططة
                    IF emp.SHIFT_ID IS NOT NULL THEN
                        SELECT START_TIME, END_TIME, GRACE_PERIOD_MINS
                        INTO v_shift_start, v_shift_end, v_late_mins
                        FROM HR_ATTENDANCE.SHIFT_TYPES
                        WHERE SHIFT_ID = emp.SHIFT_ID;

                        -- بناء أوقات المناوبة المخططة
                        v_planned_in := TO_TIMESTAMP(
                            TO_CHAR(p_attendance_date, 'YYYY-MM-DD') || ' ' || v_shift_start || ':00',
                            'YYYY-MM-DD HH24:MI:SS'
                        );
                        v_planned_out := TO_TIMESTAMP(
                            TO_CHAR(p_attendance_date, 'YYYY-MM-DD') || ' ' || v_shift_end || ':00',
                            'YYYY-MM-DD HH24:MI:SS'
                        );

                        -- حساب التأخير
                        v_late_mins := CALCULATE_LATE_MINUTES(v_planned_in, v_first_in, v_late_mins);

                        -- حساب المغادرة المبكرة
                        IF v_last_out < v_planned_out THEN
                            v_early_leave_mins := EXTRACT(DAY FROM (v_planned_out - v_last_out)) * 24 * 60 +
                                                  EXTRACT(HOUR FROM (v_planned_out - v_last_out)) * 60 +
                                                  EXTRACT(MINUTE FROM (v_planned_out - v_last_out));
                        ELSE
                            v_early_leave_mins := 0;
                        END IF;

                        -- حساب الساعات الإضافية
                        v_working_hours := CALCULATE_WORKING_HOURS(v_first_in, v_last_out);
                        v_overtime_mins := GREATEST(0, ROUND((v_working_hours - 8) * 60));
                    ELSE
                        v_late_mins := 0;
                        v_early_leave_mins := 0;
                        v_overtime_mins := 0;
                    END IF;
                END IF;

                -- إدخال أو تحديث سجل الحضور اليومي
                MERGE INTO HR_ATTENDANCE.DAILY_ATTENDANCE da
                USING (SELECT emp.EMPLOYEE_ID AS EID, TRUNC(p_attendance_date) AS ADATE FROM DUAL) src
                ON (da.EMPLOYEE_ID = src.EID AND da.ATTENDANCE_DATE = src.ADATE)
                WHEN MATCHED THEN
                    UPDATE SET
                        PLANNED_SHIFT_ID = emp.SHIFT_ID,
                        ACTUAL_IN_TIME = v_first_in,
                        ACTUAL_OUT_TIME = v_last_out,
                        LATE_MINUTES = v_late_mins,
                        EARLY_LEAVE_MINUTES = v_early_leave_mins,
                        OVERTIME_MINUTES = v_overtime_mins,
                        STATUS = v_status,
                        UPDATED_BY = p_created_by,
                        UPDATED_AT = SYSDATE
                WHEN NOT MATCHED THEN
                    INSERT (
                        EMPLOYEE_ID, ATTENDANCE_DATE, PLANNED_SHIFT_ID,
                        ACTUAL_IN_TIME, ACTUAL_OUT_TIME, LATE_MINUTES,
                        EARLY_LEAVE_MINUTES, OVERTIME_MINUTES, STATUS, CREATED_BY
                    ) VALUES (
                        emp.EMPLOYEE_ID, TRUNC(p_attendance_date), emp.SHIFT_ID,
                        v_first_in, v_last_out, v_late_mins,
                        v_early_leave_mins, v_overtime_mins, v_status, p_created_by
                    );

                -- تحديث سجلات البصمة كمعالجة
                UPDATE HR_ATTENDANCE.RAW_PUNCH_LOGS
                SET IS_PROCESSED = 1
                WHERE EMPLOYEE_ID = emp.EMPLOYEE_ID
                  AND TRUNC(PUNCH_TIME) = TRUNC(p_attendance_date);

                v_count := v_count + 1;

            EXCEPTION
                WHEN OTHERS THEN
                    -- تسجيل الخطأ ومتابعة باقي الموظفين
                    DBMS_OUTPUT.PUT_LINE('خطأ في معالجة الموظف ' || emp.EMPLOYEE_ID || ': ' || SQLERRM);
            END;
        END LOOP;

        o_records_processed := v_count;
        o_message := 'تم معالجة ' || v_count || ' سجل حضور';

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END PROCESS_DAILY_PUNCHES;

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
    ) IS
        v_request_id NUMBER;
        v_emp_status VARCHAR2(20);
    BEGIN
        -- التحقق من حالة الموظف
        SELECT STATUS INTO v_emp_status
        FROM HR_PERSONNEL.EMPLOYEES
        WHERE EMPLOYEE_ID = p_employee_id;

        IF v_emp_status != 'ACTIVE' THEN
            RAISE_APPLICATION_ERROR(-20701, 'الموظف غير نشط');
        END IF;

        -- التحقق من عدد الساعات
        IF p_hours_requested <= 0 OR p_hours_requested > 12 THEN
            RAISE_APPLICATION_ERROR(-20702, 'عدد الساعات غير صحيح (يجب أن يكون بين 1 و 12)');
        END IF;

        -- إنشاء الطلب
        INSERT INTO HR_ATTENDANCE.OVERTIME_REQUESTS (
            EMPLOYEE_ID, REQUEST_DATE, WORK_DATE, HOURS_REQUESTED,
            REASON, STATUS, CREATED_BY
        ) VALUES (
            p_employee_id, SYSDATE, p_work_date, p_hours_requested,
            p_reason, 'PENDING', p_created_by
        ) RETURNING OT_REQUEST_ID INTO v_request_id;

        o_request_id := v_request_id;
        o_message := 'تم تقديم طلب الساعات الإضافية بنجاح';

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END REQUEST_OVERTIME;

    -- ==========================================================
    -- إجراء: اعتماد طلب ساعات إضافية
    -- ==========================================================
    PROCEDURE APPROVE_OVERTIME (
        p_request_id        IN NUMBER,
        p_approved_hours    IN NUMBER,
        p_approved_by       IN NUMBER,
        o_message           OUT VARCHAR2
    ) IS
        v_status VARCHAR2(20);
    BEGIN
        SELECT STATUS INTO v_status
        FROM HR_ATTENDANCE.OVERTIME_REQUESTS
        WHERE OT_REQUEST_ID = p_request_id;

        IF v_status != 'PENDING' THEN
            RAISE_APPLICATION_ERROR(-20703, 'لا يمكن اعتماد طلب في هذه الحالة');
        END IF;

        UPDATE HR_ATTENDANCE.OVERTIME_REQUESTS
        SET STATUS = 'APPROVED',
            APPROVED_BY = p_approved_by,
            APPROVED_HOURS = p_approved_hours,
            UPDATED_BY = TO_CHAR(p_approved_by),
            UPDATED_AT = SYSDATE
        WHERE OT_REQUEST_ID = p_request_id;

        o_message := 'تم اعتماد ' || p_approved_hours || ' ساعة إضافية';

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END APPROVE_OVERTIME;

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
    ) IS
        v_count NUMBER := 0;
        v_start_date DATE;
        v_end_date DATE;
        v_current_date DATE;
        v_day_of_week VARCHAR2(10);
    BEGIN
        -- حساب بداية ونهاية الشهر
        v_start_date := TO_DATE(p_year || '-' || LPAD(p_month, 2, '0') || '-01', 'YYYY-MM-DD');
        v_end_date := LAST_DAY(v_start_date);

        -- حذف الجدول القديم إن وجد
        DELETE FROM HR_ATTENDANCE.EMPLOYEE_ROSTERS
        WHERE ROSTER_DATE BETWEEN v_start_date AND v_end_date
          AND (p_dept_id IS NULL OR EMPLOYEE_ID IN (
              SELECT EMPLOYEE_ID FROM HR_PERSONNEL.EMPLOYEES WHERE DEPT_ID = p_dept_id
          ));

        -- إنشاء الجدول لكل يوم
        v_current_date := v_start_date;

        WHILE v_current_date <= v_end_date LOOP
            v_day_of_week := TO_CHAR(v_current_date, 'DY', 'NLS_DATE_LANGUAGE=ENGLISH');

            FOR emp IN (
                SELECT EMPLOYEE_ID
                FROM HR_PERSONNEL.EMPLOYEES
                WHERE STATUS = 'ACTIVE'
                  AND (p_dept_id IS NULL OR DEPT_ID = p_dept_id)
            ) LOOP
                -- إذا كان يوم عطلة (الجمعة والسبت)
                IF v_day_of_week IN ('FRI', 'SAT') THEN
                    INSERT INTO HR_ATTENDANCE.EMPLOYEE_ROSTERS (
                        EMPLOYEE_ID, ROSTER_DATE, SHIFT_ID, IS_OFF_DAY, CREATED_BY
                    ) VALUES (
                        emp.EMPLOYEE_ID, v_current_date, NULL, 1, p_created_by
                    );
                ELSE
                    INSERT INTO HR_ATTENDANCE.EMPLOYEE_ROSTERS (
                        EMPLOYEE_ID, ROSTER_DATE, SHIFT_ID, IS_OFF_DAY, CREATED_BY
                    ) VALUES (
                        emp.EMPLOYEE_ID, v_current_date, p_default_shift_id, 0, p_created_by
                    );
                END IF;

                v_count := v_count + 1;
            END LOOP;

            v_current_date := v_current_date + 1;
        END LOOP;

        o_records_created := v_count;
        o_message := 'تم إنشاء ' || v_count || ' سجل في جدول العمل';

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END GENERATE_MONTHLY_ROSTER;

    -- ==========================================================
    -- إجراء: تصحيح سجل حضور
    -- ==========================================================
    PROCEDURE CORRECT_ATTENDANCE (
        p_record_id         IN NUMBER,
        p_actual_in_time    IN TIMESTAMP,
        p_actual_out_time   IN TIMESTAMP,
        p_reason            IN VARCHAR2,
        p_corrected_by      IN VARCHAR2,
        o_message           OUT VARCHAR2
    ) IS
        v_working_hours NUMBER;
    BEGIN
        v_working_hours := CALCULATE_WORKING_HOURS(p_actual_in_time, p_actual_out_time);

        UPDATE HR_ATTENDANCE.DAILY_ATTENDANCE
        SET ACTUAL_IN_TIME = p_actual_in_time,
            ACTUAL_OUT_TIME = p_actual_out_time,
            STATUS = 'PRESENT',
            LATE_MINUTES = 0, -- يمكن إعادة الحساب
            EARLY_LEAVE_MINUTES = 0,
            UPDATED_BY = p_corrected_by,
            UPDATED_AT = SYSDATE
        WHERE RECORD_ID = p_record_id;

        o_message := 'تم تصحيح سجل الحضور. السبب: ' || p_reason;

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END CORRECT_ATTENDANCE;

END PKG_ATTENDANCE_MANAGER;
/
