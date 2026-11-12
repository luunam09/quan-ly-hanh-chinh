using StudentAdminPortal.Models;

namespace StudentAdminPortal.Data;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Ensure database exists
        context.Database.EnsureCreated();

        // Check if users already seeded
        if (context.Users.Any())
        {
            return; // DB already seeded
        }

        // 1. Seed Users
        string adminHash = BCrypt.Net.BCrypt.HashPassword("admin123");
        string studentHash = BCrypt.Net.BCrypt.HashPassword("student123");

        var admin = new User
        {
            Username = "admin",
            PasswordHash = adminHash,
            Role = "Admin",
            FullName = "Nguyễn Văn Trưởng (Phòng CTSV)",
            Email = "ctsv@daihoc.edu.vn",
            PhoneNumber = "0243.869.2222",
            Faculty = "Phòng Công tác Sinh viên & Một Cửa",
            ClassName = "Ban Cán sự Quản lý",
            CccdNumber = "001085001234",
            Address = "Số 1 Đại Cồ Việt, Hai Bà Trưng, Hà Nội",
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddMonths(-6)
        };

        var student1 = new User
        {
            Username = "SV2026001",
            PasswordHash = studentHash,
            Role = "Student",
            FullName = "Nguyễn Văn An",
            Email = "an.nv2026001@sinhvien.edu.vn",
            PhoneNumber = "0987654321",
            DateOfBirth = new DateTime(2004, 5, 15),
            Gender = "Nam",
            ClassName = "CNTT1-K21",
            Faculty = "Công nghệ Thông tin",
            CccdNumber = "001204005678",
            Address = "Phường Dịch Vọng Hậu, Quận Cầu Giấy, Hà Nội",
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddMonths(-3)
        };

        var student2 = new User
        {
            Username = "SV2026002",
            PasswordHash = studentHash,
            Role = "Student",
            FullName = "Trần Thị Bích",
            Email = "bich.tt2026002@sinhvien.edu.vn",
            PhoneNumber = "0912345678",
            DateOfBirth = new DateTime(2004, 11, 20),
            Gender = "Nữ",
            ClassName = "QTKD2-K22",
            Faculty = "Quản trị Kinh doanh",
            CccdNumber = "034204009988",
            Address = "Phường Bạch Mai, Quận Hai Bà Trưng, Hà Nội",
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddMonths(-2)
        };

        var student3 = new User
        {
            Username = "SV2026003",
            PasswordHash = studentHash,
            Role = "Student",
            FullName = "Lê Hoàng Nam",
            Email = "nam.lh2026003@sinhvien.edu.vn",
            PhoneNumber = "0977889900",
            DateOfBirth = new DateTime(2003, 3, 10),
            Gender = "Nam",
            ClassName = "KTPM-K20",
            Faculty = "Công nghệ Thông tin",
            CccdNumber = "038203007766",
            Address = "Phường Giảng Võ, Quận Ba Đình, Hà Nội",
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddMonths(-4)
        };

        context.Users.AddRange(admin, student1, student2, student3);
        context.SaveChanges();

        // 2. Seed Procedures
        var proc1 = new Procedure
        {
            Code = "XN-VAYVON",
            Name = "Giấy xác nhận sinh viên (Vay vốn Ngân hàng CSXH)",
            Description = "Cấp giấy xác nhận đang theo học để sinh viên hoàn thiện hồ sơ vay vốn tín dụng học sinh, sinh viên tại Ngân hàng Chính sách Xã hội.",
            ProcessingDays = 2,
            RequiredDocuments = "1. Mẫu giấy đề nghị vay vốn (Mẫu 01/TDSV có chữ ký của gia đình)\n2. Ảnh chụp CCCD của sinh viên\n3. Giấy báo nhập học hoặc biên lai học phí kỳ gần nhất",
            IconClass = "bi-cash-coin",
            IsActive = true
        };

        var proc2 = new Procedure
        {
            Code = "XN-NVQS",
            Name = "Giấy xác nhận sinh viên (Tạm hoãn Nghĩa vụ Quân sự)",
            Description = "Xác nhận thời gian đào tạo chính khóa tại trường để nộp Ban chỉ huy Quân sự cấp xã/phường nơi cư trú xem xét tạm hoãn gọi nhập ngũ.",
            ProcessingDays = 1,
            RequiredDocuments = "1. Giấy triệu tập hoặc thông báo khám/đăng ký NVQS của địa phương\n2. Ảnh thẻ sinh viên hoặc CCCD",
            IconClass = "bi-shield-check",
            IsActive = true
        };

        var proc3 = new Procedure
        {
            Code = "CAPLAI-THE",
            Name = "Đơn xin cấp lại thẻ sinh viên",
            Description = "Thực hiện cấp mới thẻ sinh viên bị mất, gãy hỏng hoặc lỗi chip thanh toán / ra vào cổng trường.",
            ProcessingDays = 5,
            RequiredDocuments = "1. Đơn trình báo mất thẻ (nếu mất)\n2. Ảnh chân dung 3x4 (chụp nền trắng trong vòng 6 tháng)\n3. Biên lai nộp lệ phí làm lại thẻ (50.000 VNĐ)",
            IconClass = "bi-person-badge",
            IsActive = true
        };

        var proc4 = new Procedure
        {
            Code = "BANGDIEM-TAM",
            Name = "Đơn xin cấp bảng điểm học tập tạm thời",
            Description = "Cấp bảng điểm tích lũy học tập có xác nhận của Phòng Đào tạo phục vụ xin việc làm thêm, học bổng hoặc học nâng cao.",
            ProcessingDays = 3,
            RequiredDocuments = "1. Ảnh thẻ sinh viên\n2. Phiếu đăng ký số lượng bản in bảng điểm",
            IconClass = "bi-file-earmark-spreadsheet",
            IsActive = true
        };

        var proc5 = new Procedure
        {
            Code = "GIAY-THUCTAP",
            Name = "Giấy giới thiệu liên hệ thực tập Doanh nghiệp",
            Description = "Cấp giấy giới thiệu chính thức từ nhà trường gửi đến các cơ quan, doanh nghiệp tiếp nhận sinh viên thực tập.",
            ProcessingDays = 2,
            RequiredDocuments = "1. Tên cơ quan/công ty tiếp nhận thực tập\n2. Họ tên người phụ trách/Đơn vị tiếp nhận\n3. Đơn xin đi thực tập có chữ ký của Trưởng bộ môn/Khoa",
            IconClass = "bi-building-check",
            IsActive = true
        };

        var proc6 = new Procedure
        {
            Code = "MIEN-GIAM",
            Name = "Đơn xin miễn giảm học phí / Hỗ trợ chi phí",
            Description = "Thực hiện xét chế độ miễn, giảm học phí theo Nghị định của Chính phủ dành cho sinh viên thuộc đối tượng chính sách, hộ nghèo/cận nghèo, khuyết tật...",
            ProcessingDays = 7,
            RequiredDocuments = "1. Đơn đề nghị miễn giảm học phí theo mẫu của Bộ GD&ĐT\n2. Giấy chứng nhận hộ nghèo/cận nghèo công chứng hoặc Giấy tờ thương binh, liệt sĩ...\n3. Bản sao công chứng CCCD và Giấy khai sinh",
            IconClass = "bi-award",
            IsActive = true
        };

        var proc7 = new Procedure
        {
            Code = "BAOLUU-DIHOC",
            Name = "Đơn xin nghỉ học tạm thời / Bảo lưu kết quả",
            Description = "Thủ tục xin tạm ngừng học tập vì lý do sức khỏe, nghĩa vụ hoặc hoàn cảnh gia đình với thời hạn tối đa theo quy chế đào tạo.",
            ProcessingDays = 5,
            RequiredDocuments = "1. Đơn xin nghỉ học tạm thời có ý kiến phụ huynh\n2. Giấy tờ minh chứng (Bệnh án bệnh viện cấp huyện trở lên / Lệnh gọi NVQS...)\n3. Giấy xác nhận không nợ sách thư viện và tài chính trường",
            IconClass = "bi-pause-circle",
            IsActive = true
        };

        context.Procedures.AddRange(proc1, proc2, proc3, proc4, proc5, proc6, proc7);
        context.SaveChanges();

        // 3. Seed Realistic Requests
        var req1 = new AdministrativeRequest
        {
            RequestCode = "YC-2026-0001",
            StudentId = student1.Id,
            ProcedureId = proc1.Id,
            Status = RequestStatus.Approved,
            Purpose = "Vay vốn tín dụng học sinh sinh viên tại Ngân hàng Chính sách Xã hội huyện Ba Vì để trang trải học phí năm học 2026.",
            Quantity = 2,
            ReceiveMethod = "Digital",
            StudentNote = "Kính mong thầy cô tạo điều kiện ký duyệt sớm trước ngày 15 để kịp nộp hồ sơ ngân hàng.",
            AdminFeedback = "Hồ sơ đầy đủ, hợp lệ theo quy chế. Đã phê duyệt và xuất giấy xác nhận kèm mã xác thực điện tử.",
            ResultNote = "Giấy xác nhận điện tử đã được ký số và cấp mã QR xác thực trực tuyến. Sinh viên có thể tải về in ra sử dụng.",
            VerificationCode = "VERIF-XN-88912",
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-3),
            CompletedAt = DateTime.UtcNow.AddDays(-3),
            ExpectedDate = DateTime.UtcNow.AddDays(-3)
        };

        var req2 = new AdministrativeRequest
        {
            RequestCode = "YC-2026-0002",
            StudentId = student1.Id,
            ProcedureId = proc4.Id,
            Status = RequestStatus.NeedUpdate,
            Purpose = "Xin bảng điểm tiếng Việt tạm thời để nộp hồ sơ ứng tuyển thực tập tại Tập đoàn Viettel.",
            Quantity = 1,
            ReceiveMethod = "Digital",
            StudentNote = "Em cần bảng điểm tính đến hết kỳ 1 năm học 2025-2026 ạ.",
            AdminFeedback = "Ảnh chụp thẻ sinh viên bạn tải lên bị nhòe mờ thông tin. Vui lòng chụp rõ nét lại 2 mặt thẻ sinh viên hoặc thay bằng ảnh chụp CCCD để đối chiếu.",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UpdatedAt = DateTime.UtcNow.AddHours(-10),
            ExpectedDate = DateTime.UtcNow.AddDays(1)
        };

        var req3 = new AdministrativeRequest
        {
            RequestCode = "YC-2026-0003",
            StudentId = student1.Id,
            ProcedureId = proc3.Id,
            Status = RequestStatus.Processing,
            Purpose = "Bị rơi ví đánh mất thẻ sinh viên trên xe buýt tuyến 26 ngày hôm qua.",
            Quantity = 1,
            ReceiveMethod = "Office",
            StudentNote = "Em đã chuyển khoản 50.000 VNĐ lệ phí vào tài khoản nhà trường.",
            AdminFeedback = "Đã tiếp nhận hồ sơ và chuyển thông tin sang đơn vị in phôi thẻ sinh viên liên kết ngân hàng.",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddHours(-4),
            ExpectedDate = DateTime.UtcNow.AddDays(4)
        };

        var req4 = new AdministrativeRequest
        {
            RequestCode = "YC-2026-0004",
            StudentId = student2.Id,
            ProcedureId = proc2.Id,
            Status = RequestStatus.Pending,
            Purpose = "Bổ sung hồ sơ tạm hoãn nghĩa vụ quân sự đợt tuyển quân tháng 11 theo lệnh gọi của Ban CHQS phường Bạch Mai.",
            Quantity = 1,
            ReceiveMethod = "Digital",
            StudentNote = "Mong phòng CTSV xử lý sớm giúp em ạ.",
            CreatedAt = DateTime.UtcNow.AddHours(-3),
            ExpectedDate = DateTime.UtcNow.AddDays(1)
        };

        var req5 = new AdministrativeRequest
        {
            RequestCode = "YC-2026-0005",
            StudentId = student2.Id,
            ProcedureId = proc6.Id,
            Status = RequestStatus.Approved,
            Purpose = "Hưởng chế độ giảm học phí theo diện hộ cận nghèo năm học 2026.",
            Quantity = 1,
            ReceiveMethod = "Office",
            AdminFeedback = "Đã kiểm tra đối chiếu sổ hộ cận nghèo bản gốc, hồ sơ đủ điều kiện giảm 50% học phí theo quy định.",
            ResultNote = "Mời sinh viên đến Phòng Một Cửa (P.102 Nhà A1) nhận giấy chứng nhận và phiếu khấu trừ học phí vào giờ hành chính.",
            VerificationCode = "VERIF-MG-44219",
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            CompletedAt = DateTime.UtcNow.AddDays(-4),
            ExpectedDate = DateTime.UtcNow.AddDays(-4)
        };

        var req6 = new AdministrativeRequest
        {
            RequestCode = "YC-2026-0006",
            StudentId = student3.Id,
            ProcedureId = proc5.Id,
            Status = RequestStatus.Rejected,
            Purpose = "Xin giấy giới thiệu thực tập tại Công ty FPT Software.",
            Quantity = 1,
            ReceiveMethod = "Digital",
            StudentNote = "Thực tập đợt tốt nghiệp.",
            AdminFeedback = "Từ chối tiếp nhận: Qua kiểm tra hệ thống đào tạo, sinh viên chưa tích lũy đủ số tín chỉ quy định tối thiểu (cần đạt >= 90 tín chỉ) để được cấp giấy giới thiệu thực tập doanh nghiệp.",
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            UpdatedAt = DateTime.UtcNow.AddDays(-2),
            CompletedAt = DateTime.UtcNow.AddDays(-2)
        };

        context.Requests.AddRange(req1, req2, req3, req4, req5, req6);
        context.SaveChanges();

        // 4. Seed Attachments
        var att1 = new RequestAttachment
        {
            RequestId = req1.Id,
            FileName = "Mau_Don_Vay_Von_01TDSV.pdf",
            FilePath = "/uploads/requests/sample_don_vay_von.pdf",
            FileType = "application/pdf",
            FileSize = 245000,
            IsSupplementary = false,
            UploadedAt = DateTime.UtcNow.AddDays(-5)
        };

        var att2 = new RequestAttachment
        {
            RequestId = req1.Id,
            FileName = "CCCD_Hai_Mat.jpg",
            FilePath = "/uploads/requests/sample_cccd.jpg",
            FileType = "image/jpeg",
            FileSize = 520000,
            IsSupplementary = false,
            UploadedAt = DateTime.UtcNow.AddDays(-5)
        };

        var att3 = new RequestAttachment
        {
            RequestId = req2.Id,
            FileName = "The_Sinh_Vien_Bi_Mo.jpg",
            FilePath = "/uploads/requests/sample_thesv.jpg",
            FileType = "image/jpeg",
            FileSize = 180000,
            IsSupplementary = false,
            UploadedAt = DateTime.UtcNow.AddDays(-2)
        };

        var att4 = new RequestAttachment
        {
            RequestId = req3.Id,
            FileName = "Bien_Lai_Dong_Le_Phi_50k.jpg",
            FilePath = "/uploads/requests/sample_bienlai.jpg",
            FileType = "image/jpeg",
            FileSize = 310000,
            IsSupplementary = false,
            UploadedAt = DateTime.UtcNow.AddDays(-1)
        };

        var att5 = new RequestAttachment
        {
            RequestId = req4.Id,
            FileName = "Lenh_Goi_Kham_NVQS_2026.pdf",
            FilePath = "/uploads/requests/sample_lenh_nvqs.pdf",
            FileType = "application/pdf",
            FileSize = 412000,
            IsSupplementary = false,
            UploadedAt = DateTime.UtcNow.AddHours(-3)
        };

        context.Attachments.AddRange(att1, att2, att3, att4, att5);

        // 5. Seed Request Logs
        context.RequestLogs.AddRange(
            new RequestLog
            {
                RequestId = req1.Id,
                ActorName = student1.FullName,
                ActorRole = "Student",
                Action = "Tạo mới yêu cầu",
                Note = "Gửi yêu cầu xin cấp Giấy xác nhận sinh viên vay vốn",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new RequestLog
            {
                RequestId = req1.Id,
                ActorName = admin.FullName,
                ActorRole = "Admin",
                Action = "Tiếp nhận hồ sơ",
                Note = "Cán bộ đã tiếp nhận và đang tiến hành xác minh thông tin sinh viên",
                CreatedAt = DateTime.UtcNow.AddDays(-4)
            },
            new RequestLog
            {
                RequestId = req1.Id,
                ActorName = admin.FullName,
                ActorRole = "Admin",
                Action = "Phê duyệt & Trả kết quả",
                Note = "Hồ sơ đầy đủ, cấp mã xác thực điện tử và phê duyệt hoàn thành",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new RequestLog
            {
                RequestId = req2.Id,
                ActorName = student1.FullName,
                ActorRole = "Student",
                Action = "Tạo mới yêu cầu",
                Note = "Gửi yêu cầu cấp bảng điểm tạm thời",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new RequestLog
            {
                RequestId = req2.Id,
                ActorName = admin.FullName,
                ActorRole = "Admin",
                Action = "Yêu cầu bổ sung",
                Note = "Ảnh chụp thẻ sinh viên bị mờ, yêu cầu chụp lại hoặc nộp CCCD",
                CreatedAt = DateTime.UtcNow.AddHours(-10)
            },
            new RequestLog
            {
                RequestId = req6.Id,
                ActorName = student3.FullName,
                ActorRole = "Student",
                Action = "Tạo mới yêu cầu",
                Note = "Gửi yêu cầu cấp giấy giới thiệu thực tập",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new RequestLog
            {
                RequestId = req6.Id,
                ActorName = admin.FullName,
                ActorRole = "Admin",
                Action = "Từ chối tiếp nhận",
                Note = "Chưa đủ 90 tín chỉ tích lũy",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            }
        );

        context.SaveChanges();
    }
}
