-- =================================================================================
-- منح صلاحيات إضافية للـ Packages
-- =================================================================================

ALTER SESSION SET CONTAINER = XEPDB1;

-- منح صلاحية UPDATE على EMPLOYEES لـ HR_PERFORMANCE (لتنفيذ الجزاءات)
GRANT UPDATE ON HR_PERSONNEL.EMPLOYEES TO HR_PERFORMANCE;

-- منح صلاحية UPDATE على EMPLOYEE_LEAVE_BALANCES لـ HR_LEAVES (للخصم التلقائي)
GRANT UPDATE ON HR_LEAVES.EMPLOYEE_LEAVE_BALANCES TO HR_LEAVES;

-- منح صلاحية INSERT على LEAVE_TRANSACTIONS لـ HR_LEAVES
GRANT INSERT ON HR_LEAVES.LEAVE_TRANSACTIONS TO HR_LEAVES;

-- منح صلاحية UPDATE على LOAN_INSTALLMENTS لـ HR_PAYROLL
GRANT UPDATE ON HR_PAYROLL.LOAN_INSTALLMENTS TO HR_PAYROLL;

SELECT 'تم منح الصلاحيات الإضافية للـ Packages بنجاح!' AS STATUS FROM DUAL;
