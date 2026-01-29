/* «”„ «·„·›: ”ﬂ—Ì» _≈Ã—«¡« _«·≈Ã«“« _v1.sql
   «·Ê’›: ÌÕ ÊÌ ⁄·Ï „‰ÿﬁ «·„Ê«›ﬁ… Ê«·—›÷ Ê„“«„‰… «·—Ê” —
*/

USE [HRMS_Hospital]; 
GO

-----------------------------------------------------------
-- 1. »—Ê”ÌÃ— «⁄ „«œ «·≈Ã«“… («·–Ì ÌÕÊ· «·’›— ≈·Ï Ê«Õœ)
-----------------------------------------------------------
IF OBJECT_ID('[HR_LEAVES].[sp_ApproveLeaveRequest]', 'P') IS NOT NULL
    DROP PROCEDURE [HR_LEAVES].[sp_ApproveLeaveRequest];
GO

CREATE PROCEDURE [HR_LEAVES].[sp_ApproveLeaveRequest]
    @RequestId INT,
    @ManagerId INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @EmpID INT, @StartDate DATE, @EndDate DATE, @Days DECIMAL(18,2), @LeaveTypeID INT;
        
        SELECT @EmpID = EMPLOYEE_ID, @StartDate = CAST(START_DATE AS DATE), 
               @EndDate = CAST(END_DATE AS DATE), @Days = DAYS_COUNT, @LeaveTypeID = LEAVE_TYPE_ID
        FROM [HR_LEAVES].[LEAVE_REQUESTS] 
        WHERE REQUEST_ID = @RequestId AND [STATUS] = 'PENDING';

        IF @EmpID IS NULL
            THROW 50001, N'«·ÿ·» €Ì— „ÊÃÊœ √Ê  „  „⁄«·Ã Â „”»ﬁ«.', 1;

        UPDATE [HR_LEAVES].[LEAVE_REQUESTS] SET [STATUS] = 'APPROVED', IS_POSTED_TO_BALANCE = 1 WHERE REQUEST_ID = @RequestId;
        UPDATE [HR_LEAVES].[EMPLOYEE_LEAVE_BALANCES] SET CURRENT_BALANCE = CURRENT_BALANCE - @Days WHERE EMPLOYEE_ID = @EmpID AND LEAVE_TYPE_ID = @LeaveTypeID AND [YEAR] = YEAR(@StartDate);
        INSERT INTO [HR_LEAVES].[LEAVE_TRANSACTIONS] (EMPLOYEE_ID, LEAVE_TYPE_ID, TRANSACTION_TYPE, DAYS, TRANSACTION_DATE, REFERENCE_ID)
        VALUES (@EmpID, @LeaveTypeID, 'DEDUCTION', -@Days, GETDATE(), @RequestId);

        --  ÕœÌÀ «·—Ê” — (ﬁ·» «·‰Ÿ«„)
        UPDATE [HR_ATTENDANCE].[EMPLOYEE_ROSTERS]
        SET [STATUS] = 'ON_LEAVE', [IS_OFF_DAY] = 1
        WHERE [EMPLOYEE_ID] = @EmpID AND CAST([ROSTER_DATE] AS DATE) BETWEEN @StartDate AND @EndDate;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-----------------------------------------------------------
-- 2. »—Ê”ÌÃ— —›÷ «·≈Ã«“… (·· ÊÀÌﬁ ›ﬁÿ)
-----------------------------------------------------------
IF OBJECT_ID('[HR_LEAVES].[sp_RejectLeaveRequest]', 'P') IS NOT NULL
    DROP PROCEDURE [HR_LEAVES].[sp_RejectLeaveRequest];
GO

CREATE PROCEDURE [HR_LEAVES].[sp_RejectLeaveRequest]
    @RequestId INT,
    @ManagerId INT,
    @Comment NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [HR_LEAVES].[LEAVE_REQUESTS] 
    SET [STATUS] = 'REJECTED', REJECTION_REASON = @Comment, CREATED_AT = GETDATE(), CREATED_BY = CAST(@ManagerId AS VARCHAR(50))
    WHERE REQUEST_ID = @RequestId AND [STATUS] = 'PENDING';
END;
GO