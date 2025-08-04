Write-Host "=== SIMPLE API TEST ===" -ForegroundColor Green

# Test 1: Kiểm tra xem server có chạy không
Write-Host "`n1. Testing server connection..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5037" -Method GET -TimeoutSec 5
    Write-Host "✅ Server is running" -ForegroundColor Green
} catch {
    Write-Host "❌ Server is not running: $($_.Exception.Message)" -ForegroundColor Red
    exit
}

# Test 2: Test Swagger
Write-Host "`n2. Testing Swagger..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5037/swagger" -Method GET -TimeoutSec 5
    Write-Host "✅ Swagger UI is accessible" -ForegroundColor Green
} catch {
    Write-Host "❌ Swagger failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Test một endpoint đơn giản
Write-Host "`n3. Testing simple endpoint..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5037/api/Blog/posts" -Method GET -TimeoutSec 5
    Write-Host "✅ Blog endpoint is working" -ForegroundColor Green
    Write-Host "Response: $($response.StatusCode)" -ForegroundColor Cyan
} catch {
    Write-Host "❌ Blog endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n=== TEST COMPLETED ===" -ForegroundColor Green 