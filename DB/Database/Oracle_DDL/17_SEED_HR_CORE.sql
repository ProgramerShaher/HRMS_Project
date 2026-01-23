-- =================================================================================
-- Script: 17_SEED_HR_CORE.sql
-- Description: Insert sample data into HR_CORE tables (Countries).
-- Run as: HR_CORE
-- =================================================================================

ALTER SESSION SET CONTAINER = FREEPDB1;

INSERT INTO HR_CORE.COUNTRIES (COUNTRY_NAME_AR, COUNTRY_NAME_EN, CITIZENSHIP_NAME_AR, ISO_CODE, CREATED_BY)
VALUES ('المملكة العربية السعودية', 'Saudi Arabia', 'سعودي', 'SA', 'SYSTEM');

INSERT INTO HR_CORE.COUNTRIES (COUNTRY_NAME_AR, COUNTRY_NAME_EN, CITIZENSHIP_NAME_AR, ISO_CODE, CREATED_BY)
VALUES ('الإمارات العربية المتحدة', 'United Arab Emirates', 'إماراتي', 'AE', 'SYSTEM');

INSERT INTO HR_CORE.COUNTRIES (COUNTRY_NAME_AR, COUNTRY_NAME_EN, CITIZENSHIP_NAME_AR, ISO_CODE, CREATED_BY)
VALUES ('جمهورية مصر العربية', 'Egypt', 'مصري', 'EG', 'SYSTEM');

COMMIT;

SELECT 'Sample Countries inserted successfully.' AS STATUS FROM DUAL;
