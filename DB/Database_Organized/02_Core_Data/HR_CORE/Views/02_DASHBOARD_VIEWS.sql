-- =================================================================================
-- Dashboard Views for APEX - HR_CORE
-- Description: Ready-to-use views for building professional dashboard in APEX
-- Schema: HR_CORE
-- =================================================================================

-- ==========================================================
-- 1. V_DASHBOARD_STATS - Key Metrics (الإحصائيات الرئيسية)
-- ==========================================================
-- Usage in APEX: Badge List, Value Cards
-- ==========================================================
CREATE OR REPLACE VIEW HR_CORE.V_DASHBOARD_STATS AS
SELECT 
    (SELECT COUNT(*) FROM HR_CORE.DEPARTMENTS WHERE IS_DELETED = 0) AS TOTAL_DEPARTMENTS,
    (SELECT COUNT(*) FROM HR_CORE.JOBS WHERE IS_DELETED = 0) AS TOTAL_JOBS,
    (SELECT COUNT(*) FROM HR_CORE.BRANCHES WHERE IS_DELETED = 0) AS TOTAL_BRANCHES,
    (SELECT COUNT(*) FROM HR_CORE.BANKS WHERE IS_DELETED = 0) AS TOTAL_BANKS,
    (SELECT COUNT(*) FROM HR_CORE.CITIES WHERE IS_DELETED = 0) AS TOTAL_CITIES,
    (SELECT COUNT(*) FROM HR_CORE.COUNTRIES WHERE IS_DELETED = 0) AS TOTAL_COUNTRIES,
    (SELECT COUNT(*) FROM HR_CORE.JOB_GRADES WHERE IS_DELETED = 0) AS TOTAL_GRADES,
    (SELECT COUNT(*) FROM HR_CORE.DOCUMENT_TYPES WHERE IS_DELETED = 0) AS TOTAL_DOC_TYPES
FROM DUAL;

COMMENT ON TABLE HR_CORE.V_DASHBOARD_STATS IS 'Dashboard statistics for APEX - Key metrics';

-- ==========================================================
-- 2. V_NAVIGATION_CARDS - Navigation Cards (بطاقات التنقل)
-- ==========================================================
-- Usage in APEX: Cards Region
-- ==========================================================
CREATE OR REPLACE VIEW HR_CORE.V_NAVIGATION_CARDS AS
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

COMMENT ON TABLE HR_CORE.V_NAVIGATION_CARDS IS 'Navigation cards for APEX dashboard';

-- ==========================================================
-- 3. V_JOBS_BY_DEPARTMENT - Jobs Distribution (توزيع الوظائف)
-- ==========================================================
-- Usage in APEX: Pie Chart, Bar Chart
-- ==========================================================
CREATE OR REPLACE VIEW HR_CORE.V_JOBS_BY_DEPARTMENT AS
SELECT 
    NVL(d.DEPT_NAME_AR, 'غير محدد') AS DEPARTMENT,
    COUNT(j.JOB_ID) AS JOB_COUNT,
    d.DEPT_ID
FROM HR_CORE.DEPARTMENTS d
LEFT JOIN HR_CORE.JOBS j ON d.DEPT_ID = j.DEFAULT_GRADE_ID
WHERE d.IS_DELETED = 0
GROUP BY d.DEPT_NAME_AR, d.DEPT_ID
ORDER BY JOB_COUNT DESC;

COMMENT ON TABLE HR_CORE.V_JOBS_BY_DEPARTMENT IS 'Jobs distribution by department for charts';

-- ==========================================================
-- 4. V_BRANCHES_BY_CITY - Branches Distribution (توزيع الفروع)
-- ==========================================================
-- Usage in APEX: Map, Chart
-- ==========================================================
CREATE OR REPLACE VIEW HR_CORE.V_BRANCHES_BY_CITY AS
SELECT 
    c.CITY_NAME_AR AS CITY,
    COUNT(b.BRANCH_ID) AS BRANCH_COUNT,
    c.CITY_ID
FROM HR_CORE.CITIES c
LEFT JOIN HR_CORE.BRANCHES b ON c.CITY_ID = b.CITY_ID
WHERE c.IS_DELETED = 0
GROUP BY c.CITY_NAME_AR, c.CITY_ID
ORDER BY BRANCH_COUNT DESC;

COMMENT ON TABLE HR_CORE.V_BRANCHES_BY_CITY IS 'Branches distribution by city';

-- ==========================================================
-- 5. V_SALARY_RANGES - Salary Ranges (نطاقات الرواتب)
-- ==========================================================
-- Usage in APEX: Range Chart
-- ==========================================================
CREATE OR REPLACE VIEW HR_CORE.V_SALARY_RANGES AS
SELECT 
    GRADE_NAME,
    MIN_SALARY,
    MAX_SALARY,
    (MAX_SALARY - MIN_SALARY) AS RANGE_DIFF,
    TICKET_CLASS,
    GRADE_ID
FROM HR_CORE.JOB_GRADES
WHERE IS_DELETED = 0
ORDER BY MIN_SALARY;

COMMENT ON TABLE HR_CORE.V_SALARY_RANGES IS 'Salary ranges by job grade';

-- ==========================================================
-- 6. V_RECENT_ACTIVITY - Recent Activity (النشاط الأخير)
-- ==========================================================
-- Usage in APEX: Timeline, Activity Feed
-- ==========================================================
CREATE OR REPLACE VIEW HR_CORE.V_RECENT_ACTIVITY AS
SELECT 
    'قسم جديد: ' || DEPT_NAME_AR AS ACTIVITY,
    CREATED_AT AS ACTIVITY_DATE,
    CREATED_BY AS ACTIVITY_USER,
    'DEPARTMENT' AS ACTIVITY_TYPE,
    'fa-hospital-o' AS ICON
FROM HR_CORE.DEPARTMENTS
WHERE IS_DELETED = 0
UNION ALL
SELECT 
    'وظيفة جديدة: ' || JOB_TITLE_AR,
    CREATED_AT,
    CREATED_BY,
    'JOB',
    'fa-briefcase'
FROM HR_CORE.JOBS
WHERE IS_DELETED = 0
UNION ALL
SELECT 
    'فرع جديد: ' || BRANCH_NAME_AR,
    CREATED_AT,
    CREATED_BY,
    'BRANCH',
    'fa-map-marker'
FROM HR_CORE.BRANCHES
WHERE IS_DELETED = 0
ORDER BY 2 DESC
FETCH FIRST 20 ROWS ONLY;

COMMENT ON TABLE HR_CORE.V_RECENT_ACTIVITY IS 'Recent activity feed for dashboard';

-- ==========================================================
-- 7. V_DEPARTMENTS_TREE - Departments Hierarchy (الهيكل التنظيمي)
-- ==========================================================
-- Usage in APEX: Tree View, Org Chart
-- ==========================================================
CREATE OR REPLACE VIEW HR_CORE.V_DEPARTMENTS_TREE AS
SELECT 
    DEPT_ID,
    DEPT_NAME_AR AS DEPARTMENT_NAME,
    PARENT_DEPT_ID,
    BRANCH_ID,
    LEVEL AS DEPT_LEVEL,
    SYS_CONNECT_BY_PATH(DEPT_NAME_AR, ' > ') AS FULL_PATH
FROM HR_CORE.DEPARTMENTS
WHERE IS_DELETED = 0
START WITH PARENT_DEPT_ID IS NULL
CONNECT BY PRIOR DEPT_ID = PARENT_DEPT_ID
ORDER SIBLINGS BY DEPT_NAME_AR;

COMMENT ON TABLE HR_CORE.V_DEPARTMENTS_TREE IS 'Hierarchical view of departments';

-- ==========================================================
-- 8. V_QUICK_SEARCH - Quick Search (البحث السريع)
-- ==========================================================
-- Usage in APEX: Search Bar, Autocomplete
-- ==========================================================
CREATE OR REPLACE VIEW HR_CORE.V_QUICK_SEARCH AS
SELECT 
    'قسم' AS ITEM_TYPE,
    DEPT_NAME_AR AS ITEM_NAME,
    DEPT_ID AS ITEM_ID,
    'DEPARTMENTS' AS TARGET_PAGE
FROM HR_CORE.DEPARTMENTS
WHERE IS_DELETED = 0
UNION ALL
SELECT 
    'وظيفة',
    JOB_TITLE_AR,
    JOB_ID,
    'JOBS'
FROM HR_CORE.JOBS
WHERE IS_DELETED = 0
UNION ALL
SELECT 
    'فرع',
    BRANCH_NAME_AR,
    BRANCH_ID,
    'BRANCHES'
FROM HR_CORE.BRANCHES
WHERE IS_DELETED = 0
UNION ALL
SELECT 
    'بنك',
    BANK_NAME_AR,
    BANK_ID,
    'BANKS'
FROM HR_CORE.BANKS
WHERE IS_DELETED = 0;

COMMENT ON TABLE HR_CORE.V_QUICK_SEARCH IS 'Quick search across all HR_CORE entities';

-- ==========================================================
-- Grant Permissions (if needed for APEX workspace)
-- ==========================================================
-- GRANT SELECT ON HR_CORE.V_DASHBOARD_STATS TO APEX_WORKSPACE;
-- GRANT SELECT ON HR_CORE.V_NAVIGATION_CARDS TO APEX_WORKSPACE;
-- ... (repeat for all views)

PROMPT ========================================
PROMPT Dashboard Views Created Successfully!
PROMPT ========================================
PROMPT
PROMPT Created 8 views:
PROMPT 1. V_DASHBOARD_STATS      - Key metrics
PROMPT 2. V_NAVIGATION_CARDS     - Navigation cards
PROMPT 3. V_JOBS_BY_DEPARTMENT   - Jobs distribution
PROMPT 4. V_BRANCHES_BY_CITY     - Branches distribution
PROMPT 5. V_SALARY_RANGES        - Salary ranges
PROMPT 6. V_RECENT_ACTIVITY      - Recent activity feed
PROMPT 7. V_DEPARTMENTS_TREE     - Org hierarchy
PROMPT 8. V_QUICK_SEARCH         - Quick search
PROMPT
PROMPT Ready to use in APEX!
PROMPT ========================================
