-- Script tạo data mẫu cho ITMMS
USE [ITMMS_DB]

-- 1. Thêm Users
INSERT INTO Users (Username, Password, Email, FullName, Phone, Address, Role, CreatedAt, IsActive)
VALUES 
('admin', 'admin123', 'admin@itmms.com', 'Admin User', '0123456789', 'Hà Nội', 'Admin', GETDATE(), 1),
('doctor1', 'doctor123', 'doctor1@itmms.com', 'Bác sĩ Nguyễn Văn A', '0987654321', 'Hà Nội', 'Doctor', GETDATE(), 1),
('doctor2', 'doctor123', 'doctor2@itmms.com', 'Bác sĩ Trần Thị B', '0987654322', 'TP.HCM', 'Doctor', GETDATE(), 1),
('patient1', 'patient123', 'patient1@gmail.com', 'Nguyễn Thị C', '0987654323', 'Hà Nội', 'Customer', GETDATE(), 1),
('patient2', 'patient123', 'patient2@gmail.com', 'Lê Văn D', '0987654324', 'TP.HCM', 'Customer', GETDATE(), 1),
('patient3', 'patient123', 'patient3@gmail.com', 'Phạm Thị E', '0987654325', 'Đà Nẵng', 'Customer', GETDATE(), 1),
('patient4', 'patient123', 'thanht@gmail.com', 'Nguyễn Văn Thanh', '0987654326', 'Hà Nội', 'Customer', GETDATE(), 1);

-- 2. Thêm Doctors
INSERT INTO Doctors (UserId, Specialization, LicenseNumber, Education, Description, ExperienceYears, ConsultationFee, IsAvailable)
VALUES 
(2, 'Reproductive Medicine', 'BS001', 'Đại học Y Hà Nội', 'Chuyên gia về điều trị hiếm muộn', 10, 500000, 1),
(3, 'Gynecology', 'BS002', 'Đại học Y TP.HCM', 'Chuyên gia phụ khoa', 8, 400000, 1);

-- 3. Thêm Customers
INSERT INTO Customers (UserId, DateOfBirth, Gender, EmergencyContact)
VALUES 
(4, '1990-01-15', 'Female', '0123456789'),
(5, '1985-03-20', 'Male', '0123456790'),
(6, '1992-07-10', 'Female', '0123456791'),
(7, '1988-12-05', 'Male', '0123456792');

-- 4. Thêm TreatmentServices
INSERT INTO TreatmentServices (ServiceName, ServiceCode, Description, BasePrice, Procedures, Requirements, DurationDays, SuccessRate, IsActive)
VALUES 
('IVF Treatment', 'IVF001', 'In vitro fertilization treatment', 50000000, 'Egg retrieval, fertilization, embryo transfer', 'Age under 40, healthy uterus', 30, 65.5, 1),
('IUI Treatment', 'IUI001', 'Intrauterine insemination', 15000000, 'Sperm preparation, insemination', 'Healthy fallopian tubes', 15, 45.0, 1),
('Hormone Therapy', 'HT001', 'Hormone treatment for ovulation', 8000000, 'Medication, monitoring', 'Regular menstrual cycle', 21, 70.0, 1);

-- 5. Thêm TreatmentPlans
INSERT INTO TreatmentPlans (CustomerId, DoctorId, TreatmentServiceId, TreatmentType, Description, StartDate, EndDate, Status, CurrentPhase, PhaseDescription, NextPhaseDate, NextVisitDate, TotalCost, PaidAmount, PaymentStatus, Notes, ProgressNotes)
VALUES 
(1, 1, 1, 'IVF', 'In vitro fertilization treatment for patient', '2024-01-15', '2024-02-15', 'Active', 2, 'Egg retrieval phase', '2024-01-25', '2024-01-25', 50000000, 25000000, 'Partial', 'Patient responding well to treatment', 'Phase 1 completed successfully'),
(2, 1, 2, 'IUI', 'Intrauterine insemination treatment', '2024-01-20', '2024-02-05', 'Active', 1, 'Initial consultation', '2024-01-30', '2024-01-30', 15000000, 7500000, 'Partial', 'Treatment started', 'Initial phase in progress'),
(3, 2, 3, 'Hormone', 'Hormone therapy for ovulation', '2024-01-10', '2024-01-31', 'Completed', 3, 'Treatment completed', NULL, NULL, 8000000, 8000000, 'Paid', 'Treatment completed successfully', 'All phases completed'),
(4, 1, 1, 'IVF', 'IVF treatment for patient Thanh', '2024-02-01', '2024-03-01', 'Active', 1, 'Initial consultation', '2024-02-10', '2024-02-10', 50000000, 0, 'Pending', 'New treatment plan', 'Starting treatment');

-- 6. Thêm Appointments
INSERT INTO Appointments (CustomerId, DoctorId, TreatmentPlanId, AppointmentDate, TimeSlot, Type, Status, Notes)
VALUES 
(1, 1, 1, '2024-01-20', '09:00-10:00', 'Consultation', 'Completed', 'Initial consultation completed'),
(1, 1, 1, '2024-01-25', '08:00-09:00', 'Procedure', 'Scheduled', 'Egg retrieval procedure'),
(2, 1, 2, '2024-01-30', '10:00-11:00', 'Consultation', 'Scheduled', 'IUI consultation'),
(3, 2, 3, '2024-01-15', '14:00-15:00', 'Follow-up', 'Completed', 'Hormone therapy follow-up'),
(4, 1, 4, '2024-02-10', '09:00-10:00', 'Consultation', 'Scheduled', 'Initial consultation for patient Thanh');

-- 7. Thêm MedicalRecords
INSERT INTO MedicalRecords (CustomerId, DoctorId, AppointmentId, Symptoms, Diagnosis, Treatment, Prescription, Notes, RecordDate)
VALUES 
(1, 1, 1, 'Irregular menstrual cycle, difficulty conceiving', 'Ovulatory dysfunction', 'Hormone therapy and IVF treatment', 'Clomiphene citrate 50mg daily', 'Patient shows good response to treatment', '2024-01-20'),
(2, 1, 3, 'Infertility for 2 years', 'Unexplained infertility', 'IUI treatment recommended', 'Folic acid supplements', 'Patient ready for IUI treatment', '2024-01-30'),
(3, 2, 4, 'Irregular periods', 'Hormonal imbalance', 'Hormone therapy completed', 'Progesterone supplements', 'Treatment completed successfully', '2024-01-15'),
(4, 1, 5, 'Difficulty conceiving for 3 years', 'Male factor infertility', 'IVF treatment recommended', 'Multivitamins and antioxidants', 'Initial assessment completed', '2024-02-10');

-- 8. Thêm TestResults
INSERT INTO TestResults (CustomerId, DoctorId, TestName, TestType, Results, NormalRange, Status, TestDate, Unit, DoctorName)
VALUES 
(1, 1, 'FSH Test', 'Hormone Test', '8.5', '3.5-12.5', 'Normal', '2024-01-18', 'mIU/mL', 'Bác sĩ Nguyễn Văn A'),
(1, 1, 'LH Test', 'Hormone Test', '6.2', '2.4-12.6', 'Normal', '2024-01-18', 'mIU/mL', 'Bác sĩ Nguyễn Văn A'),
(1, 1, 'AMH Test', 'Hormone Test', '2.1', '1.0-4.0', 'Normal', '2024-01-18', 'ng/mL', 'Bác sĩ Nguyễn Văn A'),
(2, 1, 'Semen Analysis', 'Sperm Test', '15 million/mL', '15-200 million/mL', 'Low', '2024-01-25', 'million/mL', 'Bác sĩ Nguyễn Văn A'),
(3, 2, 'Thyroid Test', 'Hormone Test', '2.5', '0.4-4.0', 'Normal', '2024-01-12', 'mIU/L', 'Bác sĩ Trần Thị B'),
(4, 1, 'FSH Test', 'Hormone Test', '7.8', '3.5-12.5', 'Normal', '2024-02-05', 'mIU/mL', 'Bác sĩ Nguyễn Văn A');

-- 9. Thêm Prescriptions
INSERT INTO Prescriptions (MedicalRecordId, MedicineName, Dosage, Frequency, Duration, Instructions)
VALUES 
(1, 'Clomiphene citrate', '50mg', 'Once daily', '5 days', 'Take in the morning'),
(1, 'Folic acid', '400mcg', 'Once daily', '30 days', 'Take with food'),
(2, 'Folic acid', '400mcg', 'Once daily', '30 days', 'Take with food'),
(3, 'Progesterone', '200mg', 'Twice daily', '14 days', 'Take after meals'),
(4, 'Multivitamins', '1 tablet', 'Once daily', '30 days', 'Take in the morning');

-- 10. Thêm Feedbacks
INSERT INTO Feedbacks (CustomerId, DoctorId, AppointmentId, Rating, Comment, CreatedAt)
VALUES 
(1, 1, 1, 5, 'Bác sĩ rất tận tâm và chuyên nghiệp', '2024-01-20'),
(2, 1, 3, 4, 'Tư vấn rất chi tiết và dễ hiểu', '2024-01-30'),
(3, 2, 4, 5, 'Điều trị hiệu quả, rất hài lòng', '2024-01-15');

PRINT 'Sample data created successfully!';
PRINT 'Total records created:';
PRINT '- Users: 7';
PRINT '- Doctors: 2';
PRINT '- Customers: 4';
PRINT '- TreatmentServices: 3';
PRINT '- TreatmentPlans: 4';
PRINT '- Appointments: 5';
PRINT '- MedicalRecords: 4';
PRINT '- TestResults: 6';
PRINT '- Prescriptions: 5';
PRINT '- Feedbacks: 3'; 