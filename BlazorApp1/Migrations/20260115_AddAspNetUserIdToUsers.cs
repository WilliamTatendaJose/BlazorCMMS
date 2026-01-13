using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp1.Migrations
{
    /// <inheritdoc />
    public partial class AddAspNetUserIdToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add AspNetUserId column to Users table to link with Identity
            migrationBuilder.AddColumn<string>(
                name: "AspNetUserId",
                table: "Users",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            // Create index for faster lookups
            migrationBuilder.CreateIndex(
                name: "IX_Users_AspNetUserId",
                table: "Users",
                column: "AspNetUserId");

            // Create index on Email for sync operations
            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_AspNetUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AspNetUserId",
                table: "Users");
        }
    }
}
