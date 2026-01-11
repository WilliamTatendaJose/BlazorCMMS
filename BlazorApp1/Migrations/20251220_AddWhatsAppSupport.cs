using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp1.Migrations;

/// <summary>
/// Migration to add WhatsApp communication support
/// </summary>
public partial class AddWhatsAppSupport : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "WhatsAppMessageLogs",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                Direction = table.Column<int>(type: "int", nullable: false),
                MessageType = table.Column<int>(type: "int", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                ExternalMessageId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                RelatedEntityId = table.Column<int>(type: "int", nullable: true),
                RelatedEntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                ErrorMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                TenantId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WhatsAppMessageLogs", x => x.Id);
            });

        // Create indexes for performance
        migrationBuilder.CreateIndex(
            name: "IX_WhatsAppMessageLogs_PhoneNumber",
            table: "WhatsAppMessageLogs",
            column: "PhoneNumber");

        migrationBuilder.CreateIndex(
            name: "IX_WhatsAppMessageLogs_Timestamp",
            table: "WhatsAppMessageLogs",
            column: "Timestamp");

        migrationBuilder.CreateIndex(
            name: "IX_WhatsAppMessageLogs_ExternalMessageId",
            table: "WhatsAppMessageLogs",
            column: "ExternalMessageId");

        migrationBuilder.CreateIndex(
            name: "IX_WhatsAppMessageLogs_TenantId",
            table: "WhatsAppMessageLogs",
            column: "TenantId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "WhatsAppMessageLogs");
    }
}
