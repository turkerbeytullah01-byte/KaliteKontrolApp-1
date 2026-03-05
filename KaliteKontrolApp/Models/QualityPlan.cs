using System;
using System.Collections.Generic;

namespace KaliteKontrolApp.Models;

public class QualityPlan
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string DrawingNo { get; set; } = string.Empty;
    public int BalloonCount { get; set; } = 10;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public bool IsActive { get; set; } = true;
    
    public List<MeasurementPoint> MeasurementPoints { get; set; } = new();
}

public class MeasurementPoint
{
    public int Id { get; set; }
    public int PlanId { get; set; }
    public int BalloonNo { get; set; }
    public string Dimension { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string NominalValue { get; set; } = string.Empty;
    public string LowerTolerance { get; set; } = "0";
    public string UpperTolerance { get; set; } = "0";
    public string MeasurementTool { get; set; } = string.Empty;
    public string Frequency { get; set; } = "Her parti";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
