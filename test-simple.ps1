# Test Medical Records Workflow
$baseUrl = "http://localhost:5037"

Write-Host "============ Test Medical Records Workflow ============" -ForegroundColor Green

# 1. Test API status
Write-Host "1. Testing API status..." -ForegroundColor Yellow
try {
    $health = Invoke-RestMethod -Uri "$baseUrl/api/health" -Method GET
    Write-Host "Health Check: $($health.status)" -ForegroundColor Green
    
    $medicalTest = Invoke-RestMethod -Uri "$baseUrl/api/medicalrecords/test" -Method GET
    Write-Host "Medical Records API: $($medicalTest.message)" -ForegroundColor Green
} catch {
    Write-Host "Error checking API status: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 2. Doctor Complete Appointment
Write-Host "2. Doctor completing appointment..." -ForegroundColor Yellow
$doctorData = @{
    appointmentId = 1
    symptoms = "Hiem muon, kho thu thai sau 2 nam co gang"
    diagnosis = "Hiem muon nguyen phat - roi loan noi tiet to"
    treatment = "Kich thich buong trung, theo doi chu ky kinh nguyet"
    prescription = "Clomiphene citrate 50mg, 1 vien/ngay x 5 ngay/chu ky"
    notes = "Benh nhan can tai kham sau 1 thang"
    followUpRequired = $true
    nextAppointmentDate = "2025-07-24T14:00:00"
} | ConvertTo-Json

try {
    $result = Invoke-RestMethod -Uri "$baseUrl/api/medicalrecords/complete/1" -Method POST -Body $doctorData -ContentType "application/json"
    Write-Host "Doctor completed appointment successfully!" -ForegroundColor Green
    Write-Host "Medical Record ID: $($result.data.medicalRecordId)" -ForegroundColor Cyan
} catch {
    Write-Host "Error in doctor completion: $($_.Exception.Message)" -ForegroundColor Red
}

# 3. Get Medical Record
Write-Host "3. Getting medical record..." -ForegroundColor Yellow
try {
    $record = Invoke-RestMethod -Uri "$baseUrl/api/medicalrecords/appointment/1" -Method GET
    Write-Host "Medical Record found - ID: $($record.id)" -ForegroundColor Green
    Write-Host "Diagnosis: $($record.diagnosis)" -ForegroundColor Cyan
} catch {
    Write-Host "Error getting medical record: $($_.Exception.Message)" -ForegroundColor Red
}

# 4. Get Patient History
Write-Host "4. Getting patient history..." -ForegroundColor Yellow
try {
    $history = Invoke-RestMethod -Uri "$baseUrl/api/medicalrecords/patient/1/history" -Method GET
    Write-Host "Patient history count: $($history.Count)" -ForegroundColor Green
} catch {
    Write-Host "Error getting patient history: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "Test completed!" -ForegroundColor Green
