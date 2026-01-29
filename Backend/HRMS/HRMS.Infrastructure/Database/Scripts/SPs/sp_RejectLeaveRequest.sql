--3️⃣ بروسيجر رفض الإجازة (sp_RejectLeaveRequest)

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
    SET [STATUS] = 'REJECTED', 
        REJECTION_REASON = @Comment,
        CREATED_AT = GETDATE(),
        CREATED_BY = CAST(@ManagerId AS VARCHAR(50))
    WHERE REQUEST_ID = @RequestId AND [STATUS] = 'PENDING';
    
    PRINT 'SUCCESS: Leave Request Rejected.';
END;
GO