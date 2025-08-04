# 🎉 Hướng dẫn test cuối cùng - Database đã được setup thành công!

## **✅ Tình trạng hiện tại:**
- ✅ Database schema đã được sửa (có đầy đủ columns)
- ✅ User `thanht@gmail.com` đã được tạo (ID: 6)
- ✅ Customer record đã được tạo (Customer ID: 4)
- ✅ Treatment plan đã được tạo cho patient Thanh
- ✅ API đang chạy trên port 5037
- ✅ Health check endpoint hoạt động

## **👤 Tài khoản test:**

### **User thanht@gmail.com:**
- **User ID**: 6
- **Customer ID**: 4
- **Email**: `thanht@gmail.com`
- **Password**: `patient123`
- **Role**: Customer
- **Full Name**: Nguyễn Văn Thanh

## **🧪 Cách test API:**

### **1. Test Login:**
```bash
curl -X POST "http://localhost:5037/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "thanht@gmail.com",
    "password": "patient123"
  }'
```

### **2. Test TreatmentFlow endpoint:**
```bash
# Test với customer ID 4 (patient Thanh)
curl -X GET "http://localhost:5037/api/TreatmentFlow/patient/4" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### **3. Test qua Swagger UI:**
1. **Mở**: `http://localhost:5037/swagger/index.html`
2. **Test login**: `POST /api/Auth/login`
   - Email: `thanht@gmail.com`
   - Password: `patient123`
3. **Copy JWT token** từ response
4. **Click "Authorize"** và nhập: `Bearer YOUR_JWT_TOKEN`
5. **Test TreatmentFlow**: `GET /api/TreatmentFlow/patient/4`

## **📊 Data đã được tạo:**

### **Users (5 records):**
- ID 1: `admin@itmms.com` (Admin)
- ID 2: `doctor1@itmms.com` (Doctor)
- ID 4: `manager@itmms.com` (Manager)
- ID 5: `thanh1@gmail.com` (Customer)
- ID 6: `thanht@gmail.com` (Customer) ← **User test**

### **Customers (2 records):**
- Customer ID 3: `thanh1@gmail.com`
- Customer ID 4: `thanht@gmail.com` ← **Customer test**

### **TreatmentPlans (1 record):**
- ID 1: IVF treatment cho patient Thanh (Customer ID 4)

## **🔍 Expected Response cho TreatmentFlow:**

### **Test với customer ID 4 (patient Thanh):**
```json
{
  "success": true,
  "data": {
    "treatmentPlans": [
      {
        "id": 1,
        "treatmentType": "IVF",
        "description": "IVF treatment for patient Thanh",
        "startDate": "2024-02-01T00:00:00",
        "status": "Active",
        "currentPhase": 1,
        "notes": "New treatment plan",
        "progressNotes": "Starting treatment",
        "paymentStatus": "Pending",
        "totalCost": 50000000,
        "paidAmount": 0,
        "doctor": {
          "id": 1,
          "name": "Dr. Nguyễn Văn A",
          "specialization": "Reproductive Medicine"
        },
        "appointments": [],
        "medicalHistory": [],
        "testResults": []
      }
    ]
  }
}
```

## **🚀 Các endpoint khác để test:**

### **1. Get all users:**
```bash
GET /api/users
```

### **2. Get user profile:**
```bash
GET /api/Auth/profile?id=6
```

### **3. Get customer info:**
```bash
GET /api/customers/4
```

### **4. Get treatment plans:**
```bash
GET /api/treatment-plans
```

## **📋 Checklist test:**

- [ ] ✅ API health check: `http://localhost:5037/api/health`
- [ ] ✅ Login với `thanht@gmail.com` / `patient123`
- [ ] ✅ Lấy JWT token từ login response
- [ ] ✅ Authorize trong Swagger UI với JWT token
- [ ] ✅ Test TreatmentFlow endpoint với customer ID 4
- [ ] ✅ Kiểm tra response có treatment plan data
- [ ] ✅ Test các endpoint khác

## **🎯 Kết quả mong đợi:**

- ✅ **API hoạt động bình thường**
- ✅ **Login thành công với user thanht@gmail.com**
- ✅ **TreatmentFlow endpoint trả về data**
- ✅ **Không còn lỗi "Invalid column name"**
- ✅ **Database có đầy đủ data để test**
- ✅ **Frontend có thể tích hợp dễ dàng**

## **🔧 Nếu gặp vấn đề:**

### **1. Kiểm tra API status:**
```bash
curl http://localhost:5037/api/health
```

### **2. Kiểm tra database:**
```bash
powershell -ExecutionPolicy Bypass -File check_data.ps1
```

### **3. Restart API nếu cần:**
```bash
# Dừng API
taskkill /F /IM SWP391_ITMMS_Api.exe

# Chạy lại
dotnet run
```

### **4. Kiểm tra logs:**
```bash
dotnet run --environment Development
```

## **📝 Lưu ý quan trọng:**

1. **Customer ID**: Sử dụng `4` cho patient Thanh
2. **JWT Token**: Cần lấy từ login response và sử dụng format `Bearer TOKEN`
3. **Authorization**: Phải authorize trước khi test các protected endpoints
4. **Swagger UI**: Cách dễ nhất để test API

## **🚀 Bước tiếp theo:**

1. **Test tất cả các endpoint** trong Swagger UI
2. **Tích hợp với Frontend** sử dụng API
3. **Test các tính năng khác** như appointments, medical records
4. **Deploy lên production** nếu cần

**🎉 Chúc mừng! Database đã được setup thành công và sẵn sàng để test!** 