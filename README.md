# Hệ Thống Quản Lý Hành Chính Sinh Viên (Cổng Một Cửa Điện Tử)

Hệ thống cổng dịch vụ hành chính công một cửa dành cho sinh viên trường Đại học, được xây dựng hoàn chỉnh bằng **C# (ASP.NET Core MVC .NET 10.0)** kết hợp giao diện hiện đại **Tailwind CSS**, **Entity Framework Core (SQLite)**, **QRCoder** và **Lucide Icons**.

---

## 🚀 Công Nghệ Sử Dụng

- **Ngôn ngữ & Nền tảng:** C# (.NET 10.0 SDK)
- **Kiến trúc:** ASP.NET Core MVC (Model - View - Controller)
- **Cơ sở dữ liệu & ORM:** SQLite + Entity Framework Core (Code-First)
- **Giao diện (Frontend):** Tailwind CSS (Play CDN with Typography, Forms, Aspect-Ratio plugins) + Lucide Icons + Google Font Plus Jakarta Sans
- **Bảo mật & Phân quyền:** Cookie Authentication (`[Authorize(Roles = "Student")]`, `[Authorize(Roles = "Admin")]`), mật khẩu mã hóa bằng **BCrypt.Net-Next**
- **Tiện ích:** Tạo mã QR xác thực điện tử trực tiếp bằng **QRCoder**

---

## 👥 Hai Phân Hệ Vai Trò (Roles)

### 1. Phân hệ Sinh viên (Student)
- **Đăng nhập & Quản lý hồ sơ:** Đăng nhập bằng MSSV, xem và cập nhật thông tin cá nhân (Họ tên, SĐT, Email, CCCD, Ngày sinh, Địa chỉ), đổi mật khẩu.
- **Danh mục thủ tục trực tuyến:** Tra cứu danh sách thủ tục, đọc hướng dẫn hồ sơ giấy tờ cần chuẩn bị và thời gian giải quyết dự kiến.
- **Nộp hồ sơ trực tuyến:** Điền lý do, số lượng bản cấp, hình thức nhận (Bản điện tử / Bản cứng tại phòng Một Cửa), đính kèm file (PDF, hình ảnh CCCD, đơn,...). Tự động tạo mã hồ sơ theo chuẩn `YC-2026-XXXX`.
- **Theo dõi tiến trình (Timeline Stepper):** Theo dõi trạng thái từng bước: *Chờ tiếp nhận ➔ Đang xử lý ➔ Yêu cầu bổ sung ➔ Đã duyệt (hoặc Từ chối)*.
- **Nộp bổ sung giấy tờ:** Khi hồ sơ bị cán bộ yêu cầu bổ sung, sinh viên nhận thông báo và có form tải thêm file bổ sung để chuyển lại trạng thái thẩm định.
- **Nhận kết quả & Giấy xác nhận điện tử:** Tải/in Giấy xác nhận chuẩn thể thức văn bản hành chính Việt Nam, có dấu đỏ điện tử và **Mã QR xác thực trực tuyến**.

### 2. Phân hệ Quản trị viên (Admin - Cán bộ Một Cửa / CTSV)
- **Bảng điều khiển & Thống kê (Dashboard):**
  - Thống kê KPIs: Tổng hồ sơ, Chờ tiếp nhận, Đang xử lý, Cần bổ sung, Đã duyệt, Đã từ chối.
  - Danh sách hồ sơ cần xử lý gấp.
  - Biểu đồ phân bổ tỷ lệ hồ sơ theo từng thủ tục và theo từng Khoa/Viện.
- **Tiếp nhận & Thẩm định hồ sơ:**
  - Bộ lọc đa tiêu chí: Trạng thái, loại thủ tục, từ khóa tìm kiếm (MSSV/Họ tên/Mã hồ sơ), khoảng ngày.
  - Xem chi tiết lý lịch sinh viên và tải/xem các file minh chứng đính kèm.
  - Thao tác xử lý: Tiếp nhận ➔ Yêu cầu bổ sung (kèm ghi chú) ➔ Phê duyệt / Từ chối (kèm lý do và file kết quả).
- **Quản lý danh mục thủ tục:** Thêm mới thủ tục, sửa nội dung, cấu hình thời hạn xử lý (ngày làm việc), bật/tắt tiếp nhận trực tuyến.
- **Quản lý sinh viên:** Danh sách sinh viên trường, tìm kiếm theo lớp/khoa, xem chi tiết lý lịch và toàn bộ lịch sử yêu cầu của từng sinh viên.

---

## 🔑 Tài Khoản Mẫu Thử Nghiệm (Đã có sẵn nút bấm 1-Click Fill ở trang Đăng nhập)

| Vai trò | Tên đăng nhập | Mật khẩu | Họ và tên | Chức năng nổi bật |
| :--- | :--- | :--- | :--- | :--- |
| **Quản trị viên** | `admin` | `admin123` | Nguyễn Văn Trưởng (Phòng CTSV) | Toàn quyền Quản trị, Thống kê, Thẩm định hồ sơ, Quản lý thủ tục |
| **Sinh viên 1** | `SV2026001` | `student123` | Nguyễn Văn An (CNTT1-K21) | Có 1 hồ sơ **Đã duyệt** (xem Giấy xác nhận QR), 1 hồ sơ **Cần bổ sung giấy tờ**, 1 hồ sơ **Đang xử lý** |
| **Sinh viên 2** | `SV2026002` | `student123` | Trần Thị Bích (QTKD2-K22) | Có 1 hồ sơ **Chờ tiếp nhận**, 1 hồ sơ **Đã hoàn thành** |
| **Sinh viên 3** | `SV2026003` | `student123` | Lê Hoàng Nam (KTPM-K20) | Có 1 hồ sơ **Bị từ chối** (kèm lý do chưa đủ tín chỉ) |

---

## 🏃 Hướng Dẫn Chạy Ứng Dụng

Ứng dụng sử dụng **SQLite**, cơ sở dữ liệu `QuanLyHanhChinh.db` sẽ tự động khởi tạo và seed dữ liệu ngay lần đầu chạy.

```bash
# Di chuyển vào thư mục dự án
cd quan-ly-hanh-chinh

# Chạy ứng dụng
dotnet run --launch-profile "http"
```

Mở trình duyệt truy cập: **`http://localhost:5011`**
