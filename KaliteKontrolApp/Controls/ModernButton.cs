using KaliteKontrolApp.Utils;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KaliteKontrolApp.Controls;

public class ModernButton : Button
{
    private int _borderRadius = 8;
    private Color _buttonColor = ThemeColors.Primary;
    private Color _hoverColor = ThemeColors.PrimaryDark;
    private Color _pressedColor = ThemeColors.PrimaryDark;
    private bool _isHovering = false;
    private bool _isPressed = false;
    
    public int BorderRadius 
    { 
        get => _borderRadius; 
        set { _borderRadius = value; Invalidate(); }
    }
    
    public Color ButtonColor 
    { 
        get => _buttonColor; 
        set { _buttonColor = value; Invalidate(); }
    }
    
    public ModernButton()
    {
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        ForeColor = Color.White;
        BackColor = _buttonColor;
        Size = new Size(120, 40);
        Cursor = Cursors.Hand;
        
        MouseEnter += (s, e) => { _isHovering = true; Invalidate(); };
        MouseLeave += (s, e) => { _isHovering = false; _isPressed = false; Invalidate(); };
        MouseDown += (s, e) => { _isPressed = true; Invalidate(); };
        MouseUp += (s, e) => { _isPressed = false; Invalidate(); };
    }
    
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        
        // Determine color based on state
        Color currentColor = _buttonColor;
        if (_isPressed)
            currentColor = _pressedColor;
        else if (_isHovering)
            currentColor = _hoverColor;
        
        // Draw rounded rectangle
        using var path = GetRoundedPath(rect, _borderRadius);
        using var brush = new SolidBrush(currentColor);
        g.FillPath(brush, path);
        
        // Draw shadow for elevation effect
        if (!_isPressed)
        {
            using var shadowPath = GetRoundedPath(new Rectangle(0, 2, Width - 1, Height - 1), _borderRadius);
            using var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0));
            g.FillPath(shadowBrush, shadowPath);
            g.FillPath(brush, path);
        }
        
        // Draw text
        var textRect = new Rectangle(0, 0, Width, Height);
        TextRenderer.DrawText(g, Text, Font, textRect, ForeColor, 
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
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
}

public class ModernOutlineButton : Button
{
    private int _borderRadius = 8;
    private Color _borderColor = ThemeColors.Primary;
    private Color _hoverBgColor = ThemeColors.Primary50;
    private bool _isHovering = false;
    
    public int BorderRadius 
    { 
        get => _borderRadius; 
        set { _borderRadius = value; Invalidate(); }
    }
    
    public Color BorderColor 
    { 
        get => _borderColor; 
        set { _borderColor = value; Invalidate(); }
    }
    
    public ModernOutlineButton()
    {
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        ForeColor = _borderColor;
        BackColor = Color.White;
        Size = new Size(120, 40);
        Cursor = Cursors.Hand;
        
        MouseEnter += (s, e) => { _isHovering = true; Invalidate(); };
        MouseLeave += (s, e) => { _isHovering = false; Invalidate(); };
    }
    
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        
        using var path = GetRoundedPath(rect, _borderRadius);
        
        // Fill background on hover
        if (_isHovering)
        {
            using var bgBrush = new SolidBrush(_hoverBgColor);
            g.FillPath(bgBrush, path);
        }
        else
        {
            using var bgBrush = new SolidBrush(BackColor);
            g.FillPath(bgBrush, path);
        }
        
        // Draw border
        using var pen = new Pen(_borderColor, 1.5f);
        g.DrawPath(pen, path);
        
        // Draw text
        var textRect = new Rectangle(0, 0, Width, Height);
        TextRenderer.DrawText(g, Text, Font, textRect, ForeColor, 
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
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
}

public class IconButton : Button
{
    private Image? _icon;
    private int _iconSize = 20;
    
    public Image? Icon 
    { 
        get => _icon; 
        set { _icon = value; Invalidate(); }
    }
    
    public int IconSize 
    { 
        get => _iconSize; 
        set { _iconSize = value; Invalidate(); }
    }
    
    public IconButton()
    {
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        BackColor = Color.Transparent;
        Size = new Size(40, 40);
        Cursor = Cursors.Hand;
    }
    
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        
        // Draw hover background
        if (ClientRectangle.Contains(PointToClient(Cursor.Position)))
        {
            using var brush = new SolidBrush(Color.FromArgb(30, 0, 0, 0));
            g.FillEllipse(brush, 4, 4, Width - 8, Height - 8);
        }
        
        // Draw icon
        if (_icon != null)
        {
            var iconRect = new Rectangle((Width - _iconSize) / 2, (Height - _iconSize) / 2, _iconSize, _iconSize);
            g.DrawImage(_icon, iconRect);
        }
    }
}
