# Railway Deployment Configuration
# BlazorCMMS - Complete Railway Deployment Guide

## Prerequisites

1. **Database**: You need SQL Server database. Options:
   - Azure SQL Database (recommended)
   - AWS RDS SQL Server
   - Any hosted SQL Server instance accessible from Railway

2. **Railway Account**: Sign up at https://railway.app

## Step-by-Step Deployment

### Step 1: Prepare Your Database

1. Create a SQL Server database (e.g., on Azure SQL)
2. Note down your connection string
3. Run migrations locally first (recommended):
   ```powershell
   cd BlazorApp1
   dotnet ef database update --connection "YourConnectionString"
   ```

### Step 2: Push Your Code to GitHub

Make sure all the Railway deployment files are committed:
```bash
git add Dockerfile railway.toml docker-entrypoint.sh .railwayignore RAILWAY_DEPLOYMENT.md
git commit -m "Add Railway deployment configuration"
git push origin master
```

### Step 3: Create Railway Project

1. Go to https://railway.app/new
2. Select "Deploy from GitHub repo"
3. Choose your `BlazorCMMS` repository
4. Railway will automatically detect the Dockerfile

### Step 4: Configure Environment Variables

Add these in Railway Project → Variables:

#### Required Variables

```
ConnectionStrings__DefaultConnection
```
Your SQL Server connection string. Format:
```
Server=tcp:your-server.database.windows.net,1433;Initial Catalog=BlazorCMMS;User ID=your-username;Password=your-password;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

```
ASPNETCORE_ENVIRONMENT
```
Set to: `Production`

#### Optional Email Configuration (Resend)

```
Email__ResendApiKey=your-resend-api-key
Email__FromEmail=noreply@yourdomain.com
Email__FromName=RBM CMMS
```

#### Optional WhatsApp Configuration

```
WhatsApp__Enabled=false
WhatsApp__Meta__AccessToken=your-token
WhatsApp__Meta__PhoneNumberId=your-phone-id
WhatsApp__Meta__BusinessAccountId=your-account-id
WhatsApp__Meta__AppSecret=your-secret
WhatsApp__Meta__WebhookVerifyToken=your-verify-token
```

### Step 5: Deploy

1. Railway will automatically start building
2. First build takes 5-10 minutes
3. Watch the build logs in Railway dashboard
4. Once deployed, Railway provides a public URL

### Step 6: Database Migrations (Important!)

Your app automatically runs database seeding on startup. However:

**If database is empty:**
- The app will attempt to seed initial data
- Check logs to verify seeding completed successfully

**If you need to run migrations manually:**
```powershell
# Connect to Railway shell (if needed)
# Or run migrations locally pointing to production DB
dotnet ef database update --connection "ProductionConnectionString"
```

## Configuration Details

### Port Configuration
- Railway automatically assigns a `PORT` environment variable
- The `docker-entrypoint.sh` script handles this dynamically
- No manual port configuration needed

### SSL/HTTPS
- Railway provides automatic HTTPS for your custom domain
- The app uses HTTP internally (Railway handles SSL termination)
- `app.UseHttpsRedirection()` is included for production

### Database Connection
- Connection string format uses double underscore: `ConnectionStrings__DefaultConnection`
- Enable retry on failure (already configured in Program.cs)
- Connection timeout set to 120 seconds for migrations

## Important Notes

1. **Build Time**: First build may take 5-10 minutes due to:
   - .NET 10 SDK download
   - NuGet package restore
   - Multi-stage Docker build

2. **Database Access**: Ensure your SQL Server firewall allows Railway's IP addresses
   - For Azure SQL: Add Railway's outbound IPs to firewall rules
   - Or allow all Azure services (less secure)

3. **Environment Variables**: Use double underscore `__` for nested configuration
   - Example: `Email__ResendApiKey` maps to `Email:ResendApiKey` in appsettings.json

4. **Startup Seeding**: The app seeds data on first run:
   - Creates default roles (Admin, TenantAdmin, etc.)
   - Creates default SuperAdmin user
   - Seeds initial RBM CMMS data
   - Check logs to verify completion

## Troubleshooting

### Build Fails

**Error: "Railpack could not determine how to build"**
- ✅ **FIXED**: Dockerfile and railway.toml added to root directory

**Error: "SDK not found"**
- Verify .NET 10 is available
- Check Dockerfile uses correct base images

### Connection Errors

**"Cannot connect to database"**
- Verify connection string is correct
- Check SQL Server firewall rules allow Railway IPs
- Test connection string locally first
- Ensure database exists

**Connection string format:**
```
Server=tcp:HOST,1433;Initial Catalog=DATABASE;User ID=USERNAME;Password=PASSWORD;Encrypt=True;Connection Timeout=30;
```

### App Doesn't Start

**Check Railway Logs:**
1. Go to Railway project
2. Click "Deployments"
3. Select latest deployment
4. View "Deploy Logs" and "Runtime Logs"

**Common issues:**
- Missing `ConnectionStrings__DefaultConnection` variable
- Invalid connection string format
- Database not accessible
- Missing database migrations

### Seeding Fails

If database seeding times out:
- Increase timeout in Program.cs (currently 30 seconds)
- Or run seeding separately
- Check logs: "Database seeding timed out"

### Performance Issues

- First request may be slow (cold start)
- Railway free tier has CPU limits
- Consider upgrading to Railway Pro for production

## Railway CLI (Optional)

Install Railway CLI for easier management:
```bash
npm i -g @railway/cli
railway login
railway link # Link to your project
railway logs # View logs
railway shell # Access container shell
```

## Monitoring

**View Logs:**
- Railway Dashboard → Your Project → Deployments → Logs

**Metrics:**
- Railway provides CPU, Memory, and Network metrics
- Available in project dashboard

**Health Checks:**
- Access your app URL to verify it's running
- Check `/health` endpoint if implemented

## Updating Your App

After making code changes:
```bash
git add .
git commit -m "Your changes"
git push origin master
```

Railway automatically:
1. Detects the push
2. Rebuilds the Docker image
3. Deploys the new version
4. Zero-downtime deployment

## Cost Considerations

**Railway Pricing:**
- Free tier: $5 credit/month (good for testing)
- Pay as you go: Based on usage
- Hobby plan: $5/month + resource usage

**Database Costs:**
- Azure SQL: Starts at ~$5/month (Basic tier)
- Consider serverless SQL for dev/test

## Security Checklist

- ✅ Use environment variables for secrets (not appsettings.json)
- ✅ Enable HTTPS (Railway handles this)
- ✅ Use strong database passwords
- ✅ Restrict SQL Server firewall to Railway IPs only
- ✅ Set `ASPNETCORE_ENVIRONMENT=Production`
- ✅ Review and rotate API keys regularly

## Custom Domain (Optional)

1. Go to Railway project settings
2. Click "Custom Domain"
3. Add your domain (e.g., cmms.yourdomain.com)
4. Update DNS records as instructed
5. Railway automatically provisions SSL certificate

## Files Added for Railway

- `/Dockerfile` - Production-ready multi-stage Dockerfile
- `/railway.toml` - Railway configuration
- `/docker-entrypoint.sh` - Startup script for dynamic port binding
- `/.railwayignore` - Build optimization (excludes unnecessary files)
- `/RAILWAY_DEPLOYMENT.md` - This comprehensive guide

## Support

**Railway Documentation:** https://docs.railway.app  
**Railway Discord:** https://discord.gg/railway  
**Railway Status:** https://status.railway.app

**BlazorCMMS Issues:** Check your repository issues or documentation files

## Quick Reference Commands

```bash
# View logs
railway logs

# Access shell (if needed)
railway shell

# List environment variables
railway variables

# Set an environment variable
railway variables set KEY=value

# Restart deployment
railway up --detach
```

## Next Steps After Deployment

1. ✅ Verify app is running at Railway URL
2. ✅ Check logs for any errors
3. ✅ Test database connection
4. ✅ Verify seeding completed
5. ✅ Test login with SuperAdmin account
6. ✅ Configure custom domain (optional)
7. ✅ Set up monitoring/alerts
8. ✅ Configure email service (Resend)
9. ✅ Set up backups for database

## Default SuperAdmin Credentials

After first deployment and seeding, use:
- **Email**: Check your IdentityDataSeeder.cs for default email
- **Password**: Check your IdentityDataSeeder.cs for default password

⚠️ **IMPORTANT**: Change these immediately after first login!

---

**Last Updated**: January 2024  
**Railway CLI Version**: 3.x  
**.NET Version**: 10.0
