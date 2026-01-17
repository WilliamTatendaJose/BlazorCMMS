using System.Text;
using BlazorApp1.Data;
using BlazorApp1.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace BlazorApp1.Services;

/// <summary>
/// Service for exporting data in multiple formats (CSV, Excel, JSON, PDF)
/// </summary>
public class DataExportService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly RolePermissionService _rolePermissionService;

    public DataExportService(IDbContextFactory<ApplicationDbContext> contextFactory, RolePermissionService rolePermissionService)
    {
        _contextFactory = contextFactory;
        _rolePermissionService = rolePermissionService;
    }
    
    /// <summary>
    /// Get tenant context for filtering
    /// </summary>
    private async Task<(bool IsSuperAdmin, int? TenantId)> GetTenantContextAsync()
    {
        var isSuperAdmin = await _rolePermissionService.IsSuperAdminAsync();
        var tenantId = await _rolePermissionService.GetCurrentTenantIdAsync();
        return (isSuperAdmin, tenantId);
    }

    #region CSV Export

    /// <summary>
    /// Export assets to CSV format
    /// </summary>
    public byte[] ExportAssetsToCSV()
    {
        using var context = _contextFactory.CreateDbContext();
        var assets = context.Assets.Where(a => !a.IsRetired).ToList();
        
        var csv = new StringBuilder();
        csv.AppendLine("AssetID,Name,ModelNumber,SerialNumber,Manufacturer,Location,Department,Criticality,Status,HealthScore,NextMaintenance,CreatedDate");
        
        foreach (var asset in assets)
        {
            csv.AppendLine($"\"{asset.AssetId}\",\"{asset.Name}\",\"{asset.ModelNumber}\",\"{asset.SerialNumber}\"," +
                          $"\"{asset.EquipmentManufacturer}\",\"{asset.Location}\",\"{asset.Department}\",\"{asset.Criticality}\"," +
                          $"\"{asset.Status}\",{asset.HealthScore:F2},\"{asset.NextScheduledMaintenance:yyyy-MM-dd}\",\"{asset.CreatedDate:yyyy-MM-dd}\"");
        }
        
        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    /// <summary>
    /// Export work orders to CSV format
    /// </summary>
    public byte[] ExportWorkOrdersToCSV()
    {
        using var context = _contextFactory.CreateDbContext();
        var workOrders = context.WorkOrders
            .Include(wo => wo.Asset)
            .OrderByDescending(wo => wo.CreatedDate)
            .ToList();
        
        var csv = new StringBuilder();
        csv.AppendLine("WorkOrderID,AssetID,Type,Priority,Status,AssignedTo,DueDate,CreatedDate,CompletedDate,Description");
        
        foreach (var wo in workOrders)
        {
            var description = wo.Description?.Replace("\"", "\"\"") ?? "";
            csv.AppendLine($"\"{wo.WorkOrderId}\",\"{wo.Asset?.AssetId}\",\"{wo.Type}\",\"{wo.Priority}\"," +
                          $"\"{wo.Status}\",\"{wo.AssignedTo}\",\"{wo.DueDate:yyyy-MM-dd}\",\"{wo.CreatedDate:yyyy-MM-dd}\"," +
                          $"\"{(wo.CompletedDate?.ToString("yyyy-MM-dd") ?? "")}\",\"{description}\"");
        }
        
        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    /// <summary>
    /// Export failure modes to CSV format
    /// </summary>
    public byte[] ExportFailureModesToCSV()
    {
        using var context = _contextFactory.CreateDbContext();
        var failureModes = context.FailureModes
            .Include(fm => fm.Asset)
            .OrderByDescending(fm => fm.RPN)
            .ToList();
        
        var csv = new StringBuilder();
        csv.AppendLine("AssetID,FailureMode,Severity,Occurrence,Detection,RPN,Effect,CreatedDate");
        
        foreach (var fm in failureModes)
        {
            var effect = fm.Effect?.Replace("\"", "\"\"") ?? "";
            
            csv.AppendLine($"\"{fm.Asset?.AssetId}\",\"{fm.Mode}\",{fm.Severity},{fm.Occurrence}," +
                          $"{fm.Detection},{fm.RPN},\"{effect}\",\"{fm.CreatedDate:yyyy-MM-dd}\"");
        }
        
        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    /// <summary>
    /// Export spare parts to CSV format
    /// </summary>
    public byte[] ExportSparePartsToCSV()
    {
        using var context = _contextFactory.CreateDbContext();
        var spareParts = context.SpareParts
            .Include(sp => sp.Asset)
            .OrderBy(sp => sp.PartNumber)
            .ToList();
        
        var csv = new StringBuilder();
        csv.AppendLine("PartNumber,Description,Manufacturer,QuantityInStock,ReorderPoint,UnitCost,TotalValue,Status,LastUsedDate,LastRestockDate");
        
        foreach (var sp in spareParts)
        {
            var totalValue = sp.QuantityInStock * sp.UnitCost;
            csv.AppendLine($"\"{sp.PartNumber}\",\"{sp.Description}\",\"{sp.Manufacturer}\"," +
                          $"{sp.QuantityInStock},{sp.ReorderPoint},{sp.UnitCost:F2},{totalValue:F2}," +
                          $"\"{sp.Status}\",\"{(sp.LastUsedDate?.ToString("yyyy-MM-dd") ?? "")}\",\"{(sp.LastRestockDate?.ToString("yyyy-MM-dd") ?? "")}\"");
        }
        
        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    /// <summary>
    /// Export condition readings to CSV format
    /// </summary>
    public byte[] ExportConditionReadingsToCSV(int assetId)
    {
        using var context = _contextFactory.CreateDbContext();
        var readings = context.ConditionReadings
            .Where(cr => cr.AssetId == assetId)
            .OrderByDescending(cr => cr.ReadingDate)
            .ToList();
        
        var csv = new StringBuilder();
        csv.AppendLine("ReadingDate,Temperature,Vibration,Pressure,Notes");
        
        foreach (var reading in readings)
        {
            var notes = reading.Notes?.Replace("\"", "\"\"") ?? "";
            csv.AppendLine($"\"{reading.ReadingDate:yyyy-MM-dd HH:mm}\",{reading.Temperature}," +
                          $"{reading.Vibration},{reading.Pressure},\"{notes}\"");
        }
        
        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    /// <summary>
    /// Export documents to CSV format
    /// </summary>
    public byte[] ExportDocumentsToCSV()
    {
        using var context = _contextFactory.CreateDbContext();
        var documents = context.Documents
            .Include(d => d.Asset)
            .OrderByDescending(d => d.CreatedDate)
            .ToList();
        
        var csv = new StringBuilder();
        csv.AppendLine("FileName,Category,AssetID,CreatedDate,ExpiryDate,ViewCount,DownloadCount");
        
        foreach (var doc in documents)
        {
            csv.AppendLine($"\"{doc.FileName}\",\"{doc.Category}\",\"{doc.Asset?.AssetId}\"," +
                          $"\"{doc.CreatedDate:yyyy-MM-dd}\",\"{(doc.ExpiryDate?.ToString("yyyy-MM-dd") ?? "")}\"," +
                          $"{doc.ViewCount},{doc.DownloadCount}");
        }
        
        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    #endregion

    #region JSON Export

    /// <summary>
    /// Export all data to JSON format (comprehensive export)
    /// </summary>
    public byte[] ExportAllDataToJSON()
    {
        using var context = _contextFactory.CreateDbContext();
        
        var exportData = new
        {
            ExportDate = DateTime.Now,
            Assets = context.Assets.Where(a => !a.IsRetired).ToList(),
            WorkOrders = context.WorkOrders.Include(wo => wo.Asset).ToList(),
            FailureModes = context.FailureModes.Include(fm => fm.Asset).ToList(),
            SpareParts = context.SpareParts.Include(sp => sp.Asset).ToList(),
            Documents = context.Documents.Include(d => d.Asset).ToList(),
            ConditionReadings = context.ConditionReadings.ToList(),
            MaintenanceSchedules = context.MaintenanceSchedules.ToList(),
            ReliabilityMetrics = context.ReliabilityMetrics.ToList()
        };
        
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
        
        var json = JsonSerializer.Serialize(exportData, options);
        return Encoding.UTF8.GetBytes(json);
    }

    /// <summary>
    /// Export assets to JSON format
    /// </summary>
    public byte[] ExportAssetsToJSON()
    {
        using var context = _contextFactory.CreateDbContext();
        var assets = context.Assets.Where(a => !a.IsRetired).ToList();
        
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
        
        var json = JsonSerializer.Serialize(assets, options);
        return Encoding.UTF8.GetBytes(json);
    }

    /// <summary>
    /// Export work orders to JSON format
    /// </summary>
    public byte[] ExportWorkOrdersToJSON()
    {
        using var context = _contextFactory.CreateDbContext();
        var workOrders = context.WorkOrders
            .Include(wo => wo.Asset)
            .OrderByDescending(wo => wo.CreatedDate)
            .ToList();
        
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
        
        var json = JsonSerializer.Serialize(workOrders, options);
        return Encoding.UTF8.GetBytes(json);
    }

    #endregion

    #region Excel Export Helpers

    /// <summary>
    /// Generate proper Excel file for assets
    /// </summary>
    public byte[] ExportAssetsToExcel()
    {
        using var context = _contextFactory.CreateDbContext();
        var assets = context.Assets.Where(a => !a.IsRetired).ToList();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Assets");

            // Add header row
            var headers = new[] { "Asset ID", "Name", "Model", "Manufacturer", "Location", "Department", "Criticality", "Status", "Health Score", "Next Maintenance", "Created Date" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            // Style header row
            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#0288d1");
            headerRow.Style.Font.FontColor = XLColor.White;
            headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Add data rows
            int row = 2;
            foreach (var asset in assets)
            {
                worksheet.Cell(row, 1).Value = asset.AssetId;
                worksheet.Cell(row, 2).Value = asset.Name;
                worksheet.Cell(row, 3).Value = asset.ModelNumber;
                worksheet.Cell(row, 4).Value = asset.EquipmentManufacturer;
                worksheet.Cell(row, 5).Value = asset.Location;
                worksheet.Cell(row, 6).Value = asset.Department;
                worksheet.Cell(row, 7).Value = asset.Criticality;
                worksheet.Cell(row, 8).Value = asset.Status;
                worksheet.Cell(row, 9).Value = asset.HealthScore;
                worksheet.Cell(row, 9).Style.NumberFormat.Format = "0.00";
                worksheet.Cell(row, 10).Value = asset.NextScheduledMaintenance;
                worksheet.Cell(row, 10).Style.DateFormat.Format = "yyyy-MM-dd";
                worksheet.Cell(row, 11).Value = asset.CreatedDate;
                worksheet.Cell(row, 11).Style.DateFormat.Format = "yyyy-MM-dd";

                // Alternate row colors
                if (row % 2 == 0)
                {
                    worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#f9f9f9");
                }

                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            // Save to memory stream
            using (var ms = new MemoryStream())
            {
                workbook.SaveAs(ms);
                return ms.ToArray();
            }
        }
    }

    /// <summary>
    /// Generate proper Excel file for work orders
    /// </summary>
    public byte[] ExportWorkOrdersToExcel()
    {
        using var context = _contextFactory.CreateDbContext();
        var workOrders = context.WorkOrders
            .Include(wo => wo.Asset)
            .OrderByDescending(wo => wo.CreatedDate)
            .ToList();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Work Orders");

            // Add header row
            var headers = new[] { "Work Order ID", "Asset ID", "Type", "Priority", "Status", "Assigned To", "Due Date", "Created Date", "Completed Date", "Description" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            // Style header row
            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#0288d1");
            headerRow.Style.Font.FontColor = XLColor.White;
            headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Add data rows
            int row = 2;
            foreach (var wo in workOrders)
            {
                worksheet.Cell(row, 1).Value = wo.WorkOrderId;
                worksheet.Cell(row, 2).Value = wo.Asset?.AssetId;
                worksheet.Cell(row, 3).Value = wo.Type;
                worksheet.Cell(row, 4).Value = wo.Priority;
                worksheet.Cell(row, 5).Value = wo.Status;
                worksheet.Cell(row, 6).Value = wo.AssignedTo;
                worksheet.Cell(row, 7).Value = wo.DueDate;
                worksheet.Cell(row, 7).Style.DateFormat.Format = "yyyy-MM-dd";
                worksheet.Cell(row, 8).Value = wo.CreatedDate;
                worksheet.Cell(row, 8).Style.DateFormat.Format = "yyyy-MM-dd";
                worksheet.Cell(row, 9).Value = wo.CompletedDate;
                worksheet.Cell(row, 9).Style.DateFormat.Format = "yyyy-MM-dd";
                worksheet.Cell(row, 10).Value = wo.Description;

                // Alternate row colors
                if (row % 2 == 0)
                {
                    worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#f9f9f9");
                }

                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            // Save to memory stream
            using (var ms = new MemoryStream())
            {
                workbook.SaveAs(ms);
                return ms.ToArray();
            }
        }
    }

    /// <summary>
    /// Generate proper Excel file for spare parts
    /// </summary>
    public byte[] ExportSparePartsToExcel()
    {
        using var context = _contextFactory.CreateDbContext();
        var spareParts = context.SpareParts
            .Include(sp => sp.Asset)
            .OrderBy(sp => sp.PartNumber)
            .ToList();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Spare Parts");

            // Add header row
            var headers = new[] { "Part Number", "Description", "Manufacturer", "Quantity In Stock", "Reorder Point", "Unit Cost", "Total Value", "Status", "Last Used Date", "Last Restock Date" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            // Style header row
            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#0288d1");
            headerRow.Style.Font.FontColor = XLColor.White;
            headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Add data rows
            int row = 2;
            foreach (var sp in spareParts)
            {
                var totalValue = sp.QuantityInStock * sp.UnitCost;
                worksheet.Cell(row, 1).Value = sp.PartNumber;
                worksheet.Cell(row, 2).Value = sp.Description;
                worksheet.Cell(row, 3).Value = sp.Manufacturer;
                worksheet.Cell(row, 4).Value = sp.QuantityInStock;
                worksheet.Cell(row, 5).Value = sp.ReorderPoint;
                worksheet.Cell(row, 6).Value = sp.UnitCost;
                worksheet.Cell(row, 6).Style.NumberFormat.Format = "$#,##0.00";
                worksheet.Cell(row, 7).Value = totalValue;
                worksheet.Cell(row, 7).Style.NumberFormat.Format = "$#,##0.00";
                worksheet.Cell(row, 8).Value = sp.Status;
                worksheet.Cell(row, 9).Value = sp.LastUsedDate;
                worksheet.Cell(row, 9).Style.DateFormat.Format = "yyyy-MM-dd";
                worksheet.Cell(row, 10).Value = sp.LastRestockDate;
                worksheet.Cell(row, 10).Style.DateFormat.Format = "yyyy-MM-dd";

                // Alternate row colors
                if (row % 2 == 0)
                {
                    worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#f9f9f9");
                }

                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            // Save to memory stream
            using (var ms = new MemoryStream())
            {
                workbook.SaveAs(ms);
                return ms.ToArray();
            }
        }
    }

    #endregion

    #region Analysis Report Export

    /// <summary>
    /// Export reliability analysis report to Excel
    /// </summary>
    public byte[] ExportReliabilityAnalysisReport()
    {
        using var context = _contextFactory.CreateDbContext();
        var metrics = context.ReliabilityMetrics
            .Include(rm => rm.Asset)
            .OrderByDescending(rm => rm.CalculatedDate)
            .ToList();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Reliability Analysis");

            // Add title
            worksheet.Cell(1, 1).Value = "Reliability Analysis Report";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Range(1, 1, 1, 8).Merge();

            // Add export date
            worksheet.Cell(2, 1).Value = $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            worksheet.Cell(2, 1).Style.Font.FontSize = 11;

            // Headers
            var headers = new[] { "Asset ID", "Asset Name", "MTBF (Hours)", "MTTR (Hours)", "Availability %", "Reliability %", "OEE %", "Analysis Date" };
            var startRow = 4;
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(startRow, i + 1).Value = headers[i];
            }

            var headerRow = worksheet.Row(startRow);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#0288d1");
            headerRow.Style.Font.FontColor = XLColor.White;
            headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Add data
            int row = startRow + 1;
            foreach (var metric in metrics)
            {
                worksheet.Cell(row, 1).Value = metric.Asset?.AssetId;
                worksheet.Cell(row, 2).Value = metric.Asset?.Name;
                worksheet.Cell(row, 3).Value = metric.MTBF;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "0.00";
                worksheet.Cell(row, 4).Value = metric.MTTR;
                worksheet.Cell(row, 4).Style.NumberFormat.Format = "0.00";
                worksheet.Cell(row, 5).Value = metric.Availability;
                worksheet.Cell(row, 5).Style.NumberFormat.Format = "0.00\"%\"";
                worksheet.Cell(row, 6).Value = metric.Reliability;
                worksheet.Cell(row, 6).Style.NumberFormat.Format = "0.00\"%\"";
                worksheet.Cell(row, 7).Value = metric.OEE;
                worksheet.Cell(row, 7).Style.NumberFormat.Format = "0.00\"%\"";
                worksheet.Cell(row, 8).Value = metric.CalculatedDate;
                worksheet.Cell(row, 8).Style.DateFormat.Format = "yyyy-MM-dd";

                if (row % 2 == 0)
                {
                    worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#f9f9f9");
                }

                row++;
            }

            worksheet.Columns().AdjustToContents();

            using (var ms = new MemoryStream())
            {
                workbook.SaveAs(ms);
                return ms.ToArray();
            }
        }
    }

    /// <summary>
    /// Export condition monitoring summary report
    /// </summary>
    public byte[] ExportConditionMonitoringReport()
    {
        using var context = _contextFactory.CreateDbContext();
        var assets = context.Assets
            .Where(a => !a.IsRetired)
            .OrderBy(a => a.Name)
            .ToList();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Condition Monitoring");

            // Title
            worksheet.Cell(1, 1).Value = "Condition Monitoring Report";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Range(1, 1, 1, 7).Merge();

            worksheet.Cell(2, 1).Value = $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            worksheet.Cell(2, 1).Style.Font.FontSize = 11;

            // Headers
            var headers = new[] { "Asset ID", "Asset Name", "Location", "Status", "Health Score", "Last Reading", "Readings Count" };
            var startRow = 4;
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(startRow, i + 1).Value = headers[i];
            }

            var headerRow = worksheet.Row(startRow);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#0288d1");
            headerRow.Style.Font.FontColor = XLColor.White;

            // Data
            int row = startRow + 1;
            foreach (var asset in assets)
            {
                var readingsCount = context.ConditionReadings.Count(cr => cr.AssetId == asset.Id);
                var lastReading = context.ConditionReadings
                    .Where(cr => cr.AssetId == asset.Id)
                    .OrderByDescending(cr => cr.ReadingDate)
                    .FirstOrDefault();

                worksheet.Cell(row, 1).Value = asset.AssetId;
                worksheet.Cell(row, 2).Value = asset.Name;
                worksheet.Cell(row, 3).Value = asset.Location;
                worksheet.Cell(row, 4).Value = asset.Status;
                worksheet.Cell(row, 5).Value = asset.HealthScore;
                worksheet.Cell(row, 5).Style.NumberFormat.Format = "0.00";
                worksheet.Cell(row, 6).Value = lastReading?.ReadingDate;
                worksheet.Cell(row, 6).Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
                worksheet.Cell(row, 7).Value = readingsCount;

                if (row % 2 == 0)
                {
                    worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#f9f9f9");
                }

                row++;
            }

            worksheet.Columns().AdjustToContents();

            using (var ms = new MemoryStream())
            {
                workbook.SaveAs(ms);
                return ms.ToArray();
            }
        }
    }

    /// <summary>
    /// Export FMEA analysis report
    /// </summary>
    public byte[] ExportFMEAReport()
    {
        using var context = _contextFactory.CreateDbContext();
        var failureModes = context.FailureModes
            .Include(fm => fm.Asset)
            .OrderByDescending(fm => fm.RPN)
            .ToList();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("FMEA Analysis");

            // Title
            worksheet.Cell(1, 1).Value = "FMEA (Failure Mode and Effects Analysis) Report";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Range(1, 1, 1, 8).Merge();

            worksheet.Cell(2, 1).Value = $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            worksheet.Cell(2, 1).Style.Font.FontSize = 11;

            // Headers
            var headers = new[] { "Asset ID", "Failure Mode", "Effect", "Severity", "Occurrence", "Detection", "RPN", "Date" };
            var startRow = 4;
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(startRow, i + 1).Value = headers[i];
            }

            var headerRow = worksheet.Row(startRow);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#0288d1");
            headerRow.Style.Font.FontColor = XLColor.White;

            // Data
            int row = startRow + 1;
            foreach (var fm in failureModes)
            {
                worksheet.Cell(row, 1).Value = fm.Asset?.AssetId;
                worksheet.Cell(row, 2).Value = fm.Mode;
                worksheet.Cell(row, 3).Value = fm.Effect;
                worksheet.Cell(row, 4).Value = fm.Severity;
                worksheet.Cell(row, 5).Value = fm.Occurrence;
                worksheet.Cell(row, 6).Value = fm.Detection;
                worksheet.Cell(row, 7).Value = fm.RPN;
                worksheet.Cell(row, 7).Style.Font.Bold = true;

                // Color code RPN
                if (fm.RPN >= 100)
                    worksheet.Cell(row, 7).Style.Fill.BackgroundColor = XLColor.FromHtml("#ffcdd2");
                else if (fm.RPN >= 50)
                    worksheet.Cell(row, 7).Style.Fill.BackgroundColor = XLColor.FromHtml("#fff9c4");

                worksheet.Cell(row, 8).Value = fm.CreatedDate;
                worksheet.Cell(row, 8).Style.DateFormat.Format = "yyyy-MM-dd";

                if (row % 2 == 0)
                {
                    worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#f9f9f9");
                }

                row++;
            }

            // Add legend
            int legendRow = row + 2;
            worksheet.Cell(legendRow, 1).Value = "RPN Color Legend:";
            worksheet.Cell(legendRow, 1).Style.Font.Bold = true;

            legendRow++;
            worksheet.Cell(legendRow, 1).Value = "High Risk (RPN ? 100)";
            worksheet.Cell(legendRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#ffcdd2");

            legendRow++;
            worksheet.Cell(legendRow, 1).Value = "Medium Risk (RPN ? 50)";
            worksheet.Cell(legendRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#fff9c4");

            worksheet.Columns().AdjustToContents();

            using (var ms = new MemoryStream())
            {
                workbook.SaveAs(ms);
                return ms.ToArray();
            }
        }
    }

    /// <summary>
    /// Export maintenance schedule report
    /// </summary>
    public byte[] ExportMaintenanceScheduleReport()
    {
        using var context = _contextFactory.CreateDbContext();
        var schedules = context.MaintenanceSchedules
            .Include(ms => ms.Asset)
            .OrderBy(ms => ms.ScheduledDate)
            .ToList();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Maintenance Schedule");

            // Title
            worksheet.Cell(1, 1).Value = "Maintenance Schedule Report";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Range(1, 1, 1, 7).Merge();

            worksheet.Cell(2, 1).Value = $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

            // Headers
            var headers = new[] { "Asset ID", "Asset Name", "Scheduled Date", "Type", "Status", "Description", "Frequency" };
            var startRow = 4;
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(startRow, i + 1).Value = headers[i];
            }

            var headerRow = worksheet.Row(startRow);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#0288d1");
            headerRow.Style.Font.FontColor = XLColor.White;

            // Data
            int row = startRow + 1;
            foreach (var schedule in schedules)
            {
                worksheet.Cell(row, 1).Value = schedule.Asset?.AssetId;
                worksheet.Cell(row, 2).Value = schedule.Asset?.Name;
                worksheet.Cell(row, 3).Value = schedule.ScheduledDate;
                worksheet.Cell(row, 3).Style.DateFormat.Format = "yyyy-MM-dd";
                worksheet.Cell(row, 4).Value = schedule.Type;
                worksheet.Cell(row, 5).Value = schedule.Status;
                worksheet.Cell(row, 6).Value = schedule.Description;
                worksheet.Cell(row, 7).Value = schedule.Frequency;

                if (row % 2 == 0)
                {
                    worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#f9f9f9");
                }

                row++;
            }

            worksheet.Columns().AdjustToContents();

            using (var ms = new MemoryStream())
            {
                workbook.SaveAs(ms);
                return ms.ToArray();
            }
        }
    }

    #endregion

    #region Data Import

    /// <summary>
    /// Import assets from CSV
    /// </summary>
    public async Task<(bool Success, int ImportedCount, string Message)> ImportAssetsFromCSVAsync(byte[] fileData)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            using var reader = new StreamReader(new MemoryStream(fileData));
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<AssetImportRecord>().ToList();
            int importedCount = 0;

            foreach (var record in records)
            {
                if (string.IsNullOrWhiteSpace(record.Name)) continue;

                var existingAsset = await context.Assets.FirstOrDefaultAsync(a => a.AssetId == record.AssetId);

                if (existingAsset != null)
                {
                    existingAsset.Name = record.Name;
                    existingAsset.ModelNumber = record.ModelNumber ?? existingAsset.ModelNumber;
                    existingAsset.SerialNumber = record.SerialNumber ?? existingAsset.SerialNumber;
                    existingAsset.EquipmentManufacturer = record.Manufacturer ?? existingAsset.EquipmentManufacturer;
                    existingAsset.Location = record.Location ?? existingAsset.Location;
                    existingAsset.Department = record.Department ?? existingAsset.Department;
                    existingAsset.Criticality = record.Criticality ?? existingAsset.Criticality;
                    existingAsset.Status = record.Status ?? existingAsset.Status;
                }
                else
                {
                    var asset = new Asset
                    {
                        AssetId = record.AssetId ?? $"AST-{DateTime.Now.Ticks}",
                        Name = record.Name,
                        ModelNumber = record.ModelNumber ?? "",
                        SerialNumber = record.SerialNumber ?? "",
                        EquipmentManufacturer = record.Manufacturer ?? "",
                        Location = record.Location ?? "",
                        Department = record.Department ?? "",
                        Criticality = record.Criticality ?? "Medium",
                        Status = record.Status ?? "Active",
                        HealthScore = 100,
                        CreatedDate = DateTime.Now,
                        NextScheduledMaintenance = DateTime.Now.AddDays(30)
                    };
                    context.Assets.Add(asset);
                }

                importedCount++;
            }

            await context.SaveChangesAsync();
            return (true, importedCount, $"Successfully imported {importedCount} assets");
        }
        catch (Exception ex)
        {
            return (false, 0, $"Import failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Import work orders from CSV
    /// </summary>
    public async Task<(bool Success, int ImportedCount, string Message)> ImportWorkOrdersFromCSVAsync(byte[] fileData)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            using var reader = new StreamReader(new MemoryStream(fileData));
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<WorkOrderImportRecord>().ToList();
            int importedCount = 0;

            foreach (var record in records)
            {
                if (string.IsNullOrWhiteSpace(record.AssetId)) continue;

                var asset = await context.Assets.FirstOrDefaultAsync(a => a.AssetId == record.AssetId);
                if (asset == null) continue;

                var existingWO = await context.WorkOrders.FirstOrDefaultAsync(wo => wo.WorkOrderId == record.WorkOrderId);

                if (existingWO != null)
                {
                    existingWO.Type = record.Type ?? existingWO.Type;
                    existingWO.Priority = record.Priority ?? existingWO.Priority;
                    existingWO.Status = record.Status ?? existingWO.Status;
                    existingWO.AssignedTo = record.AssignedTo ?? existingWO.AssignedTo;
                    existingWO.DueDate = record.DueDate;
                    existingWO.Description = record.Description ?? existingWO.Description;
                    if (!string.IsNullOrEmpty(record.CompletedDate) && DateTime.TryParse(record.CompletedDate, out var completedDate))
                    {
                        existingWO.CompletedDate = completedDate;
                    }
                }
                else
                {
                    var workOrder = new WorkOrder
                    {
                        WorkOrderId = record.WorkOrderId ?? $"WO-{DateTime.Now.Ticks}",
                        AssetId = asset.Id,
                        Type = record.Type ?? "Maintenance",
                        Priority = record.Priority ?? "Medium",
                        Status = record.Status ?? "Open",
                        AssignedTo = record.AssignedTo ?? "",
                        DueDate = record.DueDate,
                        Description = record.Description ?? "",
                        CreatedDate = DateTime.Now
                    };

                    if (!string.IsNullOrEmpty(record.CompletedDate) && DateTime.TryParse(record.CompletedDate, out var completed))
                    {
                        workOrder.CompletedDate = completed;
                    }

                    context.WorkOrders.Add(workOrder);
                }

                importedCount++;
            }

            await context.SaveChangesAsync();
            return (true, importedCount, $"Successfully imported {importedCount} work orders");
        }
        catch (Exception ex)
        {
            return (false, 0, $"Import failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Import spare parts from CSV
    /// </summary>
    public async Task<(bool Success, int ImportedCount, string Message)> ImportSparePartsFromCSVAsync(byte[] fileData)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            using var reader = new StreamReader(new MemoryStream(fileData));
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<SparePartImportRecord>().ToList();
            int importedCount = 0;

            foreach (var record in records)
            {
                if (string.IsNullOrWhiteSpace(record.PartNumber)) continue;

                var existingSP = await context.SpareParts.FirstOrDefaultAsync(sp => sp.PartNumber == record.PartNumber);

                if (existingSP != null)
                {
                    existingSP.Description = record.Description ?? existingSP.Description;
                    existingSP.Manufacturer = record.Manufacturer ?? existingSP.Manufacturer;
                    existingSP.QuantityInStock = record.QuantityInStock;
                    existingSP.ReorderPoint = record.ReorderPoint;
                    existingSP.UnitCost = record.UnitCost;
                    existingSP.Status = record.Status ?? existingSP.Status;
                }
                else
                {
                    var sparePart = new SparePart
                    {
                        PartNumber = record.PartNumber,
                        Description = record.Description ?? "",
                        Manufacturer = record.Manufacturer ?? "",
                        QuantityInStock = record.QuantityInStock,
                        ReorderPoint = record.ReorderPoint,
                        UnitCost = record.UnitCost,
                        Status = record.Status ?? "Active",
                        CreatedDate = DateTime.Now
                    };
                    context.SpareParts.Add(sparePart);
                }

                importedCount++;
            }

            await context.SaveChangesAsync();
            return (true, importedCount, $"Successfully imported {importedCount} spare parts");
        }
        catch (Exception ex)
        {
            return (false, 0, $"Import failed: {ex.Message}");
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Generate HTML template for Excel export
    /// </summary>
    private string GenerateExcelHTML(string title, string[] headers)
    {
        var headersHtml = string.Join("", headers.Select(h => $"<th>{EscapeHtml(h)}</th>"));
        
        return $@"
<!DOCTYPE html>
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <meta charset=""utf-8"" />
    <style>
        table {{ border-collapse: collapse; width: 100%; }}
        th {{ background-color: #0288d1; color: white; padding: 10px; text-align: left; border: 1px solid #ddd; }}
        td {{ padding: 8px; border: 1px solid #ddd; }}
        tr:nth-child(even) {{ background-color: #f9f9f9; }}
        tr:hover {{ background-color: #f5f5f5; }}
        h2 {{ color: #333; margin-bottom: 20px; }}
        .export-info {{ font-size: 12px; color: #666; margin-bottom: 10px; }}
    </style>
</head>
<body>
    <h2>{title}</h2>
    <div class=""export-info"">Exported on {DateTime.Now:yyyy-MM-dd HH:mm:ss}</div>
    <table>
        <thead>
            <tr>
                {headersHtml}
            </tr>
        </thead>
        <tbody>
            {{CONTENT}}
        </tbody>
    </table>
</body>
</html>
";
    }

    /// <summary>
    /// Escape HTML special characters
    /// </summary>
    private string EscapeHtml(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return "";
        
        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");
    }

    /// <summary>
    /// Generate filename with timestamp
    /// </summary>
    public string GenerateExportFilename(string dataType, string format)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var extension = format.ToLower() switch
        {
            "excel" => "xlsx",
            _ => format.ToLower()
        };
        return $"{dataType}_{timestamp}.{extension}";
    }

    /// <summary>
    /// Get MIME type for file format
    /// </summary>
    public string GetMimeType(string format)
    {
        return format.ToLower() switch
        {
            "csv" => "text/csv",
            "json" => "application/json",
            "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "xls" => "application/vnd.ms-excel",
            "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "html" => "text/html",
            _ => "application/octet-stream"
        };
    }

    #endregion

    #region Summary Statistics

    /// <summary>
    /// Generate export summary with statistics (async with tenant filtering)
    /// </summary>
    public async Task<ExportSummary> GetExportSummaryAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        var (isSuperAdmin, tenantId) = await GetTenantContextAsync();
        
        var assetsQuery = context.Assets.Where(a => !a.IsRetired);
        var workOrdersQuery = context.WorkOrders.AsQueryable();
        var sparePartsQuery = context.SpareParts.AsQueryable();
        var documentsQuery = context.Documents.AsQueryable();
        var failureModesQuery = context.FailureModes.AsQueryable();
        
        if (!isSuperAdmin && tenantId.HasValue)
        {
            assetsQuery = assetsQuery.Where(a => a.TenantId == tenantId);
            workOrdersQuery = workOrdersQuery.Where(w => w.TenantId == tenantId);
            sparePartsQuery = sparePartsQuery.Where(sp => sp.TenantId == tenantId);
            documentsQuery = documentsQuery.Where(d => d.TenantId == tenantId);
            failureModesQuery = failureModesQuery.Where(fm => fm.TenantId == tenantId);
        }
        
        return new ExportSummary
        {
            TotalAssets = await assetsQuery.CountAsync(),
            TotalWorkOrders = await workOrdersQuery.CountAsync(),
            TotalSpareParts = await sparePartsQuery.CountAsync(),
            TotalDocuments = await documentsQuery.CountAsync(),
            TotalFailureModes = await failureModesQuery.CountAsync(),
            CriticalAssets = await assetsQuery.CountAsync(a => a.Status == "Critical"),
            OverdueWorkOrders = await workOrdersQuery.CountAsync(wo => wo.Status != "Completed" && wo.DueDate < DateTime.Now),
            LowStockItems = await sparePartsQuery.CountAsync(sp => sp.QuantityInStock <= sp.ReorderPoint),
            ExpiredDocuments = await documentsQuery.CountAsync(d => d.ExpiryDate.HasValue && d.ExpiryDate.Value < DateTime.Now),
            ExportDate = DateTime.Now,
            ExportedBy = "System"
        };
    }

    /// <summary>
    /// Generate export summary with statistics (synchronous - no tenant filtering)
    /// </summary>
    public ExportSummary GetExportSummary()
    {
        using var context = _contextFactory.CreateDbContext();
        
        return new ExportSummary
        {
            TotalAssets = context.Assets.Count(a => !a.IsRetired),
            TotalWorkOrders = context.WorkOrders.Count(),
            TotalSpareParts = context.SpareParts.Count(),
            TotalDocuments = context.Documents.Count(),
            TotalFailureModes = context.FailureModes.Count(),
            CriticalAssets = context.Assets.Count(a => !a.IsRetired && a.Status == "Critical"),
            OverdueWorkOrders = context.WorkOrders.Count(wo => wo.Status != "Completed" && wo.DueDate < DateTime.Now),
            LowStockItems = context.SpareParts.Count(sp => sp.QuantityInStock <= sp.ReorderPoint),
            ExpiredDocuments = context.Documents.Count(d => d.ExpiryDate.HasValue && d.ExpiryDate.Value < DateTime.Now),
            ExportDate = DateTime.Now,
            ExportedBy = "System"
        };
    }

    #endregion
}

/// <summary>
/// Export summary statistics
/// </summary>
public class ExportSummary
{
    public int TotalAssets { get; set; }
    public int TotalWorkOrders { get; set; }
    public int TotalSpareParts { get; set; }
    public int TotalDocuments { get; set; }
    public int TotalFailureModes { get; set; }
    public int CriticalAssets { get; set; }
    public int OverdueWorkOrders { get; set; }
    public int LowStockItems { get; set; }
    public int ExpiredDocuments { get; set; }
    public DateTime ExportDate { get; set; }
    public string ExportedBy { get; set; } = string.Empty;
}
