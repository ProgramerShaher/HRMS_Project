-- =================================================================================
-- بيانات أولية شاملة لجميع جداول HR_CORE
-- Oracle 23ai Free - FREEPDB1
-- =================================================================================

ALTER SESSION SET CONTAINER = FREEPDB1;

PROMPT ========================================
PROMPT إدراج البيانات الأولية لـ HR_CORE
PROMPT ========================================

-- =================================================================================
-- 1. COUNTRIES (الدول)
-- =================================================================================

INSERT INTO HR_CORE.COUNTRIES (COUNTRY_NAME_AR, COUNTRY_NAME_EN, CITIZENSHIP_NAME_AR, ISO_CODE, CREATED_BY)
VALUES ('المملكة العربية السعودية', 'Saudi Arabia', 'سعودي', 'SA', 'SYSTEM');

INSERT INTO HR_CORE.COUNTRIES (COUNTRY_NAME_AR, COUNTRY_NAME_EN, CITIZENSHIP_NAME_AR, ISO_CODE, CREATED_BY)
VALUES ('الإمارات العربية المتحدة', 'United Arab Emirates', 'إماراتي', 'AE', 'SYSTEM');

INSERT INTO HR_CORE.COUNTRIES (COUNTRY_NAME_AR, COUNTRY_NAME_EN, CITIZENSHIP_NAME_AR, ISO_CODE, CREATED_BY)
VALUES ('جمهورية مصر العربية', 'Egypt', 'مصري', 'EG', 'SYSTEM');

INSERT INTO HR_CORE.COUNTRIES (COUNTRY_NAME_AR, COUNTRY_NAME_EN, CITIZENSHIP_NAME_AR, ISO_CODE, CREATED_BY)
VALUES ('الأردن', 'Jordan', 'أردني', 'JO', 'SYSTEM');

INSERT INTO HR_CORE.COUNTRIES (COUNTRY_NAME_AR, COUNTRY_NAME_EN, CITIZENSHIP_NAME_AR, ISO_CODE, CREATED_BY)
VALUES ('الكويت', 'Kuwait', 'كويتي', 'KW', 'SYSTEM');

PROMPT ✅ تم إدراج 5 دول

-- =================================================================================
-- 2. CITIES (المدن)
-- =================================================================================

-- مدن السعودية
INSERT INTO HR_CORE.CITIES (COUNTRY_ID, CITY_NAME_AR, CITY_NAME_EN, CREATED_BY)
VALUES (1, 'الرياض', 'Riyadh', 'SYSTEM');

INSERT INTO HR_CORE.CITIES (COUNTRY_ID, CITY_NAME_AR, CITY_NAME_EN, CREATED_BY)
VALUES (1, 'جدة', 'Jeddah', 'SYSTEM');

INSERT INTO HR_CORE.CITIES (COUNTRY_ID, CITY_NAME_AR, CITY_NAME_EN, CREATED_BY)
VALUES (1, 'الدمام', 'Dammam', 'SYSTEM');

INSERT INTO HR_CORE.CITIES (COUNTRY_ID, CITY_NAME_AR, CITY_NAME_EN, CREATED_BY)
VALUES (1, 'مكة المكرمة', 'Makkah', 'SYSTEM');

-- مدن الإمارات
INSERT INTO HR_CORE.CITIES (COUNTRY_ID, CITY_NAME_AR, CITY_NAME_EN, CREATED_BY)
VALUES (2, 'دبي', 'Dubai', 'SYSTEM');

INSERT INTO HR_CORE.CITIES (COUNTRY_ID, CITY_NAME_AR, CITY_NAME_EN, CREATED_BY)
VALUES (2, 'أبوظبي', 'Abu Dhabi', 'SYSTEM');

PROMPT ✅ تم إدراج 6 مدن

-- =================================================================================
-- 3. BANKS (البنوك)
-- =================================================================================

INSERT INTO HR_CORE.BANKS (BANK_NAME_AR, BANK_NAME_EN, BANK_CODE, CREATED_BY)
VALUES ('البنك الأهلي السعودي', 'Al Ahli Bank', '10', 'SYSTEM');

INSERT INTO HR_CORE.BANKS (BANK_NAME_AR, BANK_NAME_EN, BANK_CODE, CREATED_BY)
VALUES ('بنك الراجحي', 'Al Rajhi Bank', '80', 'SYSTEM');

INSERT INTO HR_CORE.BANKS (BANK_NAME_AR, BANK_NAME_EN, BANK_CODE, CREATED_BY)
VALUES ('البنك السعودي للاستثمار', 'SAIB', '05', 'SYSTEM');

INSERT INTO HR_CORE.BANKS (BANK_NAME_AR, BANK_NAME_EN, BANK_CODE, CREATED_BY)
VALUES ('بنك الرياض', 'Riyad Bank', '20', 'SYSTEM');

PROMPT ✅ تم إدراج 4 بنوك

-- =================================================================================
-- 4. BRANCHES (الفروع)
-- =================================================================================

INSERT INTO HR_CORE.BRANCHES (BRANCH_NAME_AR, BRANCH_NAME_EN, CITY_ID, ADDRESS, CREATED_BY)
VALUES ('الفرع الرئيسي', 'Main Branch', 1, 'طريق الملك فهد، الرياض', 'SYSTEM');

INSERT INTO HR_CORE.BRANCHES (BRANCH_NAME_AR, BRANCH_NAME_EN, CITY_ID, ADDRESS, CREATED_BY)
VALUES ('فرع جدة', 'Jeddah Branch', 2, 'شارع التحلية، جدة', 'SYSTEM');

INSERT INTO HR_CORE.BRANCHES (BRANCH_NAME_AR, BRANCH_NAME_EN, CITY_ID, ADDRESS, CREATED_BY)
VALUES ('فرع الدمام', 'Dammam Branch', 3, 'الكورنيش، الدمام', 'SYSTEM');

PROMPT ✅ تم إدراج 3 فروع

-- =================================================================================
-- 5. JOB_GRADES (الدرجات الوظيفية)
-- =================================================================================

INSERT INTO HR_CORE.JOB_GRADES (GRADE_NAME, MIN_SALARY, MAX_SALARY, TICKET_CLASS, CREATED_BY)
VALUES ('الدرجة الأولى', 15000, 25000, 'Business', 'SYSTEM');

INSERT INTO HR_CORE.JOB_GRADES (GRADE_NAME, MIN_SALARY, MAX_SALARY, TICKET_CLASS, CREATED_BY)
VALUES ('الدرجة الثانية', 10000, 18000, 'Economy', 'SYSTEM');

INSERT INTO HR_CORE.JOB_GRADES (GRADE_NAME, MIN_SALARY, MAX_SALARY, TICKET_CLASS, CREATED_BY)
VALUES ('الدرجة الثالثة', 6000, 12000, 'Economy', 'SYSTEM');

INSERT INTO HR_CORE.JOB_GRADES (GRADE_NAME, MIN_SALARY, MAX_SALARY, TICKET_CLASS, CREATED_BY)
VALUES ('الدرجة الرابعة', 4000, 8000, 'Economy', 'SYSTEM');

PROMPT ✅ تم إدراج 4 درجات وظيفية

-- =================================================================================
-- 6. DEPARTMENTS (الأقسام)
-- =================================================================================

INSERT INTO HR_CORE.DEPARTMENTS (DEPT_NAME_AR, DEPT_NAME_EN, BRANCH_ID, COST_CENTER_CODE, IS_ACTIVE, CREATED_BY)
VALUES ('الموارد البشرية', 'Human Resources', 1, 'CC-HR-001', 1, 'SYSTEM');

INSERT INTO HR_CORE.DEPARTMENTS (DEPT_NAME_AR, DEPT_NAME_EN, BRANCH_ID, COST_CENTER_CODE, IS_ACTIVE, CREATED_BY)
VALUES ('المالية', 'Finance', 1, 'CC-FIN-001', 1, 'SYSTEM');

INSERT INTO HR_CORE.DEPARTMENTS (DEPT_NAME_AR, DEPT_NAME_EN, BRANCH_ID, COST_CENTER_CODE, IS_ACTIVE, CREATED_BY)
VALUES ('تقنية المعلومات', 'IT', 1, 'CC-IT-001', 1, 'SYSTEM');

INSERT INTO HR_CORE.DEPARTMENTS (DEPT_NAME_AR, DEPT_NAME_EN, BRANCH_ID, COST_CENTER_CODE, IS_ACTIVE, CREATED_BY)
VALUES ('العمليات', 'Operations', 1, 'CC-OPS-001', 1, 'SYSTEM');

INSERT INTO HR_CORE.DEPARTMENTS (DEPT_NAME_AR, DEPT_NAME_EN, BRANCH_ID, COST_CENTER_CODE, IS_ACTIVE, CREATED_BY)
VALUES ('التسويق', 'Marketing', 1, 'CC-MKT-001', 1, 'SYSTEM');

PROMPT ✅ تم إدراج 5 أقسام

-- =================================================================================
-- 7. JOBS (الوظائف)
-- =================================================================================

INSERT INTO HR_CORE.JOBS (JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, IS_MEDICAL, CREATED_BY)
VALUES ('مدير موارد بشرية', 'HR Manager', 1, 0, 'SYSTEM');

INSERT INTO HR_CORE.JOBS (JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, IS_MEDICAL, CREATED_BY)
VALUES ('محاسب', 'Accountant', 2, 0, 'SYSTEM');

INSERT INTO HR_CORE.JOBS (JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, IS_MEDICAL, CREATED_BY)
VALUES ('مبرمج', 'Programmer', 2, 0, 'SYSTEM');

INSERT INTO HR_CORE.JOBS (JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, IS_MEDICAL, CREATED_BY)
VALUES ('طبيب عام', 'General Practitioner', 1, 1, 'SYSTEM');

INSERT INTO HR_CORE.JOBS (JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, IS_MEDICAL, CREATED_BY)
VALUES ('ممرض', 'Nurse', 3, 1, 'SYSTEM');

INSERT INTO HR_CORE.JOBS (JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, IS_MEDICAL, CREATED_BY)
VALUES ('موظف استقبال', 'Receptionist', 4, 0, 'SYSTEM');

PROMPT ✅ تم إدراج 6 وظائف

-- =================================================================================
-- 8. DOCUMENT_TYPES (أنواع المستندات)
-- =================================================================================

INSERT INTO HR_CORE.DOCUMENT_TYPES (DOC_NAME_AR, IS_MANDATORY, REQUIRES_EXPIRY, ALERT_DAYS_BEFORE, CREATED_BY)
VALUES ('الهوية الوطنية', 1, 1, 60, 'SYSTEM');

INSERT INTO HR_CORE.DOCUMENT_TYPES (DOC_NAME_AR, IS_MANDATORY, REQUIRES_EXPIRY, ALERT_DAYS_BEFORE, CREATED_BY)
VALUES ('جواز السفر', 1, 1, 90, 'SYSTEM');

INSERT INTO HR_CORE.DOCUMENT_TYPES (DOC_NAME_AR, IS_MANDATORY, REQUIRES_EXPIRY, ALERT_DAYS_BEFORE, CREATED_BY)
VALUES ('رخصة القيادة', 0, 1, 30, 'SYSTEM');

INSERT INTO HR_CORE.DOCUMENT_TYPES (DOC_NAME_AR, IS_MANDATORY, REQUIRES_EXPIRY, ALERT_DAYS_BEFORE, CREATED_BY)
VALUES ('الشهادة الجامعية', 1, 0, 0, 'SYSTEM');

INSERT INTO HR_CORE.DOCUMENT_TYPES (DOC_NAME_AR, IS_MANDATORY, REQUIRES_EXPIRY, ALERT_DAYS_BEFORE, CREATED_BY)
VALUES ('شهادة الخبرة', 0, 0, 0, 'SYSTEM');

PROMPT ✅ تم إدراج 5 أنواع مستندات

-- =================================================================================
-- 9. SYSTEM_SETTINGS (إعدادات النظام)
-- =================================================================================

INSERT INTO HR_CORE.SYSTEM_SETTINGS (SETTING_KEY, SETTING_VALUE, SETTING_TYPE, DESCRIPTION_AR, IS_EDITABLE, CREATED_BY)
VALUES ('WORKING_HOURS_PER_DAY', '8', 'NUMBER', 'عدد ساعات العمل اليومية', 1, 'SYSTEM');

INSERT INTO HR_CORE.SYSTEM_SETTINGS (SETTING_KEY, SETTING_VALUE, SETTING_TYPE, DESCRIPTION_AR, IS_EDITABLE, CREATED_BY)
VALUES ('WORKING_DAYS_PER_WEEK', '5', 'NUMBER', 'عدد أيام العمل الأسبوعية', 1, 'SYSTEM');

INSERT INTO HR_CORE.SYSTEM_SETTINGS (SETTING_KEY, SETTING_VALUE, SETTING_TYPE, DESCRIPTION_AR, IS_EDITABLE, CREATED_BY)
VALUES ('ANNUAL_LEAVE_DAYS', '30', 'NUMBER', 'عدد أيام الإجازة السنوية', 1, 'SYSTEM');

INSERT INTO HR_CORE.SYSTEM_SETTINGS (SETTING_KEY, SETTING_VALUE, SETTING_TYPE, DESCRIPTION_AR, IS_EDITABLE, CREATED_BY)
VALUES ('PROBATION_PERIOD_DAYS', '90', 'NUMBER', 'فترة التجربة بالأيام', 1, 'SYSTEM');

INSERT INTO HR_CORE.SYSTEM_SETTINGS (SETTING_KEY, SETTING_VALUE, SETTING_TYPE, DESCRIPTION_AR, IS_EDITABLE, CREATED_BY)
VALUES ('COMPANY_NAME', 'مستشفى الموارد البشرية', 'STRING', 'اسم الشركة', 1, 'SYSTEM');

PROMPT ✅ تم إدراج 5 إعدادات نظام

-- =================================================================================
-- 10. ASSET_CATEGORIES (فئات الأصول)
-- =================================================================================

INSERT INTO HR_CORE.ASSET_CATEGORIES (CATEGORY_NAME_AR, CATEGORY_NAME_EN, DESCRIPTION, CREATED_BY)
VALUES ('أجهزة كمبيوتر', 'Computers', 'أجهزة كمبيوتر محمولة ومكتبية', 'SYSTEM');

INSERT INTO HR_CORE.ASSET_CATEGORIES (CATEGORY_NAME_AR, CATEGORY_NAME_EN, DESCRIPTION, CREATED_BY)
VALUES ('سيارات', 'Vehicles', 'سيارات الشركة', 'SYSTEM');

INSERT INTO HR_CORE.ASSET_CATEGORIES (CATEGORY_NAME_AR, CATEGORY_NAME_EN, DESCRIPTION, CREATED_BY)
VALUES ('أثاث مكتبي', 'Office Furniture', 'مكاتب وكراسي', 'SYSTEM');

PROMPT ✅ تم إدراج 3 فئات أصول

-- =================================================================================
-- 11. COMPANY_ASSETS (أصول الشركة)
-- =================================================================================

INSERT INTO HR_CORE.COMPANY_ASSETS (ASSET_CODE, ASSET_NAME_AR, CATEGORY_ID, SERIAL_NUMBER, PURCHASE_DATE, PURCHASE_PRICE, STATUS, CREATED_BY)
VALUES ('LAPTOP-001', 'لابتوب ديل XPS 15', 1, 'SN123456789', TO_DATE('2024-01-15', 'YYYY-MM-DD'), 8500, 'AVAILABLE', 'SYSTEM');

INSERT INTO HR_CORE.COMPANY_ASSETS (ASSET_CODE, ASSET_NAME_AR, CATEGORY_ID, SERIAL_NUMBER, PURCHASE_DATE, PURCHASE_PRICE, STATUS, CREATED_BY)
VALUES ('CAR-001', 'تويوتا كامري 2024', 2, 'VIN987654321', TO_DATE('2024-02-01', 'YYYY-MM-DD'), 95000, 'AVAILABLE', 'SYSTEM');

INSERT INTO HR_CORE.COMPANY_ASSETS (ASSET_CODE, ASSET_NAME_AR, CATEGORY_ID, PURCHASE_DATE, PURCHASE_PRICE, STATUS, CREATED_BY)
VALUES ('DESK-001', 'مكتب تنفيذي', 3, TO_DATE('2024-01-10', 'YYYY-MM-DD'), 3500, 'ASSIGNED', 'SYSTEM');

PROMPT ✅ تم إدراج 3 أصول

COMMIT;

PROMPT ========================================
PROMPT ✅ اكتمل إدراج البيانات الأولية
PROMPT ========================================

-- التحقق من البيانات
SELECT 'COUNTRIES: ' || COUNT(*) FROM HR_CORE.COUNTRIES
UNION ALL
SELECT 'CITIES: ' || COUNT(*) FROM HR_CORE.CITIES
UNION ALL
SELECT 'BANKS: ' || COUNT(*) FROM HR_CORE.BANKS
UNION ALL
SELECT 'BRANCHES: ' || COUNT(*) FROM HR_CORE.BRANCHES
UNION ALL
SELECT 'JOB_GRADES: ' || COUNT(*) FROM HR_CORE.JOB_GRADES
UNION ALL
SELECT 'DEPARTMENTS: ' || COUNT(*) FROM HR_CORE.DEPARTMENTS
UNION ALL
SELECT 'JOBS: ' || COUNT(*) FROM HR_CORE.JOBS
UNION ALL
SELECT 'DOCUMENT_TYPES: ' || COUNT(*) FROM HR_CORE.DOCUMENT_TYPES
UNION ALL
SELECT 'SYSTEM_SETTINGS: ' || COUNT(*) FROM HR_CORE.SYSTEM_SETTINGS
UNION ALL
SELECT 'ASSET_CATEGORIES: ' || COUNT(*) FROM HR_CORE.ASSET_CATEGORIES
UNION ALL
SELECT 'COMPANY_ASSETS: ' || COUNT(*) FROM HR_CORE.COMPANY_ASSETS;

PROMPT ========================================
PROMPT إجمالي البيانات المدرجة:
PROMPT - 5 دول
PROMPT - 6 مدن
PROMPT - 4 بنوك
PROMPT - 3 فروع
PROMPT - 4 درجات وظيفية
PROMPT - 5 أقسام
PROMPT - 6 وظائف
PROMPT - 5 أنواع مستندات
PROMPT - 5 إعدادات نظام
PROMPT - 3 فئات أصول
PROMPT - 3 أصول
PROMPT ========================================
