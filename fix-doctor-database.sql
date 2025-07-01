-- Script để fix data inconsistency trong database
-- Chạy trong SQL Server Management Studio

USE ITMMS_DB;
GO

-- 1. Kiểm tra Users không có tương ứng Doctor record
PRINT '=== CHECKING DOCTOR DATA INCONSISTENCY ==='
SELECT 
    u.Id as UserId,
    u.Username,
    u.FullName,
    u.Role,
    d.Id as DoctorId
FROM Users u
LEFT JOIN Doctors d ON u.Id = d.UserId
WHERE u.Role = 'Doctor'
ORDER BY u.Id;

-- 2. Tìm Users có Role='Doctor' nhưng không có Doctor record
PRINT CHAR(13) + CHAR(10) + '=== USERS WITHOUT DOCTOR RECORDS ==='
SELECT 
    u.Id as UserId,
    u.Username,
    u.FullName,
    u.Email
FROM Users u
LEFT JOIN Doctors d ON u.Id = d.UserId
WHERE u.Role = 'Doctor' AND d.Id IS NULL;

-- 3. Fix: Tạo Doctor records cho Users bị thiếu
PRINT CHAR(13) + CHAR(10) + '=== CREATING MISSING DOCTOR RECORDS ==='

DECLARE @UserId INT, @LicenseCounter INT = 1;
DECLARE @FullName NVARCHAR(100);

DECLARE doctor_cursor CURSOR FOR
SELECT u.Id, u.FullName
FROM Users u
LEFT JOIN Doctors d ON u.Id = d.UserId
WHERE u.Role = 'Doctor' AND d.Id IS NULL;

OPEN doctor_cursor;
FETCH NEXT FROM doctor_cursor INTO @UserId, @FullName;

WHILE @@FETCH_STATUS = 0
BEGIN
    DECLARE @LicenseNumber NVARCHAR(50);
    
    -- Generate unique license number
    SET @LicenseNumber = 'BS' + FORMAT(GETDATE(), 'yyyyMMdd') + FORMAT(@LicenseCounter, 'D3');
    
    -- Ensure uniqueness
    WHILE EXISTS(SELECT 1 FROM Doctors WHERE LicenseNumber = @LicenseNumber)
    BEGIN
        SET @LicenseCounter = @LicenseCounter + 1;
        SET @LicenseNumber = 'BS' + FORMAT(GETDATE(), 'yyyyMMdd') + FORMAT(@LicenseCounter, 'D3');
    END;
    
    -- Insert Doctor record
    INSERT INTO Doctors (UserId, Specialization, LicenseNumber, Education, ExperienceYears, Description, ConsultationFee, IsAvailable)
    VALUES (@UserId, N'Chưa cập nhật', @LicenseNumber, N'Chưa cập nhật', 0, N'Chưa cập nhật', 0.00, 0);
    
    PRINT 'Created Doctor record for User ID: ' + CAST(@UserId AS VARCHAR) + ' (' + @FullName + ') - License: ' + @LicenseNumber;
    
    SET @LicenseCounter = @LicenseCounter + 1;
    FETCH NEXT FROM doctor_cursor INTO @UserId, @FullName;
END;

CLOSE doctor_cursor;
DEALLOCATE doctor_cursor;

-- 4. Verification: Kiểm tra lại sau khi fix
PRINT CHAR(13) + CHAR(10) + '=== VERIFICATION AFTER FIX ==='
SELECT 
    COUNT(*) as TotalDoctorUsers
FROM Users 
WHERE Role = 'Doctor';

SELECT 
    COUNT(*) as TotalDoctorRecords  
FROM Doctors;

SELECT 
    COUNT(*) as MissingDoctorRecords
FROM Users u
LEFT JOIN Doctors d ON u.Id = d.UserId
WHERE u.Role = 'Doctor' AND d.Id IS NULL;

-- 5. Show all doctors after fix
PRINT CHAR(13) + CHAR(10) + '=== ALL DOCTORS AFTER FIX ==='
SELECT 
    u.Id as UserId,
    u.Username,
    u.FullName,
    d.Id as DoctorId,
    d.LicenseNumber,
    d.IsAvailable
FROM Users u
INNER JOIN Doctors d ON u.Id = d.UserId
WHERE u.Role = 'Doctor'
ORDER BY u.Id;

PRINT CHAR(13) + CHAR(10) + '=== FIX COMPLETED ===' 