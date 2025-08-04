# Script kiểm tra data trong database
Write-Host "Checking ITMMS Database Data..." -ForegroundColor Green

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection
    $connection.ConnectionString = "Server=localhost;Database=ITMMS_DB;Trusted_Connection=true;TrustServerCertificate=true;"
    $connection.Open()
    
    # Check Users
    Write-Host "`nUsers in database:" -ForegroundColor Yellow
    $command = New-Object System.Data.SqlClient.SqlCommand("SELECT Id, Email, FullName, Role FROM Users", $connection)
    $reader = $command.ExecuteReader()
    while ($reader.Read()) {
        Write-Host "ID: $($reader['Id']) | Email: $($reader['Email']) | Name: $($reader['FullName']) | Role: $($reader['Role'])" -ForegroundColor White
    }
    $reader.Close()
    
    # Check Customers
    Write-Host "`nCustomers in database:" -ForegroundColor Yellow
    $command = New-Object System.Data.SqlClient.SqlCommand("SELECT c.Id, u.Email, u.FullName FROM Customers c JOIN Users u ON c.UserId = u.Id", $connection)
    $reader = $command.ExecuteReader()
    while ($reader.Read()) {
        Write-Host "Customer ID: $($reader['Id']) | Email: $($reader['Email']) | Name: $($reader['FullName'])" -ForegroundColor White
    }
    $reader.Close()
    
    # Check TreatmentPlans
    Write-Host "`nTreatmentPlans in database:" -ForegroundColor Yellow
    $command = New-Object System.Data.SqlClient.SqlCommand("SELECT Id, CustomerId, TreatmentType, Status, Notes FROM TreatmentPlans", $connection)
    $reader = $command.ExecuteReader()
    while ($reader.Read()) {
        Write-Host "ID: $($reader['Id']) | Customer: $($reader['CustomerId']) | Type: $($reader['TreatmentType']) | Status: $($reader['Status'])" -ForegroundColor White
    }
    $reader.Close()
    
    # Check if thanht@gmail.com exists
    Write-Host "`nChecking for thanht@gmail.com:" -ForegroundColor Yellow
    $command = New-Object System.Data.SqlClient.SqlCommand("SELECT Id, Email, FullName FROM Users WHERE Email = 'thanht@gmail.com'", $connection)
    $reader = $command.ExecuteReader()
    if ($reader.Read()) {
        Write-Host "Found user: ID=$($reader['Id']) | Email=$($reader['Email']) | Name=$($reader['FullName'])" -ForegroundColor Green
    } else {
        Write-Host "User thanht@gmail.com not found!" -ForegroundColor Red
    }
    $reader.Close()
    
    # Get customer ID for thanht@gmail.com
    Write-Host "`nGetting customer ID for thanht@gmail.com:" -ForegroundColor Yellow
    $command = New-Object System.Data.SqlClient.SqlCommand("SELECT c.Id as CustomerId, u.Email FROM Customers c JOIN Users u ON c.UserId = u.Id WHERE u.Email = 'thanht@gmail.com'", $connection)
    $reader = $command.ExecuteReader()
    if ($reader.Read()) {
        $customerId = $reader['CustomerId']
        Write-Host "Customer ID for thanht@gmail.com: $customerId" -ForegroundColor Green
    } else {
        Write-Host "Customer record for thanht@gmail.com not found!" -ForegroundColor Red
    }
    $reader.Close()
    
    $connection.Close()
    
    Write-Host "`nDatabase check completed!" -ForegroundColor Green
}
catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
} 