# Pro Kalite Kontrol Sistemi v2.0

## 🛡️ ESET-Proof Windows Forms Uygulaması

Bu uygulama, ESET ve diğer antivirüs yazılımları tarafından engellenmeyen, tamamen çevrimdışı çalışan, flashdisk'ten taşınabilir bir kalite kontrol yönetim sistemidir.

## ✨ Özellikler

### 🎯 Temel Özellikler
- **Kalite Kontrol Planları**: Ürün bazlı ölçüm planları oluşturma ve yönetme
- **Ölçüm Raporları**: Detaylı ölçüm girişi ve otomatik sonuç değerlendirme
- **Excel Entegrasyonu**: İçe/Dışa aktarma (XLSX formatı)
- **Raporlama**: İstatistiksel analiz ve grafikler
- **Veritabanı Yedekleme**: Tek tıkla yedekleme

### 🎨 Modern UI
- Material Design 2.0 tarzı arayüz
- Yumuşak animasyonlar ve gölgeler
- Responsive layout
- Yüksek DPI desteği

### 🔒 Güvenlik & Taşınabilirlik
- **ESET-Proof**: Native Windows Forms, antivirüs uyumlu
- **Flashdisk Uyumlu**: Kurulum gerektirmez, USB'den çalışır
- **SQLite Veritabanı**: Tek dosya, taşınabilir
- **Otomatik Yedekleme**: Veri kaybını önler

## 📁 Proje Yapısı

```
KaliteKontrolApp/
├── Models/                 # Veri modelleri
│   ├── QualityPlan.cs     # Kalite planı modeli
│   ├── Measurement.cs     # Ölçüm modeli
│   └── Settings.cs        # Ayarlar modeli
├── Forms/                 # UI formları
│   ├── MainForm.cs        # Ana form
│   ├── DashboardControl.cs # Ana sayfa
│   ├── PlansControl.cs    # Planlar listesi
│   ├── PlanEditForm.cs    # Plan düzenleme
│   ├── MeasurementControl.cs # Ölçüm girişi
│   ├── MeasurementsListControl.cs # Ölçüm listesi
│   ├── MeasurementViewForm.cs # Ölçüm görüntüleme
│   ├── ReportsControl.cs  # Raporlar
│   ├── SettingsControl.cs # Ayarlar
│   └── CalculatorForm.cs  # Hesap makinesi
├── Controls/              # Özel kontroller
│   ├── ModernButton.cs    # Modern butonlar
│   ├── ModernTextBox.cs   # Modern textbox
│   ├── ModernCard.cs      # Kart bileşeni
│   └── ModernDataGrid.cs  # Modern DataGrid
├── Utils/                 # Yardımcı sınıflar
│   ├── DatabaseManager.cs # SQLite yönetimi
│   ├── ThemeColors.cs     # Tema renkleri
│   └── ExcelHelper.cs     # Excel işlemleri
├── Program.cs             # Giriş noktası
├── KaliteKontrolApp.csproj # Proje dosyası
└── app.manifest           # Uygulama manifesti
```

## 🚀 Kurulum & Çalıştırma

### Gereksinimler
- Windows 7 veya üzeri
- .NET 8.0 Runtime (tek dosya yayını ile birlikte gelir)

### Flashdisk'ten Çalıştırma
1. `KaliteKontrolApp.exe` dosyasını flashdisk'e kopyalayın
2. Uygulamayı çift tıklayarak çalıştırın
3. `Data` klasörü otomatik oluşturulacak ve veritabanı burada saklanacak

### Geliştirme Ortamı
```bash
# Projeyi klonlayın
git clone <repo-url>

# Bağımlılıkları yükleyin
dotnet restore

# Uygulamayı çalıştırın
dotnet run

# Yayın oluşturun (tek dosya)
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## 📊 Veritabanı Şeması

### QualityPlans
| Alan | Tip | Açıklama |
|------|-----|----------|
| Id | INTEGER | Birincil anahtar |
| ProductName | TEXT | Ürün adı |
| ProductCode | TEXT | Ürün kodu |
| Customer | TEXT | Müşteri |
| DrawingNo | TEXT | Çizim numarası |
| BalloonCount | INTEGER | Balon sayısı |
| CreatedAt | TEXT | Oluşturma tarihi |
| IsActive | INTEGER | Aktif/Pasif |

### MeasurementPoints
| Alan | Tip | Açıklama |
|------|-----|----------|
| Id | INTEGER | Birincil anahtar |
| PlanId | INTEGER | Plan referansı |
| BalloonNo | INTEGER | Balon numarası |
| Dimension | TEXT | Ölçü adı |
| NominalValue | TEXT | Nominal değer |
| LowerTolerance | TEXT | Alt tolerans |
| UpperTolerance | TEXT | Üst tolerans |

### Measurements
| Alan | Tip | Açıklama |
|------|-----|----------|
| Id | INTEGER | Birincil anahtar |
| PlanId | INTEGER | Plan referansı |
| ControlDate | TEXT | Kontrol tarihi |
| InvoiceNo | TEXT | Fatura numarası |
| BatchNo | TEXT | Parti numarası |
| QualityType | TEXT | Kalite tipi |
| OverallResult | TEXT | Genel sonuç |

## 🎨 Tema Renkleri

```csharp
Primary: #1976D2 (Mavi)
Secondary: #009688 (Yeşil)
Success: #4CAF50 (Başarılı)
Error: #F44336 (Hata)
Warning: #FF9800 (Uyarı)
Background: #F8F9FA (Arka plan)
```

## 🔧 Özelleştirme

### Şirket Bilgileri
Ayarlar sayfasından şirket adı ve logosu değiştirilebilir.

### Varsayılan Değerler
- Varsayılan kontrol eden kişi
- Varsayılan onaylayan kişi
- Varsayılan ölçüm adedi

## 📱 Ekran Görüntüleri

*(Ekran görüntüleri buraya eklenecek)*

## 🐛 Hata Ayıklama

### Veritabanı Hataları
- `Data` klasörünün yazma izinlerini kontrol edin
- Veritabanı dosyasının bozulmadığını doğrulayın

### Excel Hataları
- EPPlus kütüphanesinin yüklü olduğundan emin olun
- Dosya izinlerini kontrol edin

## 📝 Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

## 🤝 Katkıda Bulunma

1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add amazing feature'`)
4. Branch'e push yapın (`git push origin feature/amazing-feature`)
5. Pull Request açın

## 📞 İletişim

Sorularınız ve önerileriniz için issue açabilirsiniz.

---

**Not:** Bu uygulama ESET ve diğer antivirüs yazılımları tarafından engellenmez çünkü:
- Native Windows Forms kullanır
- Şüpheli ağ aktivitesi yoktur
- SQLite yerel veritabanı kullanır
- İmza sertifikası gerektirmez
