-- =================================================================================
-- Script: اختبار نظام Audit Security
-- الوصف: اختبارات شاملة للتحقق من عمل النظام بشكل صحيح
-- =================================================================================

SET SERVEROUTPUT ON SIZE UNLIMITED
SET ECHO ON

PROMPT ╔════════════════════════════════════════════════════════╗
PROMPT ║   اختبار نظام Audit Security                         ║
PROMPT ╚════════════════════════════════════════════════════════╝
PROMPT

-- =================================================================================
-- Test 1: اختبار Security Context - تعيين واسترجاع المستخدم
-- =================================================================================
PROMPT ========================================
PROMPT Test 1: Security Context Basic Test
PROMPT ========================================
PROMPT

DECLARE
    v_username VARCHAR2(100);
    v_user_id NUMBER;
    v_session_info VARCHAR2(4000);
BEGIN
    DBMS_OUTPUT.PUT_LINE('1.1 - تعيين معلومات المستخدم...');
    PKG_SECURITY_CONTEXT.SET_USER_INFO(
        p_username => 'ADMIN_USER',
        p_user_id => 1,
        p_branch_id => 100,
        p_dept_id => 200
    );
    
    DBMS_OUTPUT.PUT_LINE('1.2 - استرجاع اسم المستخدم...');
    v_username := PKG_SECURITY_CONTEXT.GET_CURRENT_USER();
    DBMS_OUTPUT.PUT_LINE('   المستخدم: ' || v_username);
    
    DBMS_OUTPUT.PUT_LINE('1.3 - استرجاع معرف المستخدم...');
    v_user_id := PKG_SECURITY_CONTEXT.GET_CURRENT_USER_ID();
    DBMS_OUTPUT.PUT_LINE('   معرف المستخدم: ' || v_user_id);
    
    DBMS_OUTPUT.PUT_LINE('1.4 - الحصول على معلومات الـ Session...');
    v_session_info := PKG_SECURITY_CONTEXT.GET_SESSION_INFO();
    DBMS_OUTPUT.PUT_LINE('   Session Info: ' || v_session_info);
    
    -- التحقق
    IF v_username = 'ADMIN_USER' AND v_user_id = 1 THEN
        DBMS_OUTPUT.PUT_LINE('✅ Test 1 PASSED');
    ELSE
        DBMS_OUTPUT.PUT_LINE('❌ Test 1 FAILED');
    END IF;
    
    DBMS_OUTPUT.PUT_LINE('');
END;
/

-- =================================================================================
-- Test 2: اختبار Trigger - INSERT
-- =================================================================================
PROMPT ========================================
PROMPT Test 2: Trigger Test - INSERT
PROMPT ========================================
PROMPT

DECLARE
    v_country_id NUMBER;
    v_created_by VARCHAR2(100);
    v_created_at DATE;
BEGIN
    -- تعيين المستخدم
    PKG_SECURITY_CONTEXT.SET_USER_INFO('TEST_INSERT_USER', 2);
    
    DBMS_OUTPUT.PUT_LINE('2.1 - إدخال سجل جديد في جدول COUNTRIES...');
    
    -- إدخال بدون تحديد CREATED_BY (سيتم تعبئته من الـ trigger)
    INSERT INTO HR_CORE.COUNTRIES (
        COUNTRY_NAME_AR, COUNTRY_NAME_EN, COUNTRY_CODE, PHONE_CODE
    ) VALUES (
        'دولة اختبار', 'Test Country', 'TST', '+999'
    ) RETURNING COUNTRY_ID INTO v_country_id;
    
    DBMS_OUTPUT.PUT_LINE('   تم إدخال السجل برقم: ' || v_country_id);
    
    -- التحقق من تعبئة CREATED_BY تلقائياً
    SELECT CREATED_BY, CREATED_AT
    INTO v_created_by, v_created_at
    FROM HR_CORE.COUNTRIES
    WHERE COUNTRY_ID = v_country_id;
    
    DBMS_OUTPUT.PUT_LINE('2.2 - التحقق من CREATED_BY...');
    DBMS_OUTPUT.PUT_LINE('   CREATED_BY: ' || v_created_by);
    DBMS_OUTPUT.PUT_LINE('   CREATED_AT: ' || TO_CHAR(v_created_at, 'YYYY-MM-DD HH24:MI:SS'));
    
    IF v_created_by = 'TEST_INSERT_USER' THEN
        DBMS_OUTPUT.PUT_LINE('✅ Test 2 PASSED - Trigger عمل بشكل صحيح');
    ELSE
        DBMS_OUTPUT.PUT_LINE('❌ Test 2 FAILED - CREATED_BY = ' || v_created_by);
    END IF;
    
    -- تنظيف
    DELETE FROM HR_CORE.COUNTRIES WHERE COUNTRY_ID = v_country_id;
    COMMIT;
    
    DBMS_OUTPUT.PUT_LINE('');
END;
/

-- =================================================================================
-- Test 3: اختبار Trigger - UPDATE
-- =================================================================================
PROMPT ========================================
PROMPT Test 3: Trigger Test - UPDATE
PROMPT ========================================
PROMPT

DECLARE
    v_country_id NUMBER;
    v_created_by VARCHAR2(100);
    v_updated_by VARCHAR2(100);
    v_version_no NUMBER;
BEGIN
    -- إدخال سجل للاختبار
    PKG_SECURITY_CONTEXT.SET_USER_INFO('INSERT_USER', 3);
    
    INSERT INTO HR_CORE.COUNTRIES (
        COUNTRY_NAME_AR, COUNTRY_NAME_EN, COUNTRY_CODE, PHONE_CODE
    ) VALUES (
        'دولة للتحديث', 'Update Test Country', 'UPD', '+888'
    ) RETURNING COUNTRY_ID INTO v_country_id;
    
    COMMIT;
    
    DBMS_OUTPUT.PUT_LINE('3.1 - تم إدخال السجل برقم: ' || v_country_id);
    
    -- تغيير المستخدم
    PKG_SECURITY_CONTEXT.SET_USER_INFO('UPDATE_USER', 4);
    
    DBMS_OUTPUT.PUT_LINE('3.2 - تحديث السجل...');
    
    -- تحديث السجل
    UPDATE HR_CORE.COUNTRIES
    SET COUNTRY_NAME_AR = 'دولة محدثة'
    WHERE COUNTRY_ID = v_country_id;
    
    COMMIT;
    
    -- التحقق
    SELECT CREATED_BY, UPDATED_BY, VERSION_NO
    INTO v_created_by, v_updated_by, v_version_no
    FROM HR_CORE.COUNTRIES
    WHERE COUNTRY_ID = v_country_id;
    
    DBMS_OUTPUT.PUT_LINE('3.3 - التحقق من النتائج...');
    DBMS_OUTPUT.PUT_LINE('   CREATED_BY: ' || v_created_by);
    DBMS_OUTPUT.PUT_LINE('   UPDATED_BY: ' || v_updated_by);
    DBMS_OUTPUT.PUT_LINE('   VERSION_NO: ' || v_version_no);
    
    IF v_created_by = 'INSERT_USER' 
       AND v_updated_by = 'UPDATE_USER' 
       AND v_version_no = 2 THEN
        DBMS_OUTPUT.PUT_LINE('✅ Test 3 PASSED - UPDATE trigger عمل بشكل صحيح');
    ELSE
        DBMS_OUTPUT.PUT_LINE('❌ Test 3 FAILED');
    END IF;
    
    -- تنظيف
    DELETE FROM HR_CORE.COUNTRIES WHERE COUNTRY_ID = v_country_id;
    COMMIT;
    
    DBMS_OUTPUT.PUT_LINE('');
END;
/

-- =================================================================================
-- Test 4: اختبار منع التلاعب - محاولة تعيين CREATED_BY يدوياً
-- =================================================================================
PROMPT ========================================
PROMPT Test 4: Tampering Prevention Test
PROMPT ========================================
PROMPT

DECLARE
    v_country_id NUMBER;
    v_created_by VARCHAR2(100);
BEGIN
    -- تعيين المستخدم الحقيقي
    PKG_SECURITY_CONTEXT.SET_USER_INFO('REAL_USER', 5);
    
    DBMS_OUTPUT.PUT_LINE('4.1 - محاولة إدخال سجل مع CREATED_BY يدوي...');
    
    -- محاولة التلاعب بـ CREATED_BY
    INSERT INTO HR_CORE.COUNTRIES (
        COUNTRY_NAME_AR, COUNTRY_NAME_EN, COUNTRY_CODE, PHONE_CODE,
        CREATED_BY  -- ❌ محاولة تعيينه يدوياً
    ) VALUES (
        'دولة مزورة', 'Fake Country', 'FAK', '+777',
        'HACKER'  -- ❌ محاولة انتحال الهوية
    ) RETURNING COUNTRY_ID INTO v_country_id;
    
    -- التحقق - يجب أن يتم تجاهل القيمة اليدوية
    SELECT CREATED_BY
    INTO v_created_by
    FROM HR_CORE.COUNTRIES
    WHERE COUNTRY_ID = v_country_id;
    
    DBMS_OUTPUT.PUT_LINE('4.2 - التحقق من CREATED_BY...');
    DBMS_OUTPUT.PUT_LINE('   القيمة المحاولة: HACKER');
    DBMS_OUTPUT.PUT_LINE('   القيمة الفعلية: ' || v_created_by);
    
    IF v_created_by = 'REAL_USER' THEN
        DBMS_OUTPUT.PUT_LINE('✅ Test 4 PASSED - تم منع التلاعب بنجاح');
    ELSE
        DBMS_OUTPUT.PUT_LINE('❌ Test 4 FAILED - تم التلاعب بالبيانات!');
    END IF;
    
    -- تنظيف
    DELETE FROM HR_CORE.COUNTRIES WHERE COUNTRY_ID = v_country_id;
    COMMIT;
    
    DBMS_OUTPUT.PUT_LINE('');
END;
/

-- =================================================================================
-- Test 5: اختبار Context في Sessions مختلفة
-- =================================================================================
PROMPT ========================================
PROMPT Test 5: Multiple Sessions Test
PROMPT ========================================
PROMPT

DECLARE
    v_user1 VARCHAR2(100);
    v_user2 VARCHAR2(100);
BEGIN
    DBMS_OUTPUT.PUT_LINE('5.1 - تعيين المستخدم الأول...');
    PKG_SECURITY_CONTEXT.SET_USER_INFO('USER_SESSION_1', 10);
    v_user1 := PKG_SECURITY_CONTEXT.GET_CURRENT_USER();
    DBMS_OUTPUT.PUT_LINE('   المستخدم: ' || v_user1);
    
    DBMS_OUTPUT.PUT_LINE('5.2 - تغيير المستخدم في نفس الـ Session...');
    PKG_SECURITY_CONTEXT.SET_USER_INFO('USER_SESSION_2', 20);
    v_user2 := PKG_SECURITY_CONTEXT.GET_CURRENT_USER();
    DBMS_OUTPUT.PUT_LINE('   المستخدم: ' || v_user2);
    
    IF v_user1 = 'USER_SESSION_1' AND v_user2 = 'USER_SESSION_2' THEN
        DBMS_OUTPUT.PUT_LINE('✅ Test 5 PASSED - Context يتغير بشكل صحيح');
    ELSE
        DBMS_OUTPUT.PUT_LINE('❌ Test 5 FAILED');
    END IF;
    
    DBMS_OUTPUT.PUT_LINE('');
END;
/

-- =================================================================================
-- Test 6: اختبار Default Value عند عدم تعيين Context
-- =================================================================================
PROMPT ========================================
PROMPT Test 6: Default Value Test
PROMPT ========================================
PROMPT

DECLARE
    v_user VARCHAR2(100);
BEGIN
    DBMS_OUTPUT.PUT_LINE('6.1 - مسح الـ Context...');
    PKG_SECURITY_CONTEXT.CLEAR_CONTEXT();
    
    DBMS_OUTPUT.PUT_LINE('6.2 - محاولة الحصول على المستخدم...');
    v_user := PKG_SECURITY_CONTEXT.GET_CURRENT_USER();
    DBMS_OUTPUT.PUT_LINE('   المستخدم: ' || v_user);
    
    IF v_user = 'SYSTEM' THEN
        DBMS_OUTPUT.PUT_LINE('✅ Test 6 PASSED - Default value يعمل بشكل صحيح');
    ELSE
        DBMS_OUTPUT.PUT_LINE('❌ Test 6 FAILED');
    END IF;
    
    DBMS_OUTPUT.PUT_LINE('');
END;
/

-- =================================================================================
-- النتيجة النهائية
-- =================================================================================
PROMPT ╔════════════════════════════════════════════════════════╗
PROMPT ║   ✅ اكتملت جميع الاختبارات                          ║
PROMPT ╚════════════════════════════════════════════════════════╝
PROMPT
PROMPT ملاحظات:
PROMPT - تأكد من مراجعة نتائج جميع الاختبارات أعلاه
PROMPT - يجب أن تكون جميع الاختبارات PASSED
PROMPT - إذا فشل أي اختبار، راجع الـ triggers والـ packages
PROMPT
