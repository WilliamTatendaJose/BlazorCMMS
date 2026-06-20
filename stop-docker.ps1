# Blazor CMMS - Docker Stop Script
# This script stops all Docker containers

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "Blazor CMMS - Stopping Docker" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Stopping services..." -ForegroundColor Yellow
docker-compose -f docker-compose.full.yml down

Write-Host ""
Write-Host "Services stopped successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Note: Database data is preserved in Docker volumes." -ForegroundColor Gray
Write-Host "To remove all data, run: docker-compose -f docker-compose.full.yml down -v" -ForegroundColor Gray
Write-Host ""
