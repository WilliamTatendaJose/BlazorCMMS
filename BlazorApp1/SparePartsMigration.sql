BEGIN TRANSACTION;
CREATE TABLE [SpareParts] (
    [Id] int NOT NULL IDENTITY,
    [PartNumber] nvarchar(50) NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(1000) NOT NULL,
    [Category] nvarchar(100) NOT NULL,
    [Manufacturer] nvarchar(100) NOT NULL,
    [ManufacturerPartNumber] nvarchar(100) NOT NULL,
    [Supplier] nvarchar(100) NOT NULL,
    [QuantityInStock] int NOT NULL,
    [MinimumStockLevel] int NOT NULL,
    [ReorderPoint] int NOT NULL,
    [ReorderQuantity] int NOT NULL,
    [UnitCost] decimal(18,2) NOT NULL,
    [Unit] nvarchar(50) NOT NULL,
    [Location] nvarchar(200) NOT NULL,
    [IsGeneric] bit NOT NULL,
    [AssetId] int NULL,
    [CompatibleAssets] nvarchar(500) NOT NULL,
    [Specifications] nvarchar(500) NOT NULL,
    [LastRestockDate] datetime2 NULL,
    [LastUsedDate] datetime2 NULL,
    [Status] nvarchar(50) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [ModifiedDate] datetime2 NULL,
    [CreatedBy] nvarchar(200) NOT NULL,
    [Notes] nvarchar(1000) NOT NULL,
    CONSTRAINT [PK_SpareParts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SpareParts_Assets_AssetId] FOREIGN KEY ([AssetId]) REFERENCES [Assets] ([Id]) ON DELETE SET NULL
);

CREATE TABLE [SparePartTransactions] (
    [Id] int NOT NULL IDENTITY,
    [SparePartId] int NOT NULL,
    [TransactionType] nvarchar(50) NOT NULL,
    [Quantity] int NOT NULL,
    [StockBefore] int NOT NULL,
    [StockAfter] int NOT NULL,
    [TransactionDate] datetime2 NOT NULL,
    [IssuedTo] nvarchar(200) NOT NULL,
    [WorkOrderId] int NULL,
    [AssetId] int NULL,
    [UnitCostAtTransaction] decimal(18,2) NULL,
    [TotalCost] decimal(18,2) NOT NULL,
    [TransactionBy] nvarchar(200) NOT NULL,
    [Reason] nvarchar(1000) NOT NULL,
    [Notes] nvarchar(1000) NOT NULL,
    [ReferenceNumber] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_SparePartTransactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SparePartTransactions_Assets_AssetId] FOREIGN KEY ([AssetId]) REFERENCES [Assets] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_SparePartTransactions_SpareParts_SparePartId] FOREIGN KEY ([SparePartId]) REFERENCES [SpareParts] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_SparePartTransactions_WorkOrders_WorkOrderId] FOREIGN KEY ([WorkOrderId]) REFERENCES [WorkOrders] ([Id]) ON DELETE SET NULL
);

CREATE INDEX [IX_SpareParts_AssetId] ON [SpareParts] ([AssetId]);

CREATE UNIQUE INDEX [IX_SpareParts_PartNumber] ON [SpareParts] ([PartNumber]);

CREATE INDEX [IX_SparePartTransactions_AssetId] ON [SparePartTransactions] ([AssetId]);

CREATE INDEX [IX_SparePartTransactions_SparePartId] ON [SparePartTransactions] ([SparePartId]);

CREATE INDEX [IX_SparePartTransactions_TransactionDate] ON [SparePartTransactions] ([TransactionDate]);

CREATE INDEX [IX_SparePartTransactions_WorkOrderId] ON [SparePartTransactions] ([WorkOrderId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251203141755_AddSparePartsManagement', N'10.0.0');

COMMIT;
GO

