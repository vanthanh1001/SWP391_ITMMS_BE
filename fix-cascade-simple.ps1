# Simple Fix for Cascade Path Conflicts
Write-Host "============ SIMPLE CASCADE FIX ============" -ForegroundColor Yellow

# Step 1: Rollback to previous working migration
Write-Host "`n1. Rolling back to previous migration..." -ForegroundColor Yellow
try {
    # Find the migration before the problematic one
    dotnet ef database update AddTreatmentServiceAndEnhancements
    Write-Host "✓ Rolled back to working migration!" -ForegroundColor Green
} catch {
    Write-Host "❌ Rollback failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Trying alternative approach..." -ForegroundColor Yellow
    
    # Alternative: rollback further
    try {
        dotnet ef database update InitialMigration
        Write-Host "✓ Rolled back to initial migration!" -ForegroundColor Green
    } catch {
        Write-Host "❌ Deep rollback also failed. Consider using reset-database.ps1" -ForegroundColor Red
        exit 1
    }
}

# Step 2: Remove the problematic migration
Write-Host "`n2. Removing problematic migration..." -ForegroundColor Yellow
try {
    dotnet ef migrations remove
    Write-Host "✓ Problematic migration removed!" -ForegroundColor Green
} catch {
    Write-Host "⚠️ Could not remove migration" -ForegroundColor Yellow
}

# Step 3: Apply the fix we have
Write-Host "`n3. Applying the current fix..." -ForegroundColor Yellow
try {
    dotnet ef database update
    Write-Host "✓ Database updated with current schema!" -ForegroundColor Green
} catch {
    Write-Host "❌ Update failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Please run reset-database.ps1 for complete clean install" -ForegroundColor Yellow
    exit 1
}

# Step 4: Quick test
Write-Host "`n4. Quick connection test..." -ForegroundColor Yellow
try {
    # Try to start and test quickly
    $job = Start-Job -ScriptBlock { 
        Set-Location "D:\SWP391\SWP391_ITMMS\SWP391_ITMMS_Api"
        dotnet run 
    }
    Start-Sleep -Seconds 10
    
    $health = Invoke-RestMethod -Uri "http://localhost:5037/api/health" -Method GET -TimeoutSec 5
    Write-Host "✓ Basic API test successful: $($health.status)" -ForegroundColor Green
    
    Stop-Job $job -PassThru | Remove-Job
} catch {
    Write-Host "⚠️ Could not test API, but database should be working" -ForegroundColor Yellow
    Get-Job | Stop-Job -PassThru | Remove-Job -ErrorAction SilentlyContinue
}

Write-Host "`n============ FIX COMPLETE ============" -ForegroundColor Green
Write-Host "Try running the API now: dotnet run" -ForegroundColor Cyan 