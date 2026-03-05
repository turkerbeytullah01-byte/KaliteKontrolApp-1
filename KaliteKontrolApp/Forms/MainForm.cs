using KaliteKontrolApp.Controls;
using KaliteKontrolApp.Utils;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KaliteKontrolApp.Forms;

public partial class MainForm : Form
{
    private Panel _sidebarPanel = null!;
    private Panel _contentPanel = null!;
    private Panel _headerPanel = null!;
    private Label _titleLabel = null!;
    private Label _dateLabel = null!;
    private Button _toggleButton = null!;
    private string _currentPage = "dashboard";
    private bool _sidebarExpanded = true;
    
    public MainForm()
    {
        InitializeComponent();
        InitializeModernUI();
    }
    
    private void InitializeComponent()
    {
        Text = "Pro Kalite Kontrol Sistemi";
        Size = new Size(1400, 900);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = ThemeColors.Background;
        Font = new Font("Segoe UI", 10F);
        
        // Form border style
        FormBorderStyle = FormBorderStyle.Sizable;
        MinimumSize = new Size(1000, 700);
    }
    
    private void InitializeModernUI()
    {
        // Header Panel
        _headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = Color.White
        };
        _headerPanel.Paint += HeaderPanel_Paint;
        
        // Toggle button
        _toggleButton = new Button
        {
            Size = new Size(40, 40),
            Location = new Point(20, 15),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand,
            Image = CreateMenuIcon(),
            ImageAlign = ContentAlignment.MiddleCenter
        };
        _toggleButton.FlatAppearance.BorderSize = 0;
        _toggleButton.Click += ToggleButton_Click;
        
        // Title label
        _titleLabel = new Label
        {
            Text = "Ana Sayfa",
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            ForeColor = ThemeColors.TextPrimary,
            Location = new Point(80, 12),
            AutoSize = true
        };
        
        // Date label
        _dateLabel = new Label
        {
            Text = DateTime.Now.ToString("dddd, d MMMM yyyy", new System.Globalization.CultureInfo("tr-TR")),
            Font = new Font("Segoe UI", 10F),
            ForeColor = ThemeColors.TextSecondary,
            Location = new Point(80, 40),
            AutoSize = true
        };
        
        _headerPanel.Controls.AddRange(new Control[] { _toggleButton, _titleLabel, _dateLabel });
        
        // Sidebar Panel
        _sidebarPanel = new Panel
        {
            Dock = DockStyle.Left,
            Width = 280,
            BackColor = Color.White,
            Padding = new Padding(0, 20, 0, 20)
        };
        _sidebarPanel.Paint += SidebarPanel_Paint;
        
        InitializeSidebar();
        
        // Content Panel
        _contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = ThemeColors.Background,
            Padding = new Padding(24)
        };
        
        Controls.Add(_contentPanel);
        Controls.Add(_sidebarPanel);
        Controls.Add(_headerPanel);
        
        // Load dashboard
        LoadPage("dashboard");
    }
    
    private void InitializeSidebar()
    {
        // Logo area
        var logoPanel = new Panel
        {
            Height = 80,
            Dock = DockStyle.Top,
            BackColor = Color.White,
            Padding = new Padding(20, 0, 20, 0)
        };
        
        var logoBox = new PictureBox
        {
            Size = new Size(48, 48),
            Location = new Point(20, 16),
            Image = CreateLogoImage(),
            SizeMode = PictureBoxSizeMode.Zoom
        };
        
        var companyLabel = new Label
        {
            Text = "Kalite Kontrol",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = ThemeColors.TextPrimary,
            Location = new Point(80, 18),
            AutoSize = true
        };
        
        var versionLabel = new Label
        {
            Text = $"v{Program.AppVersion}",
            Font = new Font("Segoe UI", 9F),
            ForeColor = ThemeColors.TextMuted,
            Location = new Point(80, 42),
            AutoSize = true
        };
        
        logoPanel.Controls.AddRange(new Control[] { logoBox, companyLabel, versionLabel });
        _sidebarPanel.Controls.Add(logoPanel);
        
        // Menu items
        var menuPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(12, 20, 12, 0),
            AutoScroll = true
        };
        
        var menuItems = new[]
        {
            ("dashboard", "Ana Sayfa", CreateHomeIcon()),
            ("plans", "Kalite Kontrol Planları", CreateClipboardIcon()),
            ("measurement", "Ölçüm Raporu", CreateRulerIcon()),
            ("measurementsList", "Tüm Ölçümler", CreateListIcon()),
            ("reports", "Raporlar", CreateChartIcon()),
            ("settings", "Ayarlar", CreateSettingsIcon())
        };
        
        int y = 0;
        foreach (var (id, text, icon) in menuItems)
        {
            var btn = CreateMenuButton(id, text, icon);
            btn.Location = new Point(0, y);
            btn.Click += MenuButton_Click;
            menuPanel.Controls.Add(btn);
            y += 56;
        }
        
        _sidebarPanel.Controls.Add(menuPanel);
    }
    
    private Button CreateMenuButton(string id, string text, Image icon)
    {
        var btn = new Button
        {
            Name = $"btn_{id}",
            Size = new Size(256, 48),
            FlatStyle = FlatStyle.Flat,
            BackColor = id == _currentPage ? ThemeColors.Primary50 : Color.White,
            ForeColor = id == _currentPage ? ThemeColors.Primary : ThemeColors.SidebarText,
            Font = new Font("Segoe UI", 11F, id == _currentPage ? FontStyle.Bold : FontStyle.Regular),
            Text = "    " + text,
            TextAlign = ContentAlignment.MiddleLeft,
            Image = icon,
            ImageAlign = ContentAlignment.MiddleLeft,
            TextImageRelation = TextImageRelation.ImageBeforeText,
            Cursor = Cursors.Hand,
            Tag = id
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.FlatAppearance.MouseOverBackColor = ThemeColors.SidebarHover;
        btn.FlatAppearance.MouseDownBackColor = ThemeColors.Primary50;
        
        return btn;
    }
    
    private void MenuButton_Click(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.Tag is string pageId)
        {
            LoadPage(pageId);
        }
    }
    
    private void LoadPage(string pageId)
    {
        _currentPage = pageId;
        
        // Update title
        var titles = new System.Collections.Generic.Dictionary<string, string>
        {
            ["dashboard"] = "Ana Sayfa",
            ["plans"] = "Kalite Kontrol Planları",
            ["measurement"] = "Ölçüm Raporu",
            ["measurementsList"] = "Tüm Ölçümler",
            ["reports"] = "Raporlar",
            ["settings"] = "Ayarlar"
        };
        _titleLabel.Text = titles.GetValueOrDefault(pageId, "Ana Sayfa");
        
        // Update sidebar button styles
        foreach (Control ctrl in _sidebarPanel.Controls)
        {
            if (ctrl is Panel panel)
            {
                foreach (Control btnCtrl in panel.Controls)
                {
                    if (btnCtrl is Button btn && btn.Tag is string btnId)
                    {
                        btn.BackColor = btnId == pageId ? ThemeColors.Primary50 : Color.White;
                        btn.ForeColor = btnId == pageId ? ThemeColors.Primary : ThemeColors.SidebarText;
                        btn.Font = new Font("Segoe UI", 11F, btnId == pageId ? FontStyle.Bold : FontStyle.Regular);
                    }
                }
            }
        }
        
        // Load content
        _contentPanel.Controls.Clear();
        UserControl? content = pageId switch
        {
            "dashboard" => new DashboardControl(),
            "plans" => new PlansControl(),
            "measurement" => new MeasurementControl(),
            "measurementsList" => new MeasurementsListControl(),
            "reports" => new ReportsControl(),
            "settings" => new SettingsControl(),
            _ => null
        };
        
        if (content != null)
        {
            content.Dock = DockStyle.Fill;
            _contentPanel.Controls.Add(content);
        }
    }
    
    private void ToggleButton_Click(object? sender, EventArgs e)
    {
        _sidebarExpanded = !_sidebarExpanded;
        
        // Animate sidebar width
        var targetWidth = _sidebarExpanded ? 280 : 70;
        _sidebarPanel.Width = targetWidth;
        
        // Update menu buttons
        foreach (Control ctrl in _sidebarPanel.Controls)
        {
            if (ctrl is Panel panel)
            {
                foreach (Control btnCtrl in panel.Controls)
                {
                    if (btnCtrl is Button btn)
                    {
                        btn.Width = _sidebarExpanded ? 256 : 46;
                        btn.Text = _sidebarExpanded ? "    " + GetMenuText(btn.Tag?.ToString() ?? "") : "";
                    }
                }
            }
        }
    }
    
    private string GetMenuText(string id)
    {
        return id switch
        {
            "dashboard" => "Ana Sayfa",
            "plans" => "Kalite Kontrol Planları",
            "measurement" => "Ölçüm Raporu",
            "measurementsList" => "Tüm Ölçümler",
            "reports" => "Raporlar",
            "settings" => "Ayarlar",
            _ => ""
        };
    }
    
    private void HeaderPanel_Paint(object? sender, PaintEventArgs e)
    {
        using var pen = new Pen(ThemeColors.Border, 1);
        e.Graphics.DrawLine(pen, 0, _headerPanel.Height - 1, _headerPanel.Width, _headerPanel.Height - 1);
    }
    
    private void SidebarPanel_Paint(object? sender, PaintEventArgs e)
    {
        using var pen = new Pen(ThemeColors.Border, 1);
        e.Graphics.DrawLine(pen, _sidebarPanel.Width - 1, 0, _sidebarPanel.Width - 1, _sidebarPanel.Height);
    }
    
    // Icon creation methods
    private Image CreateMenuIcon()
    {
        var bmp = new Bitmap(24, 24);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(ThemeColors.TextPrimary, 2);
        g.DrawLine(pen, 4, 6, 20, 6);
        g.DrawLine(pen, 4, 12, 20, 12);
        g.DrawLine(pen, 4, 18, 20, 18);
        return bmp;
    }
    
    private Image CreateHomeIcon()
    {
        var bmp = new Bitmap(20, 20);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(ThemeColors.Primary, 2);
        g.DrawPolygon(pen, new[] { new Point(10, 2), new Point(2, 10), new Point(4, 10), new Point(4, 16), new Point(8, 16), new Point(8, 12), new Point(12, 12), new Point(12, 16), new Point(16, 16), new Point(16, 10), new Point(18, 10) });
        return bmp;
    }
    
    private Image CreateClipboardIcon()
    {
        var bmp = new Bitmap(20, 20);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(ThemeColors.Primary, 2);
        g.DrawRectangle(pen, 4, 6, 12, 12);
        g.DrawRectangle(pen, 7, 2, 6, 4);
        return bmp;
    }
    
    private Image CreateRulerIcon()
    {
        var bmp = new Bitmap(20, 20);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(ThemeColors.Primary, 2);
        g.DrawLine(pen, 4, 16, 16, 4);
        g.DrawLine(pen, 6, 14, 6, 16);
        g.DrawLine(pen, 9, 11, 9, 13);
        g.DrawLine(pen, 12, 8, 12, 10);
        g.DrawLine(pen, 15, 5, 15, 7);
        return bmp;
    }
    
    private Image CreateListIcon()
    {
        var bmp = new Bitmap(20, 20);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(ThemeColors.Primary, 2);
        for (int i = 0; i < 4; i++)
        {
            g.DrawLine(pen, 6, 4 + i * 4, 18, 4 + i * 4);
            g.FillEllipse(Brushes.Gray, 2, 2 + i * 4, 3, 3);
        }
        return bmp;
    }
    
    private Image CreateChartIcon()
    {
        var bmp = new Bitmap(20, 20);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(ThemeColors.Primary, 2);
        g.DrawLine(pen, 2, 18, 18, 18);
        g.DrawLine(pen, 2, 18, 2, 2);
        g.DrawLine(pen, 6, 14, 6, 10);
        g.DrawLine(pen, 10, 14, 10, 6);
        g.DrawLine(pen, 14, 14, 14, 4);
        return bmp;
    }
    
    private Image CreateSettingsIcon()
    {
        var bmp = new Bitmap(20, 20);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(ThemeColors.Primary, 2);
        g.DrawEllipse(pen, 6, 6, 8, 8);
        for (int i = 0; i < 8; i++)
        {
            double angle = i * Math.PI / 4;
            int x1 = 10 + (int)(4 * Math.Cos(angle));
            int y1 = 10 + (int)(4 * Math.Sin(angle));
            int x2 = 10 + (int)(8 * Math.Cos(angle));
            int y2 = 10 + (int)(8 * Math.Sin(angle));
            g.DrawLine(pen, x1, y1, x2, y2);
        }
        return bmp;
    }
    
    private Image CreateLogoImage()
    {
        var bmp = new Bitmap(48, 48);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        
        // Background gradient
        using var brush = new LinearGradientBrush(new Rectangle(0, 0, 48, 48), 
            ThemeColors.Primary, ThemeColors.PrimaryDark, 45);
        g.FillEllipse(brush, 0, 0, 48, 48);
        
        // Letter Q
        using var font = new Font("Segoe UI", 20F, FontStyle.Bold);
        using var textBrush = new SolidBrush(Color.White);
        var size = g.MeasureString("Q", font);
        g.DrawString("Q", font, textBrush, (48 - size.Width) / 2, (48 - size.Height) / 2);
        
        return bmp;
    }
}
