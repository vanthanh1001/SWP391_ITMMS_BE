# Test Unified Auth Controller
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
    $userId = $response.user.id
} catch {
    Write-Host "FAIL - Login Failed" -ForegroundColor Red
    $userId = 2
}

# Test 2: Check Email
Write-Host "`n2. Testing Check Email..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/check-email" -Method POST -Body '"customer@test.com"' -ContentType "application/json"
    Write-Host "PASS - Check Email: exists = $($response.exists)" -ForegroundColor Green
} catch {
    Write-Host "FAIL - Check Email" -ForegroundColor Red
}

# Test 3: Get Profile
Write-Host "`n3. Testing Get Profile..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/profile?id=$userId" -Method GET
    Write-Host "PASS - Get Profile: $($response.fullName)" -ForegroundColor Green
} catch {
    Write-Host "FAIL - Get Profile" -ForegroundColor Red
}

# Test 4: Get History
Write-Host "`n4. Testing History..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/history?userId=$userId" -Method GET
    Write-Host "PASS - Get History: $($response.history.Count) records" -ForegroundColor Green
} catch {
    Write-Host "FAIL - Get History" -ForegroundColor Red
}

Write-Host "`n=== UNIFIED AUTH CONTROLLER TEST COMPLETED ===" -ForegroundColor Green
Write-Host "Success! Only 1 controller for authentication and user management!" -ForegroundColor Magenta 