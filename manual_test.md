# Hướng dẫn Test Manual cho ITMMS API

## 1. Kiểm tra Application Status

### Bước 1: Kiểm tra xem application có đang chạy không
```powershell
Get-Process | Where-Object {$_.ProcessName -like "*dotnet*"}
```

### Bước 2: Kiểm tra port
```powershell
netstat -ano | findstr :5037
```

### Bước 3: Test kết nối cơ bản
```powershell
Invoke-WebRequest -Uri "http://localhost:5037" -Method GET
```

## 2. Test các Endpoints

### Test Swagger UI
```powershell
Invoke-WebRequest -Uri "http://localhost:5037/swagger" -Method GET
```

### Test Blog Posts (không cần auth)
```powershell
Invoke-WebRequest -Uri "http://localhost:5037/api/Blog/posts" -Method GET
```

### Test Blog Categories
```powershell
Invoke-WebRequest -Uri "http://localhost:5037/api/Blog/categories" -Method GET
```

## 3. Test Auth Endpoints

### Register User
```powershell
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

Invoke-WebRequest -Uri "http://localhost:5037/api/Auth/register" -Method POST -Body $registerData -ContentType "application/json"
```

### Login
```powershell
$loginData = @{
    Email = "test@example.com"
    Password = "Test123!"
} | ConvertTo-Json

$response = Invoke-WebRequest -Uri "http://localhost:5037/api/Auth/login" -Method POST -Body $loginData -ContentType "application/json"
$result = $response.Content | ConvertFrom-Json
$token = $result.token
```

## 4. Test Doctor Schedule (cần token)

### Get Available Slots
```powershell
$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

Invoke-WebRequest -Uri "http://localhost:5037/api/DoctorSchedule/available-slots/1?date=2025-08-05" -Method GET -Headers $headers
```

### Get Doctor Schedule
```powershell
Invoke-WebRequest -Uri "http://localhost:5037/api/DoctorSchedule/schedule/1" -Method GET -Headers $headers
```

## 5. Test Appointment Endpoints

### Get Available Slots (Legacy)
```powershell
Invoke-WebRequest -Uri "http://localhost:5037/api/Appointments/available-slots/1?date=2025-08-05" -Method GET -Headers $headers
```

## 6. Test Medical Record

### Get Medical Records
```powershell
Invoke-WebRequest -Uri "http://localhost:5037/api/MedicalRecords/patient/1" -Method GET -Headers $headers
```

## Troubleshooting

### Nếu application không start:
1. Kiểm tra build errors: `dotnet build SWP391_ITMMS_Api.csproj`
2. Kiểm tra database connection
3. Kiểm tra port conflicts
4. Restart application: `dotnet run --project SWP391_ITMMS_Api.csproj`

### Nếu có lỗi 500:
1. Kiểm tra database migration: `dotnet ef database update`
2. Kiểm tra connection string trong appsettings.json
3. Kiểm tra logs trong console

### Nếu có lỗi auth:
1. Kiểm tra JWT configuration
2. Kiểm tra user roles
3. Kiểm tra token expiration 