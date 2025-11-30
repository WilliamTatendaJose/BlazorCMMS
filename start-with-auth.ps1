# Quick Start Script for RBM CMMS with Authentication

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "RBM CMMS - Quick Start with Auth" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Add migration for ApplicationUser changes
Write-Host "Step 1: Creating migration for ApplicationUser..." -ForegroundColor Yellow
dotnet ef migrations add ExtendApplicationUser --project BlazorApp1 --context ApplicationDbContext

if ($LASTEXITCODE -ne 0) {
    Write-Host "Migration creation failed. This might be okay if migration already exists." -ForegroundColor Yellow
}

Write-Host ""

# Step 2: Update database
Write-Host "Step 2: Updating database..." -ForegroundColor Yellow
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

Write-Host "Default Users Created:" -ForegroundColor Cyan
Write-Host "  Admin:    admin@company.com / Admin123!" -ForegroundColor White
Write-Host "  Engineer: sarah.johnson@company.com / Sarah123!" -ForegroundColor White
Write-Host "  Planner:  emily.brown@company.com / Emily123!" -ForegroundColor White
Write-Host "  Tech 1:   john.smith@company.com / John123!" -ForegroundColor White
Write-Host "  Tech 2:   mike.davis@company.com / Mike123!" -ForegroundColor White
Write-Host ""

Write-Host "Roles Created:" -ForegroundColor Cyan
Write-Host "  • Admin" -ForegroundColor White
Write-Host "  • Reliability Engineer" -ForegroundColor White
Write-Host "  • Planner" -ForegroundColor White
Write-Host "  • Technician" -ForegroundColor White
Write-Host ""

# Step 3: Run the application
Write-Host "Step 3: Starting application..." -ForegroundColor Yellow
Write-Host ""
Write-Host "Navigate to: https://localhost:7xxx" -ForegroundColor Green
Write-Host "Login page: /Account/Login" -ForegroundColor Green
Write-Host "RBM Dashboard: /rbm" -ForegroundColor Green
Write-Host ""

dotnet run --project BlazorApp1/BlazorApp1.csproj
