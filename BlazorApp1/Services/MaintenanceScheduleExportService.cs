using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp1.Models;
using iText.Kernel.Pdf;
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
                        using (var writer = new PdfWriter(memoryStream))
                        {
                            using (var pdfDocument = new PdfDocument(writer))
                            {
                                var document = new iText.Layout.Document(pdfDocument);

                                document.Add(new Paragraph("Maintenance Schedules Report")
                                    .SetFontSize(20));

                                document.Add(new Paragraph($"Generated on: {DateTime.Now:MMM dd, yyyy HH:mm}")
                                    .SetFontSize(10));

                                document.Add(new Paragraph($"Total Schedules: {schedules.Count}")
                                    .SetFontSize(10));

                                document.Add(new Paragraph(" "));

                                var table = new iText.Layout.Element.Table(7)
                                    .SetWidth(iText.Layout.Properties.UnitValue.CreatePercentValue(100));

                                var headers = new[] { "Asset Name", "Type", "Scheduled Date", "Duration (hrs)", "Technician", "Status", "Frequency" };
                                foreach (var header in headers)
                                {
                                    var cell = new Cell()
                                        .Add(new Paragraph(header).SetFontSize(9))
                                        .SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(70, 130, 180));
                                    table.AddHeaderCell(cell);
                                }

                                foreach (var schedule in schedules)
                                {
                                    table.AddCell(new Cell().Add(new Paragraph(schedule.AssetName).SetFontSize(8)));
                                    table.AddCell(new Cell().Add(new Paragraph(schedule.Type).SetFontSize(8)));
                                    table.AddCell(new Cell().Add(new Paragraph(schedule.ScheduledDate.ToString("MMM dd, yyyy HH:mm")).SetFontSize(8)));
                                    table.AddCell(new Cell().Add(new Paragraph(schedule.EstimatedDuration.ToString("F1")).SetFontSize(8)));
                                    table.AddCell(new Cell().Add(new Paragraph(schedule.AssignedTechnician).SetFontSize(8)));
                                    table.AddCell(new Cell().Add(new Paragraph(schedule.Status).SetFontSize(8)));
                                    table.AddCell(new Cell().Add(new Paragraph(schedule.Frequency ?? "").SetFontSize(8)));
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
    }
}
