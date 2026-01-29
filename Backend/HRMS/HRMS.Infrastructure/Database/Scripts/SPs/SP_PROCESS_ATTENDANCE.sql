
--2️⃣ بروسيجر معالجة الحضور (SP_PROCESS_ATTENDANCE)

CREATE OR ALTER PROCEDURE [HR_ATTENDANCE].[SP_PROCESS_ATTENDANCE]
    @TARGET_DATE DATE,
    @USER_ID INT 
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. تنظيف السجلات القديمة لنفس اليوم
    DELETE FROM [HR_ATTENDANCE].[DAILY_ATTENDANCE] 
    WHERE [ATTENDANCE_DATE] = @TARGET_DATE;

    -- 2. إدراج البيانات مع إضافة عمود IS_DELETED
    INSERT INTO [HR_ATTENDANCE].[DAILY_ATTENDANCE]
    (
        [EMPLOYEE_ID],
        [ATTENDANCE_DATE],
        [PLANNED_SHIFT_ID],
        [ACTUAL_IN_TIME],
        [ACTUAL_OUT_TIME],
        [LATE_MINUTES],
        [EARLY_LEAVE_MINUTES],
        [OVERTIME_MINUTES],
        [STATUS],
        [CREATED_BY],
        [CREATED_AT],
        [IS_DELETED], -- العمود الذي تسبب في الخطأ
        [VERSION_NO]
    )
    SELECT 
        R.[EMPLOYEE_ID],
        R.[ROSTER_DATE],
        R.[SHIFT_ID],
        P.[FIRST_IN],
        P.[LAST_OUT],
        
        -- حساب دقائق التأخير
        CASE 
            WHEN P.[FIRST_IN] IS NOT NULL 
                 AND DATEDIFF(MINUTE, S.[START_TIME], CAST(P.[FIRST_IN] AS TIME)) > S.[GRACE_PERIOD_MINS]
            THEN DATEDIFF(MINUTE, S.[START_TIME], CAST(P.[FIRST_IN] AS TIME))
            ELSE 0 
        END,

        -- حساب دقائق الخروج المبكر
        CASE 
            WHEN P.[LAST_OUT] IS NOT NULL 
                 AND DATEDIFF(MINUTE, CAST(P.[LAST_OUT] AS TIME), S.[END_TIME]) > 0
            THEN DATEDIFF(MINUTE, CAST(P.[LAST_OUT] AS TIME), S.[END_TIME])
            ELSE 0 
        END,

        -- حساب الإضافي المعتمد (120 دقيقة)
        ISNULL((
            SELECT SUM(CAST([APPROVED_HOURS] AS DECIMAL(18,2)) * 60) 
            FROM [HR_ATTENDANCE].[OVERTIME_REQUESTS]
            WHERE [EMPLOYEE_ID] = R.[EMPLOYEE_ID] 
              AND [WORK_DATE] = R.[ROSTER_DATE] 
              AND [STATUS] = 'APPROVED'
        ), 0),

        -- تحديد الحالة
        CASE 
            WHEN R.[IS_OFF_DAY] = 1 THEN 'OFF'
            WHEN P.[FIRST_IN] IS NOT NULL THEN 'PRESENT'
            ELSE 'ABSENT'
        END,
        
        @USER_ID,               -- CREATED_BY
        GETDATE(),              -- CREATED_AT
        0,                      -- IS_DELETED (القيمة التي كانت ناقصة)
        1                       -- VERSION_NO
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