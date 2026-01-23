-- =================================================================================
-- التحقق من صحة البيانات والـ Foreign Keys
-- Oracle 23ai - FREEPDB1
-- =================================================================================

ALTER SESSION SET CONTAINER = FREEPDB1;

SET SERVEROUTPUT ON;

PROMPT ========================================
PROMPT التحقق من Foreign Keys
PROMPT ========================================

-- 1. التحقق من وجود Foreign Keys على DEPARTMENTS
SELECT 
    constraint_name,
    constraint_type,
    r_constraint_name,
    status
FROM user_constraints
WHERE owner = 'HR_CORE'
  AND table_name = 'DEPARTMENTS'
  AND constraint_type = 'R'
ORDER BY constraint_name;

PROMPT 
PROMPT ========================================
PROMPT التحقق من البيانات غير الصحيحة
PROMPT ========================================

-- 2. البحث عن أقسام بفروع غير موجودة
SELECT 
    d.dept_id,
    d.dept_name_ar,
    d.branch_id,
    CASE 
        WHEN b.branch_id IS NULL THEN '❌ فرع غير موجود'
        ELSE '✅ صحيح'
    END AS status
FROM HR_CORE.DEPARTMENTS d
LEFT JOIN HR_CORE.BRANCHES b ON d.branch_id = b.branch_id
WHERE b.branch_id IS NULL;

PROMPT 
PROMPT ========================================
PROMPT حذف البيانات غير الصحيحة
PROMPT ========================================

-- 3. حذف الأقسام التي تشير لفروع غير موجودة
DELETE FROM HR_CORE.DEPARTMENTS
WHERE branch_id NOT IN (SELECT branch_id FROM HR_CORE.BRANCHES);

PROMPT تم حذف الأقسام غير الصحيحة

-- 4. تحديث الأقسام لتشير لفروع صحيحة
UPDATE HR_CORE.DEPARTMENTS
SET branch_id = 1  -- الفرع الرئيسي
WHERE branch_id IS NULL OR branch_id NOT IN (SELECT branch_id FROM HR_CORE.BRANCHES);

COMMIT;

PROMPT 
PROMPT ========================================
PROMPT إضافة Foreign Keys (إذا لم تكن موجودة)
PROMPT ========================================

-- 5. إضافة Foreign Key على DEPARTMENTS
BEGIN
    EXECUTE IMMEDIATE 'ALTER TABLE HR_CORE.DEPARTMENTS 
                       ADD CONSTRAINT FK_DEPT_BRANCH 
                       FOREIGN KEY (BRANCH_ID) 
                       REFERENCES HR_CORE.BRANCHES(BRANCH_ID)';
    DBMS_OUTPUT.PUT_LINE('✅ تم إضافة FK_DEPT_BRANCH');
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE = -2275 THEN
            DBMS_OUTPUT.PUT_LINE('⚠️ FK_DEPT_BRANCH موجود مسبقاً');
        ELSIF SQLCODE = -2298 THEN
            DBMS_OUTPUT.PUT_LINE('❌ خطأ: يوجد بيانات غير صحيحة');
            DBMS_OUTPUT.PUT_LINE('   نفذ الاستعلام أعلاه لحذف البيانات الخاطئة');
        ELSE
            DBMS_OUTPUT.PUT_LINE('❌ خطأ: ' || SQLERRM);
        END IF;
END;
/

PROMPT 
PROMPT ========================================
PROMPT التحقق النهائي
PROMPT ========================================

-- 6. عرض جميع الأقسام مع الفروع
SELECT 
    d.dept_id,
    d.dept_name_ar,
    d.branch_id,
    b.branch_name_ar AS branch_name,
    d.cost_center_code
FROM HR_CORE.DEPARTMENTS d
JOIN HR_CORE.BRANCHES b ON d.branch_id = b.branch_id
ORDER BY d.dept_id;

PROMPT 
PROMPT ========================================
PROMPT ✅ اكتمل التحقق والإصلاح
PROMPT ========================================
