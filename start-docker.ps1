# Blazor CMMS - Docker Quick Start Script
# This script sets up and runs the complete application stack with SQL Server

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "Blazor CMMS - Docker Quick Start" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Stop any existing containers
Write-Host "Stopping existing containers..." -ForegroundColor Yellow
docker-compose -f docker-compose.full.yml down

# Build the application
Write-Host ""
Write-Host "Building Docker images..." -ForegroundColor Yellow
docker-compose -f docker-compose.full.yml build

if ($LASTEXITCODE -ne 0) {
	Write-Host ""
	Write-Host "Build failed! Please check the error messages above." -ForegroundColor Red
	exit 1
}

# Start the services
Write-Host ""
Write-Host "Starting services..." -ForegroundColor Yellow
Write-Host "- SQL Server (this will take a moment to initialize)" -ForegroundColor Gray
Write-Host "- Blazor CMMS Application" -ForegroundColor Gray
Write-Host ""

docker-compose -f docker-compose.full.yml up -d

if ($LASTEXITCODE -eq 0) {
	Write-Host ""
	Write-Host "==================================" -ForegroundColor Green
	Write-Host "Application started successfully!" -ForegroundColor Green
	Write-Host "==================================" -ForegroundColor Green
	Write-Host ""
	Write-Host "Services:" -ForegroundColor Cyan
	Write-Host "  - Web App (HTTP):  http://localhost:8080" -ForegroundColor White
	Write-Host "  - Web App (HTTPS): https://localhost:8081" -ForegroundColor White
	Write-Host "  - SQL Server:      localhost:1433" -ForegroundColor White
	Write-Host ""
	Write-Host "Database Credentials:" -ForegroundColor Cyan
	Write-Host "  - Server:   localhost,1433" -ForegroundColor White
	Write-Host "  - Database: BlazorCMMS_Docker" -ForegroundColor White
	Write-Host "  - User:     sa" -ForegroundColor White
	Write-Host "  - Password: YourStrong@Passw0rd123" -ForegroundColor White
	Write-Host ""
	Write-Host "Waiting for application to start (30 seconds)..." -ForegroundColor Yellow
	Start-Sleep -Seconds 30

	Write-Host ""
	Write-Host "Application logs (last 20 lines):" -ForegroundColor Cyan
	docker-compose -f docker-compose.full.yml logs --tail=20 blazorapp1

	Write-Host ""
	Write-Host "Useful commands:" -ForegroundColor Cyan
	Write-Host "  - View logs:      docker-compose -f docker-compose.full.yml logs -f" -ForegroundColor White
	Write-Host "  - Stop services:  docker-compose -f docker-compose.full.yml down" -ForegroundColor White
	Write-Host "  - Restart:        docker-compose -f docker-compose.full.yml restart" -ForegroundColor White
	Write-Host ""
} else {
	Write-Host ""
	Write-Host "Failed to start services!" -ForegroundColor Red
	Write-Host "Check the logs with: docker-compose -f docker-compose.full.yml logs" -ForegroundColor Yellow
}
