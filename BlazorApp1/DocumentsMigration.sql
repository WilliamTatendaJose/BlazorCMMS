BEGIN TRANSACTION;
CREATE TABLE [Documents] (
    [Id] int NOT NULL IDENTITY,
    [DocumentNumber] nvarchar(100) NOT NULL,
    [Title] nvarchar(300) NOT NULL,
    [Description] nvarchar(2000) NOT NULL,
    [Category] nvarchar(100) NOT NULL,
    [SubCategory] nvarchar(100) NOT NULL,
    [FileName] nvarchar(500) NOT NULL,
    [FilePath] nvarchar(1000) NOT NULL,
    [FileType] nvarchar(100) NOT NULL,
    [FileSize] bigint NOT NULL,
    [Version] nvarchar(50) NOT NULL,
    [RevisionNumber] int NOT NULL,
    [Status] nvarchar(100) NOT NULL,
    [AssetId] int NULL,
    [WorkOrderId] int NULL,
    [FailureModeId] int NULL,
    [Tags] nvarchar(500) NOT NULL,
    [Author] nvarchar(200) NOT NULL,
    [Department] nvarchar(200) NOT NULL,
    [EffectiveDate] datetime2 NULL,
    [ExpiryDate] datetime2 NULL,
    [ReviewDate] datetime2 NULL,
    [ReviewedBy] nvarchar(200) NOT NULL,
    [ApprovedBy] nvarchar(200) NOT NULL,
    [ApprovalDate] datetime2 NULL,
    [AccessLevel] nvarchar(100) NOT NULL,
    [AllowedRoles] nvarchar(500) NOT NULL,
    [DownloadCount] int NOT NULL,
    [ViewCount] int NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(200) NOT NULL,
    [ModifiedDate] datetime2 NULL,
    [ModifiedBy] nvarchar(200) NOT NULL,
    [Notes] nvarchar(2000) NOT NULL,
    CONSTRAINT [PK_Documents] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Documents_Assets_AssetId] FOREIGN KEY ([AssetId]) REFERENCES [Assets] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_Documents_FailureModes_FailureModeId] FOREIGN KEY ([FailureModeId]) REFERENCES [FailureModes] ([Id]),
    CONSTRAINT [FK_Documents_WorkOrders_WorkOrderId] FOREIGN KEY ([WorkOrderId]) REFERENCES [WorkOrders] ([Id]) ON DELETE SET NULL
);

CREATE TABLE [DocumentAccessLogs] (
    [Id] int NOT NULL IDENTITY,
    [DocumentId] int NOT NULL,
    [ActionType] nvarchar(100) NOT NULL,
    [AccessDate] datetime2 NOT NULL,
    [AccessedBy] nvarchar(200) NOT NULL,
    [UserRole] nvarchar(50) NOT NULL,
    [IpAddress] nvarchar(200) NOT NULL,
    [UserAgent] nvarchar(500) NOT NULL,
    [Notes] nvarchar(1000) NOT NULL,
    CONSTRAINT [PK_DocumentAccessLogs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DocumentAccessLogs_Documents_DocumentId] FOREIGN KEY ([DocumentId]) REFERENCES [Documents] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_DocumentAccessLogs_AccessDate] ON [DocumentAccessLogs] ([AccessDate]);

CREATE INDEX [IX_DocumentAccessLogs_DocumentId] ON [DocumentAccessLogs] ([DocumentId]);

CREATE INDEX [IX_Documents_AssetId] ON [Documents] ([AssetId]);

CREATE INDEX [IX_Documents_Category] ON [Documents] ([Category]);

CREATE INDEX [IX_Documents_CreatedDate] ON [Documents] ([CreatedDate]);

CREATE UNIQUE INDEX [IX_Documents_DocumentNumber] ON [Documents] ([DocumentNumber]);

CREATE INDEX [IX_Documents_FailureModeId] ON [Documents] ([FailureModeId]);

CREATE INDEX [IX_Documents_WorkOrderId] ON [Documents] ([WorkOrderId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251203155752_AddDocumentManagement', N'10.0.0');

COMMIT;
GO

