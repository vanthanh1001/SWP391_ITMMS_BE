# Script setup database với data mẫu cho ITMMS
Write-Host "ITMMS Database Setup with Sample Data" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green

# Check SQL Server connection
Write-Host "1. Checking SQL Server connection..." -ForegroundColor Yellow

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection
    $connection.ConnectionString = "Server=localhost;Database=ITMMS_DB;Trusted_Connection=true;TrustServerCertificate=true;"
    $connection.Open()
    Write-Host "SQL Server connection successful" -ForegroundColor Green
    $connection.Close()
}
catch {
    Write-Host "SQL Server connection failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Please ensure SQL Server is running and database ITMMS_DB exists" -ForegroundColor Yellow
    exit 1
}

# Fix database schema first
Write-Host "2. Fixing database schema..." -ForegroundColor Yellow

$fixSchemaScript = @"
USE [ITMMS_DB]

-- Add Notes column to TreatmentPlans
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TreatmentPlans' AND COLUMN_NAME = 'Notes')
BEGIN
    ALTER TABLE TreatmentPlans ADD Notes NVARCHAR(500) NULL;
    PRINT 'Added Notes column to TreatmentPlans table';
END

-- Add ProgressNotes column to TreatmentPlans
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TreatmentPlans' AND COLUMN_NAME = 'ProgressNotes')
BEGIN
    ALTER TABLE TreatmentPlans ADD ProgressNotes NVARCHAR(1000) NULL;
    PRINT 'Added ProgressNotes column to TreatmentPlans table';
END

-- Add Notes column to Appointments
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'Notes')
BEGIN
    ALTER TABLE Appointments ADD Notes NVARCHAR(500) NULL;
    PRINT 'Added Notes column to Appointments table';
END

-- Add Notes column to MedicalRecords
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MedicalRecords' AND COLUMN_NAME = 'Notes')
BEGIN
    ALTER TABLE MedicalRecords ADD Notes NVARCHAR(1000) NULL;
    PRINT 'Added Notes column to MedicalRecords table';
END

-- Add Unit column to TestResults
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TestResults' AND COLUMN_NAME = 'Unit')
BEGIN
    ALTER TABLE TestResults ADD Unit NVARCHAR(20) NULL;
    PRINT 'Added Unit column to TestResults table';
END

-- Add DoctorName column to TestResults
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TestResults' AND COLUMN_NAME = 'DoctorName')
BEGIN
    ALTER TABLE TestResults ADD DoctorName NVARCHAR(100) NULL;
    PRINT 'Added DoctorName column to TestResults table';
END

-- Add PaymentStatus column to TreatmentPlans
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TreatmentPlans' AND COLUMN_NAME = 'PaymentStatus')
BEGIN
    ALTER TABLE TreatmentPlans ADD PaymentStatus NVARCHAR(20) NULL DEFAULT 'Pending';
    PRINT 'Added PaymentStatus column to TreatmentPlans table';
END

-- Add TreatmentServiceId column to TreatmentPlans
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TreatmentPlans' AND COLUMN_NAME = 'TreatmentServiceId')
BEGIN
    ALTER TABLE TreatmentPlans ADD TreatmentServiceId INT NULL;
    PRINT 'Added TreatmentServiceId column to TreatmentPlans table';
END

-- Add AppointmentId column to MedicalRecords
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MedicalRecords' AND COLUMN_NAME = 'AppointmentId')
BEGIN
    ALTER TABLE MedicalRecords ADD AppointmentId INT NULL;
    PRINT 'Added AppointmentId column to MedicalRecords table';
END

-- Add TreatmentPlanId column to Appointments
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'TreatmentPlanId')
BEGIN
    ALTER TABLE Appointments ADD TreatmentPlanId INT NULL;
    PRINT 'Added TreatmentPlanId column to Appointments table';
END

PRINT 'Database schema fix completed successfully!';
"@

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection
    $connection.ConnectionString = "Server=localhost;Database=ITMMS_DB;Trusted_Connection=true;TrustServerCertificate=true;"
    $connection.Open()
    
    $command = New-Object System.Data.SqlClient.SqlCommand($fixSchemaScript, $connection)
    $command.ExecuteNonQuery()
    
    Write-Host "Database schema updated successfully" -ForegroundColor Green
    $connection.Close()
}
catch {
    Write-Host "Database schema update failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Add sample data
Write-Host "3. Adding sample data..." -ForegroundColor Yellow

$sampleDataScript = @"
USE [ITMMS_DB]

-- Clear existing data (optional)
-- DELETE FROM Feedbacks;
-- DELETE FROM Prescriptions;
-- DELETE FROM TestResults;
-- DELETE FROM MedicalRecords;
-- DELETE FROM Appointments;
-- DELETE FROM TreatmentPlans;
-- DELETE FROM TreatmentServices;
-- DELETE FROM Customers;
-- DELETE FROM Doctors;
-- DELETE FROM Users;

-- 1. Add Users
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'admin@itmms.com')
BEGIN
    INSERT INTO Users (Username, Password, Email, FullName, Phone, Address, Role, CreatedAt, IsActive)
    VALUES 
    ('admin', 'admin123', 'admin@itmms.com', 'Admin User', '0123456789', 'Hà Nội', 'Admin', GETDATE(), 1),
    ('doctor1', 'doctor123', 'doctor1@itmms.com', 'Bác sĩ Nguyễn Văn A', '0987654321', 'Hà Nội', 'Doctor', GETDATE(), 1),
    ('doctor2', 'doctor123', 'doctor2@itmms.com', 'Bác sĩ Trần Thị B', '0987654322', 'TP.HCM', 'Doctor', GETDATE(), 1),
    ('patient1', 'patient123', 'patient1@gmail.com', 'Nguyễn Thị C', '0987654323', 'Hà Nội', 'Customer', GETDATE(), 1),
    ('patient2', 'patient123', 'patient2@gmail.com', 'Lê Văn D', '0987654324', 'TP.HCM', 'Customer', GETDATE(), 1),
    ('patient3', 'patient123', 'patient3@gmail.com', 'Phạm Thị E', '0987654325', 'Đà Nẵng', 'Customer', GETDATE(), 1),
    ('patient4', 'patient123', 'thanht@gmail.com', 'Nguyễn Văn Thanh', '0987654326', 'Hà Nội', 'Customer', GETDATE(), 1);
    PRINT 'Users added successfully';
END

-- 2. Add Doctors
IF NOT EXISTS (SELECT * FROM Doctors WHERE LicenseNumber = 'BS001')
BEGIN
    INSERT INTO Doctors (UserId, Specialization, LicenseNumber, Education, Description, ExperienceYears, ConsultationFee, IsAvailable)
    VALUES 
    (2, 'Reproductive Medicine', 'BS001', 'Đại học Y Hà Nội', 'Chuyên gia về điều trị hiếm muộn', 10, 500000, 1),
    (3, 'Gynecology', 'BS002', 'Đại học Y TP.HCM', 'Chuyên gia phụ khoa', 8, 400000, 1);
    PRINT 'Doctors added successfully';
END

-- 3. Add Customers
IF NOT EXISTS (SELECT * FROM Customers WHERE UserId = 4)
BEGIN
    INSERT INTO Customers (UserId, DateOfBirth, Gender, EmergencyContact)
    VALUES 
    (4, '1990-01-15', 'Female', '0123456789'),
    (5, '1985-03-20', 'Male', '0123456790'),
    (6, '1992-07-10', 'Female', '0123456791'),
    (7, '1988-12-05', 'Male', '0123456792');
    PRINT 'Customers added successfully';
END

-- 4. Add TreatmentServices
IF NOT EXISTS (SELECT * FROM TreatmentServices WHERE ServiceCode = 'IVF001')
BEGIN
    INSERT INTO TreatmentServices (ServiceName, ServiceCode, Description, BasePrice, Procedures, Requirements, DurationDays, SuccessRate, IsActive)
    VALUES 
    ('IVF Treatment', 'IVF001', 'In vitro fertilization treatment', 50000000, 'Egg retrieval, fertilization, embryo transfer', 'Age under 40, healthy uterus', 30, 65.5, 1),
    ('IUI Treatment', 'IUI001', 'Intrauterine insemination', 15000000, 'Sperm preparation, insemination', 'Healthy fallopian tubes', 15, 45.0, 1),
    ('Hormone Therapy', 'HT001', 'Hormone treatment for ovulation', 8000000, 'Medication, monitoring', 'Regular menstrual cycle', 21, 70.0, 1);
    PRINT 'TreatmentServices added successfully';
END

-- 5. Add TreatmentPlans
IF NOT EXISTS (SELECT * FROM TreatmentPlans WHERE CustomerId = 1)
BEGIN
    INSERT INTO TreatmentPlans (CustomerId, DoctorId, TreatmentServiceId, TreatmentType, Description, StartDate, EndDate, Status, CurrentPhase, PhaseDescription, NextPhaseDate, NextVisitDate, TotalCost, PaidAmount, PaymentStatus, Notes, ProgressNotes)
    VALUES 
    (1, 1, 1, 'IVF', 'In vitro fertilization treatment for patient', '2024-01-15', '2024-02-15', 'Active', 2, 'Egg retrieval phase', '2024-01-25', '2024-01-25', 50000000, 25000000, 'Partial', 'Patient responding well to treatment', 'Phase 1 completed successfully'),
    (2, 1, 2, 'IUI', 'Intrauterine insemination treatment', '2024-01-20', '2024-02-05', 'Active', 1, 'Initial consultation', '2024-01-30', '2024-01-30', 15000000, 7500000, 'Partial', 'Treatment started', 'Initial phase in progress'),
    (3, 2, 3, 'Hormone', 'Hormone therapy for ovulation', '2024-01-10', '2024-01-31', 'Completed', 3, 'Treatment completed', NULL, NULL, 8000000, 8000000, 'Paid', 'Treatment completed successfully', 'All phases completed'),
    (4, 1, 1, 'IVF', 'IVF treatment for patient Thanh', '2024-02-01', '2024-03-01', 'Active', 1, 'Initial consultation', '2024-02-10', '2024-02-10', 50000000, 0, 'Pending', 'New treatment plan', 'Starting treatment');
    PRINT 'TreatmentPlans added successfully';
END

-- 6. Add Appointments
IF NOT EXISTS (SELECT * FROM Appointments WHERE CustomerId = 1)
BEGIN
    INSERT INTO Appointments (CustomerId, DoctorId, TreatmentPlanId, AppointmentDate, TimeSlot, Type, Status, Notes)
    VALUES 
    (1, 1, 1, '2024-01-20', '09:00-10:00', 'Consultation', 'Completed', 'Initial consultation completed'),
    (1, 1, 1, '2024-01-25', '08:00-09:00', 'Procedure', 'Scheduled', 'Egg retrieval procedure'),
    (2, 1, 2, '2024-01-30', '10:00-11:00', 'Consultation', 'Scheduled', 'IUI consultation'),
    (3, 2, 3, '2024-01-15', '14:00-15:00', 'Follow-up', 'Completed', 'Hormone therapy follow-up'),
    (4, 1, 4, '2024-02-10', '09:00-10:00', 'Consultation', 'Scheduled', 'Initial consultation for patient Thanh');
    PRINT 'Appointments added successfully';
END

-- 7. Add MedicalRecords
IF NOT EXISTS (SELECT * FROM MedicalRecords WHERE CustomerId = 1)
BEGIN
    INSERT INTO MedicalRecords (CustomerId, DoctorId, AppointmentId, Symptoms, Diagnosis, Treatment, Prescription, Notes, RecordDate)
    VALUES 
    (1, 1, 1, 'Irregular menstrual cycle, difficulty conceiving', 'Ovulatory dysfunction', 'Hormone therapy and IVF treatment', 'Clomiphene citrate 50mg daily', 'Patient shows good response to treatment', '2024-01-20'),
    (2, 1, 3, 'Infertility for 2 years', 'Unexplained infertility', 'IUI treatment recommended', 'Folic acid supplements', 'Patient ready for IUI treatment', '2024-01-30'),
    (3, 2, 4, 'Irregular periods', 'Hormonal imbalance', 'Hormone therapy completed', 'Progesterone supplements', 'Treatment completed successfully', '2024-01-15'),
    (4, 1, 5, 'Difficulty conceiving for 3 years', 'Male factor infertility', 'IVF treatment recommended', 'Multivitamins and antioxidants', 'Initial assessment completed', '2024-02-10');
    PRINT 'MedicalRecords added successfully';
END

-- 8. Add TestResults
IF NOT EXISTS (SELECT * FROM TestResults WHERE CustomerId = 1)
BEGIN
    INSERT INTO TestResults (CustomerId, DoctorId, TestName, TestType, Results, NormalRange, Status, TestDate, Unit, DoctorName)
    VALUES 
    (1, 1, 'FSH Test', 'Hormone Test', '8.5', '3.5-12.5', 'Normal', '2024-01-18', 'mIU/mL', 'Bác sĩ Nguyễn Văn A'),
    (1, 1, 'LH Test', 'Hormone Test', '6.2', '2.4-12.6', 'Normal', '2024-01-18', 'mIU/mL', 'Bác sĩ Nguyễn Văn A'),
    (1, 1, 'AMH Test', 'Hormone Test', '2.1', '1.0-4.0', 'Normal', '2024-01-18', 'ng/mL', 'Bác sĩ Nguyễn Văn A'),
    (2, 1, 'Semen Analysis', 'Sperm Test', '15 million/mL', '15-200 million/mL', 'Low', '2024-01-25', 'million/mL', 'Bác sĩ Nguyễn Văn A'),
    (3, 2, 'Thyroid Test', 'Hormone Test', '2.5', '0.4-4.0', 'Normal', '2024-01-12', 'mIU/L', 'Bác sĩ Trần Thị B'),
    (4, 1, 'FSH Test', 'Hormone Test', '7.8', '3.5-12.5', 'Normal', '2024-02-05', 'mIU/mL', 'Bác sĩ Nguyễn Văn A');
    PRINT 'TestResults added successfully';
END

-- 9. Add Prescriptions
IF NOT EXISTS (SELECT * FROM Prescriptions WHERE MedicalRecordId = 1)
BEGIN
    INSERT INTO Prescriptions (MedicalRecordId, MedicineName, Dosage, Frequency, Duration, Instructions)
    VALUES 
    (1, 'Clomiphene citrate', '50mg', 'Once daily', '5 days', 'Take in the morning'),
    (1, 'Folic acid', '400mcg', 'Once daily', '30 days', 'Take with food'),
    (2, 'Folic acid', '400mcg', 'Once daily', '30 days', 'Take with food'),
    (3, 'Progesterone', '200mg', 'Twice daily', '14 days', 'Take after meals'),
    (4, 'Multivitamins', '1 tablet', 'Once daily', '30 days', 'Take in the morning');
    PRINT 'Prescriptions added successfully';
END

-- 10. Add Feedbacks
IF NOT EXISTS (SELECT * FROM Feedbacks WHERE CustomerId = 1)
BEGIN
    INSERT INTO Feedbacks (CustomerId, DoctorId, AppointmentId, Rating, Comment, CreatedAt)
    VALUES 
    (1, 1, 1, 5, 'Bác sĩ rất tận tâm và chuyên nghiệp', '2024-01-20'),
    (2, 1, 3, 4, 'Tư vấn rất chi tiết và dễ hiểu', '2024-01-30'),
    (3, 2, 4, 5, 'Điều trị hiệu quả, rất hài lòng', '2024-01-15');
    PRINT 'Feedbacks added successfully';
END

PRINT 'Sample data created successfully!';
"@

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection
    $connection.ConnectionString = "Server=localhost;Database=ITMMS_DB;Trusted_Connection=true;TrustServerCertificate=true;"
    $connection.Open()
    
    $command = New-Object System.Data.SqlClient.SqlCommand($sampleDataScript, $connection)
    $command.ExecuteNonQuery()
    
    Write-Host "Sample data added successfully" -ForegroundColor Green
    $connection.Close()
}
catch {
    Write-Host "Sample data addition failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Stop API if running
Write-Host "4. Stopping API if running..." -ForegroundColor Yellow

$apiProcess = Get-Process -Name "SWP391_ITMMS_Api" -ErrorAction SilentlyContinue
if ($apiProcess) {
    Stop-Process -Name "SWP391_ITMMS_Api" -Force
    Write-Host "API stopped" -ForegroundColor Green
} else {
    Write-Host "API is not running" -ForegroundColor Blue
}

# Build project
Write-Host "5. Building project..." -ForegroundColor Yellow

try {
    dotnet build
    Write-Host "Build successful" -ForegroundColor Green
}
catch {
    Write-Host "Build failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Start API
Write-Host "6. Starting API..." -ForegroundColor Yellow

try {
    Start-Process -FilePath "dotnet" -ArgumentList "run" -WorkingDirectory (Get-Location) -WindowStyle Minimized
    Write-Host "API started successfully" -ForegroundColor Green
    Write-Host "Swagger UI: http://localhost:5037/swagger/index.html" -ForegroundColor Cyan
    Write-Host "Health Check: http://localhost:5037/api/health" -ForegroundColor Cyan
}
catch {
    Write-Host "Failed to start API: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Database setup completed successfully!" -ForegroundColor Green
Write-Host "Sample data has been added to the database" -ForegroundColor Yellow
Write-Host ""
Write-Host "Test accounts:" -ForegroundColor Cyan
Write-Host "- Admin: admin@itmms.com / admin123" -ForegroundColor White
Write-Host "- Doctor: doctor1@itmms.com / doctor123" -ForegroundColor White
Write-Host "- Patient: thanht@gmail.com / patient123" -ForegroundColor White
Write-Host ""
Write-Host "Test TreatmentFlow endpoint: http://localhost:5037/api/TreatmentFlow/patient/4" -ForegroundColor Cyan 