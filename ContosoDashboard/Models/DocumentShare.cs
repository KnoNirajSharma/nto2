using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoDashboard.Models;

public class DocumentShare
{
    [Key]
    public int DocumentShareId { get; set; }

    [Required]
    public int DocumentId { get; set; }

    [Required]
    public int SharedByUserId { get; set; }

    [Required]
    public int SharedWithUserId { get; set; }

    public int? ProjectId { get; set; }

    [MaxLength(50)]
    public string AccessLevel { get; set; } = "View";

    public DateTime SharedDate { get; set; } = DateTime.UtcNow;

    public bool NotificationSent { get; set; } = true;

    [ForeignKey("DocumentId")]
    public virtual Document Document { get; set; } = null!;

    [ForeignKey("SharedByUserId")]
    public virtual User SharedByUser { get; set; } = null!;

    [ForeignKey("SharedWithUserId")]
    public virtual User SharedWithUser { get; set; } = null!;
}
