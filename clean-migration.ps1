# Clean Migration Script - Fix Database Conflict

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Cleaning Migrations and Database" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Remove all existing migrations
Write-Host "Step 1: Removing all migrations..." -ForegroundColor Yellow
Remove-Item -Path "BlazorApp1\Migrations\*" -Recurse -Force -ErrorAction SilentlyContinue
Write-Host "Migrations removed." -ForegroundColor Green
Write-Host ""

# Step 2: Drop the database
Write-Host "Step 2: Dropping database..." -ForegroundColor Yellow
dotnet ef database drop --project BlazorApp1 --context ApplicationDbContext --force

if ($LASTEXITCODE -ne 0) {
    Write-Host "Warning: Database drop failed or database didn't exist." -ForegroundColor Yellow
}
Write-Host ""

# Step 3: Create fresh migration
Write-Host "Step 3: Creating fresh migration..." -ForegroundColor Yellow
dotnet ef migrations add InitialCreate --project BlazorApp1 --context ApplicationDbContext

if ($LASTEXITCODE -ne 0) {
    Write-Host "Migration creation failed!" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Step 4: Create database and apply migration
Write-Host "Step 4: Creating database and applying migration..." -ForegroundColor Yellow
dotnet ef database update --project BlazorApp1 --context ApplicationDbContext

if ($LASTEXITCODE -ne 0) {
    Write-Host "Database update failed!" -ForegroundColor Red
    exit 1
}
Write-Host ""

Write-Host "========================================" -ForegroundColor Green
Write-Host "Database Setup Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

Write-Host "Next step: Run the application" -ForegroundColor Cyan
Write-Host "  dotnet run --project BlazorApp1" -ForegroundColor White
Write-Host ""
Write-Host "The application will automatically seed:" -ForegroundColor Cyan
Write-Host "  - Identity roles and users" -ForegroundColor White
Write-Host "  - RBM CMMS sample data" -ForegroundColor White
Write-Host ""
