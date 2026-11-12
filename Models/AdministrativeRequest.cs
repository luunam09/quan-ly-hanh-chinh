using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAdminPortal.Models;

public static class RequestStatus
{
    public const string Pending = "Pending";
    public const string Processing = "Processing";
    public const string NeedUpdate = "NeedUpdate";
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";

    public static string ToVietnamese(string status) => status switch
    {
        Pending => "Chờ tiếp nhận",
        Processing => "Đang xử lý",
        NeedUpdate => "Yêu cầu bổ sung",
        Approved => "Đã duyệt / Hoàn thành",
        Rejected => "Từ chối tiếp nhận",
        _ => status
    };

    public static string ToBadgeClass(string status) => status switch
    {
        Pending => "badge bg-warning text-dark",
        Processing => "badge bg-info text-dark",
        NeedUpdate => "badge bg-danger",
        Approved => "badge bg-success",
        Rejected => "badge bg-secondary",
        _ => "badge bg-light text-dark"
    };
}

public class AdministrativeRequest
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string RequestCode { get; set; } = string.Empty; // e.g., "YC-2026-0001"

    [Required]
    public int StudentId { get; set; }

    [ForeignKey(nameof(StudentId))]
    public User? Student { get; set; }

    [Required]
    public int ProcedureId { get; set; }

    [ForeignKey(nameof(ProcedureId))]
    public Procedure? Procedure { get; set; }

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = RequestStatus.Pending;

    [MaxLength(500)]
    public string? Purpose { get; set; } // Lý do, mục đích đề nghị giải quyết

    public int Quantity { get; set; } = 1; // Số lượng bản

    [MaxLength(50)]
    public string ReceiveMethod { get; set; } = "Digital"; // "Digital" (File mềm/Xác thực điện tử) hoặc "Office" (Nhận trực tiếp tại phòng CTSV)

    [MaxLength(1000)]
    public string? StudentNote { get; set; } // Ghi chú thêm của SV

    [MaxLength(1000)]
    public string? AdminFeedback { get; set; } // Ý kiến / lý do từ chối hoặc hướng dẫn bổ sung của Admin

    [MaxLength(255)]
    public string? ResultFileUrl { get; set; } // Đường dẫn file trả kết quả (nếu có)

    [MaxLength(1000)]
    public string? ResultNote { get; set; } // Ghi chú kết quả (VD: Đã ký số hoặc Lịch hẹn nhận bản cứng: 8h30 ngày...)

    [MaxLength(50)]
    public string? VerificationCode { get; set; } // Mã tra cứu / QR xác thực

    public DateTime? ExpectedDate { get; set; } // Ngày hẹn trả kết quả dự kiến

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    // Navigation
    public ICollection<RequestAttachment> Attachments { get; set; } = new List<RequestAttachment>();
    public ICollection<RequestLog> Logs { get; set; } = new List<RequestLog>();
}
