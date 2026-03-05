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
    private int _targetProductCount = 5;
    
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
        _planCombo.SelectedIndexChanged +=
