using KaliteKontrolApp.Forms;
using KaliteKontrolApp.Utils;
using System;
using System.IO;
using System.Windows.Forms;

namespace KaliteKontrolApp;

static class Program
{
    public static string AppVersion = "2.0.0";
    public static string DatabasePath { get; private set; } = string.Empty;
    public static string AppDataPath { get; private set; } = string.Empty;
    
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        
        // Flashdisk/Portable mod - uygulamanın bulunduğu klasörde data klasörü oluştur
        string exePath = Application.ExecutablePath;
        string appDirectory = Path.GetDirectoryName(exePath) ?? AppDomain.CurrentDomain.BaseDirectory;
        
        // Data klasörünü uygulama yanında oluştur (taşınabilirlik için)
        AppDataPath = Path.Combine(appDirectory, "Data");
        if (!Directory.Exists(AppDataPath))
        {
            Directory.CreateDirectory(AppDataPath);
        }
        
        // Veritabanı yolu
        DatabasePath = Path.Combine(AppDataPath, "kalitekontrol.db");
        
        // Veritabanını başlat
        DatabaseManager.Instance.Initialize();
        
        // Ana formu başlat
        Application.Run(new MainForm());
    }
}
