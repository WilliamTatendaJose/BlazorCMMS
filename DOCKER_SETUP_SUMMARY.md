# Blazor CMMS - Docker Setup Complete! 🚀

Your Blazor CMMS application is now fully configured to run in Docker with all dependencies and settings properly configured.

## What's Been Configured

### ✅ Configuration Files Created/Updated

1. **appsettings.json** - Main application settings with:
   - Database connection string template
   - Email configuration (Resend)
   - WhatsApp integration settings
   - LLM configuration for AI features

2. **appsettings.Development.json** - Development-specific settings
3. **appsettings.Production.json** - Production-specific settings

### ✅ Docker Files

1. **Dockerfile** - Already existed, optimized for .NET 10.0
2. **docker-compose.yml** - Basic service definition
3. **docker-compose.override.yml** - Updated with environment variables
4. **docker-compose.prod.yml** - Production configuration with health checks
5. **docker-compose.full.yml** - Complete stack with SQL Server included

### ✅ Helper Scripts

1. **start-docker.ps1** - One-command start script
2. **stop-docker.ps1** - Stop all services script
3. **.env.example** - Environment variables template

### ✅ Documentation

1. **DOCKER_DEPLOYMENT_GUIDE.md** - Complete deployment guide

## Quick Start (Easiest Method)

### Option 1: Run with Included SQL Server

This is the easiest way to get started. It includes everything you need:

```powershell
# Run the quick start script
.\start-docker.ps1
```

This will:
- Start SQL Server in Docker
- Start the Blazor CMMS application
- Configure everything automatically
- Show you the URLs to access

**Access the application at:**
- HTTP: http://localhost:8080
- HTTPS: https://localhost:8081

### Option 2: Use Your Own Database

If you have SQL Server already running:

1. Edit `docker-compose.override.yml`
2. Update the connection string:
   ```yaml
   - ConnectionStrings__DefaultConnection=Your-Connection-String-Here
   ```
3. Run:
   ```powershell
   docker-compose up -d
   ```

## Configuration Guide

### Database Connection

The application uses SQL Server with Entity Framework Core and includes:
- Connection pooling (configured)
- Retry logic for transient failures
- Database seeding on first run

**Default credentials** (when using docker-compose.full.yml):
- Server: `localhost,1433`
- Database: `BlazorCMMS_Docker`
- User: `sa`
- Password: `YourStrong@Passw0rd123`

### Email Configuration (Optional)

The app uses Resend for sending emails. To enable:

1. Sign up at https://resend.com
2. Get your API key
3. Set environment variable in `docker-compose.override.yml`:
   ```yaml
   - Email__ResendApiKey=re_your_api_key_here
   ```

**Without an API key**: Emails will be logged to console but not sent.

### WhatsApp Integration (Optional)

To enable WhatsApp notifications:

1. Set up Meta Business App
2. Configure environment variables in `docker-compose.override.yml`
3. See `WHATSAPP_INTEGRATION_GUIDE.md` for detailed setup

## Docker Commands

```powershell
# Start everything (with SQL Server)
.\start-docker.ps1

# Or manually:
docker-compose -f docker-compose.full.yml up -d

# Stop everything
.\stop-docker.ps1

# Or manually:
docker-compose -f docker-compose.full.yml down

# View logs
docker-compose -f docker-compose.full.yml logs -f

# View only app logs
docker-compose -f docker-compose.full.yml logs -f blazorapp1

# Restart just the app
docker-compose -f docker-compose.full.yml restart blazorapp1

# Rebuild after code changes
docker-compose -f docker-compose.full.yml build
docker-compose -f docker-compose.full.yml up -d
```

## Application Features

Based on the `Program.cs` analysis, your application includes:

### Identity & Security
- ASP.NET Core Identity with custom User model
- Role-based authorization (Admin, SuperAdmin, TenantAdmin, Engineer, Planner)
- Multi-tenancy support
- Email confirmation
- Password reset functionality

### Services Configured
- ✅ Data Service (EF Core with pooling)
- ✅ Current User Service
- ✅ Role & Permission Service
- ✅ User Management Service
- ✅ Work Order Service
- ✅ Units Settings Service
- ✅ Notification Service
- ✅ Theme Service
- ✅ Data Export Service (ClosedXML, EPPlus, iText7)
- ✅ Maintenance Schedule Export Service
- ✅ Recurring Maintenance Scheduler
- ✅ WhatsApp Service with LLM integration
- ✅ Email Sender (Resend)
- ✅ Tenant Management Services

### External Integrations
- **Email**: Resend API
- **WhatsApp**: Meta Business API
- **AI/LLM**: Groq, OpenAI, Gemini, Azure OpenAI support
- **Export**: Excel (ClosedXML, EPPlus), PDF (iText7), CSV

## Troubleshooting

### Cannot connect to database

**Symptom**: App logs show "Cannot connect to database"

**Solutions**:
1. If using `docker-compose.full.yml`: Wait 30 seconds for SQL Server to initialize
2. Check SQL Server is running: `docker ps`
3. Verify connection string is correct
4. Check SQL Server logs: `docker-compose -f docker-compose.full.yml logs sqlserver`

### Port already in use

**Symptom**: "port is already allocated"

**Solution**: Change ports in docker-compose file:
```yaml
ports:
  - "8090:8080"  # Use different host port
  - "8091:8081"
```

### Application shows errors on startup

**Solution**: Check the logs:
```powershell
docker-compose -f docker-compose.full.yml logs blazorapp1
```

Common issues:
- Missing or invalid configuration
- Database not accessible
- Invalid connection string format

### Want to reset everything

```powershell
# Stop and remove everything including volumes
docker-compose -f docker-compose.full.yml down -v

# Start fresh
.\start-docker.ps1
```

## Production Deployment

For production deployment, see `DOCKER_DEPLOYMENT_GUIDE.md` which includes:
- Azure SQL Database setup
- Environment variable management
- Health checks
- Resource limits
- Security best practices
- HTTPS configuration

Quick production start:
```powershell
# Create .env file from template
Copy-Item .env.example .env
# Edit .env with your production values

# Deploy
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

## File Structure

```
BlazorCMMS/
├── BlazorApp1/
│   ├── Dockerfile                          # Docker image definition
│   ├── Program.cs                          # Application startup
│   ├── appsettings.json                    # Main configuration ✨ NEW
│   ├── appsettings.Development.json        # Dev configuration ✨ UPDATED
│   └── appsettings.Production.json         # Prod configuration ✨ UPDATED
├── docker-compose.yml                      # Base compose file
├── docker-compose.override.yml             # Dev overrides ✨ UPDATED
├── docker-compose.prod.yml                 # Production config ✨ NEW
├── docker-compose.full.yml                 # Complete stack with SQL ✨ NEW
├── start-docker.ps1                        # Quick start script ✨ NEW
├── stop-docker.ps1                         # Stop script ✨ NEW
├── .env.example                            # Environment template ✨ NEW
├── DOCKER_DEPLOYMENT_GUIDE.md              # Full deployment guide ✨ NEW
└── DOCKER_SETUP_SUMMARY.md                 # This file ✨ NEW
```

## Next Steps

1. **Test the application**:
   ```powershell
   .\start-docker.ps1
   ```
   Then open http://localhost:8080

2. **Configure your database** (if not using the included SQL Server)

3. **Set up email** (optional but recommended):
   - Get Resend API key
   - Add to environment variables

4. **Review security**:
   - Change default SQL password in production
   - Use strong passwords
   - Enable HTTPS with proper certificates

5. **Explore the documentation**:
   - `DOCKER_DEPLOYMENT_GUIDE.md` - Detailed deployment guide
   - `WHATSAPP_INTEGRATION_GUIDE.md` - WhatsApp setup
   - `NOTIFICATIONS_IMPLEMENTATION_SUMMARY.md` - Notification features

## Support

- Check logs: `docker-compose -f docker-compose.full.yml logs -f`
- Review configuration: Edit `appsettings.json` or environment variables
- Database issues: Verify connection string and SQL Server status
- Port conflicts: Change ports in docker-compose files

## Summary

✅ Your application is now fully Dockerized!
✅ All configuration files are set up with proper structure
✅ Database, Email, and WhatsApp settings are templated
✅ Easy-to-use scripts for starting and stopping
✅ Complete documentation for deployment

**To start using it right now:**
```powershell
.\start-docker.ps1
```

Then visit: http://localhost:8080

Enjoy your containerized Blazor CMMS! 🎉
