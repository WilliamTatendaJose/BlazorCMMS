# Railway Quick Deploy Checklist

## ✅ Pre-Deployment
- [ ] SQL Server database created (Azure SQL recommended)
- [ ] Connection string ready
- [ ] Railway account created
- [ ] GitHub repo up to date with deployment files

## ✅ Railway Setup (5 minutes)

### 1. Create Project
- Go to https://railway.app/new
- Select "Deploy from GitHub repo"
- Choose your repository
- Railway auto-detects Dockerfile ✓

### 2. Add Environment Variables (REQUIRED)

**Copy and paste these in Railway → Variables:**

```env
ConnectionStrings__DefaultConnection=Server=tcp:YOUR_SERVER.database.windows.net,1433;Initial Catalog=BlazorCMMS;User ID=YOUR_USER;Password=YOUR_PASSWORD;Encrypt=True;Connection Timeout=30;

ASPNETCORE_ENVIRONMENT=Production
```

**Optional but recommended:**
```env
Email__ResendApiKey=YOUR_RESEND_KEY
Email__FromEmail=noreply@yourdomain.com
Email__FromName=RBM CMMS
```

### 3. Deploy
- Click "Deploy"
- Wait 5-10 minutes for initial build
- Get your Railway URL

### 4. Verify
- [ ] Open Railway URL in browser
- [ ] Check logs: Railway → Deployments → View Logs
- [ ] Look for "Database initialization finished"
- [ ] Test login (default SuperAdmin credentials in IdentityDataSeeder.cs)

## 🔥 Troubleshooting

**Build fails:**
- Check Dockerfile is in root ✓
- Check railway.toml exists ✓

**App crashes:**
- Verify `ConnectionStrings__DefaultConnection` is set
- Check database firewall allows Railway IPs
- View logs: Railway → Deployments → Runtime Logs

**Database connection fails:**
- Test connection string locally first
- Azure SQL: Add Railway IPs to firewall (or allow Azure services)
- Verify database exists

**Seeding fails:**
- Check logs for "Database seeding timed out"
- Database might be slow to connect (retry after 30-60 seconds)

## 📋 Key Files

| File | Purpose |
|------|---------|
| `/Dockerfile` | Multi-stage build for Railway |
| `/railway.toml` | Railway configuration |
| `/docker-entrypoint.sh` | Dynamic port binding |
| `/.railwayignore` | Excludes unnecessary files from build |

## 🚀 Post-Deployment

1. **Change default password** for SuperAdmin
2. Configure custom domain (optional)
3. Set up database backups
4. Configure email service (Resend)
5. Monitor logs and performance

## 💡 Common Commands (Railway CLI)

```bash
# Install Railway CLI
npm i -g @railway/cli

# Login and link
railway login
railway link

# Useful commands
railway logs              # View logs
railway variables         # List env vars
railway variables set KEY=value  # Set variable
railway up --detach      # Redeploy
```

## 📊 Environment Variables Reference

### Required
- `ConnectionStrings__DefaultConnection` - SQL Server connection string
- `ASPNETCORE_ENVIRONMENT` - Set to `Production`

### Optional - Email (Resend)
- `Email__ResendApiKey`
- `Email__FromEmail`
- `Email__FromName`

### Optional - WhatsApp
- `WhatsApp__Enabled` (true/false)
- `WhatsApp__Meta__AccessToken`
- `WhatsApp__Meta__PhoneNumberId`
- `WhatsApp__Meta__BusinessAccountId`
- `WhatsApp__Meta__AppSecret`
- `WhatsApp__Meta__WebhookVerifyToken`

### Optional - WhatsApp LLM
- `WhatsApp__LLM__Provider` (Groq, OpenAI, Gemini, AzureOpenAI)
- `WhatsApp__LLM__GroqApiKey`
- `WhatsApp__LLM__OpenAIApiKey`
- `WhatsApp__LLM__GeminiApiKey`

## 🎯 Expected Build Time
- First build: **5-10 minutes**
- Subsequent builds: **3-5 minutes**

## 💰 Cost Estimate
- Railway: $5 credit/month (free tier) or ~$1-5/month (small app)
- Azure SQL Basic: ~$5/month
- **Total**: ~$6-10/month for dev/test

## 🔗 Useful Links
- Railway Dashboard: https://railway.app/dashboard
- Railway Docs: https://docs.railway.app
- Azure SQL: https://portal.azure.com

---

**Need help?** Check RAILWAY_DEPLOYMENT.md for detailed guide.
