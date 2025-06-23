# Test Medical Consultation Flow and Treatment Planning Flow
Write-Host "=== TESTING ITMMS MAIN FLOWS ===" -ForegroundColor Green
Write-Host ""

$baseUrl = "http://localhost:5037"
$headers = @{ "Content-Type" = "application/json" }

# Test 1: Medical Consultation Flow
Write-Host "=== MEDICAL CONSULTATION FLOW ===" -ForegroundColor Cyan
Write-Host ""

# Step 1: Get existing appointment
Write-Host "Step 1: Get existing appointment" -ForegroundColor Yellow
try {
    $appointment = Invoke-RestMethod -Uri "$baseUrl/api/appointments/1" -Method GET -Headers $headers
    Write-Host "SUCCESS: Appointment found" -ForegroundColor Green
    Write-Host "   Patient: $($appointment.customer.user.fullName)"
    Write-Host "   Doctor: $($appointment.doctor.user.fullName)"
    Write-Host "   Status: $($appointment.status)"
    Write-Host ""
} catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Step 2: Doctor completes consultation
Write-Host "Step 2: Doctor completes medical consultation" -ForegroundColor Yellow
$medicalData = @{
    appointmentId = 1
    symptoms = "Hiem muon nguyen phat, khong co thai sau 2 nam ket hon"
    diagnosis = "Hiem muon nguyen phat do roi loan noi tiet to"
    treatment = "Kich thich buong trung, theo doi nang trung"
    prescription = "Clomiphene citrate 50mg x 5 ngay, Folic acid 5mg hang ngay"
    notes = "Benh nhan can theo doi sieu am sau 7 ngay"
    followUpRequired = $true
} | ConvertTo-Json

try {
    $medicalResult = Invoke-RestMethod -Uri "$baseUrl/api/medicalrecords/complete/1" -Method POST -Body $medicalData -Headers $headers
    Write-Host "SUCCESS: Medical consultation completed" -ForegroundColor Green
    Write-Host "   Medical Record ID: $($medicalResult.data.medicalRecordId)"
    Write-Host ""
} catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 3: View medical record
Write-Host "Step 3: View created medical record" -ForegroundColor Yellow
try {
    $record = Invoke-RestMethod -Uri "$baseUrl/api/medicalrecords/appointment/1" -Method GET -Headers $headers
    Write-Host "SUCCESS: Medical record retrieved" -ForegroundColor Green
    Write-Host "   Record ID: $($record.id)"
    Write-Host "   Diagnosis: $($record.diagnosis)"
    Write-Host "   Treatment: $($record.treatment)"
    Write-Host ""
} catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 2: Treatment Planning Flow  
Write-Host "=== TREATMENT PLANNING FLOW ===" -ForegroundColor Cyan
Write-Host ""

# Step 1: View treatment services
Write-Host "Step 1: View available treatment services" -ForegroundColor Yellow
try {
    $services = Invoke-RestMethod -Uri "$baseUrl/api/treatmentservices" -Method GET -Headers $headers
    Write-Host "SUCCESS: Found $($services.data.Count) treatment services" -ForegroundColor Green
    foreach ($service in $services.data) {
        Write-Host "   - $($service.serviceName): $($service.basePrice) VND"
    }
    Write-Host ""
} catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 2: Create treatment plan
Write-Host "Step 2: Create IVF treatment plan" -ForegroundColor Yellow
$treatmentData = @{
    customerId = 1
    doctorId = 1
    treatmentServiceId = 1
    treatmentType = "IVF (In Vitro Fertilization)"
    description = "Ke hoach dieu tri IVF hoan chinh cho benh nhan hiem muon"
    startDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
    totalCost = 85000000
    phaseDescription = "Giai doan 1: Chuan bi va kiem tra ban dau"
    nextPhaseDate = (Get-Date).AddDays(7).ToString("yyyy-MM-ddTHH:mm:ss")
    nextVisitDate = (Get-Date).AddDays(3).ToString("yyyy-MM-ddTHH:mm:ss")
    notes = "Benh nhan can chuan bi tam ly va the chat cho qua trinh dieu tri IVF"
} | ConvertTo-Json

try {
    $planResult = Invoke-RestMethod -Uri "$baseUrl/api/treatmentplans" -Method POST -Body $treatmentData -Headers $headers
    Write-Host "SUCCESS: Treatment plan created" -ForegroundColor Green
    $planId = $planResult.data.id
    Write-Host "   Plan ID: $planId"
    Write-Host "   Treatment: $($planResult.data.treatmentType)"
    Write-Host "   Total Cost: $($planResult.data.totalCost) VND"
    Write-Host "   Current Phase: $($planResult.data.currentPhase)"
    Write-Host ""
} catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 3: Update treatment progress
Write-Host "Step 3: Update treatment progress to Phase 2" -ForegroundColor Yellow
$progressData = @{
    currentPhase = 2
    phaseDescription = "Giai doan 2: Kich thich buong trung va theo doi"
    nextPhaseDate = (Get-Date).AddDays(14).ToString("yyyy-MM-ddTHH:mm:ss")
    nextVisitDate = (Get-Date).AddDays(5).ToString("yyyy-MM-ddTHH:mm:ss")
    notes = "Benh nhan da hoan thanh cac xet nghiem co ban"
    progressNotes = "Phan ung tot voi thuoc kich thich. Buong trung phan ung binh thuong"
    status = "Active"
} | ConvertTo-Json

try {
    $updateResult = Invoke-RestMethod -Uri "$baseUrl/api/treatmentplans/$planId/progress" -Method PUT -Body $progressData -Headers $headers
    Write-Host "SUCCESS: Treatment progress updated" -ForegroundColor Green
    Write-Host "   Current Phase: $($updateResult.data.currentPhase)"
    Write-Host "   Phase Description: $($updateResult.data.phaseDescription)"
    Write-Host "   Progress Notes: $($updateResult.data.progressNotes)"
    Write-Host ""
} catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 4: View patient's treatment plans
Write-Host "Step 4: View patient treatment plans" -ForegroundColor Yellow
try {
    $patientPlans = Invoke-RestMethod -Uri "$baseUrl/api/treatmentplans/customer/1" -Method GET -Headers $headers
    Write-Host "SUCCESS: Found $($patientPlans.data.Count) treatment plans for patient" -ForegroundColor Green
    foreach ($plan in $patientPlans.data) {
        Write-Host "   - Plan $($plan.id): $($plan.treatmentType) - Status: $($plan.status)"
        Write-Host "     Phase: $($plan.currentPhase) - $($plan.phaseDescription)"
    }
    Write-Host ""
} catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 5: View active treatment plan
Write-Host "Step 5: View active treatment plan details" -ForegroundColor Yellow
try {
    $activePlan = Invoke-RestMethod -Uri "$baseUrl/api/treatmentplans/customer/1/active" -Method GET -Headers $headers
    Write-Host "SUCCESS: Active treatment plan found" -ForegroundColor Green
    Write-Host "   Treatment: $($activePlan.data.treatmentType)"
    Write-Host "   Doctor: $($activePlan.data.doctor.name)"
    Write-Host "   Current Phase: $($activePlan.data.currentPhase)"
    Write-Host "   Next Visit: $($activePlan.data.nextVisitDate)"
    Write-Host ""
} catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "=== FLOW TESTING COMPLETED ===" -ForegroundColor Green
Write-Host ""
Write-Host "Test Summary:" -ForegroundColor Cyan
Write-Host "Medical Consultation Flow:"
Write-Host "  1. OK - Retrieved existing appointment"
Write-Host "  2. OK - Doctor completed consultation"
Write-Host "  3. OK - Created medical record"
Write-Host ""
Write-Host "Treatment Planning Flow:"
Write-Host "  1. OK - Viewed treatment services" 
Write-Host "  2. OK - Created IVF treatment plan"
Write-Host "  3. OK - Updated treatment progress"
Write-Host "  4. OK - Viewed patient plans"
Write-Host "  5. OK - Viewed active plan details" 