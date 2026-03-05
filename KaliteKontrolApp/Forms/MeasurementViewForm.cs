using KaliteKontrolApp.Controls;
using KaliteKontrolApp.Models;
using KaliteKontrolApp.Utils;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace KaliteKontrolApp.Forms;

public partial class MeasurementViewForm : Form
{
    private Measurement _measurement;
    
    public MeasurementViewForm(Measurement measurement)
    {
        _measurement = measurement;
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        Text = "Ölçüm Raporu - Görüntüle";
        Size = new Size(1000, 700);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = ThemeColors.Background;
        FormBorderStyle = FormBorderStyle.Sizable;
        MinimumSize = new Size(800, 500);
        
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 1,
            Padding = new Padding(24)
        };
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 180));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        
        // Header card
        var headerCard = new ModernCard
        {
            Dock = DockStyle.Fill,
            CardPadding = new Padding(20),
            Margin = new Padding(0, 0, 0, 16)
        };
        
        var headerLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 3
        };
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        
        // Product info
        headerLayout.Controls.Add(CreateLabel("Ürün Adı:"), 0, 0);
        headerLayout.Controls.Add(CreateValueLabel(_measurement.Plan?.ProductName ?? ""), 1, 0);
        
        headerLayout.Controls.Add(CreateLabel("Ürün Kodu:"), 2, 0);
        headerLayout.Controls.Add(CreateValueLabel(_measurement.Plan?.ProductCode ?? ""), 3, 0);
        
        headerLayout.Controls.Add(CreateLabel("Kontrol Tarihi:"), 0, 1);
        headerLayout.Controls.Add(CreateValueLabel(_measurement.ControlDate), 1, 1);
        
        headerLayout.Controls.Add(CreateLabel("Fatura No:"), 2, 1);
        headerLayout.Controls.Add(CreateValueLabel(_measurement.InvoiceNo), 3, 1);
        
        headerLayout.Controls.Add(CreateLabel("Parti No:"), 0, 2);
        headerLayout.Controls.Add(CreateValueLabel(_measurement.BatchNo), 1, 2);
        
        headerLayout.Controls.Add(CreateLabel("Kalite Tipi:"), 2, 2);
        headerLayout.Controls.Add(CreateValueLabel(GetQualityTypeText(_measurement.QualityType)), 3, 2);
        
        headerCard.Controls.Add(headerLayout);
        mainLayout.Controls.Add(headerCard, 0, 0);
        
        // Details grid
        var gridCard = new ModernCard
        {
            Dock = DockStyle.Fill,
            CardPadding = new Padding(20)
        };
        
        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ColumnHeadersHeight = 40,
            RowTemplate = { Height = 36 }
        };
        
        grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "BalloonNo", HeaderText = "Balon No", Width = 70 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Dimension", HeaderText = "Ölçü", Width = 120 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nominal", HeaderText = "Nominal", Width = 80 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Tolerance", HeaderText = "Tolerans", Width = 100 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "MeasuredValue", HeaderText = "Ölçülen Değer", Width = 120 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Result", HeaderText = "Sonuç", Width = 80 });
        
        // Load details
        foreach (var detail in _measurement.Details)
        {
            var point = _measurement.Plan?.MeasurementPoints?.FirstOrDefault(p => p.Id == detail.MeasurementPointId);
            if (point != null)
            {
                var rowIndex = grid.Rows.Add(
                    point.BalloonNo,
                    point.Dimension,
                    point.NominalValue,
                    $"{point.LowerTolerance} / +{point.UpperTolerance}",
                    detail.MeasuredValue,
                    detail.Result
                );
                
                // Style result cell
                var resultCell = grid.Rows[rowIndex].Cells["Result"];
                switch (detail.Result)
                {
                    case "OK":
                        resultCell.Style.BackColor = ThemeColors.SuccessLight;
                        resultCell.Style.ForeColor = ThemeColors.Success;
                        break;
                    case "NOK":
                        resultCell.Style.BackColor = ThemeColors.ErrorLight;
                        resultCell.Style.ForeColor = ThemeColors.Error;
                        break;
                    case "CONDITIONAL":
                        resultCell.Style.BackColor = ThemeColors.WarningLight;
                        resultCell.Style.ForeColor = ThemeColors.Warning;
                        break;
                }
            }
        }
        
        gridCard.Controls.Add(grid);
        mainLayout.Controls.Add(gridCard, 0, 1);
        
        // Footer
        var footerPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Height = 70
        };
        
        // Overall result
        var lblResult = new Label
        {
            Text = $"Genel Sonuç: {_measurement.OverallResult}",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            Location = new Point(0, 20),
            AutoSize = true
        };
        
        switch (_measurement.OverallResult)
        {
            case "OK":
                lblResult.ForeColor = ThemeColors.Success;
                break;
            case "NOK":
                lblResult.ForeColor = ThemeColors.Error;
                break;
            default:
                lblResult.ForeColor = ThemeColors.Warning;
                break;
        }
        
        // Buttons
        var btnPrint = new ModernOutlineButton
        {
            Text = "🖨️ Yazdır",
            Size = new Size(120, 44),
            Location = new Point(footerPanel.Width - 380, 13),
            Anchor = AnchorStyles.Right,
            BorderColor = ThemeColors.Primary
        };
        btnPrint.Click += BtnPrint_Click;
        
        var btnExport = new ModernOutlineButton
        {
            Text = "📤 Excel",
            Size = new Size(120, 44),
            Location = new Point(footerPanel.Width - 250, 13),
            Anchor = AnchorStyles.Right,
            BorderColor = ThemeColors.Secondary
        };
        btnExport.Click += BtnExport_Click;
        
        var btnClose = new ModernButton
        {
            Text = "Kapat",
            Size = new Size(120, 44),
            Location = new Point(footerPanel.Width - 120, 13),
            Anchor = AnchorStyles.Right,
            ButtonColor = ThemeColors.Primary
        };
        btnClose.Click += (s, e) => Close();
        
        footerPanel.Controls.AddRange(new Control[] { lblResult, btnPrint, btnExport, btnClose });
        mainLayout.Controls.Add(footerPanel, 0, 2);
        
        Controls.Add(mainLayout);
    }
    
    private Label CreateLabel(string text)
    {
        return new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 10F),
            ForeColor = ThemeColors.TextSecondary,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
    }
    
    private Label CreateValueLabel(string text)
    {
        return new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = ThemeColors.TextPrimary,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
    }
    
    private void BtnPrint_Click(object? sender, EventArgs e)
    {
        // Print dialog
        using var dialog = new PrintDialog();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            // Print implementation would go here
            MessageBox.Show("Yazdırma işlemi başlatıldı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
    
    private void BtnExport_Click(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "Excel files (*.xlsx)|*.xlsx",
            FileName = $"OlcumRaporu_{_measurement.Plan?.ProductCode}_{_measurement.ControlDate}.xlsx"
        };
        
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                if (_measurement.Plan != null)
                {
                    ExcelHelper.ExportMeasurementReport(_measurement, _measurement.Plan, dialog.FileName);
                    MessageBox.Show("Rapor başarıyla dışa aktarıldı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dışa aktarma hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    
    private string GetQualityTypeText(string type)
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
