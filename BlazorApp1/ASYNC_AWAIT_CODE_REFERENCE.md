# Async/Await Quick Reference - RBM Pages

## Standard Page Template

Use this as a template for converting all RBM pages to async/await:

```razor
@page "/rbm/example"
@rendermode InteractiveServer
@layout RBMLayout
@using Microsoft.AspNetCore.Authorization
@attribute [Authorize]
@inject BlazorApp1.Services.DataService DataService
@inject BlazorApp1.Services.CurrentUserService CurrentUser
@inject NavigationManager Navigation
@using BlazorApp1.Models

<PageTitle>Example | RBM CMMS</PageTitle>

@if (!isInitialized)
{
    <div style="text-align: center; padding: 40px;">
        <div style="font-size: 48px; margin-bottom: 16px;">?</div>
        <div>Loading...</div>
    </div>
}
else
{
    <!-- Page content here -->
}

@code {
    private bool isInitialized = false;
    private List<YourModel> items = new();

    protected override async Task OnInitializedAsync()
    {
        await CurrentUser.InitializeAsync();
        await LoadDataAsync();
        isInitialized = true;
    }

    protected override async Task OnParametersSetAsync()
    {
        await CurrentUser.InitializeAsync();
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            items = await DataService.GetItemsAsync();
            // Additional processing
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
        }
    }

    private async Task SaveAsync(YourModel item)
    {
        try
        {
            await DataService.AddAsync(item);
            ShowSuccess("Item saved successfully");
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ShowError($"Error saving item: {ex.Message}");
        }
    }

    private void ShowSuccess(string message)
    {
        // Show success notification
    }

    private void ShowError(string message)
    {
        // Show error notification
    }
}
```

## Common DataService Async Methods

```csharp
// Assets
await DataService.GetAssetsAsync()
await DataService.GetAssetAsync(id)
await DataService.AddAssetAsync(asset)
await DataService.UpdateAssetAsync(asset)
await DataService.DeleteAssetAsync(id)
await DataService.SearchAssetsAsync(term)
await DataService.GetTotalAssetsAsync()
await DataService.GetActiveAssetsAsync()
await DataService.GetCriticalAssetsCountAsync()
await DataService.GetAverageHealthScoreAsync()
await DataService.GetOverdueMaintenanceCountAsync()

// Work Orders
await DataService.GetWorkOrdersAsync()
await DataService.GetWorkOrderAsync(id)
await DataService.AddWorkOrderAsync(workOrder)
await DataService.UpdateWorkOrderAsync(workOrder)
await DataService.DeleteWorkOrderAsync(id)
await DataService.GetAllWorkOrdersAsync()

// Failure Modes
await DataService.GetFailureModesAsync()
await DataService.GetFailureModesAsync(assetId)
await DataService.AddFailureModeAsync(failureMode)
await DataService.UpdateFailureModeAsync(failureMode)
await DataService.DeleteFailureModeAsync(id)

// Condition Readings
await DataService.GetConditionReadingsAsync(assetId)
await DataService.AddConditionReadingAsync(reading)

// Documents
await DataService.GetDocumentsByAssetAsync(assetId)

// Additional
await DataService.GetAllWorkOrdersAsync()
```

## Event Handler Patterns

### Button Click with Async Method

```razor
<!-- Simple async method -->
<button class="btn" @onclick="SaveAsync">Save</button>

<!-- Async method with parameter -->
<button class="btn" @onclick="() => DeleteAsync(item.Id)">Delete</button>

<!-- Lambda with await -->
<button class="btn" @onclick="async () => await ProcessAsync()">Process</button>
```

### Select/Input Change Events

```razor
<!-- Simple async handler -->
<select @onchange="FilterChangedAsync">
    <option value="">All</option>
    <option value="active">Active</option>
</select>

<!-- With parameter -->
<input @onchange="async (e) => await SearchAsync(e.Value?.ToString())" />

// Code
private async Task FilterChangedAsync(ChangeEventArgs e)
{
    selectedFilter = e.Value?.ToString() ?? "";
    await ApplyFiltersAsync();
}

private async Task SearchAsync(string? term)
{
    searchTerm = term ?? "";
    await ApplyFiltersAsync();
}
```

## Error Handling Pattern

```csharp
private async Task OperationAsync()
{
    try
    {
        // Data loading
        var data = await DataService.GetDataAsync();
        
        // Processing
        // ...
    }
    catch (UnauthorizedAccessException ex)
    {
        ShowError($"Access denied: {ex.Message}");
        Navigation.NavigateTo("/login");
    }
    catch (ArgumentException ex)
    {
        ShowError($"Invalid input: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unexpected error: {ex.Message}");
        ShowError("An unexpected error occurred. Please try again.");
    }
}
```

## State Management Pattern

```csharp
private bool isLoading = false;
private bool isInitialized = false;
private string errorMessage = "";

private async Task LoadAsync()
{
    try
    {
        isLoading = true;
        errorMessage = "";
        StateHasChanged();
        
        // Load data
        data = await DataService.GetDataAsync();
        
        isInitialized = true;
    }
    catch (Exception ex)
    {
        errorMessage = ex.Message;
    }
    finally
    {
        isLoading = false;
        StateHasChanged();
    }
}
```

## Success/Error Notifications

```csharp
private bool showNotification = false;
private string notificationMessage = "";
private bool isSuccess = false;

private void ShowSuccess(string message)
{
    notificationMessage = message;
    isSuccess = true;
    showNotification = true;
    StateHasChanged();
    
    Task.Delay(3000).ContinueWith(_ =>
    {
        showNotification = false;
        InvokeAsync(StateHasChanged);
    });
}

private void ShowError(string message)
{
    notificationMessage = message;
    isSuccess = false;
    showNotification = true;
    StateHasChanged();
    
    Task.Delay(4000).ContinueWith(_ =>
    {
        showNotification = false;
        InvokeAsync(StateHasChanged);
    });
}

// In markup
@if (showNotification)
{
    <div class="notification @(isSuccess ? "success" : "error")">
        @notificationMessage
    </div>
}
```

## Common Mistakes to Avoid

### ? DON'T - Blocking Calls

```csharp
// Bad - Blocks thread
protected override void OnInitialized()
{
    data = DataService.GetData(); // Synchronous
}

// Bad - Synchronously waiting
protected override async Task OnInitializedAsync()
{
    data = DataService.GetDataAsync().Result; // Blocks!
}
```

### ? DO - Proper Async

```csharp
// Good - Properly async
protected override async Task OnInitializedAsync()
{
    data = await DataService.GetDataAsync(); // Non-blocking
}
```

### ? DON'T - Fire and Forget

```razor
<!-- Bad - No await -->
<button @onclick="() => SaveAsync()">Save</button>

<!-- Bad - No async in lambda -->
<button @onclick="SaveAsync">Save</button>
```

### ? DO - Proper Event Handlers

```razor
<!-- Good - Awaits the async method -->
<button @onclick="async () => await SaveAsync()">Save</button>
```

## Testing Checklist

After converting a page, verify:
- [ ] Page loads without blocking UI
- [ ] Loading indicator appears while data loads
- [ ] Data displays correctly after loading
- [ ] Create/Update/Delete operations work
- [ ] Filters and search work asynchronously
- [ ] Success/error messages display properly
- [ ] Navigation between pages is smooth
- [ ] No console errors or warnings
- [ ] Browser DevTools shows no hanging requests

---

**Use this reference when converting Pages: Analytics, Documents, SpareParts, ConditionMonitoring, and others**
