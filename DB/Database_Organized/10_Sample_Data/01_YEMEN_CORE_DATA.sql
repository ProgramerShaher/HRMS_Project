-- =================================================================================
-- Sample Data for HR_CORE - Yemen Specific
-- Description: Complete sample data for all HR_CORE tables (Yemen context)
-- =================================================================================

SET SERVEROUTPUT ON;
ALTER SESSION SET CONTAINER = FREEPDB1;
CONNECT HR_CORE/Pwd_Core_123@FREEPDB1;

PROMPT ========================================
PROMPT Inserting Sample Data for HR_CORE
PROMPT Context: Yemen (اليمن)
PROMPT ========================================

-- ==========================================================
-- 1. COUNTRIES (الدول)
-- ==========================================================
PROMPT [1/9] Inserting Countries...

INSERT INTO HR_CORE.COUNTRIES (COUNTRY_NAME_AR, COUNTRY_NAME_EN, CITIZENSHIP_NAME_AR, ISO_CODE, CREATED_BY) 
VALUES ('اليمن', 'Yemen', 'يمني', 'YE', 'SYSTEM');

INSERT INTO HR_CORE.COUNTRIES (COUNTRY_NAME_AR, COUNTRY_NAME_EN, CITIZENSHIP_NAME_AR, ISO_CODE, CREATED_BY) 
VALUES ('السعودية', 'Saudi Arabia', 'سعودي', 'SA', 'SYSTEM');

INSERT INTO HR_CORE.COUNTRIES (COUNTRY_NAME_AR, COUNTRY_NAME_EN, CITIZENSHIP_NAME_AR, ISO_CODE, CREATED_BY) 
VALUES ('مصر', 'Egypt', 'مصري', 'EG', 'SYSTEM');

INSERT INTO HR_CORE.COUNTRIES (COUNTRY_NAME_AR, COUNTRY_NAME_EN, CITIZENSHIP_NAME_AR, ISO_CODE, CREATED_BY) 
VALUES ('الأردن', 'Jordan', 'أردني', 'JO', 'SYSTEM');

INSERT INTO HR_CORE.COUNTRIES (COUNTRY_NAME_AR, COUNTRY_NAME_EN, CITIZENSHIP_NAME_AR, ISO_CODE, CREATED_BY) 
VALUES ('سوريا', 'Syria', 'سوري', 'SY', 'SYSTEM');

COMMIT;
PROMPT ✅ Inserted 5 countries

-- ==========================================================
-- 2. CITIES (المدن اليمنية)
-- ==========================================================
PROMPT [2/9] Inserting Yemeni Cities...

INSERT INTO HR_CORE.CITIES (COUNTRY_ID, CITY_NAME_AR, CITY_NAME_EN, CREATED_BY) 
VALUES (1, 'صنعاء', 'Sana''a', 'SYSTEM');

INSERT INTO HR_CORE.CITIES (COUNTRY_ID, CITY_NAME_AR, CITY_NAME_EN, CREATED_BY) 
VALUES (1, 'عدن', 'Aden', 'SYSTEM');

INSERT INTO HR_CORE.CITIES (COUNTRY_ID, CITY_NAME_AR, CITY_NAME_EN, CREATED_BY) 
VALUES (1, 'تعز', 'Taiz', 'SYSTEM');

INSERT INTO HR_CORE.CITIES (COUNTRY_ID, CITY_NAME_AR, CITY_NAME_EN, CREATED_BY) 
VALUES (1, 'الحديدة', 'Hodeidah', 'SYSTEM');

INSERT INTO HR_CORE.CITIES (COUNTRY_ID, CITY_NAME_AR, CITY_NAME_EN, CREATED_BY) 
VALUES (1, 'إب', 'Ibb', 'SYSTEM');

INSERT INTO HR_CORE.CITIES (COUNTRY_ID, CITY_NAME_AR, CITY_NAME_EN, CREATED_BY) 
VALUES (1, 'ذمار', 'Dhamar', 'SYSTEM');

INSERT INTO HR_CORE.CITIES (COUNTRY_ID, CITY_NAME_AR, CITY_NAME_EN, CREATED_BY) 
VALUES (1, 'المكلا', 'Mukalla', 'SYSTEM');

INSERT INTO HR_CORE.CITIES (COUNTRY_ID, CITY_NAME_AR, CITY_NAME_EN, CREATED_BY) 
VALUES (1, 'صعدة', 'Saada', 'SYSTEM');

COMMIT;
PROMPT ✅ Inserted 8 Yemeni cities

-- ==========================================================
-- 3. BRANCHES (الفروع)
-- ==========================================================
PROMPT [3/9] Inserting Hospital Branches...

INSERT INTO HR_CORE.BRANCHES (BRANCH_NAME_AR, BRANCH_NAME_EN, CITY_ID, ADDRESS, CREATED_BY) 
VALUES ('مستشفى الثورة - صنعاء', 'Al-Thawra Hospital - Sanaa', 1, 'شارع الزبيري، صنعاء', 'SYSTEM');

INSERT INTO HR_CORE.BRANCHES (BRANCH_NAME_AR, BRANCH_NAME_EN, CITY_ID, ADDRESS, CREATED_BY) 
VALUES ('مستشفى الجمهورية - عدن', 'Al-Jumhuriya Hospital - Aden', 2, 'كريتر، عدن', 'SYSTEM');

INSERT INTO HR_CORE.BRANCHES (BRANCH_NAME_AR, BRANCH_NAME_EN, CITY_ID, ADDRESS, CREATED_BY) 
VALUES ('مستشفى الثورة - تعز', 'Al-Thawra Hospital - Taiz', 3, 'شارع جمال، تعز', 'SYSTEM');

INSERT INTO HR_CORE.BRANCHES (BRANCH_NAME_AR, BRANCH_NAME_EN, CITY_ID, ADDRESS, CREATED_BY) 
VALUES ('مستشفى الحديدة العام', 'Hodeidah General Hospital', 4, 'شارع 26 سبتمبر، الحديدة', 'SYSTEM');

COMMIT;
PROMPT ✅ Inserted 4 hospital branches

-- ==========================================================
-- 4. DEPARTMENTS (الأقسام)
-- ==========================================================
PROMPT [4/9] Inserting Departments...

-- الأقسام الرئيسية
INSERT INTO HR_CORE.DEPARTMENTS (DEPT_NAME_AR, DEPT_NAME_EN, BRANCH_ID, COST_CENTER_CODE, IS_ACTIVE, CREATED_BY) 
VALUES ('قسم الطوارئ', 'Emergency Department', 1, 'CC-001', 1, 'SYSTEM');

INSERT INTO HR_CORE.DEPARTMENTS (DEPT_NAME_AR, DEPT_NAME_EN, BRANCH_ID, COST_CENTER_CODE, IS_ACTIVE, CREATED_BY) 
VALUES ('قسم الباطنية', 'Internal Medicine', 1, 'CC-002', 1, 'SYSTEM');

INSERT INTO HR_CORE.DEPARTMENTS (DEPT_NAME_AR, DEPT_NAME_EN, BRANCH_ID, COST_CENTER_CODE, IS_ACTIVE, CREATED_BY) 
VALUES ('قسم الجراحة', 'Surgery Department', 1, 'CC-003', 1, 'SYSTEM');

INSERT INTO HR_CORE.DEPARTMENTS (DEPT_NAME_AR, DEPT_NAME_EN, BRANCH_ID, COST_CENTER_CODE, IS_ACTIVE, CREATED_BY) 
VALUES ('قسم الأطفال', 'Pediatrics', 1, 'CC-004', 1, 'SYSTEM');

INSERT INTO HR_CORE.DEPARTMENTS (DEPT_NAME_AR, DEPT_NAME_EN, BRANCH_ID, COST_CENTER_CODE, IS_ACTIVE, CREATED_BY) 
VALUES ('قسم النساء والولادة', 'Obstetrics & Gynecology', 1, 'CC-005', 1, 'SYSTEM');

INSERT INTO HR_CORE.DEPARTMENTS (DEPT_NAME_AR, DEPT_NAME_EN, BRANCH_ID, COST_CENTER_CODE, IS_ACTIVE, CREATED_BY) 
VALUES ('قسم العظام', 'Orthopedics', 1, 'CC-006', 1, 'SYSTEM');

INSERT INTO HR_CORE.DEPARTMENTS (DEPT_NAME_AR, DEPT_NAME_EN, BRANCH_ID, COST_CENTER_CODE, IS_ACTIVE, CREATED_BY) 
VALUES ('قسم الأشعة', 'Radiology', 1, 'CC-007', 1, 'SYSTEM');

INSERT INTO HR_CORE.DEPARTMENTS (DEPT_NAME_AR, DEPT_NAME_EN, BRANCH_ID, COST_CENTER_CODE, IS_ACTIVE, CREATED_BY) 
VALUES ('قسم المختبر', 'Laboratory', 1, 'CC-008', 1, 'SYSTEM');

INSERT INTO HR_CORE.DEPARTMENTS (DEPT_NAME_AR, DEPT_NAME_EN, BRANCH_ID, COST_CENTER_CODE, IS_ACTIVE, CREATED_BY) 
VALUES ('قسم الصيدلية', 'Pharmacy', 1, 'CC-009', 1, 'SYSTEM');

INSERT INTO HR_CORE.DEPARTMENTS (DEPT_NAME_AR, DEPT_NAME_EN, BRANCH_ID, COST_CENTER_CODE, IS_ACTIVE, CREATED_BY) 
VALUES ('قسم الموارد البشرية', 'Human Resources', 1, 'CC-010', 1, 'SYSTEM');

INSERT INTO HR_CORE.DEPARTMENTS (DEPT_NAME_AR, DEPT_NAME_EN, BRANCH_ID, COST_CENTER_CODE, IS_ACTIVE, CREATED_BY) 
VALUES ('قسم المالية', 'Finance Department', 1, 'CC-011', 1, 'SYSTEM');

INSERT INTO HR_CORE.DEPARTMENTS (DEPT_NAME_AR, DEPT_NAME_EN, BRANCH_ID, COST_CENTER_CODE, IS_ACTIVE, CREATED_BY) 
VALUES ('قسم تقنية المعلومات', 'IT Department', 1, 'CC-012', 1, 'SYSTEM');

COMMIT;
PROMPT ✅ Inserted 12 departments

-- ==========================================================
-- 5. JOB_GRADES (الدرجات الوظيفية)
-- ==========================================================
PROMPT [5/9] Inserting Job Grades...

INSERT INTO HR_CORE.JOB_GRADES (GRADE_NAME, MIN_SALARY, MAX_SALARY, TICKET_CLASS, CREATED_BY) 
VALUES ('الدرجة الأولى', 300000, 500000, 'BUSINESS', 'SYSTEM');

INSERT INTO HR_CORE.JOB_GRADES (GRADE_NAME, MIN_SALARY, MAX_SALARY, TICKET_CLASS, CREATED_BY) 
VALUES ('الدرجة الثانية', 200000, 300000, 'BUSINESS', 'SYSTEM');

INSERT INTO HR_CORE.JOB_GRADES (GRADE_NAME, MIN_SALARY, MAX_SALARY, TICKET_CLASS, CREATED_BY) 
VALUES ('الدرجة الثالثة', 150000, 200000, 'ECONOMY', 'SYSTEM');

INSERT INTO HR_CORE.JOB_GRADES (GRADE_NAME, MIN_SALARY, MAX_SALARY, TICKET_CLASS, CREATED_BY) 
VALUES ('الدرجة الرابعة', 100000, 150000, 'ECONOMY', 'SYSTEM');

INSERT INTO HR_CORE.JOB_GRADES (GRADE_NAME, MIN_SALARY, MAX_SALARY, TICKET_CLASS, CREATED_BY) 
VALUES ('الدرجة الخامسة', 60000, 100000, 'ECONOMY', 'SYSTEM');

COMMIT;
PROMPT ✅ Inserted 5 job grades

-- ==========================================================
-- 6. JOBS (المسميات الوظيفية)
-- ==========================================================
PROMPT [6/9] Inserting Job Titles...

-- الوظائف الطبية
INSERT INTO HR_CORE.JOBS (JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, IS_MEDICAL, CREATED_BY) 
VALUES ('استشاري', 'Consultant', 1, 1, 'SYSTEM');

INSERT INTO HR_CORE.JOBS (JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, IS_MEDICAL, CREATED_BY) 
VALUES ('أخصائي', 'Specialist', 2, 1, 'SYSTEM');

INSERT INTO HR_CORE.JOBS (JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, IS_MEDICAL, CREATED_BY) 
VALUES ('طبيب عام', 'General Practitioner', 3, 1, 'SYSTEM');

INSERT INTO HR_CORE.JOBS (JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, IS_MEDICAL, CREATED_BY) 
VALUES ('طبيب مقيم', 'Resident Doctor', 4, 1, 'SYSTEM');

INSERT INTO HR_CORE.JOBS (JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, IS_MEDICAL, CREATED_BY) 
VALUES ('ممرض أول', 'Senior Nurse', 3, 1, 'SYSTEM');

INSERT INTO HR_CORE.JOBS (JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, IS_MEDICAL, CREATED_BY) 
VALUES ('ممرض', 'Nurse', 4, 1, 'SYSTEM');

INSERT INTO HR_CORE.JOBS (JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, IS_MEDICAL, CREATED_BY) 
VALUES ('صيدلي', 'Pharmacist', 3, 1, 'SYSTEM');

INSERT INTO HR_CORE.JOBS (JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, IS_MEDICAL, CREATED_BY) 
VALUES ('فني مختبر', 'Lab Technician', 4, 1, 'SYSTEM');

INSERT INTO HR_CORE.JOBS (JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, IS_MEDICAL, CREATED_BY) 
VALUES ('فني أشعة', 'Radiology Technician', 4, 1, 'SYSTEM');

-- الوظائف الإدارية
INSERT INTO HR_CORE.JOBS (JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, IS_MEDICAL, CREATED_BY) 
VALUES ('مدير موارد بشرية', 'HR Manager', 2, 0, 'SYSTEM');

INSERT INTO HR_CORE.JOBS (JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, IS_MEDICAL, CREATED_BY) 
VALUES ('محاسب', 'Accountant', 3, 0, 'SYSTEM');

INSERT INTO HR_CORE.JOBS (JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, IS_MEDICAL, CREATED_BY) 
VALUES ('موظف استقبال', 'Receptionist', 5, 0, 'SYSTEM');

INSERT INTO HR_CORE.JOBS (JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, IS_MEDICAL, CREATED_BY) 
VALUES ('مبرمج', 'Programmer', 3, 0, 'SYSTEM');

INSERT INTO HR_CORE.JOBS (JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, IS_MEDICAL, CREATED_BY) 
VALUES ('سائق إسعاف', 'Ambulance Driver', 5, 0, 'SYSTEM');

COMMIT;
PROMPT ✅ Inserted 14 job titles

-- ==========================================================
-- 7. DOCUMENT_TYPES (أنواع الوثائق)
-- ==========================================================
PROMPT [7/9] Inserting Document Types...

INSERT INTO HR_CORE.DOCUMENT_TYPES (DOC_NAME_AR, IS_MANDATORY, REQUIRES_EXPIRY, ALERT_DAYS_BEFORE, CREATED_BY) 
VALUES ('البطاقة الشخصية', 1, 1, 60, 'SYSTEM');

INSERT INTO HR_CORE.DOCUMENT_TYPES (DOC_NAME_AR, IS_MANDATORY, REQUIRES_EXPIRY, ALERT_DAYS_BEFORE, CREATED_BY) 
VALUES ('جواز السفر', 1, 1, 90, 'SYSTEM');

INSERT INTO HR_CORE.DOCUMENT_TYPES (DOC_NAME_AR, IS_MANDATORY, REQUIRES_EXPIRY, ALERT_DAYS_BEFORE, CREATED_BY) 
VALUES ('رخصة القيادة', 0, 1, 30, 'SYSTEM');

INSERT INTO HR_CORE.DOCUMENT_TYPES (DOC_NAME_AR, IS_MANDATORY, REQUIRES_EXPIRY, ALERT_DAYS_BEFORE, CREATED_BY) 
VALUES ('شهادة التخرج', 1, 0, 0, 'SYSTEM');

INSERT INTO HR_CORE.DOCUMENT_TYPES (DOC_NAME_AR, IS_MANDATORY, REQUIRES_EXPIRY, ALERT_DAYS_BEFORE, CREATED_BY) 
VALUES ('رخصة مزاولة المهنة', 1, 1, 60, 'SYSTEM');

INSERT INTO HR_CORE.DOCUMENT_TYPES (DOC_NAME_AR, IS_MANDATORY, REQUIRES_EXPIRY, ALERT_DAYS_BEFORE, CREATED_BY) 
VALUES ('شهادة الخبرة', 0, 0, 0, 'SYSTEM');

INSERT INTO HR_CORE.DOCUMENT_TYPES (DOC_NAME_AR, IS_MANDATORY, REQUIRES_EXPIRY, ALERT_DAYS_BEFORE, CREATED_BY) 
VALUES ('شهادة حسن السيرة والسلوك', 1, 1, 180, 'SYSTEM');

COMMIT;
PROMPT ✅ Inserted 7 document types

-- ==========================================================
-- 8. BANKS (البنوك اليمنية)
-- ==========================================================
PROMPT [8/9] Inserting Yemeni Banks...

INSERT INTO HR_CORE.BANKS (BANK_NAME_AR, BANK_NAME_EN, BANK_CODE, CREATED_BY) 
VALUES ('البنك المركزي اليمني', 'Central Bank of Yemen', 'CBY', 'SYSTEM');

INSERT INTO HR_CORE.BANKS (BANK_NAME_AR, BANK_NAME_EN, BANK_CODE, CREATED_BY) 
VALUES ('بنك اليمن الدولي', 'Yemen International Bank', 'YIB', 'SYSTEM');

INSERT INTO HR_CORE.BANKS (BANK_NAME_AR, BANK_NAME_EN, BANK_CODE, CREATED_BY) 
VALUES ('بنك التسليف التعاوني والزراعي', 'CAC Bank', 'CACB', 'SYSTEM');

INSERT INTO HR_CORE.BANKS (BANK_NAME_AR, BANK_NAME_EN, BANK_CODE, CREATED_BY) 
VALUES ('بنك اليمن والكويت', 'Yemen Kuwait Bank', 'YKB', 'SYSTEM');

INSERT INTO HR_CORE.BANKS (BANK_NAME_AR, BANK_NAME_EN, BANK_CODE, CREATED_BY) 
VALUES ('بنك التضامن الإسلامي الدولي', 'Tadhamon International Islamic Bank', 'TIIB', 'SYSTEM');

INSERT INTO HR_CORE.BANKS (BANK_NAME_AR, BANK_NAME_EN, BANK_CODE, CREATED_BY) 
VALUES ('بنك سبأ الإسلامي', 'Saba Islamic Bank', 'SIB', 'SYSTEM');

COMMIT;
PROMPT ✅ Inserted 6 Yemeni banks

-- ==========================================================
-- 9. SYSTEM_SETTINGS (إعدادات النظام)
-- ==========================================================
PROMPT [9/9] Inserting System Settings...

INSERT INTO HR_CORE.SYSTEM_SETTINGS (SETTING_KEY, SETTING_VALUE, SETTING_TYPE, DESCRIPTION_AR, CREATED_BY) 
VALUES ('COMPANY_NAME_AR', 'مستشفى الثورة العام', 'STRING', 'اسم المستشفى بالعربي', 'SYSTEM');

INSERT INTO HR_CORE.SYSTEM_SETTINGS (SETTING_KEY, SETTING_VALUE, SETTING_TYPE, DESCRIPTION_AR, CREATED_BY) 
VALUES ('COMPANY_NAME_EN', 'Al-Thawra General Hospital', 'STRING', 'اسم المستشفى بالإنجليزي', 'SYSTEM');

INSERT INTO HR_CORE.SYSTEM_SETTINGS (SETTING_KEY, SETTING_VALUE, SETTING_TYPE, DESCRIPTION_AR, CREATED_BY) 
VALUES ('CURRENCY', 'YER', 'STRING', 'العملة - ريال يمني', 'SYSTEM');

INSERT INTO HR_CORE.SYSTEM_SETTINGS (SETTING_KEY, SETTING_VALUE, SETTING_TYPE, DESCRIPTION_AR, CREATED_BY) 
VALUES ('WORKING_DAYS_PER_WEEK', '6', 'NUMBER', 'أيام العمل في الأسبوع', 'SYSTEM');

INSERT INTO HR_CORE.SYSTEM_SETTINGS (SETTING_KEY, SETTING_VALUE, SETTING_TYPE, DESCRIPTION_AR, CREATED_BY) 
VALUES ('WORKING_HOURS_PER_DAY', '8', 'NUMBER', 'ساعات العمل اليومية', 'SYSTEM');

INSERT INTO HR_CORE.SYSTEM_SETTINGS (SETTING_KEY, SETTING_VALUE, SETTING_TYPE, DESCRIPTION_AR, CREATED_BY) 
VALUES ('ANNUAL_LEAVE_DAYS', '30', 'NUMBER', 'أيام الإجازة السنوية', 'SYSTEM');

INSERT INTO HR_CORE.SYSTEM_SETTINGS (SETTING_KEY, SETTING_VALUE, SETTING_TYPE, DESCRIPTION_AR, CREATED_BY) 
VALUES ('SICK_LEAVE_DAYS', '15', 'NUMBER', 'أيام الإجازة المرضية', 'SYSTEM');

INSERT INTO HR_CORE.SYSTEM_SETTINGS (SETTING_KEY, SETTING_VALUE, SETTING_TYPE, DESCRIPTION_AR, CREATED_BY) 
VALUES ('WEEKEND_FRIDAY', '1', 'BOOLEAN', 'الجمعة عطلة', 'SYSTEM');

INSERT INTO HR_CORE.SYSTEM_SETTINGS (SETTING_KEY, SETTING_VALUE, SETTING_TYPE, DESCRIPTION_AR, CREATED_BY) 
VALUES ('COUNTRY_CODE', 'YE', 'STRING', 'رمز الدولة - اليمن', 'SYSTEM');

COMMIT;
PROMPT ✅ Inserted 9 system settings

PROMPT
PROMPT ========================================
PROMPT Sample Data Insertion Complete!
PROMPT ========================================
PROMPT
PROMPT Summary:
PROMPT - 5 Countries (اليمن + دول عربية)
PROMPT - 8 Yemeni Cities
PROMPT - 4 Hospital Branches
PROMPT - 12 Departments
PROMPT - 5 Job Grades
PROMPT - 14 Job Titles
PROMPT - 7 Document Types
PROMPT - 6 Yemeni Banks
PROMPT - 9 System Settings
PROMPT ========================================
PROMPT
PROMPT Data is ready for use!
PROMPT ========================================
