using KaliteKontrolApp.Utils;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace KaliteKontrolApp.Controls;

public class ModernDataGrid : DataGridView
{
    public ModernDataGrid()
    {
        InitializeStyles();
    }
    
    private void InitializeStyles()
    {
        // Basic settings
        BorderStyle = BorderStyle.None;
        BackgroundColor = ThemeColors.Background;
        GridColor = ThemeColors.BorderLight;
        
        // Column headers
        EnableHeadersVisualStyles = false;
        ColumnHeadersDefaultCellStyle.BackColor = ThemeColors.Background;
        ColumnHeadersDefaultCellStyle.ForeColor = ThemeColors.TextPrimary;
        ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        ColumnHeadersDefaultCellStyle.Padding = new Padding(12, 8, 12, 8);
        ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        ColumnHeadersHeight = 48;
        ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        
        // Rows
        DefaultCellStyle.BackColor = Color.White;
        DefaultCellStyle.ForeColor = ThemeColors.TextPrimary;
        DefaultCellStyle.Font = new Font("Segoe UI", 10F);
        DefaultCellStyle.Padding = new Padding(12, 8, 12, 8);
        DefaultCellStyle.SelectionBackColor = ThemeColors.Primary50;
        DefaultCellStyle.SelectionForeColor = ThemeColors.TextPrimary;
        
        AlternatingRowsDefaultCellStyle.BackColor = Color.White;
        
        RowTemplate.Height = 52;
        RowHeadersVisible = false;
        SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        MultiSelect = false;
        
        // Appearance
        CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        AllowUserToAddRows = false;
        AllowUserToDeleteRows = false;
        AllowUserToResizeRows = false;
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        
        // Scrollbars
        ScrollBars = ScrollBars.Both;
        
        // Events
        CellPainting += ModernDataGrid_CellPainting;
        RowsAdded += ModernDataGrid_RowsAdded;
    }
    
    private void ModernDataGrid_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        if (e.RowIndex == -1) // Header
        {
            e.PaintBackground(e.CellBounds, true);
            
            // Draw bottom border for header
            using var borderPen = new Pen(ThemeColors.Border, 1);
            e.Graphics.DrawLine(borderPen, e.CellBounds.Left, e.CellBounds.Bottom - 1, 
                e.CellBounds.Right, e.CellBounds.Bottom - 1);
            
            // Draw text
            TextRenderer.DrawText(e.Graphics, e.Value?.ToString() ?? "", e.CellStyle.Font,
                new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y, e.CellBounds.Width - 24, e.CellBounds.Height),
                e.CellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            
            e.Handled = true;
        }
    }
    
    private void ModernDataGrid_RowsAdded(object? sender, DataGridViewRowsAddedEventArgs e)
    {
        // Add hover effect
        for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
        {
            if (i < Rows.Count)
            {
                Rows[i].DefaultCellStyle.BackColor = i % 2 == 0 ? Color.White : Color.FromArgb(250, 250, 250);
            }
        }
    }
    
    public void AddActionColumn(string name, string header, Image? icon = null)
    {
        var column = new DataGridViewImageColumn
        {
            Name = name,
            HeaderText = header,
            Image = icon,
            Width = 60,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
        };
        Columns.Add(column);
    }
    
    public void AddStatusColumn(string name, string header)
    {
        var column = new DataGridViewTextBoxColumn
        {
            Name = name,
            HeaderText = header,
            Width = 100,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        };
        Columns.Add(column);
    }
    
    public void StyleStatusCell(DataGridViewCell cell, string status)
    {
        switch (status.ToUpper())
        {
            case "OK":
            case "UYGUN":
                cell.Style.BackColor = ThemeColors.SuccessLight;
                cell.Style.ForeColor = ThemeColors.Success;
                cell.Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                break;
            case "NOK":
            case "UYGUN DEĞİL":
                cell.Style.BackColor = ThemeColors.ErrorLight;
                cell.Style.ForeColor = ThemeColors.Error;
                cell.Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                break;
            case "CONDITIONAL":
            case "ŞARTLI":
                cell.Style.BackColor = ThemeColors.WarningLight;
                cell.Style.ForeColor = ThemeColors.Warning;
                cell.Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                break;
            default:
                cell.Style.BackColor = ThemeColors.InfoLight;
                cell.Style.ForeColor = ThemeColors.Info;
                break;
        }
    }
}

public class ModernListView : ListView
{
    public ModernListView()
    {
        View = View.Details;
        FullRowSelect = true;
        GridLines = false;
        BorderStyle = BorderStyle.None;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 10F);
        HeaderStyle = ColumnHeaderStyle.Nonclickable;
        OwnerDraw = true;
        
        DrawColumnHeader += ModernListView_DrawColumnHeader;
        DrawItem += ModernListView_DrawItem;
        DrawSubItem += ModernListView_DrawSubItem;
    }
    
    private void ModernListView_DrawColumnHeader(object? sender, DrawListViewColumnHeaderEventArgs e)
    {
        // Header background
        using var bgBrush = new SolidBrush(ThemeColors.Background);
        e.Graphics.FillRectangle(bgBrush, e.Bounds);
        
        // Header text
        using var textBrush = new SolidBrush(ThemeColors.TextPrimary);
        using var font = new Font("Segoe UI", 10F, FontStyle.Bold);
        var textRect = new Rectangle(e.Bounds.X + 12, e.Bounds.Y, e.Bounds.Width - 24, e.Bounds.Height);
        e.Graphics.DrawString(e.Header.Text, font, textBrush, textRect);
        
        // Bottom border
        using var borderPen = new Pen(ThemeColors.Border, 1);
        e.Graphics.DrawLine(borderPen, e.Bounds.Left, e.Bounds.Bottom - 1, 
            e.Bounds.Right, e.Bounds.Bottom - 1);
    }
    
    private void ModernListView_DrawItem(object? sender, DrawListViewItemEventArgs e)
    {
        // Item background
        var bgColor = e.ItemIndex % 2 == 0 ? Color.White : Color.FromArgb(250, 250, 250);
        if (e.Item.Selected)
            bgColor = ThemeColors.Primary50;
        
        using var bgBrush = new SolidBrush(bgColor);
        e.Graphics.FillRectangle(bgBrush, e.Bounds);
    }
    
    private void ModernListView_DrawSubItem(object? sender, DrawListViewSubItemEventArgs e)
    {
        var textColor = e.Item.Selected ? ThemeColors.TextPrimary : ThemeColors.TextPrimary;
        using var textBrush = new SolidBrush(textColor);
        
        var textRect = new Rectangle(e.Bounds.X + 12, e.Bounds.Y, e.Bounds.Width - 24, e.Bounds.Height);
        e.Graphics.DrawString(e.SubItem.Text, Font, textBrush, textRect);
    }
}
