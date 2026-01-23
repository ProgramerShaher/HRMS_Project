-- =================================================================================
-- عروض لوحة التحكم - Dashboard Views
-- =================================================================================
-- معدّلة لـ Oracle APEX Cloud (بدون بادئة Schema)
-- تحتوي على 8 عروض جاهزة للاستخدام في APEX
-- =================================================================================

SET DEFINE OFF;
SET SERVEROUTPUT ON;

PROMPT ========================================
PROMPT إنشاء عروض لوحة التحكم
PROMPT ========================================

-- ==========================================================
-- 1. V_DASHBOARD_STATS - الإحصائيات الرئيسية
-- ==========================================================
-- الاستخدام في APEX: Badge List, Value Cards
-- ==========================================================
CREATE OR REPLACE VIEW V_DASHBOARD_STATS AS
SELECT 
    (SELECT COUNT(*) FROM DEPARTMENTS WHERE IS_DELETED = 0) AS TOTAL_DEPARTMENTS,
    (SELECT COUNT(*) FROM JOBS WHERE IS_DELETED = 0) AS TOTAL_JOBS,
    (SELECT COUNT(*) FROM BRANCHES WHERE IS_DELETED = 0) AS TOTAL_BRANCHES,
    (SELECT COUNT(*) FROM BANKS WHERE IS_DELETED = 0) AS TOTAL_BANKS,
    (SELECT COUNT(*) FROM CITIES WHERE IS_DELETED = 0) AS TOTAL_CITIES,
    (SELECT COUNT(*) FROM COUNTRIES WHERE IS_DELETED = 0) AS TOTAL_COUNTRIES,
    (SELECT COUNT(*) FROM JOB_GRADES WHERE IS_DELETED = 0) AS TOTAL_GRADES,
    (SELECT COUNT(*) FROM DOCUMENT_TYPES WHERE IS_DELETED = 0) AS TOTAL_DOC_TYPES
FROM DUAL;

COMMENT ON TABLE V_DASHBOARD_STATS IS 'إحصائيات لوحة التحكم - المقاييس الرئيسية';

-- ==========================================================
-- 2. V_NAVIGATION_CARDS - بطاقات التنقل
-- ==========================================================
-- الاستخدام في APEX: Cards Region
-- ==========================================================
CREATE OR REPLACE VIEW V_NAVIGATION_CARDS AS
SELECT 
    'الأقسام' AS TITLE,
    'إدارة الهيكل التنظيمي للمستشفى' AS SUBTITLE,
    'fa-hospital-o' AS ICON,
    '#0088CC' AS COLOR,
    'DEPARTMENTS' AS TARGET_PAGE,
    1 AS CARD_ORDER
FROM DUAL
UNION ALL
SELECT 
    'الوظائف',
    'المسميات الوظيفية والدرجات',
    'fa-briefcase',
    '#00AA66',
    'JOBS',
    2
FROM DUAL
UNION ALL
SELECT 
    'الفروع',
    'فروع المستشفى والمواقع',
    'fa-map-marker',
    '#FF6600',
    'BRANCHES',
    3
FROM DUAL
UNION ALL
SELECT 
    'البنوك',
    'البنوك المعتمدة للرواتب',
    'fa-university',
    '#9933CC',
    'BANKS',
    4
FROM DUAL
UNION ALL
SELECT 
    'المدن',
    'المدن والمناطق',
    'fa-building-o',
    '#FF9900',
    'CITIES',
    5
FROM DUAL
UNION ALL
SELECT 
    'الدول',
    'الدول والجنسيات',
    'fa-globe',
    '#0099CC',
    'COUNTRIES',
    6
FROM DUAL
UNION ALL
SELECT 
    'أنواع الوثائق',
    'الوثائق المطلوبة للموظفين',
    'fa-file-text-o',
    '#CC0000',
    'DOCUMENT_TYPES',
    7
FROM DUAL
UNION ALL
SELECT 
    'إعدادات النظام',
    'الإعدادات العامة للنظام',
    'fa-cog',
    '#666666',
    'SETTINGS',
    8
FROM DUAL;

COMMENT ON TABLE V_NAVIGATION_CARDS IS 'بطاقات التنقل للوحة التحكم';

-- ==========================================================
-- 3. V_JOBS_BY_GRADE - توزيع الوظائف حسب الدرجة
-- ==========================================================
-- الاستخدام في APEX: Pie Chart, Bar Chart
-- ==========================================================
CREATE OR REPLACE VIEW V_JOBS_BY_GRADE AS
SELECT 
    NVL(g.GRADE_NAME, 'غير محدد') AS GRADE,
    COUNT(j.JOB_ID) AS JOB_COUNT,
    g.GRADE_ID
FROM JOB_GRADES g
LEFT JOIN JOBS j ON g.GRADE_ID = j.DEFAULT_GRADE_ID
WHERE g.IS_DELETED = 0
GROUP BY g.GRADE_NAME, g.GRADE_ID
ORDER BY JOB_COUNT DESC;

COMMENT ON TABLE V_JOBS_BY_GRADE IS 'توزيع الوظائف حسب الدرجة الوظيفية';

-- ==========================================================
-- 4. V_BRANCHES_BY_CITY - توزيع الفروع حسب المدينة
-- ==========================================================
-- الاستخدام في APEX: Map, Chart
-- ==========================================================
CREATE OR REPLACE VIEW V_BRANCHES_BY_CITY AS
SELECT 
    c.CITY_NAME_AR AS CITY,
    COUNT(b.BRANCH_ID) AS BRANCH_COUNT,
    c.CITY_ID
FROM CITIES c
LEFT JOIN BRANCHES b ON c.CITY_ID = b.CITY_ID
WHERE c.IS_DELETED = 0
GROUP BY c.CITY_NAME_AR, c.CITY_ID
ORDER BY BRANCH_COUNT DESC;

COMMENT ON TABLE V_BRANCHES_BY_CITY IS 'توزيع الفروع حسب المدينة';

-- ==========================================================
-- 5. V_SALARY_RANGES - نطاقات الرواتب
-- ==========================================================
-- الاستخدام في APEX: Range Chart
-- ==========================================================
CREATE OR REPLACE VIEW V_SALARY_RANGES AS
SELECT 
    GRADE_NAME,
    MIN_SALARY,
    MAX_SALARY,
    (MAX_SALARY - MIN_SALARY) AS RANGE_DIFF,
    TICKET_CLASS,
    GRADE_ID
FROM JOB_GRADES
WHERE IS_DELETED = 0
ORDER BY MIN_SALARY;

COMMENT ON TABLE V_SALARY_RANGES IS 'نطاقات الرواتب حسب الدرجة الوظيفية';

-- ==========================================================
-- 6. V_RECENT_ACTIVITY - النشاط الأخير
-- ==========================================================
-- الاستخدام في APEX: Timeline, Activity Feed
-- ==========================================================
CREATE OR REPLACE VIEW V_RECENT_ACTIVITY AS
SELECT 
    'قسم جديد: ' || DEPT_NAME_AR AS ACTIVITY,
    CREATED_AT AS ACTIVITY_DATE,
    CREATED_BY AS ACTIVITY_USER,
    'DEPARTMENT' AS ACTIVITY_TYPE,
    'fa-hospital-o' AS ICON
FROM DEPARTMENTS
WHERE IS_DELETED = 0
UNION ALL
SELECT 
    'وظيفة جديدة: ' || JOB_TITLE_AR,
    CREATED_AT,
    CREATED_BY,
    'JOB',
    'fa-briefcase'
FROM JOBS
WHERE IS_DELETED = 0
UNION ALL
SELECT 
    'فرع جديد: ' || BRANCH_NAME_AR,
    CREATED_AT,
    CREATED_BY,
    'BRANCH',
    'fa-map-marker'
FROM BRANCHES
WHERE IS_DELETED = 0
ORDER BY 2 DESC
FETCH FIRST 20 ROWS ONLY;

COMMENT ON TABLE V_RECENT_ACTIVITY IS 'سجل النشاط الأخير للوحة التحكم';

-- ==========================================================
-- 7. V_DEPARTMENTS_TREE - الهيكل التنظيمي
-- ==========================================================
-- الاستخدام في APEX: Tree View, Org Chart
-- ==========================================================
CREATE OR REPLACE VIEW V_DEPARTMENTS_TREE AS
SELECT 
    DEPT_ID,
    DEPT_NAME_AR AS DEPARTMENT_NAME,
    PARENT_DEPT_ID,
    BRANCH_ID,
    LEVEL AS DEPT_LEVEL,
    SYS_CONNECT_BY_PATH(DEPT_NAME_AR, ' > ') AS FULL_PATH
FROM DEPARTMENTS
WHERE IS_DELETED = 0
START WITH PARENT_DEPT_ID IS NULL
CONNECT BY PRIOR DEPT_ID = PARENT_DEPT_ID
ORDER SIBLINGS BY DEPT_NAME_AR;

COMMENT ON TABLE V_DEPARTMENTS_TREE IS 'عرض شجري للهيكل التنظيمي';

-- ==========================================================
-- 8. V_QUICK_SEARCH - البحث السريع
-- ==========================================================
-- الاستخدام في APEX: Search Bar, Autocomplete
-- ==========================================================
CREATE OR REPLACE VIEW V_QUICK_SEARCH AS
SELECT 
    'قسم' AS ITEM_TYPE,
    DEPT_NAME_AR AS ITEM_NAME,
    DEPT_ID AS ITEM_ID,
    'DEPARTMENTS' AS TARGET_PAGE
FROM DEPARTMENTS
WHERE IS_DELETED = 0
UNION ALL
SELECT 
    'وظيفة',
    JOB_TITLE_AR,
    JOB_ID,
    'JOBS'
FROM JOBS
WHERE IS_DELETED = 0
UNION ALL
SELECT 
    'فرع',
    BRANCH_NAME_AR,
    BRANCH_ID,
    'BRANCHES'
FROM BRANCHES
WHERE IS_DELETED = 0
UNION ALL
SELECT 
    'بنك',
    BANK_NAME_AR,
    BANK_ID,
    'BANKS'
FROM BANKS
WHERE IS_DELETED = 0;

COMMENT ON TABLE V_QUICK_SEARCH IS 'البحث السريع عبر جميع الكيانات';

PROMPT ========================================
PROMPT ✅ تم إنشاء 8 عروض بنجاح!
PROMPT ========================================
PROMPT
PROMPT العروض المنشأة:
PROMPT 1. V_DASHBOARD_STATS      - الإحصائيات الرئيسية
PROMPT 2. V_NAVIGATION_CARDS     - بطاقات التنقل
PROMPT 3. V_JOBS_BY_GRADE        - توزيع الوظائف
PROMPT 4. V_BRANCHES_BY_CITY     - توزيع الفروع
PROMPT 5. V_SALARY_RANGES        - نطاقات الرواتب
PROMPT 6. V_RECENT_ACTIVITY      - النشاط الأخير
PROMPT 7. V_DEPARTMENTS_TREE     - الهيكل التنظيمي
PROMPT 8. V_QUICK_SEARCH         - البحث السريع
PROMPT
PROMPT جاهزة للاستخدام في APEX!
PROMPT ========================================
