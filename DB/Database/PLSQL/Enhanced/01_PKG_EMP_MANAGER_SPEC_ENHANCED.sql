-- =================================================================================
-- الحزمة: PKG_EMP_MANAGER (Enhanced Version)
-- الوصف: إدارة الموظفين الكاملة - نظام ERP 100%
-- المخطط: HR_PERSONNEL
-- الإصدار: 2.0 - Enhanced
-- =================================================================================

CREATE OR REPLACE PACKAGE HR_PERSONNEL.PKG_EMP_MANAGER AS

    -- ==========================================================
    -- 1. إدارة الموظفين (Employee Management)
    -- ==========================================================
    
    -- إضافة موظف جديد (كامل مع جميع البيانات)
    PROCEDURE CREATE_EMPLOYEE (
        -- البيانات الأساسية
        p_first_name_ar         IN VARCHAR2,
        p_second_name_ar        IN VARCHAR2,
        p_third_name_ar         IN VARCHAR2,
        p_last_name_ar          IN VARCHAR2,
        p_full_name_en          IN VARCHAR2,
        p_gender                IN CHAR,
        p_birth_date            IN DATE,
        p_marital_status        IN VARCHAR2,
        p_nationality_id        IN NUMBER,
        
        -- البيانات الوظيفية
        p_job_id                IN NUMBER,
        p_dept_id               IN NUMBER,
        p_manager_id            IN NUMBER DEFAULT NULL,
        p_joining_date          IN DATE,
        p_email                 IN VARCHAR2,
        p_mobile                IN VARCHAR2,
        
        -- بيانات العقد
        p_contract_type         IN VARCHAR2,
        p_basic_salary          IN NUMBER,
        p_housing_allowance     IN NUMBER DEFAULT 0,
        p_transport_allowance   IN NUMBER DEFAULT 0,
        p_contract_start_date   IN DATE,
        p_contract_end_date     IN DATE DEFAULT NULL,
        
        -- المخرجات
        p_created_by            IN VARCHAR2,
        o_employee_id           OUT NUMBER,
        o_employee_number       OUT VARCHAR2,
        o_contract_id           OUT NUMBER,
        o_message               OUT VARCHAR2
    );
    
    -- تحديث بيانات الموظف
    PROCEDURE UPDATE_EMPLOYEE (
        p_employee_id           IN NUMBER,
        p_mobile                IN VARCHAR2 DEFAULT NULL,
        p_email                 IN VARCHAR2 DEFAULT NULL,
        p_marital_status        IN VARCHAR2 DEFAULT NULL,
        p_updated_by            IN VARCHAR2,
        o_message               OUT VARCHAR2
    );
    
    -- نقل موظف لقسم آخر
    PROCEDURE TRANSFER_EMPLOYEE (
        p_employee_id           IN NUMBER,
        p_new_dept_id           IN NUMBER,
        p_new_job_id            IN NUMBER DEFAULT NULL,
        p_transfer_date         IN DATE,
        p_reason                IN VARCHAR2,
        p_approved_by           IN NUMBER,
        p_created_by            IN VARCHAR2,
        o_transfer_id           OUT NUMBER,
        o_message               OUT VARCHAR2
    );
    
    -- إنهاء خدمة موظف
    PROCEDURE TERMINATE_EMPLOYEE (
        p_employee_id           IN NUMBER,
        p_termination_date      IN DATE,
        p_reason                IN VARCHAR2,
        p_updated_by            IN VARCHAR2,
        o_eos_amount            OUT NUMBER,
        o_message               OUT VARCHAR2
    );
    
    -- إعادة تفعيل موظف
    PROCEDURE REACTIVATE_EMPLOYEE (
        p_employee_id           IN NUMBER,
        p_updated_by            IN VARCHAR2,
        o_message               OUT VARCHAR2
    );
    
    -- ==========================================================
    -- 2. إدارة العقود (Contract Management)
    -- ==========================================================
    
    -- إنشاء عقد جديد
    PROCEDURE CREATE_CONTRACT (
        p_employee_id           IN NUMBER,
        p_contract_type         IN VARCHAR2,
        p_start_date            IN DATE,
        p_end_date              IN DATE DEFAULT NULL,
        p_basic_salary          IN NUMBER,
        p_housing_allowance     IN NUMBER DEFAULT 0,
        p_transport_allowance   IN NUMBER DEFAULT 0,
        p_other_allowances      IN NUMBER DEFAULT 0,
        p_vacation_days         IN NUMBER DEFAULT 30,
        p_working_hours_daily   IN NUMBER DEFAULT 8,
        p_created_by            IN VARCHAR2,
        o_contract_id           OUT NUMBER,
        o_message               OUT VARCHAR2
    );
    
    -- تجديد عقد
    PROCEDURE RENEW_CONTRACT (
        p_contract_id           IN NUMBER,
        p_new_start_date        IN DATE,
        p_new_end_date          IN DATE,
        p_new_basic_salary      IN NUMBER DEFAULT NULL,
        p_notes                 IN VARCHAR2 DEFAULT NULL,
        p_created_by            IN VARCHAR2,
        o_renewal_id            OUT NUMBER,
        o_message               OUT VARCHAR2
    );
    
    -- إنهاء عقد
    PROCEDURE TERMINATE_CONTRACT (
        p_contract_id           IN NUMBER,
        p_updated_by            IN VARCHAR2,
        o_message               OUT VARCHAR2
    );
    
    -- ==========================================================
    -- 3. إدارة الوثائق (Document Management)
    -- ==========================================================
    
    -- إضافة وثيقة
    PROCEDURE ADD_DOCUMENT (
        p_employee_id           IN NUMBER,
        p_doc_type_id           IN NUMBER,
        p_doc_number            IN VARCHAR2,
        p_issue_date            IN DATE,
        p_expiry_date           IN DATE DEFAULT NULL,
        p_issue_place           IN VARCHAR2 DEFAULT NULL,
        p_attachment_path       IN VARCHAR2 DEFAULT NULL,
        p_created_by            IN VARCHAR2,
        o_doc_id                OUT NUMBER,
        o_message               OUT VARCHAR2
    );
    
    -- تحديث وثيقة
    PROCEDURE UPDATE_DOCUMENT (
        p_doc_id                IN NUMBER,
        p_expiry_date           IN DATE,
        p_attachment_path       IN VARCHAR2 DEFAULT NULL,
        p_updated_by            IN VARCHAR2,
        o_message               OUT VARCHAR2
    );
    
    -- الحصول على الوثائق منتهية الصلاحية
    PROCEDURE GET_EXPIRING_DOCUMENTS (
        p_days_before           IN NUMBER DEFAULT 30,
        o_cursor                OUT SYS_REFCURSOR
    );
    
    -- ==========================================================
    -- 4. إدارة المؤهلات والشهادات
    -- ==========================================================
    
    -- إضافة مؤهل علمي
    PROCEDURE ADD_QUALIFICATION (
        p_employee_id           IN NUMBER,
        p_degree_type           IN VARCHAR2,
        p_major_ar              IN VARCHAR2,
        p_university_ar         IN VARCHAR2,
        p_country_id            IN NUMBER,
        p_graduation_year       IN NUMBER,
        p_grade                 IN VARCHAR2 DEFAULT NULL,
        p_attachment_path       IN VARCHAR2 DEFAULT NULL,
        p_created_by            IN VARCHAR2,
        o_qualification_id      OUT NUMBER,
        o_message               OUT VARCHAR2
    );
    
    -- إضافة شهادة مهنية
    PROCEDURE ADD_CERTIFICATION (
        p_employee_id           IN NUMBER,
        p_cert_name_ar          IN VARCHAR2,
        p_issuing_authority     IN VARCHAR2,
        p_issue_date            IN DATE,
        p_expiry_date           IN DATE DEFAULT NULL,
        p_cert_number           IN VARCHAR2 DEFAULT NULL,
        p_is_mandatory          IN NUMBER DEFAULT 0,
        p_attachment_path       IN VARCHAR2 DEFAULT NULL,
        p_created_by            IN VARCHAR2,
        o_cert_id               OUT NUMBER,
        o_message               OUT VARCHAR2
    );
    
    -- ==========================================================
    -- 5. إدارة الحسابات البنكية
    -- ==========================================================
    
    -- إضافة حساب بنكي
    PROCEDURE ADD_BANK_ACCOUNT (
        p_employee_id           IN NUMBER,
        p_bank_id               IN NUMBER,
        p_account_number        IN VARCHAR2,
        p_iban                  IN VARCHAR2,
        p_is_primary            IN NUMBER DEFAULT 0,
        p_created_by            IN VARCHAR2,
        o_account_id            OUT NUMBER,
        o_message               OUT VARCHAR2
    );
    
    -- تحديد الحساب الرئيسي
    PROCEDURE SET_PRIMARY_ACCOUNT (
        p_account_id            IN NUMBER,
        p_updated_by            IN VARCHAR2,
        o_message               OUT VARCHAR2
    );
    
    -- ==========================================================
    -- 6. إدارة جهات الاتصال للطوارئ
    -- ==========================================================
    
    -- إضافة جهة اتصال طوارئ
    PROCEDURE ADD_EMERGENCY_CONTACT (
        p_employee_id           IN NUMBER,
        p_contact_name_ar       IN VARCHAR2,
        p_relationship          IN VARCHAR2,
        p_phone_primary         IN VARCHAR2,
        p_phone_secondary       IN VARCHAR2 DEFAULT NULL,
        p_is_primary            IN NUMBER DEFAULT 0,
        p_created_by            IN VARCHAR2,
        o_contact_id            OUT NUMBER,
        o_message               OUT VARCHAR2
    );
    
    -- ==========================================================
    -- 7. إدارة العناوين
    -- ==========================================================
    
    -- إضافة عنوان
    PROCEDURE ADD_ADDRESS (
        p_employee_id           IN NUMBER,
        p_address_type          IN VARCHAR2, -- CURRENT, PERMANENT, EMERGENCY
        p_city_id               IN NUMBER,
        p_district              IN VARCHAR2,
        p_street                IN VARCHAR2,
        p_building_no           IN VARCHAR2 DEFAULT NULL,
        p_postal_code           IN VARCHAR2 DEFAULT NULL,
        p_created_by            IN VARCHAR2,
        o_address_id            OUT NUMBER,
        o_message               OUT VARCHAR2
    );
    
    -- ==========================================================
    -- 8. إدارة التابعين (Dependents)
    -- ==========================================================
    
    -- إضافة تابع
    PROCEDURE ADD_DEPENDENT (
        p_employee_id           IN NUMBER,
        p_name_ar               IN VARCHAR2,
        p_relationship          IN VARCHAR2,
        p_birth_date            IN DATE,
        p_national_id           IN VARCHAR2 DEFAULT NULL,
        p_is_eligible_ticket    IN NUMBER DEFAULT 1,
        p_is_eligible_insurance IN NUMBER DEFAULT 1,
        p_created_by            IN VARCHAR2,
        o_dependent_id          OUT NUMBER,
        o_message               OUT VARCHAR2
    );
    
    -- ==========================================================
    -- 9. دوال مساعدة (Helper Functions)
    -- ==========================================================
    
    -- توليد رقم وظيفي تلقائي
    FUNCTION GENERATE_EMPLOYEE_NUMBER RETURN VARCHAR2;
    
    -- حساب العمر
    FUNCTION CALCULATE_AGE (p_birth_date IN DATE) RETURN NUMBER;
    
    -- حساب سنوات الخدمة
    FUNCTION CALCULATE_SERVICE_YEARS (
        p_employee_id IN NUMBER,
        p_as_of_date IN DATE DEFAULT SYSDATE
    ) RETURN NUMBER;
    
    -- التحقق من صلاحية البريد الإلكتروني
    FUNCTION IS_VALID_EMAIL (p_email IN VARCHAR2) RETURN NUMBER;
    
    -- التحقق من صلاحية رقم الجوال
    FUNCTION IS_VALID_MOBILE (p_mobile IN VARCHAR2) RETURN NUMBER;
    
    -- الحصول على الراتب الإجمالي
    FUNCTION GET_TOTAL_SALARY (p_contract_id IN NUMBER) RETURN NUMBER;
    
    -- الحصول على الراتب الأساسي الحالي
    FUNCTION GET_CURRENT_BASIC_SALARY (p_employee_id IN NUMBER) RETURN NUMBER;
    
    -- التحقق من وجود موظف
    FUNCTION EMPLOYEE_EXISTS (p_employee_id IN NUMBER) RETURN NUMBER;
    
    -- الحصول على حالة الموظف
    FUNCTION GET_EMPLOYEE_STATUS (p_employee_id IN NUMBER) RETURN VARCHAR2;
    
    -- ==========================================================
    -- 10. تقارير واستعلامات (Reports & Queries)
    -- ==========================================================
    
    -- الحصول على قائمة الموظفين النشطين
    PROCEDURE GET_ACTIVE_EMPLOYEES (
        p_dept_id               IN NUMBER DEFAULT NULL,
        o_cursor                OUT SYS_REFCURSOR
    );
    
    -- الحصول على تفاصيل موظف كاملة
    PROCEDURE GET_EMPLOYEE_DETAILS (
        p_employee_id           IN NUMBER,
        o_cursor                OUT SYS_REFCURSOR
    );
    
    -- الحصول على العقود المنتهية قريباً
    PROCEDURE GET_EXPIRING_CONTRACTS (
        p_days_before           IN NUMBER DEFAULT 60,
        o_cursor                OUT SYS_REFCURSOR
    );
    
    -- إحصائيات الموظفين
    PROCEDURE GET_EMPLOYEE_STATISTICS (
        o_total_employees       OUT NUMBER,
        o_active_employees      OUT NUMBER,
        o_on_leave              OUT NUMBER,
        o_terminated            OUT NUMBER
    );

END PKG_EMP_MANAGER;
/
