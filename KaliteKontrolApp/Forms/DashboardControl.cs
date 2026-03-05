using KaliteKontrolApp.Controls;
using KaliteKontrolApp.Utils;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KaliteKontrolApp.Forms;

public partial class DashboardControl : UserControl
{
    public DashboardControl()
    {
        InitializeComponent();
        LoadDashboardData();
    }
    
    private void InitializeComponent()
    {
        BackColor = ThemeColors.Background;
        Dock = DockStyle.Fill;
        Padding = new Padding(24);
        
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 1
        };
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 140));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
        
        // Stats cards row
        var statsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(0, 0, 0, 24)
        };
        
        // Create stat cards
        var card1 = CreateStatCard("Kalite Planları", "0", ThemeColors.Primary, ThemeColors.Primary50, CreateClipboardIcon());
        var card2 = CreateStatCard("Toplam Ölçüm", "0", ThemeColors.Secondary, ThemeColors.InfoLight, CreateRulerIcon());
        var card3 = CreateStatCard("Uygun Ürünler", "0", ThemeColors.Success, ThemeColors.SuccessLight, CreateCheckIcon());
        var card4 = CreateStatCard("Uygun Değil", "0", ThemeColors.Error, ThemeColors.ErrorLight, CreateXIcon());
        
        statsPanel.Controls.AddRange(new Control[] { card1, card2, card3, card4 });
        mainLayout.Controls.Add(statsPanel, 0, 0);
        
        // Middle row - Charts and recent activities
        var middlePanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0, 0, 0, 24)
        };
        middlePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        middlePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        
        // Quality chart card
        var chartCard = new ModernCard
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 12, 0),
            CardPadding = new Padding(20)
        };
        
        var chartTitle = new Label
        {
            Text = "Kalite Dağılımı",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = ThemeColors.TextPrimary,
            Dock = DockStyle.Top,
            Height = 30
        };
        chartCard.Controls.Add(chartTitle);
        
        // Simple pie chart panel
        var pieChart = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 20, 0, 0)
        };
        pieChart.Paint += PieChart_Paint;
        chartCard.Controls.Add(pieChart);
        
        middlePanel.Controls.Add(chartCard, 0, 0);
        
        // Recent measurements card
        var recentCard = new ModernCard
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(12, 0, 0, 0),
            CardPadding = new Padding(20)
        };
        
        var recentTitle = new Label
        {
            Text = "Son Ölçümler",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = ThemeColors.TextPrimary,
            Dock = DockStyle.Top,
            Height = 30
        };
        recentCard.Controls.Add(recentTitle);
        
        var recentList = new ListView
        {
            Dock = DockStyle.Fill,
            View = View.Details,
            FullRowSelect = true,
            BorderStyle = BorderStyle.None,
            Margin = new Padding(0, 20, 0, 0),
            HeaderStyle = ColumnHeaderStyle.None
        };
        recentList.Columns.Add("Ürün", 150);
        recentList.Columns.Add("Tarih", 100);
        recentList.Columns.Add("Sonuç", 80);
        recentCard.Controls.Add(recentList);
        
        middlePanel.Controls.Add(recentCard, 1, 0);
        mainLayout.Controls.Add(middlePanel, 0, 1);
        
        // Bottom row - Quick actions
        var actionsCard = new ModernCard
        {
            Dock = DockStyle.Fill,
            CardPadding = new Padding(20)
        };
        
        var actionsTitle = new Label
        {
            Text = "Hızlı İşlemler",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = ThemeColors.TextPrimary,
            Dock = DockStyle.Top,
            Height = 30
        };
        actionsCard.Controls.Add(actionsTitle);
        
        var actionsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(0, 20, 0, 0)
        };
        
        var btnNewPlan = CreateActionButton("Yeni Plan", ThemeColors.Primary, () => 
        {
            if (Parent?.Parent?.Parent is MainForm mainForm)
            {
                mainForm.Invoke(new Action(() => 
                {
                    // Navigate to plans page
                }));
            }
        });
        
        var btnNewMeasurement = CreateActionButton("Yeni Ölçüm", ThemeColors.Secondary, () => { });
        var btnExport = CreateActionButton("Excel'e Aktar", ThemeColors.Info, ExportData);
        var btnBackup = CreateActionButton("Yedekle", ThemeColors.Warning, BackupData);
        
        actionsPanel.Controls.AddRange(new Control[] { btnNewPlan, btnNewMeasurement, btnExport, btnBackup });
        actionsCard.Controls.Add(actionsPanel);
        
        mainLayout.Controls.Add(actionsCard, 0, 2);
        
        Controls.Add(mainLayout);
    }
    
    private StatCard CreateStatCard(string title, string value, Color iconColor, Color bgColor, Image icon)
    {
        return new StatCard
        {
            Title = title,
            Value = value,
            Icon = icon,
            IconColor = iconColor,
            IconBgColor = bgColor,
            Margin = new Padding(0, 0, 16, 0),
            Size = new Size(240, 120)
        };
    }
    
    private ModernButton CreateActionButton(string text, Color color, Action onClick)
    {
        var btn = new ModernButton
        {
            Text = text,
            ButtonColor = color,
            Size = new Size(140, 44),
            Margin = new Padding(0, 0, 12, 0)
        };
        btn.Click += (s, e) => onClick();
        return btn;
    }
    
    private void LoadDashboardData()
    {
        try
        {
            var stats = DatabaseManager.Instance.GetStatistics();
            
            // Update stat cards
            foreach (Control ctrl in Controls[0].Controls[0].Controls)
            {
                if (ctrl is StatCard card)
                {
                    switch (card.Title)
                    {
                        case "Kalite Planları":
                            card.Value = stats.TotalPlans.ToString();
                            break;
                        case "Toplam Ölçüm":
                            card.Value = stats.TotalMeasurements.ToString();
                            break;
                        case "Uygun Ürünler":
                            card.Value = stats.OkProducts.ToString();
                            break;
                        case "Uygun Değil":
                            card.Value = stats.NokProducts.ToString();
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Veri yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    private void PieChart_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        
        try
        {
            var stats = DatabaseManager.Instance.GetStatistics();
            var total = stats.OkProducts + stats.NokProducts + stats.ConditionalProducts;
            
            if (total == 0)
            {
                g.DrawString("Veri yok", new Font("Segoe UI", 12F), Brushes.Gray, 50, 50);
                return;
            }
            
            var rect = new Rectangle(20, 20, 120, 120);
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
            DrawLegendItem(g, "Uygun", stats.OkProducts.ToString(), ThemeColors.Success, 160, ref legendY);
            DrawLegendItem(g, "Uygun Değil", stats.NokProducts.ToString(), ThemeColors.Error, 160, ref legendY);
            DrawLegendItem(g, "Şartlı", stats.ConditionalProducts.ToString(), ThemeColors.Warning, 160, ref legendY);
        }
        catch
        {
            g.DrawString("Grafik yüklenemedi", new Font("Segoe UI", 10F), Brushes.Gray, 20, 50);
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
    
    private void ExportData()
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "Excel files (*.xlsx)|*.xlsx",
            FileName = $"KaliteRapor_{DateTime.Now:yyyyMMdd}.xlsx"
        };
        
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var measurements = DatabaseManager.Instance.GetAllMeasurements();
                ExcelHelper.ExportMeasurementsToExcel(measurements, dialog.FileName);
                MessageBox.Show("Veriler başarıyla dışa aktarıldı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dışa aktarma hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    
    private void BackupData()
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
    
    // Icon creation methods
    private Image CreateClipboardIcon()
    {
        var bmp = new Bitmap(24, 24);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(ThemeColors.Primary, 2);
        g.DrawRectangle(pen, 5, 8, 14, 13);
        g.DrawRectangle(pen, 9, 3, 6, 5);
        return bmp;
    }
    
    private Image CreateRulerIcon()
    {
        var bmp = new Bitmap(24, 24);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(ThemeColors.Secondary, 2);
        g.DrawLine(pen, 5, 19, 19, 5);
        g.DrawLine(pen, 7, 17, 7, 19);
        g.DrawLine(pen, 10, 14, 10, 16);
        g.DrawLine(pen, 13, 11, 13, 13);
        g.DrawLine(pen, 16, 8, 16, 10);
        return bmp;
    }
    
    private Image CreateCheckIcon()
    {
        var bmp = new Bitmap(24, 24);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(ThemeColors.Success, 2.5f);
        g.DrawLine(pen, 5, 12, 10, 17);
        g.DrawLine(pen, 10, 17, 19, 7);
        return bmp;
    }
    
    private Image CreateXIcon()
    {
        var bmp = new Bitmap(24, 24);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(ThemeColors.Error, 2.5f);
        g.DrawLine(pen, 6, 6, 18, 18);
        g.DrawLine(pen, 18, 6, 6, 18);
        return bmp;
    }
}
