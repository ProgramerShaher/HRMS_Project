-- 1?? »—Ê”ÌÃ— «⁄ „«œ «·≈Ã«“… (sp_ApproveLeaveRequest)--

CREATE PROCEDURE [HR_LEAVES].[sp_ApproveLeaveRequest]
    @RequestId INT,
    @ManagerId INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        -- 1. Ã·» »Ì«‰«  «·ÿ·»
        DECLARE @EmpID INT, @StartDate DATE, @EndDate DATE, @Days DECIMAL(18,2);
        SELECT @EmpID = EMPLOYEE_ID, @StartDate = CAST(START_DATE AS DATE), 
               @EndDate = CAST(END_DATE AS DATE), @Days = DAYS_COUNT
        FROM [HR_LEAVES].[LEAVE_REQUESTS] WHERE REQUEST_ID = @RequestId AND STATUS = 'PENDING';

        IF @EmpID IS NOT NULL
        BEGIN
            -- 2.  ÕœÌÀ Õ«·… «·ÿ·»
            UPDATE [HR_LEAVES].[LEAVE_REQUESTS] 
            SET STATUS = 'APPROVED', IS_POSTED_TO_BALANCE = 1 WHERE REQUEST_ID = @RequestId;

            -- 3. Œ’„ «·—’Ìœ
            UPDATE [HR_LEAVES].[EMPLOYEE_LEAVE_BALANCES] 
            SET CURRENT_BALANCE = CURRENT_BALANCE - @Days 
            WHERE EMPLOYEE_ID = @EmpID AND YEAR = YEAR(@StartDate);

            -- 4.  ”ÃÌ· «·Õ—ﬂ… «·„«·Ì…
            INSERT INTO [HR_LEAVES].[LEAVE_TRANSACTIONS] (EMPLOYEE_ID, TRANSACTION_TYPE, DAYS, TRANSACTION_DATE, REFERENCE_ID, NOTES)
            VALUES (@EmpID, 'DEDUCTION', -@Days, GETDATE(), @RequestId, 'Approved by SP Manager: ' + CAST(@ManagerId AS VARCHAR));

            -- 5.  ÕœÌÀ «·—Ê” — ( ÕÊÌ· «·’›— ≈·Ï Ê«Õœ)
            UPDATE [HR_ATTENDANCE].[EMPLOYEE_ROSTERS]
            SET [STATUS] = 'ON_LEAVE', [IS_OFF_DAY] = 1
            WHERE [EMPLOYEE_ID] = @EmpID 
            AND CAST([ROSTER_DATE] AS DATE) BETWEEN @StartDate AND @EndDate;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END