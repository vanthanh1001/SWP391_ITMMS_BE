# Test Database Connection Script
$baseUrl = "http://localhost:5037"

Write-Host "============ Test Database Connection ============" -ForegroundColor Green

# Step 1: Try to update database
Write-Host "`n1. Updating database with latest migrations..." -ForegroundColor Yellow
try {
    dotnet ef database update
    Write-Host "✓ Database update successful!" -ForegroundColor Green
} catch {
    Write-Host "❌ Database update failed. Checking errors..." -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
}

# Step 2: Start API in background
Write-Host "`n2. Starting API server..." -ForegroundColor Yellow
Start-Process -FilePath "dotnet" -ArgumentList "run" -WindowStyle Minimized
Start-Sleep -Seconds 10

# Step 3: Test Database Health endpoint
Write-Host "`n3. Testing database health endpoint..." -ForegroundColor Yellow
try {
    $health = Invoke-RestMethod -Uri "$baseUrl/api/DatabaseTest/health" -Method GET
    Write-Host "✓ Database Health Check: $($health.message)" -ForegroundColor Green
    Write-Host "  - Users Count: $($health.statistics.usersCount)" -ForegroundColor Cyan
    Write-Host "  - Doctors Count: $($health.statistics.doctorsCount)" -ForegroundColor Cyan
    Write-Host "  - Appointments Count: $($health.statistics.appointmentsCount)" -ForegroundColor Cyan
} catch {
    Write-Host "❌ Database health check failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 4: Test Table Information
Write-Host "`n4. Testing table information..." -ForegroundColor Yellow
try {
    $tables = Invoke-RestMethod -Uri "$baseUrl/api/DatabaseTest/tables" -Method GET
    Write-Host "✓ Table Information Retrieved" -ForegroundColor Green
    foreach ($table in $tables.tables) {
        Write-Host "  - $($table.table): $($table.count) records" -ForegroundColor Cyan
    }
} catch {
    Write-Host "❌ Table information failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 5: Test Basic API Health
Write-Host "`n5. Testing basic API health..." -ForegroundColor Yellow
try {
    $api = Invoke-RestMethod -Uri "$baseUrl/api/health" -Method GET
    Write-Host "✓ API Health: $($api.status)" -ForegroundColor Green
} catch {
    Write-Host "❌ API health failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n============ Test Complete ============" -ForegroundColor Green
Write-Host "If all tests passed, your database connection is working properly!" -ForegroundColor White
Write-Host "If any tests failed, check the error messages above." -ForegroundColor Yellow 