using KaliteKontrolApp.Controls;
using KaliteKontrolApp.Models;
using KaliteKontrolApp.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace KaliteKontrolApp.Forms;

public partial class MeasurementControl : UserControl
{
    private ComboBox _planCombo = null!;
    private NumericUpDown _productCount = null!;
    private DataGridView _measurementGrid = null!;
    private List<QualityPlan> _plans = new();
    private QualityPlan? _selectedPlan;
    private int _productCount = 5;
    
    public MeasurementControl()
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
            RowCount = 3,
            ColumnCount = 1
        };
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 200));
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
            RowCount = 3
        };
        infoLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        infoLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        infoLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        infoLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        
        // Plan selection
        infoLayout.Controls.Add(CreateLabel("Kalite Planı *"), 0, 0);
        _planCombo = new ComboBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _planCombo.SelectedIndexChanged += PlanCombo_SelectedIndexChanged;
        infoLayout.Controls.Add(_planCombo, 1, 0);
        
        // Product count
        infoLayout.Controls.Add(CreateLabel("Ölçülecek Adet *"), 2, 0);
        _productCount = new NumericUpDown
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            Minimum = 1,
            Maximum = 50,
            Value = 5
        };
        _productCount.ValueChanged += ProductCount_ValueChanged;
        infoLayout.Controls.Add(_productCount, 3, 0);
        
        // Control Date
        infoLayout.Controls.Add(CreateLabel("Kontrol Tarihi *"), 0, 1);
        var dtpDate = new DateTimePicker
        {
            Name = "dtpDate",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            Format = DateTimePickerFormat.Short,
            Value = DateTime.Now
        };
        infoLayout.Controls.Add(dtpDate, 1, 1);
        
        // Invoice No
        infoLayout.Controls.Add(CreateLabel("Fatura No"), 2, 1);
        var txtInvoice = new TextBox
        {
            Name = "txtInvoice",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            BorderStyle = BorderStyle.FixedSingle
        };
        infoLayout.Controls.Add(txtInvoice, 3, 1);
        
        // Batch No
        infoLayout.Controls.Add(CreateLabel("Parti No"), 0, 2);
        var txtBatch = new TextBox
        {
            Name = "txtBatch",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            BorderStyle = BorderStyle.FixedSingle
        };
        infoLayout.Controls.Add(txtBatch, 1, 2);
        
        // Quality Type
        infoLayout.Controls.Add(CreateLabel("Kalite Tipi"), 2, 2);
        var cmbQualityType = new ComboBox
        {
            Name = "cmbQualityType",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbQualityType.Items.AddRange(new object[] { "Son Kontrol", "İlk Kontrol", "Proses Kontrol" });
        cmbQualityType.SelectedIndex = 0;
        infoLayout.Controls.Add(cmbQualityType, 3, 2);
        
        // Additional info row
        var additionalPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 80,
            Padding = new Padding(0, 10, 0, 0)
        };
        
        var addLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 2
        };
        addLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        addLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        addLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        addLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        
        addLayout.Controls.Add(CreateLabel("Kontrol Eden"), 0, 0);
        var txtControlled = new TextBox
        {
            Name = "txtControlled",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            BorderStyle = BorderStyle.FixedSingle
        };
        addLayout.Controls.Add(txtControlled, 1, 0);
        
        addLayout.Controls.Add(CreateLabel("Onaylayan"), 2, 0);
        var txtApproved = new TextBox
        {
            Name = "txtApproved",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            BorderStyle = BorderStyle.FixedSingle
        };
        addLayout.Controls.Add(txtApproved, 3, 0);
        
        addLayout.Controls.Add(CreateLabel("Cihaz Kodları"), 0, 1);
        var txtDevices = new TextBox
        {
            Name = "txtDevices",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            BorderStyle = BorderStyle.FixedSingle
        };
        addLayout.Controls.Add(txtDevices, 1, 1);
        
        addLayout.Controls.Add(CreateLabel("Notlar"), 2, 1);
        var txtNotes = new TextBox
        {
            Name = "txtNotes",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            BorderStyle = BorderStyle.FixedSingle
        };
        addLayout.Controls.Add(txtNotes, 3, 1);
        
        additionalPanel.Controls.Add(addLayout);
        
        infoCard.Controls.Add(infoLayout);
        infoCard.Controls.Add(additionalPanel);
        mainLayout.Controls.Add(infoCard, 0, 0);
        
        // Measurement grid card
        var gridCard = new ModernCard
        {
            Dock = DockStyle.Fill,
            CardPadding = new Padding(20)
        };
        
        _measurementGrid = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ColumnHeadersHeight = 50,
            RowTemplate = { Height = 40 },
            ReadOnly = false,
            EditMode = DataGridViewEditMode.EditOnEnter
        };
        
        gridCard.Controls.Add(_measurementGrid);
        mainLayout.Controls.Add(gridCard, 0, 1);
        
        // Buttons
        var buttonPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Height = 70
        };
        
        var btnClear = new ModernOutlineButton
        {
            Text = "Temizle",
            Size = new Size(120, 44),
            Location = new Point(0, 13),
            BorderColor = ThemeColors.TextMuted
        };
        btnClear.Click += BtnClear_Click;
        
        var btnSave = new ModernButton
        {
            Text = "Kaydet",
            Size = new Size(140, 44),
            Location = new Point(buttonPanel.Width - 140, 13),
            Anchor = AnchorStyles.Right,
            ButtonColor = ThemeColors.Primary
        };
        btnSave.Click += BtnSave_Click;
        
        buttonPanel.Controls.AddRange(new Control[] { btnClear, btnSave });
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
    
    private void LoadPlans()
    {
        _plans = DatabaseManager.Instance.GetAllPlans();
        _planCombo.Items.Clear();
        _planCombo.Items.Add("Plan seçin...");
        
        foreach (var plan in _plans)
        {
            _planCombo.Items.Add($"{plan.ProductCode} - {plan.ProductName}");
        }
        
        _planCombo.SelectedIndex = 0;
    }
    
    private void PlanCombo_SelectedIndexChanged(object? sender, EventArgs e)
    {
        var index = _planCombo.SelectedIndex;
        if (index > 0 && index <= _plans.Count)
        {
            _selectedPlan = _plans[index - 1];
            _selectedPlan = DatabaseManager.Instance.GetPlanById(_selectedPlan.Id);
            RefreshMeasurementGrid();
        }
        else
        {
            _selectedPlan = null;
            _measurementGrid.Columns.Clear();
        }
    }
    
    private void ProductCount_ValueChanged(object? sender, EventArgs e)
    {
        _productCount = (int)_productCount.Value;
        if (_selectedPlan != null)
        {
            RefreshMeasurementGrid();
        }
    }
    
    private void RefreshMeasurementGrid()
    {
        if (_selectedPlan?.MeasurementPoints == null) return;
        
        _measurementGrid.Columns.Clear();
        
        // Fixed columns
        _measurementGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "BalloonNo",
            HeaderText = "Balon",
            Width = 50,
            ReadOnly = true
        });
        
        _measurementGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Dimension",
            HeaderText = "Ölçü",
            Width = 100,
            ReadOnly = true
        });
        
        _measurementGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Nominal",
            HeaderText = "Nominal",
            Width = 80,
            ReadOnly = true
        });
        
        _measurementGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Tolerance",
            HeaderText = "Tolerans",
            Width = 100,
            ReadOnly = true
        });
        
        // Dynamic product columns
        for (int i = 1; i <= _productCount; i++)
        {
            _measurementGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = $"Product{i}",
                HeaderText = $"Ürün {i}",
                Width = 80
            });
        }
        
        // Result column
        _measurementGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Result",
            HeaderText = "Sonuç",
            Width = 80,
            ReadOnly = true
        });
        
        // Add rows
        foreach (var point in _selectedPlan.MeasurementPoints.OrderBy(p => p.BalloonNo))
        {
            var row = new DataGridViewRow();
            row.CreateCells(_measurementGrid);
            row.Cells[0].Value = point.BalloonNo;
            row.Cells[1].Value = point.Dimension;
            row.Cells[2].Value = point.NominalValue;
            row.Cells[3].Value = $"{point.LowerTolerance} / +{point.UpperTolerance}";
            row.Cells[_measurementGrid.Columns.Count - 1].Value = "Beklemede";
            row.Tag = point;
            _measurementGrid.Rows.Add(row);
        }
        
        _measurementGrid.CellValueChanged += MeasurementGrid_CellValueChanged;
    }
    
    private void MeasurementGrid_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        
        var row = _measurementGrid.Rows[e.RowIndex];
        var point = row.Tag as MeasurementPoint;
        if (point == null) return;
        
        // Check all product values for this row
        bool allOk = true;
        bool anyNok = false;
        
        for (int i = 4; i < _measurementGrid.Columns.Count - 1; i++)
        {
            var value = row.Cells[i].Value?.ToString();
            if (string.IsNullOrEmpty(value)) continue;
            
            if (decimal.TryParse(value, out decimal measuredValue) &&
                decimal.TryParse(point.NominalValue, out decimal nominalValue) &&
                decimal.TryParse(point.LowerTolerance, out decimal lowerTol) &&
                decimal.TryParse(point.UpperTolerance, out decimal upperTol))
            {
                var deviation = measuredValue - nominalValue;
                if (deviation < -lowerTol || deviation > upperTol)
                {
                    anyNok = true;
                    allOk = false;
                }
            }
        }
        
        var resultCell = row.Cells[_measurementGrid.Columns.Count - 1];
        if (anyNok)
        {
            resultCell.Value = "NOK";
            resultCell.Style.BackColor = ThemeColors.ErrorLight;
            resultCell.Style.ForeColor = ThemeColors.Error;
        }
        else if (allOk && row.Cells.Cast<DataGridViewCell>().Skip(4).Take(_productCount).Any(c => !string.IsNullOrEmpty(c.Value?.ToString())))
        {
            resultCell.Value = "OK";
            resultCell.Style.BackColor = ThemeColors.SuccessLight;
            resultCell.Style.ForeColor = ThemeColors.Success;
        }
    }
    
    private void BtnClear_Click(object? sender, EventArgs e)
    {
        if (MessageBox.Show("Tüm verileri temizlemek istediğinize emin misiniz?", "Onay", 
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            _planCombo.SelectedIndex = 0;
            _productCount.Value = 5;
            
            // Clear all textboxes
            foreach (Control ctrl in Controls[0].Controls[0].Controls[0].Controls)
            {
                if (ctrl is TextBox txt) txt.Text = "";
                if (ctrl is DateTimePicker dtp) dtp.Value = DateTime.Now;
                if (ctrl is ComboBox cmb && cmb != _planCombo) cmb.SelectedIndex = 0;
            }
        }
    }
    
    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (_selectedPlan == null)
        {
            MessageBox.Show("Lütfen bir kalite planı seçin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        // Collect data
        var measurement = new Measurement
        {
            PlanId = _selectedPlan.Id,
            ControlDate = ((DateTimePicker)FindControl("dtpDate")!).Value.ToString("yyyy-MM-dd"),
            InvoiceNo = ((TextBox)FindControl("txtInvoice")!).Text,
            BatchNo = ((TextBox)FindControl("txtBatch")!).Text,
            QualityType = ((ComboBox)FindControl("cmbQualityType")!).SelectedIndex switch
            {
                0 => "final",
                1 => "first",
                2 => "process",
                _ => "final"
            },
            ControlledBy = ((TextBox)FindControl("txtControlled")!).Text,
            ApprovedBy = ((TextBox)FindControl("txtApproved")!).Text,
            DeviceCodes = ((TextBox)FindControl("txtDevices")!).Text,
            Notes = ((TextBox)FindControl("txtNotes")!).Text
        };
        
        // Calculate overall result
        bool allOk = true;
        bool anyNok = false;
        
        // Add details
        foreach (DataGridViewRow row in _measurementGrid.Rows)
        {
            if (row.Tag is not MeasurementPoint point) continue;
            
            for (int i = 0; i < _productCount; i++)
            {
                var colIndex = 4 + i;
                if (colIndex >= _measurementGrid.Columns.Count - 1) break;
                
                var value = row.Cells[colIndex].Value?.ToString();
                if (string.IsNullOrEmpty(value)) continue;
                
                var detail = new MeasurementDetail
                {
                    MeasurementPointId = point.Id,
                    MeasuredValue = value,
                    Result = "OK"
                };
                
                if (decimal.TryParse(value, out decimal measuredValue) &&
                    decimal.TryParse(point.NominalValue, out decimal nominalValue) &&
                    decimal.TryParse(point.LowerTolerance, out decimal lowerTol) &&
                    decimal.TryParse(point.UpperTolerance, out decimal upperTol))
                {
                    var deviation = measuredValue - nominalValue;
                    if (deviation < -lowerTol || deviation > upperTol)
                    {
                        detail.Result = "NOK";
                        anyNok = true;
                        allOk = false;
                    }
                }
                
                measurement.Details.Add(detail);
            }
        }
        
        // Add product results
        for (int i = 0; i < _productCount; i++)
        {
            bool productOk = true;
            
            foreach (DataGridViewRow row in _measurementGrid.Rows)
            {
                var colIndex = 4 + i;
                if (colIndex >= _measurementGrid.Columns.Count - 1) break;
                
                var result = row.Cells[_measurementGrid.Columns.Count - 1].Value?.ToString();
                if (result == "NOK")
                {
                    productOk = false;
                    break;
                }
            }
            
            measurement.ProductResults.Add(new ProductResult
            {
                ProductIndex = i + 1,
                Result = productOk ? "OK" : "NOK"
            });
        }
        
        measurement.OverallResult = anyNok ? "NOK" : (allOk && measurement.Details.Count > 0 ? "OK" : "Beklemede");
        
        try
        {
            DatabaseManager.Instance.SaveMeasurement(measurement);
            MessageBox.Show("Ölçüm başarıyla kaydedildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            // Clear form
            BtnClear_Click(null, null);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Kaydetme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    private Control? FindControl(string name)
    {
        foreach (Control ctrl in Controls[0].Controls[0].Controls[0].Controls)
        {
            if (ctrl.Name == name) return ctrl;
        }
        
        // Search in additional panel
        if (Controls[0].Controls[0].Controls[1] is Panel panel)
        {
            foreach (Control ctrl in panel.Controls[0].Controls)
            {
                if (ctrl.Name == name) return ctrl;
            }
        }
        
        return null;
    }
}
