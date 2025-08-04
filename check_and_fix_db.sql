-- Script kiểm tra và sửa database ITMMS
USE [ITMMS_DB]

-- 1. Kiểm tra cấu trúc bảng TreatmentPlans
SELECT 'TreatmentPlans Columns:' as Info;
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'TreatmentPlans' 
ORDER BY ORDINAL_POSITION;

-- 2. Thêm các column còn thiếu nếu chưa có
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TreatmentPlans' AND COLUMN_NAME = 'Notes')
BEGIN
    ALTER TABLE TreatmentPlans ADD Notes NVARCHAR(500) NULL;
    PRINT 'Added Notes column to TreatmentPlans';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TreatmentPlans' AND COLUMN_NAME = 'ProgressNotes')
BEGIN
    ALTER TABLE TreatmentPlans ADD ProgressNotes NVARCHAR(1000) NULL;
    PRINT 'Added ProgressNotes column to TreatmentPlans';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TreatmentPlans' AND COLUMN_NAME = 'PaymentStatus')
BEGIN
    ALTER TABLE TreatmentPlans ADD PaymentStatus NVARCHAR(20) NULL DEFAULT 'Pending';
    PRINT 'Added PaymentStatus column to TreatmentPlans';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TreatmentPlans' AND COLUMN_NAME = 'TreatmentServiceId')
BEGIN
    ALTER TABLE TreatmentPlans ADD TreatmentServiceId INT NULL;
    PRINT 'Added TreatmentServiceId column to TreatmentPlans';
END

-- 3. Thêm column cho Appointments
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'Notes')
BEGIN
    ALTER TABLE Appointments ADD Notes NVARCHAR(500) NULL;
    PRINT 'Added Notes column to Appointments';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'TreatmentPlanId')
BEGIN
    ALTER TABLE Appointments ADD TreatmentPlanId INT NULL;
    PRINT 'Added TreatmentPlanId column to Appointments';
END

-- 4. Thêm column cho MedicalRecords
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MedicalRecords' AND COLUMN_NAME = 'Notes')
BEGIN
    ALTER TABLE MedicalRecords ADD Notes NVARCHAR(1000) NULL;
    PRINT 'Added Notes column to MedicalRecords';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MedicalRecords' AND COLUMN_NAME = 'AppointmentId')
BEGIN
    ALTER TABLE MedicalRecords ADD AppointmentId INT NULL;
    PRINT 'Added AppointmentId column to MedicalRecords';
END

-- 5. Thêm column cho TestResults
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TestResults' AND COLUMN_NAME = 'Unit')
BEGIN
    ALTER TABLE TestResults ADD Unit NVARCHAR(20) NULL;
    PRINT 'Added Unit column to TestResults';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TestResults' AND COLUMN_NAME = 'DoctorName')
BEGIN
    ALTER TABLE TestResults ADD DoctorName NVARCHAR(100) NULL;
    PRINT 'Added DoctorName column to TestResults';
END

-- 6. Kiểm tra data hiện tại
SELECT 'Current Users:' as Info;
SELECT Id, Email, FullName, Role FROM Users;

SELECT 'Current TreatmentPlans:' as Info;
SELECT Id, CustomerId, TreatmentType, Status, Notes FROM TreatmentPlans;

-- 7. Thêm user thanht@gmail.com nếu chưa có
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'thanht@gmail.com')
BEGIN
    INSERT INTO Users (Username, Password, Email, FullName, Phone, Address, Role, CreatedAt, IsActive)
    VALUES ('thanht', 'patient123', 'thanht@gmail.com', 'Nguyễn Văn Thanh', '0987654326', 'Hà Nội', 'Customer', GETDATE(), 1);
    PRINT 'Added user thanht@gmail.com';
END

-- 8. Thêm customer cho user thanht nếu chưa có
IF NOT EXISTS (SELECT * FROM Customers WHERE UserId = (SELECT Id FROM Users WHERE Email = 'thanht@gmail.com'))
BEGIN
    INSERT INTO Customers (UserId, DateOfBirth, Gender, EmergencyContact)
    VALUES ((SELECT Id FROM Users WHERE Email = 'thanht@gmail.com'), '1988-12-05', 'Male', '0123456792');
    PRINT 'Added customer for thanht@gmail.com';
END

-- 9. Thêm treatment plan cho patient Thanh nếu chưa có
IF NOT EXISTS (SELECT * FROM TreatmentPlans WHERE CustomerId = (SELECT c.Id FROM Customers c JOIN Users u ON c.UserId = u.Id WHERE u.Email = 'thanht@gmail.com'))
BEGIN
    INSERT INTO TreatmentPlans (CustomerId, DoctorId, TreatmentType, Description, StartDate, EndDate, Status, CurrentPhase, PhaseDescription, TotalCost, PaidAmount, PaymentStatus, Notes, ProgressNotes)
    VALUES (
        (SELECT c.Id FROM Customers c JOIN Users u ON c.UserId = u.Id WHERE u.Email = 'thanht@gmail.com'),
        1,
        'IVF',
        'IVF treatment for patient Thanh',
        '2024-02-01',
        '2024-03-01',
        'Active',
        1,
        'Initial consultation',
        50000000,
        0,
        'Pending',
        'New treatment plan',
        'Starting treatment'
    );
    PRINT 'Added treatment plan for patient Thanh';
END

PRINT 'Database check and fix completed!'; 