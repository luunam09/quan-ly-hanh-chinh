using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAdminPortal.Models;

public class RequestAttachment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int RequestId { get; set; }

    [ForeignKey(nameof(RequestId))]
    public AdministrativeRequest? Request { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? FileType { get; set; }

    public long FileSize { get; set; }

    /// <summary>
    /// Đánh dấu xem file này là nộp ban đầu hay là giấy tờ nộp bổ sung theo yêu cầu
    /// </summary>
    public bool IsSupplementary { get; set; } = false;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
