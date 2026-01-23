-- =================================================================================
-- بيانات تجريبية لـ HR_CORE Schema
-- =================================================================================
-- تحتوي على بيانات واقعية لمستشفى
-- جميع البيانات باللغة العربية والإنجليزية
-- =================================================================================

SET DEFINE OFF;
SET SERVEROUTPUT ON;

PROMPT ========================================
PROMPT إدخال البيانات التجريبية لـ HR_CORE
PROMPT ========================================

-- ==========================================================
-- 1. COUNTRIES - الدول
-- ==========================================================
PROMPT إدخال بيانات الدول...

INSERT INTO COUNTRIES (COUNTRY_ID, COUNTRY_NAME_AR, COUNTRY_NAME_EN, COUNTRY_CODE, NATIONALITY_AR, NATIONALITY_EN, IS_DELETED) 
VALUES (1, 'المملكة العربية السعودية', 'Saudi Arabia', 'SA', 'سعودي', 'Saudi', 0);

INSERT INTO COUNTRIES (COUNTRY_ID, COUNTRY_NAME_AR, COUNTRY_NAME_EN, COUNTRY_CODE, NATIONALITY_AR, NATIONALITY_EN, IS_DELETED) 
VALUES (2, 'مصر', 'Egypt', 'EG', 'مصري', 'Egyptian', 0);

INSERT INTO COUNTRIES (COUNTRY_ID, COUNTRY_NAME_AR, COUNTRY_NAME_EN, COUNTRY_CODE, NATIONALITY_AR, NATIONALITY_EN, IS_DELETED) 
VALUES (3, 'الأردن', 'Jordan', 'JO', 'أردني', 'Jordanian', 0);

INSERT INTO COUNTRIES (COUNTRY_ID, COUNTRY_NAME_AR, COUNTRY_NAME_EN, COUNTRY_CODE, NATIONALITY_AR, NATIONALITY_EN, IS_DELETED) 
VALUES (4, 'الإمارات العربية المتحدة', 'United Arab Emirates', 'AE', 'إماراتي', 'Emirati', 0);

INSERT INTO COUNTRIES (COUNTRY_ID, COUNTRY_NAME_AR, COUNTRY_NAME_EN, COUNTRY_CODE, NATIONALITY_AR, NATIONALITY_EN, IS_DELETED) 
VALUES (5, 'الكويت', 'Kuwait', 'KW', 'كويتي', 'Kuwaiti', 0);

INSERT INTO COUNTRIES (COUNTRY_ID, COUNTRY_NAME_AR, COUNTRY_NAME_EN, COUNTRY_CODE, NATIONALITY_AR, NATIONALITY_EN, IS_DELETED) 
VALUES (6, 'البحرين', 'Bahrain', 'BH', 'بحريني', 'Bahraini', 0);

INSERT INTO COUNTRIES (COUNTRY_ID, COUNTRY_NAME_AR, COUNTRY_NAME_EN, COUNTRY_CODE, NATIONALITY_AR, NATIONALITY_EN, IS_DELETED) 
VALUES (7, 'قطر', 'Qatar', 'QA', 'قطري', 'Qatari', 0);

INSERT INTO COUNTRIES (COUNTRY_ID, COUNTRY_NAME_AR, COUNTRY_NAME_EN, COUNTRY_CODE, NATIONALITY_AR, NATIONALITY_EN, IS_DELETED) 
VALUES (8, 'عمان', 'Oman', 'OM', 'عماني', 'Omani', 0);

INSERT INTO COUNTRIES (COUNTRY_ID, COUNTRY_NAME_AR, COUNTRY_NAME_EN, COUNTRY_CODE, NATIONALITY_AR, NATIONALITY_EN, IS_DELETED) 
VALUES (9, 'اليمن', 'Yemen', 'YE', 'يمني', 'Yemeni', 0);

INSERT INTO COUNTRIES (COUNTRY_ID, COUNTRY_NAME_AR, COUNTRY_NAME_EN, COUNTRY_CODE, NATIONALITY_AR, NATIONALITY_EN, IS_DELETED) 
VALUES (10, 'سوريا', 'Syria', 'SY', 'سوري', 'Syrian', 0);

COMMIT;

-- ==========================================================
-- 2. CITIES - المدن
-- ==========================================================
PROMPT إدخال بيانات المدن...

INSERT INTO CITIES (CITY_ID, CITY_NAME_AR, CITY_NAME_EN, COUNTRY_ID, IS_DELETED) 
VALUES (1, 'الرياض', 'Riyadh', 1, 0);

INSERT INTO CITIES (CITY_ID, CITY_NAME_AR, CITY_NAME_EN, COUNTRY_ID, IS_DELETED) 
VALUES (2, 'جدة', 'Jeddah', 1, 0);

INSERT INTO CITIES (CITY_ID, CITY_NAME_AR, CITY_NAME_EN, COUNTRY_ID, IS_DELETED) 
VALUES (3, 'مكة المكرمة', 'Makkah', 1, 0);

INSERT INTO CITIES (CITY_ID, CITY_NAME_AR, CITY_NAME_EN, COUNTRY_ID, IS_DELETED) 
VALUES (4, 'المدينة المنورة', 'Madinah', 1, 0);

INSERT INTO CITIES (CITY_ID, CITY_NAME_AR, CITY_NAME_EN, COUNTRY_ID, IS_DELETED) 
VALUES (5, 'الدمام', 'Dammam', 1, 0);

INSERT INTO CITIES (CITY_ID, CITY_NAME_AR, CITY_NAME_EN, COUNTRY_ID, IS_DELETED) 
VALUES (6, 'الخبر', 'Khobar', 1, 0);

INSERT INTO CITIES (CITY_ID, CITY_NAME_AR, CITY_NAME_EN, COUNTRY_ID, IS_DELETED) 
VALUES (7, 'أبها', 'Abha', 1, 0);

INSERT INTO CITIES (CITY_ID, CITY_NAME_AR, CITY_NAME_EN, COUNTRY_ID, IS_DELETED) 
VALUES (8, 'تبوك', 'Tabuk', 1, 0);

INSERT INTO CITIES (CITY_ID, CITY_NAME_AR, CITY_NAME_EN, COUNTRY_ID, IS_DELETED) 
VALUES (9, 'القاهرة', 'Cairo', 2, 0);

INSERT INTO CITIES (CITY_ID, CITY_NAME_AR, CITY_NAME_EN, COUNTRY_ID, IS_DELETED) 
VALUES (10, 'عمان', 'Amman', 3, 0);

COMMIT;

-- ==========================================================
-- 3. BANKS - البنوك
-- ==========================================================
PROMPT إدخال بيانات البنوك...

INSERT INTO BANKS (BANK_ID, BANK_NAME_AR, BANK_NAME_EN, BANK_CODE, SWIFT_CODE, IS_DELETED) 
VALUES (1, 'البنك الأهلي السعودي', 'Al Ahli Bank', '10', 'NCBKSAJE', 0);

INSERT INTO BANKS (BANK_ID, BANK_NAME_AR, BANK_NAME_EN, BANK_CODE, SWIFT_CODE, IS_DELETED) 
VALUES (2, 'بنك الراجحي', 'Al Rajhi Bank', '80', 'RJHISARI', 0);

INSERT INTO BANKS (BANK_ID, BANK_NAME_AR, BANK_NAME_EN, BANK_CODE, SWIFT_CODE, IS_DELETED) 
VALUES (3, 'بنك الرياض', 'Riyad Bank', '20', 'RIBLSARI', 0);

INSERT INTO BANKS (BANK_ID, BANK_NAME_AR, BANK_NAME_EN, BANK_CODE, SWIFT_CODE, IS_DELETED) 
VALUES (4, 'البنك السعودي للاستثمار', 'Saudi Investment Bank', '05', 'SIBCSARI', 0);

INSERT INTO BANKS (BANK_ID, BANK_NAME_AR, BANK_NAME_EN, BANK_CODE, SWIFT_CODE, IS_DELETED) 
VALUES (5, 'البنك السعودي الفرنسي', 'Banque Saudi Fransi', '55', 'BSFRSARI', 0);

INSERT INTO BANKS (BANK_ID, BANK_NAME_AR, BANK_NAME_EN, BANK_CODE, SWIFT_CODE, IS_DELETED) 
VALUES (6, 'البنك العربي الوطني', 'Arab National Bank', '45', 'ARNBSARI', 0);

COMMIT;

-- ==========================================================
-- 4. JOB_GRADES - الدرجات الوظيفية
-- ==========================================================
PROMPT إدخال بيانات الدرجات الوظيفية...

INSERT INTO JOB_GRADES (GRADE_ID, GRADE_NAME, MIN_SALARY, MAX_SALARY, TICKET_CLASS, ANNUAL_LEAVE_DAYS, IS_DELETED) 
VALUES (1, 'الدرجة الأولى - تنفيذي', 5000, 8000, 'اقتصادية', 21, 0);

INSERT INTO JOB_GRADES (GRADE_ID, GRADE_NAME, MIN_SALARY, MAX_SALARY, TICKET_CLASS, ANNUAL_LEAVE_DAYS, IS_DELETED) 
VALUES (2, 'الدرجة الثانية - مشرف', 8000, 12000, 'اقتصادية', 25, 0);

INSERT INTO JOB_GRADES (GRADE_ID, GRADE_NAME, MIN_SALARY, MAX_SALARY, TICKET_CLASS, ANNUAL_LEAVE_DAYS, IS_DELETED) 
VALUES (3, 'الدرجة الثالثة - أخصائي', 12000, 18000, 'درجة أعمال', 30, 0);

INSERT INTO JOB_GRADES (GRADE_ID, GRADE_NAME, MIN_SALARY, MAX_SALARY, TICKET_CLASS, ANNUAL_LEAVE_DAYS, IS_DELETED) 
VALUES (4, 'الدرجة الرابعة - مدير', 18000, 25000, 'درجة أعمال', 35, 0);

INSERT INTO JOB_GRADES (GRADE_ID, GRADE_NAME, MIN_SALARY, MAX_SALARY, TICKET_CLASS, ANNUAL_LEAVE_DAYS, IS_DELETED) 
VALUES (5, 'الدرجة الخامسة - مدير عام', 25000, 40000, 'درجة أولى', 40, 0);

INSERT INTO JOB_GRADES (GRADE_ID, GRADE_NAME, MIN_SALARY, MAX_SALARY, TICKET_CLASS, ANNUAL_LEAVE_DAYS, IS_DELETED) 
VALUES (6, 'الدرجة السادسة - تنفيذي أول', 40000, 60000, 'درجة أولى', 45, 0);

COMMIT;

-- ==========================================================
-- 5. BRANCHES - الفروع
-- ==========================================================
PROMPT إدخال بيانات الفروع...

INSERT INTO BRANCHES (BRANCH_ID, BRANCH_NAME_AR, BRANCH_NAME_EN, CITY_ID, ADDRESS_AR, ADDRESS_EN, PHONE, EMAIL, IS_DELETED) 
VALUES (1, 'الفرع الرئيسي - الرياض', 'Main Branch - Riyadh', 1, 'طريق الملك فهد، الرياض', 'King Fahd Road, Riyadh', '+966112345678', 'riyadh@hospital.sa', 0);

INSERT INTO BRANCHES (BRANCH_ID, BRANCH_NAME_AR, BRANCH_NAME_EN, CITY_ID, ADDRESS_AR, ADDRESS_EN, PHONE, EMAIL, IS_DELETED) 
VALUES (2, 'فرع جدة', 'Jeddah Branch', 2, 'طريق المدينة، جدة', 'Madinah Road, Jeddah', '+966122345678', 'jeddah@hospital.sa', 0);

INSERT INTO BRANCHES (BRANCH_ID, BRANCH_NAME_AR, BRANCH_NAME_EN, CITY_ID, ADDRESS_AR, ADDRESS_EN, PHONE, EMAIL, IS_DELETED) 
VALUES (3, 'فرع الدمام', 'Dammam Branch', 5, 'طريق الخليج، الدمام', 'Gulf Road, Dammam', '+966132345678', 'dammam@hospital.sa', 0);

INSERT INTO BRANCHES (BRANCH_ID, BRANCH_NAME_AR, BRANCH_NAME_EN, CITY_ID, ADDRESS_AR, ADDRESS_EN, PHONE, EMAIL, IS_DELETED) 
VALUES (4, 'فرع مكة المكرمة', 'Makkah Branch', 3, 'طريق الحرم، مكة المكرمة', 'Haram Road, Makkah', '+966125678901', 'makkah@hospital.sa', 0);

COMMIT;

-- ==========================================================
-- 6. DEPARTMENTS - الأقسام
-- ==========================================================
PROMPT إدخال بيانات الأقسام...

-- الأقسام الرئيسية
INSERT INTO DEPARTMENTS (DEPT_ID, DEPT_NAME_AR, DEPT_NAME_EN, PARENT_DEPT_ID, BRANCH_ID, PHONE, EMAIL, IS_DELETED) 
VALUES (1, 'الإدارة العامة', 'General Management', NULL, 1, '+966112345678', 'management@hospital.sa', 0);

INSERT INTO DEPARTMENTS (DEPT_ID, DEPT_NAME_AR, DEPT_NAME_EN, PARENT_DEPT_ID, BRANCH_ID, PHONE, EMAIL, IS_DELETED) 
VALUES (2, 'الموارد البشرية', 'Human Resources', 1, 1, '+966112345679', 'hr@hospital.sa', 0);

INSERT INTO DEPARTMENTS (DEPT_ID, DEPT_NAME_AR, DEPT_NAME_EN, PARENT_DEPT_ID, BRANCH_ID, PHONE, EMAIL, IS_DELETED) 
VALUES (3, 'الشؤون المالية', 'Finance', 1, 1, '+966112345680', 'finance@hospital.sa', 0);

INSERT INTO DEPARTMENTS (DEPT_ID, DEPT_NAME_AR, DEPT_NAME_EN, PARENT_DEPT_ID, BRANCH_ID, PHONE, EMAIL, IS_DELETED) 
VALUES (4, 'الشؤون الطبية', 'Medical Affairs', 1, 1, '+966112345681', 'medical@hospital.sa', 0);

INSERT INTO DEPARTMENTS (DEPT_ID, DEPT_NAME_AR, DEPT_NAME_EN, PARENT_DEPT_ID, BRANCH_ID, PHONE, EMAIL, IS_DELETED) 
VALUES (5, 'التمريض', 'Nursing', 4, 1, '+966112345682', 'nursing@hospital.sa', 0);

INSERT INTO DEPARTMENTS (DEPT_ID, DEPT_NAME_AR, DEPT_NAME_EN, PARENT_DEPT_ID, BRANCH_ID, PHONE, EMAIL, IS_DELETED) 
VALUES (6, 'الطوارئ', 'Emergency', 4, 1, '+966112345683', 'emergency@hospital.sa', 0);

INSERT INTO DEPARTMENTS (DEPT_ID, DEPT_NAME_AR, DEPT_NAME_EN, PARENT_DEPT_ID, BRANCH_ID, PHONE, EMAIL, IS_DELETED) 
VALUES (7, 'العمليات الجراحية', 'Surgery', 4, 1, '+966112345684', 'surgery@hospital.sa', 0);

INSERT INTO DEPARTMENTS (DEPT_ID, DEPT_NAME_AR, DEPT_NAME_EN, PARENT_DEPT_ID, BRANCH_ID, PHONE, EMAIL, IS_DELETED) 
VALUES (8, 'الأشعة', 'Radiology', 4, 1, '+966112345685', 'radiology@hospital.sa', 0);

INSERT INTO DEPARTMENTS (DEPT_ID, DEPT_NAME_AR, DEPT_NAME_EN, PARENT_DEPT_ID, BRANCH_ID, PHONE, EMAIL, IS_DELETED) 
VALUES (9, 'المختبرات', 'Laboratory', 4, 1, '+966112345686', 'lab@hospital.sa', 0);

INSERT INTO DEPARTMENTS (DEPT_ID, DEPT_NAME_AR, DEPT_NAME_EN, PARENT_DEPT_ID, BRANCH_ID, PHONE, EMAIL, IS_DELETED) 
VALUES (10, 'الصيدلية', 'Pharmacy', 4, 1, '+966112345687', 'pharmacy@hospital.sa', 0);

INSERT INTO DEPARTMENTS (DEPT_ID, DEPT_NAME_AR, DEPT_NAME_EN, PARENT_DEPT_ID, BRANCH_ID, PHONE, EMAIL, IS_DELETED) 
VALUES (11, 'تقنية المعلومات', 'IT Department', 1, 1, '+966112345688', 'it@hospital.sa', 0);

INSERT INTO DEPARTMENTS (DEPT_ID, DEPT_NAME_AR, DEPT_NAME_EN, PARENT_DEPT_ID, BRANCH_ID, PHONE, EMAIL, IS_DELETED) 
VALUES (12, 'خدمة العملاء', 'Customer Service', 1, 1, '+966112345689', 'cs@hospital.sa', 0);

COMMIT;

-- ==========================================================
-- 7. JOBS - الوظائف
-- ==========================================================
PROMPT إدخال بيانات الوظائف...

INSERT INTO JOBS (JOB_ID, JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, DEPT_ID, JOB_DESCRIPTION, IS_DELETED) 
VALUES (1, 'مدير عام', 'General Manager', 6, 1, 'الإشراف العام على المستشفى', 0);

INSERT INTO JOBS (JOB_ID, JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, DEPT_ID, JOB_DESCRIPTION, IS_DELETED) 
VALUES (2, 'مدير الموارد البشرية', 'HR Manager', 4, 2, 'إدارة شؤون الموظفين والتوظيف', 0);

INSERT INTO JOBS (JOB_ID, JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, DEPT_ID, JOB_DESCRIPTION, IS_DELETED) 
VALUES (3, 'أخصائي موارد بشرية', 'HR Specialist', 3, 2, 'تنفيذ سياسات الموارد البشرية', 0);

INSERT INTO JOBS (JOB_ID, JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, DEPT_ID, JOB_DESCRIPTION, IS_DELETED) 
VALUES (4, 'مدير مالي', 'Finance Manager', 4, 3, 'إدارة الشؤون المالية والمحاسبية', 0);

INSERT INTO JOBS (JOB_ID, JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, DEPT_ID, JOB_DESCRIPTION, IS_DELETED) 
VALUES (5, 'محاسب', 'Accountant', 2, 3, 'القيام بالعمليات المحاسبية', 0);

INSERT INTO JOBS (JOB_ID, JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, DEPT_ID, JOB_DESCRIPTION, IS_DELETED) 
VALUES (6, 'طبيب استشاري', 'Consultant Doctor', 5, 4, 'تقديم الاستشارات الطبية المتخصصة', 0);

INSERT INTO JOBS (JOB_ID, JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, DEPT_ID, JOB_DESCRIPTION, IS_DELETED) 
VALUES (7, 'طبيب عام', 'General Practitioner', 4, 4, 'تقديم الخدمات الطبية العامة', 0);

INSERT INTO JOBS (JOB_ID, JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, DEPT_ID, JOB_DESCRIPTION, IS_DELETED) 
VALUES (8, 'ممرض/ممرضة', 'Nurse', 2, 5, 'تقديم الرعاية التمريضية للمرضى', 0);

INSERT INTO JOBS (JOB_ID, JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, DEPT_ID, JOB_DESCRIPTION, IS_DELETED) 
VALUES (9, 'رئيس قسم التمريض', 'Head Nurse', 3, 5, 'الإشراف على طاقم التمريض', 0);

INSERT INTO JOBS (JOB_ID, JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, DEPT_ID, JOB_DESCRIPTION, IS_DELETED) 
VALUES (10, 'طبيب طوارئ', 'Emergency Doctor', 4, 6, 'التعامل مع حالات الطوارئ', 0);

INSERT INTO JOBS (JOB_ID, JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, DEPT_ID, JOB_DESCRIPTION, IS_DELETED) 
VALUES (11, 'جراح', 'Surgeon', 5, 7, 'إجراء العمليات الجراحية', 0);

INSERT INTO JOBS (JOB_ID, JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, DEPT_ID, JOB_DESCRIPTION, IS_DELETED) 
VALUES (12, 'أخصائي أشعة', 'Radiologist', 4, 8, 'قراءة وتفسير الأشعة', 0);

INSERT INTO JOBS (JOB_ID, JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, DEPT_ID, JOB_DESCRIPTION, IS_DELETED) 
VALUES (13, 'فني مختبر', 'Lab Technician', 2, 9, 'إجراء التحاليل المخبرية', 0);

INSERT INTO JOBS (JOB_ID, JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, DEPT_ID, JOB_DESCRIPTION, IS_DELETED) 
VALUES (14, 'صيدلي', 'Pharmacist', 3, 10, 'صرف الأدوية والاستشارات الدوائية', 0);

INSERT INTO JOBS (JOB_ID, JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, DEPT_ID, JOB_DESCRIPTION, IS_DELETED) 
VALUES (15, 'مدير تقنية المعلومات', 'IT Manager', 4, 11, 'إدارة البنية التحتية التقنية', 0);

INSERT INTO JOBS (JOB_ID, JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, DEPT_ID, JOB_DESCRIPTION, IS_DELETED) 
VALUES (16, 'مبرمج', 'Programmer', 3, 11, 'تطوير وصيانة الأنظمة', 0);

INSERT INTO JOBS (JOB_ID, JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, DEPT_ID, JOB_DESCRIPTION, IS_DELETED) 
VALUES (17, 'موظف استقبال', 'Receptionist', 1, 12, 'استقبال المرضى والزوار', 0);

INSERT INTO JOBS (JOB_ID, JOB_TITLE_AR, JOB_TITLE_EN, DEFAULT_GRADE_ID, DEPT_ID, JOB_DESCRIPTION, IS_DELETED) 
VALUES (18, 'مشرف خدمة العملاء', 'Customer Service Supervisor', 2, 12, 'الإشراف على خدمة العملاء', 0);

COMMIT;

-- ==========================================================
-- 8. DOCUMENT_TYPES - أنواع الوثائق
-- ==========================================================
PROMPT إدخال بيانات أنواع الوثائق...

INSERT INTO DOCUMENT_TYPES (DOC_TYPE_ID, DOC_TYPE_NAME_AR, DOC_TYPE_NAME_EN, IS_REQUIRED, IS_DELETED) 
VALUES (1, 'الهوية الوطنية', 'National ID', 1, 0);

INSERT INTO DOCUMENT_TYPES (DOC_TYPE_ID, DOC_TYPE_NAME_AR, DOC_TYPE_NAME_EN, IS_REQUIRED, IS_DELETED) 
VALUES (2, 'جواز السفر', 'Passport', 1, 0);

INSERT INTO DOCUMENT_TYPES (DOC_TYPE_ID, DOC_TYPE_NAME_AR, DOC_TYPE_NAME_EN, IS_REQUIRED, IS_DELETED) 
VALUES (3, 'الشهادة الجامعية', 'University Degree', 1, 0);

INSERT INTO DOCUMENT_TYPES (DOC_TYPE_ID, DOC_TYPE_NAME_AR, DOC_TYPE_NAME_EN, IS_REQUIRED, IS_DELETED) 
VALUES (4, 'شهادة الخبرة', 'Experience Certificate', 0, 0);

INSERT INTO DOCUMENT_TYPES (DOC_TYPE_ID, DOC_TYPE_NAME_AR, DOC_TYPE_NAME_EN, IS_REQUIRED, IS_DELETED) 
VALUES (5, 'التقرير الطبي', 'Medical Report', 1, 0);

INSERT INTO DOCUMENT_TYPES (DOC_TYPE_ID, DOC_TYPE_NAME_AR, DOC_TYPE_NAME_EN, IS_REQUIRED, IS_DELETED) 
VALUES (6, 'شهادة حسن السيرة والسلوك', 'Good Conduct Certificate', 1, 0);

INSERT INTO DOCUMENT_TYPES (DOC_TYPE_ID, DOC_TYPE_NAME_AR, DOC_TYPE_NAME_EN, IS_REQUIRED, IS_DELETED) 
VALUES (7, 'رخصة مزاولة المهنة', 'Professional License', 1, 0);

INSERT INTO DOCUMENT_TYPES (DOC_TYPE_ID, DOC_TYPE_NAME_AR, DOC_TYPE_NAME_EN, IS_REQUIRED, IS_DELETED) 
VALUES (8, 'صورة شخصية', 'Personal Photo', 1, 0);

INSERT INTO DOCUMENT_TYPES (DOC_TYPE_ID, DOC_TYPE_NAME_AR, DOC_TYPE_NAME_EN, IS_REQUIRED, IS_DELETED) 
VALUES (9, 'شهادة الميلاد', 'Birth Certificate', 0, 0);

INSERT INTO DOCUMENT_TYPES (DOC_TYPE_ID, DOC_TYPE_NAME_AR, DOC_TYPE_NAME_EN, IS_REQUIRED, IS_DELETED) 
VALUES (10, 'عقد العمل', 'Employment Contract', 1, 0);

COMMIT;

PROMPT ========================================
PROMPT ✅ تم إدخال جميع البيانات بنجاح!
PROMPT ========================================
PROMPT
PROMPT البيانات المدخلة:
PROMPT - 10 دول
PROMPT - 10 مدن
PROMPT - 6 بنوك
PROMPT - 6 درجات وظيفية
PROMPT - 4 فروع
PROMPT - 12 قسم
PROMPT - 18 وظيفة
PROMPT - 10 أنواع وثائق
PROMPT
PROMPT جاهزة للاستخدام في APEX!
PROMPT ========================================
