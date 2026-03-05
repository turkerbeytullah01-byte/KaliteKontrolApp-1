using KaliteKontrolApp.Controls;
using KaliteKontrolApp.Models;
using KaliteKontrolApp.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace KaliteKontrolApp.Forms;

public partial class PlansControl : UserControl
{
    private ModernDataGrid _dataGrid = null!;
    private TextBox _searchBox = null!;
    private List<QualityPlan> _plans = new();
    
    public PlansControl()
    {
        InitializeComponent();
        LoadPlans();
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
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        
        // Top toolbar
        var toolbar = new Panel
        {
            Dock = DockStyle.Fill,
            Height = 70,
            BackColor = Color.Transparent
        };
        
        // Search box
        _searchBox = new TextBox
        {
            Location = new Point(0, 15),
            Size = new Size(300, 40),
            Font = new Font("Segoe UI", 11F),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.White
        };
        _searchBox.TextChanged += SearchBox_TextChanged;
        
        // Add placeholder
        var placeholderLabel = new Label
        {
            Text = "🔍 Plan ara...",
            ForeColor = ThemeColors.TextMuted,
            Location = new Point(10, 23),
            AutoSize = true,
            BackColor = Color.White
        };
        _searchBox.Controls.Add(placeholderLabel);
        placeholderLabel.BringToFront();
        
        _searchBox.TextChanged += (s, e) =>
        {
            placeholderLabel.Visible = string.IsNullOrEmpty(_searchBox.Text);
        };
        
        // New Plan button
        var btnNew = new ModernButton
        {
            Text = "+ Yeni Plan",
            Location = new Point(320, 12),
            Size = new Size(130, 44),
            ButtonColor = ThemeColors.Primary
        };
        btnNew.Click += BtnNew_Click;
        
        // Import button
        var btnImport = new ModernOutlineButton
        {
            Text = "📥 İçe Aktar",
            Location = new Point(460, 12),
            Size = new Size(120, 44),
            BorderColor = ThemeColors.Primary
        };
        btnImport.Click += BtnImport_Click;
        
        // Export button
        var btnExport = new ModernOutlineButton
        {
            Text = "📤 Dışa Aktar",
            Location = new Point(590, 12),
            Size = new Size(120, 44),
            BorderColor = ThemeColors.Secondary
        };
        btnExport.Click += BtnExport_Click;
        
        toolbar.Controls.AddRange(new Control[] { _searchBox, btnNew, btnImport, btnExport });
        mainLayout.Controls.Add(toolbar, 0, 0);
        
        // DataGrid
        _dataGrid = new ModernDataGrid
        {
            Dock = DockStyle.Fill,
            BackgroundColor = ThemeColors.Background
        };
        
        // Add columns
        _dataGrid.Columns.Add(new DataGridViewTextBoxColumn 
        { 
            Name = "Id", 
            HeaderText = "ID", 
            Width = 60,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        });
        _dataGrid.Columns.Add(new DataGridViewTextBoxColumn 
        { 
            Name = "ProductName", 
            HeaderText = "Ürün Adı" 
        });
        _dataGrid.Columns.Add(new DataGridViewTextBoxColumn 
        { 
            Name = "ProductCode", 
            HeaderText = "Ürün Kodu" 
        });
        _dataGrid.Columns.Add(new DataGridViewTextBoxColumn 
        { 
            Name = "Customer", 
            HeaderText = "Müşteri" 
        });
        _dataGrid.Columns.Add(new DataGridViewTextBoxColumn 
        { 
            Name = "DrawingNo", 
            HeaderText = "Çizim No" 
        });
        _dataGrid.Columns.Add(new DataGridViewTextBoxColumn 
        { 
            Name = "BalloonCount", 
            HeaderText = "Balon Sayısı",
            Width = 100,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        });
        _dataGrid.Columns.Add(new DataGridViewTextBoxColumn 
        { 
            Name = "CreatedAt", 
            HeaderText = "Oluşturma Tarihi",
            Width = 120,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        });
        
        // Action buttons column
        var actionColumn = new DataGridViewButtonColumn
        {
            Name = "Actions",
            HeaderText = "İşlemler",
            Width = 150,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
            Text = "Düzenle",
            UseColumnTextForButtonValue = false
        };
        _dataGrid.Columns.Add(actionColumn);
        
        _dataGrid.CellClick += DataGrid_CellClick;
        _dataGrid.CellPainting += DataGrid_CellPainting;
        
        mainLayout.Controls.Add(_dataGrid, 0, 1);
        Controls.Add(mainLayout);
    }
    
    private void LoadPlans()
    {
        _plans = DatabaseManager.Instance.GetAllPlans();
        RefreshDataGrid();
    }
    
    private void RefreshDataGrid()
    {
        _dataGrid.Rows.Clear();
        
        var searchTerm = _searchBox.Text.ToLower();
        var filteredPlans = string.IsNullOrEmpty(searchTerm) 
            ? _plans 
            : _plans.Where(p => 
                p.ProductName.ToLower().Contains(searchTerm) ||
                p.ProductCode.ToLower().Contains(searchTerm) ||
                p.Customer.ToLower().Contains(searchTerm)).ToList();
        
        foreach (var plan in filteredPlans)
        {
            _dataGrid.Rows.Add(
                plan.Id,
                plan.ProductName,
                plan.ProductCode,
                plan.Customer,
                plan.DrawingNo,
                plan.BalloonCount,
                plan.CreatedAt.ToString("dd.MM.yyyy")
            );
        }
    }
    
    private void SearchBox_TextChanged(object? sender, EventArgs e)
    {
        RefreshDataGrid();
    }
    
    private void BtnNew_Click(object? sender, EventArgs e)
    {
        using var form = new PlanEditForm();
        if (form.ShowDialog() == DialogResult.OK)
        {
            LoadPlans();
        }
    }
    
    private void BtnImport_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Excel files (*.xlsx)|*.xlsx",
            Title = "Excel Dosyası Seç"
        };
        
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var plans = ExcelHelper.ImportPlansFromExcel(dialog.FileName);
                foreach (var plan in plans)
                {
                    DatabaseManager.Instance.SavePlan(plan);
                }
                MessageBox.Show($"{plans.Count} plan başarıyla içe aktarıldı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadPlans();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"İçe aktarma hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    
    private void BtnExport_Click(object? sender, EventArgs e)
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
                ExcelHelper.ExportPlansToExcel(_plans, dialog.FileName);
                MessageBox.Show("Planlar başarıyla dışa aktarıldı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dışa aktarma hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    
    private void DataGrid_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        
        var planId = (int)_dataGrid.Rows[e.RowIndex].Cells["Id"].Value;
        
        if (e.ColumnIndex == _dataGrid.Columns["Actions"].Index)
        {
            // Show context menu
            var menu = new ContextMenuStrip();
            menu.Items.Add("Düzenle", null, (s, ev) => EditPlan(planId));
            menu.Items.Add("Sil", null, (s, ev) => DeletePlan(planId));
            menu.Show(Cursor.Position);
        }
        else
        {
            EditPlan(planId);
        }
    }
    
    private void DataGrid_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        if (e.ColumnIndex == _dataGrid.Columns["Actions"].Index && e.RowIndex >= 0)
        {
            e.PaintBackground(e.CellBounds, true);
            
            // Draw edit button
            var editBounds = new Rectangle(e.CellBounds.X + 5, e.CellBounds.Y + 10, 60, 28);
            using var editBrush = new SolidBrush(ThemeColors.Primary);
            e.Graphics.FillRectangle(editBrush, editBounds);
            TextRenderer.DrawText(e.Graphics, "Düzenle", new Font("Segoe UI", 9F), editBounds, Color.White, 
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            
            // Draw delete button
            var deleteBounds = new Rectangle(e.CellBounds.X + 70, e.CellBounds.Y + 10, 50, 28);
            using var deleteBrush = new SolidBrush(ThemeColors.Error);
            e.Graphics.FillRectangle(deleteBrush, deleteBounds);
            TextRenderer.DrawText(e.Graphics, "Sil", new Font("Segoe UI", 9F), deleteBounds, Color.White, 
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            
            e.Handled = true;
        }
    }
    
    private void EditPlan(int planId)
    {
        var plan = DatabaseManager.Instance.GetPlanById(planId);
        if (plan != null)
        {
            using var form = new PlanEditForm(plan);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadPlans();
            }
        }
    }
    
    private void DeletePlan(int planId)
    {
        var result = MessageBox.Show("Bu planı silmek istediğinize emin misiniz?", "Onay", 
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        
        if (result == DialogResult.Yes)
        {
            DatabaseManager.Instance.DeletePlan(planId);
            LoadPlans();
        }
    }
}
