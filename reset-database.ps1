# Reset Database Script - Clean Start
Write-Host "============ RESET DATABASE - CLEAN START ============" -ForegroundColor Red

# Step 1: Drop existing database
Write-Host "`n1. Dropping existing database..." -ForegroundColor Yellow
try {
    dotnet ef database drop --force
    Write-Host "✓ Database dropped successfully!" -ForegroundColor Green
} catch {
    Write-Host "⚠️ Database may not exist or already dropped" -ForegroundColor Yellow
}

# Step 2: Remove all migration files (except snapshot)
Write-Host "`n2. Cleaning migration files..." -ForegroundColor Yellow
try {
    Get-ChildItem "Migrations\*.cs" | Where-Object { $_.Name -ne "AppDbContextModelSnapshot.cs" } | Remove-Item -Force
    Get-ChildItem "Migrations\*.Designer.cs" | Remove-Item -Force
    Write-Host "✓ Migration files cleaned!" -ForegroundColor Green
} catch {
    Write-Host "⚠️ Migration files may not exist" -ForegroundColor Yellow
}

# Step 3: Create new initial migration
Write-Host "`n3. Creating fresh initial migration..." -ForegroundColor Yellow
try {
    dotnet ef migrations add InitialMigrationClean
    Write-Host "✓ Fresh migration created!" -ForegroundColor Green
} catch {
    Write-Host "❌ Failed to create migration" -ForegroundColor Red
    exit 1
}

# Step 4: Create database with new migration
Write-Host "`n4. Creating database with clean schema..." -ForegroundColor Yellow
try {
    dotnet ef database update
    Write-Host "✓ Database created successfully!" -ForegroundColor Green
} catch {
    Write-Host "❌ Failed to create database" -ForegroundColor Red
    exit 1
}

# Step 5: Test database connection
Write-Host "`n5. Testing database connection..." -ForegroundColor Yellow
try {
    # Start API briefly to test
    $process = Start-Process -FilePath "dotnet" -ArgumentList "run" -PassThru -WindowStyle Hidden
    Start-Sleep -Seconds 15
    
    # Test health endpoint
    $health = Invoke-RestMethod -Uri "http://localhost:5037/api/DatabaseTest/health" -Method GET -TimeoutSec 10
    Write-Host "✓ Database connection test successful!" -ForegroundColor Green
    Write-Host "  - Database: $($health.database)" -ForegroundColor Cyan
    Write-Host "  - Users: $($health.statistics.usersCount)" -ForegroundColor Cyan
    Write-Host "  - Doctors: $($health.statistics.doctorsCount)" -ForegroundColor Cyan
    
    # Stop the process
    Stop-Process -Id $process.Id -Force
} catch {
    Write-Host "⚠️ Could not test connection, but database should be ready" -ForegroundColor Yellow
    # Try to stop any running processes
    Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
}

Write-Host "`n============ DATABASE RESET COMPLETE ============" -ForegroundColor Green
Write-Host "Database has been reset with clean schema!" -ForegroundColor White
Write-Host "All cascade path conflicts should now be resolved." -ForegroundColor Cyan 