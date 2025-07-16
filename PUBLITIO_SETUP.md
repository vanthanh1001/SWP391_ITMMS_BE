# Hướng Dẫn Cài Đặt Publitio Service

Dự án đã được chuyển từ Firebase Storage sang Publitio để upload và quản lý file. Publitio cung cấp API mạnh mẽ để upload, transform và quản lý media files.

## Bước 1: Đăng ký tài khoản Publitio

1. Truy cập [https://publit.io/](https://publit.io/)
2. Đăng ký tài khoản miễn phí
3. Đăng nhập vào Dashboard
4. Lấy **API Key** và **API Secret** từ Dashboard

## Bước 2: Cập nhật cấu hình

Mở file `appsettings.json` và thay thế giá trị placeholder:

```json
{
  "Publitio": {
    "ApiKey": "your_actual_api_key_here",
    "ApiSecret": "your_actual_api_secret_here"
  }
}
```

## Bước 3: Test API

1. Chạy ứng dụng:
   ```bash
   dotnet run
   ```

2. Truy cập Swagger UI: `https://localhost:5037/swagger`

3. Test endpoint `/api/FileUpload` để upload file

## Tính năng của Publitio Service

- **Upload file**: POST `/api/FileUpload`
- **Delete file**: DELETE `/api/FileUpload/{fileId}`
- Hỗ trợ nhiều định dạng: images, videos, audio, documents
- Tự động tạo thumbnail và preview URL
- Transform file on-the-fly qua URL

## Ví dụ Response khi upload thành công

```json
{
  "fileUrl": "https://media.publit.io/file/abc123.jpg"
}
```

## URL Transform (Bonus feature)

Publitio cho phép transform file qua URL:

- Resize: `https://media.publit.io/file/w_300,h_200/abc123.jpg`
- Quality: `https://media.publit.io/file/q_80/abc123.jpg`
- Format: `https://media.publit.io/file/abc123.png`

## Lưu ý

- API Key và Secret cần được bảo mật
- Publitio có free tier với giới hạn storage và bandwidth
- Check dashboard để monitor usage 