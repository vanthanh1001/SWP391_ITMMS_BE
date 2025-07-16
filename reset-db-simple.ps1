Write-Host "Resetting database..." -ForegroundColor Yellow

Write-Host "Step 1: Drop database"
dotnet ef database drop --force

Write-Host "Step 2: Remove migrations"
Remove-Item "Migrations\*.cs" -Exclude "AppDbContextModelSnapshot.cs" -Force -ErrorAction SilentlyContinue
Remove-Item "Migrations\*.Designer.cs" -Force -ErrorAction SilentlyContinue

Write-Host "Step 3: Create new migration"
dotnet ef migrations add InitialMigrationFixed

Write-Host "Step 4: Update database"
dotnet ef database update

Write-Host "Database reset complete!" -ForegroundColor Green 