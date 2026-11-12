using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAdminPortal.Models;

public class RequestLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int RequestId { get; set; }

    [ForeignKey(nameof(RequestId))]
    public AdministrativeRequest? Request { get; set; }

    [Required]
    [MaxLength(100)]
    public string ActorName { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string ActorRole { get; set; } = string.Empty; // "Student", "Admin", "System"

    [Required]
    [MaxLength(100)]
    public string Action { get; set; } = string.Empty; // "Tạo mới", "Tiếp nhận", "Yêu cầu bổ sung", "Bổ sung hồ sơ", "Phê duyệt", "Từ chối"

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
