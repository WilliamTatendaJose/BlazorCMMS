using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp1.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantIdToAllEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add TenantId to Users table
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Users",
                type: "int",
                nullable: true);

            // Add TenantId to AssetDowntime table
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "AssetDowntime",
                type: "int",
                nullable: true);

            // Add TenantId to ReliabilityMetrics table
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "ReliabilityMetrics",
                type: "int",
                nullable: true);

            // Add TenantId to MaintenanceTasks table
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "MaintenanceTasks",
                type: "int",
                nullable: true);

            // Add TenantId to AssetAttachments table
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "AssetAttachments",
                type: "int",
                nullable: true);

            // Add TenantId to SparePartTransactions table
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "SparePartTransactions",
                type: "int",
                nullable: true);

            // Add TenantId to DocumentAccessLogs table
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "DocumentAccessLogs",
                type: "int",
                nullable: true);

            // Add TenantId to NotificationSettings table
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "NotificationSettings",
                type: "int",
                nullable: true);

            // Add TenantId to NotificationLogs table
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "NotificationLogs",
                type: "int",
                nullable: true);

            // Add TenantId to UserSettings table
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "UserSettings",
                type: "int",
                nullable: true);

            // Create indexes for better query performance
            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDowntime_TenantId",
                table: "AssetDowntime",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ReliabilityMetrics_TenantId",
                table: "ReliabilityMetrics",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceTasks_TenantId",
                table: "MaintenanceTasks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAttachments_TenantId",
                table: "AssetAttachments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SparePartTransactions_TenantId",
                table: "SparePartTransactions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAccessLogs_TenantId",
                table: "DocumentAccessLogs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSettings_TenantId",
                table: "NotificationSettings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_TenantId",
                table: "NotificationLogs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_TenantId",
                table: "UserSettings",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop indexes first
            migrationBuilder.DropIndex(name: "IX_Users_TenantId", table: "Users");
            migrationBuilder.DropIndex(name: "IX_AssetDowntime_TenantId", table: "AssetDowntime");
            migrationBuilder.DropIndex(name: "IX_ReliabilityMetrics_TenantId", table: "ReliabilityMetrics");
            migrationBuilder.DropIndex(name: "IX_MaintenanceTasks_TenantId", table: "MaintenanceTasks");
            migrationBuilder.DropIndex(name: "IX_AssetAttachments_TenantId", table: "AssetAttachments");
            migrationBuilder.DropIndex(name: "IX_SparePartTransactions_TenantId", table: "SparePartTransactions");
            migrationBuilder.DropIndex(name: "IX_DocumentAccessLogs_TenantId", table: "DocumentAccessLogs");
            migrationBuilder.DropIndex(name: "IX_NotificationSettings_TenantId", table: "NotificationSettings");
            migrationBuilder.DropIndex(name: "IX_NotificationLogs_TenantId", table: "NotificationLogs");
            migrationBuilder.DropIndex(name: "IX_UserSettings_TenantId", table: "UserSettings");

            // Drop columns
            migrationBuilder.DropColumn(name: "TenantId", table: "Users");
            migrationBuilder.DropColumn(name: "TenantId", table: "AssetDowntime");
            migrationBuilder.DropColumn(name: "TenantId", table: "ReliabilityMetrics");
            migrationBuilder.DropColumn(name: "TenantId", table: "MaintenanceTasks");
            migrationBuilder.DropColumn(name: "TenantId", table: "AssetAttachments");
            migrationBuilder.DropColumn(name: "TenantId", table: "SparePartTransactions");
            migrationBuilder.DropColumn(name: "TenantId", table: "DocumentAccessLogs");
            migrationBuilder.DropColumn(name: "TenantId", table: "NotificationSettings");
            migrationBuilder.DropColumn(name: "TenantId", table: "NotificationLogs");
            migrationBuilder.DropColumn(name: "TenantId", table: "UserSettings");
        }
    }
}
