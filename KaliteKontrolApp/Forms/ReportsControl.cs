using KaliteKontrolApp.Controls;
using KaliteKontrolApp.Utils;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KaliteKontrolApp.Forms;

public partial class ReportsControl : UserControl
{
    public ReportsControl()
    {
        InitializeComponent();
        LoadStatistics();
    }
    
    private void InitializeComponent()
    {
        BackColor = ThemeColors.Background;
        Dock = DockStyle.Fill;
        Padding = new Padding(24);
        
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 2
        };
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        
        // Statistics card
        var statsCard = new ModernCard
        {
            Dock = DockStyle.Fill,
            CardPadding = new Padding(20),
            Margin = new Padding(0, 0, 12, 12)
        };
        
        var statsTitle = new Label
        {
            Text = "Genel İstatistikler",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = ThemeColors.TextPrimary,
            Dock = DockStyle.Top,
            Height = 30
        };
        statsCard.Controls.Add(statsTitle);
        
        var statsPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 20, 0, 0)
        };
        
        // Stats labels will be added dynamically
        statsPanel.Name = "statsPanel";
        statsCard.Controls.Add(statsPanel);
        
        mainLayout.Controls.Add(statsCard, 0, 0);
        
        // Pie chart card
        var pieCard = new ModernCard
        {
            Dock = DockStyle.Fill,
            CardPadding = new Padding(20),
            Margin = new Padding(12, 0, 0, 12)
        };
        
        var pieTitle = new Label
        {
            Text = "Kalite Dağılımı",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = ThemeColors.TextPrimary,
            Dock = DockStyle.Top,
            Height = 30
        };
        pieCard.Controls.Add(pieTitle);
        
        var piePanel = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 20, 0, 0)
        };
        piePanel.Paint += PiePanel_Paint;
        pieCard.Controls.Add(piePanel);
        
        mainLayout.Controls.Add(pieCard, 1, 0);
        
        // Monthly trend card
        var trendCard = new ModernCard
        {
            Dock = DockStyle.Fill,
            CardPadding = new Padding(20),
            Margin = new Padding(0, 12, 12, 0)
        };
        
        var trendTitle = new Label
        {
            Text = "Aylık Trend",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = ThemeColors.TextPrimary,
            Dock = DockStyle.Top,
            Height = 30
        };
        trendCard.Controls.Add(trendTitle);
        
        var trendPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 20, 0, 0)
        };
        trendPanel.Paint += TrendPanel_Paint;
        trendCard.Controls.Add(trendPanel);
        
        mainLayout.Controls.Add(trendCard, 0, 1);
        
        // Export card
        var exportCard = new ModernCard
        {
            Dock = DockStyle.Fill,
            CardPadding = new Padding(20),
            Margin = new Padding(12, 12, 0, 0)
        };
        
        var exportTitle = new Label
        {
            Text = "Rapor İşlemleri",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = ThemeColors.TextPrimary,
            Dock = DockStyle.Top,
            Height = 30
        };
        exportCard.Controls.Add(exportTitle);
        
        var exportPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            Margin = new Padding(0, 20, 0, 0),
            Padding = new Padding(10)
        };
        
        var btnExportPlans = new ModernButton
        {
            Text = "📋 Planları Excel'e Aktar",
            Size = new Size(220, 44),
            Margin = new Padding(0, 0, 0, 12),
            ButtonColor = ThemeColors.Primary
        };
        btnExportPlans.Click += BtnExportPlans_Click;
        
        var btnExportMeasurements = new ModernButton
        {
            Text = "📊 Ölçümleri Excel'e Aktar",
            Size = new Size(220, 44),
            Margin = new Padding(0, 0, 0, 12),
            ButtonColor = ThemeColors.Secondary
        };
        btnExportMeasurements.Click += BtnExportMeasurements_Click;
        
        var btnBackup = new ModernOutlineButton
        {
            Text = "💾 Veritabanı Yedekle",
            Size = new Size(220, 44),
            Margin = new Padding(0, 0, 0, 12),
            BorderColor = ThemeColors.Success
        };
        btnBackup.Click += BtnBackup_Click;
        
        exportPanel.Controls.AddRange(new Control[] { btnExportPlans, btnExportMeasurements, btnBackup });
        exportCard.Controls.Add(exportPanel);
        
        mainLayout.Controls.Add(exportCard, 1, 1);
        
        Controls.Add(mainLayout);
    }
    
    private void LoadStatistics()
    {
        try
        {
            var stats = DatabaseManager.Instance.GetStatistics();
            
            // Update stats panel
            foreach (Control ctrl in Controls[0].Controls)
            {
                if (ctrl is ModernCard card && card.Controls["statsPanel"] is Panel statsPanel)
                {
                    statsPanel.Controls.Clear();
                    
                    var items = new[]
                    {
                        ("Toplam Kalite Planı", stats.TotalPlans, ThemeColors.Primary),
                        ("Toplam Ölçüm", stats.TotalMeasurements, ThemeColors.Secondary),
                        ("Uygun Ürünler", stats.OkProducts, ThemeColors.Success),
                        ("Uygun Değil", stats.NokProducts, ThemeColors.Error),
                        ("Şartlı Kabul", stats.ConditionalProducts, ThemeColors.Warning)
                    };
                    
                    int y = 0;
                    foreach (var (label, value, color) in items)
                    {
                        var itemPanel = new Panel
                        {
                            Dock = DockStyle.Top,
                            Height = 40,
                            Margin = new Padding(0, 0, 0, 8)
                        };
                        
                        var colorBox = new Panel
                        {
                            Size = new Size(12, 12),
                            Location = new Point(0, 14),
                            BackColor = color
                        };
                        
                        var lblText = new Label
                        {
                            Text = label,
                            Font = new Font("Segoe UI", 11F),
                            ForeColor = ThemeColors.TextSecondary,
                            Location = new Point(20, 10),
                            AutoSize = true
                        };
                        
                        var lblValue = new Label
                        {
                            Text = value.ToString(),
                            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                            ForeColor = color,
                            Location = new Point(200, 8),
                            AutoSize = true
                        };
                        
                        itemPanel.Controls.AddRange(new Control[] { colorBox, lblText, lblValue });
                        statsPanel.Controls.Add(itemPanel);
                        y += 48;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"İstatistikler yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    private void PiePanel_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        
        try
        {
            var stats = DatabaseManager.Instance.GetStatistics();
            var total = stats.OkProducts + stats.NokProducts + stats.ConditionalProducts;
            
            if (total == 0)
            {
                g.DrawString("Veri yok", new Font("Segoe UI", 12F), Brushes.Gray, 80, 80);
                return;
            }
            
            var rect = new Rectangle(20, 20, 150, 150);
            float startAngle = 0;
            
            // OK slice
            if (stats.OkProducts > 0)
            {
                var okAngle = 360f * stats.OkProducts / total;
                using var okBrush = new SolidBrush(ThemeColors.Success);
                g.FillPie(okBrush, rect, startAngle, okAngle);
                startAngle += okAngle;
            }
            
            // NOK slice
            if (stats.NokProducts > 0)
            {
                var nokAngle = 360f * stats.NokProducts / total;
                using var nokBrush = new SolidBrush(ThemeColors.Error);
                g.FillPie(nokBrush, rect, startAngle, nokAngle);
                startAngle += nokAngle;
            }
            
            // Conditional slice
            if (stats.ConditionalProducts > 0)
            {
                var condAngle = 360f * stats.ConditionalProducts / total;
                using var condBrush = new SolidBrush(ThemeColors.Warning);
                g.FillPie(condBrush, rect, startAngle, condAngle);
            }
            
            // Legend
            int legendY = 20;
            DrawLegendItem(g, "Uygun", stats.OkProducts.ToString(), ThemeColors.Success, 190, ref legendY);
            DrawLegendItem(g, "Uygun Değil", stats.NokProducts.ToString(), ThemeColors.Error, 190, ref legendY);
            DrawLegendItem(g, "Şartlı", stats.ConditionalProducts.ToString(), ThemeColors.Warning, 190, ref legendY);
        }
        catch
        {
            g.DrawString("Grafik yüklenemedi", new Font("Segoe UI", 10F), Brushes.Gray, 20, 80);
        }
    }
    
    private void TrendPanel_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        
        // Simple bar chart
        var rect = new Rectangle(20, 20, 300, 150);
        
        using var bgBrush = new SolidBrush(Color.FromArgb(245, 245, 245));
        g.FillRectangle(bgBrush, rect);
        
        // Draw axes
        using var axisPen = new Pen(ThemeColors.TextMuted, 1);
        g.DrawLine(axisPen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
        g.DrawLine(axisPen, rect.Left, rect.Top, rect.Left, rect.Bottom);
        
        // Sample bars (would be replaced with real data)
        var barWidth = 30;
        var barGap = 20;
        var bars = new[] { 0.7f, 0.5f, 0.8f, 0.6f, 0.9f, 0.75f };
        var colors = new[] { ThemeColors.Primary, ThemeColors.Secondary, ThemeColors.Success, 
            ThemeColors.Warning, ThemeColors.Error, ThemeColors.Info };
        
        for (int i = 0; i < bars.Length; i++)
        {
            var barHeight = bars[i] * (rect.Height - 30);
            var barRect = new Rectangle(
                rect.Left + 20 + i * (barWidth + barGap),
                rect.Bottom - (int)barHeight - 10,
                barWidth,
                (int)barHeight
            );
            
            using var barBrush = new SolidBrush(colors[i % colors.Length]);
            g.FillRectangle(barBrush, barRect);
        }
        
        // Month labels
        var months = new[] { "Oca", "Şub", "Mar", "Nis", "May", "Haz" };
        for (int i = 0; i < months.Length; i++)
        {
            g.DrawString(months[i], new Font("Segoe UI", 8F), Brushes.Gray,
                rect.Left + 20 + i * (barWidth + barGap), rect.Bottom + 5);
        }
    }
    
    private void DrawLegendItem(Graphics g, string label, string value, Color color, int x, ref int y)
    {
        using var brush = new SolidBrush(color);
        g.FillEllipse(brush, x, y, 12, 12);
        
        using var textBrush = new SolidBrush(ThemeColors.TextPrimary);
        g.DrawString($"{label}: {value}", new Font("Segoe UI", 10F), textBrush, x + 20, y - 1);
        
        y += 24;
    }
    
    private void BtnExportPlans_Click(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "Excel files (*.xlsx)|*.xlsx",
            FileName = $"KalitePlanlari_{DateTime.Now:yyyyMMdd}.xlsx"
        };
        
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var plans = DatabaseManager.Instance.GetAllPlans();
                ExcelHelper.ExportPlansToExcel(plans, dialog.FileName);
                MessageBox.Show("Planlar başarıyla dışa aktarıldı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dışa aktarma hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    
    private void BtnExportMeasurements_Click(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "Excel files (*.xlsx)|*.xlsx",
            FileName = $"Olcumler_{DateTime.Now:yyyyMMdd}.xlsx"
        };
        
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var measurements = DatabaseManager.Instance.GetAllMeasurements();
                ExcelHelper.ExportMeasurementsToExcel(measurements, dialog.FileName);
                MessageBox.Show("Ölçümler başarıyla dışa aktarıldı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dışa aktarma hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    
    private void BtnBackup_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog();
        
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var backupPath = System.IO.Path.Combine(dialog.SelectedPath, 
                    $"kalitekontrol_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db");
                System.IO.File.Copy(Program.DatabasePath, backupPath);
                MessageBox.Show($"Yedekleme başarılı!\nKonum: {backupPath}", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Yedekleme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
