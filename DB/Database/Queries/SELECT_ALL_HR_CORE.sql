/*
    ========================================================================
    📊 تقرير شامل منظم: بيانات وحدة النظام الأساسي (HR_CORE)
    ========================================================================
    الغرض: عرض بيانات الجداول مترابطة (Joins) وبأسماء أعمدة عربية واضحة
*/

-- 1️⃣ تقرير المناطق الجغرافية (الدول والمدن)
-- يظهر اسم المدينة مع اسم الدولة التابعة لها
SELECT 
    ci.CITY_NAME_AR AS "المدينة",
    ci.CITY_NAME_EN AS "City Name",
    co.COUNTRY_NAME_AR AS "الدولة",
    co.COUNTRY_NAME_EN AS "Country Name"
FROM HR_CORE.CITIES ci
JOIN HR_CORE.COUNTRIES co ON ci.COUNTRY_ID = co.COUNTRY_ID
ORDER BY co.COUNTRY_NAME_AR, ci.CITY_NAME_AR;


-- 2️⃣ تقرير الهيكل التنظيمي (الفروع والأقسام)
-- يظهر اسم القسم مع الفرع والمدينة الموجود فيها
SELECT 
    d.DEPT_NAME_AR AS "القسم",
    d.DEPT_NAME_EN AS "Department",
    d.COST_CENTER_CODE AS "مركز التكلفة",
    b.BRANCH_NAME_AR AS "الفرع",
    ci.CITY_NAME_AR AS "مدينة الفرع",
    b.ADDRESS AS "العنوان"
FROM HR_CORE.DEPARTMENTS d
JOIN HR_CORE.BRANCHES b ON d.BRANCH_ID = b.BRANCH_ID
JOIN HR_CORE.CITIES ci ON b.CITY_ID = ci.CITY_ID
ORDER BY b.BRANCH_NAME_AR, d.DEPT_NAME_AR;


-- 3️⃣ تقرير الوظائف وسلم الرواتب
-- يظهر المسمى الوظيفي مع الدرجة المالية وحدود الراتب
SELECT 
    j.JOB_TITLE_AR AS "المسمى الوظيفي",
    j.JOB_TITLE_EN AS "Job Title",
    g.GRADE_NAME AS "المرتبة / الدرجة",
    g.MIN_SALARY AS "الحد الأدنى للراتب",
    g.MAX_SALARY AS "الحد الأعلى للراتب",
    g.TICKET_CLASS AS "فئة التذاكر"
FROM HR_CORE.JOBS j
JOIN HR_CORE.JOB_GRADES g ON j.DEFAULT_GRADE_ID = g.GRADE_ID
ORDER BY g.MIN_SALARY DESC, j.JOB_TITLE_AR;


-- 4️⃣ تقرير البنوك المعتمدة
SELECT 
    BANK_NAME_AR AS "اسم البنك", 
    BANK_NAME_EN AS "Bank Name",
    BANK_CODE AS "رمز البنك (Code)" 
FROM HR_CORE.BANKS 
ORDER BY BANK_NAME_AR;


-- 5️⃣ تقرير أنواع الوثائق
-- يوضح هل الوثيقة إلزامية وهل تتطلب تاريخ انتهاء
SELECT 
    DOC_NAME_AR AS "نوع الوثيقة",
    CASE WHEN IS_MANDATORY = 1 THEN 'نعم' ELSE 'لا' END AS "إلزامية؟",
    CASE WHEN REQUIRES_EXPIRY = 1 THEN 'نعم' ELSE 'لا' END AS "تتطلب تاريخ انتهاء؟",
    ALERT_DAYS_BEFORE AS "التنبيه قبل (يوم)"
FROM HR_CORE.DOCUMENT_TYPES
ORDER BY IS_MANDATORY DESC;


-- 6️⃣ تقرير إعدادات النظام
SELECT 
    DESCRIPTION_AR AS "وصف الإعداد",
    SETTING_VALUE AS "القيمة الحالية",
    SETTING_KEY AS "كود الإعداد"
FROM HR_CORE.SYSTEM_SETTINGS
ORDER BY SETTING_KEY;
