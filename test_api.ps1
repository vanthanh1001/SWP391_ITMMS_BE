# Test Script cho ITMMS API
$baseUrl = "http://localhost:5037/api"

Write-Host "=== TESTING ITMMS API ===" -ForegroundColor Green

# Test 1: Kiểm tra Swagger
Write-Host "`n1. Testing Swagger UI..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/../swagger" -Method GET
    Write-Host "✅ Swagger UI: OK" -ForegroundColor Green
} catch {
    Write-Host "❌ Swagger UI: FAILED" -ForegroundColor Red
}

# Test 2: Test Auth endpoints
Write-Host "`n2. Testing Auth endpoints..." -ForegroundColor Yellow
try {
    # Test register
    $registerData = @{
        Username = "testuser"
        Password = "Test123!"
        Email = "test@example.com"
        FullName = "Test User"
        Phone = "0123456789"
        Address = "Test Address"
        Role = "Customer"
        DateOfBirth = "1990-01-01"
        Gender = "Male"
        EmergencyContact = "0987654321"
    } | ConvertTo-Json

    $response = Invoke-WebRequest -Uri "$baseUrl/Auth/register" -Method POST -Body $registerData -ContentType "application/json"
    Write-Host "✅ Register: OK" -ForegroundColor Green
    
    # Test login
    $loginData = @{
        Email = "test@example.com"
        Password = "Test123!"
    } | ConvertTo-Json

    $response = Invoke-WebRequest -Uri "$baseUrl/Auth/login" -Method POST -Body $loginData -ContentType "application/json"
    $loginResult = $response.Content | ConvertFrom-Json
    $token = $loginResult.token
    Write-Host "✅ Login: OK" -ForegroundColor Green
    Write-Host "Token: $($token.Substring(0, 50))..." -ForegroundColor Cyan
    
} catch {
    Write-Host "❌ Auth test: FAILED - $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Test Doctor Schedule endpoints (cần token)
Write-Host "`n3. Testing Doctor Schedule endpoints..." -ForegroundColor Yellow
if ($token) {
    try {
        $headers = @{
            "Authorization" = "Bearer $token"
            "Content-Type" = "application/json"
        }
        
        # Test get available slots
        $response = Invoke-WebRequest -Uri "$baseUrl/DoctorSchedule/available-slots/1?date=2025-08-05" -Method GET -Headers $headers
        Write-Host "✅ Get Available Slots: OK" -ForegroundColor Green
        
        # Test get doctor schedule
        $response = Invoke-WebRequest -Uri "$baseUrl/DoctorSchedule/schedule/1" -Method GET -Headers $headers
        Write-Host "✅ Get Doctor Schedule: OK" -ForegroundColor Green
        
    } catch {
        Write-Host "❌ Doctor Schedule test: FAILED - $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "⚠️ Skipping Doctor Schedule test - No token available" -ForegroundColor Yellow
}

# Test 4: Test Appointment endpoints
Write-Host "`n4. Testing Appointment endpoints..." -ForegroundColor Yellow
if ($token) {
    try {
        $headers = @{
            "Authorization" = "Bearer $token"
            "Content-Type" = "application/json"
        }
        
        # Test get available slots (legacy)
        $response = Invoke-WebRequest -Uri "$baseUrl/Appointments/available-slots/1?date=2025-08-05" -Method GET -Headers $headers
        Write-Host "✅ Get Legacy Available Slots: OK" -ForegroundColor Green
        
    } catch {
        Write-Host "❌ Appointment test: FAILED - $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "⚠️ Skipping Appointment test - No token available" -ForegroundColor Yellow
}

# Test 5: Test Blog endpoints
Write-Host "`n5. Testing Blog endpoints..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/Blog/posts" -Method GET
    Write-Host "✅ Get Blog Posts: OK" -ForegroundColor Green
    
    $response = Invoke-WebRequest -Uri "$baseUrl/Blog/categories" -Method GET
    Write-Host "✅ Get Blog Categories: OK" -ForegroundColor Green
    
} catch {
    Write-Host "❌ Blog test: FAILED - $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n=== TEST COMPLETED ===" -ForegroundColor Green
Write-Host "Check the results above for any errors." -ForegroundColor Cyan 