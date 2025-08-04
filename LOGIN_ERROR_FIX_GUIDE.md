# 🔧 Hướng dẫn khắc phục lỗi Login API

## **Vấn đề đã gặp:**
```
POST http://localhost:5037/api/Auth/login
{
  "email": "thanht@gmail.com",
  "password": "123123123"
}
```

**Lỗi nhận được:**
- `Code: 400 Bad Request`
- `"The loginDto field is required"`
- `"'' is invalid within a JSON string"`

## **✅ Giải pháp đã áp dụng:**

### **1. Sửa model LoginDto**
```csharp
// Trước (có lỗi):
public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public string Password { get; set; }
}

// Sau (đã sửa):
public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";
    
    [Required]
    public string Password { get; set; } = "";
}
```

### **2. Các thay đổi khác đã thực hiện:**
- Sửa property `Result` thành `Results` trong `TestResult` model
- Sửa property `ReferenceRange` thành `NormalRange` trong DTO
- Thêm default values cho tất cả string properties

## **🧪 Cách test API login:**

### **1. Test với curl:**
```bash
curl -X POST "http://localhost:5037/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "thanht@gmail.com",
    "password": "123123123"
  }'
```

### **2. Test với Swagger UI:**
1. Mở: `http://localhost:5037/swagger/index.html`
2. Tìm endpoint: `POST /api/Auth/login`
3. Click "Try it out"
4. Nhập JSON:
```json
{
  "email": "thanht@gmail.com",
  "password": "123123123"
}
```
5. Click "Execute"

### **3. Test với Postman:**
```
Method: POST
URL: http://localhost:5037/api/Auth/login
Headers: Content-Type: application/json
Body (raw JSON):
{
  "email": "thanht@gmail.com",
  "password": "123123123"
}
```

### **4. Test với JavaScript:**
```javascript
fetch('http://localhost:5037/api/Auth/login', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    email: 'thanht@gmail.com',
    password: '123123123'
  })
})
.then(response => response.json())
.then(data => console.log(data))
.catch(error => console.error('Error:', error));
```

## **📋 Response mong đợi:**

### **Thành công (200 OK):**
```json
{
  "message": "Đăng nhập thành công",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "username": "thanht",
    "email": "thanht@gmail.com",
    "fullName": "Nguyễn Văn Thanh",
    "phone": "0123456789",
    "role": "Customer",
    "doctor": null
  }
}
```

### **Lỗi (401 Unauthorized):**
```json
{
  "message": "Email hoặc mật khẩu không đúng"
}
```

### **Lỗi validation (400 Bad Request):**
```json
{
  "message": "Dữ liệu không hợp lệ",
  "errors": {
    "Email": ["Email không đúng định dạng"],
    "Password": ["Mật khẩu không được để trống"]
  }
}
```

## **🔍 Các bước debug tiếp theo:**

### **1. Kiểm tra API đang chạy:**
```bash
# Kiểm tra port
netstat -an | findstr :5037

# Test endpoint cơ bản
curl http://localhost:5037/api/health
```

### **2. Kiểm tra database:**
- Đảm bảo user `thanht@gmail.com` tồn tại trong database
- Kiểm tra password đã được hash đúng cách

### **3. Kiểm tra logs:**
```bash
# Xem logs của API
dotnet run --verbosity detailed
```

## **🚀 Các endpoint khác để test:**

### **1. Register user:**
```bash
curl -X POST "http://localhost:5037/api/Auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "Nguyễn Văn Thanh",
    "email": "thanht@gmail.com",
    "phone": "0123456789",
    "address": "Hà Nội",
    "username": "thanht",
    "password": "123123123",
    "confirmPassword": "123123123",
    "role": "Customer"
  }'
```

### **2. Health check:**
```bash
curl http://localhost:5037/api/health
```

### **3. Swagger UI:**
```
http://localhost:5037/swagger/index.html
```

## **📝 Lưu ý quan trọng:**

1. **JSON format**: Phải sử dụng double quotes, không dùng single quotes
2. **Content-Type**: Phải là `application/json`
3. **Email format**: Phải đúng định dạng email
4. **Password**: Không được để trống
5. **CORS**: Đã được cấu hình cho phép tất cả origins

## **🔧 Nếu vẫn gặp lỗi:**

1. **Restart API:**
```bash
# Dừng API
taskkill /F /IM SWP391_ITMMS_Api.exe

# Chạy lại
dotnet run
```

2. **Clear cache:**
```bash
dotnet clean
dotnet build
```

3. **Kiểm tra database connection:**
- Đảm bảo SQL Server đang chạy
- Kiểm tra connection string trong `appsettings.json`

4. **Kiểm tra logs chi tiết:**
```bash
dotnet run --environment Development
``` 