-- =================================================================================
-- التحقق من PKG_SECURITY_CONTEXT
-- =================================================================================

SET SERVEROUTPUT ON;

PROMPT ========================================
PROMPT Checking PKG_SECURITY_CONTEXT Status
PROMPT ========================================

-- 1. Check if package exists
SELECT object_name, object_type, status 
FROM user_objects 
WHERE object_name = 'PKG_SECURITY_CONTEXT';

-- 2. Check for compilation errors
SELECT line, position, text 
FROM user_errors 
WHERE name = 'PKG_SECURITY_CONTEXT'
ORDER BY sequence;

-- 3. Try to call the function
BEGIN
    DBMS_OUTPUT.PUT_LINE('Testing PKG_SECURITY_CONTEXT.GET_CURRENT_USER...');
    DBMS_OUTPUT.PUT_LINE('Result: ' || PKG_SECURITY_CONTEXT.GET_CURRENT_USER);
EXCEPTION
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('ERROR: ' || SQLERRM);
END;
/

PROMPT ========================================
PROMPT If package is INVALID or has errors:
PROMPT 1. Run 01_PKG_SECURITY_CONTEXT_SPEC.sql
PROMPT 2. Run 02_PKG_SECURITY_CONTEXT_BODY.sql
PROMPT 3. Run this check again
PROMPT ========================================
