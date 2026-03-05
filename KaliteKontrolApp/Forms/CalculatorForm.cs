using KaliteKontrolApp.Utils;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KaliteKontrolApp.Forms;

public partial class CalculatorForm : Form
{
    private string _currentInput = "0";
    private string _previousInput = "";
    private string _operation = "";
    private bool _newNumber = true;
    
    private Label _displayLabel = null!;
    private Label _previewLabel = null!;
    
    public CalculatorForm()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        Text = "Hesap Makinesi";
        Size = new Size(320, 450);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(45, 45, 48);
        FormBorderStyle = FormBorderStyle.Sizable;
        MinimumSize = new Size(280, 400);
        MaximizeBox = false;
        
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            Padding = new Padding(0)
        };
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        
        // Display panel
        var displayPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(45, 45, 48),
            Padding = new Padding(16)
        };
        
        _previewLabel = new Label
        {
            Text = "",
            Font = new Font("Segoe UI", 12F),
            ForeColor = Color.Gray,
            Dock = DockStyle.Top,
            Height = 24,
            TextAlign = ContentAlignment.MiddleRight
        };
        
        _displayLabel = new Label
        {
            Text = "0",
            Font = new Font("Segoe UI", 36F, FontStyle.Regular),
            ForeColor = Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleRight
        };
        
        displayPanel.Controls.Add(_displayLabel);
        displayPanel.Controls.Add(_previewLabel);
        mainLayout.Controls.Add(displayPanel, 0, 0);
        
        // Buttons panel
        var buttonsPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 5,
            ColumnCount = 4,
            Padding = new Padding(8)
        };
        
        for (int i = 0; i < 5; i++)
            buttonsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
        for (int i = 0; i < 4; i++)
            buttonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        
        // Row 1: C, ⌫, ÷, ×
        buttonsPanel.Controls.Add(CreateButton("C", Color.FromArgb(255, 89, 94), ButtonType.Clear), 0, 0);
        buttonsPanel.Controls.Add(CreateButton("⌫", Color.FromArgb(255, 159, 67), ButtonType.Backspace), 1, 0);
        buttonsPanel.Controls.Add(CreateButton("÷", Color.FromArgb(54, 162, 235), ButtonType.Operator), 2, 0);
        buttonsPanel.Controls.Add(CreateButton("×", Color.FromArgb(54, 162, 235), ButtonType.Operator), 3, 0);
        
        // Row 2: 7, 8, 9, -
        buttonsPanel.Controls.Add(CreateButton("7", Color.FromArgb(64, 64, 64), ButtonType.Number), 0, 1);
        buttonsPanel.Controls.Add(CreateButton("8", Color.FromArgb(64, 64, 64), ButtonType.Number), 1, 1);
        buttonsPanel.Controls.Add(CreateButton("9", Color.FromArgb(64, 64, 64), ButtonType.Number), 2, 1);
        buttonsPanel.Controls.Add(CreateButton("-", Color.FromArgb(54, 162, 235), ButtonType.Operator), 3, 1);
        
        // Row 3: 4, 5, 6, +
        buttonsPanel.Controls.Add(CreateButton("4", Color.FromArgb(64, 64, 64), ButtonType.Number), 0, 2);
        buttonsPanel.Controls.Add(CreateButton("5", Color.FromArgb(64, 64, 64), ButtonType.Number), 1, 2);
        buttonsPanel.Controls.Add(CreateButton("6", Color.FromArgb(64, 64, 64), ButtonType.Number), 2, 2);
        buttonsPanel.Controls.Add(CreateButton("+", Color.FromArgb(54, 162, 235), ButtonType.Operator), 3, 2);
        
        // Row 4: 1, 2, 3, =
        buttonsPanel.Controls.Add(CreateButton("1", Color.FromArgb(64, 64, 64), ButtonType.Number), 0, 3);
        buttonsPanel.Controls.Add(CreateButton("2", Color.FromArgb(64, 64, 64), ButtonType.Number), 1, 3);
        buttonsPanel.Controls.Add(CreateButton("3", Color.FromArgb(64, 64, 64), ButtonType.Number), 2, 3);
        buttonsPanel.Controls.Add(CreateButton("=", Color.FromArgb(76, 175, 80), ButtonType.Equals), 3, 3);
        buttonsPanel.SetRowSpan(buttonsPanel.Controls[buttonsPanel.Controls.Count - 1], 2);
        
        // Row 5: 0, .
        buttonsPanel.Controls.Add(CreateButton("0", Color.FromArgb(64, 64, 64), ButtonType.Number), 0, 4);
        buttonsPanel.SetColumnSpan(buttonsPanel.Controls[buttonsPanel.Controls.Count - 1], 2);
        buttonsPanel.Controls.Add(CreateButton(".", Color.FromArgb(64, 64, 64), ButtonType.Decimal), 2, 4);
        
        mainLayout.Controls.Add(buttonsPanel, 0, 1);
        Controls.Add(mainLayout);
    }
    
    private enum ButtonType { Number, Operator, Equals, Clear, Backspace, Decimal }
    
    private Button CreateButton(string text, Color backColor, ButtonType type)
    {
        var btn = new Button
        {
            Text = text,
            Font = new Font("Segoe UI", 16F, FontStyle.Regular),
            ForeColor = Color.White,
            BackColor = backColor,
            FlatStyle = FlatStyle.Flat,
            Dock = DockStyle.Fill,
            Margin = new Padding(4),
            Cursor = Cursors.Hand,
            Tag = type
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backColor, 0.1f);
        btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backColor, 0.1f);
        
        // Make buttons rounded
        btn.Paint += (s, e) =>
        {
            var rect = new Rectangle(0, 0, btn.Width - 1, btn.Height - 1);
            using var path = GetRoundedPath(rect, 8);
            btn.Region = new Region(path);
        };
        
        btn.Click += Button_Click;
        return btn;
    }
    
    private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        int diameter = radius * 2;
        
        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        
        return path;
    }
    
    private void Button_Click(object? sender, EventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not ButtonType type) return;
        
        switch (type)
        {
            case ButtonType.Number:
                NumberPressed(btn.Text);
                break;
            case ButtonType.Operator:
                OperatorPressed(btn.Text);
                break;
            case ButtonType.Equals:
                Calculate();
                break;
            case ButtonType.Clear:
                Clear();
                break;
            case ButtonType.Backspace:
                Backspace();
                break;
            case ButtonType.Decimal:
                DecimalPressed();
                break;
        }
        
        UpdateDisplay();
    }
    
    private void NumberPressed(string number)
    {
        if (_newNumber)
        {
            _currentInput = number;
            _newNumber = false;
        }
        else
        {
            _currentInput = _currentInput == "0" ? number : _currentInput + number;
        }
    }
    
    private void OperatorPressed(string op)
    {
        if (!string.IsNullOrEmpty(_operation) && !_newNumber)
        {
            Calculate();
        }
        
        _previousInput = _currentInput;
        _operation = op;
        _newNumber = true;
    }
    
    private void Calculate()
    {
        if (string.IsNullOrEmpty(_operation) || string.IsNullOrEmpty(_previousInput)) return;
        
        if (double.TryParse(_previousInput, out double prev) && 
            double.TryParse(_currentInput, out double current))
        {
            double result = _operation switch
            {
                "+" => prev + current,
                "-" => prev - current,
                "×" => prev * current,
                "÷" => current != 0 ? prev / current : 0,
                _ => current
            };
            
            _currentInput = result.ToString("G12");
            _operation = "";
            _previousInput = "";
            _newNumber = true;
        }
    }
    
    private void Clear()
    {
        _currentInput = "0";
        _previousInput = "";
        _operation = "";
        _newNumber = true;
    }
    
    private void Backspace()
    {
        if (_currentInput.Length > 1)
        {
            _currentInput = _currentInput[..^1];
        }
        else
        {
            _currentInput = "0";
        }
    }
    
    private void DecimalPressed()
    {
        if (_newNumber)
        {
            _currentInput = "0.";
            _newNumber = false;
        }
        else if (!_currentInput.Contains("."))
        {
            _currentInput += ".";
        }
    }
    
    private void UpdateDisplay()
    {
        _displayLabel.Text = _currentInput;
        _previewLabel.Text = !string.IsNullOrEmpty(_operation) ? $"{_previousInput} {_operation}" : "";
    }
}
