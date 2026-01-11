-- WhatsApp Support Migration Script
-- Run this script to add WhatsApp communication tables

-- Create WhatsAppMessageLogs table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WhatsAppMessageLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[WhatsAppMessageLogs] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [PhoneNumber] NVARCHAR(20) NOT NULL,
        [Message] NVARCHAR(2000) NOT NULL,
        [Direction] INT NOT NULL DEFAULT 0,
        [MessageType] INT NOT NULL DEFAULT 0,
        [Status] INT NOT NULL DEFAULT 0,
        [ExternalMessageId] NVARCHAR(100) NULL,
        [RelatedEntityId] INT NULL,
        [RelatedEntityType] NVARCHAR(50) NULL,
        [Timestamp] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [DeliveredAt] DATETIME2 NULL,
        [ReadAt] DATETIME2 NULL,
        [ErrorMessage] NVARCHAR(500) NULL,
        [TenantId] INT NULL,
        CONSTRAINT [PK_WhatsAppMessageLogs] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    PRINT 'WhatsAppMessageLogs table created successfully';
END
ELSE
BEGIN
    PRINT 'WhatsAppMessageLogs table already exists';
END
GO

-- Create indexes for better performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_WhatsAppMessageLogs_PhoneNumber')
BEGIN
    CREATE INDEX [IX_WhatsAppMessageLogs_PhoneNumber] ON [dbo].[WhatsAppMessageLogs] ([PhoneNumber]);
    PRINT 'Index IX_WhatsAppMessageLogs_PhoneNumber created';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_WhatsAppMessageLogs_Timestamp')
BEGIN
    CREATE INDEX [IX_WhatsAppMessageLogs_Timestamp] ON [dbo].[WhatsAppMessageLogs] ([Timestamp] DESC);
    PRINT 'Index IX_WhatsAppMessageLogs_Timestamp created';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_WhatsAppMessageLogs_ExternalMessageId')
BEGIN
    CREATE INDEX [IX_WhatsAppMessageLogs_ExternalMessageId] ON [dbo].[WhatsAppMessageLogs] ([ExternalMessageId]);
    PRINT 'Index IX_WhatsAppMessageLogs_ExternalMessageId created';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_WhatsAppMessageLogs_TenantId')
BEGIN
    CREATE INDEX [IX_WhatsAppMessageLogs_TenantId] ON [dbo].[WhatsAppMessageLogs] ([TenantId]);
    PRINT 'Index IX_WhatsAppMessageLogs_TenantId created';
END
GO

PRINT 'WhatsApp support migration completed successfully';
