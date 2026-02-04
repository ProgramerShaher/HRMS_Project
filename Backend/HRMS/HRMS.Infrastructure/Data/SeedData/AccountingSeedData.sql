-- ============================================
-- Seed Data for Accounting Module
-- دليل الحسابات الأساسي
-- ============================================

-- إدراج الحسابات الرئيسية المطلوبة لترحيل الرواتب

-- 1. حساب مصروف الرواتب (Salaries Expense)
INSERT INTO ACCOUNTING.ACCOUNTS (ACCOUNT_CODE, ACCOUNT_NAME_AR, ACCOUNT_NAME_EN, ACCOUNT_TYPE, IS_ACTIVE, LEVEL, CREATED_AT, CREATED_BY)
VALUES 
('5100', 'مصروف الرواتب', 'Salaries Expense', 'EXPENSE', 1, 1, GETDATE(), 'System');

-- 2. حساب الرواتب المستحقة (Salaries Payable)
INSERT INTO ACCOUNTING.ACCOUNTS (ACCOUNT_CODE, ACCOUNT_NAME_AR, ACCOUNT_NAME_EN, ACCOUNT_TYPE, IS_ACTIVE, LEVEL, CREATED_AT, CREATED_BY)
VALUES 
('2100', 'رواتب مستحقة', 'Salaries Payable', 'LIABILITY', 1, 1, GETDATE(), 'System');

-- 3. حساب الغرامات والاستقطاعات (Penalties/Other Income)
INSERT INTO ACCOUNTING.ACCOUNTS (ACCOUNT_CODE, ACCOUNT_NAME_AR, ACCOUNT_NAME_EN, ACCOUNT_TYPE, IS_ACTIVE, LEVEL, CREATED_AT, CREATED_BY)
VALUES 
('4200', 'إيرادات أخرى - غرامات', 'Other Income - Penalties', 'REVENUE', 1, 1, GETDATE(), 'System');

-- ============================================
-- ملاحظات:
-- - يجب تشغيل هذا السكريبت بعد إنشاء Migration للجداول الجديدة
-- - الحسابات الثلاثة ضرورية لعمل ترحيل الرواتب
-- - يمكن تعديل رموز الحسابات حسب دليل الحسابات الخاص بالمستشفى
-- ============================================
