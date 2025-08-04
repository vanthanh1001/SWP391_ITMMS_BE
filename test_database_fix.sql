-- Test script to check if database columns were added successfully
USE [ITMMS_DB]

-- Check TreatmentPlans table columns
SELECT 'TreatmentPlans' as TableName, COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'TreatmentPlans' 
AND COLUMN_NAME IN ('Notes', 'ProgressNotes', 'PaymentStatus', 'TreatmentServiceId')
ORDER BY COLUMN_NAME;

-- Check Appointments table columns
SELECT 'Appointments' as TableName, COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Appointments' 
AND COLUMN_NAME IN ('Notes', 'TreatmentPlanId')
ORDER BY COLUMN_NAME;

-- Check MedicalRecords table columns
SELECT 'MedicalRecords' as TableName, COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'MedicalRecords' 
AND COLUMN_NAME IN ('Notes', 'AppointmentId')
ORDER BY COLUMN_NAME;

-- Check TestResults table columns
SELECT 'TestResults' as TableName, COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'TestResults' 
AND COLUMN_NAME IN ('Unit', 'DoctorName')
ORDER BY COLUMN_NAME;

-- Test query to see if TreatmentFlow endpoint would work
SELECT TOP 1 
    tp.Id,
    tp.TreatmentType,
    tp.Description,
    tp.StartDate,
    tp.Status,
    tp.CurrentPhase,
    tp.Notes,
    tp.ProgressNotes,
    tp.PaymentStatus,
    d.Id as DoctorId,
    u.FullName as DoctorName,
    d.Specialization
FROM TreatmentPlans tp
INNER JOIN Doctors d ON tp.DoctorId = d.Id
INNER JOIN Users u ON d.UserId = u.Id
WHERE tp.CustomerId = 4
ORDER BY tp.StartDate DESC;

PRINT 'Database fix verification completed!'; 