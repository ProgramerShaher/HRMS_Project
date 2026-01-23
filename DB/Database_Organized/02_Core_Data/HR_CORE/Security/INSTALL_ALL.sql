-- =================================================================================
-- تنصيب Security + Triggers (كل شيء في ملف واحد)
-- نفذ هذا الملف في VS Code على HR_CORE connection
-- =================================================================================

PROMPT ========================================
PROMPT Installing Security System
PROMPT ========================================

-- 1. Create Context
@@00_CREATE_CONTEXT.sql

-- 2. Create PKG_SECURITY_CONTEXT Specification
@@01_PKG_SECURITY_CONTEXT_SPEC.sql

-- 3. Create PKG_SECURITY_CONTEXT Body
@@02_PKG_SECURITY_CONTEXT_BODY.sql

PROMPT ========================================
PROMPT Installing Audit Triggers
PROMPT ========================================

-- 4. Create Audit Triggers
@@../Triggers/01_AUTO_AUDIT_TRIGGERS.sql

PROMPT ========================================
PROMPT ✅ Installation Complete!
PROMPT ========================================
PROMPT
PROMPT Verifying Installation:
PROMPT ========================================

-- Verify PKG_SECURITY_CONTEXT
SELECT object_name, object_type, status 
FROM user_objects 
WHERE object_name = 'PKG_SECURITY_CONTEXT';

-- Verify Triggers
SELECT trigger_name, status 
FROM user_triggers 
WHERE trigger_name LIKE 'TRG_%AUDIT';

PROMPT ========================================
PROMPT Expected Results:
PROMPT - PKG_SECURITY_CONTEXT (PACKAGE) - VALID
PROMPT - PKG_SECURITY_CONTEXT (PACKAGE BODY) - VALID
PROMPT - 9 Triggers - ENABLED
PROMPT ========================================
