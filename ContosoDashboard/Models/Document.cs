using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoDashboard.Models;

public class Document
{
    [Key]
    public int DocumentId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Category { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? ContentType { get; set; }

    [Required]
    [MaxLength(2048)]
    public string StoragePath { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    [Required]
    public int UploadedByUserId { get; set; }

    public int? ProjectId { get; set; }

    [MaxLength(2000)]
    public string Tags { get; set; } = string.Empty;

    public DateTime UploadedDate { get; set; } = DateTime.UtcNow;

    public DateTime? LastModifiedDate { get; set; }

    public DocumentStatus Status { get; set; } = DocumentStatus.PendingScan;

    public SecurityScanStatus SecurityScanStatus { get; set; } = SecurityScanStatus.Pending;

    public DateTime? ScanRequestedAtUtc { get; set; }

    public DateTime? ScanCompletedAtUtc { get; set; }

    public string? ScanResultSummary { get; set; }

    [ForeignKey("UploadedByUserId")]
    public virtual User UploadedByUser { get; set; } = null!;

    [ForeignKey("ProjectId")]
    public virtual Project? Project { get; set; }

    public virtual ICollection<DocumentShare> Shares { get; set; } = new List<DocumentShare>();
    public virtual ICollection<DocumentAuditLog> AuditLogs { get; set; } = new List<DocumentAuditLog>();
}

public enum DocumentStatus
{
    PendingScan,
    Approved,
    Blocked,
    Deleted,
    Failed
}

public enum SecurityScanStatus
{
    Pending,
    Clean,
    Infected,
    Failed,
    NotRequired
}
