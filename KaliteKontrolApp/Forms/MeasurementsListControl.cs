using KaliteKontrolApp.Controls;
using KaliteKontrolApp.Models;
using KaliteKontrolApp.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace KaliteKontrolApp.Forms;

public partial class MeasurementsListControl : UserControl
{
    private ModernDataGrid _dataGrid = null!;
    private TextBox _searchBox = null!;
    private List<Measurement> _measurements = new();
    
    public MeasurementsListControl()
    {
        InitializeComponent();
        LoadMeasurements();
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
        
        // Toolbar
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
        
        // Export button
        var btnExport = new ModernOutlineButton
        {
            Text = "📤 Excel'e Aktar",
            Location = new Point(320, 12),
            Size = new Size(150, 44),
            BorderColor = ThemeColors.Primary
        };
        btnExport.Click += BtnExport_Click;
        
        // Refresh button
        var btnRefresh = new ModernOutlineButton
        {
            Text = "🔄 Yenile",
            Location = new Point(480, 12),
            Size = new Size(120, 44),
            BorderColor = ThemeColors.Secondary
        };
        btnRefresh.Click += (s, e) => LoadMeasurements();
        
        toolbar.Controls.AddRange(new Control[] { _searchBox, btnExport, btnRefresh });
        mainLayout.Controls.Add(toolbar, 0, 0);
        
        // DataGrid
        _dataGrid = new ModernDataGrid
        {
            Dock = DockStyle.Fill,
            BackgroundColor = ThemeColors.Background
        };
        
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
            Name = "ControlDate", 
            HeaderText = "Kontrol Tarihi",
            Width = 120,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        });
        _dataGrid.Columns.Add(new DataGridViewTextBoxColumn 
        { 
            Name = "InvoiceNo", 
            HeaderText = "Fatura No",
            Width = 120,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        });
        _dataGrid.Columns.Add(new DataGridViewTextBoxColumn 
        { 
            Name = "BatchNo", 
            HeaderText = "Parti No",
            Width = 100,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        });
        _dataGrid.Columns.Add(new DataGridViewTextBoxColumn 
        { 
            Name = "QualityType", 
            HeaderText = "Kalite Tipi",
            Width = 100,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        });
        _dataGrid.Columns.Add(new DataGridViewTextBoxColumn 
        { 
            Name = "ControlledBy", 
            HeaderText = "Kontrol Eden",
            Width = 120,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        });
        _dataGrid.Columns.Add(new DataGridViewTextBoxColumn 
        { 
            Name = "OverallResult", 
            HeaderText = "Sonuç",
            Width = 100,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        });
        
        // Actions column
        var actionColumn = new DataGridViewButtonColumn
        {
            Name = "Actions",
            HeaderText = "İşlemler",
            Width = 150,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        };
        _dataGrid.Columns.Add(actionColumn);
        
        _dataGrid.CellClick += DataGrid_CellClick;
        _dataGrid.CellPainting += DataGrid_CellPainting;
        
        mainLayout.Controls.Add(_dataGrid, 0, 1);
        Controls.Add(mainLayout);
    }
    
    private void LoadMeasurements()
    {
        _measurements = DatabaseManager.Instance.GetAllMeasurements();
        RefreshDataGrid();
    }
    
    private void RefreshDataGrid()
    {
        _dataGrid.Rows.Clear();
        
        var searchTerm = _searchBox.Text.ToLower();
        var filteredMeasurements = string.IsNullOrEmpty(searchTerm) 
            ? _measurements 
            : _measurements.Where(m => 
                m.Plan?.ProductName.ToLower().Contains(searchTerm) == true ||
                m.Plan?.ProductCode.ToLower().Contains(searchTerm) == true ||
                m.InvoiceNo.ToLower().Contains(searchTerm) ||
                m.BatchNo.ToLower().Contains(searchTerm) ||
                m.ControlledBy.ToLower().Contains(searchTerm)).ToList();
        
        foreach (var measurement in filteredMeasurements)
        {
            var rowIndex = _dataGrid.Rows.Add(
                measurement.Id,
                measurement.Plan?.ProductName ?? "",
                measurement.Plan?.ProductCode ?? "",
                measurement.ControlDate,
                measurement.InvoiceNo,
                measurement.BatchNo,
                GetQualityTypeText(measurement.QualityType),
                measurement.ControlledBy,
                measurement.OverallResult
            );
            
            // Style result cell
            var resultCell = _dataGrid.Rows[rowIndex].Cells["OverallResult"];
            _dataGrid.StyleStatusCell(resultCell, measurement.OverallResult);
        }
    }
    
    private void SearchBox_TextChanged(object? sender, EventArgs e)
    {
        RefreshDataGrid();
    }
    
    private void BtnExport_Click(object? sender, EventArgs e)
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
                ExcelHelper.ExportMeasurementsToExcel(_measurements, dialog.FileName);
                MessageBox.Show("Ölçümler başarıyla dışa aktarıldı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        
        var measurementId = (int)_dataGrid.Rows[e.RowIndex].Cells["Id"].Value;
        
        if (e.ColumnIndex == _dataGrid.Columns["Actions"].Index)
        {
            var menu = new ContextMenuStrip();
            menu.Items.Add("Görüntüle", null, (s, ev) => ViewMeasurement(measurementId));
            menu.Items.Add("Excel'e Aktar", null, (s, ev) => ExportSingleMeasurement(measurementId));
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Sil", null, (s, ev) => DeleteMeasurement(measurementId));
            menu.Show(Cursor.Position);
        }
        else
        {
            ViewMeasurement(measurementId);
        }
    }
    
    private void DataGrid_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        if (e.ColumnIndex == _dataGrid.Columns["Actions"].Index && e.RowIndex >= 0)
        {
            e.PaintBackground(e.CellBounds, true);
            
            // View button
            var viewBounds = new Rectangle(e.CellBounds.X + 5, e.CellBounds.Y + 10, 50, 28);
            using var viewBrush = new SolidBrush(ThemeColors.Primary);
            e.Graphics.FillRectangle(viewBrush, viewBounds);
            TextRenderer.DrawText(e.Graphics, "Gör", new Font("Segoe UI", 9F), viewBounds, Color.White, 
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            
            // Export button
            var exportBounds = new Rectangle(e.CellBounds.X + 60, e.CellBounds.Y + 10, 40, 28);
            using var exportBrush = new SolidBrush(ThemeColors.Secondary);
            e.Graphics.FillRectangle(exportBrush, exportBounds);
            TextRenderer.DrawText(e.Graphics, "Ex", new Font("Segoe UI", 9F), exportBounds, Color.White, 
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            
            // Delete button
            var deleteBounds = new Rectangle(e.CellBounds.X + 105, e.CellBounds.Y + 10, 40, 28);
            using var deleteBrush = new SolidBrush(ThemeColors.Error);
            e.Graphics.FillRectangle(deleteBrush, deleteBounds);
            TextRenderer.DrawText(e.Graphics, "Sil", new Font("Segoe UI", 9F), deleteBounds, Color.White, 
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            
            e.Handled = true;
        }
    }
    
    private void ViewMeasurement(int measurementId)
    {
        var measurement = DatabaseManager.Instance.GetMeasurementById(measurementId);
        if (measurement?.Plan != null)
        {
            using var form = new MeasurementViewForm(measurement);
            form.ShowDialog();
        }
    }
    
    private void ExportSingleMeasurement(int measurementId)
    {
        var measurement = DatabaseManager.Instance.GetMeasurementById(measurementId);
        if (measurement?.Plan == null) return;
        
        using var dialog = new SaveFileDialog
        {
            Filter = "Excel files (*.xlsx)|*.xlsx",
            FileName = $"OlcumRaporu_{measurement.Plan.ProductCode}_{measurement.ControlDate}.xlsx"
        };
        
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                ExcelHelper.ExportMeasurementReport(measurement, measurement.Plan, dialog.FileName);
                MessageBox.Show("Rapor başarıyla dışa aktarıldı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dışa aktarma hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    
    private void DeleteMeasurement(int measurementId)
    {
        var result = MessageBox.Show("Bu ölçümü silmek istediğinize emin misiniz?", "Onay", 
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        
        if (result == DialogResult.Yes)
        {
            DatabaseManager.Instance.DeleteMeasurement(measurementId);
            LoadMeasurements();
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
