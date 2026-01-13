using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp1.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Note: HandOverTime and FaultTime columns were already converted to datetime2
            // These alterations are skipped if already done
            
            // Add TenantId columns only if they don't exist
            // Using raw SQL with conditional checks
            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('UserSettings') AND name = 'TenantId')
                BEGIN
                    ALTER TABLE [UserSettings] ADD [TenantId] int NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'TenantId')
                BEGIN
                    ALTER TABLE [Users] ADD [TenantId] int NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('SparePartTransactions') AND name = 'TenantId')
                BEGIN
                    ALTER TABLE [SparePartTransactions] ADD [TenantId] int NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('ReliabilityMetrics') AND name = 'TenantId')
                BEGIN
                    ALTER TABLE [ReliabilityMetrics] ADD [TenantId] int NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('NotificationSettings') AND name = 'TenantId')
                BEGIN
                    ALTER TABLE [NotificationSettings] ADD [TenantId] int NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('NotificationLogs') AND name = 'TenantId')
                BEGIN
                    ALTER TABLE [NotificationLogs] ADD [TenantId] int NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('MaintenanceTasks') AND name = 'TenantId')
                BEGIN
                    ALTER TABLE [MaintenanceTasks] ADD [TenantId] int NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('DocumentAccessLogs') AND name = 'TenantId')
                BEGIN
                    ALTER TABLE [DocumentAccessLogs] ADD [TenantId] int NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AssetDowntime') AND name = 'TenantId')
                BEGIN
                    ALTER TABLE [AssetDowntime] ADD [TenantId] int NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AssetAttachments') AND name = 'TenantId')
                BEGIN
                    ALTER TABLE [AssetAttachments] ADD [TenantId] int NULL;
                END
            ");

            // Create WhatsAppMessageLogs table only if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'WhatsAppMessageLogs')
                BEGIN
                    CREATE TABLE [WhatsAppMessageLogs] (
                        [Id] int NOT NULL IDENTITY(1,1),
                        [PhoneNumber] nvarchar(20) NOT NULL,
                        [Message] nvarchar(2000) NOT NULL,
                        [Direction] int NOT NULL,
                        [MessageType] int NOT NULL,
                        [Status] int NOT NULL,
                        [ExternalMessageId] nvarchar(100) NULL,
                        [RelatedEntityId] int NULL,
                        [RelatedEntityType] nvarchar(50) NULL,
                        [Timestamp] datetime2 NOT NULL,
                        [DeliveredAt] datetime2 NULL,
                        [ReadAt] datetime2 NULL,
                        [ErrorMessage] nvarchar(500) NULL,
                        [TenantId] int NULL,
                        CONSTRAINT [PK_WhatsAppMessageLogs] PRIMARY KEY ([Id])
                    );
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WhatsAppMessageLogs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SparePartTransactions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ReliabilityMetrics");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "NotificationSettings");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "NotificationLogs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "MaintenanceTasks");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "DocumentAccessLogs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AssetDowntime");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AssetAttachments");

            migrationBuilder.AlterColumn<string>(
                name: "HandOverTime",
                table: "WorkOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "FaultTime",
                table: "WorkOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
