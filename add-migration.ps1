# RBM CMMS Database Migration Script

Write-Host "Creating EF Core Migration for RBM CMMS..." -ForegroundColor Green

# Navigate to the project directory
$projectPath = "BlazorApp1"

# Add migration
Write-Host "`nAdding migration 'InitialRBM_CMMS'..." -ForegroundColor Yellow
dotnet ef migrations add InitialRBM_CMMS --project $projectPath --context ApplicationDbContext

# Update database
Write-Host "`nUpdating database..." -ForegroundColor Yellow
dotnet ef database update --project $projectPath --context ApplicationDbContext

Write-Host "`nMigration completed successfully!" -ForegroundColor Green
Write-Host "`nDatabase tables created:" -ForegroundColor Cyan
Write-Host "  - Assets" -ForegroundColor White
Write-Host "  - AssetAttachments" -ForegroundColor White
Write-Host "  - AssetDowntime" -ForegroundColor White
Write-Host "  - ReliabilityMetrics" -ForegroundColor White
Write-Host "  - WorkOrders" -ForegroundColor White
Write-Host "  - MaintenanceTasks" -ForegroundColor White
Write-Host "  - MaintenanceSchedules" -ForegroundColor White
Write-Host "  - ConditionReadings" -ForegroundColor White
Write-Host "  - FailureModes" -ForegroundColor White
Write-Host "  - Users" -ForegroundColor White
