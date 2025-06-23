# Test script cho Unified Auth Controller
Write-Host "=== TESTING UNIFIED AUTH CONTROLLER ===" -ForegroundColor Green

$baseUrl = "http://localhost:5037/api/auth"

# Test 1: Login
Write-Host "`n1. Testing Login..." -ForegroundColor Yellow
try {
    $loginData = @{
        email = "customer@test.com"
        password = "123456"
    }
    
    $response = Invoke-RestMethod -Uri "$baseUrl/login" -Method POST -Body ($loginData | ConvertTo-Json) -ContentType "application/json"
    Write-Host "PASS - Login Success: $($response.message)" -ForegroundColor Green
    Write-Host "  User Role: $($response.user.role)" -ForegroundColor Cyan
    
    $userId = $response.user.id
} catch {
    Write-Host "FAIL - Login Failed: $($_.Exception.Message)" -ForegroundColor Red
    $userId = 2  # fallback to customer ID
}

# Test 3: Check Email
Write-Host "`n3. Testing Check Email..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/check-email" -Method POST -Body '"customer@test.com"' -ContentType "application/json"
    Write-Host "PASS - Check Email Success: exists = $($response.exists)" -ForegroundColor Green
} catch {
    Write-Host "FAIL - Check Email Failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 4: Check Username
Write-Host "`n4. Testing Check Username..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/check-username" -Method POST -Body '"customer1"' -ContentType "application/json"
    Write-Host "✓ Check Username Success: exists = $($response.exists)" -ForegroundColor Green
} catch {
    Write-Host "✗ Check Username Failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 5: Get Profile
Write-Host "`n5. Testing Get Profile..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/profile?id=$userId" -Method GET
    Write-Host "PASS - Get Profile Success: $($response.fullName)" -ForegroundColor Green
} catch {
    Write-Host "FAIL - Get Profile Failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 6: Update Profile
Write-Host "`n6. Testing Update Profile..." -ForegroundColor Yellow
try {
    $updateData = @{
        fullName = "Updated Customer Name"
        phone = "0987654321"
        address = "Updated Address"
    }
    
    $response = Invoke-RestMethod -Uri "$baseUrl/profile?id=$userId" -Method PUT -Body ($updateData | ConvertTo-Json) -ContentType "application/json"
    Write-Host "✓ Update Profile Success: $($response.message)" -ForegroundColor Green
} catch {
    Write-Host "✗ Update Profile Failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 7: Get Treatment History
Write-Host "`n7. Testing Get Treatment History..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/history?userId=$userId" -Method GET
    Write-Host "PASS - Get History Success: Found $($response.history.Count) records" -ForegroundColor Green
    if ($response.history.Count -gt 0) {
        $firstRecord = $response.history[0]
        Write-Host "  First Record: $($firstRecord.treatmentType)" -ForegroundColor Cyan
    }
} catch {
    Write-Host "FAIL - Get History Failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 8: Create Feedback
Write-Host "`n8. Testing Create Feedback..." -ForegroundColor Yellow
try {
    $feedbackData = @{
        userId = $userId
        doctorId = 1
        appointmentId = 1
        rating = 5
        comment = "Excellent service from unified auth controller!"
    }
    
    $response = Invoke-RestMethod -Uri "$baseUrl/feedback" -Method POST -Body ($feedbackData | ConvertTo-Json) -ContentType "application/json"
    Write-Host "PASS - Create Feedback Success: $($response.message)" -ForegroundColor Green
} catch {
    Write-Host "FAIL - Create Feedback Failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 9: Change Password  
Write-Host "`n9. Testing Change Password..." -ForegroundColor Yellow
try {
    $changePasswordData = @{
        userId = $userId
        oldPassword = "123456"
        newPassword = "654321"
    }
    
    $response = Invoke-RestMethod -Uri "$baseUrl/change-password" -Method POST -Body ($changePasswordData | ConvertTo-Json) -ContentType "application/json"
    Write-Host "✓ Change Password Success: $($response.message)" -ForegroundColor Green
} catch {
    Write-Host "✗ Change Password Failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n=== UNIFIED AUTH CONTROLLER TEST COMPLETED ===" -ForegroundColor Green
Write-Host "Success! Only 1 controller for all authentication and user management!" -ForegroundColor Magenta 