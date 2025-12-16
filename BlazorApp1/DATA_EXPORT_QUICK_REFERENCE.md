# Data Export - Quick Reference Guide

## ?? Quick Start

### Access the Feature
1. Login as Admin
2. Click **"Data Export"** in sidebar (under admin section)
3. URL: `/rbm/export`

## ?? Export Types

| Format | Best For | File Type |
|--------|----------|-----------|
| **CSV** | Analysis, universal compatibility | .csv |
| **Excel** | Reports, presentations | .html (opens in Excel) |
| **JSON** | API integration, data pipelines | .json |

## ?? Available Exports

### CSV Files
- ? **Assets** - Asset inventory with status
- ? **Work Orders** - Maintenance work orders
- ? **Spare Parts** - Inventory with stock levels
- ? **Failure Modes** - FMEA analysis data
- ? **Documents** - Document metadata
- ? **Condition Readings** - Sensor data

### Excel Reports
- ?? **Assets** - Formatted asset report
- ?? **Work Orders** - Formatted work order report
- ?? **Spare Parts** - Inventory report with totals

### JSON Data
- ?? **Assets** - Asset data in JSON
- ?? **Work Orders** - Work order data in JSON
- ?? **All Data** - Complete database dump

## ?? Dashboard Summary

Real-time statistics:
- **Total Assets** - Count of active assets
- **Work Orders** - Total work orders
- **Spare Parts** - Inventory item count
- **Documents** - Total documents managed
- **Critical Assets** ?? - Critical status count
- **Overdue WOs** ?? - Past-due maintenance items

## ?? File Naming

All exports include timestamp:
```
[DataType]_[YYYYMMDD]_[HHMMSS].[format]

Example:
Assets_20251220_143022.csv
WorkOrders_20251220_143022.json
```

## ?? Common Tasks

### Export for Excel Analysis
1. Click **Assets** under CSV Export
2. Open file in Excel
3. Use filters and formulas

### Export for Backup
1. Click **All Data** under JSON Export
2. Save in secure location
3. Contains complete database snapshot

### Export for Integration
1. Click **Work Orders** under JSON Export
2. Use JSON in API calls
3. Parse with your system

## ?? Access Control

- **Admin Only** ?
- **Other Roles** ? (Access Denied)

Only users with **Admin** role can access this feature.

## ?? Technical Details

**Service**: `DataExportService`
**Component**: `DataExport.razor`
**JavaScript**: `export-module.js`
**Dependency Injection**: Registered in `Program.cs`

## ?? Troubleshooting

| Issue | Solution |
|-------|----------|
| Button disabled | Wait for current export |
| No file download | Check popup blocker |
| CSV encoding issues | Open as UTF-8 |
| Slow export | Normal for large datasets |
| Access denied | Need Admin role |

## ?? Data Included

### Assets Export
- ID, Name, Model, Manufacturer, Location
- Department, Criticality, Status, Health Score
- Last & Next Maintenance, Created Date

### Work Orders Export
- ID, Asset ID, Type, Priority, Status
- Assigned To, Due Date, Completion Date
- Full Description

### Spare Parts Export
- Part Number, Description, Manufacturer
- Stock Quantity, Reorder Point, Unit Cost
- Total Value, Stock Status, Last Used

## ?? Workflow Example

**Export for Monthly Report:**
1. Navigate to `/rbm/export`
2. Check dashboard summary
3. Click "Assets" ? Excel Export
4. Download file
5. Open in Excel
6. Create charts/analysis
7. Share with management

**Export for System Backup:**
1. Click "All Data" ? JSON Export
2. Download complete data snapshot
3. Store securely
4. Use for disaster recovery

## ?? Support

For issues:
- Check Admin role assignment
- Verify database connectivity
- Review browser console for errors
- Check application logs

## ?? UI Layout

```
???????????????????????????????????
? ?? Data Export                  ?
???????????????????????????????????
? Summary: Assets, WOs, Parts,... ?
???????????????????????????????????
? CSV Export ? Excel Export ? JSON?
? ??????????? ???????????? ???????
? ? Assets  ? ? Assets   ? ?All ??
? ? WOs     ? ? WOs      ? ?DF ??
? ? Parts   ? ? Parts    ? ?    ??
? ? Failure ? ?          ? ?    ??
? ? Docs    ? ?          ? ?    ??
? ??????????? ???????????? ???????
???????????????????????????????????
```

## ?? Important Notes

1. **Data Freshness** - Exports run in real-time from current database
2. **Large Datasets** - May take a few seconds for large exports
3. **File Size** - JSON "All Data" export can be large
4. **No Sensitive Data** - All exports are audit-logged
5. **Concurrent Exports** - Multiple users can export simultaneously

## ?? Best Practices

? **DO:**
- Use JSON for automated systems
- Use Excel for presentations
- Use CSV for analysis
- Export regularly for backups
- Store exports securely

? **DON'T:**
- Share exports with unauthorized users
- Store exports in insecure locations
- Rely on exports as sole backup
- Export sensitive data unnecessarily
- Leave exports on public terminals

---

**Last Updated**: December 20, 2024
**Version**: 1.0
**Status**: ? Production Ready
