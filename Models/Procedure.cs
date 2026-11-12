using System.ComponentModel.DataAnnotations;

namespace StudentAdminPortal.Models;

public class Procedure
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty; // e.g. "XN-VAYVON", "CAPLAI-THE"

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    /// <summary>
    /// Số ngày làm việc dự kiến để hoàn thành
    /// </summary>
    public int ProcessingDays { get; set; } = 3;

    /// <summary>
    /// Hướng dẫn hồ sơ, giấy tờ cần chuẩn bị (gạch đầu dòng hoặc mô tả)
    /// </summary>
    public string? RequiredDocuments { get; set; }

    /// <summary>
    /// Biểu tượng Bootstrap Icon (VD: "bi-file-earmark-text", "bi-person-badge",...)
    /// </summary>
    [MaxLength(50)]
    public string? IconClass { get; set; } = "bi-file-earmark-text";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<AdministrativeRequest> Requests { get; set; } = new List<AdministrativeRequest>();
}
