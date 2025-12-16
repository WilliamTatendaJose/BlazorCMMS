-- Manual Migration: Add Missing WorkOrder Columns
-- Run this script in SQL Server Management Studio if Update-Database fails

-- First, mark the initial migration as applied if not already
IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20251130155230_InitialRBM_CMMS')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20251130155230_InitialRBM_CMMS', '9.0.0');
END
GO

-- Add missing columns to WorkOrders table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'RequestedBy')
    ALTER TABLE WorkOrders ADD RequestedBy nvarchar(200) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'RequestedDate')
    ALTER TABLE WorkOrders ADD RequestedDate datetime2 NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'RequestReason')
    ALTER TABLE WorkOrders ADD RequestReason nvarchar(2000) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'ApprovedBy')
    ALTER TABLE WorkOrders ADD ApprovedBy nvarchar(200) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'ApprovedDate')
    ALTER TABLE WorkOrders ADD ApprovedDate datetime2 NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'ApprovalNotes')
    ALTER TABLE WorkOrders ADD ApprovalNotes nvarchar(2000) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'RejectedBy')
    ALTER TABLE WorkOrders ADD RejectedBy nvarchar(200) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'RejectedDate')
    ALTER TABLE WorkOrders ADD RejectedDate datetime2 NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'RejectionReason')
    ALTER TABLE WorkOrders ADD RejectionReason nvarchar(2000) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'Category')
    ALTER TABLE WorkOrders ADD Category nvarchar(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'SubCategory')
    ALTER TABLE WorkOrders ADD SubCategory nvarchar(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'Location')
    ALTER TABLE WorkOrders ADD Location nvarchar(200) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'Building')
    ALTER TABLE WorkOrders ADD Building nvarchar(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'Floor')
    ALTER TABLE WorkOrders ADD Floor nvarchar(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'ContactPerson')
    ALTER TABLE WorkOrders ADD ContactPerson nvarchar(200) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'ContactPhone')
    ALTER TABLE WorkOrders ADD ContactPhone nvarchar(50) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'ContactEmail')
    ALTER TABLE WorkOrders ADD ContactEmail nvarchar(200) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'ScheduledStartDate')
    ALTER TABLE WorkOrders ADD ScheduledStartDate datetime2 NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'ScheduledEndDate')
    ALTER TABLE WorkOrders ADD ScheduledEndDate datetime2 NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'RequiresShutdown')
    ALTER TABLE WorkOrders ADD RequiresShutdown bit NOT NULL DEFAULT 0;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'RequiresSafetyPermit')
    ALTER TABLE WorkOrders ADD RequiresSafetyPermit bit NOT NULL DEFAULT 0;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'SafetyPermitNumber')
    ALTER TABLE WorkOrders ADD SafetyPermitNumber nvarchar(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'IsRecurring')
    ALTER TABLE WorkOrders ADD IsRecurring bit NOT NULL DEFAULT 0;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'RecurrencePattern')
    ALTER TABLE WorkOrders ADD RecurrencePattern nvarchar(50) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'RecurrenceInterval')
    ALTER TABLE WorkOrders ADD RecurrenceInterval int NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'ParentWorkOrderId')
    ALTER TABLE WorkOrders ADD ParentWorkOrderId int NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'LastModifiedBy')
    ALTER TABLE WorkOrders ADD LastModifiedBy nvarchar(200) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'LastModifiedDate')
    ALTER TABLE WorkOrders ADD LastModifiedDate datetime2 NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'Department')
    ALTER TABLE WorkOrders ADD Department nvarchar(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'Originator')
    ALTER TABLE WorkOrders ADD Originator nvarchar(200) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'FaultDate')
    ALTER TABLE WorkOrders ADD FaultDate datetime2 NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'FaultTime')
    ALTER TABLE WorkOrders ADD FaultTime datetime2 NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'JobNumber')
    ALTER TABLE WorkOrders ADD JobNumber nvarchar(50) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'IsAcknowledged')
    ALTER TABLE WorkOrders ADD IsAcknowledged bit NOT NULL DEFAULT 0;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'AcknowledgedDate')
    ALTER TABLE WorkOrders ADD AcknowledgedDate datetime2 NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'AcknowledgedBy')
    ALTER TABLE WorkOrders ADD AcknowledgedBy nvarchar(200) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'LockOutRequired')
    ALTER TABLE WorkOrders ADD LockOutRequired bit NOT NULL DEFAULT 0;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'JobType')
    ALTER TABLE WorkOrders ADD JobType nvarchar(50) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'IsMechanical')
    ALTER TABLE WorkOrders ADD IsMechanical bit NOT NULL DEFAULT 0;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'IsElectrical')
    ALTER TABLE WorkOrders ADD IsElectrical bit NOT NULL DEFAULT 0;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'ArtisanName')
    ALTER TABLE WorkOrders ADD ArtisanName nvarchar(200) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'ArtisanSignature')
    ALTER TABLE WorkOrders ADD ArtisanSignature nvarchar(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'TimeSubmitted')
    ALTER TABLE WorkOrders ADD TimeSubmitted datetime2 NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'TimeDone')
    ALTER TABLE WorkOrders ADD TimeDone datetime2 NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'TimeCompleted')
    ALTER TABLE WorkOrders ADD TimeCompleted datetime2 NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'HousekeepingAffected')
    ALTER TABLE WorkOrders ADD HousekeepingAffected bit NOT NULL DEFAULT 0;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'HousekeepingNotes')
    ALTER TABLE WorkOrders ADD HousekeepingNotes nvarchar(500) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'DetailsOfRequest')
    ALTER TABLE WorkOrders ADD DetailsOfRequest nvarchar(2000) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'DetailsOfWorkCarriedOut')
    ALTER TABLE WorkOrders ADD DetailsOfWorkCarriedOut nvarchar(2000) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'CorrectiveAction')
    ALTER TABLE WorkOrders ADD CorrectiveAction nvarchar(2000) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'OriginatorVerification')
    ALTER TABLE WorkOrders ADD OriginatorVerification nvarchar(200) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'OriginatorVerificationDate')
    ALTER TABLE WorkOrders ADD OriginatorVerificationDate datetime2 NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'EngineeringForemanVerification')
    ALTER TABLE WorkOrders ADD EngineeringForemanVerification nvarchar(200) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'EngineeringForemanVerificationDate')
    ALTER TABLE WorkOrders ADD EngineeringForemanVerificationDate datetime2 NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'HODVerification')
    ALTER TABLE WorkOrders ADD HODVerification nvarchar(200) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'HODVerificationDate')
    ALTER TABLE WorkOrders ADD HODVerificationDate datetime2 NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'HandOverTime')
    ALTER TABLE WorkOrders ADD HandOverTime datetime2 NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'BreakdownType')
    ALTER TABLE WorkOrders ADD BreakdownType nvarchar(20) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'AnyOtherDetails')
    ALTER TABLE WorkOrders ADD AnyOtherDetails nvarchar(2000) NULL;

GO

-- Mark the worksOrders migration as applied
IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20251204151251_worksOrders')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20251204151251_worksOrders', '9.0.0');
END
GO

PRINT 'Migration completed successfully!';
