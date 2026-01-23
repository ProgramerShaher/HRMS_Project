-- =================================================================================
-- الحزمة: PKG_PAYROLL_MANAGER
-- الوصف: تنفيذ منطق إدارة الرواتب (Body)
-- المخطط: HR_PAYROLL
-- =================================================================================

CREATE OR REPLACE PACKAGE BODY HR_PAYROLL.PKG_PAYROLL_MANAGER AS

    -- ثوابت
    C_GOSI_EMPLOYEE_RATE CONSTANT NUMBER := 0.10; -- 10% للموظف
    C_GOSI_EMPLOYER_RATE CONSTANT NUMBER := 0.12; -- 12% للمنشأة

    -- ==========================================================
    -- دالة خاصة: حساب الراتب الأساسي والبدلات
    -- ==========================================================
    FUNCTION GET_EMPLOYEE_SALARY_COMPONENTS (
        p_employee_id IN NUMBER,
        p_component OUT NUMBER, -- BASIC
        p_housing OUT NUMBER,
        p_transport OUT NUMBER,
        p_other OUT NUMBER
    ) RETURN NUMBER IS
        v_total NUMBER := 0;
    BEGIN
        SELECT 
            NVL(BASIC_SALARY, 0),
            NVL(HOUSING_ALLOWANCE, 0),
            NVL(TRANSPORT_ALLOWANCE, 0),
            NVL(OTHER_ALLOWANCES, 0)
        INTO p_component, p_housing, p_transport, p_other
        FROM HR_PERSONNEL.CONTRACTS
        WHERE EMPLOYEE_ID = p_employee_id
          AND CONTRACT_STATUS = 'ACTIVE'
          AND ROWNUM = 1
        ORDER BY START_DATE DESC;

        v_total := p_component + p_housing + p_transport + p_other;
        RETURN v_total;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            RETURN 0;
    END;

    -- ==========================================================
    -- دالة: حساب خصم القرض الشهري
    -- ==========================================================
    FUNCTION CALCULATE_LOAN_DEDUCTION (
        p_employee_id       IN NUMBER,
        p_year              IN NUMBER,
        p_month             IN NUMBER
    ) RETURN NUMBER IS
        v_total_deduction NUMBER := 0;
        v_due_date DATE;
    BEGIN
        -- حساب تاريخ استحقاق الشهر الحالي
        v_due_date := TO_DATE(p_year || '-' || LPAD(p_month, 2, '0') || '-01', 'YYYY-MM-DD');

        SELECT NVL(SUM(inst.AMOUNT), 0)
        INTO v_total_deduction
        FROM HR_PAYROLL.LOAN_INSTALLMENTS inst
        JOIN HR_PAYROLL.LOANS l ON inst.LOAN_ID = l.LOAN_ID
        WHERE l.EMPLOYEE_ID = p_employee_id
          AND l.STATUS = 'ACTIVE'
          AND inst.IS_PAID = 0
          AND TRUNC(inst.DUE_DATE, 'MM') = TRUNC(v_due_date, 'MM');

        RETURN v_total_deduction;
    END;

    -- ==========================================================
    -- دالة: حساب الراتب الصافي
    -- ==========================================================
    FUNCTION CALCULATE_NET_SALARY (
        p_employee_id       IN NUMBER,
        p_year              IN NUMBER,
        p_month             IN NUMBER
    ) RETURN NUMBER IS
        v_basic NUMBER := 0;
        v_housing NUMBER := 0;
        v_transport NUMBER := 0;
        v_other NUMBER := 0;
        v_total_earnings NUMBER := 0;
        v_gosi_deduction NUMBER := 0;
        v_loan_deduction NUMBER := 0;
        v_total_deductions NUMBER := 0;
        v_net_salary NUMBER := 0;
    BEGIN
        -- 1. حساب الأرباح
        v_total_earnings := GET_EMPLOYEE_SALARY_COMPONENTS(
            p_employee_id, v_basic, v_housing, v_transport, v_other
        );

        -- 2. حساب التأمينات الاجتماعية (على الراتب الأساسي فقط)
        v_gosi_deduction := v_basic * C_GOSI_EMPLOYEE_RATE;

        -- 3. حساب خصم القروض
        v_loan_deduction := CALCULATE_LOAN_DEDUCTION(p_employee_id, p_year, p_month);

        -- 4. إجمالي الخصومات
        v_total_deductions := v_gosi_deduction + v_loan_deduction;

        -- 5. الصافي
        v_net_salary := v_total_earnings - v_total_deductions;

        RETURN v_net_salary;
    END;

    -- ==========================================================
    -- إجراء: إنشاء قسيمة راتب
    -- ==========================================================
    PROCEDURE GENERATE_PAYSLIP (
        p_run_id            IN NUMBER,
        p_employee_id       IN NUMBER,
        o_payslip_id        OUT NUMBER,
        o_net_salary        OUT NUMBER,
        o_message           OUT VARCHAR2
    ) IS
        v_payslip_id NUMBER;
        v_basic NUMBER := 0;
        v_housing NUMBER := 0;
        v_transport NUMBER := 0;
        v_other NUMBER := 0;
        v_total_earnings NUMBER := 0;
        v_gosi NUMBER := 0;
        v_loan NUMBER := 0;
        v_total_deductions NUMBER := 0;
        v_net NUMBER := 0;
        v_year NUMBER;
        v_month NUMBER;
    BEGIN
        -- جلب السنة والشهر من المسير
        SELECT YEAR, MONTH INTO v_year, v_month
        FROM HR_PAYROLL.PAYROLL_RUNS
        WHERE RUN_ID = p_run_id;

        -- حساب المكونات
        v_total_earnings := GET_EMPLOYEE_SALARY_COMPONENTS(
            p_employee_id, v_basic, v_housing, v_transport, v_other
        );

        v_gosi := v_basic * C_GOSI_EMPLOYEE_RATE;
        v_loan := CALCULATE_LOAN_DEDUCTION(p_employee_id, v_year, v_month);
        v_total_deductions := v_gosi + v_loan;
        v_net := v_total_earnings - v_total_deductions;

        -- إنشاء القسيمة
        -- ملاحظة: CREATED_BY سيتم تعبئته تلقائياً من الـ Trigger
        INSERT INTO HR_PAYROLL.PAYSLIPS (
            RUN_ID, EMPLOYEE_ID, BASIC_SALARY, TOTAL_ALLOWANCES,
            TOTAL_DEDUCTIONS, NET_SALARY
        ) VALUES (
            p_run_id, p_employee_id, v_basic, (v_housing + v_transport + v_other),
            v_total_deductions, v_net
        ) RETURNING PAYSLIP_ID INTO v_payslip_id;

        -- إضافة التفاصيل - الأرباح
        -- ملاحظة: CREATED_BY سيتم تعبئته تلقائياً من الـ Trigger
        INSERT INTO HR_PAYROLL.PAYSLIP_DETAILS (PAYSLIP_ID, ELEMENT_ID, ELEMENT_NAME_AR, AMOUNT, TYPE)
        SELECT v_payslip_id, 1, 'الراتب الأساسي', v_basic, 'EARNING' FROM DUAL WHERE v_basic > 0
        UNION ALL
        SELECT v_payslip_id, 2, 'بدل السكن', v_housing, 'EARNING' FROM DUAL WHERE v_housing > 0
        UNION ALL
        SELECT v_payslip_id, 3, 'بدل النقل', v_transport, 'EARNING' FROM DUAL WHERE v_transport > 0
        UNION ALL
        SELECT v_payslip_id, 4, 'بدلات أخرى', v_other, 'EARNING' FROM DUAL WHERE v_other > 0;

        -- إضافة التفاصيل - الخصومات
        -- ملاحظة: CREATED_BY سيتم تعبئته تلقائياً من الـ Trigger
        INSERT INTO HR_PAYROLL.PAYSLIP_DETAILS (PAYSLIP_ID, ELEMENT_ID, ELEMENT_NAME_AR, AMOUNT, TYPE)
        SELECT v_payslip_id, 10, 'التأمينات الاجتماعية', v_gosi, 'DEDUCTION' FROM DUAL WHERE v_gosi > 0
        UNION ALL
        SELECT v_payslip_id, 11, 'خصم القروض', v_loan, 'DEDUCTION' FROM DUAL WHERE v_loan > 0;

        -- تحديث حالة أقساط القروض
        -- ملاحظة: UPDATED_BY و UPDATED_AT سيتم تعبئتهما تلقائياً من الـ Trigger
        UPDATE HR_PAYROLL.LOAN_INSTALLMENTS
        SET IS_PAID = 1,
            PAID_IN_PAYROLL_RUN = p_run_id
        WHERE LOAN_ID IN (
            SELECT LOAN_ID FROM HR_PAYROLL.LOANS WHERE EMPLOYEE_ID = p_employee_id AND STATUS = 'ACTIVE'
        )
        AND TRUNC(DUE_DATE, 'MM') = TO_DATE(v_year || '-' || LPAD(v_month, 2, '0') || '-01', 'YYYY-MM-DD')
        AND IS_PAID = 0;

        o_payslip_id := v_payslip_id;
        o_net_salary := v_net;
        o_message := 'تم إنشاء قسيمة الراتب بنجاح';

    EXCEPTION
        WHEN OTHERS THEN
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END GENERATE_PAYSLIP;

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
    ) IS
        v_run_id NUMBER;
        v_emp_count NUMBER := 0;
        v_total NUMBER := 0;
        v_payslip_id NUMBER;
        v_net_salary NUMBER;
        v_msg VARCHAR2(500);
    BEGIN
        -- التحقق من عدم وجود مسير معتمد لنفس الشهر
        SELECT COUNT(*) INTO v_emp_count
        FROM HR_PAYROLL.PAYROLL_RUNS
        WHERE YEAR = p_year AND MONTH = p_month AND STATUS IN ('APPROVED', 'PAID');

        IF v_emp_count > 0 THEN
            RAISE_APPLICATION_ERROR(-20501, 'يوجد مسير معتمد لهذا الشهر بالفعل');
        END IF;

        -- إنشاء المسير
        -- ملاحظة: CREATED_BY سيتم تعبئته تلقائياً من الـ Trigger
        INSERT INTO HR_PAYROLL.PAYROLL_RUNS (YEAR, MONTH, RUN_DATE, STATUS)
        VALUES (p_year, p_month, SYSDATE, 'DRAFT')
        RETURNING RUN_ID INTO v_run_id;

        v_emp_count := 0;
        v_total := 0;

        -- معالجة جميع الموظفين النشطين
        FOR emp IN (
            SELECT EMPLOYEE_ID 
            FROM HR_PERSONNEL.EMPLOYEES 
            WHERE STATUS = 'ACTIVE'
        ) LOOP
            BEGIN
                GENERATE_PAYSLIP(
                    v_run_id, emp.EMPLOYEE_ID,
                    v_payslip_id, v_net_salary, v_msg
                );

                v_emp_count := v_emp_count + 1;
                v_total := v_total + v_net_salary;

            EXCEPTION
                WHEN OTHERS THEN
                    -- تسجيل الخطأ ومتابعة باقي الموظفين
                    DBMS_OUTPUT.PUT_LINE('خطأ في معالجة الموظف ' || emp.EMPLOYEE_ID || ': ' || SQLERRM);
            END;
        END LOOP;

        -- تحديث إجمالي المسير
        UPDATE HR_PAYROLL.PAYROLL_RUNS
        SET TOTAL_PAYOUT = v_total
        WHERE RUN_ID = v_run_id;

        o_run_id := v_run_id;
        o_employees_count := v_emp_count;
        o_total_payout := v_total;
        o_message := 'تم إنشاء مسير الرواتب بنجاح لـ ' || v_emp_count || ' موظف. الإجمالي: ' || v_total;

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END RUN_MONTHLY_PAYROLL;

    -- ==========================================================
    -- إجراء: اعتماد مسير الرواتب
    -- ==========================================================
    PROCEDURE APPROVE_PAYROLL_RUN (
        p_run_id            IN NUMBER,
        o_message           OUT VARCHAR2
    ) IS
        v_status VARCHAR2(20);
    BEGIN
        SELECT STATUS INTO v_status
        FROM HR_PAYROLL.PAYROLL_RUNS
        WHERE RUN_ID = p_run_id;

        IF v_status != 'DRAFT' THEN
            RAISE_APPLICATION_ERROR(-20502, 'لا يمكن اعتماد مسير في هذه الحالة');
        END IF;

        -- ملاحظة: UPDATED_BY و UPDATED_AT سيتم تعبئتهما تلقائياً من الـ Trigger
        UPDATE HR_PAYROLL.PAYROLL_RUNS
        SET STATUS = 'APPROVED'
        WHERE RUN_ID = p_run_id;

        o_message := 'تم اعتماد مسير الرواتب بنجاح';

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END APPROVE_PAYROLL_RUN;

    -- ==========================================================
    -- إجراء: منح قرض
    -- ==========================================================
    PROCEDURE GRANT_LOAN (
        p_employee_id       IN NUMBER,
        p_loan_amount       IN NUMBER,
        p_installment_count IN NUMBER,
        o_loan_id           OUT NUMBER,
        o_monthly_installment OUT NUMBER,
        o_message           OUT VARCHAR2
    ) IS
        v_loan_id NUMBER;
        v_installment_amount NUMBER;
        v_due_date DATE;
    BEGIN
        -- التحقق من الحد الأقصى للقرض (مثلاً 50% من الراتب الأساسي)
        DECLARE
            v_basic_salary NUMBER;
            v_max_loan NUMBER;
        BEGIN
            SELECT BASIC_SALARY INTO v_basic_salary
            FROM HR_PERSONNEL.CONTRACTS
            WHERE EMPLOYEE_ID = p_employee_id AND CONTRACT_STATUS = 'ACTIVE'
            AND ROWNUM = 1;

            v_max_loan := v_basic_salary * 10; -- 10 أضعاف الراتب كحد أقصى

            IF p_loan_amount > v_max_loan THEN
                RAISE_APPLICATION_ERROR(-20601, 'مبلغ القرض يتجاوز الحد المسموح (' || v_max_loan || ')');
            END IF;
        END;

        -- حساب القسط الشهري
        v_installment_amount := ROUND(p_loan_amount / p_installment_count, 2);

        -- إنشاء القرض
        -- ملاحظة: CREATED_BY سيتم تعبئته تلقائياً من الـ Trigger
        INSERT INTO HR_PAYROLL.LOANS (
            EMPLOYEE_ID, LOAN_AMOUNT, REQUEST_DATE, INSTALLMENT_COUNT, STATUS
        ) VALUES (
            p_employee_id, p_loan_amount, SYSDATE, p_installment_count, 'ACTIVE'
        ) RETURNING LOAN_ID INTO v_loan_id;

        -- إنشاء الأقساط
        v_due_date := ADD_MONTHS(TRUNC(SYSDATE, 'MM'), 1);

        FOR i IN 1..p_installment_count LOOP
            -- ملاحظة: CREATED_BY سيتم تعبئته تلقائياً من الـ Trigger
            INSERT INTO HR_PAYROLL.LOAN_INSTALLMENTS (
                LOAN_ID, INSTALLMENT_NUMBER, DUE_DATE, AMOUNT, IS_PAID
            ) VALUES (
                v_loan_id, i, v_due_date, v_installment_amount, 0
            );

            v_due_date := ADD_MONTHS(v_due_date, 1);
        END LOOP;

        o_loan_id := v_loan_id;
        o_monthly_installment := v_installment_amount;
        o_message := 'تم منح القرض بنجاح. القسط الشهري: ' || v_installment_amount;

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END GRANT_LOAN;

    -- ==========================================================
    -- دالة: حساب مكافأة نهاية الخدمة
    -- ==========================================================
    FUNCTION CALCULATE_END_OF_SERVICE (
        p_employee_id       IN NUMBER,
        p_termination_date  IN DATE
    ) RETURN NUMBER IS
        v_joining_date DATE;
        v_basic_salary NUMBER;
        v_service_years NUMBER;
        v_eos_amount NUMBER := 0;
    BEGIN
        -- جلب تاريخ التعيين والراتب الأساسي
        SELECT e.JOINING_DATE, c.BASIC_SALARY
        INTO v_joining_date, v_basic_salary
        FROM HR_PERSONNEL.EMPLOYEES e
        JOIN HR_PERSONNEL.CONTRACTS c ON e.EMPLOYEE_ID = c.EMPLOYEE_ID
        WHERE e.EMPLOYEE_ID = p_employee_id
          AND c.CONTRACT_STATUS = 'ACTIVE'
          AND ROWNUM = 1;

        -- حساب سنوات الخدمة
        v_service_years := MONTHS_BETWEEN(p_termination_date, v_joining_date) / 12;

        -- حساب المكافأة حسب نظام العمل السعودي
        IF v_service_years < 2 THEN
            v_eos_amount := 0; -- لا مكافأة قبل سنتين
        ELSIF v_service_years <= 5 THEN
            -- نصف شهر عن كل سنة من السنوات الخمس الأولى
            v_eos_amount := (v_basic_salary / 2) * v_service_years;
        ELSE
            -- نصف شهر عن أول 5 سنوات + شهر كامل عن الباقي
            v_eos_amount := (v_basic_salary / 2) * 5 + v_basic_salary * (v_service_years - 5);
        END IF;

        RETURN ROUND(v_eos_amount, 2);
    END;

    -- ==========================================================
    -- إجراء: معالجة مكافأة نهاية الخدمة
    -- ==========================================================
    PROCEDURE PROCESS_END_OF_SERVICE (
        p_employee_id       IN NUMBER,
        p_termination_date  IN DATE,
        o_eos_id            OUT NUMBER,
        o_total_amount      OUT NUMBER,
        o_message           OUT VARCHAR2
    ) IS
        v_eos_id NUMBER;
        v_amount NUMBER;
        v_service_years NUMBER;
        v_basic_salary NUMBER;
        v_joining_date DATE;
    BEGIN
        -- حساب المكافأة
        v_amount := CALCULATE_END_OF_SERVICE(p_employee_id, p_termination_date);

        -- جلب البيانات للتوثيق
        SELECT e.JOINING_DATE, c.BASIC_SALARY
        INTO v_joining_date, v_basic_salary
        FROM HR_PERSONNEL.EMPLOYEES e
        JOIN HR_PERSONNEL.CONTRACTS c ON e.EMPLOYEE_ID = c.EMPLOYEE_ID
        WHERE e.EMPLOYEE_ID = p_employee_id AND c.CONTRACT_STATUS = 'ACTIVE' AND ROWNUM = 1;

        v_service_years := ROUND(MONTHS_BETWEEN(p_termination_date, v_joining_date) / 12, 2);

        -- تسجيل المكافأة
        -- ملاحظة: CREATED_BY سيتم تعبئته تلقائياً من الـ Trigger
        INSERT INTO HR_PAYROLL.END_OF_SERVICE_CALC (
            EMPLOYEE_ID, TERMINATION_DATE, SERVICE_YEARS, LAST_BASIC_SALARY,
            TOTAL_AMOUNT, CALCULATION_NOTES, IS_PAID
        ) VALUES (
            p_employee_id, p_termination_date, v_service_years, v_basic_salary,
            v_amount, 'حساب تلقائي حسب نظام العمل السعودي', 0
        ) RETURNING EOS_ID INTO v_eos_id;

        o_eos_id := v_eos_id;
        o_total_amount := v_amount;
        o_message := 'تم حساب مكافأة نهاية الخدمة: ' || v_amount || ' ريال لـ ' || v_service_years || ' سنة خدمة';

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END PROCESS_END_OF_SERVICE;

    -- ==========================================================
    -- إجراء: إضافة تعديل يدوي
    -- ==========================================================
    PROCEDURE ADD_PAYROLL_ADJUSTMENT (
        p_employee_id       IN NUMBER,
        p_payroll_run_id    IN NUMBER,
        p_adjustment_type   IN VARCHAR2,
        p_amount            IN NUMBER,
        p_reason            IN VARCHAR2,
        p_approved_by       IN NUMBER,
        o_adjustment_id     OUT NUMBER,
        o_message           OUT VARCHAR2
    ) IS
        v_adj_id NUMBER;
    BEGIN
        -- ملاحظة: CREATED_BY سيتم تعبئته تلقائياً من الـ Trigger
        INSERT INTO HR_PAYROLL.PAYROLL_ADJUSTMENTS (
            EMPLOYEE_ID, PAYROLL_RUN_ID, ADJUSTMENT_TYPE, AMOUNT, REASON, APPROVED_BY
        ) VALUES (
            p_employee_id, p_payroll_run_id, p_adjustment_type, p_amount, p_reason, p_approved_by
        ) RETURNING ADJUSTMENT_ID INTO v_adj_id;

        o_adjustment_id := v_adj_id;
        o_message := 'تم إضافة التعديل بنجاح';

        COMMIT;

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            o_message := 'خطأ: ' || SQLERRM;
            RAISE;
    END ADD_PAYROLL_ADJUSTMENT;

END PKG_PAYROLL_MANAGER;
/
