using KaliteKontrolApp.Controls;
using KaliteKontrolApp.Models;
using KaliteKontrolApp.Utils;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace KaliteKontrolApp.Forms;

public partial class SettingsControl : UserControl
{
    private AppSettings _settings = null!;
    private PictureBox _logoPreview = null!;
    private string? _selectedLogoPath;
    
    public SettingsControl()
    {
        InitializeComponent();
        LoadSettings();
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
            ColumnCount = 1
        };
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        
        // Settings card
        var settingsCard = new ModernCard
        {
            Dock = DockStyle.Fill,
            CardPadding = new Padding(30),
            Margin = new Padding(0, 0, 0, 16)
        };
        
        var settingsLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 6
        };
        settingsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        settingsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
        
        // Company Name
        settingsLayout.Controls.Add(CreateLabel("Şirket Adı"), 0, 0);
        var txtCompany = new TextBox
        {
            Name = "txtCompany",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            BorderStyle = BorderStyle.FixedSingle
        };
        settingsLayout.Controls.Add(txtCompany, 1, 0);
        
        // Logo
        settingsLayout.Controls.Add(CreateLabel("Logo"), 0, 1);
        
        var logoPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Height = 80
        };
        
        _logoPreview = new PictureBox
        {
            Size = new Size(64, 64),
            Location = new Point(0, 8),
            BorderStyle = BorderStyle.FixedSingle,
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.White
        };
        
        var btnSelectLogo = new ModernOutlineButton
        {
            Text = "Logo Seç",
            Size = new Size(120, 40),
            Location = new Point(80, 16),
            BorderColor = ThemeColors.Primary
        };
        btnSelectLogo.Click += BtnSelectLogo_Click;
        
        var btnClearLogo = new ModernOutlineButton
        {
            Text = "Temizle",
            Size = new Size(100, 40),
            Location = new Point(210, 16),
            BorderColor = ThemeColors.Error
        };
        btnClearLogo.Click += (s, e) =>
        {
            _logoPreview.Image = null;
            _selectedLogoPath = null;
        };
        
        logoPanel.Controls.AddRange(new Control[] { _logoPreview, btnSelectLogo, btnClearLogo });
        settingsLayout.Controls.Add(logoPanel, 1, 1);
        
        // Default Controlled By
        settingsLayout.Controls.Add(CreateLabel("Varsayılan Kontrol Eden"), 0, 2);
        var txtControlled = new TextBox
        {
            Name = "txtControlled",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            BorderStyle = BorderStyle.FixedSingle
        };
        settingsLayout.Controls.Add(txtControlled, 1, 2);
        
        // Default Approved By
        settingsLayout.Controls.Add(CreateLabel("Varsayılan Onaylayan"), 0, 3);
        var txtApproved = new TextBox
        {
            Name = "txtApproved",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            BorderStyle = BorderStyle.FixedSingle
        };
        settingsLayout.Controls.Add(txtApproved, 1, 3);
        
        // Default Measurement Count
        settingsLayout.Controls.Add(CreateLabel("Varsayılan Ölçüm Adedi"), 0, 4);
        var numCount = new NumericUpDown
        {
            Name = "numCount",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            Minimum = 1,
            Maximum = 50,
            Value = 5
        };
        settingsLayout.Controls.Add(numCount, 1, 4);
        
        // Database info
        settingsLayout.Controls.Add(CreateLabel("Veritabanı Konumu"), 0, 5);
        
        var dbPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Height = 40
        };
        
        var lblDbPath = new Label
        {
            Text = Program.DatabasePath,
            Font = new Font("Segoe UI", 10F),
            ForeColor = ThemeColors.TextSecondary,
            Location = new Point(0, 10),
            AutoSize = true
        };
        
        var btnOpenFolder = new ModernOutlineButton
        {
            Text = "Klasörü Aç",
            Size = new Size(120, 36),
            Location = new Point(400, 2),
            BorderColor = ThemeColors.Secondary
        };
        btnOpenFolder.Click += (s, e) =>
        {
            var folderPath = Path.GetDirectoryName(Program.DatabasePath);
            if (!string.IsNullOrEmpty(folderPath) && Directory.Exists(folderPath))
            {
                System.Diagnostics.Process.Start("explorer.exe", folderPath);
            }
        };
        
        dbPanel.Controls.AddRange(new Control[] { lblDbPath, btnOpenFolder });
        settingsLayout.Controls.Add(dbPanel, 1, 5);
        
        settingsCard.Controls.Add(settingsLayout);
        mainLayout.Controls.Add(settingsCard, 0, 0);
        
        // Buttons
        var buttonPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Height = 70
        };
        
        var btnReset = new ModernOutlineButton
        {
            Text = "Varsayılanlara Dön",
            Size = new Size(160, 44),
            Location = new Point(0, 13),
            BorderColor = ThemeColors.TextMuted
        };
        btnReset.Click += BtnReset_Click;
        
        var btnSave = new ModernButton
        {
            Text = "Ayarları Kaydet",
            Size = new Size(160, 44),
            Location = new Point(buttonPanel.Width - 160, 13),
            Anchor = AnchorStyles.Right,
            ButtonColor = ThemeColors.Primary
        };
        btnSave.Click += BtnSave_Click;
        
        buttonPanel.Controls.AddRange(new Control[] { btnReset, btnSave });
        mainLayout.Controls.Add(buttonPanel, 0, 1);
        
        Controls.Add(mainLayout);
    }
    
    private Label CreateLabel(string text)
    {
        return new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 11F),
            ForeColor = ThemeColors.TextSecondary,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
    }
    
    private void LoadSettings()
    {
        _settings = DatabaseManager.Instance.GetSettings();
        
        if (Controls[0] is TableLayoutPanel mainLayout &&
            mainLayout.Controls[0] is ModernCard settingsCard &&
            settingsCard.Controls[0] is TableLayoutPanel settingsLayout)
        {
            ((TextBox)settingsLayout.Controls["txtCompany"]!).Text = _settings.CompanyName;
            ((TextBox)settingsLayout.Controls["txtControlled"]!).Text = _settings.DefaultControlledBy;
            ((TextBox)settingsLayout.Controls["txtApproved"]!).Text = _settings.DefaultApprovedBy;
            ((NumericUpDown)settingsLayout.Controls["numCount"]!).Value = _settings.DefaultMeasurementCount;
            
            if (!string.IsNullOrEmpty(_settings.LogoPath) && File.Exists(_settings.LogoPath))
            {
                _logoPreview.Image = Image.FromFile(_settings.LogoPath);
                _selectedLogoPath = _settings.LogoPath;
            }
        }
    }
    
    private void BtnSelectLogo_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp",
            Title = "Logo Seç"
        };
        
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                _logoPreview.Image = Image.FromFile(dialog.FileName);
                _selectedLogoPath = dialog.FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Logo yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    
    private void BtnReset_Click(object? sender, EventArgs e)
    {
        if (MessageBox.Show("Tüm ayarları varsayılan değerlere döndürmek istediğinize emin misiniz?", "Onay",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            _settings = new AppSettings();
            
            if (Controls[0] is TableLayoutPanel mainLayout &&
                mainLayout.Controls[0] is ModernCard settingsCard &&
                settingsCard.Controls[0] is TableLayoutPanel settingsLayout)
            {
                ((TextBox)settingsLayout.Controls["txtCompany"]!).Text = _settings.CompanyName;
                ((TextBox)settingsLayout.Controls["txtControlled"]!).Text = _settings.DefaultControlledBy;
                ((TextBox)settingsLayout.Controls["txtApproved"]!).Text = _settings.DefaultApprovedBy;
                ((NumericUpDown)settingsLayout.Controls["numCount"]!).Value = _settings.DefaultMeasurementCount;
            }
            
            _logoPreview.Image = null;
            _selectedLogoPath = null;
        }
    }
    
    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (Controls[0] is TableLayoutPanel mainLayout &&
            mainLayout.Controls[0] is ModernCard settingsCard &&
            settingsCard.Controls[0] is TableLayoutPanel settingsLayout)
        {
            _settings.CompanyName = ((TextBox)settingsLayout.Controls["txtCompany"]!).Text.Trim();
            _settings.DefaultControlledBy = ((TextBox)settingsLayout.Controls["txtControlled"]!).Text.Trim();
            _settings.DefaultApprovedBy = ((TextBox)settingsLayout.Controls["txtApproved"]!).Text.Trim();
            _settings.DefaultMeasurementCount = (int)((NumericUpDown)settingsLayout.Controls["numCount"]!).Value;
            
            // Copy logo to app data folder if selected
            if (!string.IsNullOrEmpty(_selectedLogoPath))
            {
                try
                {
                    var logoFileName = $"logo_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(_selectedLogoPath)}";
                    var destPath = Path.Combine(Program.AppDataPath, logoFileName);
                    File.Copy(_selectedLogoPath, destPath, true);
                    _settings.LogoPath = destPath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Logo kaydedilirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            
            try
            {
                DatabaseManager.Instance.SaveSettings(_settings);
                MessageBox.Show("Ayarlar başarıyla kaydedildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kaydetme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
