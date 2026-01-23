-- =================================================================================
-- HRMS System - Complete Installation for Oracle APEX Cloud
-- =================================================================================
-- Version: 1.0
-- Date: 2026-01-23
-- Description: Single script to install entire HRMS system on Oracle Cloud
-- =================================================================================

SET DEFINE OFF;
SET SERVEROUTPUT ON;

PROMPT ========================================
PROMPT HRMS Cloud Installation Started
PROMPT ========================================
PROMPT
PROMPT This will install:
PROMPT - Core Tables (9 tables)
PROMPT - Security Context & Package
PROMPT - Audit Triggers (9 triggers)
PROMPT - Business Logic Package
PROMPT - Dashboard Views (8 views)
PROMPT - Sample Data (Yemen)
PROMPT
PROMPT Estimated time: 3-5 minutes
PROMPT ========================================

-- =================================================================================
-- PHASE 1: CREATE TABLES
-- =================================================================================

PROMPT
PROMPT ========================================
PROMPT Phase 1/6: Creating Tables
PROMPT ========================================

-- Note: In Oracle Cloud, you already have a schema
-- No need to create schemas, just create tables

-- Include the complete table creation script here
-- This will be populated from 00_ALL_CORE_TABLES.sql

@@../02_Core_Data/HR_CORE/Tables/00_ALL_CORE_TABLES.sql

PROMPT ✅ Tables created successfully

-- =================================================================================
-- PHASE 2: CREATE SECURITY CONTEXT
-- =================================================================================

PROMPT
PROMPT ========================================
PROMPT Phase 2/6: Creating Security Context
PROMPT ========================================

@@../02_Core_Data/HR_CORE/Security/00_CREATE_CONTEXT.sql

PROMPT ✅ Security Context created

-- =================================================================================
-- PHASE 3: CREATE SECURITY PACKAGE
-- =================================================================================

PROMPT
PROMPT ========================================
PROMPT Phase 3/6: Creating Security Package
PROMPT ========================================

@@../02_Core_Data/HR_CORE/Security/01_PKG_SECURITY_CONTEXT_SPEC.sql
@@../02_Core_Data/HR_CORE/Security/02_PKG_SECURITY_CONTEXT_BODY.sql

PROMPT ✅ Security Package created

-- =================================================================================
-- PHASE 4: CREATE AUDIT TRIGGERS
-- =================================================================================

PROMPT
PROMPT ========================================
PROMPT Phase 4/6: Creating Audit Triggers
PROMPT ========================================

@@../02_Core_Data/HR_CORE/Triggers/01_AUTO_AUDIT_TRIGGERS.sql

PROMPT ✅ Audit Triggers created

-- =================================================================================
-- PHASE 5: CREATE BUSINESS LOGIC PACKAGE
-- =================================================================================

PROMPT
PROMPT ========================================
PROMPT Phase 5/6: Creating Business Package
PROMPT ========================================

@@../02_Core_Data/HR_CORE/Packages/Specifications/PKG_HR_CORE_MANAGER_SPEC.sql
@@../02_Core_Data/HR_CORE/Packages/Bodies/PKG_HR_CORE_MANAGER_BODY.sql

PROMPT ✅ Business Package created

-- =================================================================================
-- PHASE 6: CREATE DASHBOARD VIEWS
-- =================================================================================

PROMPT
PROMPT ========================================
PROMPT Phase 6/6: Creating Dashboard Views
PROMPT ========================================

@@../02_Core_Data/HR_CORE/Views/02_DASHBOARD_VIEWS.sql

PROMPT ✅ Dashboard Views created

-- =================================================================================
-- PHASE 7: INSERT SAMPLE DATA (OPTIONAL)
-- =================================================================================

PROMPT
PROMPT ========================================
PROMPT Phase 7/7: Inserting Sample Data
PROMPT ========================================

@@../10_Sample_Data/01_YEMEN_CORE_DATA.sql

PROMPT ✅ Sample Data inserted

-- =================================================================================
-- VERIFICATION
-- =================================================================================

PROMPT
PROMPT ========================================
PROMPT Installation Verification
PROMPT ========================================

-- Check Tables
SELECT 'Tables' AS object_type, COUNT(*) AS count
FROM user_tables
WHERE table_name IN (
    'COUNTRIES', 'CITIES', 'BRANCHES', 'DEPARTMENTS',
    'JOBS', 'JOB_GRADES', 'DOCUMENT_TYPES', 'BANKS', 'SYSTEM_SETTINGS'
)
UNION ALL
-- Check Triggers
SELECT 'Triggers', COUNT(*)
FROM user_triggers
WHERE trigger_name LIKE 'TRG_%AUDIT'
UNION ALL
-- Check Packages
SELECT 'Packages', COUNT(*)
FROM user_objects
WHERE object_type = 'PACKAGE'
AND object_name IN ('PKG_SECURITY_CONTEXT', 'PKG_HR_CORE_MANAGER')
UNION ALL
-- Check Views
SELECT 'Views', COUNT(*)
FROM user_views
WHERE view_name LIKE 'V_%';

PROMPT
PROMPT ========================================
PROMPT Expected Results:
PROMPT - Tables: 9
PROMPT - Triggers: 9
PROMPT - Packages: 2
PROMPT - Views: 8
PROMPT ========================================

PROMPT
PROMPT ========================================
PROMPT ✅ Installation Complete!
PROMPT ========================================
PROMPT
PROMPT Next Steps:
PROMPT 1. Go to App Builder
PROMPT 2. Create New Application
PROMPT 3. Use the Dashboard Views for charts
PROMPT 4. Create Forms using the tables
PROMPT
PROMPT Happy Building! 🚀
PROMPT ========================================
