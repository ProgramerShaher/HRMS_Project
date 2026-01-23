-- Check actual table structure
SELECT column_name, data_type, data_length, data_precision, data_scale, nullable
FROM all_tab_columns
WHERE owner = 'HR_RECRUITMENT' 
  AND table_name = 'CANDIDATES'
ORDER BY column_id;
