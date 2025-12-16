# Maintenance Schedule Export - Quick Start Guide

## How to Use Export Feature

### Accessing Export Options
1. Navigate to the Maintenance Planning page (`/rbm/maintenance-planning`)
2. Look for the export buttons in the toolbar: **Excel | Word | PDF**

### Exporting Schedules

#### Step 1: Apply Filters (Optional)
Filter schedules by:
- **Type**: Preventive, Corrective, or Inspection
- **Status**: Scheduled, In Progress, Completed, or Cancelled
- **Technician**: Select a specific technician

#### Step 2: Click Export Button
Choose your preferred format:
- **Excel** - For spreadsheet analysis with Excel (best for data analysis)
- **Word** - For document sharing in CSV format (editable in Word)
- **PDF** - For printing and sharing (best for reports)

#### Step 3: File Download
- File will automatically download to your default downloads folder
- File name includes timestamp: `MaintenanceSchedules_YYYYMMDD_HHMMSS.{extension}`

### What Gets Exported

All filtered schedules with the following information:
- Asset Name
- Maintenance Type (Preventive/Corrective/Inspection)
- Scheduled Date & Time
- Estimated Duration (hours)
- Assigned Technician Name
- Schedule Status

### File Formats

#### Excel (.xlsx)
- Professional spreadsheet format
- Colored header row (blue background)
- Alternating row colors
- Summary section with total count
- Best for: Data analysis, filtering, calculations

#### Word (.csv)
- CSV format (comma-separated values)
- Compatible with MS Word, Excel, and text editors
- Includes report metadata
- Best for: Quick sharing, document embedding

#### PDF (.pdf)
- Professional report format
- Read-only (prevents accidental changes)
- Table with formatted headers
- Best for: Printing, archiving, sharing with stakeholders

### Tips & Best Practices

1. **Filter Before Export**: Use filters to export only relevant schedules
2. **Use Excel for Analysis**: Excel format is best for sorting and analysis
3. **Use PDF for Reports**: PDF format is ideal for formal reports
4. **Check File Size**: Large exports may take a few seconds
5. **Backup Regular**: Schedule regular exports as backup

### Troubleshooting

| Issue | Solution |
|-------|----------|
| No schedules to export | Ensure there are schedules matching your filters |
| Export button not working | Check browser console for errors; clear cache |
| File won't open | Ensure you have correct application installed (Excel, Word, PDF reader) |
| Formatting issues | PDF format provides most consistent formatting |

### Examples

#### Example 1: Export Upcoming Preventive Maintenance
1. Set "Filter by Type" to "Preventive"
2. Set "Filter by Status" to "Scheduled"
3. Click **Excel** to download for analysis

#### Example 2: Export Technician's Assignments
1. Set "Filter by Technician" to desired technician name
2. Click **PDF** to create a report for that technician

#### Example 3: Export All Current Schedules
1. Leave all filters blank (All Types, All Status, All Technicians)
2. Click **Excel** for comprehensive spreadsheet view

---

For issues or feature requests, contact your system administrator.
