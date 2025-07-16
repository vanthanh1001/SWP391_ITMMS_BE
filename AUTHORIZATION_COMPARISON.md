# So Sánh Upload File: Có Authorization vs Không Authorization

## 🔒 FileUploadController (CÓ AUTHORIZATION)

### Đặc điểm:
- **Bắt buộc JWT Token**: Phải login trước khi upload
- **Phân quyền theo Role**: Doctor/Admin có quyền upload medical records
- **Bảo mật cao**: Biết được ai upload file gì
- **File naming có context**: `doctor_medical_records_20231026_143022_xray.jpg`

### Endpoints:
```
POST /api/FileUpload/upload                # Cần token
POST /api/FileUpload/medical-records       # Cần token + Doctor/Admin role  
POST /api/FileUpload/profile-images        # Cần token
GET  /api/FileUpload/health                # Không cần token (AllowAnonymous)
```

### Test Request:
```bash
# PHẢI có Authorization header
curl -X POST "https://localhost:7139/api/FileUpload/upload" \
     -H "Authorization: Bearer YOUR_TOKEN_HERE" \
     -F "file=@test.pdf"
```

### Response khi KHÔNG có token:
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

---

## 🌐 FileUploadPublicController (KHÔNG AUTHORIZATION)

### Đặc điểm:
- **Không cần JWT Token**: Upload trực tiếp mà không cần login
- **Không phân quyền**: Ai cũng có thể upload mọi thứ
- **Bảo mật thấp**: Không biết ai upload
- **File naming đơn giản**: `public_upload_20231026_143022_test.pdf`

### Endpoints:
```
POST /api/FileUploadPublic/upload          # Không cần token
POST /api/FileUploadPublic/bulk-upload     # Không cần token
GET  /api/FileUploadPublic/health          # Không cần token
```

### Test Request:
```bash
# KHÔNG cần Authorization header
curl -X POST "https://localhost:7139/api/FileUploadPublic/upload" \
     -F "file=@test.pdf"
```

### Response thành công:
```json
{
  "success": true,
  "message": "File uploaded successfully (no authentication required)",
  "data": {
    "fileName": "public_upload_20231026_143022_test.pdf",
    "originalName": "test.pdf",
    "uploadType": "public",
    "userAuthenticated": false
  }
}
```

---

## 📊 Bảng So Sánh

| Tính năng | FileUploadController (Có Auth) | FileUploadPublicController (Không Auth) |
|-----------|-------------------------------|----------------------------------------|
| **JWT Token** | ✅ Bắt buộc | ❌ Không cần |
| **Role-based Access** | ✅ Doctor/Admin cho medical records | ❌ Không phân quyền |
| **User Context** | ✅ Biết user ID, role, tên | ❌ Không biết ai upload |
| **File Naming** | ✅ `{role}_{category}_{timestamp}_{file}` | 🔸 `public_upload_{timestamp}_{file}` |
| **Security** | ✅ Cao | ❌ Thấp |
| **Audit Trail** | ✅ Có thể trace được | ❌ Không trace được |
| **Medical Records** | ✅ Chỉ Doctor/Admin | ❌ Ai cũng có thể upload |
| **Bulk Upload** | ❌ Chưa có | ✅ Có |

---

## 🧪 Test So Sánh

### Test 1: Upload KHÔNG cần token
```bash
# ✅ THÀNH CÔNG với Public Controller
curl -X POST "https://localhost:7139/api/FileUploadPublic/upload" \
     -F "file=@test.pdf"

# ❌ THẤT BẠI với Auth Controller  
curl -X POST "https://localhost:7139/api/FileUpload/upload" \
     -F "file=@test.pdf"
# Response: 401 Unauthorized
```

### Test 2: Upload CÓ token
```bash
# Lấy token trước
TOKEN=$(curl -X POST "https://localhost:7139/api/Auth/login" \
     -H "Content-Type: application/json" \
     -d '{"Email":"doctor1@itmms.com","Password":"doctor123"}' \
     | jq -r '.token')

# ✅ THÀNH CÔNG với cả hai controller
curl -X POST "https://localhost:7139/api/FileUpload/upload" \
     -H "Authorization: Bearer $TOKEN" \
     -F "file=@test.pdf"

curl -X POST "https://localhost:7139/api/FileUploadPublic/upload" \
     -F "file=@test.pdf"
```

### Test 3: Medical Records
```bash
# ✅ THÀNH CÔNG với Doctor token
curl -X POST "https://localhost:7139/api/FileUpload/medical-records" \
     -H "Authorization: Bearer $DOCTOR_TOKEN" \
     -F "file=@medical.pdf"

# ❌ Public controller không có endpoint này
curl -X POST "https://localhost:7139/api/FileUploadPublic/medical-records" \
     -F "file=@medical.pdf"
# Response: 404 Not Found
```

---

## 🚨 Khi nào dùng cái nào?

### Dùng **FileUploadController (Có Auth)** khi:
- ✅ Cần bảo mật cao
- ✅ Cần phân quyền upload
- ✅ Cần trace được ai upload gì
- ✅ Upload medical records, hồ sơ nhạy cảm
- ✅ Production environment

### Dùng **FileUploadPublicController (Không Auth)** khi:
- ✅ Upload public files (avatar, banner)
- ✅ Demo, testing nhanh
- ✅ Guest users upload
- ✅ Development environment
- ❌ **KHÔNG dùng cho production với data nhạy cảm**

---

## 🔧 Cách chuyển đổi

### Muốn tắt Authorization cho FileUploadController:
```csharp
[ApiController]
[Route("api/[controller]")]
// Bỏ dòng [Authorize] này
public class FileUploadController : ControllerBase
```

### Muốn bật Authorization cho FileUploadPublicController:
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // Thêm dòng này
public class FileUploadPublicController : ControllerBase
```

---

## 💡 Kết luận

**Có Authorization:**
- 🔒 **Bảo mật**: Bắt buộc phải login
- 👤 **Phân quyền**: Role-based access control  
- 📋 **Audit**: Biết ai làm gì
- 🏥 **Medical**: Chỉ Doctor/Admin upload được

**Không Authorization:**
- 🌐 **Public**: Ai cũng upload được
- ⚡ **Nhanh**: Không cần login
- 🧪 **Test**: Tiện cho development
- ⚠️ **Rủi ro**: Không kiểm soát được

**Khuyến nghị**: Dùng có Authorization cho production, không Authorization chỉ để test! 