# Multi-Tenancy Architecture Documentation

## System Overview

This document outlines the technical architecture of the multi-tenancy implementation in your Blazor CMMS application.

## Architecture Diagram

```
???????????????????????????????????????????????????????????????????
?                     User Interface Layer                         ?
???????????????????????????????????????????????????????????????????
?  Tenants.razor              TenantUsers.razor                    ?
?  (/rbm/tenants)             (/rbm/tenant-users/{id})             ?
?  [SuperAdmin Only]          [SuperAdmin, TenantAdmin]            ?
????????????????????????????????????????????????????????????????????
               ?                                       ?
???????????????????????????????????????????????????????????????????
?                    Services Layer                                ?
?????????????????????????????????????????????????????????????????????
?  ITenantService              ITenantManagementService             ?
?  ?? GetTenantContextAsync    ?? CreateTenantAsync                ?
?  ?? IsUserInTenantAsync      ?? GetTenantByIdAsync               ?
?  ?? GetUserTenantsAsync      ?? UpdateTenantAsync                ?
?  ?? SetTenantContext         ?? DeleteTenantAsync                ?
?                              ?? AddUserToTenantAsync             ?
?                              ?? RemoveUserFromTenantAsync        ?
?                              ?? SetUserAsAdminAsync              ?
????????????????????????????????????????????????????????????????????
               ?                                       ?
????????????????????????????????????????????????????????????????????
?                   Entity Models Layer                             ?
?????????????????????????????????????????????????????????????????????
?  Tenant (Parent Entity)                                           ?
?  ?? Id, TenantCode, Name, Description                            ?
?  ?? Contact Information                                          ?
?  ?? Limits (MaxUsers, MaxAssets, MaxDocuments)                   ?
?  ?? Status (Active/Inactive/Suspended/Archived)                  ?
?  ?? Navigation: Users (ApplicationUser)                          ?
?                                                                   ?
?  TenantUserMapping (Junction Entity)                             ?
?  ?? TenantId (FK)                                                ?
?  ?? UserId (FK)                                                  ?
?  ?? IsTenantAdmin                                                ?
?  ?? AssignedDate, RemovedDate (Soft Delete)                      ?
?  ?? Navigations: Tenant, User                                    ?
?                                                                   ?
?  ApplicationUser (Extended)                                      ?
?  ?? PrimaryTenantId (FK, Nullable)                               ?
?  ?? IsSuperAdmin                                                 ?
?  ?? Navigations: Tenant, TenantMappings                          ?
????????????????????????????????????????????????????????????????????
               ?                                       ?
????????????????????????????????????????????????????????????????????
?                    Data Access Layer                              ?
?????????????????????????????????????????????????????????????????????
?  ApplicationDbContext (IdentityDbContext<ApplicationUser>)       ?
?  ?? DbSet<Tenant> Tenants                                        ?
?  ?? DbSet<TenantUserMapping> UserTenantMappings                  ?
?  ?? DbSet<ApplicationUser> (via base class)                      ?
?  ?? OnModelCreating() with relationships & indexes               ?
????????????????????????????????????????????????????????????????????
               ?                                       ?
????????????????????????????????????????????????????????????????????
?                   SQL Server Database                             ?
?????????????????????????????????????????????????????????????????????
?  Tenants Table                                                    ?
?  ?? PK: Id                                                        ?
?  ?? UQ: TenantCode                                                ?
?  ?? Data: Name, Status, Limits, Contacts, Timestamps             ?
?                                                                   ?
?  UserTenantMappings Table                                        ?
?  ?? PK: Id                                                        ?
?  ?? FK: TenantId ? Tenants                                        ?
?  ?? FK: UserId ? AspNetUsers                                      ?
?  ?? IX: (TenantId, UserId)                                        ?
?  ?? Data: IsTenantAdmin, Dates                                    ?
?                                                                   ?
?  AspNetUsers Table (Extended)                                    ?
?  ?? PK: Id                                                        ?
?  ?? FK: PrimaryTenantId ? Tenants (Nullable)                     ?
?  ?? New Column: IsSuperAdmin                                      ?
?????????????????????????????????????????????????????????????????????
```

## Data Flow Diagram

### Tenant Creation Flow
```
User (Super Admin)
    ?
Tenants.razor (UI)
    ?
SaveTenant() method
    ?
ITenantManagementService.CreateTenantAsync()
    ?
Validate TenantCode uniqueness
    ?
Create Tenant entity
    ?
ApplicationDbContext.SaveChangesAsync()
    ?
SQL INSERT into Tenants table
    ?
Return Tenant to UI
    ?
Refresh tenant list display
```

### User Assignment Flow
```
User (Super Admin)
    ?
TenantUsers.razor (UI)
    ?
AddUserToTenant() method
    ?
ITenantManagementService.AddUserToTenantAsync()
    ?
Validate Tenant & User exist
    ?
Check if mapping exists
    ?
Create TenantUserMapping entity
    ?
Set/Update PrimaryTenantId if needed
    ?
ApplicationDbContext.SaveChangesAsync()
    ?
SQL INSERT into UserTenantMappings table
    ?
SQL UPDATE AspNetUsers table
    ?
Return success to UI
    ?
Refresh user list display
```

### Tenant Context Resolution Flow
```
User loads page
    ?
TenantService.GetTenantContextAsync() called
    ?
Check if context already cached
    ?
If cached, return immediately
    ?
If not cached:
    ?? Get AuthenticationState
    ?? Extract UserId from ClaimTypes.NameIdentifier
    ?? Query ApplicationDbContext
    ?? Load ApplicationUser with Tenant & TenantMappings
    ?? Determine if IsSuperAdmin
    ?? Get TenantId and TenantCode
    ?? Check if IsTenantAdmin in mappings
    ?? Cache TenantContext
    ?
Return cached TenantContext to caller
```

## Security Architecture

### Authentication Layer
- Uses ASP.NET Identity for user authentication
- Cookie-based authentication for Blazor Server
- IdentityRevalidatingAuthenticationStateProvider ensures auth state is revalidated

### Authorization Layer
```
Policies:
?? SuperAdminOnly ? role.Equals("SuperAdmin")
?? TenantAdminOrSuperAdmin ? role in ["TenantAdmin", "SuperAdmin"]
?? Additional policies from original app
```

### Data Isolation Strategy
1. **Role-based Access Control (RBAC)**
   - Super Admin: No tenant restriction
   - Tenant Admin: Restricted to their tenant
   - Regular User: Restricted to their primary tenant

2. **Database-level Filtering**
   - Foreign key relationships prevent orphaned data
   - Soft deletes preserve audit trail
   - Queries must filter by tenant

3. **Claim-based Verification**
   - User identity verified via Authentication State
   - Tenant membership verified via TenantUserMapping
   - IsSuperAdmin flag prevents unauthorized access

## Component Integration

### Service Dependency Injection
```csharp
// In Program.cs
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ITenantManagementService, TenantManagementService>();

// In Components
@inject ITenantService TenantService
@inject ITenantManagementService TenantManagementService
```

### Component Hierarchies
```
RBMLayout
?? Dashboard
?? Tenants (SuperAdmin only)
?  ?? TenantUsers (SuperAdmin, TenantAdmin)
?? Assets (Tenant-filtered data)
?? WorkOrders (Tenant-filtered data)
?? ... other tenant-aware components
```

## Database Schema

### Tables Created by Migration

#### Tenants Table
```sql
CREATE TABLE [Tenants] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [TenantCode] NVARCHAR(100) NOT NULL UNIQUE,
    [Name] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(500),
    [ContactPerson] NVARCHAR(200),
    [ContactPhone] NVARCHAR(20),
    [ContactEmail] NVARCHAR(200),
    [Address] NVARCHAR(500),
    [City] NVARCHAR(100),
    [Country] NVARCHAR(100),
    [PostalCode] NVARCHAR(20),
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Active',
    [IsActive] BIT NOT NULL DEFAULT 1,
    [MaxUsers] INT NOT NULL DEFAULT 50,
    [MaxAssets] INT NOT NULL DEFAULT 1000,
    [MaxDocuments] INT NOT NULL DEFAULT 10000,
    [CreatedDate] DATETIME2 NOT NULL,
    [ModifiedDate] DATETIME2,
    [CreatedBy] NVARCHAR(200) NOT NULL,
    [ModifiedBy] NVARCHAR(200)
);

CREATE UNIQUE INDEX IX_Tenants_TenantCode ON Tenants(TenantCode);
```

#### UserTenantMappings Table
```sql
CREATE TABLE [UserTenantMappings] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [TenantId] INT NOT NULL,
    [UserId] NVARCHAR(450) NOT NULL,
    [IsTenantAdmin] BIT NOT NULL DEFAULT 0,
    [AssignedDate] DATETIME2 NOT NULL,
    [RemovedDate] DATETIME2,
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

CREATE INDEX IX_UserTenantMappings_TenantId_UserId 
    ON UserTenantMappings(TenantId, UserId);
CREATE INDEX IX_UserTenantMappings_UserId 
    ON UserTenantMappings(UserId);
```

#### AspNetUsers Modifications
```sql
ALTER TABLE [AspNetUsers] ADD
    [PrimaryTenantId] INT NULL,
    [IsSuperAdmin] BIT NOT NULL DEFAULT 0;

ALTER TABLE [AspNetUsers] ADD
    FOREIGN KEY (PrimaryTenantId) REFERENCES Tenants(Id) 
    ON DELETE SET NULL;
```

## Configuration Options

### In Program.cs
```csharp
// Multi-tenancy services
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ITenantManagementService, TenantManagementService>();

// Authorization policies (add to existing)
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("SuperAdminOnly", p => p.RequireRole("SuperAdmin"))
    .AddPolicy("TenantAdminOrSuperAdmin", 
        p => p.RequireRole("SuperAdmin", "TenantAdmin"));

// Existing configurations remain unchanged
```

## Service Implementation Details

### TenantService Caching
- Caches TenantContext per user session
- Avoids repeated database queries for tenant information
- Resets when authentication state changes
- Scoped per HTTP request

### TenantManagementService Features
- Validates uniqueness of TenantCode
- Automatically assigns primary tenant when first user added
- Handles soft deletes via RemovedDate
- Reassigns users when removed from primary tenant
- Tracks audit trail with CreatedBy/ModifiedBy

## Performance Considerations

### Database Queries
- **Indexes**: Added on TenantCode, (TenantId, UserId)
- **Eager Loading**: Uses .Include() for related entities
- **Caching**: Tenant context cached per session
- **Lazy Loading**: Navigation properties loaded only when needed

### Scalability
- Tenant data isolated by ID
- No cross-tenant queries
- Soft deletes prevent cascading deletes
- Audit trail preserved for compliance

### Query Patterns
```csharp
// Good: Filter at database level
var users = await context.Set<ApplicationUser>()
    .Where(u => u.PrimaryTenantId == tenantId)
    .ToListAsync();

// Also good: Use Include for related data
var tenant = await context.Tenants
    .Include(t => t.Users)
    .FirstOrDefaultAsync(t => t.Id == id);

// Avoid: Loading all data then filtering
var allUsers = await context.Users.ToListAsync(); // Bad!
var filtered = allUsers.Where(u => u.TenantId == id);
```

## Error Handling

### Common Errors
1. **Null TenantId**: User not assigned to tenant
2. **Unauthorized Access**: User accessing wrong tenant
3. **Migration Failed**: Database state mismatch
4. **Context Disposed**: DbContext usage outside scope

### Recovery
- Verify database migration applied
- Check user TenantUserMapping exists
- Validate PrimaryTenantId is set
- Review authorization policies

## Testing Considerations

### Unit Testing
```csharp
// Mock ITenantService
var mockTenantService = new Mock<ITenantService>();
mockTenantService
    .Setup(s => s.GetTenantContextAsync())
    .ReturnsAsync(new TenantContext { TenantId = 1 });
```

### Integration Testing
- Use test database with seed data
- Create test tenants and users
- Verify authorization for each role
- Test data isolation

## Future Enhancements

1. **Explicit Tenant Filtering**
   - Add TenantId to all entities
   - Implement query filter in OnModelCreating
   - Automatic tenant filtering

2. **Middleware-based Tenant Resolution**
   - Resolve tenant from URL/header
   - Set tenant context for request
   - Automatic filtering

3. **Tenant-specific Configurations**
   - Feature flags per tenant
   - Custom branding
   - Settings per tenant

4. **Multi-database Support**
   - Separate database per tenant
   - Shared database (current)
   - Hybrid approach

5. **API-based Tenant Management**
   - REST endpoints for tenant management
   - Administrative API
   - Tenant self-service portal
