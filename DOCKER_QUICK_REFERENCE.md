# 🚀 Blazor CMMS Docker - Quick Reference

## One-Command Start
```powershell
.\start-docker.ps1
```
**Access**: http://localhost:8080

## Common Commands

### Starting Services
```powershell
# With SQL Server included (recommended for testing)
docker-compose -f docker-compose.full.yml up -d

# Development mode
docker-compose up -d

# Production mode
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

### Stopping Services
```powershell
# Stop all
docker-compose down

# Stop and remove volumes (deletes data!)
docker-compose down -v

# Quick stop script
.\stop-docker.ps1
```

### Viewing Logs
```powershell
# All services
docker-compose logs -f

# App only
docker-compose logs -f blazorapp1

# SQL Server only
docker-compose -f docker-compose.full.yml logs -f sqlserver

# Last 50 lines
docker-compose logs --tail=50
```

### Rebuilding
```powershell
# Rebuild after code changes
docker-compose build --no-cache
docker-compose up -d

# Force rebuild
docker-compose up --build
```

### Container Management
```powershell
# List running containers
docker ps

# List all containers
docker ps -a

# Restart app container
docker-compose restart blazorapp1

# Shell into container
docker exec -it blazorcmms-app /bin/bash
```

## URLs

| Service | URL |
|---------|-----|
| **Application (HTTP)** | http://localhost:8080 |
| **Application (HTTPS)** | https://localhost:8081 |
| **SQL Server** | localhost:1433 |

## Default Credentials

### SQL Server (when using docker-compose.full.yml)
- **Server**: localhost,1433
- **Database**: BlazorCMMS_Docker
- **User**: sa
- **Password**: YourStrong@Passw0rd123

## Configuration Files

| File | Purpose |
|------|---------|
| `appsettings.json` | Main app configuration |
| `appsettings.Development.json` | Dev-specific settings |
| `appsettings.Production.json` | Production settings |
| `docker-compose.yml` | Base Docker services |
| `docker-compose.override.yml` | Dev environment overrides |
| `docker-compose.prod.yml` | Production configuration |
| `docker-compose.full.yml` | Complete stack with SQL Server |
| `.env` | Environment variables (create from .env.example) |

## Environment Variables (Key Ones)

```bash
# Database
ConnectionStrings__DefaultConnection=Server=...

# Email (Optional)
Email__ResendApiKey=re_your_key
Email__FromEmail=noreply@yourdomain.com

# WhatsApp (Optional)
WhatsApp__Enabled=true
WhatsApp__Meta__AccessToken=your_token
```

## Troubleshooting

### App won't start
```powershell
# Check logs
docker-compose logs blazorapp1

# Common fix: Rebuild
docker-compose build --no-cache
docker-compose up -d
```

### Database connection error
```powershell
# Verify SQL Server is running
docker ps | findstr sqlserver

# Check SQL logs
docker-compose -f docker-compose.full.yml logs sqlserver

# Restart SQL Server
docker-compose restart sqlserver
```

### Port conflict
Edit `docker-compose.override.yml`:
```yaml
ports:
  - "8090:8080"  # Change 8080 to 8090
```

### Container keeps restarting
```powershell
# See what's failing
docker-compose logs --tail=100 blazorapp1

# Check container status
docker ps -a
```

## Data Management

### Backup Database
```powershell
# Backup while running
docker exec blazorcmms-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd123' -Q "BACKUP DATABASE [BlazorCMMS_Docker] TO DISK = '/var/opt/mssql/backup/BlazorCMMS.bak'" -C
```

### Reset Everything
```powershell
# Stop and remove all (including data)
docker-compose -f docker-compose.full.yml down -v

# Start fresh
.\start-docker.ps1
```

### Keep Data, Rebuild App
```powershell
# Stop services (keeps volumes)
docker-compose down

# Rebuild app
docker-compose build

# Start again
docker-compose up -d
```

## Health Checks

```powershell
# Check if containers are healthy
docker ps

# Look for "healthy" in STATUS column
# Example: Up 2 minutes (healthy)
```

## Performance

### View Resource Usage
```powershell
docker stats
```

### Set Resource Limits
Edit docker-compose file:
```yaml
deploy:
  resources:
	limits:
	  cpus: '2'
	  memory: 2G
```

## Quick Fixes

| Problem | Solution |
|---------|----------|
| **Port 8080 in use** | Change port in docker-compose.override.yml |
| **Can't connect to DB** | Wait 30s for SQL Server startup, check logs |
| **Old code running** | `docker-compose build --no-cache` |
| **Container exits immediately** | `docker-compose logs blazorapp1` |
| **Out of disk space** | `docker system prune -a` |

## Development Workflow

### Make Code Changes
```powershell
# 1. Edit code
# 2. Rebuild
docker-compose build

# 3. Restart
docker-compose up -d

# 4. View logs
docker-compose logs -f blazorapp1
```

### Update Configuration
```powershell
# Edit docker-compose.override.yml or .env
# Then restart
docker-compose down
docker-compose up -d
```

## Production Checklist

- [ ] Create `.env` from `.env.example`
- [ ] Set strong SA_PASSWORD
- [ ] Configure Azure SQL connection string
- [ ] Add Resend API key (if using email)
- [ ] Add WhatsApp credentials (if using WhatsApp)
- [ ] Enable HTTPS with valid certificates
- [ ] Set ASPNETCORE_ENVIRONMENT=Production
- [ ] Configure health checks
- [ ] Set up monitoring/logging
- [ ] Test disaster recovery

## Files You Can Safely Modify

✅ `docker-compose.override.yml` - Dev environment settings
✅ `.env` - Environment variables (create from .env.example)
✅ `appsettings.json` - App configuration templates
✅ Port mappings in compose files

❌ `Dockerfile` - Only if you know what you're doing
❌ `docker-compose.yml` - Base configuration

## Getting Help

1. **Check logs first**: `docker-compose logs -f`
2. **Review configuration**: Check appsettings.json and .env
3. **Verify services**: `docker ps`
4. **Read docs**: 
   - `DOCKER_SETUP_SUMMARY.md` - Overview
   - `DOCKER_DEPLOYMENT_GUIDE.md` - Detailed guide
   - `DOCKER_ARCHITECTURE.md` - Architecture diagrams

## Pro Tips

💡 Use `-d` flag to run in background (detached mode)
💡 Use `-f` with logs to follow in real-time
💡 Add `--tail=N` to limit log output
💡 Use `docker system prune` to clean up unused resources
💡 Create `.env` for sensitive configuration
💡 Check health status in `docker ps` output

---

**Quick Start**: `.\start-docker.ps1`
**Access App**: http://localhost:8080
**Stop All**: `.\stop-docker.ps1`
