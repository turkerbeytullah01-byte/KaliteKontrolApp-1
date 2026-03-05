using KaliteKontrolApp.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KaliteKontrolApp.Utils;

public static class ExcelHelper
{
    static ExcelHelper()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }
    
    public static void ExportPlansToExcel(List<QualityPlan> plans, string filePath)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Kalite Planları");
        
        // Header
        var headers = new[] { "ID", "Ürün Adı", "Ürün Kodu", "Müşteri", "Çizim No", "Balon Sayısı", "Oluşturma Tarihi" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = headers[i];
            worksheet.Cells[1, i + 1].Style.Font.Bold = true;
            worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(ThemeColors.Primary);
            worksheet.Cells[1, i + 1].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells[1, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }
        
        // Data
        for (int i = 0; i < plans.Count; i++)
        {
            var plan = plans[i];
            worksheet.Cells[i + 2, 1].Value = plan.Id;
            worksheet.Cells[i + 2, 2].Value = plan.ProductName;
            worksheet.Cells[i + 2, 3].Value = plan.ProductCode;
            worksheet.Cells[i + 2, 4].Value = plan.Customer;
            worksheet.Cells[i + 2, 5].Value = plan.DrawingNo;
            worksheet.Cells[i + 2, 6].Value = plan.BalloonCount;
            worksheet.Cells[i + 2, 7].Value = plan.CreatedAt.ToString("dd.MM.yyyy");
        }
        
        // Auto fit columns
        worksheet.Cells.AutoFitColumns();
        
        package.SaveAs(new FileInfo(filePath));
    }
    
    public static void ExportMeasurementsToExcel(List<Measurement> measurements, string filePath)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Ölçümler");
        
        // Header
        var headers = new[] { "ID", "Ürün Adı", "Ürün Kodu", "Kontrol Tarihi", "Fatura No", "Parti No", 
            "Kontrol Eden", "Onaylayan", "Kalite Tipi", "Genel Sonuç" };
        
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = headers[i];
            worksheet.Cells[1, i + 1].Style.Font.Bold = true;
            worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(ThemeColors.Primary);
            worksheet.Cells[1, i + 1].Style.Font.Color.SetColor(Color.White);
        }
        
        // Data
        for (int i = 0; i < measurements.Count; i++)
        {
            var m = measurements[i];
            worksheet.Cells[i + 2, 1].Value = m.Id;
            worksheet.Cells[i + 2, 2].Value = m.Plan?.ProductName;
            worksheet.Cells[i + 2, 3].Value = m.Plan?.ProductCode;
            worksheet.Cells[i + 2, 4].Value = m.ControlDate;
            worksheet.Cells[i + 2, 5].Value = m.InvoiceNo;
            worksheet.Cells[i + 2, 6].Value = m.BatchNo;
            worksheet.Cells[i + 2, 7].Value = m.ControlledBy;
            worksheet.Cells[i + 2, 8].Value = m.ApprovedBy;
            worksheet.Cells[i + 2, 9].Value = GetQualityTypeText(m.QualityType);
            worksheet.Cells[i + 2, 10].Value = m.OverallResult;
            
            // Color coding for result
            var resultCell = worksheet.Cells[i + 2, 10];
            switch (m.OverallResult)
            {
                case "OK":
                    resultCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    resultCell.Style.Fill.BackgroundColor.SetColor(ThemeColors.SuccessLight);
                    resultCell.Style.Font.Color.SetColor(ThemeColors.Success);
                    break;
                case "NOK":
                    resultCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    resultCell.Style.Fill.BackgroundColor.SetColor(ThemeColors.ErrorLight);
                    resultCell.Style.Font.Color.SetColor(ThemeColors.Error);
                    break;
            }
        }
        
        worksheet.Cells.AutoFitColumns();
        package.SaveAs(new FileInfo(filePath));
    }
    
    public static void ExportMeasurementReport(Measurement measurement, QualityPlan plan, string filePath)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Ölçüm Raporu");
        
        int row = 1;
        
        // Title
        worksheet.Cells[row, 1].Value = "KALİTE KONTROL ÖLÇÜM RAPORU";
        worksheet.Cells[row, 1].Style.Font.Bold = true;
        worksheet.Cells[row, 1].Style.Font.Size = 16;
        worksheet.Cells[row, 1, row, 6].Merge = true;
        worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        row += 2;
        
        // Product info
        worksheet.Cells[row, 1].Value = "Ürün Adı:";
        worksheet.Cells[row, 2].Value = plan.ProductName;
        worksheet.Cells[row, 4].Value = "Ürün Kodu:";
        worksheet.Cells[row, 5].Value = plan.ProductCode;
        row++;
        
        worksheet.Cells[row, 1].Value = "Müşteri:";
        worksheet.Cells[row, 2].Value = plan.Customer;
        worksheet.Cells[row, 4].Value = "Çizim No:";
        worksheet.Cells[row, 5].Value = plan.DrawingNo;
        row++;
        
        worksheet.Cells[row, 1].Value = "Kontrol Tarihi:";
        worksheet.Cells[row, 2].Value = measurement.ControlDate;
        worksheet.Cells[row, 4].Value = "Fatura No:";
        worksheet.Cells[row, 5].Value = measurement.InvoiceNo;
        row++;
        
        worksheet.Cells[row, 1].Value = "Parti No:";
        worksheet.Cells[row, 2].Value = measurement.BatchNo;
        worksheet.Cells[row, 4].Value = "Kalite Tipi:";
        worksheet.Cells[row, 5].Value = GetQualityTypeText(measurement.QualityType);
        row += 2;
        
        // Measurement details header
        var detailHeaders = new[] { "Balon No", "Ölçü", "Nominal", "Alt Tolerans", "Üst Tolerans", 
            "Ölçülen Değer", "Sonuç" };
        
        for (int i = 0; i < detailHeaders.Length; i++)
        {
            worksheet.Cells[row, i + 1].Value = detailHeaders[i];
            worksheet.Cells[row, i + 1].Style.Font.Bold = true;
            worksheet.Cells[row, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[row, i + 1].Style.Fill.BackgroundColor.SetColor(ThemeColors.Primary);
            worksheet.Cells[row, i + 1].Style.Font.Color.SetColor(Color.White);
        }
        row++;
        
        // Measurement details
        foreach (var detail in measurement.Details)
        {
            var point = plan.MeasurementPoints.FirstOrDefault(p => p.Id == detail.MeasurementPointId);
            if (point != null)
            {
                worksheet.Cells[row, 1].Value = point.BalloonNo;
                worksheet.Cells[row, 2].Value = point.Dimension;
                worksheet.Cells[row, 3].Value = point.NominalValue;
                worksheet.Cells[row, 4].Value = point.LowerTolerance;
                worksheet.Cells[row, 5].Value = point.UpperTolerance;
                worksheet.Cells[row, 6].Value = detail.MeasuredValue;
                worksheet.Cells[row, 7].Value = detail.Result;
                
                // Color coding
                var resultCell = worksheet.Cells[row, 7];
                switch (detail.Result)
                {
                    case "OK":
                        resultCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        resultCell.Style.Fill.BackgroundColor.SetColor(ThemeColors.SuccessLight);
                        resultCell.Style.Font.Color.SetColor(ThemeColors.Success);
                        break;
                    case "NOK":
                        resultCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        resultCell.Style.Fill.BackgroundColor.SetColor(ThemeColors.ErrorLight);
                        resultCell.Style.Font.Color.SetColor(ThemeColors.Error);
                        break;
                    case "CONDITIONAL":
                        resultCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        resultCell.Style.Fill.BackgroundColor.SetColor(ThemeColors.WarningLight);
                        resultCell.Style.Font.Color.SetColor(ThemeColors.Warning);
                        break;
                }
                row++;
            }
        }
        
        // Signatures
        row += 2;
        worksheet.Cells[row, 1].Value = "Kontrol Eden:";
        worksheet.Cells[row, 2].Value = measurement.ControlledBy;
        worksheet.Cells[row, 4].Value = "Onaylayan:";
        worksheet.Cells[row, 5].Value = measurement.ApprovedBy;
        
        worksheet.Cells.AutoFitColumns();
        package.SaveAs(new FileInfo(filePath));
    }
    
    public static List<QualityPlan> ImportPlansFromExcel(string filePath)
    {
        var plans = new List<QualityPlan>();
        
        using var package = new ExcelPackage(new FileInfo(filePath));
        var worksheet = package.Workbook.Worksheets[0];
        
        // Start from row 2 (skip header)
        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
        {
            var plan = new QualityPlan
            {
                ProductName = worksheet.Cells[row, 2].Text,
                ProductCode = worksheet.Cells[row, 3].Text,
                Customer = worksheet.Cells[row, 4].Text,
                DrawingNo = worksheet.Cells[row, 5].Text,
                BalloonCount = int.TryParse(worksheet.Cells[row, 6].Text, out int count) ? count : 10
            };
            
            plans.Add(plan);
        }
        
        return plans;
    }
    
    private static string GetQualityTypeText(string type)
    {
        return type switch
        {
            "final" => "Son Kontrol",
            "first" => "İlk Kontrol",
            "process" => "Proses Kontrol",
            _ => type
        };
    }
}
