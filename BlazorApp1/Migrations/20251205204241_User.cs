using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp1.Migrations
{
    /// <inheritdoc />
    public partial class User : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreferredUnitSystem = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TemperatureUnit = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PressureUnit = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    FlowRateUnit = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    WeightUnit = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    LengthUnit = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DistanceUnit = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ThemePreference = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DateFormat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TimeFormat = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DecimalPlaces = table.Column<int>(type: "int", nullable: false),
                    EnableNotifications = table.Column<bool>(type: "bit", nullable: false),
                    NotificationFrequency = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSettings");
        }
    }
}
