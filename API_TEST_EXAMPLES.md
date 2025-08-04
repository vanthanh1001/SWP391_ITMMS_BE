# 🔧 API Test Examples - Debug Login Issue

## **Lỗi hiện tại:**
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

## **Các cách test khác nhau:**

### **1. Test với curl (đúng format):**
```bash
curl -X POST "http://localhost:5037/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "thanht@gmail.com",
    "password": "123123123"
  }'
```

### **2. Test với Postman:**
```json
POST http://localhost:5037/api/Auth/login
Headers:
  Content-Type: application/json

Body (raw JSON):
{
  "email": "thanht@gmail.com",
  "password": "123123123"
}
```

### **3. Test với JavaScript fetch:**
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

### **4. Test với Swagger UI:**
Trong Swagger UI, sử dụng:
```json
{
  "email": "thanht@gmail.com",
  "password": "123123123"
}
```

## **Các bước debug:**

### **Bước 1: Kiểm tra API đang chạy**
```bash
# Kiểm tra port đang chạy
netstat -an | findstr :5037
```

### **Bước 2: Test endpoint cơ bản**
```bash
curl http://localhost:5037/api/health
```

### **Bước 3: Kiểm tra CORS**
```bash
curl -X OPTIONS "http://localhost:5037/api/Auth/login" \
  -H "Origin: http://localhost:3000" \
  -H "Access-Control-Request-Method: POST" \
  -H "Access-Control-Request-Headers: Content-Type"
```

### **Bước 4: Test với dữ liệu đơn giản**
```json
{
  "email": "test@test.com",
  "password": "test123"
}
```

## **Các lỗi có thể gặp:**

### **1. Lỗi JSON format:**
- Đảm bảo JSON được escape đúng cách
- Không có ký tự đặc biệt trong email
- Sử dụng double quotes

### **2. Lỗi Content-Type:**
- Phải có header: `Content-Type: application/json`
- Không sử dụng `application/x-www-form-urlencoded`

### **3. Lỗi CORS:**
- Kiểm tra CORS configuration trong Program.cs
- Đảm bảo origin được cho phép

### **4. Lỗi Model Binding:**
- Kiểm tra tên properties trong JSON có khớp với model không
- Đảm bảo không có extra fields

## **Giải pháp:**

### **1. Sửa JSON format:**
```json
{
  "email": "thanht@gmail.com",
  "password": "123123123"
}
```

### **2. Kiểm tra Program.cs CORS:**
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

app.UseCors("AllowAll");
```

### **3. Test với user có sẵn:**
```json
{
  "email": "admin@itmms.com",
  "password": "admin123"
}
```

## **Expected Response:**
```json
{
  "success": true,
  "message": "Đăng nhập thành công",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": 1,
      "fullName": "Admin User",
      "email": "admin@itmms.com",
      "role": "Admin"
    }
  }
}
``` 