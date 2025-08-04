# 🎉 Hướng dẫn test với data mẫu đã được tạo

## **✅ Tình trạng hiện tại:**
- ✅ Database schema đã được sửa
- ✅ Sample data đã được thêm vào database
- ✅ API đang chạy trên port 5037
- ✅ Swagger UI có thể truy cập

## **👥 Tài khoản test đã được tạo:**

### **1. Admin Account:**
- **Email**: `admin@itmms.com`
- **Password**: `admin123`
- **Role**: Admin

### **2. Doctor Accounts:**
- **Email**: `doctor1@itmms.com`
- **Password**: `doctor123`
- **Role**: Doctor
- **Specialization**: Reproductive Medicine

- **Email**: `doctor2@itmms.com`
- **Password**: `doctor123`
- **Role**: Doctor
- **Specialization**: Gynecology

### **3. Patient Accounts:**
- **Email**: `thanht@gmail.com` (Patient Thanh - Customer ID: 4)
- **Password**: `patient123`
- **Role**: Customer

- **Email**: `patient1@gmail.com` (Customer ID: 1)
- **Password**: `patient123`
- **Role**: Customer

## **🧪 Cách test API:**

### **1. Test Login:**
```bash
# Test login với patient Thanh
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
3. **Test TreatmentFlow**: `GET /api/TreatmentFlow/patient/{customerId}`

## **📊 Data mẫu đã được tạo:**

### **Users (7 records):**
- 1 Admin
- 2 Doctors
- 4 Patients (bao gồm patient Thanh)

### **TreatmentPlans (4 records):**
- Patient 1: IVF treatment (Active)
- Patient 2: IUI treatment (Active)
- Patient 3: Hormone therapy (Completed)
- Patient 4 (Thanh): IVF treatment (Active)

### **Appointments (5 records):**
- Các lịch hẹn cho từng treatment plan

### **MedicalRecords (4 records):**
- Hồ sơ y tế cho từng patient

### **TestResults (6 records):**
- Kết quả xét nghiệm hormone, tinh trùng, etc.

### **Prescriptions (5 records):**
- Đơn thuốc cho từng medical record

### **Feedbacks (3 records):**
- Đánh giá từ patients

## **🔍 Expected Response cho TreatmentFlow:**

### **Test với customer ID 4 (patient Thanh):**
```json
{
  "success": true,
  "data": {
    "treatmentPlans": [
      {
        "id": 4,
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
          "name": "Bác sĩ Nguyễn Văn A",
          "specialization": "Reproductive Medicine",
          "experienceYears": 10
        },
        "treatmentService": {
          "id": 1,
          "serviceName": "IVF Treatment",
          "serviceCode": "IVF001",
          "basePrice": 50000000,
          "durationDays": 30,
          "successRate": 65.5
        },
        "appointments": [
          {
            "id": 5,
            "appointmentDate": "2024-02-10T09:00:00",
            "timeSlot": "09:00-10:00",
            "type": "Consultation",
            "status": "Scheduled",
            "notes": "Initial consultation for patient Thanh"
          }
        ],
        "medicalHistory": [
          {
            "id": 4,
            "recordDate": "2024-02-10T00:00:00",
            "symptoms": "Difficulty conceiving for 3 years",
            "diagnosis": "Male factor infertility",
            "treatment": "IVF treatment recommended",
            "prescription": "Multivitamins and antioxidants",
            "notes": "Initial assessment completed"
          }
        ],
        "testResults": [
          {
            "id": 6,
            "testName": "FSH Test",
            "testType": "Hormone Test",
            "results": "7.8",
            "normalRange": "3.5-12.5",
            "status": "Normal",
            "testDate": "2024-02-05T00:00:00",
            "unit": "mIU/mL"
          }
        ]
      }
    ]
  }
}
```

## **🚀 Các endpoint khác để test:**

### **1. Get all doctors:**
```bash
GET /api/doctors
```

### **2. Get treatment services:**
```bash
GET /api/treatment-services
```

### **3. Get appointments for patient:**
```bash
GET /api/appointments/patient/4
```

### **4. Get medical records:**
```bash
GET /api/medical-records/patient/4
```

### **5. Get test results:**
```bash
GET /api/test-results/patient/4
```

## **📋 Checklist test:**

- [ ] Login với tài khoản `thanht@gmail.com` / `patient123`
- [ ] Test TreatmentFlow endpoint với customer ID 4
- [ ] Kiểm tra response có đầy đủ data
- [ ] Test các endpoint khác
- [ ] Kiểm tra Swagger UI hoạt động bình thường

## **🎯 Kết quả mong đợi:**

- ✅ **API hoạt động bình thường**
- ✅ **TreatmentFlow endpoint trả về data đầy đủ**
- ✅ **Không còn lỗi "Invalid column name"**
- ✅ **Có data mẫu để test**
- ✅ **Frontend có thể tích hợp dễ dàng**

## **🔧 Nếu gặp vấn đề:**

### **1. Kiểm tra API status:**
```bash
curl http://localhost:5037/api/health
```

### **2. Kiểm tra database:**
```sql
-- Kiểm tra data đã được tạo
SELECT COUNT(*) as UserCount FROM Users;
SELECT COUNT(*) as TreatmentPlanCount FROM TreatmentPlans;
SELECT COUNT(*) as AppointmentCount FROM Appointments;
```

### **3. Restart API nếu cần:**
```bash
# Dừng API
taskkill /F /IM SWP391_ITMMS_Api.exe

# Chạy lại
dotnet run
```

**Chúc mừng! Database đã được setup với data mẫu và sẵn sàng để test! 🎉** 