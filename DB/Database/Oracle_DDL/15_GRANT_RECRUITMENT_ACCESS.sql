-- =================================================================================
-- Script: 15_GRANT_RECRUITMENT_ACCESS.sql
-- Description: Granting permissions on HR_RECRUITMENT tables to HR_CORE user
-- Reason: The API connects as HR_CORE and needs to access Recruitment tables.
-- Run as: SYSTEM or HR_RECRUITMENT
-- =================================================================================

ALTER SESSION SET CONTAINER = XEPDB1;

-- Grant permissions on JOB_VACANCIES
GRANT SELECT, INSERT, UPDATE, DELETE ON HR_RECRUITMENT.JOB_VACANCIES TO HR_CORE;

-- Grant permissions on CANDIDATES
GRANT SELECT, INSERT, UPDATE, DELETE ON HR_RECRUITMENT.CANDIDATES TO HR_CORE;

-- Grant permissions on APPLICATIONS
GRANT SELECT, INSERT, UPDATE, DELETE ON HR_RECRUITMENT.APPLICATIONS TO HR_CORE;

-- Grant permissions on INTERVIEWS
GRANT SELECT, INSERT, UPDATE, DELETE ON HR_RECRUITMENT.INTERVIEWS TO HR_CORE;

-- Grant permissions on OFFERS
GRANT SELECT, INSERT, UPDATE, DELETE ON HR_RECRUITMENT.OFFERS TO HR_CORE;

-- Allow HR_CORE to read the sequences if any (Identity columns handle this usually, but good practice)
-- GRANT SELECT ON HR_RECRUITMENT.ISEQ$$_... TO HR_CORE; -- (Identity columns manage their own sequences usually)

SELECT 'Permissions on HR_RECRUITMENT tables granted to HR_CORE successfully.' AS STATUS FROM DUAL;
