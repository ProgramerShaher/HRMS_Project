-- =================================================================================
-- الحزمة: PKG_LEAVE_MANAGER
-- الوصف: تنفيذ منطق إدارة الإجازات (Body)
-- المخطط: HR_LEAVES
-- =================================================================================

CREATE OR REPLACE PACKAGE BODY HR_LEAVES.PKG_LEAVE_MANAGER AS

    -- ==========================================================
    -- دالة خاصة: التحقق من وجود تعارض في التواريخ
    -- ==========================================================
    FUNCTION CHECK_DATE_CONFLICT (
        p_employee_id   IN NUMBER,
        p_start_date    IN DATE,
        p_end_date      IN DATE,
        p_exclude_req_id IN NUMBER DEFAULT NULL
    ) RETURN NUMBER IS
        v_count NUMBER;
    BEGIN
        SELECT COUNT(*)
        INTO v_count
        FROM HR_LEAVES.LEAVE_REQUESTS
        WHERE EMPLOYEE_ID = p_employee_id
          AND STATUS IN ('PENDING', 'MANAGER_APPROVED', 'HR_APPROVED')
          AND (p_exclude_req_id IS NULL OR REQUEST_ID != p_exclude_req_id)
          AND (
              (p_start_date BETWEEN START_DATE AND END_DATE) OR
              (p_end_date BETWEEN START_DATE AND END_DATE) OR
              (START_DATE BETWEEN p_start_date AND p_end_date)
          );
        
        RETURN v_count;
    END;

    -- ==========================================================
    -- دالة: حساب عدد أيام الإجازة (بدون عطل نهاية الأسبوع)
    -- ==========================================================
    FUNCTION CALCULATE_LEAVE_DAYS (
        p_start_date        IN DATE,
        p_end_date          IN DATE,
        p_exclude_weekends  IN NUMBER DEFAULT 1
    ) RETURN NUMBER IS
        v_days NUMBER := 0;
        v_current_date DATE := p_start_date;
        v_day_of_week VARCHAR2(10);
    BEGIN
        -- التحقق من صحة التواريخ
        IF p_end_date < p_start_date THEN
            RAISE_APPLICATION_ERROR(-20101, 'تاريخ الانتهاء يجب أن يكون بعد تاريخ البداية');
        END IF;

        WHILE v_current_date <= p_end_date LOOP
            v_day_of_week := TO_CHAR(v_current_date, 'DY', 'NLS_DATE_LANGUAGE=ENGLISH');
            
            -- إذا كان استثناء عطلة نهاية الأسبوع مفعّل
            IF p_exclude_weekends = 1 THEN
                -- استثناء الجمعة والسبت (حسب التقويم السعودي)
                IF v_day_of_week NOT IN ('FRI', 'SAT') THEN
                    v_days := v_days + 1;
                END IF;
            ELSE
                v_days := v_days + 1;
            END IF;
            
            v_current_date := v_current_date + 1;
        END LOOP;

        RETURN v_days;
    END;

    -- ==========================================================
    -- دالة: حساب رصيد الإجازات المتبقي
    -- ==========================================================
    FUNCTION GET_LEAVE_BALANCE (
        p_employee_id       IN NUMBER,
        p_leave_type_id     IN NUMBER,
        p_year              IN NUMBER DEFAULT EXTRACT(YEAR FROM SYSDATE)
    ) RETURN NUMBER IS
        v_balance NUMBER := 0;
    BEGIN
        SELECT NVL(CURRENT_BALANCE, 0)
        INTO v_balance
        FROM HR_LEAVES.EMPLOYEE_LEAVE_BALANCES
        WHERE EMPLOYEE_ID = p_employee_id
          AND LEAVE_TYPE_ID = p_leave_type_id
          AND YEAR = p_year;
        
        RETURN v_balance;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            RETURN 0;
    END;

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
    ) IS
        v_request_id NUMBER;
        v_days_count NUMBER;
        v_current_balance NUMBER;
        v_requires_attachment NUMBER;
        v_is_paid NUMBER;
        v_conflict_count NUMBER;
        v_employee_status VARCHAR2(20);
    BEGIN
        -- 1. التحقق من حالة الموظف
        SELECT STATUS INTO v_employee_status
        FROM HR_PERSONNEL.EMPLOYEES
        WHERE EMPLOYEE_ID = p_employee_id;
        
        IF v_employee_status != 'ACTIVE' THEN
            RAISE_APPLICATION_ERROR(-20201, 'لا يمكن طلب إجازة لموظف غير نشط');
        END IF;

        -- 2. حساب عدد الأيام
        v_days_count := CALCULATE_LEAVE_DAYS(p_start_date, p_end_date, 1);
        
        IF v_days_count <= 0 THEN
            RAISE_APPLICATION_ERROR(-20202, 'عدد أيام الإجازة يجب أن يكون أكبر من صفر');
        END IF;

        -- 3. التحقق من تعارض التواريخ
        v_conflict_count := CHECK_DATE_CONFLICT(p_employee_id, p_start_date, p_end_date);
        
        IF v_conflict_count > 0 THEN
            RAISE_APPLICATION_ERROR(-20203, 'يوجد تعارض مع طلب إجازة آخر في نفس الفترة');
        END IF;

        -- 4. التحقق من نوع الإجازة
        SELECT IS_PAID, REQUIRES_ATTACHMENT
        INTO v_is_paid, v_requires_attachment
        FROM HR_LEAVES.LEAVE_TYPES
        WHERE LEAVE_TYPE_ID = p_leave_type_id;

        -- 5. التحقق من المرفق إذا كان مطلوباً
        IF v_requires_attachment = 1 AND p_attachment_path IS NULL THEN
            RAISE_APPLICATION_ERROR(-20204, 'هذا النوع من الإجازات يتطلب إرفاق مستند');
        END IF;

        -- 6. التحقق من الرصيد (للإجازات المدفوعة فقط)
        IF v_is_paid = 1 THEN
            v_current_balance := GET_LEAVE_BALANCE(p_employee_id, p_leave_type_id, EXTRACT(YEAR FROM p_start_date));
            
            IF v_current_balance < v_days_count THEN
                RAISE_APPLICATION_ERROR(-20205, 
                    'الرصيد المتبقي (' || v_current_balance || ' يوم) غير كافٍ. المطلوب: ' || v_days_count || ' يوم');
            END IF;
        END IF;

        -- 7. إدخال طلب الإجازة
        -- ملاحظة: CREATED_BY و CREATED_AT سيتم تعبئتهما تلقائياً من الـ Trigger
        INSERT INTO HR_LEAVES.LEAVE_REQUESTS (
            EMPLOYEE_ID, LEAVE_TYPE_ID, START_DATE, END_DATE, DAYS_COUNT,
            REASON, ATTACHMENT_PATH, STATUS, IS_POSTED_TO_BALANCE
        ) VALUES (
            p_employee_id, p_leave_type_id, p_start_date, p_end_date, v_days_count,
            p_reason, p_attachment_path, 'PENDING', 0
        ) RETURNING REQUEST_ID INTO v_request_id;

        -- 8. إرجاع النتائج
        o_request_id := v_request_id;
        o_days_count := v_days_count;
        o_message := 'تم تقديم طلب الإجازة بنجاح. رقم الطلب: ' || v_request_id;

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END REQUEST_LEAVE;

    -- ==========================================================
    -- إجراء: اعتماد/رفض طلب إجازة
    -- ==========================================================
    PROCEDURE APPROVE_LEAVE_REQUEST (
        p_request_id        IN NUMBER,
        p_action            IN VARCHAR2,
        p_approval_level    IN VARCHAR2,
        p_rejection_reason  IN VARCHAR2 DEFAULT NULL,
        o_message           OUT VARCHAR2
    ) IS
        v_current_status VARCHAR2(20);
        v_new_status VARCHAR2(20);
        v_employee_id NUMBER;
        v_leave_type_id NUMBER;
        v_days_count NUMBER;
        v_is_paid NUMBER;
        v_year NUMBER;
    BEGIN
        -- 1. جلب بيانات الطلب
        SELECT STATUS, EMPLOYEE_ID, LEAVE_TYPE_ID, DAYS_COUNT, EXTRACT(YEAR FROM START_DATE)
        INTO v_current_status, v_employee_id, v_leave_type_id, v_days_count, v_year
        FROM HR_LEAVES.LEAVE_REQUESTS
        WHERE REQUEST_ID = p_request_id;

        -- 2. التحقق من الحالة الحالية
        IF v_current_status NOT IN ('PENDING', 'MANAGER_APPROVED') THEN
            RAISE_APPLICATION_ERROR(-20301, 'لا يمكن اعتماد/رفض طلب في هذه الحالة');
        END IF;

        -- 3. تحديد الحالة الجديدة
        IF p_action = 'APPROVE' THEN
            IF p_approval_level = 'MANAGER' THEN
                v_new_status := 'MANAGER_APPROVED';
            ELSIF p_approval_level = 'HR' THEN
                v_new_status := 'HR_APPROVED';
            ELSE
                RAISE_APPLICATION_ERROR(-20302, 'مستوى الاعتماد غير صحيح');
            END IF;
        ELSIF p_action = 'REJECT' THEN
            v_new_status := 'REJECTED';
        ELSE
            RAISE_APPLICATION_ERROR(-20303, 'الإجراء غير صحيح (APPROVE أو REJECT فقط)');
        END IF;

        -- 4. تحديث حالة الطلب
        -- ملاحظة: UPDATED_BY, UPDATED_AT, VERSION_NO سيتم تعبئتها تلقائياً من الـ Trigger
        UPDATE HR_LEAVES.LEAVE_REQUESTS
        SET STATUS = v_new_status,
            REJECTION_REASON = p_rejection_reason
        WHERE REQUEST_ID = p_request_id;

        -- 5. إذا تم الاعتماد النهائي من HR، خصم من الرصيد
        IF v_new_status = 'HR_APPROVED' THEN
            -- التحقق إذا كانت الإجازة مدفوعة
            SELECT IS_PAID INTO v_is_paid
            FROM HR_LEAVES.LEAVE_TYPES
            WHERE LEAVE_TYPE_ID = v_leave_type_id;

            IF v_is_paid = 1 THEN
                -- خصم من الرصيد
                -- ملاحظة: UPDATED_BY و UPDATED_AT سيتم تعبئتهما تلقائياً من الـ Trigger
                UPDATE HR_LEAVES.EMPLOYEE_LEAVE_BALANCES
                SET CURRENT_BALANCE = CURRENT_BALANCE - v_days_count
                WHERE EMPLOYEE_ID = v_employee_id
                  AND LEAVE_TYPE_ID = v_leave_type_id
                  AND YEAR = v_year;

                -- تسجيل في جدول الحركات
                -- ملاحظة: CREATED_BY سيتم تعبئته تلقائياً من الـ Trigger
                INSERT INTO HR_LEAVES.LEAVE_TRANSACTIONS (
                    EMPLOYEE_ID, LEAVE_TYPE_ID, TRANSACTION_TYPE, DAYS,
                    TRANSACTION_DATE, REFERENCE_ID, NOTES
                ) VALUES (
                    v_employee_id, v_leave_type_id, 'DEDUCTION', -v_days_count,
                    SYSDATE, p_request_id, 'خصم إجازة معتمدة'
                );

                -- تحديث علامة الترحيل
                UPDATE HR_LEAVES.LEAVE_REQUESTS
                SET IS_POSTED_TO_BALANCE = 1
                WHERE REQUEST_ID = p_request_id;
            END IF;

            o_message := 'تم اعتماد الطلب وخصم ' || v_days_count || ' يوم من الرصيد';
        ELSIF v_new_status = 'REJECTED' THEN
            o_message := 'تم رفض الطلب. السبب: ' || p_rejection_reason;
        ELSE
            o_message := 'تم اعتماد الطلب من المدير. في انتظار اعتماد الموارد البشرية';
        END IF;

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END APPROVE_LEAVE_REQUEST;

    -- ==========================================================
    -- إجراء: استحقاق الإجازات الشهري
    -- ==========================================================
    PROCEDURE MONTHLY_LEAVE_ACCRUAL (
        p_year              IN NUMBER,
        p_month             IN NUMBER,
        o_employees_count   OUT NUMBER,
        o_total_days        OUT NUMBER,
        o_message           OUT VARCHAR2
    ) IS
        v_emp_count NUMBER := 0;
        v_total_days NUMBER := 0;
        v_accrual_days NUMBER;
    BEGIN
        -- استحقاق الإجازات لجميع الموظفين النشطين
        FOR emp IN (
            SELECT e.EMPLOYEE_ID, lt.LEAVE_TYPE_ID, ar.DAYS_PER_PERIOD
            FROM HR_PERSONNEL.EMPLOYEES e
            CROSS JOIN HR_LEAVES.LEAVE_TYPES lt
            LEFT JOIN HR_LEAVES.LEAVE_ACCRUAL_RULES ar ON lt.LEAVE_TYPE_ID = ar.LEAVE_TYPE_ID
            WHERE e.STATUS = 'ACTIVE'
              AND ar.ACCRUAL_FREQUENCY = 'MONTHLY'
        ) LOOP
            v_accrual_days := emp.DAYS_PER_PERIOD;

            -- تحديث أو إدخال الرصيد
            -- ملاحظة: CREATED_BY, UPDATED_BY سيتم تعبئتهما تلقائياً من الـ Trigger
            MERGE INTO HR_LEAVES.EMPLOYEE_LEAVE_BALANCES bal
            USING (SELECT emp.EMPLOYEE_ID AS EMP_ID, emp.LEAVE_TYPE_ID AS TYPE_ID FROM DUAL) src
            ON (bal.EMPLOYEE_ID = src.EMP_ID AND bal.LEAVE_TYPE_ID = src.TYPE_ID AND bal.YEAR = p_year)
            WHEN MATCHED THEN
                UPDATE SET CURRENT_BALANCE = CURRENT_BALANCE + v_accrual_days
            WHEN NOT MATCHED THEN
                INSERT (EMPLOYEE_ID, LEAVE_TYPE_ID, CURRENT_BALANCE, YEAR)
                VALUES (src.EMP_ID, src.TYPE_ID, v_accrual_days, p_year);

            -- تسجيل الحركة
            -- ملاحظة: CREATED_BY سيتم تعبئته تلقائياً من الـ Trigger
            INSERT INTO HR_LEAVES.LEAVE_TRANSACTIONS (
                EMPLOYEE_ID, LEAVE_TYPE_ID, TRANSACTION_TYPE, DAYS,
                TRANSACTION_DATE, NOTES
            ) VALUES (
                emp.EMPLOYEE_ID, emp.LEAVE_TYPE_ID, 'ACCRUAL', v_accrual_days,
                SYSDATE, 'استحقاق شهري - ' || p_month || '/' || p_year
            );

            v_emp_count := v_emp_count + 1;
            v_total_days := v_total_days + v_accrual_days;
        END LOOP;

        o_employees_count := v_emp_count;
        o_total_days := v_total_days;
        o_message := 'تم استحقاق ' || v_total_days || ' يوم لـ ' || v_emp_count || ' موظف';

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END MONTHLY_LEAVE_ACCRUAL;

    -- ==========================================================
    -- إجراء: ترحيل الرصيد للسنة الجديدة
    -- ==========================================================
    PROCEDURE CARRY_FORWARD_BALANCE (
        p_employee_id       IN NUMBER DEFAULT NULL,
        p_from_year         IN NUMBER,
        p_to_year           IN NUMBER,
        o_employees_count   OUT NUMBER,
        o_message           OUT VARCHAR2
    ) IS
        v_count NUMBER := 0;
        v_carry_forward_days NUMBER;
    BEGIN
        FOR bal IN (
            SELECT b.EMPLOYEE_ID, b.LEAVE_TYPE_ID, b.CURRENT_BALANCE, lt.IS_CARRY_FORWARD
            FROM HR_LEAVES.EMPLOYEE_LEAVE_BALANCES b
            JOIN HR_LEAVES.LEAVE_TYPES lt ON b.LEAVE_TYPE_ID = lt.LEAVE_TYPE_ID
            WHERE b.YEAR = p_from_year
              AND lt.IS_CARRY_FORWARD = 1
              AND (p_employee_id IS NULL OR b.EMPLOYEE_ID = p_employee_id)
              AND b.CURRENT_BALANCE > 0
        ) LOOP
            v_carry_forward_days := bal.CURRENT_BALANCE;

            -- إنشاء رصيد جديد للسنة الجديدة
            -- ملاحظة: CREATED_BY سيتم تعبئته تلقائياً من الـ Trigger
            INSERT INTO HR_LEAVES.EMPLOYEE_LEAVE_BALANCES (
                EMPLOYEE_ID, LEAVE_TYPE_ID, CURRENT_BALANCE, YEAR
            ) VALUES (
                bal.EMPLOYEE_ID, bal.LEAVE_TYPE_ID, v_carry_forward_days, p_to_year
            );

            -- تسجيل الحركة
            -- ملاحظة: CREATED_BY سيتم تعبئته تلقائياً من الـ Trigger
            INSERT INTO HR_LEAVES.LEAVE_TRANSACTIONS (
                EMPLOYEE_ID, LEAVE_TYPE_ID, TRANSACTION_TYPE, DAYS,
                TRANSACTION_DATE, NOTES
            ) VALUES (
                bal.EMPLOYEE_ID, bal.LEAVE_TYPE_ID, 'CARRY_FORWARD', v_carry_forward_days,
                SYSDATE, 'ترحيل من ' || p_from_year || ' إلى ' || p_to_year
            );

            v_count := v_count + 1;
        END LOOP;

        o_employees_count := v_count;
        o_message := 'تم ترحيل الرصيد لـ ' || v_count || ' موظف';

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END CARRY_FORWARD_BALANCE;

    -- ==========================================================
    -- إجراء: إلغاء طلب إجازة
    -- ==========================================================
    PROCEDURE CANCEL_LEAVE_REQUEST (
        p_request_id        IN NUMBER,
        o_message           OUT VARCHAR2
    ) IS
        v_status VARCHAR2(20);
    BEGIN
        SELECT STATUS INTO v_status
        FROM HR_LEAVES.LEAVE_REQUESTS
        WHERE REQUEST_ID = p_request_id;

        IF v_status != 'PENDING' THEN
            RAISE_APPLICATION_ERROR(-20401, 'لا يمكن إلغاء طلب معتمد أو مرفوض');
        END IF;

        -- ملاحظة: UPDATED_BY و UPDATED_AT سيتم تعبئتهما تلقائياً من الـ Trigger
        UPDATE HR_LEAVES.LEAVE_REQUESTS
        SET STATUS = 'REJECTED',
            REJECTION_REASON = 'تم الإلغاء من قبل الموظف'
        WHERE REQUEST_ID = p_request_id;

        o_message := 'تم إلغاء الطلب بنجاح';

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END CANCEL_LEAVE_REQUEST;

END PKG_LEAVE_MANAGER;
/
