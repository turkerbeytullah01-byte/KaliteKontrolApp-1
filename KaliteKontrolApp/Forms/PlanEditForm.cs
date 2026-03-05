using KaliteKontrolApp.Controls;
using KaliteKontrolApp.Models;
using KaliteKontrolApp.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace KaliteKontrolApp.Forms;

public partial class PlanEditForm : Form
{
    private QualityPlan _plan;
    private List<MeasurementPoint> _points = new();
    private DataGridView _pointsGrid = null!;
    private bool _isEdit;
    
    public PlanEditForm(QualityPlan? plan = null)
    {
        _plan = plan ?? new QualityPlan();
        _isEdit = plan != null;
        _points = _plan.MeasurementPoints?.ToList() ?? new List<MeasurementPoint>();
        
        InitializeComponent();
        LoadPlanData();
    }
    
    private void InitializeComponent()
    {
        Text = _isEdit ? "Plan Düzenle" : "Yeni Plan";
        Size = new Size(900, 700);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = ThemeColors.Background;
        FormBorderStyle = FormBorderStyle.Sizable;
        MinimumSize = new Size(800, 600);
        
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
        
        // Info card
        var infoCard = new ModernCard
        {
            Dock = DockStyle.Fill,
            CardPadding = new Padding(20),
            Margin = new Padding(0, 0, 0, 16)
        };
        
        var infoLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 2
        };
        infoLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        infoLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        infoLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        infoLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        
        // Product Name
        infoLayout.Controls.Add(CreateLabel("Ürün Adı *"), 0, 0);
        var txtProductName = new TextBox 
        { 
            Name = "txtProductName",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            BorderStyle = BorderStyle.FixedSingle
        };
        infoLayout.Controls.Add(txtProductName, 1, 0);
        
        // Product Code
        infoLayout.Controls.Add(CreateLabel("Ürün Kodu *"), 2, 0);
        var txtProductCode = new TextBox 
        { 
            Name = "txtProductCode",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            BorderStyle = BorderStyle.FixedSingle
        };
        infoLayout.Controls.Add(txtProductCode, 3, 0);
        
        // Customer
        infoLayout.Controls.Add(CreateLabel("Müşteri"), 0, 1);
        var txtCustomer = new TextBox 
        { 
            Name = "txtCustomer",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            BorderStyle = BorderStyle.FixedSingle
        };
        infoLayout.Controls.Add(txtCustomer, 1, 1);
        
        // Drawing No
        infoLayout.Controls.Add(CreateLabel("Çizim No"), 2, 1);
        var txtDrawingNo = new TextBox 
        { 
            Name = "txtDrawingNo",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            BorderStyle = BorderStyle.FixedSingle
        };
        infoLayout.Controls.Add(txtDrawingNo, 3, 1);
        
        infoCard.Controls.Add(infoLayout);
        mainLayout.Controls.Add(infoCard, 0, 0);
        
        // Points card
        var pointsCard = new ModernCard
        {
            Dock = DockStyle.Fill,
            CardPadding = new Padding(20)
        };
        
        var pointsHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 40
        };
        
        var lblPoints = new Label
        {
            Text = "Ölçüm Noktaları",
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = ThemeColors.TextPrimary,
            Dock = DockStyle.Left,
            Width = 200
        };
        
        var btnAddPoint = new ModernButton
        {
            Text = "+ Nokta Ekle",
            Size = new Size(120, 36),
            Dock = DockStyle.Right,
            ButtonColor = ThemeColors.Secondary
        };
        btnAddPoint.Click += BtnAddPoint_Click;
        
        pointsHeader.Controls.AddRange(new Control[] { lblPoints, btnAddPoint });
        pointsCard.Controls.Add(pointsHeader);
        
        // Points grid
        _pointsGrid = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ColumnHeadersHeight = 40,
            RowTemplate = { Height = 36 }
        };
        
        _pointsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "BalloonNo", HeaderText = "Balon No", Width = 80 });
        _pointsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Dimension", HeaderText = "Ölçü", Width = 120 });
        _pointsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Açıklama" });
        _pointsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "NominalValue", HeaderText = "Nominal", Width = 100 });
        _pointsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "LowerTolerance", HeaderText = "Alt Tol.", Width = 80 });
        _pointsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "UpperTolerance", HeaderText = "Üst Tol.", Width = 80 });
        _pointsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "MeasurementTool", HeaderText = "Ölçü Aleti", Width = 120 });
        _pointsGrid.Columns.Add(new DataGridViewButtonColumn { Name = "Delete", HeaderText = "", Width = 60, Text = "Sil" });
        
        _pointsGrid.CellClick += PointsGrid_CellClick;
        _pointsGrid.CellEndEdit += PointsGrid_CellEndEdit;
        
        pointsCard.Controls.Add(_pointsGrid);
        mainLayout.Controls.Add(pointsCard, 0, 1);
        
        // Buttons
        var buttonPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Height = 70
        };
        
        var btnCancel = new ModernOutlineButton
        {
            Text = "İptal",
            Size = new Size(120, 44),
            Location = new Point(buttonPanel.Width - 260, 13),
            Anchor = AnchorStyles.Right,
            BorderColor = ThemeColors.TextMuted
        };
        btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
        
        var btnSave = new ModernButton
        {
            Text = "Kaydet",
            Size = new Size(120, 44),
            Location = new Point(buttonPanel.Width - 130, 13),
            Anchor = AnchorStyles.Right,
            ButtonColor = ThemeColors.Primary
        };
        btnSave.Click += BtnSave_Click;
        
        buttonPanel.Controls.AddRange(new Control[] { btnCancel, btnSave });
        mainLayout.Controls.Add(buttonPanel, 0, 2);
        
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
    
    private void LoadPlanData()
    {
        if (_isEdit)
        {
            if (Controls[0] is TableLayoutPanel mainLayout &&
                mainLayout.Controls[0] is ModernCard infoCard &&
                infoCard.Controls[0] is TableLayoutPanel infoLayout)
            {
                ((TextBox)infoLayout.Controls["txtProductName"]!).Text = _plan.ProductName;
                ((TextBox)infoLayout.Controls["txtProductCode"]!).Text = _plan.ProductCode;
                ((TextBox)infoLayout.Controls["txtCustomer"]!).Text = _plan.Customer;
                ((TextBox)infoLayout.Controls["txtDrawingNo"]!).Text = _plan.DrawingNo;
            }
        }
        
        RefreshPointsGrid();
    }
    
    private void RefreshPointsGrid()
    {
        _pointsGrid.Rows.Clear();
        
        foreach (var point in _points.OrderBy(p => p.BalloonNo))
        {
            _pointsGrid.Rows.Add(
                point.BalloonNo,
                point.Dimension,
                point.Description,
                point.NominalValue,
                point.LowerTolerance,
                point.UpperTolerance,
                point.MeasurementTool,
                "Sil"
            );
        }
    }
    
    private void BtnAddPoint_Click(object? sender, EventArgs e)
    {
        var nextBalloonNo = _points.Count > 0 ? _points.Max(p => p.BalloonNo) + 1 : 1;
        
        var point = new MeasurementPoint
        {
            BalloonNo = nextBalloonNo,
            Dimension = "",
            Description = "",
            NominalValue = "",
            LowerTolerance = "0",
            UpperTolerance = "0",
            MeasurementTool = ""
        };
        
        _points.Add(point);
        RefreshPointsGrid();
    }
    
    private void PointsGrid_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        
        if (e.ColumnIndex == _pointsGrid.Columns["Delete"].Index)
        {
            _points.RemoveAt(e.RowIndex);
            RefreshPointsGrid();
        }
    }
    
    private void PointsGrid_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _points.Count) return;
        
        var point = _points[e.RowIndex];
        var columnName = _pointsGrid.Columns[e.ColumnIndex].Name;
        var value = _pointsGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "";
        
        switch (columnName)
        {
            case "BalloonNo":
                if (int.TryParse(value, out int balloonNo))
                    point.BalloonNo = balloonNo;
                break;
            case "Dimension":
                point.Dimension = value;
                break;
            case "Description":
                point.Description = value;
                break;
            case "NominalValue":
                point.NominalValue = value;
                break;
            case "LowerTolerance":
                point.LowerTolerance = value;
                break;
            case "UpperTolerance":
                point.UpperTolerance = value;
                break;
            case "MeasurementTool":
                point.MeasurementTool = value;
                break;
        }
    }
    
    private void BtnSave_Click(object? sender, EventArgs e)
    {
        // Validate
        if (Controls[0] is TableLayoutPanel mainLayout &&
            mainLayout.Controls[0] is ModernCard infoCard &&
            infoCard.Controls[0] is TableLayoutPanel infoLayout)
        {
            var productName = ((TextBox)infoLayout.Controls["txtProductName"]!).Text.Trim();
            var productCode = ((TextBox)infoLayout.Controls["txtProductCode"]!).Text.Trim();
            
            if (string.IsNullOrEmpty(productName))
            {
                MessageBox.Show("Ürün adı zorunludur!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (string.IsNullOrEmpty(productCode))
            {
                MessageBox.Show("Ürün kodu zorunludur!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            _plan.ProductName = productName;
            _plan.ProductCode = productCode;
            _plan.Customer = ((TextBox)infoLayout.Controls["txtCustomer"]!).Text.Trim();
            _plan.DrawingNo = ((TextBox)infoLayout.Controls["txtDrawingNo"]!).Text.Trim();
            _plan.BalloonCount = _points.Count;
            _plan.MeasurementPoints = _points;
            
            try
            {
                DatabaseManager.Instance.SavePlan(_plan);
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kaydetme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
