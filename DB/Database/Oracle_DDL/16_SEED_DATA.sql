-- =================================================================================
-- Script: 16_SEED_DATA.sql
-- Description: Insert sample data into HR_RECRUITMENT tables for testing.
-- Run as: HR_RECRUITMENT or SYSTEM
-- =================================================================================

ALTER SESSION SET CONTAINER = FREEPDB1;

-- Insert Sample Candidates
INSERT INTO HR_RECRUITMENT.CANDIDATES (
    FIRST_NAME_AR, FAMILY_NAME_AR, FULL_NAME_EN, EMAIL, PHONE, 
    CV_FILE_PATH, LINKEDIN_PROFILE, CREATED_BY
) VALUES (
    'أحمد', 'علي', 'Ahmed Ali', 'ahmed.ali@example.com', '0501234567', 
    '/files/cvs/ahmed_ali.pdf', 'linkedin.com/in/ahmedali', 'SYSTEM'
);

INSERT INTO HR_RECRUITMENT.CANDIDATES (
    FIRST_NAME_AR, FAMILY_NAME_AR, FULL_NAME_EN, EMAIL, PHONE, 
    CV_FILE_PATH, LINKEDIN_PROFILE, CREATED_BY
) VALUES (
    'سارة', 'محمد', 'Sara Mohammed', 'sara.mohammed@example.com', '0509876543', 
    '/files/cvs/sara_mohammed.pdf', 'linkedin.com/in/saramohammed', 'SYSTEM'
);

INSERT INTO HR_RECRUITMENT.CANDIDATES (
    FIRST_NAME_AR, FAMILY_NAME_AR, FULL_NAME_EN, EMAIL, PHONE, 
    CV_FILE_PATH, LINKEDIN_PROFILE, CREATED_BY
) VALUES (
    'خالد', 'عبدالله', 'Khaled Abdullah', 'khaled.abdullah@example.com', '0561122334', 
    '/files/cvs/khaled_abdullah.pdf', 'linkedin.com/in/khaledabdullah', 'SYSTEM'
);

COMMIT;

SELECT 'Sample data inserted successfully (3 Candidates).' AS STATUS FROM DUAL;
