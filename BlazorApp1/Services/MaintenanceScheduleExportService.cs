using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp1.Models;
using iText.Kernel.Pdf;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Layout;
using iText.Layout.Element;
using OfficeOpenXml;

namespace BlazorApp1.Services
{
    public class MaintenanceScheduleExportService
    {
        public async Task<byte[]> ExportToExcelAsync(List<MaintenanceSchedule> schedules)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Schedules");

                    worksheet.Column(1).Width = 20;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 25;
                    worksheet.Column(4).Width = 12;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 12;

                    var headerRow = worksheet.Row(1);
                    headerRow.Height = 25;
                    
                    var headers = new[] { "Asset Name", "Type", "Scheduled Date", "Duration (hrs)", "Technician", "Status", "Frequency" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = worksheet.Cells[1, i + 1];
                        cell.Value = headers[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(70, 130, 180));
                        cell.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    }

                    for (int i = 0; i < schedules.Count; i++)
                    {
                        var schedule = schedules[i];
                        var rowNum = i + 2;

                        worksheet.Cells[rowNum, 1].Value = schedule.AssetName;
                        worksheet.Cells[rowNum, 2].Value = schedule.Type;
                        worksheet.Cells[rowNum, 3].Value = schedule.ScheduledDate.ToString("MMM dd, yyyy HH:mm");
                        worksheet.Cells[rowNum, 4].Value = schedule.EstimatedDuration;
                        worksheet.Cells[rowNum, 5].Value = schedule.AssignedTechnician;
                        worksheet.Cells[rowNum, 6].Value = schedule.Status;
                        worksheet.Cells[rowNum, 7].Value = schedule.Frequency ?? "";

                        if (i % 2 == 0)
                        {
                            for (int j = 1; j <= 7; j++)
                            {
                                worksheet.Cells[rowNum, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                worksheet.Cells[rowNum, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(245, 245, 245));
                            }
                        }
                    }

                    var summaryRow = schedules.Count + 3;
                    worksheet.Cells[summaryRow, 1].Value = "Total:";
                    worksheet.Cells[summaryRow, 2].Value = schedules.Count;
                    worksheet.Cells[summaryRow, 1].Style.Font.Bold = true;

                    return await Task.FromResult(package.GetAsByteArray());
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error exporting to Excel: {ex.Message}", ex);
            }
        }

        public async Task<byte[]> ExportToWordAsync(List<MaintenanceSchedule> schedules)
        {
            try
            {
                var csv = new System.Text.StringBuilder();
                csv.AppendLine("MAINTENANCE SCHEDULES REPORT");
                csv.AppendLine($"Generated on: {DateTime.Now:MMM dd, yyyy HH:mm}");
                csv.AppendLine($"Total Schedules: {schedules.Count}");
                csv.AppendLine();
                csv.AppendLine("Asset Name,Type,Scheduled Date,Duration (hrs),Technician,Status,Frequency");
                
                foreach (var schedule in schedules)
                {
                    csv.AppendLine($"{schedule.AssetName},{schedule.Type},{schedule.ScheduledDate:MMM dd yyyy HH:mm},{schedule.EstimatedDuration},{schedule.AssignedTechnician},{schedule.Status},{schedule.Frequency ?? ""}");
                }

                return await Task.FromResult(System.Text.Encoding.UTF8.GetBytes(csv.ToString()));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error exporting to Word: {ex.Message}", ex);
            }
        }

        public async Task<byte[]> ExportToPdfAsync(List<MaintenanceSchedule> schedules)
        {
            try
            {
                return await Task.Run(() =>
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        var writerProperties = new WriterProperties();
                        writerProperties.SetCompressionLevel(CompressionConstants.DEFAULT_COMPRESSION);
                        
                        using (var writer = new PdfWriter(memoryStream, writerProperties))
                        {
                            using (var pdfDocument = new PdfDocument(writer))
                            {
                                var document = new iText.Layout.Document(pdfDocument);
                                var defaultFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                                document.Add(new Paragraph("Maintenance Schedules Report").SetFont(defaultFont)
                                    .SetFontSize(20));

                                document.Add(new Paragraph($"Generated on: {DateTime.Now:MMM dd, yyyy HH:mm}").SetFont(defaultFont)
                                    .SetFontSize(10));

                                document.Add(new Paragraph($"Total Schedules: {schedules.Count}").SetFont(defaultFont)
                                    .SetFontSize(10));

                                document.Add(new Paragraph(" ").SetFont(defaultFont));

                                var table = new iText.Layout.Element.Table(7)
                                    .SetWidth(iText.Layout.Properties.UnitValue.CreatePercentValue(100));

                                var headers = new[] { "Asset Name", "Type", "Scheduled Date", "Duration (hrs)", "Technician", "Status", "Frequency" };
                                foreach (var header in headers)
                                {
                                    var cell = new Cell()
                                        .Add(new Paragraph(header).SetFont(defaultFont).SetFontSize(9))
                                        .SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(70, 130, 180));
                                    table.AddHeaderCell(cell);
                                }

                                foreach (var schedule in schedules)
                                {
                                    table.AddCell(new Cell().Add(new Paragraph(schedule.AssetName ?? string.Empty).SetFont(defaultFont).SetFontSize(8)));
                                    table.AddCell(new Cell().Add(new Paragraph(schedule.Type ?? string.Empty).SetFont(defaultFont).SetFontSize(8)));
                                    table.AddCell(new Cell().Add(new Paragraph(schedule.ScheduledDate.ToString("MMM dd, yyyy HH:mm")).SetFont(defaultFont).SetFontSize(8)));
                                    table.AddCell(new Cell().Add(new Paragraph(schedule.EstimatedDuration.ToString("F1")).SetFont(defaultFont).SetFontSize(8)));
                                    table.AddCell(new Cell().Add(new Paragraph(schedule.AssignedTechnician ?? string.Empty).SetFont(defaultFont).SetFontSize(8)));
                                    table.AddCell(new Cell().Add(new Paragraph(schedule.Status ?? string.Empty).SetFont(defaultFont).SetFontSize(8)));
                                    table.AddCell(new Cell().Add(new Paragraph(schedule.Frequency ?? string.Empty).SetFont(defaultFont).SetFontSize(8)));
                                }

                                document.Add(table);
                                document.Close();
                            }
                        }

                        return memoryStream.ToArray();
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error exporting to PDF: {ex.Message}", ex);
            }
        }

        public async Task<byte[]> ExportToCalendarPdfAsync(List<MaintenanceSchedule> schedules)
        {
            try
            {
                if (schedules == null || schedules.Count == 0)
                    return new byte[0];

                return await Task.Run(() =>
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        var writerProperties = new WriterProperties();
                        writerProperties.SetCompressionLevel(CompressionConstants.DEFAULT_COMPRESSION);
                        
                        using (var writer = new PdfWriter(memoryStream, writerProperties))
                        {
                            using (var pdfDocument = new PdfDocument(writer))
                            {
                                var document = new iText.Layout.Document(pdfDocument);
                                var defaultFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                                // Title
                                document.Add(new Paragraph("Maintenance Schedule Calendar").SetFont(defaultFont)
                                    .SetFontSize(18));

                                document.Add(new Paragraph($"Generated on: {DateTime.Now:MMMM yyyy}").SetFont(defaultFont)
                                    .SetFontSize(11));

                                document.Add(new Paragraph($"Total Schedules: {schedules.Count}").SetFont(defaultFont)
                                    .SetFontSize(10));

                                document.Add(new Paragraph(" ").SetFont(defaultFont));

                                // Group by month
                                var groupedByMonth = schedules
                                    .GroupBy(s => new { s.ScheduledDate.Year, s.ScheduledDate.Month })
                                    .OrderBy(g => g.Key.Year)
                                    .ThenBy(g => g.Key.Month);

                                foreach (var monthGroup in groupedByMonth)
                                {
                                    var year = monthGroup.Key.Year;
                                    var month = monthGroup.Key.Month;
                                    var monthDate = new DateTime(year, month, 1);

                                    document.Add(new Paragraph($"{monthDate:MMMM yyyy}")
                                        .SetFontSize(14));

                                    // Create calendar table (7 columns for days of week)
                                    var calendarTable = new iText.Layout.Element.Table(7)
                                        .SetWidth(iText.Layout.Properties.UnitValue.CreatePercentValue(100));

                                    // Day headers
                                    string[] dayNames = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
                                    foreach (var dayName in dayNames)
                                    {
                                        var cell = new Cell()
                                            .Add(new Paragraph(dayName).SetFont(defaultFont).SetFontSize(10))
                                            .SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(100, 149, 237))
                                            .SetMinHeight(20);
                                        calendarTable.AddHeaderCell(cell);
                                    }

                                    // Calendar days
                                    var firstDay = new DateTime(year, month, 1);
                                    var lastDay = firstDay.AddMonths(1).AddDays(-1);
                                    var startDate = firstDay.AddDays(-(int)firstDay.DayOfWeek);
                                    var endDate = lastDay.AddDays(6 - (int)lastDay.DayOfWeek);

                                    for (var date = startDate; date <= endDate; date = date.AddDays(1))
                                    {
                                        var daySchedules = schedules.Where(s => s.ScheduledDate.Date == date.Date).ToList();
                                        var isCurrentMonth = date.Month == month;

                                        var dayCell = new Cell()
                                            .SetMinHeight(60);

                                        if (!isCurrentMonth)
                                            dayCell.SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(240, 240, 240));

                                        // Day number
                                        dayCell.Add(new Paragraph(date.Day.ToString()).SetFont(defaultFont)
                                            .SetFontSize(11));

                                        // Add schedules for this day
                                        foreach (var schedule in daySchedules)
                                        {
                                            dayCell.Add(new Paragraph(" "));
                                            var scheduleText = $"{schedule.AssetName}\n{schedule.Type}\n{schedule.AssignedTechnician}";
                                            dayCell.Add(new Paragraph(scheduleText).SetFont(defaultFont)
                                                .SetFontSize(7));

                                            if (!string.IsNullOrEmpty(schedule.Description))
                                            {
                                                var description = schedule.Description.Length > 50 
                                                    ? schedule.Description.Substring(0, 50) + "..." 
                                                    : schedule.Description;
                                                dayCell.Add(new Paragraph(description).SetFont(defaultFont)
                                                    .SetFontSize(6));
                                            }
                                        }

                                        calendarTable.AddCell(dayCell);
                                    }

                                    document.Add(calendarTable);
                                    document.Add(new Paragraph(" "));

                                    // Add details section for this month
                                    document.Add(new Paragraph("Schedule Details").SetFont(defaultFont)
                                        .SetFontSize(12));

                                    var detailsTable = new iText.Layout.Element.Table(5)
                                        .SetWidth(iText.Layout.Properties.UnitValue.CreatePercentValue(100));

                                    // Details headers
                                    var detailHeaders = new[] { "Date", "Asset", "Type", "Technician", "Description" };
                                    foreach (var header in detailHeaders)
                                    {
                                        var cell = new Cell()
                                            .Add(new Paragraph(header).SetFont(defaultFont).SetFontSize(9))
                                            .SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(70, 130, 180));
                                        detailsTable.AddHeaderCell(cell);
                                    }

                                    // Add schedule details
                                    foreach (var schedule in monthGroup.OrderBy(s => s.ScheduledDate))
                                    {
                                        detailsTable.AddCell(new Cell().Add(new Paragraph(schedule.ScheduledDate.ToString("MMM dd, HH:mm")).SetFont(defaultFont).SetFontSize(8)));
                                        detailsTable.AddCell(new Cell().Add(new Paragraph(schedule.AssetName ?? string.Empty).SetFont(defaultFont).SetFontSize(8)));
                                        detailsTable.AddCell(new Cell().Add(new Paragraph(schedule.Type ?? string.Empty).SetFont(defaultFont).SetFontSize(8)));
                                        detailsTable.AddCell(new Cell().Add(new Paragraph(schedule.AssignedTechnician ?? string.Empty).SetFont(defaultFont).SetFontSize(8)));

                                        var description = string.IsNullOrEmpty(schedule.Description) 
                                            ? "N/A" 
                                            : (schedule.Description.Length > 100 
                                                ? schedule.Description.Substring(0, 100) + "..." 
                                                : schedule.Description);
                                        detailsTable.AddCell(new Cell().Add(new Paragraph(description).SetFont(defaultFont).SetFontSize(7)));
                                    }

                                    document.Add(detailsTable);
                                    document.Add(new Paragraph("\n"));
                                }

                                document.Close();
                            }
                        }

                        return memoryStream.ToArray();
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error exporting calendar PDF: {ex.Message}", ex);
            }
        }
    }
}
