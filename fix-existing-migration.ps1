# Alternative: Keep Database, Just Fix Migration

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Fixing Migration (Preserving Database)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "This script will:" -ForegroundColor Yellow
Write-Host "  1. Remove the failed migration" -ForegroundColor White
Write-Host "  2. Create a baseline migration" -ForegroundColor White
Write-Host "  3. Mark it as already applied" -ForegroundColor White
Write-Host ""

# Step 1: Remove the last migration
Write-Host "Step 1: Removing failed migration..." -ForegroundColor Yellow
dotnet ef migrations remove --project BlazorApp1 --context ApplicationDbContext --force

Write-Host ""

# Step 2: List what's currently in the database
Write-Host "Step 2: Checking database state..." -ForegroundColor Yellow
dotnet ef migrations list --project BlazorApp1 --context ApplicationDbContext

Write-Host ""

# Step 3: Create a migration that just adds RBM tables
Write-Host "Step 3: Creating migration for RBM tables only..." -ForegroundColor Yellow
dotnet ef migrations add AddRBMCMMSTables --project BlazorApp1 --context ApplicationDbContext

Write-Host ""
Write-Host "========================================" -ForegroundColor Yellow
Write-Host "IMPORTANT: Manual Step Required" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host ""
Write-Host "Before running 'dotnet ef database update', you need to:" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Open the migration file in:" -ForegroundColor White
Write-Host "   BlazorApp1\Migrations\*_AddRBMCMMSTables.cs" -ForegroundColor Cyan
Write-Host ""
Write-Host "2. Remove any CreateTable statements for:" -ForegroundColor White
Write-Host "   - AspNetRoles" -ForegroundColor Red
Write-Host "   - AspNetUsers" -ForegroundColor Red
Write-Host "   - AspNetUserRoles" -ForegroundColor Red
Write-Host "   - AspNetUserClaims" -ForegroundColor Red
Write-Host "   - AspNetUserLogins" -ForegroundColor Red
Write-Host "   - AspNetUserTokens" -ForegroundColor Red
Write-Host "   - AspNetRoleClaims" -ForegroundColor Red
Write-Host ""
Write-Host "3. Keep only the RBM CMMS tables:" -ForegroundColor White
Write-Host "   - Assets" -ForegroundColor Green
Write-Host "   - AssetAttachments" -ForegroundColor Green
Write-Host "   - AssetDowntime" -ForegroundColor Green
Write-Host "   - ReliabilityMetrics" -ForegroundColor Green
Write-Host "   - WorkOrders" -ForegroundColor Green
Write-Host "   - MaintenanceTasks" -ForegroundColor Green
Write-Host "   - MaintenanceSchedules" -ForegroundColor Green
Write-Host "   - ConditionReadings" -ForegroundColor Green
Write-Host "   - FailureModes" -ForegroundColor Green
Write-Host "   - Users" -ForegroundColor Green
Write-Host ""
Write-Host "4. Then run:" -ForegroundColor White
Write-Host "   dotnet ef database update --project BlazorApp1" -ForegroundColor Cyan
Write-Host ""
Write-Host "OR if you prefer the clean approach, run:" -ForegroundColor Yellow
Write-Host "   .\clean-migration.ps1" -ForegroundColor Cyan
Write-Host ""
