-- Script sửa lỗi database schema cho ITMMS
-- Chạy script này để thêm các column còn thiếu

USE [ITMMS_DB]  -- Thay đổi tên database nếu cần

-- 1. Thêm column Notes vào bảng TreatmentPlans nếu chưa có
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'TreatmentPlans' AND COLUMN_NAME = 'Notes')
BEGIN
    ALTER TABLE TreatmentPlans ADD Notes NVARCHAR(500) NULL;
    PRINT 'Added Notes column to TreatmentPlans table';
END

-- 2. Thêm column ProgressNotes vào bảng TreatmentPlans nếu chưa có
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'TreatmentPlans' AND COLUMN_NAME = 'ProgressNotes')
BEGIN
    ALTER TABLE TreatmentPlans ADD ProgressNotes NVARCHAR(1000) NULL;
    PRINT 'Added ProgressNotes column to TreatmentPlans table';
END

-- 3. Thêm column Notes vào bảng Appointments nếu chưa có
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'Notes')
BEGIN
    ALTER TABLE Appointments ADD Notes NVARCHAR(500) NULL;
    PRINT 'Added Notes column to Appointments table';
END

-- 4. Thêm column Notes vào bảng MedicalRecords nếu chưa có
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'MedicalRecords' AND COLUMN_NAME = 'Notes')
BEGIN
    ALTER TABLE MedicalRecords ADD Notes NVARCHAR(1000) NULL;
    PRINT 'Added Notes column to MedicalRecords table';
END

-- 5. Thêm column Unit vào bảng TestResults nếu chưa có
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'TestResults' AND COLUMN_NAME = 'Unit')
BEGIN
    ALTER TABLE TestResults ADD Unit NVARCHAR(20) NULL;
    PRINT 'Added Unit column to TestResults table';
END

-- 6. Thêm column DoctorName vào bảng TestResults nếu chưa có
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'TestResults' AND COLUMN_NAME = 'DoctorName')
BEGIN
    ALTER TABLE TestResults ADD DoctorName NVARCHAR(100) NULL;
    PRINT 'Added DoctorName column to TestResults table';
END

-- 7. Kiểm tra và thêm các column khác nếu cần
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'TreatmentPlans' AND COLUMN_NAME = 'PaymentStatus')
BEGIN
    ALTER TABLE TreatmentPlans ADD PaymentStatus NVARCHAR(20) NULL DEFAULT 'Pending';
    PRINT 'Added PaymentStatus column to TreatmentPlans table';
END

-- 8. Thêm column TreatmentServiceId vào bảng TreatmentPlans nếu chưa có
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'TreatmentPlans' AND COLUMN_NAME = 'TreatmentServiceId')
BEGIN
    ALTER TABLE TreatmentPlans ADD TreatmentServiceId INT NULL;
    PRINT 'Added TreatmentServiceId column to TreatmentPlans table';
END

-- 9. Thêm foreign key constraint cho TreatmentServiceId nếu chưa có
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS 
               WHERE CONSTRAINT_NAME = 'FK_TreatmentPlans_TreatmentServices_TreatmentServiceId')
BEGIN
    ALTER TABLE TreatmentPlans 
    ADD CONSTRAINT FK_TreatmentPlans_TreatmentServices_TreatmentServiceId 
    FOREIGN KEY (TreatmentServiceId) REFERENCES TreatmentServices(Id);
    PRINT 'Added foreign key constraint for TreatmentServiceId';
END

-- 10. Thêm column AppointmentId vào bảng MedicalRecords nếu chưa có
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'MedicalRecords' AND COLUMN_NAME = 'AppointmentId')
BEGIN
    ALTER TABLE MedicalRecords ADD AppointmentId INT NULL;
    PRINT 'Added AppointmentId column to MedicalRecords table';
END

-- 11. Thêm foreign key constraint cho AppointmentId nếu chưa có
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS 
               WHERE CONSTRAINT_NAME = 'FK_MedicalRecords_Appointments_AppointmentId')
BEGIN
    ALTER TABLE MedicalRecords 
    ADD CONSTRAINT FK_MedicalRecords_Appointments_AppointmentId 
    FOREIGN KEY (AppointmentId) REFERENCES Appointments(Id);
    PRINT 'Added foreign key constraint for AppointmentId';
END

-- 12. Thêm column TreatmentPlanId vào bảng Appointments nếu chưa có
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'TreatmentPlanId')
BEGIN
    ALTER TABLE Appointments ADD TreatmentPlanId INT NULL;
    PRINT 'Added TreatmentPlanId column to Appointments table';
END

-- 13. Thêm foreign key constraint cho TreatmentPlanId nếu chưa có
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS 
               WHERE CONSTRAINT_NAME = 'FK_Appointments_TreatmentPlans_TreatmentPlanId')
BEGIN
    ALTER TABLE Appointments 
    ADD CONSTRAINT FK_Appointments_TreatmentPlans_TreatmentPlanId 
    FOREIGN KEY (TreatmentPlanId) REFERENCES TreatmentPlans(Id);
    PRINT 'Added foreign key constraint for TreatmentPlanId';
END

PRINT 'Database schema fix completed successfully!'; 