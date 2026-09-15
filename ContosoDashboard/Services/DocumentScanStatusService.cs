namespace ContosoDashboard.Services;

using ContosoDashboard.Models;

public interface IDocumentScanStatusService
{
    Task QueueScanAsync(Document document, string storedPath);
    Task UpdateScanStatusAsync(Document document, SecurityScanStatus status, string? summary = null);
}

public class DocumentScanStatusService : IDocumentScanStatusService
{
    public Task QueueScanAsync(Document document, string storedPath)
    {
        document.Status = DocumentStatus.PendingScan;
        document.SecurityScanStatus = SecurityScanStatus.Pending;
        document.ScanRequestedAtUtc = DateTime.UtcNow;
        document.ScanCompletedAtUtc = null;
        document.ScanResultSummary = null;

        return Task.CompletedTask;
    }

    public Task UpdateScanStatusAsync(Document document, SecurityScanStatus status, string? summary = null)
    {
        document.SecurityScanStatus = status;
        document.ScanCompletedAtUtc = DateTime.UtcNow;
        document.ScanResultSummary = summary;

        document.Status = status switch
        {
            SecurityScanStatus.Clean => DocumentStatus.Approved,
            SecurityScanStatus.Infected => DocumentStatus.Blocked,
            SecurityScanStatus.Failed => DocumentStatus.Failed,
            _ => document.Status
        };

        return Task.CompletedTask;
    }
}
