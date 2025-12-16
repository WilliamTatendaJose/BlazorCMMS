using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp1.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantIdToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add TenantId to Assets table
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Assets",
                type: "int",
                nullable: true);

            // Add TenantId to WorkOrders table
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "WorkOrders",
                type: "int",
                nullable: true);

            // Add TenantId to Documents table
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Documents",
                type: "int",
                nullable: true);

            // Add TenantId to SpareParts table
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "SpareParts",
                type: "int",
                nullable: true);

            // Add TenantId to FailureModes table
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "FailureModes",
                type: "int",
                nullable: true);

            // Add TenantId to MaintenanceSchedules table
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "MaintenanceSchedules",
                type: "int",
                nullable: true);

            // Add TenantId to ConditionReadings table
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "ConditionReadings",
                type: "int",
                nullable: true);

            // Create indexes for better query performance
            migrationBuilder.CreateIndex(
                name: "IX_Assets_TenantId",
                table: "Assets",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_TenantId",
                table: "WorkOrders",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_TenantId",
                table: "Documents",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SpareParts_TenantId",
                table: "SpareParts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FailureModes_TenantId",
                table: "FailureModes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceSchedules_TenantId",
                table: "MaintenanceSchedules",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ConditionReadings_TenantId",
                table: "ConditionReadings",
                column: "TenantId");

            // Add foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Tenants_TenantId",
                table: "Assets",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Tenants_TenantId",
                table: "WorkOrders",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Tenants_TenantId",
                table: "Documents",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SpareParts_Tenants_TenantId",
                table: "SpareParts",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_FailureModes_Tenants_TenantId",
                table: "FailureModes",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceSchedules_Tenants_TenantId",
                table: "MaintenanceSchedules",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ConditionReadings_Tenants_TenantId",
                table: "ConditionReadings",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign keys
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Tenants_TenantId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Tenants_TenantId",
                table: "WorkOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Tenants_TenantId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_SpareParts_Tenants_TenantId",
                table: "SpareParts");

            migrationBuilder.DropForeignKey(
                name: "FK_FailureModes_Tenants_TenantId",
                table: "FailureModes");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceSchedules_Tenants_TenantId",
                table: "MaintenanceSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_ConditionReadings_Tenants_TenantId",
                table: "ConditionReadings");

            // Drop indexes
            migrationBuilder.DropIndex(
                name: "IX_Assets_TenantId",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_TenantId",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_Documents_TenantId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_SpareParts_TenantId",
                table: "SpareParts");

            migrationBuilder.DropIndex(
                name: "IX_FailureModes_TenantId",
                table: "FailureModes");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceSchedules_TenantId",
                table: "MaintenanceSchedules");

            migrationBuilder.DropIndex(
                name: "IX_ConditionReadings_TenantId",
                table: "ConditionReadings");

            // Drop columns
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SpareParts");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "FailureModes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "MaintenanceSchedules");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ConditionReadings");
        }
    }
}
