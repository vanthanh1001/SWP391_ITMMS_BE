# ITMMS API - Hệ thống Quản lý và Theo dõi Điều trị Hiếm muộn

## Tổng quan

Hệ thống ITMMS (Infertility Treatment Management and Monitoring System) là một API backend được phát triển bằng ASP.NET Core, hỗ trợ quản lý và theo dõi quá trình điều trị hiếm muộn.

## Công nghệ sử dụng

- **Framework**: ASP.NET Core 8.0
- **Database**: SQL Server với Entity Framework Core
- **Authentication**: BCrypt cho hash password
- **Documentation**: Swagger/OpenAPI
- **Validation**: Data Annotations

## Cấu trúc hệ thống

### Roles (Vai trò người dùng)
- **Guest**: Khách vãng lai, có thể xem thông tin công khai
- **Customer**: Bệnh nhân, có thể đặt lịch hẹn, xem kết quả điều trị
- **Doctor**: Bác sĩ, quản lý lịch hẹn, ghi nhận kết quả khám
- **Manager**: Quản lý, xem báo cáo tổng quan
- **Admin**: Quản trị viên, quản lý toàn bộ hệ thống

### Các chức năng chính

#### Cho Customer (Bệnh nhân):
- Đăng ký/Đăng nhập tài khoản
- Tìm kiếm và đặt lịch hẹn với bác sĩ
- Xem lịch sử điều trị và kết quả xét nghiệm
- Đánh giá và phản hồi về bác sĩ
- Theo dõi kế hoạch điều trị

#### Cho Doctor (Bác sĩ):
- Quản lý lịch hẹn và thời gian khám
- Ghi nhận kết quả khám bệnh và chẩn đoán
- Tạo và quản lý kế hoạch điều trị
- Nhập kết quả xét nghiệm
- Kê đơn thuốc
- Viết blog y khoa

#### Cho Manager/Admin:
- Quản lý thông tin bác sĩ
- Xem báo cáo và thống kê tổng quan
- Quản lý người dùng
- Phê duyệt nội dung blog

## Database Schema

### Các bảng chính:
- **Users**: Thông tin người dùng
- **Doctors**: Thông tin chi tiết bác sĩ
- **Customers**: Thông tin chi tiết bệnh nhân
- **Appointments**: Lịch hẹn khám
- **TreatmentPlans**: Kế hoạch điều trị
- **MedicalRecords**: Hồ sơ bệnh án
- **TestResults**: Kết quả xét nghiệm
- **Prescriptions**: Đơn thuốc
- **Feedback**: Đánh giá và phản hồi
- **BlogPosts**: Bài viết blog

## API Endpoints

### Authentication (api/auth)
```
POST /api/auth/register     - Đăng ký tài khoản
POST /api/auth/login        - Đăng nhập
POST /api/auth/check-email  - Kiểm tra email đã tồn tại
POST /api/auth/check-username - Kiểm tra username đã tồn tại
```

### Doctors (api/doctors)
```
GET  /api/doctors              - Lấy danh sách tất cả bác sĩ
GET  /api/doctors/{id}         - Lấy thông tin bác sĩ theo ID
GET  /api/doctors/{id}/schedule - Xem lịch trình bác sĩ
GET  /api/doctors/{id}/feedback - Xem đánh giá của bác sĩ
GET  /api/doctors/specializations - Lấy danh sách chuyên khoa
GET  /api/doctors/search       - Tìm kiếm bác sĩ
PUT  /api/doctors/{id}         - Cập nhật thông tin bác sĩ
PUT  /api/doctors/{id}/availability - Cập nhật trạng thái khả dụng
```

### Appointments (api/appointments)
```
POST /api/appointments         - Tạo lịch hẹn mới
GET  /api/appointments/{id}    - Lấy thông tin lịch hẹn
GET  /api/appointments/customer/{customerId} - Lịch hẹn của bệnh nhân
GET  /api/appointments/doctor/{doctorId}     - Lịch hẹn của bác sĩ
GET  /api/appointments/available-slots       - Xem khung giờ trống
PUT  /api/appointments/{id}/status           - Cập nhật trạng thái
DELETE /api/appointments/{id}                - Hủy lịch hẹn
```

### Health Check
```
GET  /api/health              - Kiểm tra tình trạng API
GET  /                        - Trang chủ API
```

## Cài đặt và Chạy

### Yêu cầu hệ thống
- .NET 8.0 SDK
- SQL Server (LocalDB hoặc SQL Server Express)
- Visual Studio 2022 hoặc VS Code

### Các bước cài đặt

1. **Clone project**:
```bash
git clone <repository-url>
cd SWP391_ITMMS_BE
```

2. **Cài đặt dependencies**:
```bash
dotnet restore
```

3. **Cấu hình database**:
   - Cập nhật connection string trong `appsettings.json`
   - Mặc định sử dụng Windows Authentication với LocalDB

4. **Tạo database**:
```bash
dotnet ef database update
```

5. **Chạy ứng dụng**:
```bash
dotnet run
```

### URL truy cập
- **API**: `https://localhost:7139` hoặc `http://localhost:5139`
- **Swagger UI**: `https://localhost:7139/swagger`

## Dữ liệu mẫu

Hệ thống sẽ tự động tạo dữ liệu mẫu khi khởi tạo database:

### Admin Account
- **Email**: admin@itmms.com
- **Password**: admin123
- **Role**: Admin

### Doctor Account
- **Email**: doctor1@itmms.com
- **Password**: doctor123
- **Role**: Doctor
- **Specialization**: Sản phụ khoa - Hiếm muộn

## Cấu trúc thư mục

```
SWP391_ITMMS_BE/
├── Controllers/           # API Controllers
│   ├── AuthController.cs
│   ├── DoctorsController.cs
│   ├── AppointmentsController.cs
│   ├── UserController.cs
│   └── BlogController.cs
├── Data/                  # Database Context
│   └── AppDbContext.cs
├── Models/                # Entity Models và DTOs
│   ├── User.cs
│   └── BlogPost.cs
├── Services/              # Business Logic Services
│   ├── IUserService.cs
│   ├── UserService.cs
│   ├── IAppointmentService.cs
│   └── AppointmentService.cs
├── Migrations/            # EF Core Migrations
├── Properties/
├── Program.cs             # Application entry point
└── appsettings.json       # Configuration
```

## Tính năng đã triển khai

✅ **User Management**
- Đăng ký/Đăng nhập với validation
- Phân quyền theo role
- Hash password với BCrypt

✅ **Doctor Management**
- Quản lý thông tin bác sĩ
- Tìm kiếm theo chuyên khoa
- Xem lịch trình và đánh giá

✅ **Appointment System**
- Đặt lịch hẹn với validation thời gian
- Quản lý trạng thái lịch hẹn
- Kiểm tra khung giờ trống

✅ **Database Design**
- Schema hoàn chỉnh với relationships
- Data seeding
- Migration scripts

✅ **API Documentation**
- Swagger UI
- RESTful API design
- Error handling

## Tính năng cần phát triển thêm

🔲 **Medical Records Management**
- CRUD operations cho hồ sơ bệnh án
- Prescription management
- Test results tracking

🔲 **Treatment Plans**
- Tạo và theo dõi kế hoạch điều trị
- Progress tracking
- Cost management

🔲 **Feedback System**
- Rating và comment
- Review management

🔲 **Blog System**
- Quản lý bài viết y khoa
- Content moderation

🔲 **Advanced Features**
- JWT Authentication
- File upload (images, documents)
- Email notifications
- Real-time chat
- Reporting và analytics

## Testing

Để test API, bạn có thể sử dụng:
1. **Swagger UI**: Truy cập `/swagger` để test interactively
2. **Postman**: Import các endpoint từ swagger
3. **Unit Tests**: Sẽ được thêm trong phiên bản tiếp theo

## Liên hệ

Nếu có thắc mắc hoặc cần hỗ trợ, vui lòng liên hệ team phát triển.

---

**Lưu ý**: Đây là phiên bản MVP (Minimum Viable Product). Các tính năng sẽ được bổ sung và cải thiện trong các phiên bản tiếp theo.