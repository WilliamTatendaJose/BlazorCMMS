using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp1.Migrations
{
    /// <inheritdoc />
    public partial class worksOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AcknowledgedBy",
                table: "WorkOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "AcknowledgedDate",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnyOtherDetails",
                table: "WorkOrders",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApprovalNotes",
                table: "WorkOrders",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "WorkOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArtisanName",
                table: "WorkOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ArtisanSignature",
                table: "WorkOrders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BreakdownType",
                table: "WorkOrders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Building",
                table: "WorkOrders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "WorkOrders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "WorkOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "WorkOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "WorkOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CorrectiveAction",
                table: "WorkOrders",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "WorkOrders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DetailsOfRequest",
                table: "WorkOrders",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DetailsOfWorkCarriedOut",
                table: "WorkOrders",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EngineeringForemanVerification",
                table: "WorkOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EngineeringForemanVerificationDate",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FaultDate",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FaultTime",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Floor",
                table: "WorkOrders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HODVerification",
                table: "WorkOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "HODVerificationDate",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HandOverTime",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HousekeepingAffected",
                table: "WorkOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "HousekeepingNotes",
                table: "WorkOrders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsAcknowledged",
                table: "WorkOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsElectrical",
                table: "WorkOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMechanical",
                table: "WorkOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecurring",
                table: "WorkOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "JobNumber",
                table: "WorkOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JobType",
                table: "WorkOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "WorkOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "WorkOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "LockOutRequired",
                table: "WorkOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Originator",
                table: "WorkOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginatorVerification",
                table: "WorkOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "OriginatorVerificationDate",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentWorkOrderId",
                table: "WorkOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecurrenceInterval",
                table: "WorkOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecurrencePattern",
                table: "WorkOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RejectedBy",
                table: "WorkOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedDate",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "WorkOrders",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestReason",
                table: "WorkOrders",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestedBy",
                table: "WorkOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedDate",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresSafetyPermit",
                table: "WorkOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresShutdown",
                table: "WorkOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SafetyPermitNumber",
                table: "WorkOrders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledEndDate",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledStartDate",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubCategory",
                table: "WorkOrders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeCompleted",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeDone",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeSubmitted",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkOrderSparesUsed",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkOrderId = table.Column<int>(type: "int", nullable: false),
                    RequisitionNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ItemDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    SparePartId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderSparesUsed", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkOrderSparesUsed_SpareParts_SparePartId",
                        column: x => x.SparePartId,
                        principalTable: "SpareParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WorkOrderSparesUsed_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderSparesUsed_SparePartId",
                table: "WorkOrderSparesUsed",
                column: "SparePartId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderSparesUsed_WorkOrderId",
                table: "WorkOrderSparesUsed",
                column: "WorkOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkOrderSparesUsed");

            migrationBuilder.DropColumn(
                name: "AcknowledgedBy",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "AcknowledgedDate",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "AnyOtherDetails",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ApprovalNotes",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ArtisanName",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ArtisanSignature",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "BreakdownType",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "Building",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "CorrectiveAction",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "DetailsOfRequest",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "DetailsOfWorkCarriedOut",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "EngineeringForemanVerification",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "EngineeringForemanVerificationDate",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "FaultDate",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "FaultTime",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "Floor",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "HODVerification",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "HODVerificationDate",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "HandOverTime",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "HousekeepingAffected",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "HousekeepingNotes",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "IsAcknowledged",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "IsElectrical",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "IsMechanical",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "IsRecurring",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "JobNumber",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "JobType",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "LockOutRequired",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "Originator",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "OriginatorVerification",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "OriginatorVerificationDate",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ParentWorkOrderId",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "RecurrenceInterval",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "RecurrencePattern",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "RejectedBy",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "RejectedDate",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "RequestReason",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "RequestedBy",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "RequestedDate",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "RequiresSafetyPermit",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "RequiresShutdown",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "SafetyPermitNumber",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ScheduledEndDate",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ScheduledStartDate",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "SubCategory",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "TimeCompleted",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "TimeDone",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "TimeSubmitted",
                table: "WorkOrders");
        }
    }
}
