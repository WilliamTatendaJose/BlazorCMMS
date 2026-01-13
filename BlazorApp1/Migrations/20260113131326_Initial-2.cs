using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp1.Migrations
{
    /// <inheritdoc />
    public partial class Initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add IsActive to TenantUserMappings/UserTenantMappings only if it doesn't exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'TenantUserMappings')
                AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TenantUserMappings') AND name = 'IsActive')
                BEGIN
                    ALTER TABLE [TenantUserMappings] ADD [IsActive] bit NOT NULL DEFAULT 1;
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'UserTenantMappings')
                AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('UserTenantMappings') AND name = 'IsActive')
                BEGIN
                    ALTER TABLE [UserTenantMappings] ADD [IsActive] bit NOT NULL DEFAULT 1;
                END
            ");

            // Add AspNetUserId to Users only if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'AspNetUserId')
                BEGIN
                    ALTER TABLE [Users] ADD [AspNetUserId] nvarchar(450) NULL;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('UserTenantMappings') AND name = 'IsActive')
                BEGIN
                    ALTER TABLE [UserTenantMappings] DROP COLUMN [IsActive];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TenantUserMappings') AND name = 'IsActive')
                BEGIN
                    ALTER TABLE [TenantUserMappings] DROP COLUMN [IsActive];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'AspNetUserId')
                BEGIN
                    ALTER TABLE [Users] DROP COLUMN [AspNetUserId];
                END
            ");
        }
    }
}
