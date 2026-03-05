using KaliteKontrolApp.Utils;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KaliteKontrolApp.Controls;

public class ModernTextBox : TextBox
{
    private Color _borderColor = ThemeColors.Border;
    private Color _focusedColor = ThemeColors.Primary;
    private Color _placeholderColor = ThemeColors.TextMuted;
    private string _placeholder = string.Empty;
    private bool _isFocused = false;
    
    public Color BorderColor 
    { 
        get => _borderColor; 
        set { _borderColor = value; Invalidate(); }
    }
    
    public string Placeholder 
    { 
        get => _placeholder; 
        set { _placeholder = value; Invalidate(); }
    }
    
    public ModernTextBox()
    {
        BorderStyle = BorderStyle.None;
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.White;
        ForeColor = ThemeColors.TextPrimary;
        Height = 40;
        Padding = new Padding(12, 8, 12, 8);
        
        GotFocus += (s, e) => { _isFocused = true; Invalidate(); };
        LostFocus += (s, e) => { _isFocused = false; Invalidate(); };
        TextChanged += (s, e) => Invalidate();
    }
    
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        
        // Draw background
        using var bgBrush = new SolidBrush(BackColor);
        g.FillRectangle(bgBrush, rect);
        
        // Draw border
        var currentBorderColor = _isFocused ? _focusedColor : _borderColor;
        using var borderPen = new Pen(currentBorderColor, _isFocused ? 2 : 1);
        g.DrawRectangle(borderPen, rect);
        
        // Draw placeholder
        if (string.IsNullOrEmpty(Text) && !string.IsNullOrEmpty(_placeholder) && !_isFocused)
        {
            using var placeholderBrush = new SolidBrush(_placeholderColor);
            var placeholderRect = new Rectangle(12, 8, Width - 24, Height - 16);
            g.DrawString(_placeholder, Font, placeholderBrush, placeholderRect);
        }
    }
    
    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x000F) // WM_PAINT
        {
            base.WndProc(ref m);
            OnPaint(new PaintEventArgs(Graphics.FromHwnd(Handle), ClientRectangle));
        }
        else
        {
            base.WndProc(ref m);
        }
    }
}

public class ModernComboBox : ComboBox
{
    private Color _borderColor = ThemeColors.Border;
    private Color _focusedColor = ThemeColors.Primary;
    
    public Color BorderColor 
    { 
        get => _borderColor; 
        set { _borderColor = value; Invalidate(); }
    }
    
    public ModernComboBox()
    {
        FlatStyle = FlatStyle.Flat;
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.White;
        ForeColor = ThemeColors.TextPrimary;
        Height = 40;
        DropDownStyle = ComboBoxStyle.DropDownList;
    }
    
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        
        // Draw background
        using var bgBrush = new SolidBrush(BackColor);
        g.FillRectangle(bgBrush, rect);
        
        // Draw border
        using var borderPen = new Pen(Focused ? _focusedColor : _borderColor, Focused ? 2 : 1);
        g.DrawRectangle(borderPen, rect);
        
        // Draw dropdown arrow
        var arrowX = Width - 20;
        var arrowY = Height / 2;
        using var arrowBrush = new SolidBrush(ThemeColors.TextSecondary);
        var points = new[]
        {
            new Point(arrowX - 4, arrowY - 2),
            new Point(arrowX + 4, arrowY - 2),
            new Point(arrowX, arrowY + 3)
        };
        g.FillPolygon(arrowBrush, points);
        
        // Draw text
        if (SelectedItem != null)
        {
            using var textBrush = new SolidBrush(ForeColor);
            g.DrawString(SelectedItem.ToString(), Font, textBrush, 12, (Height - Font.Height) / 2);
        }
    }
}

public class ModernNumericUpDown : NumericUpDown
{
    private Color _borderColor = ThemeColors.Border;
    private Color _focusedColor = ThemeColors.Primary;
    
    public ModernNumericUpDown()
    {
        BorderStyle = BorderStyle.FixedSingle;
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.White;
        ForeColor = ThemeColors.TextPrimary;
        Height = 40;
        Controls[0].Visible = false; // Hide default buttons
    }
    
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        
        var g = e.Graphics;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        
        // Draw border
        using var borderPen = new Pen(Focused ? _focusedColor : _borderColor, Focused ? 2 : 1);
        g.DrawRectangle(borderPen, rect);
    }
}

public class SearchTextBox : TextBox
{
    private string _placeholder = "Ara...";
    
    public string Placeholder 
    { 
        get => _placeholder; 
        set { _placeholder = value; Invalidate(); }
    }
    
    public SearchTextBox()
    {
        BorderStyle = BorderStyle.None;
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.White;
        ForeColor = ThemeColors.TextPrimary;
        Height = 44;
        
        // Add search icon
        var iconPanel = new Panel
        {
            Width = 40,
            Height = Height,
            Dock = DockStyle.Left,
            BackColor = Color.White
        };
        
        iconPanel.Paint += (s, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw search icon
            using var pen = new Pen(ThemeColors.TextMuted, 2);
            var circleRect = new Rectangle(10, 12, 14, 14);
            g.DrawEllipse(pen, circleRect);
            g.DrawLine(pen, 21, 23, 26, 28);
        };
        
        Controls.Add(iconPanel);
        Padding = new Padding(44, 10, 12, 10);
    }
    
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        
        // Draw rounded background
        using var path = GetRoundedPath(rect, 8);
        using var bgBrush = new SolidBrush(BackColor);
        g.FillPath(bgBrush, path);
        
        // Draw border
        using var borderPen = new Pen(ThemeColors.Border, 1);
        g.DrawPath(borderPen, path);
        
        // Draw placeholder
        if (string.IsNullOrEmpty(Text) && !Focused)
        {
            using var placeholderBrush = new SolidBrush(ThemeColors.TextMuted);
            g.DrawString(_placeholder, Font, placeholderBrush, 44, (Height - Font.Height) / 2);
        }
    }
    
    private System.Drawing.Drawing2D.GraphicsPath GetRoundedPath(Rectangle rect, int radius)
    {
        var path = new System.Drawing.Drawing2D.GraphicsPath();
        int diameter = radius * 2;
        
        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        
        return path;
    }
}
