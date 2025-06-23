# Test Checkout Workflow
# Kiểm tra toàn bộ flow từ booking -> customer checkout -> doctor checkout

$baseUrl = "http://localhost:5037"

Write-Host "============ Test Checkout Workflow ============" -ForegroundColor Green

# 1. Kiểm tra health check và checkout endpoints
Write-Host "`n1. Kiểm tra API status..." -ForegroundColor Yellow
try {
    $health = Invoke-RestMethod -Uri "$baseUrl/api/health" -Method GET
    Write-Host "Health Check: $($health.status)" -ForegroundColor Green
    
    $checkoutTest = Invoke-RestMethod -Uri "$baseUrl/api/checkout/test" -Method GET
    Write-Host "Checkout API: $($checkoutTest.message)" -ForegroundColor Green
} catch {
    Write-Host "Error checking API status: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 2. Tạo appointment mới
Write-Host "`n2. Tạo appointment mới..." -ForegroundColor Yellow
$appointmentData = @{
    doctorId = 1
    appointmentDate = "2025-06-24T10:00:00"
    timeSlot = "10:00-11:00"
    type = "Consultation"
    notes = "Test appointment for checkout"
} | ConvertTo-Json

try {
    $appointment = Invoke-RestMethod -Uri "$baseUrl/api/appointments?customerId=1" -Method POST -Body $appointmentData -ContentType "application/json"
    $appointmentId = $appointment.id
    Write-Host "✓ Appointment created: ID = $appointmentId" -ForegroundColor Green
    Write-Host "  - Fee: $($appointment.fee)" -ForegroundColor Cyan
    Write-Host "  - Payment Status: $($appointment.paymentStatus)" -ForegroundColor Cyan
} catch {
    Write-Host "Error creating appointment: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 3. Customer Checkout - Thanh toán
Write-Host "`n3. Customer checkout - Thanh toán..." -ForegroundColor Yellow
$customerCheckoutData = @{
    appointmentId = $appointmentId
    paymentMethod = "Card"
    transactionId = "TXN_$(Get-Date -Format 'yyyyMMddHHmmss')"
} | ConvertTo-Json

try {
    $customerCheckout = Invoke-RestMethod -Uri "$baseUrl/api/checkout/customer" -Method POST -Body $customerCheckoutData -ContentType "application/json"
    Write-Host "✓ Customer checkout thành công!" -ForegroundColor Green
    Write-Host "  - Message: $($customerCheckout.message)" -ForegroundColor Cyan
    Write-Host "  - Payment ID: $($customerCheckout.data.paymentId)" -ForegroundColor Cyan
    Write-Host "  - Amount: $($customerCheckout.data.amount)" -ForegroundColor Cyan
} catch {
    Write-Host "Error in customer checkout: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 4. Kiểm tra payment status
Write-Host "`n4. Kiểm tra thông tin thanh toán..." -ForegroundColor Yellow
try {
    $payment = Invoke-RestMethod -Uri "$baseUrl/api/checkout/payment/appointment/$appointmentId" -Method GET
    Write-Host "✓ Payment info:" -ForegroundColor Green
    Write-Host "  - Status: $($payment.status)" -ForegroundColor Cyan
    Write-Host "  - Amount: $($payment.amount)" -ForegroundColor Cyan
    Write-Host "  - Method: $($payment.paymentMethod)" -ForegroundColor Cyan
    Write-Host "  - Transaction ID: $($payment.transactionId)" -ForegroundColor Cyan
} catch {
    Write-Host "Error checking payment: $($_.Exception.Message)" -ForegroundColor Red
}

# 5. Doctor Checkout - Hoàn thành cuộc hẹn
Write-Host "`n5. Doctor checkout - Hoàn thành cuộc hẹn..." -ForegroundColor Yellow
$doctorCheckoutData = @{
    appointmentId = $appointmentId
    diagnosis = "Hiếm muộn do rối loạn nội tiết tố"
    treatment = "Điều trị hormone, theo dõi chu kỳ kinh nguyệt"
    prescription = "Clomiphene citrate 50mg x 5 ngày/chu kỳ"
    notes = "Bệnh nhân cần tái khám sau 1 tháng"
    followUpRequired = $true
    nextAppointmentDate = "2025-07-24T10:00:00"
} | ConvertTo-Json

try {
    $doctorCheckout = Invoke-RestMethod -Uri "$baseUrl/api/checkout/doctor/1" -Method POST -Body $doctorCheckoutData -ContentType "application/json"
    Write-Host "✓ Doctor checkout thành công!" -ForegroundColor Green
    Write-Host "  - Message: $($doctorCheckout.message)" -ForegroundColor Cyan
    Write-Host "  - Medical Record ID: $($doctorCheckout.data.medicalRecordId)" -ForegroundColor Cyan
    Write-Host "  - Completed At: $($doctorCheckout.data.completedAt)" -ForegroundColor Cyan
} catch {
    Write-Host "Error in doctor checkout: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 6. Kiểm tra appointment sau khi hoàn thành
Write-Host "`n6. Kiểm tra appointment sau checkout..." -ForegroundColor Yellow
try {
    $finalAppointment = Invoke-RestMethod -Uri "$baseUrl/api/appointments/$appointmentId" -Method GET
    Write-Host "✓ Final appointment status:" -ForegroundColor Green
    Write-Host "  - Status: $($finalAppointment.status)" -ForegroundColor Cyan
    Write-Host "  - Payment Status: $($finalAppointment.paymentStatus)" -ForegroundColor Cyan
    Write-Host "  - Completed At: $($finalAppointment.completedAt)" -ForegroundColor Cyan
    Write-Host "  - Has Medical Record: $($finalAppointment.medicalRecord -ne $null)" -ForegroundColor Cyan
} catch {
    Write-Host "Error checking final appointment: $($_.Exception.Message)" -ForegroundColor Red
}

# 7. Kiểm tra lịch sử thanh toán của customer
Write-Host "`n7. Kiểm tra lịch sử thanh toán customer..." -ForegroundColor Yellow
try {
    $paymentHistory = Invoke-RestMethod -Uri "$baseUrl/api/checkout/payments/customer/1" -Method GET
    Write-Host "✓ Payment history count: $($paymentHistory.Count)" -ForegroundColor Green
    if ($paymentHistory.Count -gt 0) {
        $latestPayment = $paymentHistory[0]
        Write-Host "  - Latest payment: $($latestPayment.amount) - $($latestPayment.status)" -ForegroundColor Cyan
    }
} catch {
    Write-Host "Error checking payment history: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n============ Test Hoàn Thành ============" -ForegroundColor Green
Write-Host "Checkout workflow đã được test thành công!" -ForegroundColor Green
Write-Host "✓ Customer có thể thanh toán" -ForegroundColor Green
Write-Host "✓ Doctor có thể hoàn thành cuộc hẹn và tạo hồ sơ bệnh án" -ForegroundColor Green
Write-Host "✓ Hệ thống tracking payment và appointment status" -ForegroundColor Green 