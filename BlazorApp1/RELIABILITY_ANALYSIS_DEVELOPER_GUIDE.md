# ????? Reliability Analysis - Developer Implementation Guide

## Overview for Developers

This guide explains the implementation details of the Reliability Analysis feature for developers who need to maintain, extend, or troubleshoot the system.

---

## ??? Architecture

### Component Structure

```
ReliabilityAnalysis.razor (Main Component)
??? Parameters
?   ??? AssetId (nullable)
??? State
?   ??? FleetView (AssetId == null)
?   ??? AssetView (AssetId != null)
??? Child Components (integrated)
?   ??? Metrics Cards
?   ??? Filter Section
?   ??? Data Tables
?   ??? Charts
?   ??? Status Indicators
??? Services
    ??? DataService
    ??? CurrentUserService
    ??? NavigationManager
```

### Data Flow

```
1. OnInitializedAsync
   ??> CurrentUser.InitializeAsync()
   ??> LoadData()

2. LoadData
   ??> GetAssets()
   ??> If AssetId
   ?   ??> GetAsset(AssetId)
   ?   ??> GetReliabilityMetrics(AssetId)
   ?   ??> GetFailureModes(AssetId)
   ?   ??> GetAssetDowntime(AssetId)
   ??> Else
       ??> CalculateFleetMetrics()
       ??> ApplyFilters()

3. UI Renders
   ??> Dashboard (Fleet view)
   ??> DetailPage (Asset view)
```

---

## ?? Implementation Details

### 1. Component Lifecycle

```csharp
protected override async Task OnInitializedAsync()
{
    // Called on first render
    await CurrentUser.InitializeAsync();
    LoadData();
    isInitialized = true;
}

protected override async Task OnParametersSetAsync()
{
    // Called when parameters change (route change)
    await CurrentUser.InitializeAsync();
    LoadData();
}
```

### 2. State Management

**Key State Variables:**
```csharp
private List<Asset> assets = new();                    // All assets
private List<ReliabilityMetric> reliabilityData = new(); // Filtered metrics
private List<ReliabilityMetric> selectedMetrics = new(); // Asset-specific
private List<FailureMode> assetFailureModes = new();     // FMEA data
private List<AssetDowntime> downtimeEvents = new();      // Downtime records

private string selectedPeriod = "Monthly";              // Filter
private string selectedAssetFilter = "all";             // Filter
private double fleetAvailability = 0;                   // Fleet KPI
```

### 3. Data Loading Logic

```csharp
private void LoadData()
{
    // 1. Load all assets
    assets = DataService.GetAssets();
    
    // 2. If AssetId parameter exists
    if (AssetId.HasValue && AssetId.Value > 0)
    {
        selectedAsset = assets.FirstOrDefault(a => a.Id == AssetId.Value);
        if (selectedAsset != null)
        {
            // Load asset-specific data
            selectedMetrics = DataService.GetReliabilityMetrics(selectedAsset.Id);
            assetFailureModes = DataService.GetFailureModes(selectedAsset.Id);
            downtimeEvents = DataService.GetAssetDowntime(selectedAsset.Id);
        }
    }
    // 3. Otherwise, calculate fleet metrics
    else
    {
        CalculateFleetMetrics();
    }
}
```

### 4. Filtering Logic

```csharp
private void ApplyFilters()
{
    var filtered = reliabilityData.AsEnumerable();

    // Apply criticality filter
    if (selectedAssetFilter == "critical")
    {
        filtered = filtered.Where(m => m.Availability < 85);
    }
    
    // Apply availability filter
    else if (selectedAssetFilter == "low-availability")
    {
        filtered = filtered.Where(m => m.Availability < 90);
    }
    
    // Apply downtime filter
    else if (selectedAssetFilter == "high-downtime")
    {
        filtered = filtered.Where(m => m.TotalDowntimeHours > 5);
    }

    reliabilityData = filtered.ToList();
}
```

### 5. Metric Calculation

```csharp
private void CalculateFleetMetrics()
{
    // Gather data for current period
    foreach (var asset in assets)
    {
        var metrics = DataService.GetReliabilityMetrics(asset.Id);
        if (metrics.Any())
        {
            var latest = metrics
                .OrderByDescending(m => m.MetricDate)
                .First();
            
            // Match period
            if (latest.Period == selectedPeriod)
            {
                reliabilityData.Add(latest);
            }
        }
    }

    // Calculate fleet averages
    if (reliabilityData.Any())
    {
        fleetAvailability = reliabilityData.Average(m => m.Availability);
        avgMTBF = reliabilityData.Average(m => m.MTBF);
        avgMTTR = reliabilityData.Average(m => m.MTTR);
        avgOEE = reliabilityData.Average(m => m.OEE);
        criticalReliabilityCount = reliabilityData.Count(m => m.Availability < 80);
    }
}
```

### 6. UI Conditional Rendering

**Fleet View (AssetId == null):**
```razor
@if (AssetId == null)
{
    <!-- Dashboard with metrics and tables -->
    <Metrics />
    <Filters />
    <Table />
    <Charts />
}
```

**Asset View (AssetId != null):**
```razor
@else if (selectedAsset != null)
{
    <!-- Detailed asset analysis -->
    <AssetHeader />
    <MetricCards />
    <PerformanceIndicators />
    <TrendCharts />
    <DowntimeHistory />
    <FailureModes />
}
```

---

## ?? DataService Methods

### Reliability Metrics

```csharp
// Get all metrics for asset
public List<ReliabilityMetric> GetReliabilityMetrics(int assetId)
{
    using var context = _contextFactory.CreateDbContext();
    return context.ReliabilityMetrics
        .Where(rm => rm.AssetId == assetId)
        .OrderByDescending(rm => rm.MetricDate)
        .ToList();
}

// Add new metric
public void AddReliabilityMetric(ReliabilityMetric metric)
{
    using var context = _contextFactory.CreateDbContext();
    metric.CalculatedDate = DateTime.Now;
    context.ReliabilityMetrics.Add(metric);
    context.SaveChanges();
}
```

### Downtime Management

```csharp
// Get downtime for asset
public List<AssetDowntime> GetAssetDowntime(int assetId)
{
    using var context = _contextFactory.CreateDbContext();
    return context.AssetDowntime
        .Where(ad => ad.AssetId == assetId)
        .OrderByDescending(ad => ad.StartTime)
        .ToList();
}

// Add downtime event
public void AddAssetDowntime(AssetDowntime downtime)
{
    using var context = _contextFactory.CreateDbContext();
    downtime.RecordedDate = DateTime.Now;
    context.AssetDowntime.Add(downtime);
    context.SaveChanges();
}
```

### Downtime Statistics

```csharp
// Calculate total downtime hours
public double GetTotalDowntimeHours(int assetId, DateTime? startDate = null, DateTime? endDate = null)
{
    using var context = _contextFactory.CreateDbContext();
    var query = context.AssetDowntime
        .Where(ad => ad.AssetId == assetId);
    
    if (startDate.HasValue) query = query.Where(ad => ad.StartTime >= startDate);
    if (endDate.HasValue) query = query.Where(ad => ad.StartTime <= endDate);
    
    return query.Sum(ad => 
        (ad.EndTime.HasValue ? (ad.EndTime.Value - ad.StartTime).TotalHours 
                             : (DateTime.Now - ad.StartTime).TotalHours));
}

// Total downtime events
public int GetTotalDowntimeEvents(int assetId)
{
    using var context = _contextFactory.CreateDbContext();
    return context.AssetDowntime
        .Count(ad => ad.AssetId == assetId);
}

// Production loss impact
public decimal GetTotalProductionLoss(int assetId)
{
    using var context = _contextFactory.CreateDbContext();
    return context.AssetDowntime
        .Where(ad => ad.AssetId == assetId)
        .Sum(ad => ad.ProductionLoss);
}

// Financial impact
public decimal GetTotalFinancialImpact(int assetId)
{
    using var context = _contextFactory.CreateDbContext();
    return context.AssetDowntime
        .Where(ad => ad.AssetId == assetId)
        .Sum(ad => ad.FinancialImpact);
}
```

---

## ?? Helper Methods

### Color Functions

```csharp
// Availability color coding
private string GetAvailabilityColor(double availability)
{
    if (availability >= 95) return "#43a047";      // Green
    if (availability >= 90) return "#fb8c00";      // Orange
    if (availability >= 85) return "#fbc02d";      // Yellow
    return "#e53935";                               // Red
}

// OEE color coding
private string GetOEEColor(double oee)
{
    if (oee >= 85) return "#43a047";                // Green
    if (oee >= 70) return "#fb8c00";                // Orange
    return "#e53935";                               // Red
}

// RPN (Risk Priority) color
private string GetRPNColor(int rpn)
{
    if (rpn > 200) return "#e53935";                // Red
    if (rpn >= 100) return "#fb8c00";               // Orange
    return "#43a047";                               // Green
}
```

### Status Functions

```csharp
// Reliability status classification
private string GetReliabilityStatus(ReliabilityMetric metric)
{
    if (metric.Availability >= 95 && metric.OEE >= 85) return "Excellent";
    if (metric.Availability >= 90 && metric.OEE >= 70) return "Good";
    if (metric.Availability >= 85 && metric.OEE >= 60) return "Fair";
    if (metric.Availability >= 80 && metric.OEE >= 50) return "Poor";
    return "Critical";
}

// MTBF categorization
private string GetMTBFCategory(double mtbf)
{
    if (mtbf >= 1000) return "> 1000h";
    if (mtbf >= 500) return "500-1000h";
    if (mtbf >= 200) return "200-500h";
    return "< 200h";
}

// Issue identification
private (string Icon, string Title, string Description, string Color) GetReliabilityIssue(ReliabilityMetric metric)
{
    if (metric.Availability < 80)
        return ("??", "Critical Availability", "System availability below 80%", "#e53935");
    if (metric.MTTR > 8)
        return ("??", "Long Repair Time", "Average repair time exceeds 8 hours", "#fb8c00");
    if (metric.FailureCount > 10)
        return ("??", "High Failure Rate", "More than 10 failures in period", "#fbc02d");
    return ("??", "Requires Attention", "Reliability metrics below targets", "#1976d2");
}
```

### Assessment Functions

```csharp
private (string Title, string Description, string BgColor, string BorderColor, string TextColor, List<string> Recommendations) 
GetReliabilityAssessment(ReliabilityMetric metric)
{
    if (metric.Availability >= 95 && metric.OEE >= 85)
    {
        return (
            "Excellent Performance",
            "Asset is performing at or above world-class standards",
            "#e8f5e9",
            "#43a047",
            "#2e7d32",
            new List<string>
            {
                "Maintain current preventive maintenance schedule",
                "Monitor for any deterioration trends",
                "Document best practices for other assets"
            }
        );
    }
    // ... more conditions
}
```

---

## ?? Integration Points

### Asset Integration
```csharp
selectedAsset = DataService.GetAsset(AssetId.Value);
// Properties: Id, AssetId, Name, HealthScore, Status, Location, etc.
```

### FailureMode Integration
```csharp
assetFailureModes = DataService.GetFailureModes(selectedAsset.Id);
// Properties: Mode, Cause, Effect, Severity, Occurrence, Detection, RPN
```

### WorkOrder Integration
```csharp
// Via AssetDowntime.RelatedWorkOrderId
dt.RelatedWorkOrderId  // Links to work order
```

---

## ? Testing Guide

### Unit Test Examples

```csharp
[TestClass]
public class ReliabilityAnalysisTests
{
    [TestMethod]
    public void GetAvailabilityColor_HighAvailability_ReturnsGreen()
    {
        var color = component.GetAvailabilityColor(95);
        Assert.AreEqual("#43a047", color);
    }

    [TestMethod]
    public void CalculateFleetMetrics_WithMetrics_CalculatesAverages()
    {
        // Arrange
        var metrics = new List<ReliabilityMetric>
        {
            new() { Availability = 95, OEE = 90, MTBF = 1000 },
            new() { Availability = 85, OEE = 80, MTBF = 500 }
        };

        // Act
        component.CalculateFleetMetrics();

        // Assert
        Assert.AreEqual(90, component.fleetAvailability);
    }

    [TestMethod]
    public void GetMTBFCategory_LowValue_ReturnsCorrectCategory()
    {
        var category = component.GetMTBFCategory(150);
        Assert.AreEqual("< 200h", category);
    }
}
```

### Integration Test Examples

```csharp
[TestClass]
public class ReliabilityAnalysisIntegrationTests
{
    [TestMethod]
    public async Task LoadData_WithAssetId_LoadsAssetMetrics()
    {
        // Navigate to specific asset
        var result = await client.GetAsync("/rbm/reliability-analysis/5");
        Assert.IsTrue(result.IsSuccessStatusCode);
        
        // Verify metrics loaded
        var content = await result.Content.ReadAsStringAsync();
        Assert.IsTrue(content.Contains("MTBF"));
    }

    [TestMethod]
    public async Task ApplyFilters_WithCriticalFilter_ShowsOnlyLowAvailability()
    {
        // Apply critical filter
        component.selectedAssetFilter = "critical";
        component.ApplyFilters();

        // Verify all assets have < 85% availability
        foreach (var metric in component.reliabilityData)
        {
            Assert.IsTrue(metric.Availability < 85);
        }
    }
}
```

---

## ?? Debugging Tips

### Enable Debug Output

```csharp
// In LoadData method
Console.WriteLine($"Loading {assets.Count} assets");
Console.WriteLine($"Fleet Availability: {fleetAvailability:F1}%");
Console.WriteLine($"Critical Assets: {criticalReliabilityCount}");
```

### Check Data Availability

```csharp
// Verify metrics exist
if (!reliabilityData.Any())
{
    Console.WriteLine("Warning: No reliability metrics found");
    Console.WriteLine("Available assets: " + assets.Count);
}
```

### Validate Calculations

```csharp
// Verify metric calculations
var metric = reliabilityData.First();
var calculatedAvg = reliabilityData.Average(m => m.Availability);
Console.WriteLine($"Calculated Average Availability: {calculatedAvg:F1}%");
```

---

## ?? Performance Considerations

### Database Query Optimization

**Good (Efficient):**
```csharp
var metrics = context.ReliabilityMetrics
    .Where(rm => rm.AssetId == assetId)
    .OrderByDescending(rm => rm.MetricDate)
    .Take(12)  // Limit results
    .ToList();
```

**Bad (Inefficient):**
```csharp
var allMetrics = context.ReliabilityMetrics.ToList();
var filtered = allMetrics.Where(rm => rm.AssetId == assetId).ToList();
var sorted = filtered.OrderByDescending(rm => rm.MetricDate).ToList();
```

### Calculated Properties

**Good (Client-side):**
```csharp
[NotMapped]
public double DurationHours => 
    EndTime.HasValue 
        ? (EndTime.Value - StartTime).TotalHours 
        : (DateTime.Now - StartTime).TotalHours;
```

**Bad (Database calculation):**
```csharp
public double DurationHours { get; set; }  // Requires DB recalculation
```

---

## ?? Common Extensions

### Add Export to CSV

```csharp
private void ExportMetricsToCSV()
{
    var csv = new StringBuilder();
    csv.AppendLine("Asset ID,Name,MTBF,MTTR,Availability,OEE");
    
    foreach (var metric in reliabilityData)
    {
        var asset = assets.FirstOrDefault(a => a.Id == metric.AssetId);
        csv.AppendLine($"{asset?.AssetId},{asset?.Name},{metric.MTBF:F0}," +
                      $"{metric.MTTR:F1},{metric.Availability:F1},{metric.OEE:F1}");
    }
    
    // Download file...
}
```

### Add Predictive Analytics

```csharp
private double PredictNextMonthMTBF(List<ReliabilityMetric> historicalData)
{
    // Simple linear regression
    var dataPoints = historicalData.OrderBy(m => m.MetricDate).ToList();
    var xValues = Enumerable.Range(0, dataPoints.Count).Select(i => (double)i).ToArray();
    var yValues = dataPoints.Select(d => d.MTBF).ToArray();
    
    // Calculate trend
    var slope = CalculateSlope(xValues, yValues);
    var nextValue = yValues.Last() + slope;
    
    return nextValue;
}
```

### Add Email Alerts

```csharp
private async Task SendCriticalAssetAlert(ReliabilityMetric metric)
{
    if (metric.Availability < 80)
    {
        var asset = assets.FirstOrDefault(a => a.Id == metric.AssetId);
        var message = $"Critical: {asset?.Name} has {metric.Availability:F1}% availability";
        
        // Send email to reliability team...
        await EmailService.SendAsync("reliability@company.com", "Critical Asset Alert", message);
    }
}
```

---

## ?? Related Files

- `BlazorApp1/Components/Pages/RBM/ReliabilityAnalysis.razor` - Main component
- `BlazorApp1/Services/DataService.cs` - Data access layer
- `BlazorApp1/Models/ReliabilityMetric.cs` - Metric model
- `BlazorApp1/Models/AssetDowntime.cs` - Downtime model
- `BlazorApp1/Data/ApplicationDbContext.cs` - Database context

---

## ?? Support

For implementation questions:
1. Review this guide
2. Check the component code comments
3. Review DataService methods
4. Check related test cases
5. Consult with team lead

---

**Version:** 1.0
**Last Updated:** December 15, 2024
**Status:** ? Ready for Development
