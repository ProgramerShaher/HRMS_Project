-- =================================================================================
-- سكربت حذف شامل لجميع الجداول من جميع الـ Schemas
-- تحذير: هذا سيحذف كل شيء!
-- =================================================================================

-- التأكد من الاتصال بـ PDB
ALTER SESSION SET CONTAINER = XEPDB1;

SET SERVEROUTPUT ON;

DECLARE
    v_count NUMBER := 0;
BEGIN
    DBMS_OUTPUT.PUT_LINE('========================================');
    DBMS_OUTPUT.PUT_LINE('بدء حذف جميع الجداول...');
    DBMS_OUTPUT.PUT_LINE('========================================');
    
    -- حذف جداول HR_PERFORMANCE
    FOR t IN (SELECT table_name FROM all_tables WHERE owner = 'HR_PERFORMANCE') LOOP
        EXECUTE IMMEDIATE 'DROP TABLE HR_PERFORMANCE.' || t.table_name || ' CASCADE CONSTRAINTS PURGE';
        v_count := v_count + 1;
        DBMS_OUTPUT.PUT_LINE('تم حذف: HR_PERFORMANCE.' || t.table_name);
    END LOOP;
    
    -- حذف جداول HR_RECRUITMENT
    FOR t IN (SELECT table_name FROM all_tables WHERE owner = 'HR_RECRUITMENT') LOOP
        EXECUTE IMMEDIATE 'DROP TABLE HR_RECRUITMENT.' || t.table_name || ' CASCADE CONSTRAINTS PURGE';
        v_count := v_count + 1;
        DBMS_OUTPUT.PUT_LINE('تم حذف: HR_RECRUITMENT.' || t.table_name);
    END LOOP;
    
    -- حذف جداول HR_PAYROLL
    FOR t IN (SELECT table_name FROM all_tables WHERE owner = 'HR_PAYROLL') LOOP
        EXECUTE IMMEDIATE 'DROP TABLE HR_PAYROLL.' || t.table_name || ' CASCADE CONSTRAINTS PURGE';
        v_count := v_count + 1;
        DBMS_OUTPUT.PUT_LINE('تم حذف: HR_PAYROLL.' || t.table_name);
    END LOOP;
    
    -- حذف جداول HR_LEAVES
    FOR t IN (SELECT table_name FROM all_tables WHERE owner = 'HR_LEAVES') LOOP
        EXECUTE IMMEDIATE 'DROP TABLE HR_LEAVES.' || t.table_name || ' CASCADE CONSTRAINTS PURGE';
        v_count := v_count + 1;
        DBMS_OUTPUT.PUT_LINE('تم حذف: HR_LEAVES.' || t.table_name);
    END LOOP;
    
    -- حذف جداول HR_ATTENDANCE
    FOR t IN (SELECT table_name FROM all_tables WHERE owner = 'HR_ATTENDANCE') LOOP
        EXECUTE IMMEDIATE 'DROP TABLE HR_ATTENDANCE.' || t.table_name || ' CASCADE CONSTRAINTS PURGE';
        v_count := v_count + 1;
        DBMS_OUTPUT.PUT_LINE('تم حذف: HR_ATTENDANCE.' || t.table_name);
    END LOOP;
    
    -- حذف جداول HR_PERSONNEL
    FOR t IN (SELECT table_name FROM all_tables WHERE owner = 'HR_PERSONNEL') LOOP
        EXECUTE IMMEDIATE 'DROP TABLE HR_PERSONNEL.' || t.table_name || ' CASCADE CONSTRAINTS PURGE';
        v_count := v_count + 1;
        DBMS_OUTPUT.PUT_LINE('تم حذف: HR_PERSONNEL.' || t.table_name);
    END LOOP;
    
    -- حذف جداول HR_CORE
    FOR t IN (SELECT table_name FROM all_tables WHERE owner = 'HR_CORE') LOOP
        EXECUTE IMMEDIATE 'DROP TABLE HR_CORE.' || t.table_name || ' CASCADE CONSTRAINTS PURGE';
        v_count := v_count + 1;
        DBMS_OUTPUT.PUT_LINE('تم حذف: HR_CORE.' || t.table_name);
    END LOOP;
    
    DBMS_OUTPUT.PUT_LINE('========================================');
    DBMS_OUTPUT.PUT_LINE('تم حذف ' || v_count || ' جدول بنجاح!');
    DBMS_OUTPUT.PUT_LINE('========================================');
END;
/

-- التحقق من عدم وجود جداول متبقية
SELECT owner, COUNT(*) as remaining_tables 
FROM all_tables 
WHERE owner IN ('HR_CORE', 'HR_PERSONNEL', 'HR_ATTENDANCE', 'HR_LEAVES', 'HR_PAYROLL', 'HR_RECRUITMENT', 'HR_PERFORMANCE')
GROUP BY owner
ORDER BY owner;

-- إذا لم تظهر أي نتائج، فهذا يعني أن جميع الجداول تم حذفها بنجاح
SELECT 'جميع الجداول تم حذفها بنجاح! النظام نظيف الآن.' AS STATUS FROM DUAL;
