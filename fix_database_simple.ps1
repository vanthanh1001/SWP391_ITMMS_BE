# Simple Database Fix Script for ITMMS
Write-Host "ITMMS Database Fix Script" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green

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

# Run SQL script to fix database
Write-Host "2. Running database fix script..." -ForegroundColor Yellow

$sqlScript = @"
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
    
    $command = New-Object System.Data.SqlClient.SqlCommand($sqlScript, $connection)
    $command.ExecuteNonQuery()
    
    Write-Host "Database schema updated successfully" -ForegroundColor Green
    $connection.Close()
}
catch {
    Write-Host "Database update failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Stop API if running
Write-Host "3. Stopping API if running..." -ForegroundColor Yellow

$apiProcess = Get-Process -Name "SWP391_ITMMS_Api" -ErrorAction SilentlyContinue
if ($apiProcess) {
    Stop-Process -Name "SWP391_ITMMS_Api" -Force
    Write-Host "API stopped" -ForegroundColor Green
} else {
    Write-Host "API is not running" -ForegroundColor Blue
}

# Build project
Write-Host "4. Building project..." -ForegroundColor Yellow

try {
    dotnet build
    Write-Host "Build successful" -ForegroundColor Green
}
catch {
    Write-Host "Build failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Start API
Write-Host "5. Starting API..." -ForegroundColor Yellow

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
Write-Host "Database fix completed successfully!" -ForegroundColor Green
Write-Host "You can now test the TreatmentFlow API endpoint" -ForegroundColor Yellow
Write-Host "Test URL: http://localhost:5037/api/TreatmentFlow/patient/4" -ForegroundColor Cyan 