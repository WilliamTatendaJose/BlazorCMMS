using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp1.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenancy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Note: Tenants table and AspNetUsers columns were already created by migration 20251209091112_Tenants
            // This migration is now a no-op but kept for migration history
            // It originally attempted to create tables that already exist
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // This migration is a no-op, nothing to undo
        }
    }
}
