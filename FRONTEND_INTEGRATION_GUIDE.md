# 🏥 ITMMS Frontend Integration Guide
## Hướng dẫn tích hợp Frontend với API Hệ thống Quản lý Điều trị Hiếm muộn

---

## 📋 **Tổng quan hệ thống**

ITMMS (Infertility Treatment Management and Monitoring System) là hệ thống quản lý và theo dõi quá trình điều trị hiếm muộn với 10 tính năng chính:

1. **Homepage** - Trang chủ giới thiệu
2. **Service Registration** - Đăng ký dịch vụ điều trị
3. **Treatment Process Management** - Quản lý quá trình điều trị
4. **Reminders and Scheduling** - Nhắc nhở và lịch trình
5. **Medical Record Keeping** - Ghi nhận hồ sơ y tế
6. **Service and Pricing Declaration** - Khai báo dịch vụ và giá
7. **Doctor Information Management** - Quản lý thông tin bác sĩ
8. **Rating and Feedback** - Đánh giá và phản hồi
9. **User Profile and Order History** - Hồ sơ người dùng và lịch sử
10. **Dashboard & Report** - Bảng điều khiển và báo cáo

---

## 🔧 **Cấu hình API**

### **Base URL**
```
Development: https://localhost:7139/api
Production: https://your-domain.com/api
```

### **Authentication**
Hệ thống sử dụng JWT Bearer Token:
```javascript
Authorization: Bearer <your-jwt-token>
```

### **Response Format**
```json
{
  "success": true,
  "message": "Thành công",
  "data": { ... }
}
```

---

## 🚀 **API Endpoints Chi tiết**

### **1. Authentication (Xác thực)**

#### **Đăng nhập**
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Đăng nhập thành công",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": 1,
      "fullName": "Nguyễn Văn A",
      "email": "user@example.com",
      "role": "Customer"
    }
  }
}
```

#### **Đăng ký**
```http
POST /api/auth/register
Content-Type: application/json

{
  "fullName": "Nguyễn Văn A",
  "email": "user@example.com",
  "phone": "0123456789",
  "address": "Hà Nội",
  "username": "user123",
  "password": "password123",
  "confirmPassword": "password123",
  "role": "Customer"
}
```

---

### **2. Homepage (Trang chủ)**

#### **Lấy thống kê tổng quan**
```http
GET /api/dashboard/stats
Authorization: Bearer <token>
```

**Response:**
```json
{
  "success": true,
  "data": {
    "totalCustomers": 150,
    "totalDoctors": 12,
    "totalAppointments": 89,
    "completedTreatments": 67,
    "totalRevenue": 2500000000,
    "averageRating": 4.5,
    "activeTreatmentPlans": 23,
    "pendingAppointments": 15
  }
}
```

#### **Lấy danh sách dịch vụ**
```http
GET /api/treatment-services
Authorization: Bearer <token>
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "serviceName": "IVF - Thụ tinh trong ống nghiệm",
      "serviceCode": "IVF-001",
      "description": "Phương pháp điều trị hiếm muộn tiên tiến",
      "basePrice": 50000000,
      "durationDays": 30,
      "successRate": 65.5,
      "procedures": "6 giai đoạn điều trị",
      "requirements": "Khám sức khỏe tổng quát"
    },
    {
      "id": 2,
      "serviceName": "IUI - Thụ tinh nhân tạo",
      "serviceCode": "IUI-001",
      "description": "Phương pháp thụ tinh nhân tạo",
      "basePrice": 15000000,
      "durationDays": 14,
      "successRate": 25.0,
      "procedures": "4 giai đoạn điều trị",
      "requirements": "Khám sức khỏe cơ bản"
    }
  ]
}
```

#### **Lấy blog chia sẻ kinh nghiệm**
```http
GET /api/blog
Authorization: Bearer <token>
```

---

### **3. Service Registration (Đăng ký dịch vụ)**

#### **Tạo kế hoạch điều trị**
```http
POST /api/treatment-plans
Authorization: Bearer <token>
Content-Type: application/json

{
  "customerId": 1,
  "doctorId": 2,
  "treatmentServiceId": 1,
  "treatmentType": "IVF",
  "description": "Điều trị IVF cho cặp vợ chồng",
  "startDate": "2024-01-15T00:00:00",
  "totalCost": 50000000,
  "phaseDescription": "Bắt đầu giai đoạn 1: Khám sức khỏe",
  "nextPhaseDate": "2024-01-20T00:00:00",
  "notes": "Bệnh nhân cần chuẩn bị tinh thần"
}
```

#### **Lấy danh sách bác sĩ**
```http
GET /api/doctors
Authorization: Bearer <token>
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "Dr. Nguyễn Thị B",
      "specialization": "Sản phụ khoa - Hiếm muộn",
      "experienceYears": 15,
      "education": "Tiến sĩ Y khoa",
      "description": "Chuyên gia điều trị hiếm muộn với 15 năm kinh nghiệm",
      "isAvailable": true,
      "phone": "0123456789",
      "email": "doctor@example.com"
    }
  ]
}
```

---

### **4. Treatment Process Management (Quản lý quá trình điều trị)**

#### **Xem quy trình điều trị của bệnh nhân**
```http
GET /api/TreatmentFlow/patient/{customerId}
Authorization: Bearer <token>
```

**Response:**
```json
{
  "success": true,
  "data": {
    "treatmentPlan": {
      "id": 1,
      "treatmentType": "IVF",
      "description": "Điều trị IVF cho cặp vợ chồng",
      "startDate": "2024-01-15T00:00:00",
      "status": "Active",
      "currentPhase": 2,
      "phaseDescription": "Kích thích buồng trứng",
      "nextPhaseDate": "2024-01-20T00:00:00",
      "nextVisitDate": "2024-01-18T09:00:00",
      "totalCost": 50000000,
      "paidAmount": 20000000,
      "paymentStatus": "Partial",
      "progressNotes": "Bắt đầu tiêm thuốc kích thích buồng trứng",
      "doctor": {
        "id": 2,
        "name": "Dr. Nguyễn Thị B",
        "specialization": "Sản phụ khoa - Hiếm muộn",
        "experienceYears": 15
      }
    },
    "appointments": [
      {
        "id": 1,
        "appointmentDate": "2024-01-18T09:00:00",
        "timeSlot": "09:00-10:00",
        "type": "Consultation",
        "status": "Scheduled",
        "notes": "Khám định kỳ",
        "medicalRecord": {
          "id": 1,
          "recordDate": "2024-01-15T00:00:00",
          "symptoms": "Khó thụ thai trong 2 năm",
          "diagnosis": "Hiếm muộn không rõ nguyên nhân",
          "treatment": "Điều trị IVF",
          "prescription": "Thuốc kích thích buồng trứng"
        }
      }
    ],
    "medicalHistory": [
      {
        "id": 1,
        "recordDate": "2024-01-15T00:00:00",
        "doctorName": "Dr. Nguyễn Thị B",
        "symptoms": "Khó thụ thai trong 2 năm",
        "diagnosis": "Hiếm muộn không rõ nguyên nhân",
        "treatment": "Điều trị IVF",
        "prescription": "Thuốc kích thích buồng trứng"
      }
    ],
    "testResults": [
      {
        "id": 1,
        "testName": "Xét nghiệm hormone FSH",
        "testType": "Blood Test",
        "results": "8.5 mIU/mL",
        "normalRange": "3.5-12.5 mIU/mL",
        "status": "Normal",
        "testDate": "2024-01-15T00:00:00"
      }
    ]
  }
}
```

#### **Cập nhật giai đoạn điều trị (Bác sĩ)**
```http
PUT /api/TreatmentFlow/treatment-plan/{id}/phase
Authorization: Bearer <token>
Content-Type: application/json

{
  "currentPhase": 3,
  "phaseDescription": "Chọc hút trứng",
  "nextPhaseDate": "2024-01-25T00:00:00",
  "nextVisitDate": "2024-01-23T08:00:00",
  "progressNotes": "Hoàn thành giai đoạn kích thích, chuẩn bị chọc hút trứng",
  "notes": "Bệnh nhân cần nhịn ăn từ 22h tối hôm trước",
  "status": "Active"
}
```

#### **Xem timeline điều trị**
```http
GET /api/TreatmentFlow/timeline/{customerId}
Authorization: Bearer <token>
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "date": "2024-01-15T00:00:00",
      "type": "TreatmentPlan",
      "title": "Bắt đầu điều trị IVF",
      "description": "Khám sức khỏe tổng quát và tư vấn điều trị",
      "status": "Completed"
    },
    {
      "date": "2024-01-16T08:00:00",
      "type": "Appointment",
      "title": "Khám định kỳ",
      "description": "Khám và siêu âm buồng trứng",
      "status": "Scheduled"
    },
    {
      "date": "2024-01-17T00:00:00",
      "type": "TestResult",
      "title": "Kết quả xét nghiệm hormone",
      "description": "FSH: 8.5 mIU/mL (Bình thường)",
      "status": "Normal"
    }
  ]
}
```

#### **Xem bước tiếp theo**
```http
GET /api/TreatmentFlow/next-steps/{treatmentPlanId}
Authorization: Bearer <token>
```

**Response:**
```json
{
  "success": true,
  "data": {
    "currentPhase": 2,
    "phaseDescription": "Kích thích buồng trứng",
    "nextSteps": [
      {
        "step": 3,
        "title": "Chọc hút trứng",
        "description": "Thủ thuật chọc hút trứng",
        "duration": "1 ngày"
      },
      {
        "step": 4,
        "title": "Thụ tinh trong ống nghiệm",
        "description": "Thụ tinh trứng với tinh trùng",
        "duration": "3-5 ngày"
      }
    ],
    "estimatedCompletion": "2024-01-25T00:00:00"
  }
}
```

---

### **5. Reminders and Scheduling (Nhắc nhở và lịch trình)**

#### **Lấy lịch nhắc nhở**
```http
GET /api/TreatmentFlow/reminders/{customerId}
Authorization: Bearer <token>
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "treatmentPlan": {
        "id": 1,
        "treatmentType": "IVF",
        "currentPhase": 2,
        "phaseDescription": "Kích thích buồng trứng",
        "nextPhaseDate": "2024-01-20T00:00:00",
        "nextVisitDate": "2024-01-18T09:00:00",
        "progressNotes": "Bắt đầu tiêm thuốc kích thích"
      },
      "upcomingAppointments": [
        {
          "id": 1,
          "appointmentDate": "2024-01-18T09:00:00",
          "timeSlot": "09:00-10:00",
          "type": "Consultation",
          "notes": "Khám định kỳ"
        }
      ],
      "phaseReminders": {
        "nextPhaseDate": "2024-01-20T00:00:00",
        "nextVisitDate": "2024-01-18T09:00:00",
        "currentPhase": 2,
        "phaseDescription": "Kích thích buồng trứng"
      }
    }
  ]
}
```

#### **Lấy lịch hẹn của bệnh nhân**
```http
GET /api/appointments/customer/{customerId}
Authorization: Bearer <token>
```

#### **Tạo lịch hẹn mới**
```http
POST /api/appointments
Authorization: Bearer <token>
Content-Type: application/json

{
  "customerId": 1,
  "doctorId": 2,
  "treatmentPlanId": 1,
  "appointmentDate": "2024-01-18T09:00:00",
  "timeSlot": "09:00-10:00",
  "type": "Consultation",
  "notes": "Khám định kỳ"
}
```

#### **Xem khung giờ trống**
```http
GET /api/appointments/available-slots?doctorId=2&date=2024-01-18
Authorization: Bearer <token>
```

---

### **6. Medical Record Keeping (Ghi nhận hồ sơ y tế)**

#### **Tạo hồ sơ y tế (Bác sĩ)**
```http
POST /api/TreatmentFlow/medical-record
Authorization: Bearer <token>
Content-Type: application/json

{
  "customerId": 1,
  "doctorId": 2,
  "appointmentId": 1,
  "symptoms": "Khó thụ thai trong 2 năm",
  "diagnosis": "Hiếm muộn không rõ nguyên nhân",
  "treatment": "Điều trị IVF",
  "prescription": "Thuốc kích thích buồng trứng",
  "notes": "Bệnh nhân cần theo dõi chặt chẽ"
}
```

#### **Ghi nhận kết quả xét nghiệm (Bác sĩ)**
```http
POST /api/TreatmentFlow/test-result
Authorization: Bearer <token>
Content-Type: application/json

{
  "customerId": 1,
  "doctorId": 2,
  "testName": "Xét nghiệm hormone FSH",
  "testType": "Blood Test",
  "results": "8.5 mIU/mL",
  "normalRange": "3.5-12.5 mIU/mL",
  "status": "Normal"
}
```

---

### **7. Service and Pricing Declaration (Khai báo dịch vụ và giá)**

#### **Lấy danh sách dịch vụ**
```http
GET /api/treatment-services
Authorization: Bearer <token>
```

#### **Tạo dịch vụ mới (Admin)**
```http
POST /api/treatment-services
Authorization: Bearer <token>
Content-Type: application/json

{
  "serviceName": "IVF - Thụ tinh trong ống nghiệm",
  "serviceCode": "IVF-001",
  "description": "Phương pháp điều trị hiếm muộn tiên tiến",
  "basePrice": 50000000,
  "durationDays": 30,
  "successRate": 65.5,
  "procedures": "6 giai đoạn điều trị: Khám sức khỏe, Kích thích buồng trứng, Chọc hút trứng, Thụ tinh, Chuyển phôi, Theo dõi thai",
  "requirements": "Khám sức khỏe tổng quát, Xét nghiệm hormone, Siêu âm buồng trứng"
}
```

---

### **8. Doctor Information Management (Quản lý thông tin bác sĩ)**

#### **Lấy danh sách bác sĩ**
```http
GET /api/doctors
Authorization: Bearer <token>
```

#### **Tìm kiếm bác sĩ**
```http
GET /api/doctors/search?specialization=IVF&experienceYears=10
Authorization: Bearer <token>
```

#### **Xem lịch trình bác sĩ**
```http
GET /api/doctors/{id}/schedule
Authorization: Bearer <token>
```

#### **Cập nhật thông tin bác sĩ**
```http
PUT /api/doctors/{id}
Authorization: Bearer <token>
Content-Type: application/json

{
  "specialization": "Sản phụ khoa - Hiếm muộn",
  "experienceYears": 15,
  "description": "Chuyên gia điều trị hiếm muộn với 15 năm kinh nghiệm",
  "isAvailable": true
}
```

---

### **9. Rating and Feedback (Đánh giá và phản hồi)**

#### **Tạo đánh giá**
```http
POST /api/feedback
Authorization: Bearer <token>
Content-Type: application/json

{
  "customerId": 1,
  "doctorId": 2,
  "appointmentId": 1,
  "rating": 5,
  "comment": "Bác sĩ rất tận tâm và chuyên nghiệp, giải thích rõ ràng về quá trình điều trị"
}
```

#### **Xem đánh giá của bác sĩ**
```http
GET /api/doctors/{id}/feedback
Authorization: Bearer <token>
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "rating": 5,
      "comment": "Bác sĩ rất tận tâm và chuyên nghiệp",
      "createdAt": "2024-01-15T00:00:00",
      "customerName": "Nguyễn Văn A"
    }
  ]
}
```

---

### **10. User Profile and Order History (Hồ sơ người dùng và lịch sử)**

#### **Xem hồ sơ người dùng**
```http
GET /api/users/profile
Authorization: Bearer <token>
```

#### **Cập nhật hồ sơ**
```http
PUT /api/users/profile
Authorization: Bearer <token>
Content-Type: application/json

{
  "fullName": "Nguyễn Văn A",
  "phone": "0123456789",
  "address": "Hà Nội"
}
```

#### **Xem thống kê tiến độ**
```http
GET /api/TreatmentFlow/progress-stats/{customerId}
Authorization: Bearer <token>
```

**Response:**
```json
{
  "success": true,
  "data": {
    "stats": [
      {
        "status": "Active",
        "count": 1,
        "totalCost": 50000000,
        "paidAmount": 20000000
      },
      {
        "status": "Completed",
        "count": 2,
        "totalCost": 80000000,
        "paidAmount": 80000000
      }
    ],
    "activePlan": {
      "id": 1,
      "treatmentType": "IVF",
      "currentPhase": 2,
      "phaseDescription": "Kích thích buồng trứng",
      "startDate": "2024-01-15T00:00:00",
      "daysInTreatment": 3,
      "progressPercentage": 33.3
    }
  }
}
```

---

### **11. Dashboard & Report (Bảng điều khiển và báo cáo)**

#### **Dashboard tổng quan**
```http
GET /api/dashboard/stats
Authorization: Bearer <token>
```

#### **Báo cáo theo tháng**
```http
GET /api/dashboard/monthly-report?year=2024&month=1
Authorization: Bearer <token>
```

**Response:**
```json
{
  "success": true,
  "data": {
    "month": 1,
    "year": 2024,
    "newCustomers": 25,
    "completedAppointments": 89,
    "revenue": 250000000,
    "treatmentPlansStarted": 15,
    "treatmentPlansCompleted": 8
  }
}
```

---

## 💻 **Frontend Integration Examples**

### **1. API Client Setup**

```javascript
// apiClient.js
const API_BASE = 'https://localhost:7139/api';

class ApiClient {
  constructor() {
    this.baseURL = API_BASE;
  }

  getToken() {
    return localStorage.getItem('token');
  }

  async request(endpoint, options = {}) {
    const token = this.getToken();
    const url = `${this.baseURL}${endpoint}`;
    
    const config = {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...(token && { 'Authorization': `Bearer ${token}` }),
        ...options.headers
      }
    };

    try {
      const response = await fetch(url, config);
      const data = await response.json();
      
      if (!response.ok) {
        throw new Error(data.message || 'Có lỗi xảy ra');
      }
      
      return data;
    } catch (error) {
      console.error('API Error:', error);
      throw error;
    }
  }

  // Authentication
  async login(email, password) {
    return this.request('/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password })
    });
  }

  async register(userData) {
    return this.request('/auth/register', {
      method: 'POST',
      body: JSON.stringify(userData)
    });
  }

  // Treatment Flow
  async getPatientTreatmentFlow(customerId) {
    return this.request(`/TreatmentFlow/patient/${customerId}`);
  }

  async updateTreatmentPhase(treatmentPlanId, phaseData) {
    return this.request(`/TreatmentFlow/treatment-plan/${treatmentPlanId}/phase`, {
      method: 'PUT',
      body: JSON.stringify(phaseData)
    });
  }

  async getTreatmentTimeline(customerId) {
    return this.request(`/TreatmentFlow/timeline/${customerId}`);
  }

  async getTreatmentReminders(customerId) {
    return this.request(`/TreatmentFlow/reminders/${customerId}`);
  }

  // Appointments
  async getAppointments(customerId) {
    return this.request(`/appointments/customer/${customerId}`);
  }

  async createAppointment(appointmentData) {
    return this.request('/appointments', {
      method: 'POST',
      body: JSON.stringify(appointmentData)
    });
  }

  // Doctors
  async getDoctors() {
    return this.request('/doctors');
  }

  async getDoctorById(doctorId) {
    return this.request(`/doctors/${doctorId}`);
  }

  // Services
  async getTreatmentServices() {
    return this.request('/treatment-services');
  }

  // Dashboard
  async getDashboardStats() {
    return this.request('/dashboard/stats');
  }
}

export default new ApiClient();
```

### **2. React Components**

#### **Authentication Component**
```jsx
// LoginForm.jsx
import React, { useState } from 'react';
import apiClient from '../services/apiClient';

const LoginForm = ({ onLoginSuccess }) => {
  const [formData, setFormData] = useState({
    email: '',
    password: ''
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const response = await apiClient.login(formData.email, formData.password);
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('user', JSON.stringify(response.data.user));
      onLoginSuccess(response.data.user);
    } catch (error) {
      setError(error.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="login-form">
      <h2>Đăng nhập</h2>
      {error && <div className="error">{error}</div>}
      
      <div className="form-group">
        <label>Email:</label>
        <input
          type="email"
          value={formData.email}
          onChange={(e) => setFormData({...formData, email: e.target.value})}
          required
        />
      </div>

      <div className="form-group">
        <label>Mật khẩu:</label>
        <input
          type="password"
          value={formData.password}
          onChange={(e) => setFormData({...formData, password: e.target.value})}
          required
        />
      </div>

      <button type="submit" disabled={loading}>
        {loading ? 'Đang đăng nhập...' : 'Đăng nhập'}
      </button>
    </form>
  );
};

export default LoginForm;
```

#### **Treatment Plan Component**
```jsx
// TreatmentPlan.jsx
import React, { useState, useEffect } from 'react';
import apiClient from '../services/apiClient';

const TreatmentPlan = ({ customerId }) => {
  const [treatmentFlow, setTreatmentFlow] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchTreatmentFlow();
  }, [customerId]);

  const fetchTreatmentFlow = async () => {
    try {
      setLoading(true);
      const response = await apiClient.getPatientTreatmentFlow(customerId);
      setTreatmentFlow(response.data);
    } catch (error) {
      setError(error.message);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <div>Đang tải...</div>;
  if (error) return <div className="error">{error}</div>;
  if (!treatmentFlow) return <div>Không có dữ liệu</div>;

  return (
    <div className="treatment-plan">
      <h2>Quy trình điều trị</h2>
      
      <div className="plan-overview">
        <h3>Thông tin kế hoạch</h3>
        <div className="plan-details">
          <p><strong>Loại điều trị:</strong> {treatmentFlow.treatmentPlan.treatmentType}</p>
          <p><strong>Giai đoạn hiện tại:</strong> {treatmentFlow.treatmentPlan.currentPhase}</p>
          <p><strong>Mô tả giai đoạn:</strong> {treatmentFlow.treatmentPlan.phaseDescription}</p>
          <p><strong>Ngày tiếp theo:</strong> {new Date(treatmentFlow.treatmentPlan.nextPhaseDate).toLocaleDateString('vi-VN')}</p>
          <p><strong>Trạng thái:</strong> {treatmentFlow.treatmentPlan.status}</p>
        </div>
      </div>

      <div className="appointments">
        <h3>Lịch hẹn</h3>
        {treatmentFlow.appointments.map(appointment => (
          <div key={appointment.id} className="appointment-card">
            <h4>{appointment.type}</h4>
            <p>Ngày: {new Date(appointment.appointmentDate).toLocaleDateString('vi-VN')}</p>
            <p>Giờ: {appointment.timeSlot}</p>
            <p>Trạng thái: {appointment.status}</p>
            {appointment.notes && <p>Ghi chú: {appointment.notes}</p>}
          </div>
        ))}
      </div>

      <div className="medical-history">
        <h3>Lịch sử y tế</h3>
        {treatmentFlow.medicalHistory.map(record => (
          <div key={record.id} className="medical-record">
            <h4>Ngày: {new Date(record.recordDate).toLocaleDateString('vi-VN')}</h4>
            <p><strong>Bác sĩ:</strong> {record.doctorName}</p>
            <p><strong>Triệu chứng:</strong> {record.symptoms}</p>
            <p><strong>Chẩn đoán:</strong> {record.diagnosis}</p>
            <p><strong>Điều trị:</strong> {record.treatment}</p>
          </div>
        ))}
      </div>
    </div>
  );
};

export default TreatmentPlan;
```

#### **Dashboard Component**
```jsx
// Dashboard.jsx
import React, { useState, useEffect } from 'react';
import apiClient from '../services/apiClient';

const Dashboard = () => {
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchDashboardStats();
  }, []);

  const fetchDashboardStats = async () => {
    try {
      const response = await apiClient.getDashboardStats();
      setStats(response.data);
    } catch (error) {
      console.error('Error fetching dashboard stats:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <div>Đang tải...</div>;

  return (
    <div className="dashboard">
      <h1>Bảng điều khiển</h1>
      
      <div className="stats-grid">
        <div className="stat-card">
          <h3>Tổng bệnh nhân</h3>
          <p className="stat-number">{stats.totalCustomers}</p>
        </div>
        
        <div className="stat-card">
          <h3>Tổng bác sĩ</h3>
          <p className="stat-number">{stats.totalDoctors}</p>
        </div>
        
        <div className="stat-card">
          <h3>Điều trị hoàn thành</h3>
          <p className="stat-number">{stats.completedTreatments}</p>
        </div>
        
        <div className="stat-card">
          <h3>Doanh thu</h3>
          <p className="stat-number">{stats.totalRevenue.toLocaleString('vi-VN')} VNĐ</p>
        </div>
        
        <div className="stat-card">
          <h3>Đánh giá trung bình</h3>
          <p className="stat-number">{stats.averageRating}/5</p>
        </div>
        
        <div className="stat-card">
          <h3>Kế hoạch đang thực hiện</h3>
          <p className="stat-number">{stats.activeTreatmentPlans}</p>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
```

### **3. State Management (Redux Toolkit)**

```javascript
// treatmentSlice.js
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import apiClient from '../services/apiClient';

export const fetchTreatmentFlow = createAsyncThunk(
  'treatment/fetchTreatmentFlow',
  async (customerId) => {
    const response = await apiClient.getPatientTreatmentFlow(customerId);
    return response.data;
  }
);

export const updateTreatmentPhase = createAsyncThunk(
  'treatment/updateTreatmentPhase',
  async ({ treatmentPlanId, phaseData }) => {
    const response = await apiClient.updateTreatmentPhase(treatmentPlanId, phaseData);
    return response.data;
  }
);

const treatmentSlice = createSlice({
  name: 'treatment',
  initialState: {
    currentPlan: null,
    appointments: [],
    medicalHistory: [],
    testResults: [],
    loading: false,
    error: null
  },
  reducers: {
    clearTreatment: (state) => {
      state.currentPlan = null;
      state.appointments = [];
      state.medicalHistory = [];
      state.testResults = [];
    }
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchTreatmentFlow.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchTreatmentFlow.fulfilled, (state, action) => {
        state.loading = false;
        state.currentPlan = action.payload.treatmentPlan;
        state.appointments = action.payload.appointments;
        state.medicalHistory = action.payload.medicalHistory;
        state.testResults = action.payload.testResults;
      })
      .addCase(fetchTreatmentFlow.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message;
      })
      .addCase(updateTreatmentPhase.fulfilled, (state, action) => {
        state.currentPlan = { ...state.currentPlan, ...action.payload };
      });
  }
});

export const { clearTreatment } = treatmentSlice.actions;
export default treatmentSlice.reducer;
```

### **4. CSS Styling Example**

```css
/* styles.css */
.treatment-plan {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

.plan-overview {
  background: #f8f9fa;
  padding: 20px;
  border-radius: 8px;
  margin-bottom: 20px;
}

.plan-details {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 15px;
}

.appointment-card {
  background: white;
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 15px;
  margin-bottom: 10px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.medical-record {
  background: white;
  border-left: 4px solid #007bff;
  padding: 15px;
  margin-bottom: 15px;
  border-radius: 4px;
}

.dashboard {
  padding: 20px;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
  margin-top: 20px;
}

.stat-card {
  background: white;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  text-align: center;
}

.stat-number {
  font-size: 2rem;
  font-weight: bold;
  color: #007bff;
  margin: 10px 0;
}

.login-form {
  max-width: 400px;
  margin: 50px auto;
  padding: 30px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0,0,0,0.1);
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  font-weight: bold;
}

.form-group input {
  width: 100%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 16px;
}

button {
  width: 100%;
  padding: 12px;
  background: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 16px;
  cursor: pointer;
}

button:hover {
  background: #0056b3;
}

button:disabled {
  background: #ccc;
  cursor: not-allowed;
}

.error {
  color: #dc3545;
  background: #f8d7da;
  padding: 10px;
  border-radius: 4px;
  margin-bottom: 20px;
}
```

---

## 🚀 **Deployment Checklist**

### **Frontend Deployment**
- [ ] Build production version
- [ ] Configure environment variables
- [ ] Set up API base URL for production
- [ ] Test all API endpoints
- [ ] Implement error handling
- [ ] Add loading states
- [ ] Test authentication flow
- [ ] Validate form inputs
- [ ] Test responsive design

### **API Integration Testing**
- [ ] Test all CRUD operations
- [ ] Verify authentication/authorization
- [ ] Test file uploads (if any)
- [ ] Validate response formats
- [ ] Test error scenarios
- [ ] Performance testing

### **Security Considerations**
- [ ] Implement proper token storage
- [ ] Add token refresh mechanism
- [ ] Validate user permissions
- [ ] Sanitize user inputs
- [ ] Implement rate limiting
- [ ] Add CSRF protection

---

## 📞 **Support**

Nếu gặp vấn đề trong quá trình tích hợp, vui lòng:

1. Kiểm tra console logs để xem lỗi chi tiết
2. Verify API endpoints với Swagger UI: `https://localhost:7139/swagger`
3. Kiểm tra network tab trong browser dev tools
4. Đảm bảo CORS được cấu hình đúng
5. Verify JWT token format và expiration

---

**Tài liệu này sẽ giúp bạn tích hợp Frontend với API ITMMS một cách hiệu quả và đầy đủ!** 🎉 