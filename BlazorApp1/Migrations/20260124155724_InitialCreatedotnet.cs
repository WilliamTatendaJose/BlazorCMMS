using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp1.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreatedotnet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AssetType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EquipmentManufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Criticality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InstallationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModelNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HealthScore = table.Column<double>(type: "float", nullable: false),
                    Uptime = table.Column<double>(type: "float", nullable: false),
                    Downtime = table.Column<double>(type: "float", nullable: false),
                    LastMaintenance = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextScheduledMaintenance = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsRetired = table.Column<bool>(type: "bit", nullable: false),
                    RetirementDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ManufactureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NotificationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RecipientAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalMessageId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RelatedAssetId = table.Column<int>(type: "int", nullable: true),
                    RelatedWorkOrderId = table.Column<int>(type: "int", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MaxUsers = table.Column<int>(type: "int", nullable: false),
                    MaxAssets = table.Column<int>(type: "int", nullable: false),
                    MaxDocuments = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    AspNetUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

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
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetAttachments_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConditionReadings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    ReadingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Temperature = table.Column<double>(type: "float", nullable: true),
                    Vibration = table.Column<double>(type: "float", nullable: true),
                    Pressure = table.Column<double>(type: "float", nullable: true),
                    OilAnalysis = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Current = table.Column<double>(type: "float", nullable: true),
                    Voltage = table.Column<double>(type: "float", nullable: true),
                    NoiseLevel = table.Column<double>(type: "float", nullable: true),
                    FlowRate = table.Column<double>(type: "float", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RecordedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OverallStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AlertGenerated = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConditionReadings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConditionReadings_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FailureModes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    Mode = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Cause = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Effect = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    Occurrence = table.Column<int>(type: "int", nullable: false),
                    Detection = table.Column<int>(type: "int", nullable: false),
                    CurrentControls = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RecommendedActions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ResponsiblePerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TargetCompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActionsTaken = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    RevisedSeverity = table.Column<int>(type: "int", nullable: true),
                    RevisedOccurrence = table.Column<int>(type: "int", nullable: true),
                    RevisedDetection = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FailureModes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FailureModes_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    AssetName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssignedTechnician = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    EstimatedDuration = table.Column<double>(type: "float", nullable: false),
                    ActualDuration = table.Column<double>(type: "float", nullable: true),
                    Frequency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NextScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletionNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceSchedules_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReliabilityMetrics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    MetricDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MTBF = table.Column<double>(type: "float", nullable: false),
                    MTTR = table.Column<double>(type: "float", nullable: false),
                    MTTF = table.Column<double>(type: "float", nullable: false),
                    Availability = table.Column<double>(type: "float", nullable: false),
                    Reliability = table.Column<double>(type: "float", nullable: false),
                    OEE = table.Column<double>(type: "float", nullable: false),
                    FailureCount = table.Column<int>(type: "int", nullable: false),
                    TotalDowntimeHours = table.Column<double>(type: "float", nullable: false),
                    TotalUptimeHours = table.Column<double>(type: "float", nullable: false),
                    Period = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CalculatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReliabilityMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReliabilityMetrics_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpareParts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ManufacturerPartNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Supplier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    QuantityInStock = table.Column<int>(type: "int", nullable: false),
                    MinimumStockLevel = table.Column<int>(type: "int", nullable: false),
                    ReorderPoint = table.Column<int>(type: "int", nullable: false),
                    ReorderQuantity = table.Column<int>(type: "int", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsGeneric = table.Column<bool>(type: "bit", nullable: false),
                    AssetId = table.Column<int>(type: "int", nullable: true),
                    CompatibleAssets = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Specifications = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LastRestockDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUsedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpareParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpareParts_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "WorkOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkOrderId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    RequestedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AssignedTo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TimeSubmitted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TimeDone = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TimeCompleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstimatedCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EstimatedDowntime = table.Column<int>(type: "int", nullable: false),
                    ActualDowntime = table.Column<double>(type: "float", nullable: false),
                    LaborHours = table.Column<double>(type: "float", nullable: true),
                    CompletionNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DetailsOfWorkCarriedOut = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CorrectiveAction = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AnyOtherDetails = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RejectedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RejectedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsAcknowledged = table.Column<bool>(type: "bit", nullable: false),
                    AcknowledgedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AcknowledgedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Building = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Floor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SubCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsMechanical = table.Column<bool>(type: "bit", nullable: false),
                    IsElectrical = table.Column<bool>(type: "bit", nullable: false),
                    JobType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ScheduledStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScheduledEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequiresShutdown = table.Column<bool>(type: "bit", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LockOutRequired = table.Column<bool>(type: "bit", nullable: false),
                    RequiresSafetyPermit = table.Column<bool>(type: "bit", nullable: false),
                    SafetyPermitNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ArtisanName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PartsUsed = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    JobNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HousekeepingAffected = table.Column<bool>(type: "bit", nullable: false),
                    HousekeepingNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RequestReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Originator = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FaultDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FaultTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    RecurrencePattern = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RecurrenceInterval = table.Column<int>(type: "int", nullable: false),
                    ParentWorkOrderId = table.Column<int>(type: "int", nullable: true),
                    ArtisanSignature = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DetailsOfRequest = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    OriginatorVerification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OriginatorVerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EngineeringForemanVerification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EngineeringForemanVerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HODVerification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HODVerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HandOverTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BreakdownType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkOrders_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PrimaryTenantId = table.Column<int>(type: "int", nullable: true),
                    IsSuperAdmin = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Tenants_PrimaryTenantId",
                        column: x => x.PrimaryTenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AssetDowntime",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    RelatedWorkOrderId = table.Column<int>(type: "int", nullable: true),
                    ProductionLoss = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FinancialImpact = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RecordedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RecordedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetDowntime", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetDowntime_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetDowntime_WorkOrders_RelatedWorkOrderId",
                        column: x => x.RelatedWorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SubCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RevisionNumber = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsConfidential = table.Column<bool>(type: "bit", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Author = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccessLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AllowedRoles = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    DownloadCount = table.Column<int>(type: "int", nullable: false),
                    AssetId = table.Column<int>(type: "int", nullable: true),
                    WorkOrderId = table.Column<int>(type: "int", nullable: true),
                    FailureModeId = table.Column<int>(type: "int", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Documents_FailureModes_FailureModeId",
                        column: x => x.FailureModeId,
                        principalTable: "FailureModes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Documents_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkOrderId = table.Column<int>(type: "int", nullable: true),
                    ScheduleId = table.Column<int>(type: "int", nullable: true),
                    TaskName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    EstimatedDuration = table.Column<double>(type: "float", nullable: false),
                    ActualDuration = table.Column<double>(type: "float", nullable: true),
                    AssignedTo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ToolsRequired = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    PartsRequired = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SafetyLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SafetyPrecautions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CompletionNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceTasks_MaintenanceSchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "MaintenanceSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenanceTasks_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SparePartTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SparePartId = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    StockBefore = table.Column<int>(type: "int", nullable: false),
                    StockAfter = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssuedTo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WorkOrderId = table.Column<int>(type: "int", nullable: true),
                    AssetId = table.Column<int>(type: "int", nullable: true),
                    UnitCostAtTransaction = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TransactionBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SparePartTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SparePartTransactions_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SparePartTransactions_SpareParts_SparePartId",
                        column: x => x.SparePartId,
                        principalTable: "SpareParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SparePartTransactions_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

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

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserPasskeys",
                columns: table => new
                {
                    CredentialId = table.Column<byte[]>(type: "varbinary(1024)", maxLength: 1024, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserPasskeys", x => x.CredentialId);
                    table.ForeignKey(
                        name: "FK_AspNetUserPasskeys_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmailCriticalAlerts = table.Column<bool>(type: "bit", nullable: false),
                    EmailWorkOrderDue = table.Column<bool>(type: "bit", nullable: false),
                    EmailMaintenanceSchedule = table.Column<bool>(type: "bit", nullable: false),
                    EmailWeeklyReport = table.Column<bool>(type: "bit", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SmsCriticalOnly = table.Column<bool>(type: "bit", nullable: false),
                    EnableEmailNotifications = table.Column<bool>(type: "bit", nullable: false),
                    EnableSmsNotifications = table.Column<bool>(type: "bit", nullable: false),
                    QuietHoursStart = table.Column<TimeOnly>(type: "time", nullable: true),
                    QuietHoursEnd = table.Column<TimeOnly>(type: "time", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationSettings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTenantMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsTenantAdmin = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RemovedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTenantMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTenantMappings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTenantMappings_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentAccessLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccessDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccessedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UserRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentAccessLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentAccessLogs_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserPasskeys_UserId",
                table: "AspNetUserPasskeys",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PrimaryTenantId",
                table: "AspNetUsers",
                column: "PrimaryTenantId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAttachments_AssetId",
                table: "AssetAttachments",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDowntime_AssetId",
                table: "AssetDowntime",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDowntime_RelatedWorkOrderId",
                table: "AssetDowntime",
                column: "RelatedWorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDowntime_StartTime",
                table: "AssetDowntime",
                column: "StartTime");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetId",
                table: "Assets",
                column: "AssetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConditionReadings_AssetId",
                table: "ConditionReadings",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_ConditionReadings_ReadingDate",
                table: "ConditionReadings",
                column: "ReadingDate");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAccessLogs_AccessDate",
                table: "DocumentAccessLogs",
                column: "AccessDate");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAccessLogs_DocumentId",
                table: "DocumentAccessLogs",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_AssetId",
                table: "Documents",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Category",
                table: "Documents",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CreatedDate",
                table: "Documents",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DocumentNumber",
                table: "Documents",
                column: "DocumentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_FailureModeId",
                table: "Documents",
                column: "FailureModeId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_WorkOrderId",
                table: "Documents",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_FailureModes_AssetId",
                table: "FailureModes",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceSchedules_AssetId",
                table: "MaintenanceSchedules",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceTasks_ScheduleId",
                table: "MaintenanceTasks",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceTasks_WorkOrderId",
                table: "MaintenanceTasks",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_CreatedDate",
                table: "NotificationLogs",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_UserId_CreatedDate",
                table: "NotificationLogs",
                columns: new[] { "UserId", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSettings_UserId",
                table: "NotificationSettings",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReliabilityMetrics_AssetId",
                table: "ReliabilityMetrics",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_ReliabilityMetrics_MetricDate",
                table: "ReliabilityMetrics",
                column: "MetricDate");

            migrationBuilder.CreateIndex(
                name: "IX_SpareParts_AssetId",
                table: "SpareParts",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_SpareParts_PartNumber",
                table: "SpareParts",
                column: "PartNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SparePartTransactions_AssetId",
                table: "SparePartTransactions",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_SparePartTransactions_SparePartId",
                table: "SparePartTransactions",
                column: "SparePartId");

            migrationBuilder.CreateIndex(
                name: "IX_SparePartTransactions_TransactionDate",
                table: "SparePartTransactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_SparePartTransactions_WorkOrderId",
                table: "SparePartTransactions",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTenantMappings_TenantId_UserId",
                table: "UserTenantMappings",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserTenantMappings_UserId",
                table: "UserTenantMappings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_AssetId",
                table: "WorkOrders",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_WorkOrderId",
                table: "WorkOrders",
                column: "WorkOrderId",
                unique: true);

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
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserPasskeys");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AssetAttachments");

            migrationBuilder.DropTable(
                name: "AssetDowntime");

            migrationBuilder.DropTable(
                name: "ConditionReadings");

            migrationBuilder.DropTable(
                name: "DocumentAccessLogs");

            migrationBuilder.DropTable(
                name: "MaintenanceTasks");

            migrationBuilder.DropTable(
                name: "NotificationLogs");

            migrationBuilder.DropTable(
                name: "NotificationSettings");

            migrationBuilder.DropTable(
                name: "ReliabilityMetrics");

            migrationBuilder.DropTable(
                name: "SparePartTransactions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "UserTenantMappings");

            migrationBuilder.DropTable(
                name: "WhatsAppMessageLogs");

            migrationBuilder.DropTable(
                name: "WorkOrderSparesUsed");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "MaintenanceSchedules");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "SpareParts");

            migrationBuilder.DropTable(
                name: "FailureModes");

            migrationBuilder.DropTable(
                name: "WorkOrders");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropTable(
                name: "Assets");
        }
    }
}
