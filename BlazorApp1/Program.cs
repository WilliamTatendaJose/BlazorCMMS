using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlazorApp1.Components;
using BlazorApp1.Components.Account;
using BlazorApp1.Data;
using BlazorApp1.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

// Register HttpContextAccessor for tenant context
builder.Services.AddHttpContextAccessor();

// Register RBM CMMS Services
builder.Services.AddScoped<DataService>();
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<RolePermissionService>();
builder.Services.AddScoped<UserManagementService>();
builder.Services.AddScoped<WorkOrderService>();
builder.Services.AddScoped<UnitsSettingsService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<DataExportService>();
builder.Services.AddScoped<MaintenanceScheduleExportService>();
builder.Services.AddScoped<RecurringMaintenanceScheduler>();

// Register WhatsApp Service with Meta API
builder.Services.AddHttpClient("MetaWhatsApp", client =>
{
    client.BaseAddress = new Uri("https://graph.facebook.com/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddHttpClient("LLMClient"); // For Groq, OpenAI, Gemini, Azure OpenAI
builder.Services.AddScoped<WhatsAppLLMService>();
builder.Services.AddScoped<WhatsAppService>();

// Add API Controllers for webhooks
builder.Services.AddControllers();

// Register Multi-tenancy Services
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ITenantManagementService, TenantManagementService>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Add DbContextFactory for DataService - use AddPooledDbContextFactory with retry logic
builder.Services.AddPooledDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        // Enable retry on failure for transient errors (Azure SQL)
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
        
        // Command timeout for long-running migrations
        sqlOptions.CommandTimeout(120);
    }));

// Add scoped DbContext for Identity (uses the factory)
builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    return factory.CreateDbContext();
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
        
        // Password settings (production-ready)
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
        
        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
        
        // User settings
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// Register EmailSender as scoped for proper DI with IConfiguration
builder.Services.AddScoped<IEmailSender<ApplicationUser>, EmailSender>();

// Add authorization policies
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
    .AddPolicy("SuperAdminOnly", policy => policy.RequireRole("SuperAdmin"))
    .AddPolicy("TenantAdminOrSuperAdmin", policy => policy.RequireRole("TenantAdmin", "SuperAdmin"))
    .AddPolicy("SuperAdminOrTenantAdmin", policy => policy.RequireRole("SuperAdmin", "TenantAdmin"))
    .AddPolicy("EngineerOrAdmin", policy => policy.RequireRole("Admin", "Reliability Engineer"))
    .AddPolicy("CanEdit", policy => policy.RequireRole("Admin", "Reliability Engineer", "Planner"))
    .AddPolicy("CanDelete", policy => policy.RequireRole("Admin", "Reliability Engineer"));

var app = builder.Build();

// Seed the database with timeout protection
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Starting database initialization...");
        
        // Create a cancellation token with timeout (30 seconds)
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Test database connection first
        logger.LogInformation("Testing database connection...");
        if (!await context.Database.CanConnectAsync(cts.Token))
        {
            logger.LogWarning("Cannot connect to database. Skipping seeding.");
        }
        else
        {
            logger.LogInformation("Database connected. Seeding data...");
            
            // Seed RBM CMMS data
            await DbInitializer.SeedAsync(context);
            logger.LogInformation("DbInitializer completed.");
            
            // Seed Identity roles and users
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await IdentityDataSeeder.SeedRolesAndUsersAsync(userManager, roleManager, context);
            logger.LogInformation("IdentityDataSeeder completed.");
            
            // Sync users to legacy table
            await IdentityDataSeeder.SyncAllUsersToLegacyTableAsync(userManager, context);
            logger.LogInformation("User sync completed.");
        }
    }
    catch (OperationCanceledException)
    {
        logger.LogWarning("Database seeding timed out. The app will continue without seeding.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding the database. The app will continue.");
    }
    
    logger.LogInformation("Database initialization finished.");
}

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

// Map API Controllers (for WhatsApp webhooks)
app.MapControllers();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
