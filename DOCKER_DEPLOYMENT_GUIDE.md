# Blazor CMMS - Docker Deployment Guide

This guide explains how to run the Blazor CMMS application using Docker.

## Prerequisites

- Docker Desktop installed
- SQL Server (local or Azure SQL Database)
- (Optional) Resend API key for email functionality
- (Optional) Meta WhatsApp Business API credentials

## Quick Start - Development

### 1. Configure Database Connection

Edit `docker-compose.override.yml` and update the connection string:

```yaml
- ConnectionStrings__DefaultConnection=Server=host.docker.internal;Database=BlazorCMMS_Docker;User Id=sa;Password=YourPassword;TrustServerCertificate=True;
```

**Note:** `host.docker.internal` allows the container to connect to services running on your host machine.

### 2. Build and Run

```powershell
# Build the Docker image
docker-compose build

# Start the application
docker-compose up
```

### 3. Access the Application

- HTTP: http://localhost:8080
- HTTPS: https://localhost:8081

## Production Deployment

### 1. Create Environment File

Copy the example environment file:

```powershell
Copy-Item .env.example .env
```

Edit `.env` and configure your production settings:

```bash
# Database
CONNECTION_STRING=Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=BlazorCMMS;User ID=username;Password=password;Encrypt=True;TrustServerCertificate=False;

# Email
EMAIL_RESEND_API_KEY=re_your_api_key_here
EMAIL_FROM_EMAIL=noreply@yourdomain.com
EMAIL_FROM_NAME=Your App Name

# WhatsApp (Optional)
WHATSAPP_ENABLED=true
WHATSAPP_ACCESS_TOKEN=your_meta_access_token
WHATSAPP_PHONE_NUMBER_ID=your_phone_number_id
```

### 2. Deploy with Production Configuration

```powershell
# Build
docker-compose -f docker-compose.yml -f docker-compose.prod.yml build

# Run
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

### 3. View Logs

```powershell
docker-compose logs -f blazorapp1
```

## Configuration Details

### Database Connection

The application requires SQL Server. You have several options:

#### Option 1: Local SQL Server
```
Server=host.docker.internal;Database=BlazorCMMS;User Id=sa;Password=YourPassword;TrustServerCertificate=True;
```

#### Option 2: Azure SQL Database
```
Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=BlazorCMMS;User ID=username;Password=password;Encrypt=True;TrustServerCertificate=False;
```

#### Option 3: SQL Server in Docker
```yaml
# Add to docker-compose.yml
services:
  sqlserver:
	image: mcr.microsoft.com/mssql/server:2022-latest
	environment:
	  - ACCEPT_EULA=Y
	  - SA_PASSWORD=YourStrong@Passw0rd
	ports:
	  - "1433:1433"
	volumes:
	  - sqldata:/var/opt/mssql

  blazorapp1:
	depends_on:
	  - sqlserver
	environment:
	  - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=BlazorCMMS;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;

volumes:
  sqldata:
```

### Email Configuration (Optional)

The app uses Resend for email delivery. Without an API key, emails will be logged but not sent.

1. Sign up at https://resend.com
2. Get your API key
3. Set environment variable:
   ```
   EMAIL_RESEND_API_KEY=re_your_key_here
   ```

### WhatsApp Integration (Optional)

To enable WhatsApp notifications:

1. Set up Meta Business App
2. Configure environment variables:
   - `WHATSAPP_ENABLED=true`
   - `WHATSAPP_ACCESS_TOKEN`
   - `WHATSAPP_PHONE_NUMBER_ID`
   - `WHATSAPP_BUSINESS_ACCOUNT_ID`
   - `WHATSAPP_APP_SECRET`
   - `WHATSAPP_WEBHOOK_VERIFY_TOKEN`

See `WHATSAPP_INTEGRATION_GUIDE.md` for detailed setup.

## Environment Variables Reference

| Variable | Description | Required | Default |
|----------|-------------|----------|---------|
| `ASPNETCORE_ENVIRONMENT` | Environment name (Development/Production) | No | Development |
| `ConnectionStrings__DefaultConnection` | Database connection string | Yes | - |
| `Email__ResendApiKey` | Resend API key | No | Empty (logs only) |
| `Email__FromEmail` | Sender email address | No | noreply@rbmcmms.com |
| `Email__FromName` | Sender display name | No | RBM CMMS |
| `WhatsApp__Enabled` | Enable WhatsApp integration | No | false |
| `WhatsApp__UseLLM` | Enable AI responses | No | false |
| `WhatsApp__Meta__AccessToken` | Meta API access token | If enabled | - |
| `WhatsApp__Meta__PhoneNumberId` | WhatsApp phone number ID | If enabled | - |

## Docker Commands Reference

```powershell
# Build the image
docker-compose build

# Start in foreground (see logs)
docker-compose up

# Start in background
docker-compose up -d

# Stop containers
docker-compose down

# View logs
docker-compose logs -f

# Rebuild and restart
docker-compose up --build

# Remove all containers and volumes
docker-compose down -v
```

## Troubleshooting

### Cannot connect to database

**Error:** `Cannot open database "BlazorCMMS" requested by the login`

**Solution:** 
1. Ensure SQL Server is running
2. Create the database manually or let EF migrations create it
3. Check connection string and credentials

### Port already in use

**Error:** `Bind for 0.0.0.0:8080 failed: port is already allocated`

**Solution:** Change the port in `docker-compose.override.yml`:
```yaml
ports:
  - "8090:8080"  # Use port 8090 on host
```

### Application exits immediately

**Solution:** Check logs:
```powershell
docker-compose logs blazorapp1
```

Common issues:
- Invalid connection string
- Database not accessible
- Missing configuration

## Health Check

The production configuration includes a health check endpoint. Check container health:

```powershell
docker ps
```

Look for "healthy" status in the STATUS column.

## Updating the Application

```powershell
# Pull latest code
git pull

# Rebuild and restart
docker-compose down
docker-compose build --no-cache
docker-compose up -d
```

## Security Best Practices

1. **Never commit `.env` file** - It's in `.gitignore`
2. **Use strong passwords** for database
3. **Use secrets management** in production (Azure Key Vault, Docker Secrets)
4. **Enable HTTPS** with proper certificates
5. **Limit port exposure** - Only expose necessary ports

## Performance Tuning

For production, consider:

1. **Resource limits** in docker-compose:
```yaml
services:
  blazorapp1:
	deploy:
	  resources:
		limits:
		  cpus: '2'
		  memory: 2G
		reservations:
		  cpus: '1'
		  memory: 1G
```

2. **Connection pooling** - Already configured in `Program.cs`
3. **Database retry logic** - Already configured for transient failures

## Support

For issues or questions:
- Check logs: `docker-compose logs -f`
- Review configuration in `appsettings.json`
- Consult application documentation
