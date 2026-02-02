-- =============================================
-- Script Name: 05_Create_Approvals_Tables.sql
-- Description: Creates tables for Leave Approval History and Workflow Approvals
-- Author: Antigravity
-- Date: 2026-02-02
-- =============================================

USE [HRMS_DB]
GO

-- 1. Create LEAVE_APPROVAL_HISTORY Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HR_LEAVES].[LEAVE_APPROVAL_HISTORY]') AND type in (N'U'))
BEGIN
    CREATE TABLE [HR_LEAVES].[LEAVE_APPROVAL_HISTORY](
        [HISTORY_ID] [int] IDENTITY(1,1) NOT NULL,
        [REQUEST_ID] [int] NOT NULL,
        [ACTION_TYPE] [nvarchar](50) NOT NULL,
        [PERFORMED_BY_USER_ID] [nvarchar](50) NULL,
        [PERFORMED_BY_EMPLOYEE_ID] [int] NULL,
        [ACTION_DATE] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
        [COMMENT] [nvarchar](500) NULL,
        [PREVIOUS_STATUS] [nvarchar](50) NULL,
        [NEW_STATUS] [nvarchar](50) NULL,
        
        -- Base Entity Columns
        [CREATED_BY] [nvarchar](50) NULL,
        [CREATED_AT] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
        [UPDATED_BY] [nvarchar](50) NULL,
        [UPDATED_AT] [datetime2](7) NULL,
        [IS_DELETED] [tinyint] NOT NULL DEFAULT 0,
        [VERSION_NO] [int] NOT NULL DEFAULT 1,

        CONSTRAINT [PK_LEAVE_APPROVAL_HISTORY] PRIMARY KEY CLUSTERED ([HISTORY_ID] ASC),
        CONSTRAINT [FK_LEAVE_APPROVAL_HISTORY_REQUESTS] FOREIGN KEY ([REQUEST_ID]) REFERENCES [HR_LEAVES].[LEAVE_REQUESTS] ([REQUEST_ID])
    );
    
    PRINT 'Table [HR_LEAVES].[LEAVE_APPROVAL_HISTORY] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [HR_LEAVES].[LEAVE_APPROVAL_HISTORY] already exists.';
END
GO

-- 2. Create WORKFLOW_APPROVALS Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HR_CORE].[WORKFLOW_APPROVALS]') AND type in (N'U'))
BEGIN
    CREATE TABLE [HR_CORE].[WORKFLOW_APPROVALS](
        [APPROVAL_ID] [bigint] IDENTITY(1,1) NOT NULL,
        [REQUEST_TYPE] [nvarchar](50) NOT NULL,
        [REQUEST_ID] [int] NOT NULL,
        [APPROVER_LEVEL] [tinyint] NOT NULL,
        [APPROVER_ID] [int] NOT NULL,
        [STATUS] [nvarchar](20) NULL DEFAULT 'PENDING',
        [APPROVAL_DATE] [datetime2](7) NULL,
        [COMMENTS] [nvarchar](500) NULL,

        -- Base Entity Columns
        [CREATED_BY] [nvarchar](50) NULL,
        [CREATED_AT] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
        [UPDATED_BY] [nvarchar](50) NULL,
        [UPDATED_AT] [datetime2](7) NULL,
        [IS_DELETED] [tinyint] NOT NULL DEFAULT 0,
        [VERSION_NO] [int] NOT NULL DEFAULT 1,

        CONSTRAINT [PK_WORKFLOW_APPROVALS] PRIMARY KEY CLUSTERED ([APPROVAL_ID] ASC)
    );
    -- Note: APPROVER_ID probably references HR_PERSONNEL.EMPLOYEES, but request ID references dynamic tables based on type.
    -- We can add FK for Approver if needed.
    ALTER TABLE [HR_CORE].[WORKFLOW_APPROVALS]  WITH CHECK ADD  CONSTRAINT [FK_WORKFLOW_APPROVALS_EMPLOYEES] FOREIGN KEY([APPROVER_ID])
    REFERENCES [HR_PERSONNEL].[EMPLOYEES] ([EMPLOYEE_ID]);

    ALTER TABLE [HR_CORE].[WORKFLOW_APPROVALS] CHECK CONSTRAINT [FK_WORKFLOW_APPROVALS_EMPLOYEES];

    PRINT 'Table [HR_CORE].[WORKFLOW_APPROVALS] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [HR_CORE].[WORKFLOW_APPROVALS] already exists.';
END
GO
