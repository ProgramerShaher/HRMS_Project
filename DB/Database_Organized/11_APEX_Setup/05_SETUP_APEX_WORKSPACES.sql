-- =================================================================================
-- إعداد APEX Workspaces لجميع الـ Schemas (9 workspaces)
-- يجب تشغيله بعد إنشاء الـ Schemas
-- =================================================================================

ALTER SESSION SET CONTAINER = FREEPDB1;

PROMPT ========================================
PROMPT إنشاء APEX Workspaces (9 workspaces)
PROMPT ========================================

-- =================================================================================
-- 1. Workspace: HR_CORE
-- =================================================================================

BEGIN
    APEX_INSTANCE_ADMIN.ADD_WORKSPACE(
        p_workspace_id   => NULL,
        p_workspace      => 'HR_CORE',
        p_primary_schema => 'HR_CORE',
        p_additional_schemas => 'HR_PERSONNEL:HR_ATTENDANCE:HR_LEAVES:HR_PAYROLL:HR_RECRUITMENT:HR_PERFORMANCE'
    );
    
    APEX_UTIL.SET_WORKSPACE(p_workspace => 'HR_CORE');
    
    APEX_UTIL.CREATE_USER(
        p_user_name     => 'ADMIN',
        p_email_address => 'admin@hospital.com',
        p_web_password  => 'Admin@2026',
        p_developer_privs => 'ADMIN:CREATE:DATA_LOADER:EDIT:HELP:MONITOR:SQL'
    );
    
    COMMIT;
END;
/

PROMPT ✅ 1/9 - تم إنشاء Workspace: HR_CORE

-- =================================================================================
-- 2. Workspace: HR_PERSONNEL
-- =================================================================================

BEGIN
    APEX_INSTANCE_ADMIN.ADD_WORKSPACE(
        p_workspace      => 'HR_PERSONNEL',
        p_primary_schema => 'HR_PERSONNEL',
        p_additional_schemas => 'HR_CORE'
    );
    
    APEX_UTIL.SET_WORKSPACE(p_workspace => 'HR_PERSONNEL');
    
    APEX_UTIL.CREATE_USER(
        p_user_name     => 'HR_MANAGER',
        p_email_address => 'hr@hospital.com',
        p_web_password  => 'HRManager@2026',
        p_developer_privs => 'CREATE:DATA_LOADER:EDIT:HELP:MONITOR:SQL'
    );
    
    COMMIT;
END;
/

PROMPT ✅ 2/9 - تم إنشاء Workspace: HR_PERSONNEL

-- =================================================================================
-- 3. Workspace: HR_ATTENDANCE
-- =================================================================================

BEGIN
    APEX_INSTANCE_ADMIN.ADD_WORKSPACE(
        p_workspace      => 'HR_ATTENDANCE',
        p_primary_schema => 'HR_ATTENDANCE',
        p_additional_schemas => 'HR_CORE:HR_PERSONNEL'
    );
    
    APEX_UTIL.SET_WORKSPACE(p_workspace => 'HR_ATTENDANCE');
    
    APEX_UTIL.CREATE_USER(
        p_user_name     => 'ATTENDANCE_ADMIN',
        p_email_address => 'attendance@hospital.com',
        p_web_password  => 'Attend@2026',
        p_developer_privs => 'CREATE:EDIT:HELP:MONITOR'
    );
    
    COMMIT;
END;
/

PROMPT ✅ 3/9 - تم إنشاء Workspace: HR_ATTENDANCE

-- =================================================================================
-- 4. Workspace: HR_LEAVES
-- =================================================================================

BEGIN
    APEX_INSTANCE_ADMIN.ADD_WORKSPACE(
        p_workspace      => 'HR_LEAVES',
        p_primary_schema => 'HR_LEAVES',
        p_additional_schemas => 'HR_CORE:HR_PERSONNEL'
    );
    
    APEX_UTIL.SET_WORKSPACE(p_workspace => 'HR_LEAVES');
    
    APEX_UTIL.CREATE_USER(
        p_user_name     => 'LEAVE_ADMIN',
        p_email_address => 'leaves@hospital.com',
        p_web_password  => 'Leaves@2026',
        p_developer_privs => 'CREATE:EDIT:HELP:MONITOR'
    );
    
    COMMIT;
END;
/

PROMPT ✅ 4/9 - تم إنشاء Workspace: HR_LEAVES

-- =================================================================================
-- 5. Workspace: HR_PAYROLL
-- =================================================================================

BEGIN
    APEX_INSTANCE_ADMIN.ADD_WORKSPACE(
        p_workspace      => 'HR_PAYROLL',
        p_primary_schema => 'HR_PAYROLL',
        p_additional_schemas => 'HR_CORE:HR_PERSONNEL'
    );
    
    APEX_UTIL.SET_WORKSPACE(p_workspace => 'HR_PAYROLL');
    
    APEX_UTIL.CREATE_USER(
        p_user_name     => 'PAYROLL_ADMIN',
        p_email_address => 'payroll@hospital.com',
        p_web_password  => 'Payroll@2026',
        p_developer_privs => 'CREATE:EDIT:HELP:MONITOR'
    );
    
    COMMIT;
END;
/

PROMPT ✅ 5/9 - تم إنشاء Workspace: HR_PAYROLL

-- =================================================================================
-- 6. Workspace: HR_RECRUITMENT
-- =================================================================================

BEGIN
    APEX_INSTANCE_ADMIN.ADD_WORKSPACE(
        p_workspace      => 'HR_RECRUITMENT',
        p_primary_schema => 'HR_RECRUITMENT',
        p_additional_schemas => 'HR_CORE:HR_PERSONNEL'
    );
    
    APEX_UTIL.SET_WORKSPACE(p_workspace => 'HR_RECRUITMENT');
    
    APEX_UTIL.CREATE_USER(
        p_user_name     => 'RECRUIT_ADMIN',
        p_email_address => 'recruitment@hospital.com',
        p_web_password  => 'Recruit@2026',
        p_developer_privs => 'CREATE:EDIT:HELP:MONITOR'
    );
    
    COMMIT;
END;
/

PROMPT ✅ 6/9 - تم إنشاء Workspace: HR_RECRUITMENT

-- =================================================================================
-- 7. Workspace: HR_PERFORMANCE
-- =================================================================================

BEGIN
    APEX_INSTANCE_ADMIN.ADD_WORKSPACE(
        p_workspace      => 'HR_PERFORMANCE',
        p_primary_schema => 'HR_PERFORMANCE',
        p_additional_schemas => 'HR_CORE:HR_PERSONNEL'
    );
    
    APEX_UTIL.SET_WORKSPACE(p_workspace => 'HR_PERFORMANCE');
    
    APEX_UTIL.CREATE_USER(
        p_user_name     => 'PERF_ADMIN',
        p_email_address => 'performance@hospital.com',
        p_web_password  => 'Perform@2026',
        p_developer_privs => 'CREATE:EDIT:HELP:MONITOR'
    );
    
    COMMIT;
END;
/

PROMPT ✅ 7/9 - تم إنشاء Workspace: HR_PERFORMANCE

-- =================================================================================
-- 8. Workspace: HR_SYSTEM_ADMIN
-- =================================================================================

BEGIN
    APEX_INSTANCE_ADMIN.ADD_WORKSPACE(
        p_workspace      => 'HR_SYSTEM_ADMIN',
        p_primary_schema => 'HR_System_Admin',
        p_additional_schemas => 'HR_CORE:HR_PERSONNEL:HR_ATTENDANCE:HR_LEAVES:HR_PAYROLL:HR_RECRUITMENT:HR_PERFORMANCE'
    );
    
    APEX_UTIL.SET_WORKSPACE(p_workspace => 'HR_SYSTEM_ADMIN');
    
    APEX_UTIL.CREATE_USER(
        p_user_name     => 'SYSTEM_ADMIN',
        p_email_address => 'sysadmin@hospital.com',
        p_web_password  => 'SysAdmin@2026',
        p_developer_privs => 'ADMIN:CREATE:DATA_LOADER:EDIT:HELP:MONITOR:SQL'
    );
    
    COMMIT;
END;
/

PROMPT ✅ 8/9 - تم إنشاء Workspace: HR_SYSTEM_ADMIN

-- =================================================================================
-- 9. Workspace: HR_SYSTEM_PDB
-- =================================================================================

BEGIN
    APEX_INSTANCE_ADMIN.ADD_WORKSPACE(
        p_workspace      => 'HR_SYSTEM_PDB',
        p_primary_schema => 'HR_System_PDB',
        p_additional_schemas => 'HR_CORE'
    );
    
    APEX_UTIL.SET_WORKSPACE(p_workspace => 'HR_SYSTEM_PDB');
    
    APEX_UTIL.CREATE_USER(
        p_user_name     => 'PDB_ADMIN',
        p_email_address => 'pdb@hospital.com',
        p_web_password  => 'PDB@2026',
        p_developer_privs => 'CREATE:EDIT:HELP:MONITOR'
    );
    
    COMMIT;
END;
/

PROMPT ✅ 9/9 - تم إنشاء Workspace: HR_SYSTEM_PDB

-- =================================================================================
-- التحقق من الـ Workspaces
-- =================================================================================

PROMPT ========================================
PROMPT التحقق من APEX Workspaces
PROMPT ========================================

SELECT workspace, workspace_id, TO_CHAR(created_on, 'YYYY-MM-DD') as created
FROM apex_workspaces
WHERE workspace LIKE 'HR%'
ORDER BY workspace;

PROMPT ========================================
PROMPT ✅ اكتمل إعداد APEX Workspaces (9/9)
PROMPT ========================================

PROMPT 
PROMPT 🌐 للوصول إلى APEX:
PROMPT ===================================
PROMPT URL: http://localhost:8080/ords
PROMPT 
PROMPT 📋 Workspaces & Users:
PROMPT ===================================
PROMPT 1. HR_CORE          → User: ADMIN            | Pass: Admin@2026
PROMPT 2. HR_PERSONNEL     → User: HR_MANAGER       | Pass: HRManager@2026
PROMPT 3. HR_ATTENDANCE    → User: ATTENDANCE_ADMIN | Pass: Attend@2026
PROMPT 4. HR_LEAVES        → User: LEAVE_ADMIN      | Pass: Leaves@2026
PROMPT 5. HR_PAYROLL       → User: PAYROLL_ADMIN    | Pass: Payroll@2026
PROMPT 6. HR_RECRUITMENT   → User: RECRUIT_ADMIN    | Pass: Recruit@2026
PROMPT 7. HR_PERFORMANCE   → User: PERF_ADMIN       | Pass: Perform@2026
PROMPT 8. HR_SYSTEM_ADMIN  → User: SYSTEM_ADMIN     | Pass: SysAdmin@2026
PROMPT 9. HR_SYSTEM_PDB    → User: PDB_ADMIN        | Pass: PDB@2026
PROMPT ===================================
