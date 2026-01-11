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

// Register WhatsApp Service
builder.Services.AddHttpClient("TwilioWhatsApp");
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

// Add DbContextFactory for DataService - use AddPooledDbContextFactory
builder.Services.AddPooledDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

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

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Seed RBM CMMS data
        var context = services.GetRequiredService<ApplicationDbContext>();
        await DbInitializer.SeedAsync(context);
        
        // Seed Identity roles and users
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await IdentityDataSeeder.SeedRolesAndUsersAsync(userManager, roleManager);
        
        // Seed SuperAdmin role if not exists
        if (!await roleManager.RoleExistsAsync("SuperAdmin"))
        {
            await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
        }
        
        // Seed TenantAdmin role if not exists
        if (!await roleManager.RoleExistsAsync("TenantAdmin"))
        {
            await roleManager.CreateAsync(new IdentityRole("TenantAdmin"));
        }
        
        // Seed Viewer role if not exists
        if (!await roleManager.RoleExistsAsync("Viewer"))
        {
            await roleManager.CreateAsync(new IdentityRole("Viewer"));
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
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
