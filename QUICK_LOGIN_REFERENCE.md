# 🔐 Quick Login Reference - Blazor CMMS

## Application URL
**http://localhost:8080**

---

## User Accounts (All passwords: Test123!)

### 🔴 SuperAdmin (Highest Access)
```
Email:    superadmin@company.com
Password: Test123!
```

### 🟠 Admin
```
Email:    admin@company.com
Password: Test123!
```

### 🟡 Reliability Engineer
```
Email:    sarah.johnson@company.com
Password: Test123!
```

### 🟢 Planner
```
Email:    emily.brown@company.com
Password: Test123!
```

### 🔵 Supervisor
```
Email:    david.wilson@company.com
Password: Test123!
```

### 🟣 Technician #1
```
Email:    john.smith@company.com
Password: Test123!
```

### 🟣 Technician #2
```
Email:    mike.davis@company.com
Password: Test123!
```

### ⚪ Viewer (Read-Only)
```
Email:    viewer@company.com
Password: Test123!
```

---

## Database Access

### Via Docker
```bash
docker exec -it blazorcmms-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd123' -d BlazorCMMS_Docker -C
```

### Connection String
```
Server: localhost,1433
Database: BlazorCMMS_Docker
User: sa
Password: YourStrong@Passw0rd123
```

---

## Quick Commands

```bash
# View logs
docker logs blazorcmms-app -f

# Restart app
docker restart blazorcmms-app

# Stop all
docker-compose -f docker-compose.full.yml down

# Start all
docker-compose -f docker-compose.full.yml up -d
```

---

⚠️ **Remember to change default passwords after first login!**
