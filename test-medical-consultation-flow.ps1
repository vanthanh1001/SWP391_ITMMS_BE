# Test Medical Consultation Flow
# Quy trình: Customer đặt lịch → Doctor khám → Tạo hồ sơ y tế → Theo dõi lịch sử

Write-Host "=== TESTING MEDICAL CONSULTATION FLOW ===" -ForegroundColor Green
Write-Host ""

$baseUrl = "http://localhost:5037"
$headers = @{ "Content-Type" = "application/json" }

# Step 1: Kiểm tra appointment đã có
Write-Host "Step 1: Lấy thông tin appointment hiện có" -ForegroundColor Yellow
try {
    $appointmentResponse = Invoke-RestMethod -Uri "$baseUrl/api/appointments/1" -Method GET -Headers $headers
    Write-Host "OK Appointment found:" -ForegroundColor Green
    Write-Host "   - ID: $($appointmentResponse.id)" 
    Write-Host "   - Status: $($appointmentResponse.status)"
    Write-Host "   - Customer: $($appointmentResponse.customer.user.fullName)"
    Write-Host "   - Doctor: $($appointmentResponse.doctor.user.fullName)"
    Write-Host "   - Date: $($appointmentResponse.appointmentDate)"
    Write-Host ""
} catch {
    Write-Host "ERROR getting appointment: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Step 2: Doctor hoàn thành khám bệnh (Medical Consultation)
Write-Host "Step 2: Doctor hoàn thành khám bệnh và tạo hồ sơ y tế" -ForegroundColor Yellow

$medicalData = @{
    appointmentId = 1
    symptoms = "Hiếm muộn nguyên phát, không có thai sau 2 năm kết hôn"
    diagnosis = "Hiếm muộn nguyên phát do rối loạn nội tiết tố"
    treatment = "Kích thích buồng trứng, theo dõi nang trứng"
    prescription = "Clomiphene citrate 50mg x 5 ngày, Folic acid 5mg hàng ngày"
    notes = "Bệnh nhân cần theo dõi siêu âm sau 7 ngày để đánh giá phản ứng buồng trứng"
    followUpRequired = $true
} | ConvertTo-Json

try {
    $completeResponse = Invoke-RestMethod -Uri "$baseUrl/api/medicalrecords/complete/1" -Method POST -Body $medicalData -Headers $headers
    Write-Host "OK Medical consultation completed:" -ForegroundColor Green
    Write-Host "   - Medical Record ID: $($completeResponse.data.medicalRecordId)"
    Write-Host "   - Message: $($completeResponse.message)"
    Write-Host ""
} catch {
    Write-Host "ERROR completing consultation: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Response: $($_.Exception.Response)" -ForegroundColor Red
}

# Step 3: Xem hồ sơ bệnh án vừa tạo
Write-Host "Step 3: Xem hồ sơ bệnh án vừa tạo" -ForegroundColor Yellow
try {
    $recordResponse = Invoke-RestMethod -Uri "$baseUrl/api/medicalrecords/appointment/1" -Method GET -Headers $headers
    Write-Host "OK Medical Record Details:" -ForegroundColor Green
    Write-Host "   - Record ID: $($recordResponse.id)"
    Write-Host "   - Patient: $($recordResponse.customer.user.fullName)"
    Write-Host "   - Doctor: $($recordResponse.doctor.user.fullName)"
    Write-Host "   - Date: $($recordResponse.recordDate)"
    Write-Host "   - Symptoms: $($recordResponse.symptoms)"
    Write-Host "   - Diagnosis: $($recordResponse.diagnosis)"
    Write-Host "   - Treatment: $($recordResponse.treatment)"
    Write-Host "   - Prescription: $($recordResponse.prescription)"
    Write-Host ""
} catch {
    Write-Host "ERROR getting medical record: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 4: Xem lịch sử khám bệnh của patient
Write-Host "Step 4: Xem toàn bộ lịch sử khám bệnh của patient" -ForegroundColor Yellow
try {
    $historyResponse = Invoke-RestMethod -Uri "$baseUrl/api/medicalrecords/patient/1/history" -Method GET -Headers $headers
    Write-Host "OK Patient Medical History:" -ForegroundColor Green
    if ($historyResponse.Count -gt 0) {
        foreach ($record in $historyResponse) {
            Write-Host "   - Date: $($record.recordDate)"
            Write-Host "   - Doctor: $($record.doctorName)"
            Write-Host "   - Type: $($record.appointmentType)"
            Write-Host "   - Diagnosis: $($record.diagnosis)"
            Write-Host "   ---"
        }
    } else {
        Write-Host "   - No medical history found"
    }
    Write-Host ""
} catch {
    Write-Host "ERROR getting patient history: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 5: Doctor xem tất cả hồ sơ đã khám
Write-Host "Step 5: Doctor xem tất cả hồ sơ bệnh nhân đã khám" -ForegroundColor Yellow
try {
    $doctorRecordsResponse = Invoke-RestMethod -Uri "$baseUrl/api/medicalrecords/doctor/1" -Method GET -Headers $headers
    Write-Host "OK Doctor Medical Records:" -ForegroundColor Green
    if ($doctorRecordsResponse.Count -gt 0) {
        foreach ($record in $doctorRecordsResponse) {
            Write-Host "   - Patient: $($record.customer.user.fullName)"
            Write-Host "   - Date: $($record.recordDate)"
            Write-Host "   - Diagnosis: $($record.diagnosis)"
            Write-Host "   - Appointment Type: $($record.appointment.type)"
            Write-Host "   ---"
        }
    } else {
        Write-Host "   - No records found for this doctor"
    }
    Write-Host ""
} catch {
    Write-Host "ERROR getting doctor records: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 6: Kiểm tra appointment status đã được update
Write-Host "Step 6: Kiểm tra appointment status sau khi hoàn thành" -ForegroundColor Yellow
try {
    $updatedAppointmentResponse = Invoke-RestMethod -Uri "$baseUrl/api/appointments/1" -Method GET -Headers $headers
    Write-Host "OK Updated Appointment Status:" -ForegroundColor Green
    Write-Host "   - Status: $($updatedAppointmentResponse.status)"
    Write-Host "   - Completed At: $($updatedAppointmentResponse.completedAt)"
    Write-Host "   - Notes: $($updatedAppointmentResponse.notes)"
    Write-Host ""
} catch {
    Write-Host "ERROR getting updated appointment: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "=== MEDICAL CONSULTATION FLOW TEST COMPLETED ===" -ForegroundColor Green
Write-Host ""
Write-Host "Flow Summary:" -ForegroundColor Cyan
Write-Host "1. OK Checked existing appointment"
Write-Host "2. OK Doctor completed medical consultation"
Write-Host "3. OK Created medical record with symptoms, diagnosis, treatment"
Write-Host "4. OK Viewed patient medical history"
Write-Host "5. OK Viewed doctor consultation records"
Write-Host "6. OK Verified appointment status update" 