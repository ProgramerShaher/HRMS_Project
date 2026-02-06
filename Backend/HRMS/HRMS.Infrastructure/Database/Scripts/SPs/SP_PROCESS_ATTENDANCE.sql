CREATE OR ALTER PROCEDURE [HR_ATTENDANCE].[SP_PROCESS_ATTENDANCE]
    @TARGET_DATE DATE,
    @USER_ID INT 
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. تنظيف السجلات القديمة
    DELETE FROM [HR_ATTENDANCE].[DAILY_ATTENDANCE] 
    WHERE [ATTENDANCE_DATE] = @TARGET_DATE;

    -- 2. الإدخال (كل المنطق في جملة واحدة نظيفة)
    INSERT INTO [HR_ATTENDANCE].[DAILY_ATTENDANCE]
    (
        [EMPLOYEE_ID], [ATTENDANCE_DATE], [PLANNED_SHIFT_ID], 
        [ACTUAL_IN_TIME], [ACTUAL_OUT_TIME], [LATE_MINUTES], 
        [EARLY_LEAVE_MINUTES], [OVERTIME_MINUTES], [STATUS], 
        [CREATED_BY], [CREATED_AT], [IS_DELETED], [VERSION_NO]
    )
    SELECT 
        R.[EMPLOYEE_ID],
        R.[ROSTER_DATE],
        R.[SHIFT_ID],
        P.[FIRST_IN],
        P.[LAST_OUT],
        
        -- ✅ حساب التأخير الصافي (مع خصم الأذونات ومنع القيم السالبة فوراً)
        CASE 
            -- الشرط الأول: هل الموظف بصم وتأخر عن فترة السماح؟
            WHEN P.[FIRST_IN] IS NOT NULL 
                 AND DATEDIFF(MINUTE, S.[START_TIME], CAST(P.[FIRST_IN] AS TIME)) > S.[GRACE_PERIOD_MINS] 
            THEN 
                -- الشرط الثاني: هل (التأخير - الإذن) أقل من صفر؟
                CASE 
                    WHEN (
                        DATEDIFF(MINUTE, S.[START_TIME], CAST(P.[FIRST_IN] AS TIME)) - 
                        ISNULL((SELECT SUM([HOURS] * 60) 
                                FROM [HR_ATTENDANCE].[PERMISSION_REQUESTS] 
                                WHERE [EMPLOYEE_ID] = R.[EMPLOYEE_ID] 
                                AND CAST([PERMISSION_DATE] AS DATE) = R.[ROSTER_DATE] 
                                AND [PERMISSION_TYPE] = 'LateEntry' 
                                AND [STATUS] = 'APPROVED'), 0)
                    ) < 0 
                    THEN 0 -- إذا كان الإذن يغطي وزيادة، النتيجة صفر
                    ELSE 
                        -- وإلا، احسب الفرق
                        (DATEDIFF(MINUTE, S.[START_TIME], CAST(P.[FIRST_IN] AS TIME)) - 
                         ISNULL((SELECT SUM([HOURS] * 60) 
                                 FROM [HR_ATTENDANCE].[PERMISSION_REQUESTS] 
                                 WHERE [EMPLOYEE_ID] = R.[EMPLOYEE_ID] 
                                 AND CAST([PERMISSION_DATE] AS DATE) = R.[ROSTER_DATE] 
                                 AND [PERMISSION_TYPE] = 'LateEntry' 
                                 AND [STATUS] = 'APPROVED'), 0))
                END
            ELSE 0 
        END AS [LATE_MINUTES],

        -- حساب الخروج المبكر
        CASE 
            WHEN P.[LAST_OUT] IS NOT NULL AND DATEDIFF(MINUTE, CAST(P.[LAST_OUT] AS TIME), S.[END_TIME]) > 0
            THEN DATEDIFF(MINUTE, CAST(P.[LAST_OUT] AS TIME), S.[END_TIME])
            ELSE 0 
        END AS [EARLY_LEAVE_MINUTES],

        -- حساب الإضافي المعتمد
        ISNULL((
            SELECT SUM(CAST([APPROVED_HOURS] AS DECIMAL(18,2)) * 60) 
            FROM [HR_ATTENDANCE].[OVERTIME_REQUESTS]
            WHERE [EMPLOYEE_ID] = R.[EMPLOYEE_ID] 
              AND CAST([WORK_DATE] AS DATE) = R.[ROSTER_DATE] 
              AND [STATUS] = 'APPROVED'
        ), 0) AS [OVERTIME_MINUTES],

        -- الحالة
        CASE 
            WHEN R.[IS_OFF_DAY] = 1 THEN 'OFF'
            WHEN P.[FIRST_IN] IS NOT NULL THEN 'PRESENT'
            ELSE 'ABSENT'
        END AS [STATUS],
        
        @USER_ID, GETDATE(), 0, 1
    FROM [HR_ATTENDANCE].[EMPLOYEE_ROSTERS] R
    JOIN [HR_ATTENDANCE].[SHIFT_TYPES] S ON R.[SHIFT_ID] = S.[SHIFT_ID]
    LEFT JOIN (
        SELECT [EMPLOYEE_ID], 
               MIN(CASE WHEN [PUNCH_TYPE] = 'IN' THEN [PUNCH_TIME] END) AS [FIRST_IN],
               MAX(CASE WHEN [PUNCH_TYPE] = 'OUT' THEN [PUNCH_TIME] END) AS [LAST_OUT]
        FROM [HR_ATTENDANCE].[RAW_PUNCH_LOGS]
        WHERE CAST([PUNCH_TIME] AS DATE) = @TARGET_DATE
        GROUP BY [EMPLOYEE_ID]
    ) P ON R.[EMPLOYEE_ID] = P.[EMPLOYEE_ID]
    WHERE R.[ROSTER_DATE] = @TARGET_DATE;
END
GO