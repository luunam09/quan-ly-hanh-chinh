using System.ComponentModel.DataAnnotations;
using StudentAdminPortal.Models;

namespace StudentAdminPortal.ViewModels;

public class CreateRequestViewModel
{
    [Required]
    public int ProcedureId { get; set; }

    public Procedure? Procedure { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập lý do / mục đích yêu cầu")]
    [Display(Name = "Lý do / Mục đích nộp đơn")]
    public string Purpose { get; set; } = string.Empty;

    [Range(1, 10, ErrorMessage = "Số lượng bản từ 1 đến 10")]
    [Display(Name = "Số lượng bản")]
    public int Quantity { get; set; } = 1;

    [Required]
    [Display(Name = "Hình thức nhận kết quả")]
    public string ReceiveMethod { get; set; } = "Digital"; // "Digital" hoặc "Office"

    [Display(Name = "Ghi chú thêm")]
    public string? StudentNote { get; set; }

    [Display(Name = "Tệp hồ sơ / giấy tờ đính kèm")]
    public List<IFormFile>? Attachments { get; set; }
}

public class SupplementRequestViewModel
{
    [Required]
    public int RequestId { get; set; }

    [Display(Name = "Ghi chú bổ sung")]
    public string? Note { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn ít nhất một tệp để bổ sung")]
    [Display(Name = "Tệp bổ sung")]
    public List<IFormFile> Files { get; set; } = new();
}

public class ReviewRequestViewModel
{
    public int RequestId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn trạng thái mới")]
    public string NewStatus { get; set; } = string.Empty;

    public string? AdminFeedback { get; set; }

    public string? ResultNote { get; set; }

    public DateTime? ExpectedDate { get; set; }

    public IFormFile? ResultFile { get; set; }
}

public class StudentDashboardViewModel
{
    public User Student { get; set; } = null!;
    public int TotalRequests { get; set; }
    public int PendingRequests { get; set; }
    public int ProcessingRequests { get; set; }
    public int NeedUpdateRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int RejectedRequests { get; set; }
    public List<AdministrativeRequest> RecentRequests { get; set; } = new();
    public List<Procedure> AvailableProcedures { get; set; } = new();
}

public class AdminDashboardViewModel
{
    public int TotalRequests { get; set; }
    public int PendingRequests { get; set; }
    public int ProcessingRequests { get; set; }
    public int NeedUpdateRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int RejectedRequests { get; set; }
    public int TotalStudents { get; set; }
    public int TotalProcedures { get; set; }
    public List<AdministrativeRequest> UrgentRequests { get; set; } = new();
    public Dictionary<string, int> RequestsByProcedure { get; set; } = new();
    public Dictionary<string, int> RequestsByFaculty { get; set; } = new();
}

public class RequestListFilterViewModel
{
    public string? Status { get; set; }
    public int? ProcedureId { get; set; }
    public string? Search { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public List<AdministrativeRequest> Requests { get; set; } = new();
    public List<Procedure> Procedures { get; set; } = new();
}
