namespace ContosoDashboard.Services;

public class DocumentScanJobMessage
{
    public int DocumentId { get; set; }
    public string StoredPath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string QueueName { get; set; } = "document-scan";
}
