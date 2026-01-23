-- =================================================================================
-- الحزمة: PKG_EMP_MANAGER
-- الوصف: إدارة الموظفين والعقود (إضافة، تعديل، إنهاء خدمة)
-- المخطط: HR_PERSONNEL
-- =================================================================================

/* 
   تعليمات التنفيذ:
   1. تأكد من الاتصال بمستخدم HR_PERSONNEL أو أدمن
   2. نفذ الـ SPECIFICATION أولاً
   3. نفذ الـ BODY ثانياً
*/

-- =================================================================================
-- 1. SPECIFICATION (الواجهة الخارجية - الدوال المتاحة للاستدعاء)
-- =================================================================================
CREATE OR REPLACE PACKAGE HR_PERSONNEL.PKG_EMP_MANAGER AS

    -- إجراء لإضافة موظف جديد مع عقده الأول في خطوة واحدة (Transactional)
    PROCEDURE CREATE_NEW_EMPLOYEE (
        p_first_name_ar      IN VARCHAR2,
        p_family_name_ar     IN VARCHAR2,
        p_full_name_en       IN VARCHAR2,
        p_national_id        IN VARCHAR2, -- يستخدم للتحقق من التكرار
        p_nationality_id     IN NUMBER,
        p_birth_date         IN DATE,
        p_gender             IN VARCHAR2,
        p_job_id            IN NUMBER,
        p_dept_id           IN NUMBER,
        p_basic_salary      IN NUMBER,
        p_joining_date      IN DATE,
        
        -- مخرجات
        o_employee_id       OUT NUMBER,
        o_employee_number   OUT VARCHAR2
    );

    -- إجراء لتحديث بيانات الموظف الأساسية
    PROCEDURE UPDATE_EMPLOYEE_INFO (
        p_employee_id       IN NUMBER,
        p_mobile            IN VARCHAR2,
        p_email             IN VARCHAR2,
        p_marital_status    IN VARCHAR2
    );

    -- دالة لحساب الراتب الإجمالي (للعرض فقط)
    FUNCTION GET_TOTAL_SALARY (p_contract_id IN NUMBER) RETURN NUMBER;

END PKG_EMP_MANAGER;
/
