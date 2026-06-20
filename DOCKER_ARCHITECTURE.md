# Blazor CMMS - Docker Architecture

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                         Docker Host                              │
│                                                                  │
│  ┌────────────────────────────────────────────────────────┐    │
│  │          blazorcmms-network (Bridge Network)            │    │
│  │                                                          │    │
│  │  ┌─────────────────────┐      ┌──────────────────────┐ │    │
│  │  │   SQL Server        │      │   Blazor CMMS App    │ │    │
│  │  │   Container         │◄─────┤   Container          │ │    │
│  │  │                     │      │                      │ │    │
│  │  │ Port: 1433          │      │ Ports: 8080, 8081    │ │    │
│  │  │ Image: mssql-2022   │      │ Image: blazorapp1    │ │    │
│  │  │                     │      │                      │ │    │
│  │  │ Volume:             │      │ .NET 10.0 Runtime    │ │    │
│  │  │ blazorcmms-sqldata  │      │                      │ │    │
│  │  └─────────────────────┘      └──────────────────────┘ │    │
│  │           │                              │               │    │
│  └───────────┼──────────────────────────────┼───────────────┘    │
│              │                              │                    │
│              ▼                              ▼                    │
│     ┌─────────────────┐           ┌─────────────────┐          │
│     │   Docker Volume  │           │   Published     │          │
│     │   (Persistent    │           │   Ports to      │          │
│     │    Database)     │           │   Host          │          │
│     └─────────────────┘           └─────────────────┘          │
└─────────────────────────────────────────────────────────────────┘
											│
											▼
								   ┌─────────────────┐
								   │  Host Machine   │
								   │                 │
								   │  localhost:8080 │
								   │  localhost:8081 │
								   │  localhost:1433 │
								   └─────────────────┘
```

## Component Details

### SQL Server Container
- **Image**: `mcr.microsoft.com/mssql/server:2022-latest`
- **Container Name**: `blazorcmms-sqlserver`
- **Internal Port**: 1433 (accessible to app container)
- **External Port**: 1433 (accessible from host)
- **Volume**: `blazorcmms-sqldata` (persistent storage)
- **Health Check**: Runs every 10s to ensure database is ready
- **Environment**:
  - `ACCEPT_EULA=Y`
  - `SA_PASSWORD=YourStrong@Passw0rd123`
  - `MSSQL_PID=Developer`

### Blazor CMMS App Container
- **Image**: `blazorapp1` (built from Dockerfile)
- **Container Name**: `blazorcmms-app`
- **Base Image**: `mcr.microsoft.com/dotnet/aspnet:10.0`
- **Build Image**: `mcr.microsoft.com/dotnet/sdk:10.0`
- **Internal Ports**: 8080 (HTTP), 8081 (HTTPS)
- **External Ports**: 8080 (HTTP), 8081 (HTTPS)
- **Depends On**: SQL Server (waits for healthy status)
- **Environment Variables**:
  - Connection string to SQL Server
  - Application configuration
  - Email settings
  - WhatsApp settings

## Data Flow

```
User Browser
	│
	│ HTTP/HTTPS Request
	│
	▼
Host Machine (localhost:8080/8081)
	│
	│ Docker Port Mapping
	│
	▼
Blazor App Container (Port 8080/8081)
	│
	├── ASP.NET Core Pipeline
	│   ├── Authentication/Authorization
	│   ├── Razor Components
	│   └── API Controllers
	│
	├── Services Layer
	│   ├── Data Service (EF Core)
	│   ├── Work Order Service
	│   ├── User Management
	│   ├── Email Service (Resend API)
	│   └── WhatsApp Service (Meta API)
	│
	│ Database Queries
	│
	▼
SQL Server Container (Port 1433)
	│
	└── Database: BlazorCMMS_Docker
		├── Tables (Identity, Work Orders, Assets, etc.)
		└── Persistent Storage (Docker Volume)
```

## External Integrations

```
Blazor App Container
	│
	├─► Resend API (Email)
	│   └── https://api.resend.com
	│
	├─► Meta WhatsApp API
	│   └── https://graph.facebook.com
	│
	└─► LLM APIs (Optional)
		├── Groq API
		├── OpenAI API
		├── Gemini API
		└── Azure OpenAI
```

## Network Communication

### Internal Container Communication
- **Network**: `blazorcmms-network` (Bridge driver)
- **DNS Resolution**: Containers resolve each other by service name
- **Example**: App connects to SQL using `Server=sqlserver`

### External Access
- **App HTTP**: `localhost:8080` → `blazorapp1:8080`
- **App HTTPS**: `localhost:8081` → `blazorapp1:8081`
- **SQL Server**: `localhost:1433` → `sqlserver:1433`

## Configuration Injection

```
docker-compose.yml
	│
	├── Environment Variables
	│   ├── ASPNETCORE_ENVIRONMENT
	│   ├── ConnectionStrings__DefaultConnection
	│   ├── Email__*
	│   └── WhatsApp__*
	│
	└─► Override appsettings.json at runtime
		│
		└─► Application Configuration
			├── Program.cs reads configuration
			├── Services use IConfiguration
			└── Connection strings, API keys applied
```

## Volume Management

```
Host File System
	│
	└── Docker Volumes
		│
		└── blazorcmms-sqldata
			│
			├── /var/opt/mssql/data  (Database files)
			├── /var/opt/mssql/log   (Transaction logs)
			└── /var/opt/mssql/backup (Backups)

Persistence: Data survives container restarts
Removal: Only deleted with 'docker-compose down -v'
```

## Build Process

```
1. Build Stage (Multi-stage Dockerfile)
   ├── FROM mcr.microsoft.com/dotnet/sdk:10.0
   ├── COPY project files
   ├── dotnet restore
   ├── COPY source code
   ├── dotnet build
   └── dotnet publish

2. Final Stage
   ├── FROM mcr.microsoft.com/dotnet/aspnet:10.0
   ├── COPY published files from build stage
   ├── EXPOSE ports 8080, 8081
   └── ENTRYPOINT ["dotnet", "BlazorApp1.dll"]

3. Docker Compose
   ├── Build image: docker-compose build
   └── Create containers: docker-compose up
```

## Deployment Scenarios

### Development (docker-compose.full.yml)
- Includes SQL Server container
- Development configuration
- Port 8080/8081 for app
- Port 1433 for SQL Server
- Logs set to Information level

### Production (docker-compose.prod.yml)
- Uses external database (Azure SQL)
- Production configuration
- Port 80/443 (standard HTTP/HTTPS)
- Health checks enabled
- Logs set to Warning level
- Auto-restart enabled

### Development without SQL Server (docker-compose.yml + override)
- Uses external SQL Server
- Configurable connection string
- Development settings
- Local development workflow

## Comparison: Local vs Docker

| Aspect | Local Development | Docker Development |
|--------|------------------|-------------------|
| **Database** | Local installation required | Containerized SQL Server |
| **Configuration** | appsettings.json | Environment variables |
| **Portability** | Machine-specific | Works anywhere with Docker |
| **Isolation** | Shares host resources | Isolated containers |
| **Clean Up** | Manual uninstall | `docker-compose down` |
| **Consistency** | Varies by machine | Identical across environments |
| **Startup** | Direct .NET run | Container orchestration |
| **Debugging** | Full VS debugging | Logs + attach debugger |

## Security Layers

```
┌─────────────────────────────────────────┐
│           Security Layers                │
├─────────────────────────────────────────┤
│ 1. Docker Network Isolation             │
│    ├─ Private network between containers│
│    └─ Only exposed ports accessible     │
├─────────────────────────────────────────┤
│ 2. ASP.NET Core Security                │
│    ├─ HTTPS enforcement                 │
│    ├─ Anti-forgery tokens               │
│    └─ Authentication/Authorization      │
├─────────────────────────────────────────┤
│ 3. Database Security                    │
│    ├─ SQL Server authentication         │
│    ├─ Encrypted connections             │
│    └─ Role-based access                 │
├─────────────────────────────────────────┤
│ 4. Secrets Management                   │
│    ├─ Environment variables (Docker)    │
│    ├─ User Secrets (Development)        │
│    └─ Azure Key Vault (Production)      │
└─────────────────────────────────────────┘
```

## Scaling Options

### Horizontal Scaling (Multiple App Containers)
```yaml
services:
  blazorapp1:
	deploy:
	  replicas: 3  # Run 3 instances
	load_balancer:
	  # Add load balancer configuration
```

### Vertical Scaling (Resource Limits)
```yaml
services:
  blazorapp1:
	deploy:
	  resources:
		limits:
		  cpus: '2'
		  memory: 2G
```

## Monitoring

```
Container Metrics
	├── docker stats
	├── docker ps
	└── docker logs

Application Logs
	├── Console output
	├── ASP.NET Core logging
	└── Structured logging

Health Checks
	├── Container health status
	├── Database connectivity
	└── Application endpoints
```

This architecture provides:
✅ Complete isolation
✅ Easy deployment
✅ Consistent environments
✅ Scalability
✅ Production-ready setup
