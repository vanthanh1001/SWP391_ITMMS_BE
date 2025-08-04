# 🔧 Hướng dẫn sửa lỗi Database Schema

## **Vấn đề hiện tại:**
```
Error: Invalid column name 'Notes'
Invalid column name 'Notes'
```

**Nguyên nhân:** Database schema không khớp với Entity Framework models. Các column như `Notes`, `ProgressNotes`, `Unit`, `DoctorName` chưa được tạo trong database.

## **✅ Giải pháp:**

### **Bước 1: Chạy script SQL để sửa database**

1. **Mở SQL Server Management Studio (SSMS)**
2. **Kết nối đến database ITMMS**
3. **Chạy script `Database_Fix_Script.sql`**

Hoặc sử dụng command line:
```bash
sqlcmd -S localhost -d ITMMS_DB -i Database_Fix_Script.sql
```

### **Bước 2: Kiểm tra connection string**

Mở file `appsettings.json` và kiểm tra connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ITMMS_DB;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### **Bước 3: Tạo migration mới (nếu cần)**

```bash
# Tạo migration mới
dotnet ef migrations add FixDatabaseSchema

# Apply migration
dotnet ef database update
```

### **Bước 4: Restart API**

```bash
# Dừng API hiện tại
taskkill /F /IM SWP391_ITMMS_Api.exe

# Chạy lại API
dotnet run
```

## **🧪 Test API sau khi sửa:**

### **1. Test TreatmentFlow endpoint:**
```bash
curl -X GET "http://localhost:5037/api/TreatmentFlow/patient/4" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### **2. Test với Swagger UI:**
1. Mở: `http://localhost:5037/swagger/index.html`
2. Tìm endpoint: `GET /api/TreatmentFlow/patient/{customerId}`
3. Nhập `customerId = 4`
4. Click "Execute"

### **3. Expected Response:**
```json
{
  "success": true,
  "data": {
    "treatmentPlans": [
      {
        "id": 1,
        "treatmentType": "IVF",
        "description": "In vitro fertilization treatment",
        "startDate": "2024-01-15T00:00:00",
        "status": "Active",
        "currentPhase": 2,
        "notes": "Patient responding well to treatment",
        "progressNotes": "Phase 2 completed successfully",
        "doctor": {
          "id": 1,
          "name": "Dr. Nguyen Van A",
          "specialization": "Reproductive Medicine"
        },
        "appointments": [...],
        "medicalHistory": [...],
        "testResults": [...]
      }
    ]
  }
}
```

## **🔍 Các column đã được thêm:**

### **TreatmentPlans table:**
- `Notes` (NVARCHAR(500))
- `ProgressNotes` (NVARCHAR(1000))
- `PaymentStatus` (NVARCHAR(20))
- `TreatmentServiceId` (INT)

### **Appointments table:**
- `Notes` (NVARCHAR(500))
- `TreatmentPlanId` (INT)

### **MedicalRecords table:**
- `Notes` (NVARCHAR(1000))
- `AppointmentId` (INT)

### **TestResults table:**
- `Unit` (NVARCHAR(20))
- `DoctorName` (NVARCHAR(100))

## **🚨 Nếu vẫn gặp lỗi:**

### **1. Kiểm tra database connection:**
```bash
# Test connection
sqlcmd -S localhost -d ITMMS_DB -Q "SELECT 1"
```

### **2. Kiểm tra table structure:**
```sql
-- Kiểm tra cấu trúc bảng TreatmentPlans
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'TreatmentPlans'
ORDER BY ORDINAL_POSITION;
```

### **3. Kiểm tra logs API:**
```bash
dotnet run --environment Development
```

### **4. Reset database (nếu cần):**
```bash
# Xóa tất cả migrations
dotnet ef database drop --force

# Tạo migration mới
dotnet ef migrations add InitialCreate

# Apply migration
dotnet ef database update
```

## **📋 Các bước tiếp theo:**

1. **Chạy script SQL** để sửa database schema
2. **Restart API** để áp dụng thay đổi
3. **Test endpoint** `/api/TreatmentFlow/patient/{customerId}`
4. **Kiểm tra response** có đúng format không
5. **Test các endpoint khác** nếu cần

## **✅ Kết quả mong đợi:**

- ✅ API không còn lỗi "Invalid column name"
- ✅ TreatmentFlow endpoint trả về dữ liệu đúng
- ✅ Tất cả các endpoint khác hoạt động bình thường
- ✅ Database schema khớp với Entity Framework models 