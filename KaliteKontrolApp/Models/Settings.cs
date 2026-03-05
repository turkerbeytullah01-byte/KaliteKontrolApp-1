namespace KaliteKontrolApp.Models;

public class AppSettings
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = "Şirket Adı";
    public string? LogoPath { get; set; }
    public string DefaultControlledBy { get; set; } = string.Empty;
    public string DefaultApprovedBy { get; set; } = string.Empty;
    public int DefaultMeasurementCount { get; set; } = 5;
    public string Theme { get; set; } = "Light";
}
