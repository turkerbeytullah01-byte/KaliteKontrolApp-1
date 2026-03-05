using KaliteKontrolApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace KaliteKontrolApp.Utils;

public class DatabaseManager
{
    private static DatabaseManager? _instance;
    private readonly string _connectionString;
    
    public static DatabaseManager Instance => _instance ??= new DatabaseManager();
    
    private DatabaseManager()
    {
        _connectionString = $"Data Source={Program.DatabasePath};Version=3;";
    }
    
    public void Initialize()
    {
        if (!File.Exists(Program.DatabasePath))
        {
            SQLiteConnection.CreateFile(Program.DatabasePath);
        }
        
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        
        CreateTables(connection);
        SeedDefaultData(connection);
    }
    
    private void CreateTables(SQLiteConnection connection)
    {
        var commands = new[]
        {
            @"CREATE TABLE IF NOT EXISTS QualityPlans (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ProductName TEXT NOT NULL,
                ProductCode TEXT NOT NULL,
                Customer TEXT,
                DrawingNo TEXT,
                BalloonCount INTEGER DEFAULT 10,
                CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
                UpdatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
                IsActive INTEGER DEFAULT 1
            )",
            
            @"CREATE TABLE IF NOT EXISTS MeasurementPoints (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                PlanId INTEGER NOT NULL,
                BalloonNo INTEGER NOT NULL,
                Dimension TEXT,
                Description TEXT,
                NominalValue TEXT,
                LowerTolerance TEXT DEFAULT '0',
                UpperTolerance TEXT DEFAULT '0',
                MeasurementTool TEXT,
                Frequency TEXT DEFAULT 'Her parti',
                CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (PlanId) REFERENCES QualityPlans(Id) ON DELETE CASCADE
            )",
            
            @"CREATE TABLE IF NOT EXISTS Measurements (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                PlanId INTEGER NOT NULL,
                ControlDate TEXT NOT NULL,
                InvoiceNo TEXT,
                DeviceCodes TEXT,
                ControlledBy TEXT,
                ApprovedBy TEXT,
                BatchNo TEXT,
                QualityType TEXT DEFAULT 'final',
                Notes TEXT,
                CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
                OverallResult TEXT DEFAULT 'Beklemede',
                FOREIGN KEY (PlanId) REFERENCES QualityPlans(Id) ON DELETE CASCADE
            )",
            
            @"CREATE TABLE IF NOT EXISTS MeasurementDetails (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                MeasurementId INTEGER NOT NULL,
                MeasurementPointId INTEGER NOT NULL,
                MeasuredValue TEXT,
                Result TEXT DEFAULT 'Beklemede',
                Notes TEXT,
                FOREIGN KEY (MeasurementId) REFERENCES Measurements(Id) ON DELETE CASCADE,
                FOREIGN KEY (MeasurementPointId) REFERENCES MeasurementPoints(Id) ON DELETE CASCADE
            )",
            
            @"CREATE TABLE IF NOT EXISTS ProductResults (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                MeasurementId INTEGER NOT NULL,
                ProductIndex INTEGER NOT NULL,
                Result TEXT DEFAULT 'OK',
                FOREIGN KEY (MeasurementId) REFERENCES Measurements(Id) ON DELETE CASCADE
            )",
            
            @"CREATE TABLE IF NOT EXISTS AppSettings (
                Id INTEGER PRIMARY KEY CHECK (Id = 1),
                CompanyName TEXT DEFAULT 'Şirket Adı',
                LogoPath TEXT,
                DefaultControlledBy TEXT,
                DefaultApprovedBy TEXT,
                DefaultMeasurementCount INTEGER DEFAULT 5,
                Theme TEXT DEFAULT 'Light'
            )"
        };
        
        foreach (var cmd in commands)
        {
            using var command = new SQLiteCommand(cmd, connection);
            command.ExecuteNonQuery();
        }
    }
    
    private void SeedDefaultData(SQLiteConnection connection)
    {
        // Varsayılan ayarları ekle
        using var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM AppSettings", connection);
        var count = Convert.ToInt32(checkCmd.ExecuteScalar());
        
        if (count == 0)
        {
            using var cmd = new SQLiteCommand(@"
                INSERT INTO AppSettings (Id, CompanyName, DefaultMeasurementCount, Theme)
                VALUES (1, 'Şirket Adı', 5, 'Light')", connection);
            cmd.ExecuteNonQuery();
        }
    }
    
    // QualityPlan CRUD Operations
    public List<QualityPlan> GetAllPlans()
    {
        var plans = new List<QualityPlan>();
        
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        
        using var cmd = new SQLiteCommand(@"
            SELECT * FROM QualityPlans WHERE IsActive = 1 ORDER BY UpdatedAt DESC", connection);
        
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            plans.Add(MapQualityPlan(reader));
        }
        
        return plans;
    }
    
    public QualityPlan? GetPlanById(int id)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        
        using var cmd = new SQLiteCommand("SELECT * FROM QualityPlans WHERE Id = @Id", connection);
        cmd.Parameters.AddWithValue("@Id", id);
        
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var plan = MapQualityPlan(reader);
            plan.MeasurementPoints = GetMeasurementPoints(connection, id);
            return plan;
        }
        
        return null;
    }
    
    public int SavePlan(QualityPlan plan)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        
        using var transaction = connection.BeginTransaction();
        
        try
        {
            int planId;
            
            if (plan.Id == 0)
            {
                using var cmd = new SQLiteCommand(@"
                    INSERT INTO QualityPlans (ProductName, ProductCode, Customer, DrawingNo, BalloonCount, CreatedAt, UpdatedAt, IsActive)
                    VALUES (@ProductName, @ProductCode, @Customer, @DrawingNo, @BalloonCount, @CreatedAt, @UpdatedAt, @IsActive);
                    SELECT last_insert_rowid();", connection);
                
                AddPlanParameters(cmd, plan);
                planId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                using var cmd = new SQLiteCommand(@"
                    UPDATE QualityPlans 
                    SET ProductName = @ProductName, ProductCode = @ProductCode, Customer = @Customer,
                        DrawingNo = @DrawingNo, BalloonCount = @BalloonCount, UpdatedAt = @UpdatedAt
                    WHERE Id = @Id", connection);
                
                AddPlanParameters(cmd, plan);
                cmd.Parameters.AddWithValue("@Id", plan.Id);
                cmd.ExecuteNonQuery();
                planId = plan.Id;
                
                // Eski measurement point'leri sil
                using var deleteCmd = new SQLiteCommand("DELETE FROM MeasurementPoints WHERE PlanId = @PlanId", connection);
                deleteCmd.Parameters.AddWithValue("@PlanId", planId);
                deleteCmd.ExecuteNonQuery();
            }
            
            // Yeni measurement point'leri ekle
            foreach (var point in plan.MeasurementPoints)
            {
                using var cmd = new SQLiteCommand(@"
                    INSERT INTO MeasurementPoints (PlanId, BalloonNo, Dimension, Description, NominalValue, 
                        LowerTolerance, UpperTolerance, MeasurementTool, Frequency)
                    VALUES (@PlanId, @BalloonNo, @Dimension, @Description, @NominalValue, 
                        @LowerTolerance, @UpperTolerance, @MeasurementTool, @Frequency)", connection);
                
                cmd.Parameters.AddWithValue("@PlanId", planId);
                cmd.Parameters.AddWithValue("@BalloonNo", point.BalloonNo);
                cmd.Parameters.AddWithValue("@Dimension", point.Dimension);
                cmd.Parameters.AddWithValue("@Description", point.Description);
                cmd.Parameters.AddWithValue("@NominalValue", point.NominalValue);
                cmd.Parameters.AddWithValue("@LowerTolerance", point.LowerTolerance);
                cmd.Parameters.AddWithValue("@UpperTolerance", point.UpperTolerance);
                cmd.Parameters.AddWithValue("@MeasurementTool", point.MeasurementTool);
                cmd.Parameters.AddWithValue("@Frequency", point.Frequency);
                cmd.ExecuteNonQuery();
            }
            
            transaction.Commit();
            return planId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
    
    public void DeletePlan(int id)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        
        using var cmd = new SQLiteCommand("UPDATE QualityPlans SET IsActive = 0 WHERE Id = @Id", connection);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }
    
    // Measurement CRUD Operations
    public List<Measurement> GetAllMeasurements()
    {
        var measurements = new List<Measurement>();
        
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        
        using var cmd = new SQLiteCommand(@"
            SELECT m.*, p.ProductName, p.ProductCode 
            FROM Measurements m
            JOIN QualityPlans p ON m.PlanId = p.Id
            ORDER BY m.CreatedAt DESC", connection);
        
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            measurements.Add(MapMeasurement(reader));
        }
        
        return measurements;
    }
    
    public Measurement? GetMeasurementById(int id)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        
        using var cmd = new SQLiteCommand(@"
            SELECT m.*, p.ProductName, p.ProductCode 
            FROM Measurements m
            JOIN QualityPlans p ON m.PlanId = p.Id
            WHERE m.Id = @Id", connection);
        cmd.Parameters.AddWithValue("@Id", id);
        
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var measurement = MapMeasurement(reader);
            measurement.Details = GetMeasurementDetails(connection, id);
            measurement.ProductResults = GetProductResults(connection, id);
            return measurement;
        }
        
        return null;
    }
    
    public int SaveMeasurement(Measurement measurement)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        
        using var transaction = connection.BeginTransaction();
        
        try
        {
            int measurementId;
            
            if (measurement.Id == 0)
            {
                using var cmd = new SQLiteCommand(@"
                    INSERT INTO Measurements (PlanId, ControlDate, InvoiceNo, DeviceCodes, ControlledBy, 
                        ApprovedBy, BatchNo, QualityType, Notes, OverallResult)
                    VALUES (@PlanId, @ControlDate, @InvoiceNo, @DeviceCodes, @ControlledBy, 
                        @ApprovedBy, @BatchNo, @QualityType, @Notes, @OverallResult);
                    SELECT last_insert_rowid();", connection);
                
                AddMeasurementParameters(cmd, measurement);
                measurementId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                using var cmd = new SQLiteCommand(@"
                    UPDATE Measurements 
                    SET PlanId = @PlanId, ControlDate = @ControlDate, InvoiceNo = @InvoiceNo,
                        DeviceCodes = @DeviceCodes, ControlledBy = @ControlledBy, ApprovedBy = @ApprovedBy,
                        BatchNo = @BatchNo, QualityType = @QualityType, Notes = @Notes, OverallResult = @OverallResult
                    WHERE Id = @Id", connection);
                
                AddMeasurementParameters(cmd, measurement);
                cmd.Parameters.AddWithValue("@Id", measurement.Id);
                cmd.ExecuteNonQuery();
                measurementId = measurement.Id;
                
                // Eski detayları sil
                using var deleteDetails = new SQLiteCommand("DELETE FROM MeasurementDetails WHERE MeasurementId = @Id", connection);
                deleteDetails.Parameters.AddWithValue("@Id", measurementId);
                deleteDetails.ExecuteNonQuery();
                
                using var deleteProducts = new SQLiteCommand("DELETE FROM ProductResults WHERE MeasurementId = @Id", connection);
                deleteProducts.Parameters.AddWithValue("@Id", measurementId);
                deleteProducts.ExecuteNonQuery();
            }
            
            // Detayları ekle
            foreach (var detail in measurement.Details)
            {
                using var cmd = new SQLiteCommand(@"
                    INSERT INTO MeasurementDetails (MeasurementId, MeasurementPointId, MeasuredValue, Result, Notes)
                    VALUES (@MeasurementId, @MeasurementPointId, @MeasuredValue, @Result, @Notes)", connection);
                
                cmd.Parameters.AddWithValue("@MeasurementId", measurementId);
                cmd.Parameters.AddWithValue("@MeasurementPointId", detail.MeasurementPointId);
                cmd.Parameters.AddWithValue("@MeasuredValue", detail.MeasuredValue);
                cmd.Parameters.AddWithValue("@Result", detail.Result);
                cmd.Parameters.AddWithValue("@Notes", detail.Notes);
                cmd.ExecuteNonQuery();
            }
            
            // Ürün sonuçlarını ekle
            foreach (var product in measurement.ProductResults)
            {
                using var cmd = new SQLiteCommand(@"
                    INSERT INTO ProductResults (MeasurementId, ProductIndex, Result)
                    VALUES (@MeasurementId, @ProductIndex, @Result)", connection);
                
                cmd.Parameters.AddWithValue("@MeasurementId", measurementId);
                cmd.Parameters.AddWithValue("@ProductIndex", product.ProductIndex);
                cmd.Parameters.AddWithValue("@Result", product.Result);
                cmd.ExecuteNonQuery();
            }
            
            transaction.Commit();
            return measurementId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
    
    public void DeleteMeasurement(int id)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        
        using var cmd = new SQLiteCommand("DELETE FROM Measurements WHERE Id = @Id", connection);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }
    
    // Settings Operations
    public AppSettings GetSettings()
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        
        using var cmd = new SQLiteCommand("SELECT * FROM AppSettings WHERE Id = 1", connection);
        using var reader = cmd.ExecuteReader();
        
        if (reader.Read())
        {
            return new AppSettings
            {
                Id = reader.GetInt32(0),
                CompanyName = reader.GetString(1),
                LogoPath = reader.IsDBNull(2) ? null : reader.GetString(2),
                DefaultControlledBy = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                DefaultApprovedBy = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                DefaultMeasurementCount = reader.GetInt32(5),
                Theme = reader.GetString(6)
            };
        }
        
        return new AppSettings();
    }
    
    public void SaveSettings(AppSettings settings)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        
        using var cmd = new SQLiteCommand(@"
            UPDATE AppSettings 
            SET CompanyName = @CompanyName, LogoPath = @LogoPath, 
                DefaultControlledBy = @DefaultControlledBy, DefaultApprovedBy = @DefaultApprovedBy,
                DefaultMeasurementCount = @DefaultMeasurementCount, Theme = @Theme
            WHERE Id = 1", connection);
        
        cmd.Parameters.AddWithValue("@CompanyName", settings.CompanyName);
        cmd.Parameters.AddWithValue("@LogoPath", (object?)settings.LogoPath ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DefaultControlledBy", settings.DefaultControlledBy);
        cmd.Parameters.AddWithValue("@DefaultApprovedBy", settings.DefaultApprovedBy);
        cmd.Parameters.AddWithValue("@DefaultMeasurementCount", settings.DefaultMeasurementCount);
        cmd.Parameters.AddWithValue("@Theme", settings.Theme);
        cmd.ExecuteNonQuery();
    }
    
    // Statistics
    public (int TotalPlans, int TotalMeasurements, int OkProducts, int NokProducts, int ConditionalProducts) GetStatistics()
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        
        using var cmd1 = new SQLiteCommand("SELECT COUNT(*) FROM QualityPlans WHERE IsActive = 1", connection);
        var totalPlans = Convert.ToInt32(cmd1.ExecuteScalar());
        
        using var cmd2 = new SQLiteCommand("SELECT COUNT(*) FROM Measurements", connection);
        var totalMeasurements = Convert.ToInt32(cmd2.ExecuteScalar());
        
        using var cmd3 = new SQLiteCommand("SELECT Result, COUNT(*) FROM ProductResults GROUP BY Result", connection);
        using var reader = cmd3.ExecuteReader();
        
        int okProducts = 0, nokProducts = 0, conditionalProducts = 0;
        while (reader.Read())
        {
            var result = reader.GetString(0);
            var count = reader.GetInt32(1);
            
            switch (result)
            {
                case "OK": okProducts = count; break;
                case "NOK": nokProducts = count; break;
                case "CONDITIONAL": conditionalProducts = count; break;
            }
        }
        
        return (totalPlans, totalMeasurements, okProducts, nokProducts, conditionalProducts);
    }
    
    // Helper Methods
    private List<MeasurementPoint> GetMeasurementPoints(SQLiteConnection connection, int planId)
    {
        var points = new List<MeasurementPoint>();
        
        using var cmd = new SQLiteCommand("SELECT * FROM MeasurementPoints WHERE PlanId = @PlanId ORDER BY BalloonNo", connection);
        cmd.Parameters.AddWithValue("@PlanId", planId);
        
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            points.Add(new MeasurementPoint
            {
                Id = reader.GetInt32(0),
                PlanId = reader.GetInt32(1),
                BalloonNo = reader.GetInt32(2),
                Dimension = reader.GetString(3),
                Description = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                NominalValue = reader.GetString(5),
                LowerTolerance = reader.GetString(6),
                UpperTolerance = reader.GetString(7),
                MeasurementTool = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                Frequency = reader.GetString(9)
            });
        }
        
        return points;
    }
    
    private List<MeasurementDetail> GetMeasurementDetails(SQLiteConnection connection, int measurementId)
    {
        var details = new List<MeasurementDetail>();
        
        using var cmd = new SQLiteCommand("SELECT * FROM MeasurementDetails WHERE MeasurementId = @MeasurementId", connection);
        cmd.Parameters.AddWithValue("@MeasurementId", measurementId);
        
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            details.Add(new MeasurementDetail
            {
                Id = reader.GetInt32(0),
                MeasurementId = reader.GetInt32(1),
                MeasurementPointId = reader.GetInt32(2),
                MeasuredValue = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                Result = reader.GetString(4),
                Notes = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
            });
        }
        
        return details;
    }
    
    private List<ProductResult> GetProductResults(SQLiteConnection connection, int measurementId)
    {
        var results = new List<ProductResult>();
        
        using var cmd = new SQLiteCommand("SELECT * FROM ProductResults WHERE MeasurementId = @MeasurementId ORDER BY ProductIndex", connection);
        cmd.Parameters.AddWithValue("@MeasurementId", measurementId);
        
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            results.Add(new ProductResult
            {
                Id = reader.GetInt32(0),
                MeasurementId = reader.GetInt32(1),
                ProductIndex = reader.GetInt32(2),
                Result = reader.GetString(3)
            });
        }
        
        return results;
    }
    
    private QualityPlan MapQualityPlan(SQLiteDataReader reader)
    {
        return new QualityPlan
        {
            Id = reader.GetInt32(0),
            ProductName = reader.GetString(1),
            ProductCode = reader.GetString(2),
            Customer = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
            DrawingNo = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
            BalloonCount = reader.GetInt32(5),
            CreatedAt = DateTime.Parse(reader.GetString(6)),
            UpdatedAt = DateTime.Parse(reader.GetString(7)),
            IsActive = reader.GetInt32(8) == 1
        };
    }
    
    private Measurement MapMeasurement(SQLiteDataReader reader)
    {
        return new Measurement
        {
            Id = reader.GetInt32(0),
            PlanId = reader.GetInt32(1),
            ControlDate = reader.GetString(2),
            InvoiceNo = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
            DeviceCodes = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
            ControlledBy = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
            ApprovedBy = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
            BatchNo = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
            QualityType = reader.GetString(8),
            Notes = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
            CreatedAt = DateTime.Parse(reader.GetString(10)),
            OverallResult = reader.GetString(11),
            Plan = new QualityPlan 
            { 
                ProductName = reader.GetString(12),
                ProductCode = reader.GetString(13)
            }
        };
    }
    
    private void AddPlanParameters(SQLiteCommand cmd, QualityPlan plan)
    {
        cmd.Parameters.AddWithValue("@ProductName", plan.ProductName);
        cmd.Parameters.AddWithValue("@ProductCode", plan.ProductCode);
        cmd.Parameters.AddWithValue("@Customer", plan.Customer);
        cmd.Parameters.AddWithValue("@DrawingNo", plan.DrawingNo);
        cmd.Parameters.AddWithValue("@BalloonCount", plan.BalloonCount);
        cmd.Parameters.AddWithValue("@CreatedAt", plan.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
        cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        cmd.Parameters.AddWithValue("@IsActive", plan.IsActive ? 1 : 0);
    }
    
    private void AddMeasurementParameters(SQLiteCommand cmd, Measurement measurement)
    {
        cmd.Parameters.AddWithValue("@PlanId", measurement.PlanId);
        cmd.Parameters.AddWithValue("@ControlDate", measurement.ControlDate);
        cmd.Parameters.AddWithValue("@InvoiceNo", measurement.InvoiceNo);
        cmd.Parameters.AddWithValue("@DeviceCodes", measurement.DeviceCodes);
        cmd.Parameters.AddWithValue("@ControlledBy", measurement.ControlledBy);
        cmd.Parameters.AddWithValue("@ApprovedBy", measurement.ApprovedBy);
        cmd.Parameters.AddWithValue("@BatchNo", measurement.BatchNo);
        cmd.Parameters.AddWithValue("@QualityType", measurement.QualityType);
        cmd.Parameters.AddWithValue("@Notes", measurement.Notes);
        cmd.Parameters.AddWithValue("@OverallResult", measurement.OverallResult);
    }
}
