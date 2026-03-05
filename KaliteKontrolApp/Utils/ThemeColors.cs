using System.Drawing;

namespace KaliteKontrolApp.Utils;

public static class ThemeColors
{
    // Primary Colors - Modern Blue
    public static Color Primary = Color.FromArgb(25, 118, 210);
    public static Color PrimaryDark = Color.FromArgb(21, 101, 192);
    public static Color PrimaryLight = Color.FromArgb(66, 165, 245);
    public static Color Primary50 = Color.FromArgb(227, 242, 253);
    
    // Secondary Colors - Teal
    public static Color Secondary = Color.FromArgb(0, 150, 136);
    public static Color SecondaryDark = Color.FromArgb(0, 121, 107);
    public static Color SecondaryLight = Color.FromArgb(77, 182, 172);
    
    // Status Colors
    public static Color Success = Color.FromArgb(76, 175, 80);
    public static Color SuccessLight = Color.FromArgb(232, 245, 233);
    public static Color Warning = Color.FromArgb(255, 152, 0);
    public static Color WarningLight = Color.FromArgb(255, 243, 224);
    public static Color Error = Color.FromArgb(244, 67, 54);
    public static Color ErrorLight = Color.FromArgb(255, 235, 238);
    public static Color Info = Color.FromArgb(33, 150, 243);
    public static Color InfoLight = Color.FromArgb(227, 242, 253);
    
    // Neutral Colors
    public static Color Background = Color.FromArgb(248, 249, 250);
    public static Color Surface = Color.White;
    public static Color Card = Color.White;
    public static Color Divider = Color.FromArgb(224, 224, 224);
    
    // Text Colors
    public static Color TextPrimary = Color.FromArgb(33, 37, 41);
    public static Color TextSecondary = Color.FromArgb(108, 117, 125);
    public static Color TextMuted = Color.FromArgb(134, 142, 150);
    public static Color TextOnPrimary = Color.White;
    
    // Sidebar
    public static Color SidebarBg = Color.White;
    public static Color SidebarText = Color.FromArgb(73, 80, 87);
    public static Color SidebarActive = Color.FromArgb(25, 118, 210);
    public static Color SidebarActiveBg = Color.FromArgb(227, 242, 253);
    public static Color SidebarHover = Color.FromArgb(248, 249, 250);
    
    // Border
    public static Color Border = Color.FromArgb(222, 226, 230);
    public static Color BorderLight = Color.FromArgb(233, 236, 239);
    
    // Shadow
    public static Color Shadow = Color.FromArgb(0, 0, 0, 12);
}
