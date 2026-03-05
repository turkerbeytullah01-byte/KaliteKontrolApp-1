using System;
using System.Collections.Generic;

namespace KaliteKontrolApp.Models;

public class Measurement
{
    public int Id { get; set; }
    public int PlanId { get; set; }
    public string ControlDate { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
    public string InvoiceNo { get; set; } = string.Empty;
    public string DeviceCodes { get; set; } = string.Empty;
    public string ControlledBy { get; set; } = string.Empty;
    public string ApprovedBy { get; set; } = string.Empty;
    public string BatchNo { get; set; } = string.Empty;
    public string QualityType { get; set; } = "final"; // final, first, process
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string OverallResult { get; set; } = "Beklemede"; // OK, NOK, CONDITIONAL, Beklemede
    
    // Navigation properties
    public QualityPlan? Plan { get; set; }
    public List<MeasurementDetail> Details { get; set; } = new();
    public List<ProductResult> ProductResults { get; set; } = new();
}

public class MeasurementDetail
{
    public int Id { get; set; }
    public int MeasurementId { get; set; }
    public int MeasurementPointId { get; set; }
    public string MeasuredValue { get; set; } = string.Empty;
    public string Result { get; set; } = "Beklemede"; // OK, NOK, CONDITIONAL
    public string Notes { get; set; } = string.Empty;
}

public class ProductResult
{
    public int Id { get; set; }
    public int MeasurementId { get; set; }
    public int ProductIndex { get; set; }
    public string Result { get; set; } = "OK"; // OK, NOK, CONDITIONAL
}
