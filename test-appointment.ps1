# Test Appointment Booking Script
$baseUrl = "http://localhost:5037"

Write-Host "=== Testing ITMMS Appointment Booking ===" -ForegroundColor Green

# Step 1: Register Customer
Write-Host "`n1. Registering new customer..." -ForegroundColor Yellow
$registerBody = @{
    fullName = "Nguyen Thi B"
    email = "customer1@test.com"
    phone = "0123456789"
    address = "Ha Noi"
    username = "customer1"
    password = "123456"
    confirmPassword = "123456"
    role = "Customer"
} | ConvertTo-Json

try {
    $registerResponse = Invoke-WebRequest -Uri "$baseUrl/api/auth/register" -Method POST -ContentType "application/json" -Body $registerBody
    Write-Host "✅ Customer registered successfully" -ForegroundColor Green
    $registerData = $registerResponse.Content | ConvertFrom-Json
    Write-Host "Customer ID: $($registerData.user.id)" -ForegroundColor Cyan
    $customerId = $registerData.user.id
} catch {
    Write-Host "❌ Registration failed: $_" -ForegroundColor Red
    # If user already exists, assume customerId = 3 (after admin=1, doctor=2)
    $customerId = 3
    Write-Host "Using existing customer ID: $customerId" -ForegroundColor Yellow
}

# Step 2: Get Doctors List
Write-Host "`n2. Getting doctors list..." -ForegroundColor Yellow
try {
    $doctorsResponse = Invoke-WebRequest -Uri "$baseUrl/api/doctors" -Method GET
    $doctorsData = $doctorsResponse.Content | ConvertFrom-Json
    Write-Host "✅ Found $($doctorsData.doctors.Count) doctors" -ForegroundColor Green
    
    if ($doctorsData.doctors.Count -gt 0) {
        $doctorId = $doctorsData.doctors[0].id
        $doctorName = $doctorsData.doctors[0].fullName
        Write-Host "Selected Doctor: $doctorName (ID: $doctorId)" -ForegroundColor Cyan
    } else {
        Write-Host "❌ No doctors found" -ForegroundColor Red
        exit
    }
} catch {
    Write-Host "❌ Failed to get doctors: $_" -ForegroundColor Red
    exit
}

# Step 3: Check Available Slots
Write-Host "`n3. Checking available time slots..." -ForegroundColor Yellow
$tomorrow = (Get-Date).AddDays(1).ToString("yyyy-MM-dd")
try {
    $slotsResponse = Invoke-WebRequest -Uri "$baseUrl/api/appointments/available-slots?doctorId=$doctorId&date=$tomorrow" -Method GET
    $slotsData = $slotsResponse.Content | ConvertFrom-Json
    Write-Host "✅ Available slots for $tomorrow`: $($slotsData.availableSlots -join ', ')" -ForegroundColor Green
    
    if ($slotsData.availableSlots.Count -gt 0) {
        $selectedSlot = $slotsData.availableSlots[0]
        Write-Host "Selected time slot: $selectedSlot" -ForegroundColor Cyan
    } else {
        Write-Host "❌ No available slots" -ForegroundColor Red
        $selectedSlot = "09:00-10:00"  # Use default
        Write-Host "Using default slot: $selectedSlot" -ForegroundColor Yellow
    }
} catch {
    Write-Host "❌ Failed to get available slots: $_" -ForegroundColor Red
    $selectedSlot = "09:00-10:00"  # Use default
}

# Step 4: Book Appointment
Write-Host "`n4. Booking appointment..." -ForegroundColor Yellow
$appointmentBody = @{
    doctorId = $doctorId
    appointmentDate = "$tomorrow" + "T09:00:00"
    timeSlot = $selectedSlot
    type = "Consultation"
    notes = "Test appointment booking"
} | ConvertTo-Json

try {
    $appointmentResponse = Invoke-WebRequest -Uri "$baseUrl/api/appointments?customerId=$customerId" -Method POST -ContentType "application/json" -Body $appointmentBody
    $appointmentData = $appointmentResponse.Content | ConvertFrom-Json
    Write-Host "✅ Appointment booked successfully!" -ForegroundColor Green
    Write-Host "Appointment ID: $($appointmentData.appointment.id)" -ForegroundColor Cyan
    Write-Host "Date: $($appointmentData.appointment.appointmentDate)" -ForegroundColor Cyan
    Write-Host "Time: $($appointmentData.appointment.timeSlot)" -ForegroundColor Cyan
    Write-Host "Status: $($appointmentData.appointment.status)" -ForegroundColor Cyan
} catch {
    Write-Host "❌ Failed to book appointment: $_" -ForegroundColor Red
    Write-Host "Error details: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 5: Get Customer's Appointments
Write-Host "`n5. Getting customer's appointments..." -ForegroundColor Yellow
try {
    $customerAppointmentsResponse = Invoke-WebRequest -Uri "$baseUrl/api/appointments/customer/$customerId" -Method GET
    $customerAppointments = $customerAppointmentsResponse.Content | ConvertFrom-Json
    Write-Host "✅ Customer has $($customerAppointments.appointments.Count) appointments" -ForegroundColor Green
    
    foreach ($apt in $customerAppointments.appointments) {
        Write-Host "- Appointment $($apt.id): $($apt.appointmentDate) at $($apt.timeSlot) - Status: $($apt.status)" -ForegroundColor White
    }
} catch {
    Write-Host "❌ Failed to get customer appointments: $_" -ForegroundColor Red
}

Write-Host "`n=== Test Complete ===" -ForegroundColor Green 