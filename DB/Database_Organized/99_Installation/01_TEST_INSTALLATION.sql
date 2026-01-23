-- Quick Test Script - Verify Installation
-- Run this after installation to verify everything works

SET SERVEROUTPUT ON
ALTER SESSION SET CONTAINER = FREEPDB1;

PROMPT ========================================
PROMPT Testing HRMS Installation
PROMPT ========================================

PROMPT
PROMPT [1/5] Checking Schemas...
SELECT username, account_status, created 
FROM dba_users 
WHERE username LIKE 'HR_%'
ORDER BY username;

PROMPT
PROMPT [2/5] Checking Tables...
SELECT owner, COUNT(*) as table_count
FROM all_tables
WHERE owner LIKE 'HR_%'
GROUP BY owner
ORDER BY owner;

PROMPT
PROMPT [3/5] Checking Foreign Keys...
SELECT owner, COUNT(*) as fk_count
FROM all_constraints
WHERE owner LIKE 'HR_%'
  AND constraint_type = 'R'
GROUP BY owner
ORDER BY owner;

PROMPT
PROMPT [4/5] Checking Packages...
SELECT owner, object_name, status
FROM all_objects
WHERE owner LIKE 'HR_%'
  AND object_type = 'PACKAGE'
ORDER BY owner, object_name;

PROMPT
PROMPT [5/5] Checking Views and Triggers...
SELECT owner, object_type, COUNT(*) as count
FROM all_objects
WHERE owner LIKE 'HR_%'
  AND object_type IN ('VIEW', 'TRIGGER')
GROUP BY owner, object_type
ORDER BY owner, object_type;

PROMPT
PROMPT ========================================
PROMPT Expected Results:
PROMPT ========================================
PROMPT Schemas: 7
PROMPT Tables: 75
PROMPT Foreign Keys: 92
PROMPT Packages: 7 (all VALID)
PROMPT Views: 15+
PROMPT Triggers: 10+
PROMPT ========================================

PROMPT
PROMPT Testing complete!
